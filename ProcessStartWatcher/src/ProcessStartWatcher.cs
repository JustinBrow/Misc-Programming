using System;
using System.Threading;
using System.Management;
using System.Diagnostics;

namespace HomeLab
{
    class Program
    {
        static void Main(string[] args)
        {           
            ManagementEventWatcher w = null;
            WqlEventQuery q;
            try
            {
                q = new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName ='notepad.exe'");
                w = new ManagementEventWatcher(q);
                w.EventArrived += new EventArrivedEventHandler(ProcessStartEventArrived);
                w.Start();
                Console.ReadKey(); // block main thread for test purposes
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                w.Stop();
            }
        }

        static void ProcessStartEventArrived(object sender, EventArrivedEventArgs e)
        {
            foreach (PropertyData pd in e.NewEvent.Properties)
            {
                Console.WriteLine("\n============================= =========");
                Console.WriteLine("{0},{1},{2}", pd.Name, pd.Type, pd.Value);
            }
        }
    }
}
