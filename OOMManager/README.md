## OOMManager  
Small "service" application to notify our team when an application is potentially leaking memory (Looking at you Adobe Reader). The memory limit chosen (18 GB) is fairly arbitrary. Couldn't figure out how to run the application logic in a separate thread so it just runs on a timer.  

### To-do  
Kill the offending process and email the user/help desk.

### Install  
Hijacked a couple methods in the installer class so it can install/uninstall itself.  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\temp\OOMManager.exe /LogFile=  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -u C:\temp\OOMManager.exe /LogFile=
