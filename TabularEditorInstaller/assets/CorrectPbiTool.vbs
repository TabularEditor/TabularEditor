' VBScript.
Dim targetDir, jsonFileDir
targetDir = Session.Property("CustomActionData")

Set Shell = CreateObject("WScript.Shell")
jsonFile = Shell.ExpandEnvironmentStrings("%CommonProgramFiles%\microsoft shared\Power BI Desktop\External Tools\TabularEditor.pbitool.json")

Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objFile = objFSO.OpenTextFile(jsonFile, 1)
strText = objFile.ReadAll
objFile.Close

strTargetDir = Replace(targetDir, "\", "\\") & "TabularEditor.exe"
strNewText = Replace(strText, "<TABULAR_EDITOR_PATH>", strTargetDir)
Set objFile = objFSO.OpenTextFile(jsonFile, 2)
objFile.WriteLine strNewText
objFile.Close
