' VBScript.
Set Shell = CreateObject("WScript.Shell")
Set FSO = CreateObject("Scripting.FileSystemObject")

path = Shell.SpecialFolders("Programs") & "\Tabular Editor\Tabular Editor.lnk"
If FSO.FileExists(path) Then
	FSO.DeleteFile path
	FSO.DeleteFolder Shell.SpecialFolders("Programs") & "\Tabular Editor"
End If

path = Shell.SpecialFolders("AllUsersPrograms") & "\Tabular Editor\Tabular Editor.lnk"
If FSO.FileExists(path) Then
	FSO.DeleteFile path
	FSO.DeleteFolder Shell.SpecialFolders("AllUsersPrograms") & "\Tabular Editor"
End If