using System;
using System.ServiceProcess;

namespace HomeLab
{
   static class Program
   {
      static void Main(string[] args)
      {
         if (Environment.UserInteractive)
         {
            WksprtStopper service = new WksprtStopper();
            service.TestStartupAndStop(args);
         }
         else
         {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
               new WksprtStopper()
            };
            ServiceBase.Run(ServicesToRun);
         }
      }
   }
}
