using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Windows.Forms;

namespace HomeLab
{
   public static class Program
   {
      public static void Main()
      {
         using (Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,  Screen.PrimaryScreen.Bounds.Height))
         {
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
               gfx.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                  Screen.PrimaryScreen.Bounds.Y,
                                  0, 0,
                                  bmp.Size,
                                  CopyPixelOperation.SourceCopy);
            }
            using (MemoryStream ram = new MemoryStream())
            {
               bmp.Save(ram, ImageFormat.Png);
               ram.Position = 0;
               using (MailMessage mail = new MailMessage())
               {
                  if (!string.IsNullOrEmpty(UserPrincipal.Current.EmailAddress))
                  {
                     mail.From = new MailAddress(UserPrincipal.Current.EmailAddress);
                  }
                  else
                  {
                     mail.From = new MailAddress(UserPrincipal.Current.UserPrincipalName);
                  }
                  mail.To.Add(new MailAddress("helpdesk@example.com"));
                  mail.Subject = "ScreenShot";
                  mail.Attachments.Add(new Attachment(ram, "capture.png", "image/png"));
                  mail.IsBodyHtml = true;
                  using (SmtpClient client = new SmtpClient("mail.example.com"))
                  {
                     try
                     {
                        client.Send(mail);
                     }
                     catch (Exception ex)
                     {
                        MessageBox.Show(ex.Message.ToString());
                     }
                  }
               }
            }
         }
      }
   }
}
