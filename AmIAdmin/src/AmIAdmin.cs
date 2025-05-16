using System;
using System.Security.Principal;
using System.Windows.Forms;

namespace HomeLab
{
   public static class Program
   {
      public static void Main()
      {
         MessageBox.Show(IsAdministrator().ToString());
      }
      public static bool IsAdministrator()
      {
         using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
         {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
         }
      }
   }
}
