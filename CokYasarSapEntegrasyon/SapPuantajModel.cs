using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CokYasarSapEntegrasyon
{
    public class SapPuantajModel
    {

        PdksDataContext ctx;
        public SapPuantajModel()
        {
            ctx = new PdksDataContext();
        }

        public string GetPermisionNarCode(string sapPermissionCode)
        {
            return ctx.NARSOFT_SAP_CALKODUs.FirstOrDefault(x => x.UCRET_TURU == sapPermissionCode)?.NARSOFT_KODU ?? string.Empty;
        }

        public void CreateDayPermission(List<ENT_GUNIZIN_TEST> model)
        {
            ctx.ENT_GUNIZIN_TESTs.InsertAllOnSubmit(model);

            ctx.SubmitChanges();
        }

        public void  CreateTimePermission(List<ENT_SAATIZIN> model)
        {
            ctx.ENT_SAATIZINs.InsertAllOnSubmit(model);
           
            ctx.SubmitChanges();
        }

        
    }
}