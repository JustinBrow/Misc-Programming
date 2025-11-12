## CouldIAdmin  
Using the method `WindowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator)` will tell you if the current process is running with an elevated token, but not if you *could* elevate.  
This method tells you if your user account could elevate a process.
