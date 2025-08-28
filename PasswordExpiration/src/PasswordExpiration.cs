using System;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HomeLab
{
   public class Shell32
   {
      public static void WindowsSecurity()
      {
         Shell shell = new Shell();
         IShellDispatch shellDispatch = (IShellDispatch)shell;
         shellDispatch.WindowsSecurity();
         Marshal.ReleaseComObject(shellDispatch);
         Marshal.ReleaseComObject(shell);
      }
   }
   
   [ComImport, Guid("13709620-C279-11CE-A49E-444553540000")]
   class Shell {
   }
   
   [ComImport, Guid("D8F015C0-C278-11CE-A49E-444553540000")]
   [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
   public interface IShellDispatch {
      void WindowsSecurity();
   }
   
   public static class Program
   {
      [STAThread]
      public static void Main()
      {
         DialogResult result = DialogResult.No;
         UserPrincipal user;
         
         using(PrincipalContext context = new PrincipalContext(ContextType.Domain))
         {
            user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserPrincipal.Current.SamAccountName);
         }
         if (user.PasswordNeverExpires)
         {
            Environment.Exit(0);
         }
         DateTime? pwdLastSet = (DateTime?)user.LastPasswordSet;
         if (ReferenceEquals(pwdLastSet, null))
         {
            Environment.Exit(0);
         }
         DateTime pwdExpiration = Convert.ToDateTime(pwdLastSet).AddMonths(8);
         TimeSpan timeUntilExpiration = pwdExpiration.Subtract(DateTime.UtcNow);
         //MessageBox.Show(String.Format("{0} - {1} = {2}", pwdExpiration.ToLocalTime(), DateTime.UtcNow.ToLocalTime(), timeUntilExpiration.ToString()));
         if (timeUntilExpiration.Days < 1)
         {
            result = MessageBox.Show("Your password expires today.\r\nDo you want to change it now?", "", MessageBoxButtons.YesNo);
         }
         else if (timeUntilExpiration.Days == 1)
         {
            result = MessageBox.Show("Your password expires tomorrow.\r\nDo you want to change it now?", "", MessageBoxButtons.YesNo);
         }
         else if (timeUntilExpiration.Days <= 7)
         {
            result = MessageBox.Show(String.Format("Your password expires in {0} days.\r\nDo you want to change it now?", timeUntilExpiration.Days), "", MessageBoxButtons.YesNo);
         }
         else if (timeUntilExpiration.Days <= 14)
         {
            result = MessageBox.Show(String.Format("Your password expires in {0} days.\r\nDo you want to change it now?", timeUntilExpiration.Days), "", MessageBoxButtons.YesNo);
         }
         /*else if (true)
         {
            result = MessageBox.Show(String.Format("Your password expires in {0} days.\r\nDo you want to change it now?", timeUntilExpiration.Days), "", MessageBoxButtons.YesNo);
         }*/
         if (result == DialogResult.Yes)
         {
            Shell32.WindowsSecurity();
         }
         if (result == DialogResult.No)
         {
            Environment.Exit(0);
         }
      }
   }
}
