using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using HomeLab.WindowsAPIs;

//http://csharptest.net/955/building-a-windows-service-part-4-extending-the-service-installer/index.html

namespace HomeLab
{
   [RunInstaller(true)]
   public partial class ProjectInstaller : System.Configuration.Install.Installer
   {
      private static readonly string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "HomeLab");
      private static readonly string installPath = Path.Combine(basePath, "GeoWatcherService");
      
      public ProjectInstaller()
      {
         InitializeComponent();
      }
      public override void Install(IDictionary stateSaver)
      {
         if (!Directory.Exists(installPath))
         {
            Directory.CreateDirectory(installPath);
         }
         
         File.Copy(this.Context.Parameters["assemblypath"], Path.Combine(installPath, "GeoWatcherService.exe"));
         
         base.Install(stateSaver);
      }
      public override void Uninstall(IDictionary savedState)
      {
         using (ServiceController service = new ServiceController("GeoWatcherService"))
         {
            if (service.Status == ServiceControllerStatus.Running)
            {
               service.Stop();
            }
         }
         
         base.Uninstall(savedState);
         
         RemoveAppDir();
      }
      public override void Commit(IDictionary savedState)
      {
         base.Commit(savedState);
         
         using (ServiceController service = new ServiceController("GeoWatcherService"))
         {
            Advapi32.ChangeServiceConfig(service, Path.Combine(installPath, "GeoWatcherService.exe"), "");
            service.Start();
         }
      }
      public override void Rollback(IDictionary savedState)
      {
         base.Rollback(savedState);
         
         RemoveAppDir();
      }
      private void RemoveAppDir()
      {
         if (Directory.Exists(installPath))
         {
            try
            {
               Directory.Delete(installPath, true);
            }
            catch (IOException ex)
            {
               Console.WriteLine(ex);
               Console.WriteLine("This is expected if you invoked uninstall against the service executable using the service executable.");
            }
         }
         
         if (Directory.Exists(basePath))
         {
            DirectoryInfo di = new DirectoryInfo(basePath);
            if (0 == di.GetFiles().Length && 0 == di.GetDirectories().Length)
            {
               di.Delete();
            }
         }
      }
   }
}
