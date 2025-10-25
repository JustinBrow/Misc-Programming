## DeleteTool
File/Folder GPOs can't make decisions based on file properties. Thus "DeleteTool" was born.

### Usage
Delete files/folders from `AppData\Local\Temp` if they are older than 7 days and `Z:\Downloads` if they are older than 30 days.  
`C:\Program Files\HomeLab\DeleteTool.exe "%LOCALAPPDATA%\Temp",7 "Z:\Downloads",30`
