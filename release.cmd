REM .exe is automatically signed in a post-build event when built under "Release" mode
REM However, .msi file still needs to be signed.
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x86\signtool.exe" sign /sha1 b2e9378dcc9030818a76527aa083f373772ae50b /tr http://timestamp.sectigo.com /td SHA256 /fd certHash TabularEditorInstaller\Release\TabularEditorInstaller.msi