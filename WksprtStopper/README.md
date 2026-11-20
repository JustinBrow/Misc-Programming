## WksprtStopper  
We don't allow RDP logon using saved credentials in our organization. WksprtStopper closes `wksprt.exe` when it launches so it doesn't try to SSO additional RDP logins.
### To-do
Service install to a user writable directory and runs as SYSTEM. This introduces a security vulnerability. Fix this!
