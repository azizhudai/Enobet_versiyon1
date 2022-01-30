using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enobet_versiyon1.Models
{
    public class NobetListeModel
    {
        public int NobetListId { get; set; }
        public DateTime NobetTarihi { get; set; }
        public int KullaniciId { get; set; }
        public string KullaniciAdi { get; set; }

    }
}