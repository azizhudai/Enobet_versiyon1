using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enobet_versiyon1.Models
{
    public class NobetDegisimModel
    {
        public string NobetTarihiKendisi { get; set; }
        public string NobetTarihiBaskasi { get; set; }
        public string IslemYapanAdi { get; set; }
        public string OnYeniNobetTarihiKendisi { get; set; }
        public string OnEskiNobetTarihiKendisi { get; set; }
        public string OnNobetDegisimTalepEdilenPersonelTarih_Adi { get; set; }

        public string NobetTarihiTekTarafliKendisi { get; set; }
        public int NobetListeId { get; set; }
        public int MahalId { get; set; }
        public string MahalAdi { get; set; }
        public int AmirId { get; set; }
        public string AmirAdi { get; set; }
        public string TalepEdeninAmirAdi { get; set; }
        public string OnayMakamAdi { get; set; }
        public string AmirEposta { get; set; }
        public string Mazeret { get; set; }
        public int NobetKidemliId { get; set; }
        public string NobetKidemliAdi { get; set; }

        public string TekNobetTarihiKendisi { get; set; }
        public string TekOnYeniNobetTarihiKendisi { get; set; }
        public string TekAmirAdi { get; set; }
        public string TekTalepEdeninAmirAdi { get; set; }
        public string TekNobetKidemliAdi { get; set; }
        public string TekMazeret { get; set; }
    }
}