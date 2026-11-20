## WksprtStopper  
We don't allow RDP logon using saved credentials in our organization. WksprtStopper closes `wksprt.exe` when it launches so it doesn't try to SSO additional RDP logins.

### To-do
Service install to a user writable directory and runs as SYSTEM. This introduces a security vulnerability. Fix this!

### Install  
Hijacked a couple methods in the installer class so it can install/uninstall itself.  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\temp\WksprtStopper.exe /LogFile=  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -u C:\temp\WksprtStopper.exe /LogFile=
