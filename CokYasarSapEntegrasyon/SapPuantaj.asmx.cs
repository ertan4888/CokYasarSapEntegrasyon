//using Serilog;
//using Serilog.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;

namespace CokYasarSapEntegrasyon
{
    /// <summary>
    /// Summary description for SapPuantaj
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SapPuantaj : System.Web.Services.WebService
    {

        private readonly PdksDataContext ctx;
        private string userName = "Ckhpersap";
        private string passWord = "C0KYsr22*p";
        private DatabaseLog _log;
        public SapPuantaj()
        {
            ctx = new PdksDataContext();
            _log = new DatabaseLog();
        }

        [WebMethod]
        public  void GetPersonelIzinInfo()
        {

            try
            {
                _log.Log(new Nar_Log
                {
                    Message = "GetPersonelIzinInfo methodu başladı",
                    MessageTemplate = "GetPersonelIzinInfo",
                    Level = "Information",
                    RowId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                });


                System.Net.NetworkCredential cred = new System.Net.NetworkCredential();
                cred.UserName = userName;
                cred.Password = passWord;
                var get_SapVendor = new sapPuantajService.ZCYHR_WS_001();
                var get_SapVendor_Input = new sapPuantajService.ZCYHR_FG003_001();
                var get_SapVendor_response = new sapPuantajService.ZCYHR_FG003_001Response();


                get_SapVendor.PreAuthenticate = true;
                get_SapVendor.Credentials = cred;
                get_SapVendor_Input.IV_BEGDA = DateTime.Now.Date.AddMonths(-2).ToString("yyyyMMdd");//2 ay öncesi
                get_SapVendor_Input.IV_ENDDA = DateTime.Now.Date.ToString("yyyyMMdd");

                get_SapVendor_Input.IV_BTRTL = string.Empty;
                get_SapVendor_Input.IV_WERKS = string.Empty;
                get_SapVendor_Input.IV_PERNR = string.Empty;
                get_SapVendor_response = get_SapVendor.ZCYHR_FG003_001(get_SapVendor_Input);

                if (get_SapVendor_response.ET_OUT.Any())
                {

                    _log.Log(new Nar_Log
                    {
                        Message = string.Format("{0}-{1}-{2}", "Sap ZCYHR_WS_001 servisinden geri dönüş alınarak işlem başarı ile tamamlandı.", "Servisten alınan data sayısı", get_SapVendor_response.ET_OUT.Count()),
                        MessageTemplate = "PostPersonelVardiyaInfo",
                        Level = "Information",
                        RequestBody = JsonConvert.SerializeObject(get_SapVendor_Input),
                        TimeStamp = DateTime.Now,
                        ResponseBody = JsonConvert.SerializeObject(get_SapVendor_response.ET_OUT.ToList()),
                        RowId = Guid.NewGuid(),
                    });

                    var sapPuantajModel = new SapPuantajModel();
                    var permissionDays = new List<ENT_GUNIZIN_TEST>();
                    var permissionTimes = new List<ENT_SAATIZIN>();
                    foreach (var item in get_SapVendor_response.ET_OUT)
                    {

                        if (string.IsNullOrEmpty(item.BEGUZ))
                        {


                            var permissionDay = new ENT_GUNIZIN_TEST();
                            permissionDay.BASTARIH = !string.IsNullOrEmpty(item.BEGDA) ? DateTime.ParseExact(item.BEGDA, "yyyyMMdd", CultureInfo.InvariantCulture) : DateTime.Parse("1900-01-01");
                            permissionDay.BITTARIH = !string.IsNullOrEmpty(item.ENDDA) ? DateTime.ParseExact(item.ENDDA, "yyyyMMdd", CultureInfo.InvariantCulture) : DateTime.Parse("1900-01-01");
                            TimeSpan gunFarki = permissionDay.BITTARIH - permissionDay.BASTARIH;
                            permissionDay.GUN = gunFarki.Days;
                            permissionDay.NEDEN = sapPuantajModel.GetPermisionNarCode(item.AWART);
                            permissionDay.PRSICIL = item.PERNR;
                            permissionDays.Add(permissionDay);
                            continue;
                        }


                    
                        var permissionTime = new ENT_SAATIZIN();
                        permissionTime.BASSAAT = !string.IsNullOrEmpty(item.BEGUZ) ?DateTime.Parse(BaseHelper.ConvertNumberToTime(item.BEGUZ)) : DateTime.Parse("1900-01-01");
                        permissionTime.BITSAAT = !string.IsNullOrEmpty(item.ENDUZ) ? DateTime.Parse(BaseHelper.ConvertNumberToTime(item.ENDUZ)) : DateTime.Parse("1900-01-01");
                        permissionTime.TARIH = !string.IsNullOrEmpty(item.BEGDA) ? DateTime.ParseExact(item.BEGDA, "yyyyMMdd", CultureInfo.InvariantCulture) : DateTime.Parse("1900-01-01");
                        permissionTime.NEDEN = sapPuantajModel.GetPermisionNarCode(item.AWART);
                        permissionTime.PRSICIL = item.PERNR;
                        permissionTimes.Add(permissionTime);

                    }
                    sapPuantajModel.CreateDayPermission(permissionDays);
                    sapPuantajModel.CreateTimePermission(permissionTimes);

                }
                _log.Log(new Nar_Log
                {
                    Message = "GetPersonelIzinInfo methodu verileri başarı ile database sistemine basıldı",
                    MessageTemplate = "GetPersonelIzinInfo",
                    Level = "Information",
                    RowId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                });



            }
            catch (Exception ex)
            {
                _log.Log(new Nar_Log
                {

                    Message = "GetPersonelIzinInfo methodunda hata alındı",
                    MessageTemplate = "GetPersonelIzinInfo",
                    Level = "Error",
                    Exception = ex.ToString(),
                    TimeStamp = DateTime.Now,
                    RowId = Guid.NewGuid(),

                });
            }
            

        }

        [WebMethod]
        public void GetSapPersonel()
        {
            try
            {
                _log.Log(new Nar_Log
                {
                    Message = "GetSapPersonel methodu başladı",
                    MessageTemplate = "GetSapPersonel",
                    Level = "Information",
                    RequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            IV_BTRTL = string.Empty,
                            IV_BUKRS = string.Empty,
                            IV_DATUM = DateTime.Now.Date.ToString("yyyyMMdd"),
                            IV_WERKS = string.Empty,
                            IV_PERNR = string.Empty

                        }),
                    RowId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                });



                System.Net.NetworkCredential cred = new System.Net.NetworkCredential();
                cred.UserName = userName;
                cred.Password = passWord;
                //cred.Password = "LWT" + Convert.ToChar(34) + "!3ls$lKb";
                var get_SapVendor = new sapPuantajService.ZCYHR_WS_001();
                var get_SapVendor_Input = new sapPuantajService.ZCYHR_FG003_002();
                var get_SapVendor_response = new sapPuantajService.ZCYHR_FG003_002Response();


                get_SapVendor.PreAuthenticate = false;
                get_SapVendor.Credentials = cred;
                get_SapVendor_Input.IV_BTRTL = string.Empty;
                get_SapVendor_Input.IV_BUKRS = string.Empty;
                get_SapVendor_Input.IV_DATUM = DateTime.Now.Date.ToString("yyyyMMdd");
                get_SapVendor_Input.IV_WERKS = string.Empty;
                get_SapVendor_Input.IV_PERNR = string.Empty;
                get_SapVendor_response = get_SapVendor.ZCYHR_FG003_002(get_SapVendor_Input);


                foreach (var personel in get_SapVendor_response.ET_OUT)
                {
                    var ent_Personel = new ENT_PERSONEL_TEST();
                    ent_Personel.SICILNO = personel?.PERNR.TrimStart('0');
                    ent_Personel.ADI = personel.VORNA;
                    ent_Personel.SOYADI = personel.NACHN;
                    ent_Personel.PERSONEL_TIPI_KODU = personel.BUKRS;
                    ent_Personel.PERSONEL_TIPI_ADI = personel.BUTXT;
                    ent_Personel.DEPARTMAN_KODU = personel.WERKS;
                    ent_Personel.DEPARTMAN_ADI = personel.PBTXT;
                    ent_Personel.ALT_DEPARTMAN_ADI = personel.BTEXT;
                    ent_Personel.ALT_DEPARTMAN_KODU = personel.BTRTL;
                    ent_Personel.MASRAF_MERKEZI_KODU = personel.KOSTL;
                    ent_Personel.MASRAF_MERKEZI_ADI = personel.LTEXT;
                    ent_Personel.ISE_GIRIS_TARIHI = (personel.HIREDATE != "0000-00-00") ? DateTime.Parse(personel.HIREDATE) : (DateTime?)null;
                    ent_Personel.ISTEN_CIKIS_TARIHI = (personel.FIREDATE != "0000-00-00") ? DateTime.Parse(personel.FIREDATE) : (DateTime?)null;
                    ent_Personel.CIKIS_NEDENI_KODU = personel.MASSG;
                    ent_Personel.CIKIS_NEDENI_ADI = personel.MGTXT;
                    ent_Personel.TOPLULUGA_GIRIS_TARIHI = (personel.TOPDT != "0000-00-00") ? DateTime.Parse(personel.TOPDT) : (DateTime?)null;
                    ent_Personel.AMIR1_KODU = personel?.YNTSCL.TrimStart('0');
                    ent_Personel.AMIR1_ADI = personel.YNTAD;
                    ent_Personel.AMIR2_KODU = personel?.YNTSCL2.TrimStart('0');
                    ent_Personel.AMIR2_ADI = personel.YNTAD2;
                    ent_Personel.AMIR3_KODU = personel?.YNTSCL3.TrimStart('0'); 
                    ent_Personel.AMIR3_ADI = personel.YNTAD3;
                    ent_Personel.AMIR4_KODU = personel?.YNTSCL4.TrimStart('0'); 
                    ent_Personel.AMIR4_ADI = personel.YNTAD4;
                    ent_Personel.AMIR5_KODU = personel?.YNTSCL5.TrimStart('0');
                    ent_Personel.AMIR5_ADI = personel.YNTAD5;
                    ent_Personel.MESLEK_KODU = personel.PERSG.TrimStart('0'); //Çalışan  Grubu Kodu
                    ent_Personel.MESLEK_ADI = personel.PERSG_TX;//Çalışan  Grubu Tanımı
                    ent_Personel.GOREV_KODU = personel?.PLANS.TrimStart('0'); // Personel PozisyonKodu
                    ent_Personel.GOREV_ADI = personel.PLNTX;// Personel Tanımı
                    ent_Personel.KADRO_KODU = personel.PERSK;//Çalışan Alt Grubu Kodu
                    ent_Personel.KADRO_ADI = personel.PTEXT; //Çalışan Alt Grubu Tanım
                    ent_Personel.SUBE_KODU = personel.ORGEH; // Çalışan Organizyon Birim Kodu
                    ent_Personel.SUBE_ADI = personel.ORGTX;// Çalışan organizasyon Birim Tanımı
                    ent_Personel.GRUP_KODU = personel.STELL; // Çalışan İş Kodu
                    ent_Personel.GRUP_ADI = personel.STLTX; // Çalışan İş Kodu Tanımı
                    ctx.ENT_PERSONEL_TESTs.InsertOnSubmit(ent_Personel);

                }

                ctx.SubmitChanges();
                _log.Log(new Nar_Log
                {
                    Message = string.Format("{0}-{1}", "GetSapPersonel işlemi başarı ile bitti.Toplam Gelen sayı", get_SapVendor_response.ET_OUT.Count()),
                    MessageTemplate = "GetSapPersonel",
                    Level = "Information",
                    RequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            IV_BTRTL = string.Empty,
                            IV_BUKRS = string.Empty,
                            IV_DATUM = DateTime.Now.Date.ToString("yyyyMMdd"),
                            IV_WERKS = string.Empty,
                            IV_PERNR = string.Empty

                        }),
                    RowId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now,
                    ResponseBody = JsonConvert.SerializeObject(get_SapVendor_response.ET_OUT),
                });

            }
            catch (Exception ex)
            {

                _log.Log(new Nar_Log
                {
                    Message = "GetSapPersonel methodunda hata alındı",
                    MessageTemplate = "GetSapPersonel",
                    Level = "Error",
                    Exception = ex.ToString(),
                    TimeStamp = DateTime.Now,
                    RowId = Guid.NewGuid(),
                });
            }

        }


    }
}
