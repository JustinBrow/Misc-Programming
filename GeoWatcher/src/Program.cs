using System;
using System.ServiceProcess;

namespace HomeLab
{
   public static class Program
   {
      public static void Main()
      {
         ServiceBase[] ServicesToRun;
         ServicesToRun = new ServiceBase[]
         {
            new GeoWatcherService()
         };
         ServiceBase.Run(ServicesToRun);
      }
   }
}
