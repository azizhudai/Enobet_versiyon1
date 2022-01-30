using DYS.Web.Models;
using Enobet_versiyon1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace Enobet_versiyon1.Controllers
{
    //public class Sonuc
    //{
    //    public string Mesaj { get; set; }
    //}
    public class HomeController : Controller
    {

        public class KullaniciList
        {
            public int KullaniciId { get; set; }
            public string Parola { get; set; }
        }
        public ActionResult Index()
        {

            /* ModelContext mc = new ModelContext();
            // Common.EncodeStr(dataUserArray[7]);
             var query =  ""; //"select ('Pl'+substring(Tcno,0,7)) as Parola from Kullanici";
             var PswList = mc.Database.SqlQuery<KullaniciList>("select Parola,KullaniciId from Kullanici"
                          ).ToList();

             for(int i=0;i<PswList.Count;i++)
             {
                 var newPsw = Common.EncodeStr(PswList[i].Parola);
                 mc.Database.SqlQuery<object>("UPDATE Kullanici SET Parola='"+ newPsw + "' where KullaniciId= "+ PswList[i].KullaniciId
                          ).ToList();

             }*/

            //var sonuc = Common.HotmailMailSend2();
            // var model = new Sonuc();
            //model.Mesaj = sonuc;
            //TempData["Result"] = sonuc;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> NobetListeGetir(string date)
        {
            try
            {
                if (!string.IsNullOrEmpty(date))
                {
                    var dateArray = date.Split('-');
                    var DBDate = dateArray[2] + '.' + dateArray[1] + '.' + dateArray[0];
                    var DayNumber = Convert.ToInt32(dateArray[2]) - 1;
                    var Month = Convert.ToInt32(dateArray[1]);
                    var Year = Convert.ToInt32(dateArray[0]);
                    var MonthStr = "";
                    //ModelContext mc = new ModelContext();
                    /*if (Month < 10)
                    {
                        MonthStr = "0" + Month;
                    }
                    else
                    {
                        MonthStr = Month.ToString();
                    }*/
                    using (ModelContext mc = new ModelContext())
                    {
                        var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetListe.sql"), Encoding.Default);
                        var NobetList = await mc.Database.SqlQuery<string>(query, new System.Data.SqlClient.SqlParameter("Day", DayNumber)
                            , new SqlParameter("Month", Month),
                             new SqlParameter("Year", Year)
                         ).FirstOrDefaultAsync();
                        JsonResult result = Json(new { State = 1, NobetList = NobetList }, JsonRequestBehavior.AllowGet);
                        result.MaxJsonLength = Int32.MaxValue;
                        return result;
                    }

                }
                else
                {
                    return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public class NobetListe
        {
            public string KullaniciFulAdi { get; set; }
            public int KullaniciId { get; set; }
            public string NobetTarihi { get; set; }
            public string MahalAdi { get; set; }
        }
        public PartialViewResult Menu()
        {
            var model = new MenuModel();

            var sayfaIdler = new List<int>();
            var sayfaAltIdler = new List<int>();
            List<Sayfalar> sayfaList = new List<Sayfalar>();
            ModelContext mc = new ModelContext();
            var query = "";
            if (Session["KullaniciId"] != null)
            {
                int KullaniciId = Convert.ToInt32(Session["KullaniciId"].ToString());

                query = "select * from Kullanici where KullaniciId=@KullaniciId and Aktif=1 ";
                var kullanici = mc.Database.SqlQuery<KullaniciModel>(query, new System.Data.SqlClient.SqlParameter("KullaniciId", KullaniciId)).FirstOrDefault();
                if (kullanici != null)
                {
                    byte rolId = kullanici.RolId;

                    // query = "select * from SayfaYetki where Aktif=1 and RolId=@RolId";
                    query = "select * from Sayfa where SayfaId in(select SayfaId from SayfaYetki where Aktif=1 and RolId=@RolId) and Aktif=1 order by Sira";
                    sayfaList = mc.Database.SqlQuery<Sayfalar>(query, new System.Data.SqlClient.SqlParameter("RolId", rolId)).ToList();
                    /*foreach(SayfaYetkiler t in rolYetkiList)
                    {

                    }*/
                    /*foreach (SayfaYetkiler t in rolYetkiList)
                    {
                       // Sayfalar sayfa = SayfalarBLL.SelectWithId(t.SayfaId);
                        query = "select * from Sayfa where Aktif=1 and SayfaId=@SayfaId";
                        Sayfalar sayfa = mc.Database.SqlQuery<Sayfalar>(query, new System.Data.SqlClient.SqlParameter("SayfaId", t.SayfaId)).FirstOrDefault();

                        if (sayfa != null)
                        {
                            if (sayfa.UstId != null)
                                sayfaIdler.Add(Convert.ToInt32(sayfa.UstId));
                            sayfaAltIdler.Add(t.SayfaId);
                        }
                    }*/
                }
            }
            else // dış kullanıcı ise
            {
                query = "select * from Sayfa where SayfaId in(6,7) and Aktif=1 order by SayfaId,Sira";
                sayfaList = mc.Database.SqlQuery<Sayfalar>(query).ToList();
            }
            model.Sayfaidler = sayfaList;

            return PartialView(model);
        }
        public bool SessionCheck()
        {
            if (Session["KullaniciId"] == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public bool CheckIsPage(int pageId)
        {
            var isSession = SessionCheck();
            if (isSession == false)
                return false;
            ModelContext mc = new ModelContext();
            var query = "select top 1 SayfaId from SayfaYetki where Aktif=1 and RolId=(select RolId from Kullanici Where Aktif=1 and KullaniciId=@KullaniciId) and SayfaId =@SayfaId";
            var sayfaid = mc.Database.SqlQuery<int?>(query, new System.Data.SqlClient.SqlParameter("KullaniciId", Session["KullaniciId"]),
                 new System.Data.SqlClient.SqlParameter("SayfaId", pageId)).FirstOrDefault();
            if (sayfaid != pageId)
                return false;

            return true;
        }
        public ActionResult NobetDegisimTalep()
        {
            if (CheckIsPage(16) == false)
                return RedirectToAction("Login", "Login");
            try
            {
                var model = new NobetDegisimModel();
                var date = DateTime.Now;

                var currentMonth = date.Month;
                var cMonthStr = "";

                //var cMonthInt = Convert.ToInt32(cMonthStr);
                var cYearInt = date.Year;
                var cDay = date.Day;
               /* if (cDay > 25)
                {
                    if (currentMonth == 12)
                    {
                        currentMonth = 1;
                        cYearInt = cYearInt + 1;
                    }
                    else
                    {
                        currentMonth = currentMonth + 1;
                    }
                }*/
                var year = cYearInt.ToString();
                if (currentMonth < 10)
                {
                    cMonthStr = '0' + currentMonth.ToString();
                }
                else
                    cMonthStr = currentMonth.ToString();


                var MahalId = Convert.ToInt32(Session["MahalId"]);

                var listOfParam = new List<SelectListItem>();
                var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
                listOfParam.Add(item1);
                var uid = Convert.ToInt32(Session["KullaniciId"]);

                var mc = new ModelContext();
                var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimSorgula_Yil_Ay_Mahal_Getir.sql"), Encoding.Default);
                var NobetListeGetir = mc.Database.SqlQuery<NobetDegisimDB>(query2,
                     //new System.Data.SqlClient.SqlParameter("Mahal", MahalId)
                     new SqlParameter("MahalId", MahalId),
                     new SqlParameter("Yil", cYearInt),
                     new SqlParameter("Ay", currentMonth)
                 ).FirstOrDefault();
                JavaScriptSerializer js = new JavaScriptSerializer();
                if (NobetListeGetir.NobetListeJson != null)
                {
                    var NobetListeGetirDes = js.Deserialize<List<NobetListe>>(NobetListeGetir.NobetListeJson);

                    ViewBag.ddlNobetListe = DummyList(NobetListeGetirDes, uid, year, cMonthStr);
                    ViewBag.ddlDeDegisiklikYapmakIstenilenListe = DegisiklikYapmakIstenilenListe(NobetListeGetirDes, uid, year, cMonthStr);

                    model.IslemYapanAdi = Session["Rutbe"] + " " + Session["KullaniciAdi"] + " " + Session["KullaniciSoyadi"];
                    model.NobetListeId = NobetListeGetir.NobetListeId;
                    model.MahalId = MahalId;
                    model.AmirId = NobetListeGetir.MahalAmirId;
                    model.AmirAdi = NobetListeGetir.AmirFulAdi;
                    model.AmirEposta = NobetListeGetir.AmirEposta;
                    model.MahalAdi = NobetListeGetir.MahalAdi;

                    model.NobetKidemliId = NobetListeGetir.NobetKidemliId;
                    model.NobetKidemliAdi = NobetListeGetir.NobetKidemliAdi;
                }
                else
                {
                    ViewBag.ddlNobetListe = listOfParam;
                    ViewBag.ddlDeDegisiklikYapmakIstenilenListe = listOfParam;
                }


                //listOfParam.AddRange(NobetListeGetir.Select(t => new SelectListItem { Text = t.NobetTarihi+' '+t.KullaniciFulAdi, Value = t.NobetTarihi.ToString() }));
                /*foreach(var item in NobetListeGetir)
                {
                    if(item.KullaniciId != uid || !item.NobetTarihi.Contains(year+'-'+ cMonthStr))
                    {
                        listOfParam.Add(new SelectListItem {Text = item.NobetTarihi +' '+item.KullaniciFulAdi,Value=item.NobetTarihi,Disabled=true});
                    }
                    else
                    {
                        listOfParam.Add(new SelectListItem { Text = item.NobetTarihi + ' ' + item.KullaniciFulAdi, Value = item.NobetTarihi, Disabled = false });
                    }
                }*/

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("/Shared/Error");
            }

        }
        public IEnumerable<SelectListItem> DummyList(IEnumerable<NobetListe> NobetListeGetir, int uid, string year, string cMonthStr)
        {
            List<SelectListItem> listOfParam = new List<SelectListItem>();
            var listOfParam2 = new List<SelectListItem>();
            var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
            listOfParam2.Add(item1);
            foreach (var item in NobetListeGetir)
            {
                var date = DateTime.Now;
                var NobetTarihiDate = Convert.ToDateTime(item.NobetTarihi);
                if (item.KullaniciId != uid || NobetTarihiDate < date)//!item.NobetTarihi.Contains(year + '-' + cMonthStr))
                {
                    listOfParam2.Add(new SelectListItem() { Text = Convert.ToDateTime(item.NobetTarihi).ToString("dd.MM.yyyy") + " || " + item.KullaniciFulAdi, Value = item.KullaniciId.ToString(), Disabled = true });
                }
                else
                {
                    listOfParam2.Add(new SelectListItem() { Text = Convert.ToDateTime(item.NobetTarihi).ToString("dd.MM.yyyy") + " || " + item.KullaniciFulAdi, Value = item.KullaniciId.ToString(), Disabled = false });
                }
            }

            return listOfParam2;
        }

        public IEnumerable<SelectListItem> DegisiklikYapmakIstenilenListe(IEnumerable<NobetListe> NobetListeGetir, int uid, string year, string cMonthStr)
        {
            List<SelectListItem> listOfParam = new List<SelectListItem>();
            var listOfParam2 = new List<SelectListItem>();
            var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
            listOfParam2.Add(item1);
            foreach (var item in NobetListeGetir)
            {
                var date = DateTime.Now;
                var NobetTarihiDate = Convert.ToDateTime(item.NobetTarihi);
                if (item.KullaniciId != uid && NobetTarihiDate > date)
                {
                    listOfParam2.Add(new SelectListItem() { Text = Convert.ToDateTime(item.NobetTarihi).ToString("dd.MM.yyyy") + " || " + item.KullaniciFulAdi, Value = item.KullaniciId.ToString(), Disabled = false });
                }
                else
                {
                    listOfParam2.Add(new SelectListItem() { Text = Convert.ToDateTime(item.NobetTarihi).ToString("dd.MM.yyyy") + " || " + item.KullaniciFulAdi, Value = item.KullaniciId.ToString(), Disabled = true });
                }
            }

            return listOfParam2;
        }
        public JsonResult NobetDegisimKayit(List<string> data)
        {
            try
            {
                if (CheckIsPage(16) == false)
                {
                    RedirectToAction("Login", "Login");
                    return Json(null);
                }
                //  return RedirectToAction("Login", "Login");

                if (data.Count > 0)
                {
                    var NobetListeId = Convert.ToInt32(data[0]);

                    var EskiTalepEttigiKisininTarihi = Convert.ToDateTime(data[1].Trim());
                    var YeniTalepEttigiKisininTarihi = Convert.ToDateTime(data[2].Trim());
                    var DegisimTipi = Convert.ToInt32(data[3]);
                    var TalepEdenPersonelId = data[4];
                    var TalepEttigiPersonelId = data[5];
                    var YeniTalepEdenKisininTarihi = Convert.ToDateTime(data[6].Trim());
                    var EskiTalepEdenKisininTarihi = Convert.ToDateTime(data[7].Trim());
                    var MahalId = Convert.ToInt32(data[8]);
                    var IslemYapanPersonelId = Convert.ToInt32(Session["KullaniciId"]);
                    var Durum = 0;
                    var AmirId = Convert.ToInt32(data[9]);

                    var KendiAdi = data[10];
                    var BaskaAdi = data[11];
                    var AmirEspota = data[12];

                    var Mazeret = data[13];

                    var mc = new ModelContext();
                    var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalepEkle.sql"), Encoding.Default);
                    {
                        var gonderilecekPersonelId = 0;
                        var mailBaslik = "";
                        var mailAciklamaHtml = "";
                        var onayNo = Common.EncodeStr(Common.RandomString(16));
                        var DurumBilgisiNedir = "";
                        var DurumEncode = "";
                        var AmirEspotaEncode = "";
                        var RedEncode = "";

                        var queryDurumTespit = " declare @AmirMi tinyint;set @AmirMi=0;select @AmirMi=COUNT(MahalId) from Mahal where MahalAmirId=@AmirId and @MahalId=MahalId;select @AmirMi ";
                        var DurumSonuc = mc.Database.SqlQuery<Byte>(queryDurumTespit, new SqlParameter("AmirId", TalepEttigiPersonelId), new SqlParameter("MahalId", MahalId)).FirstOrDefault();
                        if (DurumSonuc == 0)
                            DurumBilgisiNedir = "1";
                        else if (DurumSonuc == 1)
                            DurumBilgisiNedir = "2";

                        if (DegisimTipi == 2)
                        {
                            var TalepEpostaHtml = NobetDegisimEpostaHtml(TalepEttigiPersonelId, DurumBilgisiNedir, AmirEspota, data[2], data[6], KendiAdi, BaskaAdi, onayNo, Mazeret);
                            mailBaslik = "Nöbet Değişim - Karşılıklı Nöbet Değişim";
                            gonderilecekPersonelId = Convert.ToInt32(TalepEttigiPersonelId);
                            /*gonderilecekPersonelId = Convert.ToInt32(TalepEttigiPersonelId);
                            mailBaslik = "Nöbet Değişim - Karşılıklı Nöbet Değişim";
                            DurumEncode = Common.EncodeStr(DurumBilgisiNedir);
                            RedEncode = Common.EncodeStr("3");
                            AmirEspotaEncode = Common.EncodeStr(AmirEspota);
                            var hrml = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"Views/Home/KarsilikliNobetDegisimHtml.cshtml"), Encoding.Default)
                                .Replace("TEPA", KendiAdi)
                                .Replace("TEYT", data[2] + " -> " + data[6])
                                .Replace("TEPPA", BaskaAdi)
                                .Replace("TEYYT", data[6] + " -> " + data[2])
                                .Replace("criptNo", onayNo)
                                .Replace("EncodeDurum", DurumEncode)
                                .Replace("SonucEncode", RedEncode)
                                .Replace("AmirE", AmirEspotaEncode);*/

                            mailAciklamaHtml = TalepEpostaHtml;//hrml;

                            //mailAciklamaHtml = '<table style="width: 100 % "><tr><th>Firstname</th><th>Lastname</th><th>Age</th></tr><tr><td>Jill</td><td>Smith</td><td>50</td></tr><tr><td>Eve</td><td>Jackson</td><td>94</td></tr></table><br/>   ';
                        }
                        else if (DegisimTipi == 1)
                        {
                            gonderilecekPersonelId = AmirId;
                            mailBaslik = "Nöbet Değişim - Tek Gün Nöbet Değişim";
                            mailAciklamaHtml = "";//"<table><tr><th>Firstname</th><th>Lastname</th><th>Age</th></tr><tr><td>Jill</td><td>Smith</td><td>50</td></tr><tr><td>Eve</td><td>Jackson</td><td>94</td></tr></table>";//'<table style="width: 100 % "><tr><th>Firstname</th><th>Lastname</th><th>Age</th></tr><tr><td>Jill</td><td>Smith</td><td>50</td></tr><tr><td>Eve</td><td>Jackson</td><td>94</td></tr></table>';
                        }
                        var gonderilecekMailCekQuery = "select top 1 Eposta from Kullanici where KullaniciId=@KullaniciId";
                        var gonderilecekMailCek = mc.Database.SqlQuery<string>(gonderilecekMailCekQuery, new SqlParameter("KullaniciId", gonderilecekPersonelId)).FirstOrDefault();
                        if (gonderilecekMailCek != null)
                        {
                            var sonuc = Common.HotmailMailSend3(gonderilecekMailCek, mailBaslik, mailAciklamaHtml); //Common.HotmailMailSend3(gonderilecekMailCek, mailBaslik, mailAciklamaHtml);
                            if (sonuc == "1")
                            {

                                var NobetDegisimTalepEkle = mc.Database.SqlQuery<int>(query2,
                                    new SqlParameter("NobetListeJsonId", NobetListeId),
                                     new SqlParameter("EskiTalepEttigiKisininTarihi", EskiTalepEttigiKisininTarihi),
                                     new SqlParameter("YeniTalepEttigiKisininTarihi", YeniTalepEttigiKisininTarihi),
                                     new SqlParameter("DegisimTipi", DegisimTipi),
                                     new SqlParameter("TalepEdenPersonelId", TalepEdenPersonelId),
                                     new SqlParameter("TalepEttigiPersonelId", TalepEttigiPersonelId),
                                     new SqlParameter("YeniTalepEdenKisininTarihi", YeniTalepEdenKisininTarihi),
                                     new SqlParameter("EskiTalepEdenKisininTarihi", EskiTalepEdenKisininTarihi),
                                     new SqlParameter("MahalId", MahalId),
                                     new SqlParameter("IslemYapanPersonelId", IslemYapanPersonelId),
                                     new SqlParameter("AmirId", AmirId),
                                     new SqlParameter("Link", onayNo),
                                     new SqlParameter("Durum", Durum),
                                     new SqlParameter("Mzrt", Mazeret)
                                 ).FirstOrDefault();
                                if (NobetDegisimTalepEkle != 0)
                                {
                                    var aciklama = gonderilecekMailCek + "Personele Mail Atılmıştır!";
                                    var queryNobetTalepDetayEkle = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\TalepDetayEkle.sql"), Encoding.Default);
                                    var dataTalepDetayEkle = mc.Database.SqlQuery<object>(queryNobetTalepDetayEkle, new SqlParameter("Eposta", gonderilecekMailCek)
                                        , new SqlParameter("Link", onayNo)
                                        , new SqlParameter("Aciklama", aciklama)).FirstOrDefault();
                                    /*  var queryDegisim = "update NobetDegisimTalep set Link=@Link where Id=@Id";//"declare @TalepId varchar; select @TalepId=Id from NobetDegisimTalep ";
                                      mc.Database.SqlQuery<object>(queryDegisim, new SqlParameter("Link", onayNo), new SqlParameter("Id", NobetDegisimTalepEkle)).FirstOrDefault();
                                      */
                                    return Json(new { State = 1, id = "Talebiniz Mail Olarak İletilmiştir!", Message = "Başarılı!" }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                    return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                            }

                            else
                                return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                        }
                        //TempData["Result"] = sonuc;


                    }

                    /* else
                         return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);*/
                }
                else
                {
                    return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { State = 2, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public string NobetDegisimEpostaHtml(string TalepEttigiPersonelId, string DurumBilgisiNedir, string AmirEspota, string yeniTarih, string eskiTarih, string KendiAdi, string BaskaAdi, string onayNo, string Mazeret)
        {

            var gonderilecekPersonelId = 0;
            var mailBaslik = "";
            // var mailAciklamaHtml = "";
            // var onayNo = Common.EncodeStr(Common.RandomString(16));
            // var DurumBilgisiNedir = "";
            var DurumEncode = "";
            var AmirEspotaEncode = "";
            var RedEncode = "";

            gonderilecekPersonelId = Convert.ToInt32(TalepEttigiPersonelId);
            mailBaslik = "Nöbet Değişim - Karşılıklı Nöbet Değişim";
            DurumEncode = Common.EncodeStr(DurumBilgisiNedir);
            RedEncode = Common.EncodeStr("3");
            AmirEspotaEncode = Common.EncodeStr(AmirEspota);
            var hrml = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"Views/Home/KarsilikliNobetDegisimHtml.cshtml"), Encoding.Default)
                .Replace("TEPA", KendiAdi)
                .Replace("TEYT", eskiTarih + " -> " + yeniTarih)
                .Replace("TEPPA", BaskaAdi)
                .Replace("TEYYT", yeniTarih + " -> " + eskiTarih)
                .Replace("criptNo", onayNo)
                .Replace("EncodeDurum", DurumEncode)
                .Replace("SonucEncode", RedEncode)
                .Replace("AmirE", AmirEspotaEncode)
                .Replace("Mzrt", Mazeret);
            // mailAciklamaHtml = hrml;
            return hrml;
        }
        public string NobetDegisimEpostaHtmlDoldur(string DurumBilgisiNedir, string onayNo, int NobetDegisimId, string mailBaslik, string GonderilecekEposta)
        {
            var gonderilecekMail = "";
            var gonderilecekPersonelId = 0;
            //var mailBaslik = "";
            // var mailAciklamaHtml = "";
            // var onayNo = Common.EncodeStr(Common.RandomString(16));
            // var DurumBilgisiNedir = "";
            var DurumEncode = "";
            var AmirEspotaEncode = "";
            var RedEncode = "";

            var PersonnelsInfo = GetNobetDegisimTalepProsedurPersonellerGetir(NobetDegisimId);

            if (GonderilecekEposta == "")
                GonderilecekEposta = PersonnelsInfo.TalepEttigiPerEposta;

            //gonderilecekPersonelId = Convert.ToInt32(PersonnelsInfo.Ta.TalepEttigiPersonelId);
            mailBaslik = "Nöbet Değişim - Karşılıklı Nöbet Değişim";
            DurumEncode = Common.EncodeStr("1");//(DurumBilgisiNedir);
            RedEncode = Common.EncodeStr("3");
            var TalepEttigiEspotaEncode = Common.EncodeStr(GonderilecekEposta);
            var html = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"Views/Home/KarsilikliNobetDegisimHtml.cshtml"), Encoding.Default)
                .Replace("TEPA", PersonnelsInfo.TalepEdenPerAdi)
                .Replace("TEYT", PersonnelsInfo.YeniTalepEdenKisininTarihi.ToString("dd.MM.yyyy") + " -> " + PersonnelsInfo.EskiTalepEdenKisininTarihi.ToString("dd.MM.yyyy"))
                .Replace("TEPPA", PersonnelsInfo.TalepEttigiPerAdi)
                .Replace("TEYYT", PersonnelsInfo.EskiTalepEdenKisininTarihi.ToString("dd.MM.yyyy") + " -> " + PersonnelsInfo.YeniTalepEdenKisininTarihi.ToString("dd.MM.yyyy"))
                .Replace("criptNo", onayNo)
                .Replace("EncodeDurum", DurumEncode)
                .Replace("SonucEncode", RedEncode)
                .Replace("AmirE", TalepEttigiEspotaEncode)
                .Replace("NK", PersonnelsInfo.NobetKidemliAdi + " (" + PersonnelsInfo.NobetKidemliOnayAdi + ")")
                .Replace("TalepAmir", PersonnelsInfo.KendiAmirAdi + " (" + PersonnelsInfo.KendiAmirOnayAdi + ")")
                .Replace("EttiA", PersonnelsInfo.TalepAmirAdi + " (" + PersonnelsInfo.TalepEttigiAmirOnayAdi + ")")
                .Replace("OnayMak", PersonnelsInfo.OnayMakamAdi + " (" + PersonnelsInfo.OnayMakamiOnayAdi + ")")

                .Replace("Mzrt", PersonnelsInfo.Mazeret);
            // mailAciklamaHtml = hrml;

            var sonuc = Common.HotmailMailSend3(GonderilecekEposta, mailBaslik, html); //Common.HotmailMailSend3(gonderilecekMailCek, mailBaslik, mailAciklamaHtml);
            if (sonuc == "1")
                gonderilecekMail = GonderilecekEposta;

            return gonderilecekMail;

        }

        //tek nöbet değişim
        public string NobetDegisimTekEpostaHtmlDoldur(string DurumBilgisiNedir, string onayNo, int NobetDegisimId, string mailBaslik, string GonderilecekEposta)
        {
            var gonderilecekMail = "";
            var gonderilecekPersonelId = 0;
            //var mailBaslik = "";
            // var mailAciklamaHtml = "";
            // var onayNo = Common.EncodeStr(Common.RandomString(16));
            // var DurumBilgisiNedir = "";
            var DurumEncode = "";
            var AmirEspotaEncode = "";
            var RedEncode = "";

            var PersonnelsInfo = GetNobetDegisimTalepProsedurPersonellerGetir(NobetDegisimId);

            if (GonderilecekEposta == "")
                GonderilecekEposta = PersonnelsInfo.NobetKidemliPerEposta;
            //talep eden kişi nöbet kıdemlisi ise direk olarak talep eden personelin amirine gönder
            if (GonderilecekEposta == Session["Eposta"].ToString())
            {
                GonderilecekEposta = PersonnelsInfo.KendiAmirEposta;
            }

            //gonderilecekPersonelId = Convert.ToInt32(PersonnelsInfo.Ta.TalepEttigiPersonelId);
            mailBaslik = "Nöbet Değişim - Tek Gün Nöbet Değişim";
            DurumEncode = Common.EncodeStr("1");//(DurumBilgisiNedir);
            RedEncode = Common.EncodeStr("3");
            var TalepEttigiEspotaEncode = Common.EncodeStr(GonderilecekEposta);
            var html = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"Views/Home/KarsilikliNobetDegisimTekHtml.cshtml"), Encoding.Default)
                .Replace("TEPA", PersonnelsInfo.TalepEdenPerAdi)
                .Replace("TEYT", PersonnelsInfo.YeniTalepEdenKisininTarihi.ToString("dd.MM.yyyy"))   // + " -> " + PersonnelsInfo.EskiTalepEdenKisininTarihi.ToString("dd.MM.yyyy"))
                .Replace("TEPPA", PersonnelsInfo.TalepEttigiPerAdi)
                //.Replace("TEYYT", PersonnelsInfo.EskiTalepEdenKisininTarihi.ToString("dd.MM.yyyy") + " -> " + PersonnelsInfo.YeniTalepEdenKisininTarihi.ToString("dd.MM.yyyy"))
                .Replace("criptNo", onayNo)
                .Replace("EncodeDurum", DurumEncode)
                .Replace("SonucEncode", RedEncode)
                .Replace("AmirE", TalepEttigiEspotaEncode)
                .Replace("NK", PersonnelsInfo.NobetKidemliAdi + " (" + PersonnelsInfo.NobetKidemliOnayAdi + ")")
                .Replace("TalepAmir", PersonnelsInfo.KendiAmirAdi + " (" + PersonnelsInfo.KendiAmirOnayAdi + ")")
                .Replace("EttiA", PersonnelsInfo.TalepAmirAdi + " (" + PersonnelsInfo.TalepEttigiAmirOnayAdi + ")")
                .Replace("OnayMak", PersonnelsInfo.OnayMakamAdi + " (" + PersonnelsInfo.OnayMakamiOnayAdi + ")")

                .Replace("Mzrt", PersonnelsInfo.Mazeret);
            // mailAciklamaHtml = hrml;

            var sonuc = Common.HotmailMailSend3(GonderilecekEposta, mailBaslik, html); //Common.HotmailMailSend3(gonderilecekMailCek, mailBaslik, mailAciklamaHtml);
            if (sonuc == "1")
                gonderilecekMail = GonderilecekEposta;

            return gonderilecekMail;

        }

        public int GonderilecekPersonelBelirle()
        {
            //talep eden kişi veya talep ettiği kişi nöbet kıdemlisi olabilir
            // ''  veya '' kendi amiri olabilir
            //2 farklı amir olabilir yada tek amir olabilir
            return 0;
        }
        [HttpGet]
        public ActionResult TalepSonucSayfa(string perid, string onay, string durum, string eposta)
        {
            try
            {
                var str = Common.DecodeStr(durum);
                //var aa =Request.RawUrl;
                var durumInt = Convert.ToInt16(str);
                var EpostaDecode = Common.DecodeStr(eposta);
                var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimOnay_Onayno_Durum.sql"), Encoding.Default);
                //var query = "declare @TalepId int;select @TalepId=Id from NobetDegisimTalep where Link=@Link;   update NobetDegisimTalep set Durum=@Durum where Id=@TalepId";
                var mc = new ModelContext();
                var sonuc = mc.Database.SqlQuery<ReturnMessage>(query, new SqlParameter("Link", onay), new SqlParameter("Durum", durumInt)
                    , new SqlParameter("Eposta", EpostaDecode)).FirstOrDefault();

                if (sonuc.Mesaj != "0")
                {

                    TempData["Result"] = sonuc.Mesaj;
                    if (sonuc.StateId != 2 && sonuc.StateId != 3)
                        if (durumInt == 1)// amir onayı için mail gönder amire
                        {
                            // var gonderilecekMailCekQuery = "select top 1 Eposta from Kullanici where KullaniciId=@KullaniciId";
                            //var gonderilecekMailCek = mc.Database.SqlQuery<string>(gonderilecekMailCekQuery, new SqlParameter("KullaniciId", gonderilecekPersonelId)).FirstOrDefault();
                            /*
                               }
                             */
                            //
                            var queryNobetTalepGetir = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalepSorgula_Amir_Link.sql"), Encoding.Default);
                            var data = mc.Database.SqlQuery<NobetDeigisimSorgula_Amir_Link>(queryNobetTalepGetir, new SqlParameter("Link", onay)).FirstOrDefault();
                            var mailBaslik = "Nöbet Değişim - Karşılıklı Nöbet Değişim";
                            if (data != null)
                            {
                                var DurumBilgisiNedir = "2";
                                var YeniTalepEttigiKisininTarihi = Convert.ToDateTime(data.YeniTalepEttigiKisininTarihi).ToString("dd.MM.yyyy");
                                var YeniTalepEdenKisininTarihi = Convert.ToDateTime(data.YeniTalepEdenKisininTarihi).ToString("dd.MM.yyyy");
                                var TalepEpostaHtml = NobetDegisimEpostaHtml(data.TalepEttigiPersonelId.ToString(), DurumBilgisiNedir, data.AmirEspota,
                                    YeniTalepEttigiKisininTarihi, YeniTalepEdenKisininTarihi, data.KendiAdi, data.BaskaAdi, onay, data.Mazeret);
                                var sonucMail = Common.HotmailMailSend3(EpostaDecode, mailBaslik, TalepEpostaHtml);// Common.HotmailMailSend2.HotmailMailSend3(EpostaDecode, mailBaslik, TalepEpostaHtml);
                                if (sonucMail == "1")
                                {
                                    var aciklama = EpostaDecode + " Nöbetçi Amire Mail Atılmıştır!";
                                    var queryNobetTalepDetayEkle = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\TalepDetayEkle.sql"), Encoding.Default);
                                    var dataTalepDetayEkle = mc.Database.SqlQuery<object>(queryNobetTalepDetayEkle, new SqlParameter("Eposta", EpostaDecode)
                                        , new SqlParameter("Link", onay)
                                        , new SqlParameter("Aciklama", aciklama)).FirstOrDefault();

                                    TempData["Result"] = "Nöbetçi Amire Mail Atılmıştır!";
                                }
                            }
                            else
                            {
                                TempData["Result"] = "İşlem yapılacak talep bulunamadı!";
                            }
                        }
                        else
                        {
                            TempData["Result"] = sonuc.Mesaj;
                        }
                }
                else
                {
                    TempData["Result"] = sonuc.ErrorMessage;
                }

                //TempData["Result"] = "İşlem Tamamlanmıştır!";
                return View();
            }
            catch (Exception ex)
            {

                //throw; 
                TempData["Result"] = ex.Message;
                return View();
            }

        }
        [HttpGet]
        public ActionResult TalepSonucIslemiSayfa(string perid, string onay, string durum, string eposta)
        {
            try
            {
                var str = Common.DecodeStr(durum);
                //var aa =Request.RawUrl;
                var durumInt = Convert.ToInt16(str);
                var EpostaDecode = Common.DecodeStr(eposta);
                var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimOnay_Onayno_Durum.sql"), Encoding.Default);
                //var query = "declare @TalepId int;select @TalepId=Id from NobetDegisimTalep where Link=@Link;   update NobetDegisimTalep set Durum=@Durum where Id=@TalepId";
                var mc = new ModelContext();
                var sonuc = mc.Database.SqlQuery<ReturnMessage>(query, new SqlParameter("Link", onay), new SqlParameter("Durum", durumInt)
                    , new SqlParameter("Eposta", EpostaDecode)).FirstOrDefault();

                if (sonuc.Mesaj != "")
                {

                    TempData["Result"] = sonuc.Mesaj;
                    if (sonuc.StateId == 5)
                    {
                        /* if (durumInt == 1 || durumInt == 2)//  onayı için mail gönder tekrar
                         {*/
                        var mailBaslik = "";
                        mailBaslik = TekMiKarsilikliMiBaslikBelirle(2);
                        var gonderilecekMailCek = NobetDegisimTekEpostaHtmlDoldur("", onay, sonuc.TalepId, mailBaslik, sonuc.SiradakiEpostaGonderilecekPer);

                        if (gonderilecekMailCek != "")
                        {
                            /*var aciklama = gonderilecekMailCek + "Personele E-Posta Gönderilmiştir!";
                            var queryNobetTalepDetayEkle = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\TalepDetayEkle.sql"), Encoding.Default);
                            var dataTalepDetayEkle = mc.Database.SqlQuery<object>(queryNobetTalepDetayEkle, new SqlParameter("Eposta", gonderilecekMailCek)
                                , new SqlParameter("Link", onay)
                                , new SqlParameter("Aciklama", aciklama)).FirstOrDefault();*/

                            /*  var queryDegisim = "update NobetDegisimTalep set Link=@Link where Id=@Id";//"declare @TalepId varchar; select @TalepId=Id from NobetDegisimTalep ";
                              mc.Database.SqlQuery<object>(queryDegisim, new SqlParameter("Link", onayNo), new SqlParameter("Id", NobetDegisimTalepEkle)).FirstOrDefault();
                              */

                            TempData["Result"] = sonuc.Mesaj + " || " + gonderilecekMailCek + "E-Posta Gönderildi."; //her işlemde talep eden personele de eposta gönder...

                            //  return Json(new { State = 1, id = "Talebiniz Mail Olarak İletilmiştir!", Message = "Başarılı!" }, JsonRequestBehavior.AllowGet);

                        }
                        /*else
                            return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);*/
                        /* }
                         else
                         {
                             TempData["Result"] = sonuc.Mesaj;
                         }*/
                    }
                    if (sonuc.StateId != 5)
                    {
                        TempData["Result"] = sonuc.Mesaj;
                    }
                }
                else
                {
                    TempData["Result"] = "Hata oluştu!";//sonuc.ErrorMessage;
                }

                //TempData["Result"] = "İşlem Tamamlanmıştır!";
                return View();
            }
            catch (Exception ex)
            {

                //throw; 
                TempData["Result"] = ex.Message;
                return View();
            }

        }
        [HttpGet]
        public ActionResult TekTalepSonucIslemiSayfa(string perid, string onay, string durum, string eposta)
        {
            try
            {
                var str = Common.DecodeStr(durum);
                //var aa =Request.RawUrl;
                var durumInt = Convert.ToInt16(str);
                var EpostaDecode = Common.DecodeStr(eposta);
                var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimOnay_Tek.sql"), Encoding.Default);
                //var query = "declare @TalepId int;select @TalepId=Id from NobetDegisimTalep where Link=@Link;   update NobetDegisimTalep set Durum=@Durum where Id=@TalepId";
                var mc = new ModelContext();
                var sonuc = mc.Database.SqlQuery<ReturnMessage>(query, new SqlParameter("Link", onay), new SqlParameter("Durum", durumInt)
                    , new SqlParameter("Eposta", EpostaDecode)).FirstOrDefault();

                if (sonuc.Mesaj != "")
                {

                    TempData["Result"] = sonuc.Mesaj;
                    if (sonuc.StateId == 5)
                    {
                        /* if (durumInt == 1 || durumInt == 2)//  onayı için mail gönder tekrar
                         {*/
                        var mailBaslik = "";
                        mailBaslik = TekMiKarsilikliMiBaslikBelirle(1);
                        var gonderilecekMailCek = NobetDegisimEpostaHtmlDoldur("", onay, sonuc.TalepId, mailBaslik, sonuc.SiradakiEpostaGonderilecekPer);

                        if (gonderilecekMailCek != "")
                        {
                            /*var aciklama = gonderilecekMailCek + "Personele E-Posta Gönderilmiştir!";
                            var queryNobetTalepDetayEkle = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\TalepDetayEkle.sql"), Encoding.Default);
                            var dataTalepDetayEkle = mc.Database.SqlQuery<object>(queryNobetTalepDetayEkle, new SqlParameter("Eposta", gonderilecekMailCek)
                                , new SqlParameter("Link", onay)
                                , new SqlParameter("Aciklama", aciklama)).FirstOrDefault();*/

                            /*  var queryDegisim = "update NobetDegisimTalep set Link=@Link where Id=@Id";//"declare @TalepId varchar; select @TalepId=Id from NobetDegisimTalep ";
                              mc.Database.SqlQuery<object>(queryDegisim, new SqlParameter("Link", onayNo), new SqlParameter("Id", NobetDegisimTalepEkle)).FirstOrDefault();
                              */

                            TempData["Result"] = sonuc.Mesaj + " || " + gonderilecekMailCek + "E-Posta Gönderildi."; //her işlemde talep eden personele de eposta gönder...



                            //  return Json(new { State = 1, id = "Talebiniz Mail Olarak İletilmiştir!", Message = "Başarılı!" }, JsonRequestBehavior.AllowGet);

                        }
                        /*else
                            return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);*/
                        /* }
                         else
                         {
                             TempData["Result"] = sonuc.Mesaj;
                         }*/
                    }
                    if (sonuc.StateId != 5)
                    {
                        TempData["Result"] = sonuc.Mesaj;
                    }
                }
                else
                {
                    TempData["Result"] = "Hata oluştu!";//sonuc.ErrorMessage;
                }

                //TempData["Result"] = "İşlem Tamamlanmıştır!";
                return View();
            }
            catch (Exception ex)
            {

                //throw; 
                TempData["Result"] = ex.Message;
                return View();
            }

        }
        [HttpPost]
        public async Task<JsonResult> NobetDegisimListe()
        {
            try
            {
                var kid = Convert.ToInt32(Session["KullaniciId"]);
                using (ModelContext mc = new ModelContext())
                {
                    var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalepListe.sql"), Encoding.Default);
                    var NobetList = await mc.Database.SqlQuery<string>(query, new SqlParameter("TalepEdenPersonelId", kid)).FirstOrDefaultAsync();
                    JsonResult result = Json(new { State = 1, NobetList = NobetList }, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = Int32.MaxValue;
                    return result;
                }

                //                return Json(new { State = 2, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { State = 2, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult NobetDegisimKaydetVeMailGonder(List<string> data)
        {
            try
            {
                if (CheckIsPage(16) == false)
                {
                    RedirectToAction("Login", "Login");
                    return Json(null);
                }
                if (data.Count > 0)
                {
                    var NobetListeId = Convert.ToInt32(data[0]);

                    var EskiTalepEttigiKisininTarihi = Convert.ToDateTime(data[1].Trim());
                    var YeniTalepEttigiKisininTarihi = Convert.ToDateTime(data[2].Trim());
                    var DegisimTipi = Convert.ToInt32(data[3]);
                    var TalepEdenPersonelId = data[4];
                    var TalepEttigiPersonelId = data[5];
                    var YeniTalepEdenKisininTarihi = Convert.ToDateTime(data[6].Trim());
                    var EskiTalepEdenKisininTarihi = Convert.ToDateTime(data[7].Trim());
                    var MahalId = Convert.ToInt32(data[8]);
                    var IslemYapanPersonelId = Convert.ToInt32(Session["KullaniciId"]);
                    var Durum = 0;
                    var KendiAmirId = Convert.ToInt32(data[9]);
                    var TalepAmirId = Convert.ToInt32(data[10]);
                    var OnayMakamId = Convert.ToInt32(data[11]);

                    var KendiAdi = data[12];
                    var BaskaAdi = data[13];
                    //var AmirEspota = data[12];

                    var Mazeret = data[14];

                    var NobetKidemliId = Convert.ToInt32(data[15]);

                    var mc = new ModelContext();

                    /////
                    ///

                    var TarihKontrol = new DateTime();
                    //------------------------------------- önemli
                    //önceden işlemi tamamlanmamış talep var mı bak
                    TarihKontrol = EskiTalepEdenKisininTarihi;
                    var mevcutTarihKontrol = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalep_MevcutKontrol.sql"), Encoding.Default);
                    var mevcutTarihKontrolResult = mc.Database.SqlQuery<DegisimKontrolPer>(mevcutTarihKontrol, new SqlParameter("Tarih", TarihKontrol)
                        , new SqlParameter("MahalId", MahalId)
                            ).FirstOrDefault();
                    if (mevcutTarihKontrolResult != null)
                    {
                        return Json(new { State = 2, Message = "Nöbet Değişim Tarihini Değişirmeniz gerekmektedir. " + mevcutTarihKontrolResult.IslemYapanPerAdi + " personel önceden talep açmıştır.! " + mevcutTarihKontrolResult.IslemTarihi.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TarihKontrol = EskiTalepEttigiKisininTarihi;
                        mevcutTarihKontrol = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalep_MevcutKontrol.sql"), Encoding.Default);
                        mevcutTarihKontrolResult = mc.Database.SqlQuery<DegisimKontrolPer>(mevcutTarihKontrol, new SqlParameter("Tarih", TarihKontrol)
                            , new SqlParameter("MahalId", MahalId)
                               ).FirstOrDefault();
                    }
                    if (mevcutTarihKontrolResult != null)
                        return Json(new { State = 2, Message = "Nöbet Değişim Tarihini Değişirmeniz gerekmektedir. " + mevcutTarihKontrolResult.IslemYapanPerAdi + " personel önceden talep açmıştır.! " + mevcutTarihKontrolResult.IslemTarihi.ToString() }, JsonRequestBehavior.AllowGet);


                    ///


                    var queryTalepEkle = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalepEkle.sql"), Encoding.Default);
                    {
                        // var gonderilecekPersonelId = 0;

                        // var gonderilecekMailCekQuery = "select top 1 Eposta from Kullanici where KullaniciId=@KullaniciId";
                        // var gonderilecekMailCek = mc.Database.SqlQuery<string>(gonderilecekMailCekQuery, new SqlParameter("KullaniciId", gonderilecekPersonelId)).FirstOrDefault();
                        /*if (gonderilecekMailCek != null)
                        {*/

                        /*  if (sonuc == "1")
                          {*/
                        var onayNo = Common.EncodeStr(Common.RandomString(16));

                        var NobetDegisimTalepEkle = mc.Database.SqlQuery<int>(queryTalepEkle,
                            new SqlParameter("NobetListeJsonId", NobetListeId),
                             new SqlParameter("EskiTalepEttigiKisininTarihi", EskiTalepEttigiKisininTarihi),
                             new SqlParameter("YeniTalepEttigiKisininTarihi", YeniTalepEttigiKisininTarihi),
                             new SqlParameter("DegisimTipi", DegisimTipi),
                             new SqlParameter("TalepEdenPersonelId", TalepEdenPersonelId),
                             new SqlParameter("TalepEttigiPersonelId", TalepEttigiPersonelId),
                             new SqlParameter("YeniTalepEdenKisininTarihi", YeniTalepEdenKisininTarihi),
                             new SqlParameter("EskiTalepEdenKisininTarihi", EskiTalepEdenKisininTarihi),
                             new SqlParameter("MahalId", MahalId),
                             new SqlParameter("IslemYapanPersonelId", IslemYapanPersonelId),
                             //new SqlParameter("AmirId", AmirId),
                             new SqlParameter("Link", onayNo),
                             new SqlParameter("Durum", Durum),
                             new SqlParameter("Mazeret", Mazeret)
                             , new SqlParameter("NobetKidemliId", NobetKidemliId)
                             , new SqlParameter("KendiAmirId", KendiAmirId)
                             , new SqlParameter("TalepEttigiAmirId", TalepAmirId)
                             , new SqlParameter("OnayMakamiId", OnayMakamId)
                         ).FirstOrDefault();
                        if (NobetDegisimTalepEkle != 0)
                        {
                            //var mailAciklamaHtml = "";
                            //var DurumBilgisiNedir = "";
                            //var DurumEncode = "";
                            //var AmirEspotaEncode = "";
                            //var RedEncode = "";

                            var mailBaslik = "";
                            mailBaslik = TekMiKarsilikliMiBaslikBelirle(DegisimTipi);
                            
                            var gonderilecekMailCek = NobetDegisimEpostaHtmlDoldur("", onayNo, NobetDegisimTalepEkle, mailBaslik, "");

                            if (gonderilecekMailCek != "")
                            {
                                var aciklama = gonderilecekMailCek + "Personele E-Posta Atılmıştır!";
                                var queryNobetTalepDetayEkle = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\TalepDetayEkle.sql"), Encoding.Default);
                                var dataTalepDetayEkle = mc.Database.SqlQuery<object>(queryNobetTalepDetayEkle, new SqlParameter("Eposta", gonderilecekMailCek)
                                    //, new SqlParameter("IslemYapanPersonelId", Convert.ToInt32(Session["KullaniciId"]))
                                    , new SqlParameter("Link", onayNo)
                                    , new SqlParameter("Aciklama", aciklama)).FirstOrDefault();
                                /*  var queryDegisim = "update NobetDegisimTalep set Link=@Link where Id=@Id";//"declare @TalepId varchar; select @TalepId=Id from NobetDegisimTalep ";
                                  mc.Database.SqlQuery<object>(queryDegisim, new SqlParameter("Link", onayNo), new SqlParameter("Id", NobetDegisimTalepEkle)).FirstOrDefault();
                                  */
                                return Json(new { State = 1, id = "Talebiniz Mail Olarak İletilmiştir!", Message = "Başarılı!" }, JsonRequestBehavior.AllowGet);

                            }
                            else
                                return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);


                        }
                        else
                            return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                        /*}

                        else
                            return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                        */

                        // }
                        /*else
                        {
                            return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                        }*/
                        //TempData["Result"] = sonuc;


                    }

                    /* else
                         return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);*/
                }
                else
                {
                    return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { State = 2, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult NobetTarihKontrol(string tarih,string MahalId)
        {
            try
            {
                var checkDate = Convert.ToDateTime(tarih.Trim());//.ToString("yyyy-MM-dd");
                var mc = new ModelContext();
                var mevcutTarihKontrol = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalep_MevcutKontrol.sql"), Encoding.Default);
                var mevcutTarihKontrolResult = mc.Database.SqlQuery<DegisimKontrolPer>(mevcutTarihKontrol, new SqlParameter("Tarih", checkDate)
                    , new SqlParameter("MahalId", Convert.ToInt32(MahalId))
                        ).FirstOrDefault();
                if (mevcutTarihKontrolResult != null)
                {
                    return Json(new { State = 2, Message = "Nöbet Değişim Tarihini Değişirmeniz gerekmektedir. "+ mevcutTarihKontrolResult.IslemYapanPerAdi + " personel önceden talep açmıştır.! İşlem Tarihi: " + mevcutTarihKontrolResult.IslemTarihi.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { State = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
            }
        }
        public class DegisimKontrolPer
        {
            public string IslemYapanPerAdi { get; set; }
            public DateTime? IslemTarihi { get; set; }
        }
        public JsonResult NobetTekDegisimKaydetVeMailGonder(List<string> data)
        {
            try
            {
                if (CheckIsPage(16) == false)
                {
                    RedirectToAction("Login", "Login");
                    return Json(null);
                }
                if (data.Count > 0)
                {
                    var NobetListeId = Convert.ToInt32(data[0]);

                    var DegisimTipi = Convert.ToInt32(data[1]);
                    //var TekNobetTarihiKendisi = Convert.ToDateTime(data[1].Trim());
                    var TalepEttigiPerId = Convert.ToInt32(data[2]);
                    var MahalId = Convert.ToInt32(data[3]);

                    //var TekTalepEttigiPersonelId = data[5];

                    var Durum = 0;
                    var KendiAmirId = Convert.ToInt32(data[4]);
                    var TalepAmirId = Convert.ToInt32(data[5]);
                    var OnayMakamId = Convert.ToInt32(data[6]);

                    var Mazeret = data[7];

                    var NobetKidemliId = Convert.ToInt32(data[8]);

                    var YeniTalepEdilenTarih = Convert.ToDateTime(data[9].Trim());
                    var mazeretSTR = data[10];
                    //var TekTalepEttigiPersonelId = data[10];

                    var IslemYapanPersonelId = Convert.ToInt32(Session["KullaniciId"]);

                    var mc = new ModelContext();

                    ///

                    var TarihKontrol = new DateTime();
                    //------------------------------------- önemli
                    //önceden işlemi tamamlanmamış talep var mı bak
                    TarihKontrol = YeniTalepEdilenTarih;
                    var mevcutTarihKontrol = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalep_MevcutKontrol.sql"), Encoding.Default);
                    var mevcutTarihKontrolResult = mc.Database.SqlQuery<DegisimKontrolPer>(mevcutTarihKontrol, new SqlParameter("Tarih", TarihKontrol)
                        , new SqlParameter("MahalId", MahalId)
                            ).FirstOrDefault();
                    if (mevcutTarihKontrolResult != null)
                    {
                        return Json(new { State = 2, Message = "Nöbet Değişim Tarihini Değişirmeniz gerekmektedir. " + mevcutTarihKontrolResult.IslemYapanPerAdi + " personel önceden talep açmıştır.! " + mevcutTarihKontrolResult.IslemTarihi.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    /*else
                    {
                        TarihKontrol = EskiTalepEttigiKisininTarihi;
                        mevcutTarihKontrol = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalep_MevcutKontrol.sql"), Encoding.Default);
                        mevcutTarihKontrolResult = mc.Database.SqlQuery<DegisimKontrolPer>(mevcutTarihKontrol, new SqlParameter("Tarih", TarihKontrol)
                            , new SqlParameter("MahalId", MahalId)
                               ).FirstOrDefault();
                    }
                    if (mevcutTarihKontrolResult != null)
                        return Json(new { State = 2, Message = "Nöbet Değişim Tarihini Değişirmeniz gerekmektedir. " + mevcutTarihKontrolResult.IslemYapanPerAdi + " personel önceden talep açmıştır.! " + mevcutTarihKontrolResult.IslemTarihi.ToString() }, JsonRequestBehavior.AllowGet);
                    */

                    ///

                    var queryTalepEkle = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalepEkle.sql"), Encoding.Default);
                    {
                        // var gonderilecekPersonelId = 0;

                        // var gonderilecekMailCekQuery = "select top 1 Eposta from Kullanici where KullaniciId=@KullaniciId";
                        // var gonderilecekMailCek = mc.Database.SqlQuery<string>(gonderilecekMailCekQuery, new SqlParameter("KullaniciId", gonderilecekPersonelId)).FirstOrDefault();
                        /*if (gonderilecekMailCek != null)
                        {*/

                        /*  if (sonuc == "1")
                          {*/
                        var onayNo = Common.EncodeStr(Common.RandomString(16));

                        var NobetDegisimTalepEkle = mc.Database.SqlQuery<int>(queryTalepEkle,
                            new SqlParameter("NobetListeJsonId", NobetListeId),
                             new SqlParameter("EskiTalepEttigiKisininTarihi", string.Empty),
                             new SqlParameter("YeniTalepEttigiKisininTarihi", string.Empty),
                             new SqlParameter("DegisimTipi", DegisimTipi),
                             new SqlParameter("TalepEdenPersonelId", IslemYapanPersonelId),
                             new SqlParameter("TalepEttigiPersonelId", TalepEttigiPerId),
                             new SqlParameter("YeniTalepEdenKisininTarihi", YeniTalepEdilenTarih),
                             new SqlParameter("EskiTalepEdenKisininTarihi", string.Empty),
                             new SqlParameter("MahalId", MahalId),
                             new SqlParameter("IslemYapanPersonelId", IslemYapanPersonelId),
                             //new SqlParameter("AmirId", AmirId),
                             new SqlParameter("Link", onayNo),
                             new SqlParameter("Durum", Durum),
                             new SqlParameter("Mazeret", mazeretSTR)
                             , new SqlParameter("NobetKidemliId", NobetKidemliId)
                             , new SqlParameter("KendiAmirId", KendiAmirId)
                             , new SqlParameter("TalepEttigiAmirId", TalepAmirId)
                             , new SqlParameter("OnayMakamiId", OnayMakamId)
                         ).FirstOrDefault();
                        if (NobetDegisimTalepEkle != 0)
                        {
                            //var mailAciklamaHtml = "";
                            //var DurumBilgisiNedir = "";
                            //var DurumEncode = "";
                            //var AmirEspotaEncode = "";
                            //var RedEncode = "";

                            var mailBaslik = "";
                            mailBaslik = TekMiKarsilikliMiBaslikBelirle(DegisimTipi);
                            //var queryNobetTalepDetayEkle = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\TalepDetayEkle.sql"), Encoding.Default);

                            var gonderilecekMailCek = NobetDegisimTekEpostaHtmlDoldur("", onayNo, NobetDegisimTalepEkle, mailBaslik, "");

                            if (gonderilecekMailCek != "")
                            {
                                var aciklama = gonderilecekMailCek + "Personele E-Posta Atılmıştır!";
                                var queryNobetTalepDetayEkle = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\TalepDetayEkle.sql"), Encoding.Default);
                                var dataTalepDetayEkle = mc.Database.SqlQuery<object>(queryNobetTalepDetayEkle, new SqlParameter("Eposta", gonderilecekMailCek)
                                    , new SqlParameter("Link", onayNo)
                                    , new SqlParameter("Aciklama", aciklama)).FirstOrDefault();
                                /*  var queryDegisim = "update NobetDegisimTalep set Link=@Link where Id=@Id";//"declare @TalepId varchar; select @TalepId=Id from NobetDegisimTalep ";
                                  mc.Database.SqlQuery<object>(queryDegisim, new SqlParameter("Link", onayNo), new SqlParameter("Id", NobetDegisimTalepEkle)).FirstOrDefault();
                                  */
                                return Json(new { State = 1, id = "Talebiniz Mail Olarak İletilmiştir!", Message = "Başarılı!" }, JsonRequestBehavior.AllowGet);

                            }
                            else
                                return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);


                        }
                        else
                            return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                        /*}

                        else
                            return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                        */

                        // }
                        /*else
                        {
                            return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                        }*/
                        //TempData["Result"] = sonuc;


                    }

                    /* else
                         return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);*/
                }
                else
                {
                    return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        public ProsedurPersonellerModel GetNobetDegisimTalepProsedurPersonellerGetir(int NobetDegisimTalepId)//(int NobetKidemliId, int KendiAmirId, int TalepAmirId, int OnayMakamId)
        {
            var mc = new ModelContext();
            var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetDegisimTalep_ProsedurPersonelListe.sql"), Encoding.Default);
            var getResult = mc.Database.SqlQuery<ProsedurPersonellerModel>(query,
                new SqlParameter("NobetDegisimTalepId", NobetDegisimTalepId)
                   /*new SqlParameter("NobetKidemliId", NobetKidemliId)
                   , new SqlParameter("KendiAmirId", KendiAmirId)
                    , new SqlParameter("TalepAmirId", TalepAmirId)
                     , new SqlParameter("OnayMakamId", OnayMakamId)*/
                   ).FirstOrDefault();

            return getResult;
        }
        public string TekMiKarsilikliMiBaslikBelirle(int DegisimTipi)//,int NobetDegisimTalepId)
        {
            var mailBaslik = "";
            //var gonderilecekPersonelId = 0;
            if (DegisimTipi == 2)
            {
                //var queryTalepEttigiPerEposta = "select Eposta from Kullanici Where Eposta=@Eposta";
                //var personelInfo = GetNobetDegisimTalepProsedurPersonellerGetir(NobetDegisimTalepId);
                //var TalepEpostaHtml = NobetDegisimEpostaHtml(TalepEttigiPersonelId, DurumBilgisiNedir, AmirEspota, data[2], data[6], KendiAdi, BaskaAdi, onayNo, Mazeret);
                mailBaslik = "Nöbet Değişim - Karşılıklı Nöbet Değişim";
                //gonderilecekPersonelId = Convert.ToInt32(TalepEttigiPersonelId);
                /*gonderilecekPersonelId = Convert.ToInt32(TalepEttigiPersonelId);
                mailBaslik = "Nöbet Değişim - Karşılıklı Nöbet Değişim";
                DurumEncode = Common.EncodeStr(DurumBilgisiNedir);
                RedEncode = Common.EncodeStr("3");
                AmirEspotaEncode = Common.EncodeStr(AmirEspota);
                var hrml = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"Views/Home/KarsilikliNobetDegisimHtml.cshtml"), Encoding.Default)
                    .Replace("TEPA", KendiAdi)
                    .Replace("TEYT", data[2] + " -> " + data[6])
                    .Replace("TEPPA", BaskaAdi)
                    .Replace("TEYYT", data[6] + " -> " + data[2])
                    .Replace("criptNo", onayNo)
                    .Replace("EncodeDurum", DurumEncode)
                    .Replace("SonucEncode", RedEncode)
                    .Replace("AmirE", AmirEspotaEncode);*/

                // mailAciklamaHtml = TalepEpostaHtml;//hrml;

                //mailAciklamaHtml = '<table style="width: 100 % "><tr><th>Firstname</th><th>Lastname</th><th>Age</th></tr><tr><td>Jill</td><td>Smith</td><td>50</td></tr><tr><td>Eve</td><td>Jackson</td><td>94</td></tr></table><br/>   ';
            }
            else if (DegisimTipi == 1)
            {
                // gonderilecekPersonelId = AmirId;
                mailBaslik = "Nöbet Değişim - Tek Gün Nöbet Değişim";
                // mailAciklamaHtml = "";//"<table><tr><th>Firstname</th><th>Lastname</th><th>Age</th></tr><tr><td>Jill</td><td>Smith</td><td>50</td></tr><tr><td>Eve</td><td>Jackson</td><td>94</td></tr></table>";//'<table style="width: 100 % "><tr><th>Firstname</th><th>Lastname</th><th>Age</th></tr><tr><td>Jill</td><td>Smith</td><td>50</td></tr><tr><td>Eve</td><td>Jackson</td><td>94</td></tr></table>';
            }
            else
            {
                mailBaslik = "";
            }
            return mailBaslik;
        }
    }
    public class ProsedurPersonellerModel
    {
        public string TalepEdenPerAdi { get; set; }
        public string TalepEttigiPerAdi { get; set; }
        public string TalepEttigiPerEposta { get; set; }
        public string NobetKidemliAdi { get; set; }
        public string NobetKidemliPerEposta { get; set; }
        public string KendiAmirEposta { get; set; }
        public string KendiAmirAdi { get; set; }
        public string TalepAmirAdi { get; set; }
        public string OnayMakamAdi { get; set; }
        public string TalepEttigiPersonelOnayAdi { get; set; }
        public string NobetKidemliOnayAdi { get; set; }
        public string KendiAmirOnayAdi { get; set; }
        public string TalepEttigiAmirOnayAdi { get; set; }
        public string OnayMakamiOnayAdi { get; set; }
        public DateTime EskiTalepEdenKisininTarihi { get; set; }
        public DateTime YeniTalepEdenKisininTarihi { get; set; }
        public string Mazeret { get; set; }

        /*public int NobetKidemliId { get; set; }
        public int KendiAmirId { get; set; }
        public int TalepAmirId { get; set; }
        public int OnayMakamId { get; set; }*/
    }
    public class NobetDeigisimSorgula_Amir_Link
    {
        public int TalepEttigiPersonelId { get; set; }
        public string AmirEspota { get; set; }
        public DateTime YeniTalepEttigiKisininTarihi { get; set; }
        public DateTime YeniTalepEdenKisininTarihi { get; set; }
        public string KendiAdi { get; set; }
        public string BaskaAdi { get; set; }
        public string Mazeret { get; set; }
    }
    public class ReturnMessage
    {
        public string Mesaj { get; set; }
        public string ErrorMessage { get; set; }
        public int StateId { get; set; }
        public string SiradakiEpostaGonderilecekPer { get; set; }
        public int TalepId { get; set; }
    }

    public class NobetDegisimDB
    {
        public string NobetListeJson { get; set; }
        public int NobetListeId { get; set; }
        public string MahalAdi { get; set; }
        public string AmirFulAdi { get; set; }
        public int MahalAmirId { get; set; }
        public string AmirEposta { get; set; }
        public int NobetKidemliId { get; set; }
        public string NobetKidemliAdi { get; set; }
    }
    public class CustomSelectItem : SelectListItem
    {
        public bool Enabled { get; set; }
    }

    public static class CustomHtmlHelpers
    {
        public static MvcHtmlString MyDropDownList(this HtmlHelper html, IEnumerable<CustomSelectItem> selectList)
        {
            var selectDoc = XDocument.Parse(html.DropDownList("", (IEnumerable<SelectListItem>)selectList).ToString());

            var options = from XElement el in selectDoc.Element("select").Descendants()
                          select el;

            foreach (var item in options)
            {
                var itemValue = item.Attribute("value");
                if (!selectList.Where(x => x.Value == itemValue.Value).Single().Enabled)
                    item.SetAttributeValue("disabled", "disabled");
            }

            // rebuild the control, resetting the options with the ones you modified
            selectDoc.Root.ReplaceNodes(options.ToArray());
            return MvcHtmlString.Create(selectDoc.ToString());
        }
    }
    /*public class NobetListeGetir
    {
        //public int KullaniciId
    }*/
}