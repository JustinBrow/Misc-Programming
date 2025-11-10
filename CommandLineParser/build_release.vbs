Option Explicit
Dim ScriptDir, ProjectName
Dim intReturn


Dim wshShell : Set wshShell = CreateObject("WScript.Shell")
Dim objFSO : Set objFSO = CreateObject("Scripting.FileSystemObject")

ScriptDir = objFSO.GetParentFolderName(WScript.ScriptFullName)
ProjectName = objFSO.GetFolder(ScriptDir).Name

If objFSO.FolderExists(ScriptDir & "\out\release\") Then
   objFSO.DeleteFolder ScriptDir & "\out\release", True
End If

If Not objFSO.FolderExists(ScriptDir & "\out\") Then
   objFSO.CreateFolder ScriptDir & "\out\"
End If

If Not objFSO.FolderExists(ScriptDir & "\out\release\") Then
   objFSO.CreateFolder ScriptDir & "\out\release\"
End If

intReturn = wshShell.Run("C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" & _
   " /platform:AnyCpu" & _
   " /out:" & ScriptDir & "\out\release\" & ProjectName & ".exe" & _
   " /target:winexe" & _
   " /optimize+" & _
   " " & ScriptDir & "\src\" & ProjectName & ".cs", 0, True)

If intReturn = 0 Then
   'wshShell.Run(ScriptDir & "\out\release\" & ProjectName & ".exe")
ElseIf intReturn <> 0 Then
   wshShell.Run("cmd.exe /k" & "C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" & _
   " /platform:AnyCpu" & _
   " /out:" & ScriptDir & "\out\release\" & ProjectName & ".exe" & _
   " /target:winexe" & _
   " /optimize+" & _
   " " & ScriptDir & "\src\" & ProjectName & ".cs")
End If
