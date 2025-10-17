using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using HomeLab.WindowsAPIs;

namespace HomeLab
{
   [RunInstaller(true)]
   public partial class ProjectInstaller : System.Configuration.Install.Installer
   {
      private static readonly string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\HomeLab";
      private string installPath = basePath + "\\OOMManager";
      
      public ProjectInstaller()
      {
         InitializeComponent();
      }
      public override void Install(IDictionary stateSaver)
      {
         if (!Directory.Exists(installPath))
         {
            Directory.CreateDirectory(installPath);
            
            File.Copy(this.Context.Parameters["assemblypath"], installPath + "\\OOMManager.exe");
         }
         
         base.Install(stateSaver);
      }
      public override void Uninstall(IDictionary savedState)
      {
         using (ServiceController service = new ServiceController("OOMManagerService"))
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
         
         using (ServiceController service = new ServiceController("OOMManagerService"))
         {
            Advapi32.ChangeServiceConfig(service, installPath + "\\OOMManager.exe", "");
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
            Directory.Delete(installPath, true);
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
