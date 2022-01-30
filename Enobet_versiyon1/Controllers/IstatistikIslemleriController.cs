using Enobet_versiyon1.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Enobet_versiyon1.Controllers
{
    public class IstatistikIslemleriController : Controller
    {
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
        public List<MahalModel> GetMahalListDDL()
        {
            ModelContext mc = new ModelContext();
            var dataList = new List<MahalModel>();
            var rolid = Convert.ToInt32(Session["RolId"].ToString());
            var userid = Convert.ToInt32(Session["KullaniciId"].ToString());
            var mahalId = Convert.ToInt32(Session["MahalId"].ToString());
            if (rolid == 1)
                dataList = mc.Database.SqlQuery<MahalModel>("select M.*,K.KullaniciId As MahalAmirId,KLSinif.Adi+' '+ KLRutbe.Adi+' '+ K.KullaniciAdi+' '+K.KullaniciSoyadi As AmirFulAdi from Mahal M" +
  " left join Kullanici K on K.KullaniciId = M.MahalAmirId" +
  " left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif = 1" +
  " left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1" +
  " where M.Aktif = 1").ToList();

            //("Select * from Mahal where Aktif=1").ToList();
            if (rolid == 3)
                dataList = mc.Database.SqlQuery<MahalModel>("select M.*,K.KullaniciId As MahalAmirId,KLSinif.Adi+' '+ KLRutbe.Adi+' '+ K.KullaniciAdi+' '+K.KullaniciSoyadi As AmirFulAdi from Mahal M" +
  " left join Kullanici K on K.KullaniciId = M.MahalAmirId" +
  " left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif = 1" +
  " left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1" +
  " where M.Aktif = 1 and M.MahalId=@MahalId", new SqlParameter("KullaniciId", userid), new SqlParameter("MahalId", mahalId)).ToList();
            //in(select MahalId from NobetKidemli where KullaniciId=@KullaniciId)

            //("select * from Mahal where MahalId in(select MahalId from NobetKidemli where KullaniciId=@KullaniciId) and Aktif=1", new SqlParameter("KullaniciId", userid)).ToList();

            return dataList;
        }
        // GET: Istatistik
        public ActionResult KullaniciIstatistik()
        {
            try
            {
                if (CheckIsPage(17) == false)
                    return RedirectToAction("Login", "Login");

                var model = new KullaniciIstatistikModel();
                ///mahallist ---
                var mahallist = GetMahalListDDL();
                //var mahalDataList = mc.Database.SqlQuery<MahalModel>("Select * from Mahal", Common.GetDBNullOrValue<int>(5)).ToList();
                //  var mahalModelList = new List<MahalModel>();
                model.MahalList.AddRange(mahallist);

                var listOfParam = new List<SelectListItem>();
                var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
                listOfParam.Add(item1);
                var list = mahallist;//mahalDataList; //RolBLL.Select(exp).ToList();
                listOfParam.AddRange(list.Select(t => new SelectListItem { Text = t.MahalAdi.ToString(), Value = t.MahalId.ToString() }));
                ViewBag.ddlMahalId = listOfParam;

                return View("KullaniciIstatistik", model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("/Shared/Error");
            }

        }
        public ActionResult PersonelIstatistik()
        {
            try
            {
                if (CheckIsPage(17) == false)
                    return RedirectToAction("Login", "Login");

                var model = new KullaniciIstatistikModel();
                ///mahallist ---
                var mahallist = GetMahalListDDL();
                //var mahalDataList = mc.Database.SqlQuery<MahalModel>("Select * from Mahal", Common.GetDBNullOrValue<int>(5)).ToList();
                //  var mahalModelList = new List<MahalModel>();
                model.MahalList.AddRange(mahallist);

                var listOfParam = new List<SelectListItem>();
                var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
                listOfParam.Add(item1);
                var list = mahallist;//mahalDataList; //RolBLL.Select(exp).ToList();
                listOfParam.AddRange(list.Select(t => new SelectListItem { Text = t.MahalAdi.ToString(), Value = t.MahalId.ToString() }));
                ViewBag.ddlMahalId = listOfParam;

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("/Shared/Error");
            }
        }

        public ActionResult PersonelMazeret()
        {
            try
            {
                if (CheckIsPage(18) == false)
                    return RedirectToAction("Login", "Login");



                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("/Shared/Error");
            }
        }

        [HttpPost]
        public async Task<JsonResult> PersonelNobetListTable(string month, string year)
        {
            try
            {
                if (String.IsNullOrEmpty(Session["KullaniciId"].ToString()))
                    return Json(new { State = 3, Message = "Tekrar Giriş Yapın!" }, JsonRequestBehavior.AllowGet);

                var kid = Convert.ToInt32(Session["KullaniciId"]);
                var Month = Convert.ToInt32(month);
                var Year = Convert.ToInt32(year);
                var MahalId = Convert.ToInt32(Session["MahalId"]);

                using (ModelContext mc = new ModelContext())
                {
                    var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\SahsiNobetListe_Getir.sql"), Encoding.Default);
                    var NobetList = await mc.Database.SqlQuery<string>(query,
                        new SqlParameter("KullaniciId", kid)
                        , new SqlParameter("Month", Month)
                        , new SqlParameter("Year", Year)
                        , new SqlParameter("MahalId", MahalId)
                        ).FirstOrDefaultAsync();


                    var queryHaftalikAnaliz = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetMahalAylik_HaftaAnalizHesabi.sql"), Encoding.Default);
                    var HaftalikAnalizList = await mc.Database.SqlQuery<MahalAylik_HaftalikAnaliz>(queryHaftalikAnaliz
                        , new SqlParameter("MahalId", MahalId)
                        , new SqlParameter("Month", Month)
                        , new SqlParameter("Year", year)
                        ).ToListAsync();

                    var AnalizList = new MahalAylik_HaftalikAnaliz();
                    var LastAnalizList = new List<LastMahalAylik_HaftalikAnaliz>();
                    /*int col = 7, row = 2;
                    var matrix = new Matrix();
                    matrix.Numbers = new int[col][];*/
                    var list = new List<Array>();

                    if (HaftalikAnalizList.Count > 0)
                    {
                        /* AnalizList.PazartesiList.AddRange(HaftalikAnalizList.Where(p => p.GunAdi == "Pazartesi").ToList());
                         AnalizList.SaliList.AddRange(HaftalikAnalizList.Where(p => p.GunAdi == "Salı").ToList());
                         AnalizList.CarsambaList.AddRange(HaftalikAnalizList.Where(p => p.GunAdi == "Çarşamba").ToList());
                         AnalizList.PersembeList.AddRange(HaftalikAnalizList.Where(p => p.GunAdi == "Perşembe").ToList());
                         AnalizList.CumaList.AddRange(HaftalikAnalizList.Where(p => p.GunAdi == "Cuma").ToList());
                         AnalizList.CumartesiList.AddRange(HaftalikAnalizList.Where(p => p.GunAdi == "Cumartesi").ToList());
                         AnalizList.PazarList.AddRange(HaftalikAnalizList.Where(p => p.GunAdi == "Pazar").ToList());
                         */
                        //.Select(p=>p.KullaniciId).ToList();
                        //   var GrupKullanicis = KullaniciList.GroupBy<int>();

                        var KullaniciList = HaftalikAnalizList.GroupBy(p => p.KullaniciId).ToList();
                        var array6 = new object[KullaniciList.Count, 8];
                        

                        var arrry2d = new string[9][];

                        var MahalPersonelGunAnalizList = new List<MahalPersonelGunAnaliz>();

                        //LastAnalizList.AddRange(new LastMahalAylik_HaftalikAnaliz { AnalizList.PazartesiList.GetRange() });
                        for (var x = 0; x < KullaniciList.Count; x++)
                        {
                            foreach (var index in HaftalikAnalizList)
                            {
                                //if()
                                //arrry2d[x][i] = "aziz";
                                //LastAnalizList.Add(AnalizList.PazartesiList.Where(p => p.KullaniciId == 20014).Select())

                                //20014
                                //20015
                                //20017
                                var go = true;
                                for (var i = 0; i < 7; i++)
                                {
                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Pazartesi")
                                    {
                                        /*var LastAnaliz = new LastMahalAylik_HaftalikAnaliz
                                        {
                                            fulAdi = index.fulAdi,
                                            GunAdi = index.GunAdi,
                                            Total = index.Total
                                        };
                                        LastAnalizList.Add(LastAnaliz);
                                        go = false;*/

                                        array6[x, 0] = index.Total;

                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Salı")
                                    {
                                        array6[x, 1] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Çarşamba")
                                    {
                                        array6[x, 2] = index.Total;
                                        // break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Perşembe")
                                    {
                                        array6[x, 3] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Cuma")
                                    {
                                        array6[x, 4] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Cumartesi")
                                    {
                                        array6[x, 5] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Pazar")
                                    {
                                        array6[x, 6] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }


                                }
                                /*if (go == false)
                                    break;*/


                            }

                        }

                        for (var x = 0; x < KullaniciList.Count; x++)
                        {
                            //MahalPersonelGunAnalizList.Add(new MahalPersonelGunAnaliz { fulAdi = HaftalikAnalizList.Where(p => p.KullaniciId == KullaniciList.ElementAt(0).Key).Select(p => p.fulAdi).FirstOrDefault() });
                            var fulAdi = HaftalikAnalizList.Where(p => p.KullaniciId == KullaniciList.ElementAt(x).Key).Select(p => p.fulAdi).FirstOrDefault();
                            array6[x,7] = fulAdi;
                        }

                        for (var i = 0; i < KullaniciList.Count; i++)
                        {
                            for (var y = 0; y < 7; y++)
                            {
                                if (array6[i, y] == null)
                                    array6[i, y] = 0;
                            }
                        }

                        /* for(var x = 1; x < 8; x++)
                         {
                             MahalPersonelGunAnalizList.Append
                         }*/
                        list.Add(array6);
                        var aa = JsonConvert.SerializeObject(array6);
                        /*for (int i = 0; i < col; i++)
                            matrix.Numbers[i] = new int[row];*/

                        /*foreach (var Analiz in HaftalikAnalizList)
                        {
                            if (Analiz.GunAdi == "Pazartesi")
                            {
                                AnalizList.Add(new MahalAylik_HaftalikAnaliz { GunAdi = Analiz.GunAdi, fulAdi = Analiz.fulAdi, Total = Analiz.Total });
                            }
                            else if (Analiz.GunAdi == "Salı")
                            {
                                AnalizList.Add(new MahalAylik_HaftalikAnaliz { GunAdi = Analiz.GunAdi, fulAdi = Analiz.fulAdi, Total = Analiz.Total });
                            }
                            else if (Analiz.GunAdi == "Çarşamba")
                            {
                                AnalizList.Add(new MahalAylik_HaftalikAnaliz { GunAdi = Analiz.GunAdi, fulAdi = Analiz.fulAdi, Total = Analiz.Total });
                            }
                            else if (Analiz.GunAdi == "Perşembe")
                            {
                                AnalizList.Add(new MahalAylik_HaftalikAnaliz { GunAdi = Analiz.GunAdi, fulAdi = Analiz.fulAdi, Total = Analiz.Total });
                            }
                            else if (Analiz.GunAdi == "Cuma")
                            {
                                AnalizList.Add(new MahalAylik_HaftalikAnaliz { GunAdi = Analiz.GunAdi, fulAdi = Analiz.fulAdi, Total = Analiz.Total });
                            }
                            else if (Analiz.GunAdi == "Cumartesi")
                            {
                                AnalizList.Add(new MahalAylik_HaftalikAnaliz { GunAdi = Analiz.GunAdi, fulAdi = Analiz.fulAdi, Total = Analiz.Total });
                            }
                            else if (Analiz.GunAdi == "Pazar")
                            {
                                AnalizList.Add(new MahalAylik_HaftalikAnaliz { GunAdi = Analiz.GunAdi, fulAdi = Analiz.fulAdi, Total = Analiz.Total });
                            }
                        }*/
                        JsonResult result2 = Json(new { State = 1, NobetList = NobetList, AnalizList = AnalizList, array6 = aa }, JsonRequestBehavior.AllowGet);
                        result2.MaxJsonLength = Int32.MaxValue;
                        return result2;
                    }
                    else
                    {
                        JsonResult result = Json(new { State = 1, NobetList = NobetList, AnalizList = AnalizList, array6 = list }, JsonRequestBehavior.AllowGet);
                        result.MaxJsonLength = Int32.MaxValue;
                        return result;
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public class Matrix
        {
            List<Array> A { get; set; }
        }
        public class MahalAylik_HaftalikAnaliz
        {
            public int KullaniciId { get; set; }
            public string GunAdi { get; set; }
            public string fulAdi { get; set; }
            public int Total { get; set; }
            public List<MahalAylik_HaftalikAnaliz> PazartesiList { get; set; }
            public List<MahalAylik_HaftalikAnaliz> SaliList { get; set; }
            public List<MahalAylik_HaftalikAnaliz> CarsambaList { get; set; }
            public List<MahalAylik_HaftalikAnaliz> PersembeList { get; set; }
            public List<MahalAylik_HaftalikAnaliz> CumaList { get; set; }
            public List<MahalAylik_HaftalikAnaliz> CumartesiList { get; set; }
            public List<MahalAylik_HaftalikAnaliz> PazarList { get; set; }
            public MahalAylik_HaftalikAnaliz()
            {
                PazartesiList = new List<MahalAylik_HaftalikAnaliz>();
                SaliList = new List<MahalAylik_HaftalikAnaliz>();
                CarsambaList = new List<MahalAylik_HaftalikAnaliz>();
                PersembeList = new List<MahalAylik_HaftalikAnaliz>();
                CumaList = new List<MahalAylik_HaftalikAnaliz>();
                CumartesiList = new List<MahalAylik_HaftalikAnaliz>();
                PazarList = new List<MahalAylik_HaftalikAnaliz>();

            }

        }
        public class LastMahalAylik_HaftalikAnaliz
        {
            public int? KullaniciId { get; set; }
            public string GunAdi { get; set; }
            public string fulAdi { get; set; }
            public int Total { get; set; }
        }
        public class MahalPersonelGunAnaliz
        {
            public string fulAdi { get; set; }
            public string Pazartesi { get; set; }
            public string Sali { get; set; }
            public string Carsamba { get; set; }
            public string Persembe { get; set; }
            public string Cuma { get; set; }
            public string Cumartesi { get; set; }
            public string Pazar { get; set; }

        }

        [HttpPost]
        public async Task<JsonResult> MahalNobetListTable(string month, string year)
        {
            try
            {
                if (String.IsNullOrEmpty(Session["KullaniciId"].ToString()))
                    return Json(new { State = 3, Message = "Tekrar Giriş Yapın!" }, JsonRequestBehavior.AllowGet);

                //var kid = Convert.ToInt32(Session["KullaniciId"]);
                var Month = Convert.ToInt32(month);
                var Year = Convert.ToInt32(year);
                var MahalId = Convert.ToInt32(Session["MahalId"]);

                using (ModelContext mc = new ModelContext())
                {
                    var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetListe_Aylik_Getir.sql"), Encoding.Default);
                    var NobetList = await mc.Database.SqlQuery<string>(query,
                         // new SqlParameter("KullaniciId", kid)
                         new SqlParameter("Month", Month)
                        , new SqlParameter("Year", Year)
                        , new SqlParameter("MahalId", MahalId)
                        ).FirstOrDefaultAsync();

                    //chart işlemi
                    var query3 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetYillikKullaniciHesabi.sql"), Encoding.Default);
                    var UserTotalYear = mc.Database.SqlQuery<UserTotalYearData>(query3, new SqlParameter("MahalId", MahalId),
                         new SqlParameter("Year", year)
                     ).ToList();

                    //yıl bazında total hafta analizi

                    var queryHaftalikAnaliz = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetMahalAylik_YillikAnalizHesabi.sql"), Encoding.Default);
                    var HaftalikAnalizList = await mc.Database.SqlQuery<MahalAylik_HaftalikAnaliz>(queryHaftalikAnaliz
                        , new SqlParameter("MahalId", MahalId)
                        , new SqlParameter("Month", Month)
                         ,new SqlParameter("Year", year)
                        ).ToListAsync();

                    var AnalizList = new MahalAylik_HaftalikAnaliz();
                    var LastAnalizList = new List<LastMahalAylik_HaftalikAnaliz>();
                    /*int col = 7, row = 2;
                    var matrix = new Matrix();
                    matrix.Numbers = new int[col][];*/
                    var list = new List<Array>();

                    if (HaftalikAnalizList.Count > 0)
                    {

                        var KullaniciList = HaftalikAnalizList.GroupBy(p => p.KullaniciId).ToList();
                        var array6 = new object[KullaniciList.Count, 8];

                        var arrry2d = new string[9][];

                        var MahalPersonelGunAnalizList = new List<MahalPersonelGunAnaliz>();

                        //LastAnalizList.AddRange(new LastMahalAylik_HaftalikAnaliz { AnalizList.PazartesiList.GetRange() });
                        for (var x = 0; x < KullaniciList.Count; x++)
                        {
                            foreach (var index in HaftalikAnalizList)
                            {
                                //if()
                                //arrry2d[x][i] = "aziz";
                                //LastAnalizList.Add(AnalizList.PazartesiList.Where(p => p.KullaniciId == 20014).Select())

                                //20014
                                //20015
                                //20017
                                var go = true;
                                for (var i = 0; i < 7; i++)
                                {
                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Pazartesi")
                                    {
                                        /*var LastAnaliz = new LastMahalAylik_HaftalikAnaliz
                                        {
                                            fulAdi = index.fulAdi,
                                            GunAdi = index.GunAdi,
                                            Total = index.Total
                                        };
                                        LastAnalizList.Add(LastAnaliz);
                                        go = false;*/

                                        array6[x, 0] = index.Total;

                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Salı")
                                    {
                                        array6[x, 1] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Çarşamba")
                                    {
                                        array6[x, 2] = index.Total;
                                        // break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Perşembe")
                                    {
                                        array6[x, 3] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Cuma")
                                    {
                                        array6[x, 4] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Cumartesi")
                                    {
                                        array6[x, 5] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }

                                    if (index.KullaniciId == KullaniciList.ElementAt(x).Key && index.GunAdi == "Pazar")
                                    {
                                        array6[x, 6] = index.Total;
                                        break;
                                        //arrry2d[x][i]=index.Total.ToString();
                                    }


                                }
                              


                            }

                        }

                        for (var x = 0; x < KullaniciList.Count; x++)
                        {
                            //MahalPersonelGunAnalizList.Add(new MahalPersonelGunAnaliz { fulAdi = HaftalikAnalizList.Where(p => p.KullaniciId == KullaniciList.ElementAt(0).Key).Select(p => p.fulAdi).FirstOrDefault() });
                            var fulAdi = HaftalikAnalizList.Where(p => p.KullaniciId == KullaniciList.ElementAt(x).Key).Select(p => p.fulAdi).FirstOrDefault();
                            array6[x, 7] = fulAdi;

                        }

                        for(var i = 0; i < KullaniciList.Count; i++)
                        {
                            for(var y = 0; y < 7; y++)
                            {
                                if (array6[i, y] == null)
                                    array6[i, y] = 0;
                            }
                        }
                        
                        list.Add(array6);
                        var aa = JsonConvert.SerializeObject(array6);
                        
                        JsonResult result2 = Json(new { State = 1, NobetList = NobetList, AnalizList = AnalizList, array6 = aa, UserTotalYear = UserTotalYear }, JsonRequestBehavior.AllowGet);
                        result2.MaxJsonLength = Int32.MaxValue;
                        return result2;
                    }
                    else
                    {
                        JsonResult result = Json(new { State = 1, NobetList = NobetList, AnalizList = AnalizList, array6 = list, UserTotalYear = UserTotalYear }, JsonRequestBehavior.AllowGet);
                        result.MaxJsonLength = Int32.MaxValue;
                        return result;
                    }

                    /* JsonResult result = Json(new { State = 1, NobetList = NobetList, UserTotalYear = UserTotalYear }, JsonRequestBehavior.AllowGet);
                     result.MaxJsonLength = Int32.MaxValue;
                     return result;*/
                }
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public class UserTotalYearData
        {
            public string UserTotalYear { get; set; }
            public string FullName { get; set; }
        }
        [HttpPost]
        public JsonResult PersonelMazeretKaydet(List<string> Data)
        {
            try
            {
                if (Session["KullaniciId"] == null)
                {
                    RedirectToAction("Login", "Login");
                    return Json(null);
                }
                var kullaniciId = Convert.ToInt32(Session["KullaniciId"]);
                var mahalId = Convert.ToInt32(Session["MahalId"]);

                var TarihAralikArry = Data[0].Trim().Split('-');
                var baslangicTarihi = Convert.ToDateTime(TarihAralikArry[0]);
                var bitisTarihi = Convert.ToDateTime(TarihAralikArry[1]);
                var Aciklama = Data[1];

                var mc = new ModelContext();

                var queryKontrol = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetMazeretTarihAralik_Kontrol.sql"), Encoding.Default);
                var mazeretKontrol = mc.Database.SqlQuery<int>(queryKontrol, new SqlParameter("KullaniciId", kullaniciId)
                    , new SqlParameter("MahalId", mahalId)
                    , new SqlParameter("BaslangicTarihi", baslangicTarihi)
                    , new SqlParameter("BitisTarihi", bitisTarihi)
                    //, new SqlParameter("MazeretAciklama", Aciklama)
                    //, new SqlParameter("Durum", true)
                    ).FirstOrDefault();
                if (mazeretKontrol == 0)
                {
                    var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetMazeretTarih_Ekle.sql"), Encoding.Default);
                    var mazeretSonuc = mc.Database.SqlQuery<int>(query, new SqlParameter("KullaniciId", kullaniciId)
                        , new SqlParameter("MahalId", mahalId)
                        , new SqlParameter("BaslangicTarihi", baslangicTarihi)
                        , new SqlParameter("BitisTarihi", bitisTarihi)
                        , new SqlParameter("MazeretAciklama", Aciklama)
                        , new SqlParameter("MazeretBildiriTarihi", DateTime.Now)
                        , new SqlParameter("Durum", true)).FirstOrDefault();
                    if (mazeretSonuc > 0)
                    {
                        return Json(new { State = 1, Message = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { State = 2, Message = "Kayıt Başarısız. Tekrar Deneyin." }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(new { State = 2, Message = "Kayıt Başarısız. Mevcut Mazeretler ile Uyuşmuyor. Tarih Aralığını Değiştirin." }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                return Json(new { State = 2, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult MazeretDelete(string mid)
        {
            try
            {
                if (!String.IsNullOrEmpty(mid))
                {
                    var mc = new ModelContext();
                    var midint = Convert.ToInt32(mid);
                    var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetMazeretTarih_Silme.sql"), Encoding.Default);
                    var mazeretKontrol = mc.Database.SqlQuery<string>(query
                    , new SqlParameter("Id", midint)
                    //, new SqlParameter("MazeretAciklama", Aciklama)
                    //, new SqlParameter("Durum", true)
                    ).FirstOrDefault();
                    return Json(new { State = 1, Message = mazeretKontrol, title = "Nöbet Mazeret Silme İşlemi" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { State = 2, Message = "Hata Oluştu!" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { State = 2, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> PersonelMazeretListe()
        {
            try
            {
                if (Session["KullaniciId"] == null)
                {
                    RedirectToAction("Login", "Login");
                    //return Json(null);
                }
                var kullaniciId = Convert.ToInt32(Session["KullaniciId"]);
                var mahalId = Convert.ToInt32(Session["MahalId"]);


                //var mc = new ModelContext();
                //var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetMazeretTarih_Ekle.sql"), Encoding.Default);


                using (ModelContext mc = new ModelContext())
                {
                    var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetMazeretTarih_Liste.sql"), Encoding.Default);
                    var mazeretSonuc = await mc.Database.SqlQuery<string>(query
                        , new SqlParameter("KullaniciId", kullaniciId)
                        , new SqlParameter("MahalId", mahalId)
                        , new SqlParameter("Durum", true)
                        ).FirstOrDefaultAsync();
                    JsonResult result = Json(new { State = 1, mazeretSonuc = mazeretSonuc }, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = Int32.MaxValue;
                    return result;
                }

                /* if (mazeretSonuc  != null)
                 {
                     return Json(new { State = 1, Message = "Kayıt Başarılı" }, JsonRequestBehavior.AllowGet);
                 }
                 else
                 {
                     return Json(new { State = 2, Message = "Kayıt Başarısız. Tekrar Deneyin." }, JsonRequestBehavior.AllowGet);

                 }*/

            }
            catch (Exception ex)
            {
                return Json(new { State = 2, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DetayliIstatistik()
        {
            try
            {
                if (CheckIsPage(20) == false)
                    return RedirectToAction("Login", "Login");

                var model = new DetayliIstatistikModel();
                ///mahallist ---
                var mahallist = GetMahalListDDL();
                //var mahalDataList = mc.Database.SqlQuery<MahalModel>("Select * from Mahal", Common.GetDBNullOrValue<int>(5)).ToList();
                //  var mahalModelList = new List<MahalModel>();
                model.MahalList.AddRange(mahallist);

                var listOfParam = new List<SelectListItem>();
                var item1 = new SelectListItem { Text = "-Tüm Mahaller-", Value = "0" };
                listOfParam.Add(item1);
                var list = mahallist;//mahalDataList; //RolBLL.Select(exp).ToList();
                listOfParam.AddRange(list.Select(t => new SelectListItem { Text = t.MahalAdi.ToString(), Value = t.MahalId.ToString() }));
                ViewBag.ddlMahalId = listOfParam;

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("/Shared/Error");
            }
        }

        [HttpPost]
        public async Task<JsonResult> DetayliPersonelNobetListTable(string month, string year, string mahalId, string uId)
        {
            try
            {
                if (String.IsNullOrEmpty(Session["KullaniciId"].ToString()))
                    return Json(new { State = 3, Message = "Tekrar Giriş Yapın!" }, JsonRequestBehavior.AllowGet);

                if (String.IsNullOrEmpty(uId))
                    uId = "0";

                var kid = Convert.ToInt32(Session["KullaniciId"]);
                var Month = month;//Convert.ToInt32(month);
                var Year = year;//Convert.ToInt32(year);
                var MahalId = mahalId;// Convert.ToInt32(Session["MahalId"]);

                using (ModelContext mc = new ModelContext())
                {
                    var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\Detaylstatistik.sql"), Encoding.Default);
                    var NobetList = await mc.Database.SqlQuery<string>(query,
                        new SqlParameter("KullaniciId", uId)
                        , new SqlParameter("Month", Month)
                        , new SqlParameter("Year", Year)
                        , new SqlParameter("MahalId", MahalId)
                        ).FirstOrDefaultAsync();

                    JsonResult result = Json(new { State = 1, NobetList = NobetList }, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = Int32.MaxValue;
                    return result;


                }
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



    }
    public class NobetMazeretTarihModel
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public int MahalId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string MazeretAciklama { get; set; }
        public byte Durum { get; set; }

    }
}