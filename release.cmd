@ECHO OFF
SETLOCAL

REM Enable windows installer logging
REM    msiexec /i "C:\temp\installer.msi" /L*V "C:\temp\file.log"

SET BuildConfiguration=Release
SET RootFolder=%~dp0\
SET PublishFolder=%rootfolder%TabularEditor\bin\%BuildConfiguration%
SET SignTool="C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\signtool.exe"
SET DevEnv="C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe"

REM Build EXE (REM .exe is automatically signed in a post-build event when built under "Release" mode)
REM %DevEnv% TabularEditor.sln /Clean
REM %DevEnv% TabularEditor.sln /Rebuild "%BuildConfiguration%|AnyCpu" &REM /Out "%~dp0release.cmd.log"

REM Sign EXE 
REM %SignTool% sign /sha1 b2e9378dcc9030818a76527aa083f373772ae50b /tr http://timestamp.sectigo.com /td SHA256 "%PublishFolder%\TabularEditor.exe"

REM Build MSI
CD /d "%~dp0TabularEditorInstaller"
IF EXIST *.msi    DEL *.msi
IF EXIST *.wixobj DEL *.wixobj
IF EXIST *.wixpdb DEL *.wixpdb
.\wix\candle.exe Product.wxs -dPublishFolder="%PublishFolder%" -dRootFolder="%RootFolder%" -nologo
.\wix\light.exe Product.wixobj -ext WixUIExtension.dll -ext WixUtilExtension.dll -cultures:en-us -loc Product_en-us.wxl -out "TabularEditor.Installer.msi" -sice:ICE61 -nologo

REM Sign MSI
REM %SignTool% sign /sha1 b2e9378dcc9030818a76527aa083f373772ae50b /tr http://timestamp.sectigo.com /td SHA256 TabularEditor.Installer.msi

EXIT /b %ERRORLEVEL%
