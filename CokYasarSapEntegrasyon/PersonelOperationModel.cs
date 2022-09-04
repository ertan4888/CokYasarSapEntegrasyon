using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
namespace CokYasarSapEntegrasyon
{
    public class PersonelOperationModel
    {

        private readonly PdksDataContext ctx;
        public PersonelOperationModel()
        {
            ctx = new PdksDataContext();
        }


        public string GetPersonelLdapInformation()
        {
            try
            {


                

                var _log = new DatabaseLog();
                _log.Log(new Nar_Log
                {
                    Message = "LDAP",
                    MessageTemplate = "LDAP",
                    Level = "Information",
                    RequestBody = "Bağlantı yapıldı",
                    RowId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                });

                string password = "?!rz=MKnIeRT$u";

               
                DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://TR0601DCSR01/dc=cokyasar,dc=local", @"cokyasar\ckhperad", password);
                DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = "(objectCategory=user)";
               
                foreach (SearchResult item in directorySearcher.FindAll())
                {
                    DirectoryEntry directoryEntryItem = item.GetDirectoryEntry();

                    var aasAMAccountName = directoryEntryItem.Properties["sAMAccountName"].Value;//
                    var userName = directoryEntryItem.Properties["displayName"].Value;
                    var userPassword = directoryEntryItem.Properties["password"].Value;

                    var kullanici = ctx.KULLANICIs.FirstOrDefault(x => x.DOMAIN_USERNAME== userName.ToString());
                    kullanici.SIFRE = userPassword?.ToString();
                    ctx.SubmitChanges();

                    _log.Log(new Nar_Log
                    {
                        Message = "LDAP",
                        MessageTemplate = "LDAP",
                        Level = "Information",
                        RequestBody = $" Kayıt edilen kullanıcı  {userName}",
                        RowId = Guid.NewGuid(),
                        TimeStamp = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                var _log = new DatabaseLog();
                _log.Log(new Nar_Log
                {
                    Message = "LDAP",
                    MessageTemplate = "LDAP",
                    Level = "Error",
                    RequestBody = ex.ToString(),
                    RowId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                });
            }
         

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