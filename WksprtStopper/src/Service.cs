using System;
using System.IO;
using System.Threading;
using System.Management;
using System.Diagnostics;
using System.ServiceProcess;

//https://www.c-sharpcorner.com/article/create-windows-services-in-c-sharp/
namespace HomeLab
{
   public partial class WksprtStopper : ServiceBase
   {
      ManagementEventWatcher watcher = new ManagementEventWatcher();
      
      public WksprtStopper()
      {
         InitializeComponent();
      }
      
      protected override void OnStart(string[] args)
      {
         WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName ='wksprt.exe'");
         watcher.Query = query;
         watcher.EventArrived += new EventArrivedEventHandler(ProcessStartEventArrived);
         
         try
         {
            watcher.Start();
            WriteToFile(DateTime.Now + ": RemoteAppStopper Service started");
         }
         catch (Exception ex)
         {
            WriteToFile(DateTime.Now + ": " + ex.ToString());
         }
      }
      
      protected override void OnStop()
      {
         try
         {
            watcher.Stop();
            WriteToFile(DateTime.Now + ": RemoteAppStopper Service stopped");
         }
         catch (Exception ex)
         {
            WriteToFile(DateTime.Now + ": " + ex.ToString());
         }
      }
      
      protected override void OnShutdown()
      {
         try
         {
            watcher.Stop();
            WriteToFile(DateTime.Now + ": RemoteAppStopper Service stopped");
            WriteToFile(DateTime.Now + ": System shutting down");
         }
         catch (Exception ex)
         {
            WriteToFile(DateTime.Now + ": " + ex.ToString());
         }
      }
      
      internal void TestStartupAndStop(string[] args)
      {
         this.OnStart(args);
         Console.WriteLine("Press any key to continue...");
         Console.ReadKey();
         this.OnStop();
      }
      
      private void ProcessStartEventArrived(object sender, EventArrivedEventArgs e)
      {
         WriteToFile(DateTime.Now + ": Found RemoteApp and Desktop Connection Runtime");
         //Thread.Sleep(5000);
         int pid = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value);
         
         try
         {
            Process.GetProcessById(pid).Kill();
            WriteToFile(DateTime.Now + ": Stopped RemoteApp and Desktop Connection Runtime process: PID " + pid);
         }
         catch (Exception ex)
         {
            WriteToFile(DateTime.Now + ": " + ex.ToString());
         }
      }
      
      private void WriteToFile(string Message)
      {
         string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
         if (!Directory.Exists(path))
         {
            Directory.CreateDirectory(path);
         }
         
         string filepath = Path.Combine(path, "WksprtStopper.log");
         if (!File.Exists(filepath))
         {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filepath))
            {
               sw.WriteLine(Message);
            }
         }
         else
         {
            // Append to the existing file.
            using (StreamWriter sw = File.AppendText(filepath))
            {
               sw.WriteLine(Message);
            }
         }
      }
   }
}
