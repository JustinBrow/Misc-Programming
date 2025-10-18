using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.PowerShell;

namespace HomeLab
{
   class Program
   {
      static void Main(string[] args)
      {
         if (args.Length == 0) return;
         CommandLineOption[] result = CommandLineParser.ParseOptions(args).ToArray();
         if (!isElevated() && Array.Exists(result, Arg => Arg.Name == "elevate"))
         {
            ExecuteAsAdmin(Process.GetCurrentProcess().MainModule.FileName, args);
         }
         else
         {
            InitialSessionState iss = InitialSessionState.CreateDefault();
            iss.ExecutionPolicy = ExecutionPolicy.Unrestricted;
            using (PowerShell PowerShellInstance = PowerShell.Create(iss))
            {
               Collection<PSObject> PSOutput = null;
               foreach (CommandLineOption item in result)
               {
                  if (item.Name == "elevate") continue;
                  if (item.Name == "script")
                  {
                     FileInfo fi = new FileInfo(item.Value);
                     if (fi.Extension == ".ps1")
                     {
                        string Script = File.ReadAllText(item.Value);
                        PowerShellInstance.AddScript(Script);
                     }
                     else
                     {
                        try
                        {
                           throw new ArgumentException(item.Value);
                        }
                        catch (Exception ex)
                        {
                           AllocConsole();
                           Console.WriteLine(ex.ToString());
                           Console.ReadKey();
                           FreeConsole();
                           Environment.Exit(1);
                        }
                     }
                     continue;
                  }
                  PowerShellInstance.AddParameter(item.Name, item.Value);
               }
               PowerShellInstance.Streams.Error.DataAdded       += new EventHandler<DataAddedEventArgs>(Error_DataAdded);
               PowerShellInstance.Streams.Warning.DataAdded     += new EventHandler<DataAddedEventArgs>(Warning_DataAdded);
               PowerShellInstance.Streams.Verbose.DataAdded     += new EventHandler<DataAddedEventArgs>(Verbose_DataAdded);
               PowerShellInstance.Streams.Information.DataAdded += new EventHandler<DataAddedEventArgs>(Information_DataAdded);
               PowerShellInstance.Streams.Debug.DataAdded       += new EventHandler<DataAddedEventArgs>(Debug_DataAdded);
               try
               {
                  AllocConsole();
                  Console.WriteLine("INFO: Launching '" + Array.Find(result, Arg => Arg.Name == "script").Value +"'");
                  PSOutput = PowerShellInstance.Invoke();
                  if (PSOutput != null)
                  {
                     Console.WriteLine(String.Join(Environment.NewLine, PSOutput));
                     Thread.Sleep(5000);
                  }
               }
               catch (Exception ex)
               {
                  Console.WriteLine(ex.Message.ToString());
                  if (PSOutput != null)
                  {
                     Console.WriteLine(String.Join(Environment.NewLine, PSOutput));
                  }
                  Console.ReadKey();
               }
               finally
               {
                  FreeConsole();
               }
            }
         }
      }
      internal static bool isElevated()
      {
         try
         {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
               WindowsPrincipal principal = new WindowsPrincipal(identity);
               return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
         }
         catch
         {
            return false;
         }
      }
      internal static void ExecuteAsAdmin(string fileName, string[] args)
      {
         ProcessStartInfo startInfo = new ProcessStartInfo();
         startInfo.FileName = fileName;
         if (args.Length >= 1)
         {
            startInfo.Arguments = GetArgumentStr(new List<string>(args));
         }
         startInfo.UseShellExecute = true;
         startInfo.Verb = "runas";
         try
         {
            Process.Start(startInfo);
         }
         catch (Exception ex)
         {
            AllocConsole();
            Console.WriteLine(ex.ToString());
            Console.ReadKey();
            FreeConsole();
         }
      }
      internal static string GetArgumentStr(List<string> argList)
      {
         if (argList == null || argList.Count == 0)
         {
            return string.Empty;
         }
         StringBuilder sb = new StringBuilder();
         foreach (string arg in argList)
         {
            PasteArguments.AppendArgument(sb, arg);
         }
         return sb.ToString();
      }
      internal static void Error_DataAdded(object sender, DataAddedEventArgs e)
      {
         Console.WriteLine( ((PSDataCollection<ErrorRecord>) sender)[e.Index].ToString() );
      }
      internal static void Warning_DataAdded(object sender, DataAddedEventArgs e)
      {
         Console.WriteLine( ((PSDataCollection<WarningRecord>) sender)[e.Index].ToString() );
      }
      internal static void Verbose_DataAdded(object sender, DataAddedEventArgs e)
      {
         Console.WriteLine( ((PSDataCollection<VerboseRecord>) sender)[e.Index].ToString() );
      }
      internal static void Information_DataAdded(object sender, DataAddedEventArgs e)
      {
         Console.WriteLine( ((PSDataCollection<InformationRecord>) sender)[e.Index].ToString() );
      }
      internal static void Debug_DataAdded(object sender, DataAddedEventArgs e)
      {
         Console.WriteLine( ((PSDataCollection<DebugRecord>) sender)[e.Index].ToString() );
      }
      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      static extern bool AllocConsole();
      
      [DllImport("kernel32.dll", SetLastError = true)]
      static extern bool FreeConsole();
   }
   internal enum OptionTypeEnum
   {
      LongName,
      ShortName,
      Symbol
   }
   internal class CommandLineOption
   {
      public string Name { get; set; }
      public string Value { get; set; }
      public OptionTypeEnum OptionType { get; set; }
   }
   public static class CommandLineParser
   {
      internal static IList<CommandLineOption> ParseOptions(string[] arguments)
      {
         List<CommandLineOption> results = new List<CommandLineOption>();
         
         CommandLineOption lastOption = null;
         
         foreach (string argument in arguments)
         {
            if (string.IsNullOrWhiteSpace(argument))
            {
               continue;
            }
            
            if (argument.StartsWith("--", StringComparison.Ordinal))
            {
               if (lastOption != null)
               {
                  results.Add(lastOption);
               }
               
               lastOption = new CommandLineOption
               {
                  OptionType = OptionTypeEnum.LongName,
                  Name = argument.Substring(2)
               };
            }
            else if (argument.StartsWith("-", StringComparison.Ordinal))
            {
               if (lastOption != null)
               {
                  results.Add(lastOption);
               }
               
               lastOption = new CommandLineOption
               {
                  OptionType = OptionTypeEnum.ShortName,
                  Name = argument.Substring(1)
               };
            }
            else if (lastOption == null)
            {
               results.Add(new CommandLineOption
               {
                  OptionType = OptionTypeEnum.Symbol,
                  Name = argument
               });
            }
            else
            {
               lastOption.Value = argument;
               
               results.Add(lastOption);
               
               lastOption = null;
            }
         }
         
         if(lastOption != null)
         {
            results.Add(lastOption);
         }
         
         return results;
      }
   }
}
