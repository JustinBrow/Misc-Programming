using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
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
         using (PrincipalSearchResult<Principal> searchResult = UserPrincipal.Current.GetAuthorizationGroups())
         {
            return searchResult.Where(x => x.SamAccountName == "Administrators").Any();
         }
      }
   }
}
