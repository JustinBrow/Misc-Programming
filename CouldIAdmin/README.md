## CouldIAdmin  
Using the method `WindowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator)` will tell you if the running process is currently elevated, but not if you *could* elevate.  
This method tells you if your user account could elevate a process.
