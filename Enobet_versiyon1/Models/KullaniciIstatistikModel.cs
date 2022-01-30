using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enobet_versiyon1.Models
{
    public class KullaniciIstatistikModel
    {
        public int? BirlikId { get; set; }
        public int MahalId { get; set; }
        public string MazeretBaslamaBitisTarihi { get; set; }
        public string MazeretAciklama { get; set; }

        public List<MahalModel> MahalList { get; set; }

        public KullaniciIstatistikModel()
        {
            MahalList = new List<MahalModel>();
        }
    }
}