## OOMManager  
Small "service" application to notify our team when an application is potentially leaking memory (Looking at you Adobe Reader). The memory limit chosen (18 GB) is fairly arbitrary.  

### Install  
Hijacked a couple methods in the installer class so it can install/uninstall itself.  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\temp\OOMManager.exe /LogFile=  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -u C:\temp\OOMManager.exe /LogFile=
