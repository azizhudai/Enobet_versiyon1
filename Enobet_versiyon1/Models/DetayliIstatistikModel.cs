using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enobet_versiyon1.Models
{
    public class DetayliIstatistikModel
    {
        public int? BirlikId { get; set; }
        public int MahalId { get; set; }
        public int AylikNobetMahalId { get; set; }
        public List<MahalModel> MahalList { get; set; }

        public DetayliIstatistikModel()
        {
            MahalList = new List<MahalModel>();
        }
    }
}