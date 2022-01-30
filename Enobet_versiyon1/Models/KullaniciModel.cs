using Enobet_versiyon1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enobet_versiyon1.Models
{
    public class KullaniciModel
    {
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; }
        public string KullaniciSoyadi { get; set; }
        public string Eposta { get; set; }
        public int? BirlikId { get; set; }
        public int? SinifId { get; set; }
        public int RutbeId { get; set; }
        public int? MahalId { get; set; }
        public Byte RolId { get; set; }
        public string Tcno { get; set; }
        public string Parola { get; set; }
        public string Rutbe { get; set; }
        public byte Aktif { get; set; }
        public bool? NobettenCikar { get; set; }
        public List<KullaniciModel> KullaniciList { get; set; }
        public List<BirlikModel> BirlikList { get; set; }
        public List<MahalModel> MahalList { get; set; }
        public List<KullaniciKategoriListe> KullaniciKategoriListeList { get; set; }
        public KullaniciModel()
        {
            KullaniciList = new List<KullaniciModel>();
            MahalList = new List<MahalModel>();
            BirlikList = new List<BirlikModel>();
            KullaniciKategoriListeList = new List<KullaniciKategoriListe>();
        }
        public class KullaniciKategoriListe
        {
            public int KategoriListeId { get; set; }
            public string Adi { get; set; }
            public int KategoriValue { get; set; }

        }
        public class SayfaModel
        {
            public int SayfaId { get; set; }
            public string Adi { get; set; }
        }

    }
}