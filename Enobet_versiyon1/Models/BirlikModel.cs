using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enobet_versiyon1.Models
{
    public class BirlikModel
    {
        public int BirlikId { get; set; }

        public string BirlikAdi { get; set; }
        public List<BirlikModel> ListBirlik { get; set; }
        public BirlikModel()
        {
            ListBirlik = new List<BirlikModel>();
        }
    }
}