using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace HomeLab
{
   public static class Program
   {
      public static string AddQuotesIfRequired(string path)
      {
         return !string.IsNullOrWhiteSpace(path) ? 
            path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ? 
               "\"" + path + "\"" : path : 
               string.Empty;
      }
      public static void Main()
      {
         string filename;
         string[] Resources, core, umbrella;
         Assembly Application = Assembly.GetExecutingAssembly();
         Resources = Application.GetManifestResourceNames();
         //MessageBox.Show(String.Join("\r\n", Resources));
         foreach (string Resource in Resources)
         {
            using (Stream stream = Application.GetManifestResourceStream(Resource))
            {
               filename = Path.Combine(Path.GetTempPath(), Resource);
               //MessageBox.Show(filename);
               using (FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write))
               {
                  try
                  {
                     stream.CopyTo(file);
                  }
                  catch (Exception e)
                  {
                     //MessageBox.Show(e.Message);
                  }
               }
            }
         }
         try
         {
            if (!Directory.Exists("C:\\ProgramData\\Cisco\\Cisco Secure Client\\Umbrella"))
            {
               Directory.CreateDirectory("C:\\ProgramData\\Cisco\\Cisco Secure Client\\Umbrella");
            }
         }
         catch (Exception e)
         {
            //MessageBox.Show(e.Message);
         }
         filename = Path.Combine(Path.GetTempPath(), "OrgInfo.json");
         try
         {
            File.Copy(filename, Path.Combine("C:\\ProgramData\\Cisco\\Cisco Secure Client\\Umbrella", "OrgInfo.json"), true);
            File.Delete(filename);
         }
         catch (Exception e)
         {
            MessageBox.Show(e.Message);
         }
         filename = Path.Combine(Path.GetTempPath(), "cisco-secure-client-win-core-vpn-predeploy-k9.msi");
         try
         {
            core = new string[6]{"/package", AddQuotesIfRequired(filename), "/norestart", "/passive", "LOCKDOWN=1", "PRE_DEPLOY_DISABLE_VPN=1"};
            ProcessStartInfo startInfo = new ProcessStartInfo("msiexec.exe");
            startInfo.Arguments = String.Join(" ", core);
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            File.Delete(filename);
         }
         catch (Exception e)
         {
            //MessageBox.Show(e.Message);
         }
         filename = Path.Combine(Path.GetTempPath(), "cisco-secure-client-win-umbrella-predeploy-k9.msi");
         try
         {
            umbrella = new string[5]{"/package", AddQuotesIfRequired(filename), "/norestart", "/passive", "LOCKDOWN=1"};
            ProcessStartInfo startInfo = new ProcessStartInfo("msiexec.exe");
            startInfo.Arguments = String.Join(" ", umbrella);
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            File.Delete(filename);
         }
         catch (Exception e)
         {
            //MessageBox.Show(e.Message);
         }
      }
   }
}
