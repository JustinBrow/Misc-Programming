namespace HomeLab
{
   partial class OOMManagerService
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
         this.CanShutdown = true;
         this.ServiceName = "OOMManagerService";
      }
   }
}
