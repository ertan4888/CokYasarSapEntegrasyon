using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CokYasarSapEntegrasyon
{
    public class PuantajModel
    {
        public string AdSoyad { get; set; }
        public string UcretTuru { get; set; }
        public string UcretTuruMetni { get; set; }
        public decimal? GunSaat { get; set; }
        public string BirimKod { get; set; }
        public string BirimTanim { get; set; }
       
        public DateTime? Tarih { get; set; }
        public string PersonelNo { get; set; }
        public string PersonelAlani { get; set; }
        public string PersonelAltAlani { get; set; }
        public string SirketKodu { get; set; }
     
    }
}