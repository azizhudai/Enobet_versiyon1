using Enobet_versiyon1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Enobet_versiyon1.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        public List<KullaniciModel.KullaniciKategoriListe> GetKategoriListeDDL()
        {
            ModelContext mc = new ModelContext();
            var dataList = new List<KullaniciModel.KullaniciKategoriListe>();
            dataList = mc.Database.SqlQuery<KullaniciModel.KullaniciKategoriListe>("select Adi,KategoriListeId,KategoriValue from KategoriListe where KategoriValue in(1,2) and Aktif=1 order by Sira").ToList();
            /* var rolid = Convert.ToInt32(Session["RolId"].ToString());
             var userid = Convert.ToInt32(Session["KullaniciId"].ToString());
             if (rolid == 1)
                 dataList = mc.Database.SqlQuery<MahalModel>("Select * from Mahal where Aktif=1").ToList();
             if (rolid == 3)
                 dataList = mc.Database.SqlQuery<MahalModel>("select * from Mahal where MahalId in(select MahalId from NobetKidemli where KullaniciId=@KullaniciId) and Aktif=1", new SqlParameter("KullaniciId", userid)).ToList();
             */
            return dataList;
        }
        public ActionResult Register()
        {
            //kategori sınıf ve rütbe için...
            var kategoriList = GetKategoriListeDDL();
            var listOfParamSinif = new List<SelectListItem>();
            var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };

            listOfParamSinif.Add(item1);
            listOfParamSinif.AddRange(kategoriList.Where(p => p.KategoriValue == 1).Select(t => new SelectListItem { Text = t.Adi.ToString(), Value = t.KategoriListeId.ToString() }));
            ViewBag.ddlSinifId = listOfParamSinif;
            // rütbe için
            var listOfParamRutbe = new List<SelectListItem>();
            listOfParamRutbe.Add(item1);
            listOfParamRutbe.AddRange(kategoriList.Where(p => p.KategoriValue == 2).Select(t => new SelectListItem { Text = t.Adi.ToString(), Value = t.KategoriListeId.ToString() }));
            ViewBag.ddlRutbeId = listOfParamRutbe;

            //mahal liste
            var mc = new ModelContext();
            //var dataList = new List<MahalModel>();
            var dataList = mc.Database.SqlQuery<MahalModel>("select MahalId,MahalAdi from Mahal where Aktif=1 order by Sira").ToList();

            var listOfParam = new List<SelectListItem>();
            //var itemMahal = new SelectListItem { Text = "-Seçiniz-", Value = "" };
            listOfParam.Add(item1);
            var list = dataList;//mahalDataList; //RolBLL.Select(exp).ToList();
            listOfParam.AddRange(list.Select(t => new SelectListItem { Text = t.MahalAdi.ToString(), Value = t.MahalId.ToString() }));
            ViewBag.ddlMahalId = listOfParam;

            return View();
        }
        [HttpPost]
        [Obsolete]
        public ActionResult Login(FormCollection collection)
        {
            try
            {
                var TcNo = collection["tcNo"].ToString();
                var Password = collection["Password"];
                if (string.IsNullOrEmpty(TcNo) || string.IsNullOrEmpty(Password))
                    return RedirectToAction("Login");

                Password = Common.EncodeStr(Password);
                ModelContext mc = new ModelContext();
                var query = "";
                var result = new KullaniciModel();
                // if (!string.IsNullOrEmpty(TcNo) && !string.IsNullOrEmpty(Password))
                // {
                query = "select * from Kullanici where Tcno=@Tcno and Aktif=1 ";
                /*query = "declare @RowId int,@Msg varchar(50); set @RowId=0; set @Msg=''; " +
               "Insert Into Mahal(MahalAdi) Values(@MahalAdi);" +
                  " set @Msg = 'Ekleme Başarılı';" +
                                   " select top 1 @RowId = KullaniciId from Kullanici " +  where MahalId = @PersonelId and DahiliNo = @DahiliNo and Gsm = @Gsm" +
                               " select cast(@RowId as int)RowId, @Msg Mesaj"; ;*/
                result = mc.Database.SqlQuery<KullaniciModel>(query, new System.Data.SqlClient.SqlParameter("TcNo", TcNo)).FirstOrDefault();
                if (result != null)
                {
                    if (result.Parola == Password)
                    {
                        Session["KullaniciId"] = result.KullaniciId;
                        //Session["Rutbe"] = result.Rutbe;
                        Session["KullaniciAdi"] = result.KullaniciAdi;
                        Session["KullaniciSoyadi"] = result.KullaniciSoyadi;
                        Session["RolId"] = result.RolId;
                        Session["Eposta"] = result.Eposta;
                        Session["Tcno"] = result.Tcno;
                        Session["BirlikId"] = result.BirlikId;
                        Session["MahalId"] = result.MahalId;

                        //log işlemleri 
                        var ip = Common.GetIpAddress();
                        var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LogKayit.sql"), Encoding.Default);
                        var resultLog = mc.Database.SqlQuery<GeneralResult>(query2, new SqlParameter("SayfaId", 12), new SqlParameter("Aciklama", "RolId: " + result.RolId.ToString() + " TC No: " + result.Tcno + " MahalId:" + result.MahalId.ToString())
                            , new SqlParameter("IslemYapanKullaniciId", result.KullaniciId)
                            , new SqlParameter("IslemSaati", DateTime.Now)
                            , new SqlParameter("Ip", ip)
                            , new SqlParameter("IslemId", 18)).FirstOrDefault();

                        return RedirectToAction("Index", "Home");
                        //return Json(new { State = 1, Message = "Şifreler Uyuşmuyor!", title = "Kullanıcı Giriş İşlemi" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        TempData["Result"] = "Şifre Hatalı!";
                        return RedirectToAction("Login");
                        //return Json(new { State = 2, Message = "Şifreler Uyuşmuyor!", title = "Kullanıcı Giriş İşlemi" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    TempData["Result"] = "Kayıtlı Personel Bulunamadı!";
                    return RedirectToAction("Login");
                    //return Json(new { State = 2, Message = "Kayıtlı Personel Bulunamadı!", title = "Kullanıcı Giriş İşlemi" }, JsonRequestBehavior.AllowGet);
                }
                //}
                return View();
            }
            catch (Exception ex)
            {
                // return View("/Shared/Error");
                return RedirectToAction("Error");
            }
        }
        [HttpPost]
        [Obsolete]
        public JsonResult KullaniciEkle(List<string> dataUserArray)
        {
            try
            {
                if (dataUserArray == null || dataUserArray.Count < 1)
                    return Json(new { State = 2, Message = "Hata Oluştu!" }, JsonRequestBehavior.AllowGet);

                //var userId = dataUserArray[0];
                var resultPassword = Common.ValidatePasswordRegular(dataUserArray[6]);
                if(!resultPassword)
                    return Json(new { State = 2, Message = "Parola Formatı Uymuyor. Tekrar Deneyin!<br>(En az 8 en fazla 15 karakter, 1 Küçük 1 Büyük harf ve 1 tane Sayı girin.)", title = "Kullanıcı Ekleme İşlemi" }, JsonRequestBehavior.AllowGet);
                //var userRank = dataUserArray[0];
                var sinifId = dataUserArray[0];
                var rutbeId = dataUserArray[1];
                var userName = dataUserArray[2];
                var userSurname = dataUserArray[3];
                var Email = dataUserArray[4];
                var Tcno = dataUserArray[5];
                var password =  Common.EncodeStr(dataUserArray[6]);//  dataUserArray[6];
                var MahalId = Convert.ToInt32(dataUserArray[7]);
                //var BirlikId = Convert.ToInt32(dataUserArray[7]);
                //var MahalId = Convert.ToInt32(dataUserArray[8]);
                //var rolId = Convert.ToInt32(dataUserArray[9]);
                var NobettenCikar = 0;
                ModelContext mc = new ModelContext();
                var query = "";
                var result = new GeneralResult();


                var ip = Common.GetIpAddress();
                var islemYapanId = Convert.ToInt32(Session["KullaniciId"]);
                var islemTarihi = DateTime.Now;

                /*  if (userId == "0") //ekleme işlemi
                  {*/
                query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\PersonelKendiKayit.sql"), Encoding.Default);

                /*"declare @RowId int,@Msg varchar(50),@IsExist tinyint,@NobetKidemliEposta varchar(max),@DetaiMessage varchar(max),@KullaniciId int; " +
                " set @RowId=0; set @IsExist=0" +
                "select top 1 @IsExist=count(KullaniciId) from Kullanici Where Eposta=@Email or Tcno=@Tcno " +
                " if @IsExist!=0 " +
                " begin " +
                "  set @Msg = 'Aynı T.C. Nolu veya E-Postalı Personel Zaten Sistemde Mevcut!'; set @RowId=0 " +
                " end " +
                " else " +
                " begin " +
            "insert into Kullanici(SinifId,RutbeId,KullaniciAdi,KullaniciSoyadi,Eposta,Tcno,Parola,RolId,Aktif,NobettenCikar,MahalId) values(@SinifId," +
            "@RutbeId,@userName,@userSurname,@Email,@Tcno,@password,0,@Aktif,@NobettenCikar,@MahalId) " +
            " SELECT @KullaniciId=SCOPE_IDENTITY(); select @KullaniciId; " +
            " select top 1 @NobetKidemliEposta=Eposta from Kullanici where MahalId=2 and RolId=3; " +
            " select @DetaiMessage=dbo.Fn_FulAd(@KullaniciId,'') from Kullanici where  KullaniciId=@KullaniciId " +
            " select top 1 @RowId = KullaniciId from Kullanici where Tcno = @Tcno" +
            " set @Msg = 'Ekleme Başarılı'" +
            " set @DetaiMessage += 'Talep Eden Personel: '+@DetaiMessage; "+
            "end" +
            " select @RowId RowId, @Msg Mesaj,@DetaiMessage DetaiMessage,@NobetKidemliEposta NobetKidemliEposta ";*/

                var userAddResult = mc.Database.SqlQuery<GeneralResultDetail>(query, new SqlParameter("RutbeId", Convert.ToInt32(rutbeId)),
                    new SqlParameter("SinifId", Convert.ToInt32(sinifId)),
                new SqlParameter("userName", userName)
               , new SqlParameter("userSurname", userSurname)
               , new SqlParameter("Email", Email)
               , new SqlParameter("Tcno", Tcno)
               , new SqlParameter("password", password)
               , new SqlParameter("NobettenCikar", NobettenCikar)
               , new SqlParameter("Aktif", 2)
               , new SqlParameter("MahalId", MahalId)
                , new SqlParameter("IslemYapanId", islemYapanId)
                   , new SqlParameter("IP", ip)
                   , new SqlParameter("IslemTarihi", islemTarihi)
               ).FirstOrDefault();
                if (userAddResult.RowId != 0)
                {
                    //log işlemleri 
                  //  var ip = Common.GetIpAddress();
                    var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LogKayit.sql"), Encoding.Default);
                    var resultLog = mc.Database.SqlQuery<GeneralResult>(query2, new SqlParameter("SayfaId", 13), new SqlParameter("Aciklama", "Rol Id: " + "0")
                        , new SqlParameter("IslemYapanKullaniciId", userAddResult.RowId)
                        , new SqlParameter("IslemSaati", DateTime.Now)
                        , new SqlParameter("Ip", ip)
                        , new SqlParameter("IslemId", 15)).FirstOrDefault();

                    var mailBaslik = "E-Nöbet Sistemi - Personel Sistem Giriş Talebi";
                    var mailAciklamaHtml = userAddResult.DetaiMessage+ ".<br>E-Nöbet Sistemi üzerinden Kullanıcı Listeleme ekranına gelerek kullanıcının Durum Bilgisi alanını düzenlemeniz gerekmektedir<br>" +
                        "<a href='https://www.focaus.dzkk.tsk/enobet/YoneticiIslemleri/KullaniciIslemleri' target='_blank'>E-Nöbet Sistemi</a> ";

                    Common.HotmailMailSend3(userAddResult.NobetKidemliEposta, mailBaslik, mailAciklamaHtml);

                    return Json(new { State = 1, Message = "Ekleme Başarılı. Nöbet Kıdemlisi tarafından onay için hesabınız Bekleme'dedir. Onaylandıktan sonra E-Posta ile geri dönüş sağlanacaktır.", title = "Kullanıcı Ekleme İşlemi" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { State = 2, Message = userAddResult.Mesaj, title = "Kullanıcı Ekleme İşlemi" }, JsonRequestBehavior.AllowGet);
                //}
                /*  else // update 
                  {
                      query = "declare @RowId int,@Msg varchar(50); set @RowId=0; " +
                          "update Kullanici set Rutbe=@userRank,KullaniciAdi=@userName,KullaniciSoyadi=@userSurname,Eposta=@Email,Tcno=@Tcno,Parola=@password," +
                          "BirlikId=@BirlikId,MahalId=@MahalId,RolId=@rolId,Aktif=@Aktif where KullaniciId = @userId " +
                          " select top 1 @RowId = KullaniciId from Kullanici where KullaniciId = @userId" +
                          " set @Msg = 'Ekleme Başarılı'" +
                          " select @RowId RowId, @Msg Mesaj";
                      var userIdint = Convert.ToInt32(userId);
                      var userAddResult2 = mc.Database.SqlQuery<GeneralResult>(query, new System.Data.SqlClient.SqlParameter("userRank", userRank),
                      new System.Data.SqlClient.SqlParameter("userName", userName)
                     , new System.Data.SqlClient.SqlParameter("userSurname", userSurname)
                     , new System.Data.SqlClient.SqlParameter("Email", Email)
                     , new System.Data.SqlClient.SqlParameter("Tcno", Tcno)
                     , new System.Data.SqlClient.SqlParameter("password", password)
                     , new System.Data.SqlClient.SqlParameter("BirlikId", BirlikId)
                     , new System.Data.SqlClient.SqlParameter("MahalId", MahalId)
                     , new System.Data.SqlClient.SqlParameter("rolId", rolId)
                     , new System.Data.SqlClient.SqlParameter("Aktif", true)
                      , new System.Data.SqlClient.SqlParameter("userId", true)
                     ).FirstOrDefault();
                      if (userAddResult2.RowId != 0)
                          return Json(new { State = 1, Message = "Güncelleme Başarılı!", title = "Kullanıcı Güncelleme İşlemi" }, JsonRequestBehavior.AllowGet);
                      else
                          return Json(new { State = 2, Message = "Güncelleme Hatalı!", title = "Kullanıcı Güncelleme İşlemi" }, JsonRequestBehavior.AllowGet);
                  }*/

            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LoginOff()
        {
            SessionClear();
            return RedirectToAction("Login", "Login");
            //return null;
        }
        public void SessionClear()
        {
            Session.RemoveAll(); // removes all session value specific to this user
            Session.Clear();
        }
    }

    public class GeneralResult
    {
        public int RowId { get; set; }
        public string Mesaj { get; set; }
    }
    public class GeneralResultDetail
    {
        public int RowId { get; set; }
        public string Mesaj { get; set; }
        public string NobetKidemliEposta { get; set; }
        public string DetaiMessage { get; set; }
    }
    public sealed class SessionControl
    {
        //public SessionControl();
    }
}