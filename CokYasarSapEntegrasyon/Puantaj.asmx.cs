using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;

namespace CokYasarSapEntegrasyon
{
    /// <summary>
    /// Summary description for Puantaj
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Puantaj : System.Web.Services.WebService
    {
        private readonly PdksDataContext ctx;
        private DatabaseLog _log;
        private string userName = "Ckhpersap";
        private string passWord = "C0KYsr22*p";
        public Puantaj()
        {
            ctx = new PdksDataContext();
            _log = new DatabaseLog();
        }

        [WebMethod]
        public List<PuantajModel> GetPuantaj(int ay, int yil, string personelNo, string personelAlani, string personelAltAlani, string sirketKodu)
        {

            try
            {
                _log.Log(new Nar_Log
                {
                    Message = "GetPuantaj methodu başladı",
                    MessageTemplate = "GetPuantaj",
                    Level = "Information",
                    RequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            Ay = ay,
                            Yil = yil,
                            PersonelNo = personelNo,
                            PersonelAlani = personelAlani,
                            PersonelAltAlani = personelAltAlani,
                            SirketKodu = sirketKodu,

                        }),
                    RowId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                });

                var _listPuantaj = new List<PuantajModel>();

                if (ay == 0 || yil == 0)
                    return null;

                var lastDateOfMonth = LastDateOfMonth(ay, yil);

                var query = from puantaj in ctx.AYPUAN1s
                            join personel in ctx.PERSONEL1s on puantaj.PRSICIL equals personel.SICILNO
                            join cros in ctx.NARSOFT_SAP_CALKODUs on puantaj.CLKODU.ToString() equals cros.NARSOFT_KODU
                            where
                            (puantaj.AY == ay && puantaj.YIL == yil) || personel.PERTIP == sirketKodu || personel.DEPART == personelAlani || personel.ALTDEPART == personelAltAlani
                            select new PuantajModel
                            {
                                AdSoyad = string.Format("{0} {1}", personel.ADI, personel.SOYADI),
                                PersonelNo = puantaj.PRSICIL,
                                GunSaat = (cros.ZAMAN_BIRIM == "GUN") ? puantaj.TOPGUN : puantaj.ONDSURE,
                                UcretTuru = cros.UCRET_TURU,
                                UcretTuruMetni = cros.UCRET_TURU_METNI,
                                BirimKod = cros.ZAMAN_KOD,
                                BirimTanim = cros.ZAMAN_BIRIM,
                                Tarih = personel.ISCIKT == null ? lastDateOfMonth : personel.ISCIKT.Value.AddDays(-1),
                                PersonelAlani = personel.DEPART.Trim(),
                                PersonelAltAlani = (personel.ALTDEPART == null) ? string.Empty : personel.ALTDEPART.Trim(),
                                SirketKodu = personel.PERTIP.Trim()

                            };

                _listPuantaj = query.ToList();
                // if (personelNo != "") bu alan sap den null olarak geliyor. Bizim testlerde "" ile kontrol gerekiyor.
                if (personelNo != null)
                    _listPuantaj = _listPuantaj.Where(x => x.PersonelNo == personelNo).ToList();

                if (personelAlani != null)
                    _listPuantaj = _listPuantaj.Where(x => x.PersonelAlani == personelAlani).ToList();

                if (personelAltAlani != null)
                    _listPuantaj = _listPuantaj.Where(x => x.PersonelAltAlani == personelAltAlani).ToList();

                if (sirketKodu != null)
                    _listPuantaj = _listPuantaj.Where(x => x.SirketKodu == sirketKodu).ToList();


                _log.Log(new Nar_Log
                {
                    Message = string.Format("{0}-{1}", "GetPuantaj işlemi başarı ile bitti.Toplam gönderilen sayı", _listPuantaj),
                    MessageTemplate = "GetPuantaj",
                    Level = "Information",
                    RequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(
                       new
                       {
                           Ay = ay,
                           Yil = yil,
                           PersonelNo = personelNo,
                           PersonelAlani = personelAlani,
                           PersonelAltAlani = personelAltAlani,
                           SirketKodu = sirketKodu,

                       }),
                    ResponseBody = JsonConvert.SerializeObject(_listPuantaj),
                    RowId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                });

                return _listPuantaj;
            }
            catch (Exception ex)
            {
                _log.Log(new Nar_Log
                {

                    Message = "GetPuantaj methodunda hata alındı",
                    MessageTemplate = "GetPuantaj",
                    Level = "Error",
                    Exception = ex.ToString(),
                    TimeStamp = DateTime.Now,
                    RowId = Guid.NewGuid(),

                });

                return null;
            }
        }

        [WebMethod]
        public string PostPersonelVardiyaInfo()
        {
            try
            {

                _log.Log(new Nar_Log
                {
                    Message = "PostPersonelVardiyaInfo methodu başladı",
                    MessageTemplate = "PostPersonelVardiyaInfo",
                    Level = "Information",
                    RowId = Guid.NewGuid(),
                    TimeStamp = DateTime.Now
                });



                var _listData = new List<sapPuantajService.ZCYHR_S023>();


                var a = ctx.NARSOFT_SAP_POSTAs.Where(x => x.TARIHSAAT.Value.Date == DateTime.Now.Date).ToList();
                             

                var query = (from posta in ctx.NARSOFT_SAP_POSTAs.Where(x => x.ISLEMTARIHI.Value.Date == DateTime.Now.Date).ToList()
                             join vardiya in ctx.NARSOFT_SAP_VRKODUs on posta.POSTA equals vardiya.VR_NARSOFT

                             select new sapPuantajService.ZCYHR_S023
                             {
                                 PERNR = posta.SICILNO.TrimEnd(),
                                 DATUM = posta.TARIHSAAT.Value.ToString("yyyyMMdd"),
                                 TPROG = vardiya.VR_SAP
                             }).ToList();




                if (!query.Any())
                {
                    _log.Log(new Nar_Log
                    {
                        Message = "Sap ZCYHR_FG003_003 servisi çağrıldı",
                        MessageTemplate = "PostPersonelVardiyaInfo",
                        Level = "Information",
                        RequestBody = JsonConvert.SerializeObject(query.ToList()),
                        TimeStamp = DateTime.Now,
                        RowId = Guid.NewGuid(),
                        ResponseBody = "Posta Verisi bulunamadı"
                    });
                    return "Posta Verisi bulunamadı";
                }

                _log.Log(new Nar_Log
                {
                    Message = "Sap ZCYHR_FG003_003 servisi çağrıldı",
                    MessageTemplate = "PostPersonelVardiyaInfo",
                    Level = "Information",
                    RequestBody = JsonConvert.SerializeObject(query.ToList()),
                    TimeStamp = DateTime.Now,
                    RowId = Guid.NewGuid(),
                });

                System.Net.NetworkCredential cred = new System.Net.NetworkCredential();
                cred.UserName = userName;
                cred.Password = passWord;
                var get_SapVendor = new sapPuantajService.ZCYHR_WS_001();
                var get_SapVendor_Input = new sapPuantajService.ZCYHR_FG003_003();
                var get_SapVendor_response = new sapPuantajService.ZCYHR_FG003_003Response();


                get_SapVendor.PreAuthenticate = false;
                get_SapVendor.Credentials = cred;
                get_SapVendor_Input.IT_DATA = query.ToArray();


                get_SapVendor_response = get_SapVendor.ZCYHR_FG003_003(get_SapVendor_Input);

                _log.Log(new Nar_Log
                {
                    Message = string.Format("{0}-{1}-{2}", "Sap ZCYHR_FG003_003 servisinden geri dönüş alınarak işlem başarı ile tamamlandı.", "Servisten alınan data sayısı", get_SapVendor_response.ET_MESSAGE.Count()),
                    MessageTemplate = "PostPersonelVardiyaInfo",
                    Level = "Information",
                    RequestBody = JsonConvert.SerializeObject(query.ToList()),
                    TimeStamp = DateTime.Now,
                    ResponseBody = JsonConvert.SerializeObject(get_SapVendor_response.ET_MESSAGE.ToList()),
                    RowId = Guid.NewGuid(),
                });


                if (get_SapVendor_response.ET_MESSAGE.Any())
                    UpdatePostaFlag(get_SapVendor_response.ET_MESSAGE.ToList());


                return "İşlem Başarılı";
            }
            catch (Exception ex)
            {
                _log.Log(new Nar_Log
                {

                    Message = "PostPersonelVardiyaInfo methodunda hata alındı",
                    MessageTemplate = "PostPersonelVardiyaInfo",
                    Level = "Error",
                    Exception = ex.ToString(),
                    TimeStamp = DateTime.Now,
                    RowId = Guid.NewGuid(),

                });
                return ex.Message;
            }

        }

        public DateTime? LastDateOfMonth(int ay, int yil)
        {
            int aySonGun = DateTime.DaysInMonth(yil, ay);
            string sonTarih = string.Format("{0}.{1}.{2}", aySonGun, ay, yil);
            return DateTime.Parse(sonTarih);
        }


        private void UpdatePostaFlag(List<sapPuantajService.ZCYHR_S024> messages)
        {
            foreach (var result in messages)
            {
                if (result.MESSAGE == "Başarılı!")
                {
                    var posta = ctx.NARSOFT_SAP_POSTAs.FirstOrDefault(x => x.FLAG == true && x.SICILNO == result.PERNR.TrimStart('0'));
                    if (posta != null)
                        posta.FLAG = false;


                }
            }
            ctx.SubmitChanges();

        }


    }
}
