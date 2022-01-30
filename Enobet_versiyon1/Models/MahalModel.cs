using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enobet_versiyon1.Models
{
    public class MahalModel
    {
        public int MahalId { get; set;}

        public string MahalAdi { get; set; }
        public string DahiliNo { get; set; }
        public int? MahalAmirId { get; set; }
        public string AmirFulAdi { get; set; }
        public int? Sira { get; set; }
        public List<MahalModel> ListMahal{ get; set; }
        public MahalKidemli MahalKidemli { get; set; }
        public MahalModel()
        {
            ListMahal = new List<MahalModel>();
            MahalKidemli = new MahalKidemli();
        }
    }
    public class MahalKidemli
    {
        public string MahalList { get; set; }
        public List<string> SelectedValues { get; set; }

        public MahalKidemli()
        {
            SelectedValues = new List<string>();
        }
    }
}