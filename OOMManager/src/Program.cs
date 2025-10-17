using System;
using System.ServiceProcess;

namespace HomeLab
{
   static class Program
   {
      static void Main(string[] args)
      {
         ServiceBase[] ServicesToRun;
         ServicesToRun = new ServiceBase[]
         {
            new OOMManagerService()
         };
         ServiceBase.Run(ServicesToRun);
      }
   }
}
