using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Mail;
using System.Security.Principal;
using System.ServiceProcess;
using System.Timers;
using System.Threading;
using HomeLab.WindowsAPIs;

namespace HomeLab
{
   public partial class OOMManagerService : ServiceBase
   {
      private System.Timers.Timer timer;
      
      public OOMManagerService()
      {
         InitializeComponent();
      }
      
      private static void OOMManager(object sender, ElapsedEventArgs e)
      {
         foreach (Process proc in Process.GetProcesses())
         {
            double memory = Math.Round((proc.WorkingSet64 / (1024.0*1024.0*1024.0)), 0);
            if (memory >= 18.0)
            {
               WriteToFile(DateTime.Now + String.Format(": Process {0} with Id {1} has exceeded 18 GB memory usage ({2} GB)", proc.MainModule.ModuleName, proc.Id, memory));
               SendEmail(Environment.MachineName, proc.MainModule.ModuleName, GetProcessUser(proc), proc.Id, memory);
            }
         }
      }
      
      protected override void OnStart(string[] args)
      {
         timer = new System.Timers.Timer(30000);
         timer.Elapsed += OOMManager;
         timer.AutoReset = true;
         timer.Start();
         WriteToFile(DateTime.Now + ": OOMManagerService started");
      }
      
      protected override void OnStop()
      {
         RequestAdditionalTime(5000);
         WriteToFile(DateTime.Now + ": OOMManagerService stopping");
         timer.Stop();
      }
      
      protected override void OnShutdown()
      {
         RequestAdditionalTime(5000);
         WriteToFile(DateTime.Now + ": Recieved shutdown signal");
         WriteToFile(DateTime.Now + ": OOMManagerService stopping");
         timer.Stop();
      }
      
      internal static void WriteToFile(string Message)
      {
         string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
         if (!Directory.Exists(path))
         {
            Directory.CreateDirectory(path);
         }
         
         string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\OOMManager_" + DateTime.Now.ToShortDateString().Replace('/', '_') + ".log";
         if (!File.Exists(filepath))
         {
            using (StreamWriter sw = File.CreateText(filepath))
            {
               sw.WriteLine(Message);
            }
         }
         else
         {
            using (StreamWriter sw = File.AppendText(filepath))
            {
               sw.WriteLine(Message);
            }
         }
      }
      
      private static string GetProcessUser(Process process)
      {
         string user;
         
         IntPtr processHandle = IntPtr.Zero;
         try
         {
            Advapi32.OpenProcessToken(process.Handle, 8, out processHandle);
            using (WindowsIdentity wi = new WindowsIdentity(processHandle))
            {
               user = wi.Name;
            }
            return user.Contains(@"\") ? user.Substring(user.IndexOf(@"\") + 1) : user;
         }
         catch
         {
            return null;
         }
         finally
         {
            if (processHandle != IntPtr.Zero)
            {
               Kernel32.CloseHandle(processHandle);
            }
         }
      }
      
      private static void SendEmail(string Computer, string Application, string Owner, int processID, double memory)
      {
         SmtpClient emailClient = new SmtpClient("mail.example.com");
         emailClient.UseDefaultCredentials = false;
         MailMessage eMail = new MailMessage();
         eMail.Subject = "OOMManager alert";
         eMail.To.Add(new MailAddress("helpdesk@example.com"));
         eMail.Bcc.Add(new MailAddress("supervisor@example.com"));
         eMail.IsBodyHtml = true;
         eMail.From = new MailAddress("noreply@example.com");
         eMail.Body = String.Format(@"
            <html><head></head><body>
            <p>An application is utilizing an excessive amount of system memory (>18 GB)</p>
            <p>REF:<br>
            {0}, {1}, {2}, PID {3}, {4} GB<br>
            </p>
            </body></html>",
            Computer,
            Application,
            Owner,
            processID,
            memory);
         emailClient.Send(eMail);
         eMail.Dispose();
         emailClient.Dispose();
      }
   }
}
