using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Management.Infrastructure;

namespace HomeLab
{
   public class SpaceMonApplication : ApplicationContext
   {
      private readonly Object _Lock = new Object();
      private readonly Thread thread;
      private Boolean _KeepWorking = true;
      
      public SpaceMonApplication()
      {
         Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
         InitializeComponent();
         Thread thread = new Thread(new ThreadStart(WatchDiskSpace));
         thread.Start();
      }
      
      private void InitializeComponent()
      {
      }
      
      private void OnApplicationExit(object sender, EventArgs e)
      {
         thread.Join();
      }
      
      private void CloseMenuItem_Click(object sender, EventArgs e)
      {
         _KeepWorking = false;
         //lock (_Lock) // Doesn't work. Fix me
         //{
            Application.Exit();
         //}
      }
      private void WatchDiskSpace()
      {
         string Username = Environment.UserName;
         CimSession cimSession = CimSession.Create("localhost");
         
         while (_KeepWorking == true)
         {
            lock (_Lock)
            {
               IEnumerable<CimInstance> queryInstances =
                  cimSession.QueryInstances(@"root\cimv2",
                                            "WQL",
                                            @"SELECT Label, FreeSpace, Capacity FROM Win32_Volume WHERE Label LIKE '%" + Username + "'");
                                            
               foreach (CimInstance cimInstance in queryInstances)
               {
                  String volumeLabel = Convert.ToString(cimInstance.CimInstanceProperties["Label"].Value);
                  if (volumeLabel.IndexOf(Username, StringComparison.OrdinalIgnoreCase) > 0)
                  {
                     double FreeSpace = Convert.ToDouble(cimInstance.CimInstanceProperties["FreeSpace"].Value);
                     double Capacity = Convert.ToDouble(cimInstance.CimInstanceProperties["Capacity"].Value);
                     double PercentFree =  Math.Round(FreeSpace / Capacity * 100, 0);
                     if (PercentFree < 10)
                     {
                        SendEmail(PercentFree, Capacity, FreeSpace);
                     }
                  }
                  cimInstance.Dispose();
               }
            }
            Thread.Sleep(1800000); // 30 minutes
            //Thread.Sleep(300000); // 5 minutes
         }
         cimSession.Dispose();
      }
      private void SendEmail(double PercentFree, double Capacity, double FreeSpace)
      {
         SmtpClient emailClient = new SmtpClient("mail.example.com");
         emailClient.UseDefaultCredentials = false;
         MailMessage eMail = new MailMessage();
         eMail.Subject = "Full profile alert for " + UserPrincipal.Current.GivenName;
         if (!String.IsNullOrEmpty(UserPrincipal.Current.EmailAddress))
         {
            eMail.To.Add(new MailAddress(UserPrincipal.Current.EmailAddress));
         }
         else
         {
            Application.Exit();
         }
         eMail.Bcc.Add(new MailAddress("supervisor@example.com"));
         eMail.IsBodyHtml = true;
         eMail.From = new MailAddress("helpdesk@example.com");
         eMail.Body = String.Format(@"
            <html><head></head><body>
            <p>Dear {0},</p>
            <p>Your profile is {1}% full. If you are experiencing problems, please sign out and then sign back in to clear some temporary data. If the problem continues, please contact the help desk.</p>
            <p>REF:<br>
            {2}, {3}, {4}<br>
            {5} MB used of {6} MB allotted.
            </p>
            </body></html>",
            UserPrincipal.Current.GivenName,
            100 - PercentFree,
            Environment.MachineName,
            UserPrincipal.Current.SamAccountName,
            DateTime.Now,
            (Capacity - FreeSpace).ToSize(UnitConverter.Units.MB),
            Capacity.ToSize(UnitConverter.Units.MB));
         emailClient.Send(eMail);
         eMail.Dispose();
         emailClient.Dispose();
      }
   }
   public static class UnitConverter
   {
      public enum Units
      {
         Byte, KB, MB, GB, TB, PB, EB, ZB, YB
      }
      
      public static string ToSize(this double value, Units unit)
      {
         return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0");
      }
   }
   public static class Program
   {
      [STAThread]
      static void Main()
      {
         if (IsAdministrator())
         {
            Environment.Exit(0);
         }
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new SpaceMonApplication());
      }
      static bool IsAdministrator()
      {
         using (PrincipalSearchResult<Principal> searchResult = UserPrincipal.Current.GetAuthorizationGroups())
         {
            return searchResult.Where(group => group.SamAccountName == "Administrators").Any();
         }
      }
   }
}
