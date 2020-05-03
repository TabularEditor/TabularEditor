REM .exe is automatically signed in a post-build event when built under "Release" mode
REM However, .msi file still needs to be signed.
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\signtool.exe" sign /sha1 30A970D493EF44A01D9A18FDDE948D69DCF30DD4 /tr http://timestamp.globalsign.com/?signature=sha2 /td SHA256 TabularEditorInstaller\Release\TabularEditorInstaller.msi