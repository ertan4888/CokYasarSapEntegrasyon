
using System.Web.Services;

namespace CokYasarSapEntegrasyon
{
    /// <summary>
    /// Summary description for PersonelOperation
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PersonelOperation : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetPersonelLdapInformation()
        {
            var personelOperationModel = new PersonelOperationModel();
            personelOperationModel.GetPersonelLdapInformation();

            return "Hello World";
        }
    }
}
