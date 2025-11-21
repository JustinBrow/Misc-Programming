using System;
using System.IO;

namespace HomeLab
{
   public static class Program
   {
      public static void Main(string[] args)
      {
         foreach(string path in args)
         {
            if(File.Exists(path))
            {
               ProcessFile(path);
            }
            else if(Directory.Exists(path))
            {
               ProcessDirectory(path);
            }
            else
            {
               //Console.WriteLine("{0} is not a valid file or directory.", path);
            }
         }
         //Console.ReadKey(); // pause before application termination
      }
      public static void ProcessDirectory(string targetDirectory)
      {
         string[] fileEntries = Directory.GetFiles(targetDirectory);
         foreach(string fileName in fileEntries)
         {
            ProcessFile(fileName);
         }
         
         string [] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
         foreach(string subdirectory in subdirectoryEntries)
         {
            ProcessDirectory(subdirectory);
            DirectoryInfo di = new DirectoryInfo(subdirectory);
            if (di.GetFiles().Length == 0)
            {
               try
               {
                  di.Attributes = FileAttributes.Normal;
                  di.Delete();
               }
               catch (Exception ex)
               {
               }
               //Console.WriteLine("Processed directory '{0}'. File count '{1}'", subdirectory, di.GetFiles().Length); // debug print
            }
         }
      }
      public static void ProcessFile(string path)
      {
         FileInfo fi = new FileInfo(path);
         if (fi.LastWriteTime < DateTime.Now.AddDays(-7))
         {
            try
            {
               fi.Attributes = FileAttributes.Normal;
               fi.Delete();
            }
            catch (Exception ex)
            {
            }
            //Console.WriteLine("Processed file '{0}'. Last modified '{1}'", path, fi.LastWriteTime); // debug print
         }
      }
   }
}
