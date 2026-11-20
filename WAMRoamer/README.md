## WAM Roamer  
Tyring to make FSLogix RoamIdentity work. Not using batch in $CurrentYear.

### Compilation  
`"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" /platform:AnyCpu /out:C:\Programming\C#\WAMRoamer\out\release\WAMRoamer.exe /target:winexe /optimize+ C:\Programming\C#\WAMRoamer\src\WAMRoamer.cs`

### Usage  
Use the `/logon` parameter for logon and `/logoff` parameter for logoff.  
<img width="786" height="563" alt="image" src="https://github.com/user-attachments/assets/7069fa48-bfbb-427b-a701-107224c6e31a" />  

### Caveats  
If you enable the "Run logon scripts synchronously" GPO. This will delay logons until the `Microsoft.AAD.BrokerPlugin_cw5n1h2txyewy` folder has been created.

### Reference/Inspiration  
https://www.reddit.com/r/fslogix/comments/1coydnl/roam_identity_missing_something/lmesi5j/
