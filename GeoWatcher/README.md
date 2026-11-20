## Geowatcher
Query the Windows Geoloacation API to track device locations.

### To-do
Service install to a user writable directory and runs as SYSTEM. This introduces a security vulnerability. Fix this!
Query and report nearby Wi-Fi SSIDs and signal strength

### Install  
Hijacked a couple methods in the installer class so it can install/uninstall itself.  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\temp\GeoWatcher.exe /LogFile=  
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe -u C:\temp\GeoWatcher.exe /LogFile=

### Reference/Inspiration  
https://learn.microsoft.com/en-us/dotnet/api/system.device.location.geocoordinatewatcher
https://github.com/ABoredAnalyst/GeoLocator
