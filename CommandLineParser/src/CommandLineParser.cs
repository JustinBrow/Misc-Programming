using System;
using System.Linq;
using System.Collections.Generic;

namespace HomeLab
{
   internal enum OptionTypeEnum
   {
      LongName,
      ShortName,
      Symbol
   }
   internal class CommandLineOption
   {
      internal string Name { get; set; }
      internal string Value { get; set; }
      internal OptionTypeEnum OptionType { get; set; }
   }
   internal static class CommandLineParser
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
   public class Program
   {
      public static void Main(string[] args)
      {
         CommandLineOption[] result = CommandLineParser.ParseOptions(args).ToArray();
         foreach (CommandLineOption item in result)
         {
            Console.WriteLine(item.Name + " " + item.Value + " " + item.OptionType);
         }
         Console.ReadKey();
      }
   }
}
