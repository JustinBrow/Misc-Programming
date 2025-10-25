Option Explicit
Dim ScriptDir, ProjectName
Dim intReturn


Dim wshShell : Set wshShell = CreateObject("WScript.Shell")
Dim objFSO : Set objFSO = CreateObject("Scripting.FileSystemObject")

ScriptDir = objFSO.GetParentFolderName(WScript.ScriptFullName)
ProjectName = objFSO.GetFolder(ScriptDir).Name

If objFSO.FolderExists(ScriptDir & "\out\debug\") Then
   objFSO.DeleteFolder ScriptDir & "\out\debug", True
End If

If Not objFSO.FolderExists(ScriptDir & "\out\") Then
   objFSO.CreateFolder ScriptDir & "\out\"
End If

If Not objFSO.FolderExists(ScriptDir & "\out\debug\") Then
   objFSO.CreateFolder ScriptDir & "\out\debug\"
End If

intReturn = wshShell.Run("C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" & _
   " /platform:AnyCpu" & _
   " /out:" & ScriptDir & "\out\debug\" & ProjectName & ".exe" & _
   " /target:winexe" & _
   " /define:DEBUG" & _
   " /debug:full" & _
   " /lib:C:\Windows\Microsoft.NET\assembly\GAC_MSIL\" & _
   " /reference:.\System.DirectoryServices.AccountManagement\v4.0_4.0.0.0__b77a5c561934e089\System.DirectoryServices.AccountManagement.dll" & _
   " " & ScriptDir & "\src\" & ProjectName & ".cs", 0, True)

If intReturn = 0 Then
   wshShell.Run(ScriptDir & "\out\debug\" & ProjectName & ".exe")
ElseIf intReturn <> 0 Then
   wshShell.Run("cmd.exe /k" & "C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" & _
   " /platform:AnyCpu" & _
   " /out:" & ScriptDir & "\out\debug\" & ProjectName & ".exe" & _
   " /target:winexe" & _
   " /define:DEBUG" & _
   " /debug:full" & _
   " /lib:C:\Windows\Microsoft.NET\assembly\GAC_MSIL\" & _
   " /reference:.\System.DirectoryServices.AccountManagement\v4.0_4.0.0.0__b77a5c561934e089\System.DirectoryServices.AccountManagement.dll" & _
   " " & ScriptDir & "\src\" & ProjectName & ".cs")
End If
