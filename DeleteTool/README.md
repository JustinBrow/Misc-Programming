## DeleteTool
File/Folder GPOs and FSLogix redirections can't make decisions based on file properties. Thus "DeleteTool" was born.

### Usage V1
Delete files/folders from `AppData\Local\Temp` and `Z:\Downloads` if they are older than 7 days.  
`C:\Program Files\HomeLab\DeleteTool.exe "%LOCALAPPDATA%\Temp" "Z:\Downloads"`

### Usage V2
Delete files/folders from `AppData\Local\Temp` if they are older than 7 days and `Z:\Downloads` if they are older than 30 days.  
`C:\Program Files\HomeLab\DeleteTool.exe "%LOCALAPPDATA%\Temp",7 "Z:\Downloads",30`

### Usage V3
Config is now a JSON file. Store the JSON file with the application executable.  
`C:\Program Files\HomeLab\DeleteTool.exe`
