using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace CokYasarSapEntegrasyon
{
    public class PersonelOperationModel
    {
        public string GetPersonelLdapInformation()
        {
            string password = @" ""!rz=MKnIeRT$u";

            DirectoryEntry entry = new DirectoryEntry("LDAP://cokyasar.local", @"cokyasar\ckhperad", password);
            DirectorySearcher dsearch = new DirectorySearcher(entry);

            foreach (SearchResult sResultSet in dsearch.FindAll())
            {

                // Login Name
                Debug.Write(GetProperty(sResultSet, "cn"));
            }

            //using (var context = new PrincipalContext(ContextType.Domain, "cokyasar.local", @"cokyasar\ckhperad", password))
            //{
            //    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
            //    {
            //        foreach (var result in searcher.FindAll())
            //        {
            //            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
            //            Console.WriteLine("First Name: " + de.Properties["givenName"].Value);
            //            Console.WriteLine("Last Name : " + de.Properties["sn"].Value);
            //            Console.WriteLine("SAM account name   : " + de.Properties["samAccountName"].Value);
            //            Console.WriteLine("User principal name: " + de.Properties["userPrincipalName"].Value);
            //            Console.WriteLine();
            //        }
            //    }
            //}

            return string.Empty;
        }

        public string GetProperty(SearchResult searchResult,string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return searchResult.Properties[PropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }


    }
}