<#
.SYNOPSIS
    Produces a complete, signed Tabular Editor 2.x release.

.DESCRIPTION
    Runs the full release pipeline in the only order that produces correctly signed
    output:

        1. Build TabularEditor.csproj (and its project references) with MSBuild.
           This is a .NET Framework 4.8 application, so MSBuild is used rather than
           "dotnet build".
        2. Sign TOMWrapper.dll first, then TabularEditor.exe last.
        3. Build the installer (.vdproj) with devenv.com. MSBuild cannot build
           Visual Studio deployment projects.
        4. Extract the MSI payload with an administrative install and verify that
           the TabularEditor.exe inside the MSI is still signed.
        5. Zip the extracted payload into the portable archive.
        6. Sign the MSI as the final step and copy it to its published names.
        7. Write release-notes-header.md, the SHA-256 header block that goes at the top
           of the GitHub release notes. Hashes are taken from the final published bytes,
           after signing.

    ORDER IS LOAD-BEARING. TabularEditor.exe references TOMWrapper.dll. Signing
    rewrites a file and therefore updates its timestamp, so signing TOMWrapper.dll
    *after* TabularEditor.exe makes the reference newer than the output. MSBuild then
    considers the exe out of date and silently recompiles it during the installer
    build, replacing the signed exe with an unsigned one. That is what shipped in
    2.29.0. Signing TOMWrapper.dll first, and re-stamping the exe afterwards, keeps
    the exe the newest file so nothing rebuilds. Step 4 verifies it rather than
    trusting it.

    Note also that this script deliberately does not pass -coe (continue on error) to
    AzureSignTool. That flag let a failed signature pass unnoticed.

    SIGNING CONFIGURATION

    The tenant, key vault and certificate are read from the environment, so that this
    public repository carries no description of the signing infrastructure:

        TE_SIGN_TENANT_ID      Entra tenant to authenticate against
        TE_SIGN_KEYVAULT       Key vault holding the code signing certificate
        TE_SIGN_CERT_NAME      Name of the certificate in that vault
        TE_SIGN_TIMESTAMP_URL  Optional. Defaults to the GlobalSign RFC 3161 endpoint.

    Set them as user environment variables, or create release-config.ps1 next to this
    script, which is gitignored and dot-sourced automatically:

        $env:TE_SIGN_TENANT_ID = '...'
        $env:TE_SIGN_KEYVAULT  = '...'
        $env:TE_SIGN_CERT_NAME = '...'

    They are only needed when signing. A -SkipSigning build requires no configuration.

    PREREQUISITES

    AzureSignTool  dotnet tool install --global AzureSignTool
    Az.Accounts    Install-Module Az -Scope CurrentUser
    Visual Studio  with the "Microsoft Visual Studio Installer Projects" extension

.PARAMETER Configuration
    Build configuration. Defaults to Release.

.PARAMETER SkipSigning
    Runs the whole pipeline without Azure sign-in or signing. Use for test builds.
    The output is NOT publishable.

.PARAMETER SkipInstaller
    Builds and signs the binaries only. Skips the installer, zip and MSI signing.

.PARAMETER Clean
    Deletes previous build and installer output before building.

.PARAMETER SignSetupExe
    Also sign the setup.exe bootstrapper emitted next to the MSI. Off by default,
    matching historical releases, which shipped it unsigned.

.EXAMPLE
    .\build-release.ps1
    Full signed release. Prompts once for Azure sign-in.

.EXAMPLE
    .\build-release.ps1 -SkipSigning -Clean
    Full dry run with no Azure sign-in, to verify the build and packaging steps.
#>
[CmdletBinding()]
param(
    [string] $Configuration = 'Release',
    [switch] $SkipSigning,
    [switch] $SkipInstaller,
    [switch] $Clean,
    [switch] $SignSetupExe
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# --------------------------------------------------------------------------------------
# Configuration
# --------------------------------------------------------------------------------------

$RepoRoot = $PSScriptRoot

# Signing configuration comes from the environment so that no detail of the signing
# infrastructure lives in this public repository. Set these once as user-level
# environment variables, or put them in release-config.ps1 next to this script, which is
# gitignored and dot-sourced below if present:
#
#     $env:TE_SIGN_TENANT_ID = '<your Entra tenant id>'
#     $env:TE_SIGN_KEYVAULT  = '<your key vault name>'
#     $env:TE_SIGN_CERT_NAME = '<your certificate name>'
#
# None of these are secrets, but keeping them out of the repo avoids advertising the
# signing setup. They are only required when actually signing, so -SkipSigning runs need
# no configuration at all.
$LocalConfig = Join-Path $RepoRoot 'release-config.ps1'
if (Test-Path $LocalConfig) { . $LocalConfig }

$TenantId        = $env:TE_SIGN_TENANT_ID
$KeyVaultName    = $env:TE_SIGN_KEYVAULT
$CertificateName = $env:TE_SIGN_CERT_NAME
$TimestampUrl    = if ($env:TE_SIGN_TIMESTAMP_URL) { $env:TE_SIGN_TIMESTAMP_URL } else { 'http://timestamp.globalsign.com/tsa/r6advanced1' }

# Where the published artefacts are downloaded from. Used to build the download links in
# the release notes header.
$DownloadBaseUrl = 'https://cdn.tabulareditor.com/files'

$SolutionFile    = Join-Path $RepoRoot 'TabularEditor.sln'
$AppProject      = Join-Path $RepoRoot 'TabularEditor\TabularEditor.csproj'
$InstallerProject= 'TabularEditorInstaller\TabularEditorInstaller.vdproj'
$VdprojFile      = Join-Path $RepoRoot $InstallerProject

$AppBinDir       = Join-Path $RepoRoot "TabularEditor\bin\$Configuration"
$AppObjDir       = Join-Path $RepoRoot "TabularEditor\obj\$Configuration"
$TomBinDir       = Join-Path $RepoRoot "TOMWrapper\bin\$Configuration"
$InstallerOutDir = Join-Path $RepoRoot "TabularEditorInstaller\$Configuration"

# The vdproj emits this name. The published names are derived from it further down.
$RawMsiName      = 'TabularEditorInstaller.msi'

# --------------------------------------------------------------------------------------
# Helpers
# --------------------------------------------------------------------------------------

$script:StepNumber = 0

function Write-Step {
    param([string] $Message)
    $script:StepNumber++
    Write-Host ''
    Write-Host ("=" * 78) -ForegroundColor Cyan
    Write-Host (" {0}. {1}" -f $script:StepNumber, $Message) -ForegroundColor Cyan
    Write-Host ("=" * 78) -ForegroundColor Cyan
}

function Write-Info { param([string] $Message) Write-Host "    $Message" -ForegroundColor Gray }
function Write-Ok   { param([string] $Message) Write-Host "    $Message" -ForegroundColor Green }
function Write-Warn { param([string] $Message) Write-Host "    $Message" -ForegroundColor Yellow }

function Invoke-Native {
    <#
      Runs a native executable and throws if it returns a non-zero exit code.
      Native tools do not raise terminating errors, so every call is checked.
    #>
    param(
        [Parameter(Mandatory)] [string]   $FilePath,
        [Parameter(Mandatory)] [string[]] $Arguments,
        [string] $ErrorMessage = 'Command failed'
    )
    Write-Verbose "$FilePath $($Arguments -join ' ')"
    & $FilePath @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$ErrorMessage (exit code $LASTEXITCODE): $FilePath $($Arguments -join ' ')"
    }
}

function Find-VisualStudio {
    <#
      Locates a Visual Studio installation that has both MSBuild and devenv.com.
      Prefers vswhere, falls back to well-known paths.
    #>
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    $candidates = @()

    if (Test-Path $vswhere) {
        $found = & $vswhere -latest -prerelease -products * -requires Microsoft.Component.MSBuild -property installationPath 2>$null
        foreach ($path in $found) { if ($path) { $candidates += $path } }
    }

    foreach ($root in @($env:ProgramFiles, ${env:ProgramFiles(x86)})) {
        if (-not $root) { continue }
        $vsRoot = Join-Path $root 'Microsoft Visual Studio'
        if (-not (Test-Path $vsRoot)) { continue }
        foreach ($version in (Get-ChildItem $vsRoot -Directory -ErrorAction SilentlyContinue | Sort-Object Name -Descending)) {
            foreach ($edition in (Get-ChildItem $version.FullName -Directory -ErrorAction SilentlyContinue)) {
                $candidates += $edition.FullName
            }
        }
    }

    foreach ($candidate in $candidates) {
        $msbuild = Join-Path $candidate 'MSBuild\Current\Bin\MSBuild.exe'
        $devenv  = Join-Path $candidate 'Common7\IDE\devenv.com'
        if ((Test-Path $msbuild) -and (Test-Path $devenv)) {
            return [PSCustomObject]@{
                Root         = $candidate
                MSBuild      = $msbuild
                Devenv       = $devenv
                MajorVersion = (Get-Item $devenv).VersionInfo.FileMajorPart
                DisableOopb  = Join-Path $candidate 'Common7\IDE\CommonExtensions\Microsoft\VSI\DisableOutOfProcBuild\DisableOutOfProcBuild.exe'
            }
        }
    }

    throw 'Could not locate a Visual Studio installation containing both MSBuild.exe and devenv.com.'
}

function Enable-VdprojCommandLineBuild {
    <#
      Visual Studio deployment projects cannot be built out of process. Without this
      setting, devenv fails pre-build validation from the command line with:

          ERROR: An error occurred while validating.  HRESULT = '8000000A'

      even though the identical build succeeds inside the IDE. Microsoft ships
      DisableOutOfProcBuild.exe with the Installer Projects extension to set it. The
      change is a single per-user DWORD under HKCU and is idempotent, so it is applied
      automatically here rather than being a manual prerequisite.
    #>
    param([Parameter(Mandatory)] $VisualStudio)

    $pattern = "$($VisualStudio.MajorVersion).0_*_Config"
    $configured = Get-ChildItem 'HKCU:\SOFTWARE\Microsoft\VisualStudio' -ErrorAction SilentlyContinue |
        Where-Object { $_.PSChildName -like $pattern } |
        ForEach-Object { Get-ItemProperty "$($_.PSPath)\MSBuild" -Name 'EnableOutOfProcBuild' -ErrorAction SilentlyContinue } |
        Where-Object { $_.EnableOutOfProcBuild -eq 0 }

    if ($configured) {
        Write-Info 'Out-of-process build already disabled for deployment projects.'
        return
    }

    if (-not (Test-Path $VisualStudio.DisableOopb)) {
        Write-Warn 'DisableOutOfProcBuild.exe not found. If the installer build fails validation with HRESULT 8000000A, install the "Microsoft Visual Studio Installer Projects" extension.'
        return
    }

    Write-Warn 'Disabling out-of-process build so devenv can build the .vdproj (one-time, per user).'
    Push-Location (Split-Path $VisualStudio.DisableOopb -Parent)
    try {
        Invoke-Native -FilePath $VisualStudio.DisableOopb -Arguments @() -ErrorMessage 'DisableOutOfProcBuild failed'
    }
    finally { Pop-Location }
}

function Get-ProductVersion {
    <#
      Reads ProductVersion from the .vdproj. This is the value that names the
      published MSI and zip, so it is the single source of truth for the release
      number rather than the assembly version.
    #>
    param([Parameter(Mandatory)] [string] $VdprojPath)

    $line = Select-String -Path $VdprojPath -Pattern '"ProductVersion"\s*=\s*"8:([^"]+)"' | Select-Object -First 1
    if (-not $line) { throw "Could not read ProductVersion from $VdprojPath" }
    return $line.Matches[0].Groups[1].Value
}

function Connect-SigningAccount {
    if ($SkipSigning) { return }

    if (-not (Get-Module -ListAvailable -Name Az.Accounts)) {
        throw 'The Az.Accounts PowerShell module is required for signing. Install it with: Install-Module Az -Scope CurrentUser'
    }
    Import-Module Az.Accounts -ErrorAction Stop | Out-Null

    $context = Get-AzContext -ErrorAction SilentlyContinue
    if (-not $context -or $context.Tenant.Id -ne $TenantId) {
        Write-Info 'Signing in to Azure...'
        Connect-AzAccount -TenantId $TenantId | Out-Null
    } else {
        Write-Info "Reusing existing Azure session for $($context.Account.Id)"
    }
}

function Get-SigningToken {
    <#
      Tokens are short lived, so one is fetched immediately before each signing call
      rather than once at the start of the build.
    #>
    $token = Get-AzAccessToken -ResourceUrl 'https://vault.azure.net'
    return [System.Net.NetworkCredential]::new('', $token.Token).Password
}

function Invoke-CodeSign {
    <#
      Signs the given files, in the order given, with one AzureSignTool invocation.
      -mdop 1 keeps it sequential so the resulting timestamps follow the list order.
      -coe is deliberately NOT passed: a failed signature must fail the build.
    #>
    param(
        [Parameter(Mandatory)] [string[]] $Files,
        [Parameter(Mandatory)] [string]   $Description
    )

    $existing = @($Files | Where-Object { Test-Path $_ })
    if ($existing.Count -eq 0) { throw "Nothing to sign for '$Description'. None of the expected files exist." }

    foreach ($file in $Files) {
        if ($file -notin $existing) { Write-Warn "Not found, skipping: $file" }
    }

    if ($SkipSigning) {
        Write-Warn "-SkipSigning: would have signed $($existing.Count) file(s) for '$Description'"
        foreach ($file in $existing) { Write-Warn "    $($file.Replace($RepoRoot + '\', ''))" }
        return
    }

    $listFile = Join-Path ([System.IO.Path]::GetTempPath()) ("tesign-" + [guid]::NewGuid().ToString('N') + '.txt')
    try {
        Set-Content -Path $listFile -Value ($existing -join "`r`n") -Encoding ASCII

        $accessToken = Get-SigningToken
        $arguments = @(
            'sign',
            '-kvu', "https://$KeyVaultName.vault.azure.net",
            '-kvc', $CertificateName,
            '-kva', $accessToken,
            '-tr',  $TimestampUrl,
            '-td',  'sha256',
            '-v',
            '-mdop', '1',
            '-ifl', $listFile
        )

        Invoke-Native -FilePath 'AzureSignTool' -Arguments $arguments -ErrorMessage "Signing failed for '$Description'"
    }
    finally {
        Remove-Item $listFile -Force -ErrorAction SilentlyContinue
    }

    foreach ($file in $existing) {
        Assert-Signed -Path $file
    }
    Write-Ok "Signed $($existing.Count) file(s): $Description"
}

function Assert-Signed {
    param([Parameter(Mandatory)] [string] $Path)
    $signature = Get-AuthenticodeSignature -FilePath $Path
    if ($signature.Status -ne 'Valid') {
        throw "Expected a valid signature on $Path but found status '$($signature.Status)'."
    }
}

function Get-SignatureStatus {
    param([Parameter(Mandatory)] [string] $Path)
    if (-not (Test-Path $Path)) { return 'Missing' }
    return (Get-AuthenticodeSignature -FilePath $Path).Status.ToString()
}

# --------------------------------------------------------------------------------------
# Preflight
# --------------------------------------------------------------------------------------

Write-Step 'Preflight'

$vs = Find-VisualStudio
Write-Info "Visual Studio:  $($vs.Root)"
Write-Info "MSBuild:        $($vs.MSBuild)"
Write-Info "devenv:         $($vs.Devenv)"

foreach ($required in @($SolutionFile, $AppProject, $VdprojFile)) {
    if (-not (Test-Path $required)) { throw "Required file not found: $required" }
}

if (-not (Test-Path (Join-Path $RepoRoot 'packages'))) {
    Write-Warn 'The packages folder is missing. This solution uses packages.config, so restore it in Visual Studio before building.'
}

if (-not $SkipSigning) {
    if (-not (Get-Command AzureSignTool -ErrorAction SilentlyContinue)) {
        throw 'AzureSignTool was not found on PATH. Install it with: dotnet tool install --global AzureSignTool'
    }
    Write-Info "AzureSignTool:  $((Get-Command AzureSignTool).Source)"

    # Fail here rather than three minutes into a build that cannot be signed.
    $missing = @()
    if (-not $TenantId)        { $missing += 'TE_SIGN_TENANT_ID' }
    if (-not $KeyVaultName)    { $missing += 'TE_SIGN_KEYVAULT' }
    if (-not $CertificateName) { $missing += 'TE_SIGN_CERT_NAME' }
    if ($missing.Count -gt 0) {
        throw ("Signing configuration missing: {0}. Set these environment variables, or create release-config.ps1 next to this script (see the comments at the top of it). Use -SkipSigning to build without signing." -f ($missing -join ', '))
    }
    Write-Info "Key vault:      $KeyVaultName / $CertificateName"
} else {
    Write-Warn 'Running with -SkipSigning. The output of this build is NOT publishable.'
}

$version = Get-ProductVersion -VdprojPath $VdprojFile
Write-Info "Product version: $version  (from $InstallerProject)"

$PublishedMsiName    = "TabularEditor.Installer.msi"
$VersionedMsiName    = "TabularEditor.$version.Installer.msi"
$VersionedZipName    = "TabularEditor.$version.zip"
$PortableZipName     = "TabularEditor.Portable.zip"

if ($Clean) {
    Write-Info 'Cleaning previous output...'
    foreach ($dir in @($AppBinDir, $AppObjDir, $TomBinDir, $InstallerOutDir)) {
        if (Test-Path $dir) { Remove-Item $dir -Recurse -Force }
    }
    Write-Ok 'Clean complete.'
}

# devenv empties the installer output folder as part of building the .vdproj, which
# destroys the MSI and zips of any release still sitting there. Move them aside first,
# so a rebuild can never silently delete artefacts that were already published.
$previousArtifacts = @()
if (Test-Path $InstallerOutDir) {
    $previousArtifacts = @(Get-ChildItem $InstallerOutDir -File |
        Where-Object { $_.Extension -in @('.msi', '.zip') -and $_.Name -ne $RawMsiName })
}
if ($previousArtifacts.Count -gt 0) {
    $archiveDir = Join-Path $RepoRoot ('TabularEditorInstaller\_previous\' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
    New-Item -ItemType Directory -Path $archiveDir -Force | Out-Null
    foreach ($artifact in $previousArtifacts) { Move-Item $artifact.FullName $archiveDir -Force }
    Write-Warn "Moved $($previousArtifacts.Count) existing artefact(s) to $($archiveDir.Replace($RepoRoot + '\', ''))"
}

Connect-SigningAccount

# --------------------------------------------------------------------------------------
# 1. Build the application
# --------------------------------------------------------------------------------------

Write-Step 'Build application (MSBuild, .NET Framework 4.8)'

# Building the app project rather than the solution, because the solution also contains
# the .vdproj deployment project, which MSBuild cannot build. Project references pull in
# TOMWrapper and AntlrGrammars.
Invoke-Native -FilePath $vs.MSBuild -Arguments @(
    $AppProject,
    '-t:Build',
    "-p:Configuration=$Configuration",
    '-m',
    '-v:minimal',
    '-nologo'
) -ErrorMessage 'Application build failed'

$appExe = Join-Path $AppBinDir 'TabularEditor.exe'
if (-not (Test-Path $appExe)) { throw "Build reported success but $appExe was not produced." }

$fileVersion = (Get-Item $appExe).VersionInfo.FileVersion
Write-Ok "Built TabularEditor.exe  (file version $fileVersion)"

# --------------------------------------------------------------------------------------
# 2. Sign the binaries
# --------------------------------------------------------------------------------------

Write-Step 'Sign binaries'

# ORDER MATTERS. TOMWrapper.dll is a reference of TabularEditor.exe, so it must be signed
# first. If it were signed last it would be newer than the exe, and the installer build
# would treat the exe as out of date and recompile it, discarding the signature.
$tomWrapperCopies = @(
    (Join-Path $TomBinDir 'TOMWrapper.dll'),
    (Join-Path $AppBinDir 'TOMWrapper.dll')
)
$appExeCopies = @(
    (Join-Path $AppObjDir 'TabularEditor.exe'),
    (Join-Path $AppBinDir 'TabularEditor.exe')
)

Invoke-CodeSign -Files $tomWrapperCopies -Description 'TOMWrapper.dll'
Invoke-CodeSign -Files $appExeCopies     -Description 'TabularEditor.exe'

# Belt and braces: make the exe unambiguously the newest build artefact, so no
# incremental check anywhere can decide it needs rebuilding.
$stamp = Get-Date
foreach ($copy in $appExeCopies) {
    if (Test-Path $copy) { (Get-Item $copy).LastWriteTime = $stamp }
}
Write-Info 'Re-stamped TabularEditor.exe so it is newer than all of its references.'

if ($SkipInstaller) {
    Write-Step 'Done (installer skipped)'
    Write-Warn '-SkipInstaller was specified. No MSI or zip was produced.'
    return
}

# --------------------------------------------------------------------------------------
# 3. Build the installer
# --------------------------------------------------------------------------------------

Write-Step 'Build installer (devenv, .vdproj)'

Enable-VdprojCommandLineBuild -VisualStudio $vs

# MSBuild cannot build Visual Studio deployment projects, so devenv.com is used. It also
# rebuilds project dependencies if they look out of date, which is precisely why the
# signing order above matters.
# The log cannot live in the installer output folder: devenv clears that folder as part
# of the build and warns that it cannot remove the file it is currently writing to.
$buildLog = Join-Path ([System.IO.Path]::GetTempPath()) ("te-installer-build-" + [guid]::NewGuid().ToString('N') + '.log')

Write-Info 'This can take a minute...'
$devenvFailed = $null
try {
    Invoke-Native -FilePath $vs.Devenv -Arguments @(
        $SolutionFile,
        '/Build', $Configuration,
        '/Project', $InstallerProject,
        '/Out', $buildLog
    ) -ErrorMessage 'Installer build failed'
}
catch { $devenvFailed = $_ }

if (Test-Path $buildLog) {
    # devenv does not reliably write to the console when redirected, so the log is the
    # only place the real diagnosis lives. Surface it rather than just an exit code.
    $logLines = Get-Content $buildLog
    foreach ($line in ($logLines | Where-Object { $_ -match 'ERROR|error |========== Build' })) {
        Write-Info $line.Trim()
    }
    # "up-to-date" here means devenv did not recompile the app, which is what keeps the
    # signature on TabularEditor.exe intact.
    $summaryLine = $logLines | Where-Object { $_ -match '^={2,}\s*Build:' } | Select-Object -First 1
    if ($summaryLine) { Write-Info $summaryLine.Trim() }
}

if ($devenvFailed) {
    Write-Warn "Full build log: $buildLog"
    if (Select-String -Path $buildLog -Pattern '8000000A' -Quiet -ErrorAction SilentlyContinue) {
        Write-Warn 'HRESULT 8000000A means out-of-process build is still enabled. Run DisableOutOfProcBuild.exe from the VSI folder of your Visual Studio install.'
    }
    throw $devenvFailed
}

$rawMsi = Join-Path $InstallerOutDir $RawMsiName
if (-not (Test-Path $rawMsi)) { throw "Installer build reported success but $rawMsi was not produced." }

# Keep the log next to the output now that devenv has finished with the folder.
Copy-Item $buildLog (Join-Path $InstallerOutDir 'installer-build.log') -Force
Remove-Item $buildLog -Force -ErrorAction SilentlyContinue

Write-Ok "Built $RawMsiName"

# --------------------------------------------------------------------------------------
# 4. Extract the MSI payload and verify the signature survived
# --------------------------------------------------------------------------------------

Write-Step 'Extract MSI payload and verify signatures'

$payloadDir = Join-Path $InstallerOutDir '_payload'
if (Test-Path $payloadDir) { Remove-Item $payloadDir -Recurse -Force }
New-Item -ItemType Directory -Path $payloadDir | Out-Null

# An administrative install unpacks the MSI without installing it, giving the exact file
# set a user receives. This replaces the old manual "install, then zip Program Files".
$msiexec = Start-Process -FilePath 'msiexec.exe' `
    -ArgumentList @('/a', "`"$rawMsi`"", '/qn', "TARGETDIR=`"$payloadDir`"") `
    -Wait -PassThru
if ($msiexec.ExitCode -ne 0) {
    throw "Administrative install of $RawMsiName failed with exit code $($msiexec.ExitCode)."
}

$payloadExe = Join-Path $payloadDir 'TabularEditor.exe'
if (-not (Test-Path $payloadExe)) { throw "The extracted MSI payload does not contain TabularEditor.exe." }

if (-not $SkipSigning) {
    # The gate that 2.29.0 lacked. If the installer build quietly recompiled the exe,
    # the signature is gone and the release stops here instead of shipping.
    Assert-Signed -Path $payloadExe
    Assert-Signed -Path (Join-Path $payloadDir 'TOMWrapper.dll')
    Write-Ok 'TabularEditor.exe and TOMWrapper.dll inside the MSI are signed.'
} else {
    Write-Warn "Signature verification skipped. Payload exe status: $(Get-SignatureStatus -Path $payloadExe)"
}

# --------------------------------------------------------------------------------------
# 5. Build the portable zip
# --------------------------------------------------------------------------------------

Write-Step 'Build portable zip'

# The payload folder also holds a copy of the MSI and the Power BI external tools JSON,
# which lands outside the program folder. Neither belongs in the portable archive.
$stagingDir = Join-Path $InstallerOutDir '_zipstaging'
if (Test-Path $stagingDir) { Remove-Item $stagingDir -Recurse -Force }
New-Item -ItemType Directory -Path $stagingDir | Out-Null

Get-ChildItem $payloadDir -Force |
    Where-Object { $_.Name -ne 'Common Files Folder' -and $_.Extension -ne '.msi' } |
    ForEach-Object { Copy-Item $_.FullName -Destination $stagingDir -Recurse -Force }

Add-Type -AssemblyName System.IO.Compression.FileSystem

if ($SkipSigning) {
    # A dry run must not clobber the published archives of a release that may already
    # be out in the wild, so it writes clearly marked names instead.
    $VersionedZipName = "TabularEditor.$version.UNSIGNED.zip"
    $PortableZipName  = "TabularEditor.Portable.UNSIGNED.zip"
    Write-Warn "-SkipSigning: writing $VersionedZipName instead of the published names."
}

$versionedZip = Join-Path $InstallerOutDir $VersionedZipName
$portableZip  = Join-Path $InstallerOutDir $PortableZipName
foreach ($zip in @($versionedZip, $portableZip)) {
    if (Test-Path $zip) { Remove-Item $zip -Force }
}

[System.IO.Compression.ZipFile]::CreateFromDirectory(
    $stagingDir, $versionedZip, [System.IO.Compression.CompressionLevel]::Optimal, $false)
Copy-Item $versionedZip $portableZip -Force

$zipFileCount = (Get-ChildItem $stagingDir -Recurse -File).Count
Write-Ok "Created $VersionedZipName and $PortableZipName ($zipFileCount files)"

Remove-Item $stagingDir -Recurse -Force
Remove-Item $payloadDir -Recurse -Force

# --------------------------------------------------------------------------------------
# 6. Sign the MSI, last
# --------------------------------------------------------------------------------------

Write-Step 'Sign installer'

$filesToSign = @($rawMsi)
if ($SignSetupExe) {
    $setupExe = Join-Path $InstallerOutDir 'setup.exe'
    if (Test-Path $setupExe) { $filesToSign += $setupExe }
    else { Write-Warn 'setup.exe was not produced, nothing to sign.' }
}

Invoke-CodeSign -Files $filesToSign -Description 'Installer'

$publishedMsi = Join-Path $InstallerOutDir $PublishedMsiName
$versionedMsi = Join-Path $InstallerOutDir $VersionedMsiName

if ($SkipSigning) {
    # Never let a dry run overwrite the published, signed MSI of a release that may
    # already be out in the wild.
    Write-Warn "-SkipSigning: leaving $PublishedMsiName and $VersionedMsiName untouched."
} else {
    Copy-Item $rawMsi $publishedMsi -Force
    Copy-Item $rawMsi $versionedMsi -Force
    Write-Ok "Published as $PublishedMsiName and $VersionedMsiName"
}

# --------------------------------------------------------------------------------------
# 8. Release notes header
# --------------------------------------------------------------------------------------

Write-Step 'Write release notes header'

# Hashes must come from the final published bytes. The MSI is hashed after signing,
# because signing rewrites the file, and the zip after the binaries inside it were
# signed. Running this step last is what makes the hashes correct.
if ($SkipSigning) {
    $headerMsi  = $rawMsi
    $headerFile = Join-Path $InstallerOutDir 'release-notes-header.UNSIGNED.md'
} else {
    $headerMsi  = $versionedMsi
    $headerFile = Join-Path $InstallerOutDir 'release-notes-header.md'
}

$msiHash = (Get-FileHash $headerMsi      -Algorithm SHA256).Hash
$zipHash = (Get-FileHash $versionedZip   -Algorithm SHA256).Hash

$lines = @()
if ($SkipSigning) {
    $lines += '> [!WARNING]'
    $lines += '> Produced by a -SkipSigning dry run. These are NOT the published hashes. Do not paste this into a release.'
    $lines += ''
}
$lines += "- Windows installer: [$VersionedMsiName]($DownloadBaseUrl/$VersionedMsiName)"
$lines += "  - SHA256: ``$msiHash``"
$lines += "- Portable version: [$VersionedZipName]($DownloadBaseUrl/$VersionedZipName)"
$lines += "  - SHA256: ``$zipHash``"

# GitHub release bodies use LF, and no BOM, so write the file that way.
$content = ($lines -join "`n") + "`n"
[System.IO.File]::WriteAllText($headerFile, $content, (New-Object System.Text.UTF8Encoding($false)))

Write-Ok "Wrote $(Split-Path $headerFile -Leaf)"
Write-Host ''
foreach ($line in $lines) { Write-Host "    $line" }

# --------------------------------------------------------------------------------------
# Summary
# --------------------------------------------------------------------------------------

Write-Step 'Release summary'

$artifacts = @(
    $rawMsi,
    $versionedMsi,
    $publishedMsi,
    (Join-Path $InstallerOutDir 'setup.exe'),
    $versionedZip,
    $portableZip,
    $headerFile
)

$summary = foreach ($artifact in $artifacts) {
    $exists = Test-Path $artifact
    [PSCustomObject]@{
        Artifact  = Split-Path $artifact -Leaf
        Signature = if ($artifact -match '\.(zip|md)$') { 'n/a' } else { Get-SignatureStatus -Path $artifact }
        SizeMB    = if ($exists) { [math]::Round((Get-Item $artifact).Length / 1MB, 2) } else { 0 }
    }
}
$summary | Format-Table -AutoSize | Out-String -Width 120 | Write-Host

Write-Host "    Version:  $version"
Write-Host "    Output:   $InstallerOutDir"

if ($SkipSigning) {
    Write-Warn 'This build was produced with -SkipSigning and must not be published.'
} else {
    Write-Ok 'Release build complete and verified.'
}
