using System;
using System.IO;

namespace HomeLab
{
   public static class Program
   {
      public static void Main(string[] args)
      {
         string AADBrokerDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", "Microsoft.AAD.BrokerPlugin_cw5n1h2txyewy");
         string AADBrokerDirectoryBackup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages", "Microsoft.AAD.BrokerPlugin_cw5n1h2txyewy_backup");
         
         foreach (string arg in args)
         {
            if (arg == "/logon")
            {
               if (Directory.Exists(AADBrokerDirectoryBackup))
               {
                  while (!Directory.Exists(AADBrokerDirectory))
                  {
                     System.Threading.Thread.Sleep(1000);
                  }
                  CopyDirectory(AADBrokerDirectoryBackup, AADBrokerDirectory, true);
               }
               Environment.Exit(0);
            }
            if (arg == "/logoff")
            {
               if (Directory.Exists(AADBrokerDirectory))
               {
                  CopyDirectory(AADBrokerDirectory, AADBrokerDirectoryBackup, true);
               }
               Environment.Exit(0);
            }
         }
      }
      static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
      {
         
         DirectoryInfo dir = new DirectoryInfo(sourceDir);
         
         if (!dir.Exists)
         {
            throw new DirectoryNotFoundException(String.Format("Source directory not found: {0}", dir.FullName));
         }
         
         DirectoryInfo[] dirs = dir.GetDirectories();
         
         Directory.CreateDirectory(destinationDir);
         
         foreach (FileInfo file in dir.GetFiles())
         {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
         }
         
         if (recursive)
         {
            foreach (DirectoryInfo subDir in dirs)
            {
               string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
               CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
         }
      }
   }
}
