
using System;

namespace CokYasarSapEntegrasyon
{
    public class DatabaseLog
    {
        private readonly PdksDataContext ctx;
        public DatabaseLog()
        {
            ctx = new PdksDataContext();
        }
        public bool Log(Nar_Log model)
        {
            
                ctx.Nar_Logs.InsertOnSubmit(model);
                ctx.SubmitChanges();
           
          
            return true;
        }
    }
}