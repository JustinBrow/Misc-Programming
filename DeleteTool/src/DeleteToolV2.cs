using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HomeLab
{
   public static class Program
   {
      public static void Main(string[] args)
      {
         #if DEBUG
         AllocConsole();
         #endif
         int days = 7;
         foreach(string arg in args)
         {
            string[] param = arg.Split(',');
            string path = param[0];
            try
            {
               days = Math.Abs(Int32.Parse(param[1]));
            }
            catch
            {
               // https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-#ERROR_BAD_ARGUMENTS
               Environment.Exit(160);
            }
            #if DEBUG
            Console.WriteLine("Files  older than'{0}' will be deleted", DateTime.Now.AddDays(-days)); // debug print
            #endif
            if(File.Exists(path))
            {
               ProcessFile(path, days);
            }
            else if(Directory.Exists(path))
            {
               ProcessDirectory(path, days);
            }
            else
            {
               //Console.WriteLine("{0} is not a valid file or directory.", path);
            }
         }
         #if DEBUG
         Console.ReadKey(); // pause before application termination
         FreeConsole();
         #endif
      }
      public static void ProcessDirectory(string targetDirectory, int days)
      {
         string[] fileEntries = Directory.GetFiles(targetDirectory);
         foreach(string fileName in fileEntries)
         {
            ProcessFile(fileName, days);
         }
         
         string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
         foreach(string subdirectory in subdirectoryEntries)
         {
            ProcessDirectory(subdirectory, days);
            DirectoryInfo di = new DirectoryInfo(subdirectory);
            if (di.GetFiles().Length == 0)
            {
               try
               {
                  di.Attributes = FileAttributes.Normal;
                  #if !DEBUG
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
      public static void ProcessFile(string path, int days)
      {
         FileInfo fi = new FileInfo(path);
         if (fi.LastWriteTime < DateTime.Now.AddDays(-days))
         {
            try
            {
               fi.Attributes = FileAttributes.Normal;
               #if !DEBUG
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
      static extern bool AllocConsole();
      
      [DllImport("kernel32.dll", SetLastError = true)]
      static extern bool FreeConsole();
      #endif
   }
}
