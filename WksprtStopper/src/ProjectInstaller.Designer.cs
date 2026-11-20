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
         
         this.serviceInstaller.Description = "Closes wksprt.exe a.k.a. RemoteApp and Desktop Connection Runtime";
         this.serviceInstaller.DisplayName = "WksprtStopper";
         this.serviceInstaller.ServiceName = "WksprtStopper";
         this.serviceInstaller.StartType = ServiceStartMode.Automatic;
         //this.serviceInstaller.DelayedAutoStart = true;
         
         this.Installers.AddRange(new Installer[] {
         this.serviceProcessInstaller,
         this.serviceInstaller});
      }
      
      private ServiceProcessInstaller serviceProcessInstaller;
      private ServiceInstaller serviceInstaller;
   }
}
