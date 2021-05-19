@ECHO OFF
SETLOCAL

REM .exe is automatically signed in a post-build event when built under "Release" mode
REM However, .msi file still needs to be signed.
REM "C:\Program Files (x86)\Windows Kits\10\bin\10.0.18362.0\x86\signtool.exe" sign /sha1 b2e9378dcc9030818a76527aa083f373772ae50b /tr http://timestamp.sectigo.com /td SHA256 TabularEditorInstaller\Release\TabularEditorInstaller.msi

REM Enable windows installer logging
REM    msiexec /i "C:\temp\installer.msi" /L*V "C:\temp\file.log"

SET productversion="2.16.1"
SET rootfolder=%~dp0\
SET publishfolder=%rootfolder%\TabularEditor\bin\Debug

CD /d "%~dp0TabularEditorInstaller"
IF EXIST *.msi    DEL *.msi
IF EXIST *.wixobj DEL *.wixobj
IF EXIST *.wixpdb DEL *.wixpdb
.\wix\candle.exe Product.wxs -dproductVersion="%productversion%" -dPublishFolder="%publishfolder%" -dRootFolder="%rootfolder%" -nologo
.\wix\light.exe Product.wixobj -ext WixUIExtension.dll -ext WixUtilExtension.dll -cultures:en-us -loc Product_en-us.wxl -out "TabularEditor.Installer.msi" -sice:ICE61 -nologo

EXIT /b %ERRORLEVEL%
