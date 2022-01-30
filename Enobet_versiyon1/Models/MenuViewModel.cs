using System.Collections.Generic;

namespace DYS.Web.Models
{
    public class MenuModel
    {
       public List<MenuViewModel> Menus { get; set; }
       public List<Sayfalar> Sayfaidler { get; set; }
    }
    public class MenuViewModel
    {
       public Sayfalar Sayfa { get; set; }
       public List<Sayfalar> List { get; set; }
     
    }
    public class Sayfalar
    {
        public int SayfaId { get; set; }
        public string Adi { get; set; }
        public string ControllerAdi { get; set; }
        public string ActionAdi { get; set; }
        public string FotografYolu { get; set; }
        public bool? UstBaslik { get; set; }
        public int? UstId { get; set; }
        public int? Sira { get; set; }
        public bool Aktif { get; set; }
    }
    public class SayfaYetkiler
    {
        public int SayfaYetkiId { get; set; }
        public int SayfaId { get; set; }
        public byte RolId { get; set; }
        public bool Aktif { get; set; }
    }
}