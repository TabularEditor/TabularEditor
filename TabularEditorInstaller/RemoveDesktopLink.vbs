' VBScript.
Set Shell = CreateObject("WScript.Shell")
Set FSO = CreateObject("Scripting.FileSystemObject")

path = Shell.SpecialFolders("Desktop") & "\Tabular Editor.lnk"
If FSO.FileExists(path) Then
	FSO.DeleteFile path
End If

path = Shell.SpecialFolders("AllUsersDesktop") & "\Tabular Editor.lnk"
If FSO.FileExists(path) Then
	FSO.DeleteFile path
End If