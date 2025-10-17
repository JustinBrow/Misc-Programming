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
         using (PrincipalSearchResult<Principal> authorizationGroups = UserPrincipal.Current.GetAuthorizationGroups())
         {
            return authorizationGroups.Where((Principal x) => x.Sid == new SecurityIdentifier("S-1-5-32-544")).Any();
         }
      }
   }
}
