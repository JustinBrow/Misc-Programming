using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace HomeLab
{
   internal class myScreen
   {
      public myScreen(Screen yourScreen)
      {
         DeviceName = yourScreen.DeviceName;
         Width = yourScreen.Bounds.Width;
         Height = yourScreen.Bounds.Height;
         IsPrimary = yourScreen.Primary;
      }
      public string DeviceName { get; set;}
      public int Width { get; set;}
      public int Height { get; set;}
      public bool IsPrimary { get; set;}
   }
   public class Program
   {
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
         using (SqlConnection connection = new SqlConnection("Data Source=DB.Example.com;Initial Catalog=Example_Telemetry;Integrated Security=true;"))
         {
            SqlCommand command = new SqlCommand("UserMonitorsTeletry", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@UserName", Environment.UserName));
            command.Parameters.Add(new SqlParameter("@MonitorsJSON", serializer.Serialize(screens)));
            try
            {
               command.Connection.Open();
               command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
               MessageBox.Show(e.Message);
            }
         }
      }
   }
  
}
