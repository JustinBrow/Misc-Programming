using System;
using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace HomeLab
{
   internal class evtRecord
   {
      public evtRecord (string usr, string evt, string msg)
      {
         UserName = usr;
         Event = evt;
         Message = msg;
      }
      public string UserName { get; set; }
      public string Event { get; set; }
      public string Message { get; set; }
   }
   internal class myScreen
   {
      public myScreen(Screen screen)
      {
         Name = screen.DeviceName.TrimStart("\\.".ToCharArray());
         Width = screen.Bounds.Width;
         Height = screen.Bounds.Height;
         IsPrimary = screen.Primary;
      }
      public string Name { get; set;}
      public int Width { get; set;}
      public int Height { get; set;}
      public bool IsPrimary { get; set;}
   }
   public class Program
   {
      //[STAThread]
      public static void Main()
      {
         List<myScreen> screens = new List<myScreen>();
         JavaScriptSerializer serializer = new JavaScriptSerializer();
         foreach (Screen screen in Screen.AllScreens)
         {
            //MessageBox.Show(String.Format("ScreenName: {0}\r\nScreenDimensions: {1} x {2}\r\nIsPrimary: {3}",
               //screen.DeviceName, screen.Bounds.Width, screen.Bounds.Height, screen.Primary));
            screens.Add(new myScreen(screen));
         }
         //MessageBox.Show(serializer.Serialize(screens));
         //Clipboard.SetText(serializer.Serialize(screens));
         if (EventLog.SourceExists("HomeLab"))
         {
            using (EventLog eventLog = new EventLog("Application"))
            {
               evtRecord evtR = new evtRecord(Environment.UserName, "ScreenSize", serializer.Serialize(screens));
               eventLog.Source = "HomeLab";
               //MessageBox.Show(serializer.Serialize(evtR));
               //Clipboard.SetText(serializer.Serialize(evtR));
               eventLog.WriteEntry(serializer.Serialize(evtR), EventLogEntryType.Information, 1536);
            }
         }
      }
   }
}
