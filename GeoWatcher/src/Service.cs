using System;
using System.Device.Location;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.Management;
using System.ServiceProcess;
using System.Web.Script.Serialization;

namespace HomeLab
{
   internal partial class GeoWatcherService : ServiceBase
   {
      private static readonly GeoCoordinateWatcher GeoWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
      private static readonly HttpClient client = new HttpClient();
      private static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
      private static System.Timers.Timer timer = new System.Timers.Timer(1000 * 60 * 15); // 15 minutes
      internal GeoWatcherService()
      {
         InitializeComponent();
      }
      
      private void UpdateLocation()
      {
         while (GeoWatcher.Status != GeoPositionStatus.Ready)
         {
            Thread.Sleep(100);
         }
         //WriteToFile(serializer.Serialize(new Geolocation(GeoWatcher)));
         try
         {
            //HttpResponseMessage response = await
            client.PostAsync("https://api.example.com/post", new StringContent(serializer.Serialize(new Geolocation(GeoWatcher)), System.Text.Encoding.UTF8, "application/json"));
            //string responseBody = await response.Content.ReadAsStringAsync();
            //WriteToFile(responseBody);
            /* C# >= 5
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/post");
            request.Content = new StringContent(serializer.Serialize(new Geolocation(GeoWatcher)), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.Send(request);
            WriteToFile(response.ToString());
            */
         }
         catch (Exception ex)
         {
            WriteToFile(ex.ToString());
         }
         //client.PostAsync("http://localhost/post", new StringContent(serializer.Serialize(new Geolocation(GeoWatcher)), System.Text.Encoding.UTF8, "application/json"));
      }
      
      private void UpdateLocation(object sender, System.Timers.ElapsedEventArgs e)
      {
         while (GeoWatcher.Status != GeoPositionStatus.Ready)
         {
            Thread.Sleep(100);
         }
         //WriteToFile(serializer.Serialize(new Geolocation(GeoWatcher)));
         try
         {
            //HttpResponseMessage response = await
            client.PostAsync("https://api.example.com/post", new StringContent(serializer.Serialize(new Geolocation(GeoWatcher)), System.Text.Encoding.UTF8, "application/json"));
            //string responseBody = await response.Content.ReadAsStringAsync();
            //WriteToFile(responseBody);
         }
         catch (Exception ex)
         {
            //timer.Stop();
            //timer.Interval = 1000 * 60 * 20; // 20 minutes
            //timer.Start();
            WriteToFile(ex.ToString());
         }
         //client.PostAsync("http://localhost/post", new StringContent(serializer.Serialize(new Geolocation(GeoWatcher)), System.Text.Encoding.UTF8, "application/json"));
      }
      
      protected override void OnStart(string[] args)
      {
         ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
         GeoWatcher.Start();
         //WriteToFile(GeoWatcher.Position.Location.Latitude.ToString());
         //WriteToFile(GeoWatcher.Position.Location.Longitude.ToString());
         //WriteToFile(GeoWatcher.Position.Timestamp.LocalDateTime.ToString("M/d/yyyy h:m:ss tt"));
         client.DefaultRequestHeaders.Add("X-Client", Environment.MachineName);
         UpdateLocation();
         timer.Elapsed += UpdateLocation;
         timer.AutoReset = true;
         timer.Start();
         //WriteToFile(DateTime.Now + ": GeoWatcher Service started");
         //base.OnStart();
      }
      
      protected override void OnStop()
      {
         //WriteToFile(DateTime.Now + ": GeoWatcher Service stopping");
         timer.Stop();
         GeoWatcher.Stop();
         //base.OnStop();
      }
      
      protected override void OnShutdown()
      {
         //WriteToFile(DateTime.Now + ": GeoWatcher Service stopping");
         //WriteToFile(DateTime.Now + ": System shutting down");
         timer.Stop();
         GeoWatcher.Stop();
         //base.OnShutdown();
      }
      
      private class Geolocation
      {
         public Geolocation (GeoCoordinateWatcher GeoWatcher)
         {
            this.Timestamp = GeoWatcher.Position.Timestamp.LocalDateTime.ToString("M/d/yyyy h:mm:ss tt");
            this.Latitude = GeoWatcher.Position.Location.Latitude.ToString();
            this.Longitude = GeoWatcher.Position.Location.Longitude.ToString();
            this.Hostname = Environment.MachineName;
         }
         public string Timestamp { get; set; }
         public string Latitude { get; set; }
         public string Longitude { get; set; }
         public string Hostname { get; set; }
      }
      internal static void WriteToFile(string Message)
      {
         string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
         if (!Directory.Exists(path))
         {
            Directory.CreateDirectory(path);
         }
         
         string filepath = Path.Combine(path, "GeoWatcherService.log");
         if (!File.Exists(filepath))
         {
            using (StreamWriter sw = File.CreateText(filepath))
            {
               sw.WriteLine(Message);
            }
         }
         else
         {
            using (StreamWriter sw = File.AppendText(filepath))
            {
               sw.WriteLine(Message);
            }
         }
      }
   }
}
