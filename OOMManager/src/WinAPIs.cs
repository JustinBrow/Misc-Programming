using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace HomeLab.WindowsAPIs
{
   internal static class Advapi32
   {
      [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfigW", ExactSpelling = true,
         CharSet = CharSet.Unicode, SetLastError = true)]
      private static extern int ChangeServiceConfig(
         SafeHandle hService,
         int nServiceType,
         int nStartType,
         int nErrorControl,
         String lpBinaryPathName,
         String lpLoadOrderGroup,
         IntPtr lpdwTagId,
         [In] String lpDependencies,
         String lpServiceStartName,
         String lpPassword,
         String lpDisplayName);
      
      [DllImport("advapi32.dll", SetLastError = true)]
      internal static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);
      
      internal static void ChangeServiceConfig(ServiceController sc, string exePath, string arguments)
      {
         exePath = Path.GetFullPath(exePath.Trim(' ', '\'', '"'));
         string fqExec = String.Format("\"{0}\" {1}", exePath, arguments).TrimEnd();
         
         const int notChanged = -1;
         if (0 == ChangeServiceConfig(sc.ServiceHandle, notChanged, notChanged, notChanged, fqExec,
                                      null, IntPtr.Zero, null, null, null, null))
            throw new Win32Exception();
      }
   }
   
   internal static class Kernel32
   {
      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool CloseHandle(IntPtr hObject);
   }
}
