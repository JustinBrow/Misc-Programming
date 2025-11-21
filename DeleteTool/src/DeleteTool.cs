using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;

namespace HomeLab
{
   public static class Program
   {
      private class Path
      {
         public string path { get; set; }
         public int days { get; set; }
      }
      public static void Main()
      {
         List<Path> Paths = new List<Path>();
         JavaScriptSerializer serializer = new JavaScriptSerializer();
         #if DEBUG
         AllocConsole();
         IntPtr handle = GetConsoleWindow();
         ShowWindow(handle, SW_MAXIMIZE);
         #endif
         // Make a JSON file using PowerShell
         // @(@{path='C:\temp'; days=1}, @{path='C:\Users'; days=2}, @{path='C:\Windows'; days=7}) | ConvertTo-Json -Compress
         string configPath = AppDomain.CurrentDomain.BaseDirectory + "\\DeleteTool.json";
         if(!File.Exists(configPath))
         {
            // https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-#ERROR_FILE_NOT_FOUND
            Environment.Exit(2);
         }
         string JSON = File.ReadAllText(configPath);
         try
         {
            Paths = serializer.Deserialize<List<Path>>(JSON);
         }
         catch (Exception ex)
         {
            #if DEBUG
            Console.WriteLine("JSON parse error: " + ex.ToString()); // debug print
            Console.ReadKey(); // pause before application termination
            #endif
            // https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-#ERROR_INVALID_DATA
            Environment.Exit(13);
         }
         if (Paths.Count > 0)
         {
            foreach(Path arg in Paths)
            {
               string path = Environment.ExpandEnvironmentVariables(arg.path);
               DateTime days = DateTime.Now.AddDays(-(Math.Abs(arg.days)));
               if(File.Exists(path))
               {
                  #if DEBUG
                  Console.WriteLine("If older than '{0}' '{1}' will be deleted", days, path); // debug print
                  #endif
                  ProcessFile(path, days);
               }
               else if(Directory.Exists(path))
               {
                  #if DEBUG
                  Console.WriteLine("Files older than '{0}' under '{1}' will be deleted", days, path); // debug print
                  #endif
                  ProcessDirectory(path, days);
               }
               else
               {
                  #if DEBUG
                  Console.WriteLine("{0} is not a valid file or directory.", path);
                  #endif
               }
            }
         }
         #if DEBUG
         Console.WriteLine("Done!");
         Console.ReadKey(); // pause before application termination
         FreeConsole();
         #endif
      }
      private static void ProcessDirectory(string targetDirectory, DateTime days)
      {
         string[] fileEntries;
         string [] subdirectoryEntries;
         try
         {
            fileEntries = Directory.GetFiles(targetDirectory);
            subdirectoryEntries = Directory.GetDirectories(targetDirectory);
         }
         catch (UnauthorizedAccessException ex)
         {
            #if DEBUG
            Console.WriteLine("Inaccessible directory '{0}'", targetDirectory); // debug print
            #endif
            return;
         }
         
         foreach(string fileName in fileEntries)
         {
            ProcessFile(fileName, days);
         }
         
         foreach(string subdirectory in subdirectoryEntries)
         {
            int countFiles = 0;
            DirectoryInfo di;
            ProcessDirectory(subdirectory, days);
            try
            {
               di = new DirectoryInfo(subdirectory);
               countFiles = di.GetFiles().Length;
            }
            catch (UnauthorizedAccessException ex)
            {
               #if DEBUG
               Console.WriteLine("Inaccessible directory '{0}'", subdirectory); // debug print
               #endif
               continue;
            }
            if (countFiles == 0)
            {
               try
               {
                  #if !DEBUG
                  di.Attributes = FileAttributes.Normal;
                  di.Delete();
                  #endif
               }
               catch (Exception ex)
               {
               }
               #if DEBUG
               Console.WriteLine("Processed directory '{0}'. File count '{1}'", subdirectory, di.GetFiles().Length); // debug print
               #endif
            }
         }
      }
      private static void ProcessFile(string path, DateTime days)
      {
         FileInfo fi;
         try
         {
            fi = new FileInfo(path);
         }
         catch (UnauthorizedAccessException ex)
         {
            #if DEBUG
            Console.WriteLine("Inaccessible file '{0}'", path); // debug print
            #endif
            return;
         }
         if (fi.LastWriteTime < days)
         {
            try
            {
               #if !DEBUG
               fi.Attributes = FileAttributes.Normal;
               fi.Delete();
               #endif
            }
            catch (Exception ex)
            {
            }
            #if DEBUG
            Console.WriteLine("Processed file '{0}'. Last modified '{1}'", path, fi.LastWriteTime); // debug print
            #endif
         }
      }
      #if DEBUG
      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool AllocConsole();
      
      [DllImport("kernel32.dll", SetLastError = true)]
      private static extern bool FreeConsole();
      
      [DllImport("kernel32.dll", ExactSpelling = true)]
      private static extern IntPtr GetConsoleWindow();
      
      [DllImport("user32.dll")]
      private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
      
      private const int SW_MAXIMIZE = 3;
      #endif
   }
}
