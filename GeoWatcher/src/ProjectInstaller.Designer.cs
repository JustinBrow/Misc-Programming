using System;
using System.ServiceProcess;
using System.Configuration.Install;

namespace HomeLab
{
   partial class ProjectInstaller
   {
      private System.ComponentModel.IContainer components = null;
      
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }
      
      private void InitializeComponent()
      {
         this.serviceProcessInstaller = new ServiceProcessInstaller();
         this.serviceInstaller = new ServiceInstaller();
         
         this.serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
         this.serviceProcessInstaller.Password = null;
         this.serviceProcessInstaller.Username = null;
         
         this.serviceInstaller.Description = "GeoWatcherService";
         this.serviceInstaller.DisplayName = "GeoWatcherService";
         this.serviceInstaller.ServiceName = "GeoWatcherService";
         this.serviceInstaller.StartType = ServiceStartMode.Automatic;
         
         this.Installers.AddRange(new Installer[] {
         this.serviceProcessInstaller,
         this.serviceInstaller});
      }
      
      private ServiceProcessInstaller serviceProcessInstaller;
      private ServiceInstaller serviceInstaller;
   }
}
