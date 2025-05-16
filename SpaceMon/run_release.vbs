Option Explicit
Dim ScriptDir, ProjectName, Compiled
Dim intReturn


Dim wshShell : Set wshShell = CreateObject("WScript.Shell")
Dim objFSO : Set objFSO = CreateObject("Scripting.FileSystemObject")

ScriptDir = objFSO.GetParentFolderName(WScript.ScriptFullName)
ProjectName = objFSO.GetFolder(ScriptDir).Name

If objFSO.FileExists(ScriptDir & "\out\release\" & ProjectName & ".exe") Then
   wshShell.Run(ScriptDir & "\out\release\" & ProjectName & ".exe")
End If
