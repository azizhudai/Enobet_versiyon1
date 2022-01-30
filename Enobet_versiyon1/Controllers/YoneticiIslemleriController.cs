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
using System.Web.UI.WebControls;

namespace Enobet_versiyon1.Controllers
{
    public class YoneticiIslemleriController : Controller
    {
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
  " where M.Aktif = 1 order by M.Sira").ToList();

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

        // GET: YoneticiIslemleri
        [Obsolete]
        public ActionResult MahalIslemleri()
        {
            if (CheckIsPage(2) == false)
                return RedirectToAction("Login", "Login");
            //Common.HotmailMailSend2();//"deneme", "<div>deneem body</div><div>deneme2 body</div>","azizhudaikaratas@gmail.com");
            //ViewBag.Message = null;
            //  ModelContext mc = new ModelContext();
            var model = new MahalModel();
            var dataList = new List<MahalModel>();

            /* if (rolid == 1)
              dataList = mc.Database.SqlQuery<MahalModel>("Select * from Mahal", Common.GetDBNullOrValue<int>(5)).ToList();
             if(rolid == 3)
                 dataList = mc.Database.SqlQuery<MahalModel>("select * from Mahal where MahalId in(select MahalId from NobetKidemli where KullaniciId=@KullaniciId) and Aktif=1", new SqlParameter("KullaniciId", userid)).ToList();
             */
            //  var mahalModelList = new List<MahalModel>();

            var mc = new ModelContext();
            //log işlemleri 
            var ip = Common.GetIpAddress();
            var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LogKayit.sql"), Encoding.Default);
            var resultLog = mc.Database.SqlQuery<GeneralResult>(query2, new SqlParameter("SayfaId", 2), new SqlParameter("Aciklama", "")
                , new SqlParameter("IslemYapanKullaniciId", Convert.ToInt32(Session["KullaniciId"]))
                , new SqlParameter("IslemSaati", DateTime.Now)
                , new SqlParameter("Ip", ip)
                , new SqlParameter("IslemId", 19)).FirstOrDefault();

            model.ListMahal.AddRange(GetMahalListDDL());

            return View(model);
        }

        public JsonResult SearchFulPersonelListGetir(string term)
        {
            try
            {
                var mc = new ModelContext();
                var query = "select cast(KullaniciId as varchar) as value, (KLSinif.Adi+' '+ KLRutbe.Adi +' '+KullaniciAdi+' '+KullaniciSoyadi)+' | '+Eposta as label from Kullanici " +
                    "left join KategoriListe KLSinif on KLSinif.KategoriListeId = SinifId and KLSinif.Aktif=1 " +
                    "left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = RutbeId and KLRutbe.Aktif = 1 " +
                    "Where (KullaniciAdi like '%" + term + "%' or KullaniciSoyadi like '%" + term + "%') order by KullaniciAdi"; // RolId=3 and 
                var result = mc.Database.SqlQuery<UserData>(query).Select(x => new { label = x.label, value = x.value.ToString() }).ToList();
                //    var data = result.Select(m => new { label = m.label,value=m.value }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Hatalı Veri!", JsonRequestBehavior.AllowGet);
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

        [Obsolete]
        public void LogKayitEkle(ModelContext mc, int SayfaId, int IslemId, string Aciklama)
        {
            //log işlemleri 
            var ip = Common.GetIpAddress();
            var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LogKayit.sql"), Encoding.Default);
            var resultLog = mc.Database.SqlQuery<GeneralResult>(query2, new SqlParameter("SayfaId", SayfaId), new SqlParameter("Aciklama", Aciklama)
                , new SqlParameter("IslemYapanKullaniciId", Convert.ToInt32(Session["KullaniciId"]))
                , new SqlParameter("IslemSaati", DateTime.Now)
                , new SqlParameter("Ip", ip)
                , new SqlParameter("IslemId", IslemId)).FirstOrDefault();
        }

        //[PermissionsFilter("4")]
        [Obsolete]
        public ActionResult KullaniciIslemleri()
        {
            if (CheckIsPage(4) == false)
                return RedirectToAction("Login", "Login");
            ModelContext mc = new ModelContext();
            var model = new KullaniciModel();

            /*var kullaniciDataList = mc.Database.SqlQuery<KullaniciModel>("Select * from Kullanici", Common.GetDBNullOrValue<int>(5)).ToList();
            //  var mahalModelList = new List<MahalModel>();
            model.KullaniciList.AddRange(kullaniciDataList);
            */


            //birliklist 

            var birliklDataList = mc.Database.SqlQuery<BirlikModel>("Select * from Birlik").ToList();
            //  var mahalModelList = new List<MahalModel>();
            model.BirlikList.AddRange(birliklDataList);
            var listOfParam0 = new List<SelectListItem>();
            var item0 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
            listOfParam0.Add(item0);
            // var list = birliklDataList; //RolBLL.Select(exp).ToList();
            listOfParam0.AddRange(birliklDataList.Select(t => new SelectListItem { Text = t.BirlikAdi.ToString(), Value = t.BirlikId.ToString() }));
            ViewBag.ddlBirlikId = listOfParam0;
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

            //kategori sınıf ve rütbe için...
            var kategoriList = GetKategoriListeDDL();
            var listOfParamSinif = new List<SelectListItem>();
            listOfParamSinif.Add(item1);
            listOfParamSinif.AddRange(kategoriList.Where(p => p.KategoriValue == 1).Select(t => new SelectListItem { Text = t.Adi.ToString(), Value = t.KategoriListeId.ToString() }));
            ViewBag.ddlSinifId = listOfParamSinif;
            // rütbe için
            var listOfParamRutbe = new List<SelectListItem>();
            listOfParamRutbe.Add(item1);
            listOfParamRutbe.AddRange(kategoriList.Where(p => p.KategoriValue == 2).Select(t => new SelectListItem { Text = t.Adi.ToString(), Value = t.KategoriListeId.ToString() }));
            ViewBag.ddlRutbeId = listOfParamRutbe;

            //kulanıcı durum bilgisi viewbag ddl
            var listOfParamDurum = new List<SelectListItem>();
            //var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
            listOfParamDurum.Add(new SelectListItem { Text = "Aktif", Value = "1" });
            //var list = mahallist;//mahalDataList; //RolBLL.Select(exp).ToList();
            listOfParamDurum.Add(new SelectListItem { Text = "Beklemede", Value = "2" });
            listOfParamDurum.Add(new SelectListItem { Text = "Tayin Oldu", Value = "3" });
            listOfParamDurum.Add(new SelectListItem { Text = "Emekli Oldu", Value = "4" });
            ViewBag.ddlKullaniciDurumId = listOfParamDurum;

            //Log Kayit
            //LogKayitEkle(mc, 4, 22, "");

            return View(model);
        }

        [HttpPost]
        [Obsolete]
        public ActionResult MahalEkle(FormCollection collection)
        {
            var islemId = 0;

            var MahalAdi = collection["MahalAdi"];
            var DahiliNo = collection["DahiliNo"];
            var mahalId = collection["mid"];
            var mahalAmirId = collection["uId"];
            var Sira = collection["Order"];
            ModelContext mc = new ModelContext();
            var query = "";
            var result = new GeneralResult();

            var ip = Common.GetIpAddress();
            var islemYapanId = Convert.ToInt32(Session["KullaniciId"]);
            var islemTarihi = DateTime.Now;
            if (string.IsNullOrEmpty(mahalId))
                mahalId = "0";
            if (mahalId == "0")
            {
                query = "declare @RowId int,@Msg varchar(50); set @RowId=0; set @Msg=''; " +
               "Insert Into Mahal(MahalAdi,DahiliNo,MahalAmirId,Aktif,IslemYapanId,IP,IslemTarihi,Sira) Values(@MahalAdi,@DahiliNo,@MahalAmirId,1,@IslemYapanId,@IP,@IslemTarih,@Sira);" +
                  " set @Msg = 'Ekleme Başarılı';" +
                                   " select top 1 @RowId = MahalId from Mahal " +  //where MahalId = @PersonelId and DahiliNo = @DahiliNo and Gsm = @Gsm" +
                               " select cast(@RowId as int)RowId, @Msg Mesaj"; ;
                result = mc.Database.SqlQuery<GeneralResult>(query, new SqlParameter("MahalAdi", MahalAdi), new SqlParameter("DahiliNo", DahiliNo)
                    , new SqlParameter("MahalAmirId", mahalAmirId)
                      , new SqlParameter("IslemYapanId", islemYapanId)
                   , new SqlParameter("IP", ip)
                   , new SqlParameter("IslemTarih", islemTarihi)
                   ,new SqlParameter("Sira", Sira)
                    ).FirstOrDefault();
                islemId = 15;
            }
            else
            {
                var mahalIdInt = Convert.ToInt32(mahalId);
                query = "declare @RowId int,@Msg varchar(50); set @RowId=0; set @Msg=''; " +
               "update Mahal set MahalAdi=@MahalAdi,DahiliNo=@DahiliNo,MahalAmirId=@MahalAmirId,IslemYapanId=@IslemYapanId,IP=@IP,IslemTarihi=@IslemTarihi,Sira=@Sira where MahalId=@mahalIdInt ;" +
                  " set @Msg = 'Güncelleme Başarılı';" +
                                   " select top 1 @RowId = MahalId from Mahal where MahalId=@mahalIdInt; " +  //where MahalId = @PersonelId and DahiliNo = @DahiliNo and Gsm = @Gsm" +
                               " select cast(@RowId as int)RowId, @Msg Mesaj"; ;
                result = mc.Database.SqlQuery<GeneralResult>(query, new SqlParameter("MahalAdi", MahalAdi), new SqlParameter("DahiliNo", DahiliNo), new SqlParameter("MahalAmirId", mahalAmirId),
                    new SqlParameter("mahalIdInt", mahalIdInt)
                      , new SqlParameter("IslemYapanId", islemYapanId)
                   , new SqlParameter("IP", ip)
                   , new SqlParameter("IslemTarihi", islemTarihi)
                   , new SqlParameter("Sira", Sira)
                    ).FirstOrDefault();
                islemId = 17;
            }
            //ViewBag.Message = "";
            if (result != null && result.RowId != 0)
            {
                //başarılı
                TempData["Result"] = result.Mesaj;
                //  ViewBag.Message = result.Mesaj;

                //log işlemleri 
                //var ip = Common.GetIpAddress();
                var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LogKayit.sql"), Encoding.Default);
                var resultLog = mc.Database.SqlQuery<GeneralResult>(query2, new SqlParameter("SayfaId", 2), new SqlParameter("Aciklama", "MahalAdi: " + MahalAdi + " DahiliNo: " + DahiliNo)
                    , new SqlParameter("IslemYapanKullaniciId", Convert.ToInt32(Session["KullaniciId"]))
                    , new SqlParameter("IslemSaati", DateTime.Now)
                    , new SqlParameter("Ip", ip)
                    , new SqlParameter("IslemId", islemId)).FirstOrDefault(); // nöbetçi amirinin adı çekilecek
            }
            return RedirectToAction("MahalIslemleri");
        }

        [Obsolete]
        public JsonResult KullaniciEkle(List<string> dataUserArray)
        {
            try
            {
                var islemId = 0;
                if (dataUserArray == null || dataUserArray.Count < 1)
                    return Json(new { State = 2, Message = "Hata Oluştu!" }, JsonRequestBehavior.AllowGet);

                //şifre formatı kontrolü yapılır.
                var resultPassword = Common.ValidatePasswordRegular(dataUserArray[7]);
                if (!resultPassword)
                    return Json(new { State = 2, Message = "Parola Formatı Uymuyor. Tekrar Deneyin!<br>(En az 8, en fazla 15 karakter; 1 Küçük, 1 Büyük harf ve 1 tane Sayı girin.)", title = "Kullanıcı İşlemi" }, JsonRequestBehavior.AllowGet);


                var userId = dataUserArray[0];

                var sinifId = dataUserArray[1];
                var rutbeId = dataUserArray[2];
                var userName = dataUserArray[3];
                var userSurname = dataUserArray[4];
                var Email = dataUserArray[5];
                var Tcno = dataUserArray[6];
                var password = Common.EncodeStr(dataUserArray[7]); //parolayı şifreler 
                var BirlikId = Convert.ToInt32(dataUserArray[8]);
                var MahalId = Convert.ToInt32(dataUserArray[9]);
                var rolId = Convert.ToInt32(dataUserArray[10]);

                var NobettenCikar = dataUserArray[11];

                var girisYapanPerRolId = Session["RolId"].ToString();
                // giriş yapan nöbet kıdemlisi mi kontrol edilir.
                if (girisYapanPerRolId == "3")
                {
                    rolId = 0;
                    NobettenCikar = "False";
                }

                var aktif = Convert.ToInt32(dataUserArray[12]);

                var NobettenCikarBool = false;
                if (NobettenCikar == "True")
                    NobettenCikarBool = true;
                else
                    NobettenCikarBool = false;

                ModelContext mc = new ModelContext();
                var query = "";
                var result = new GeneralResult();

                var ip = Common.GetIpAddress();
                var islemYapanId = Convert.ToInt32(Session["KullaniciId"]);
                var islemTarihi = DateTime.Now;

                if (userId == "0") //ekleme işlemi
                {
                    query = "declare @RowId int,@Msg varchar(50),@IsExist tinyint; set @RowId=0; set @IsExist=0" +

                        " declare @KidemliSayisi int,@YetkiliAdi varchar(max),@YetkiliId int;" +
                        " set @KidemliSayisi=0;set @YetkiliAdi='';set @YetkiliId=0; " +

                    " select top 1 @IsExist=count(KullaniciId) from Kullanici Where Eposta=@Email or Tcno=@Tcno " +
                    " if @IsExist!=0 " +
                    " begin " +
                    "  set @Msg = 'Aynı T.C. Nolu veya E-postalı Personel Zaten Sistemde Mevcut!'; set @RowId=0 " +
                    " end " +
                    " else " +
                    " begin " +

                    " if @rolId=3" +
                    " begin" +
                    " select top 1 @YetkiliId=KullaniciId from Kullanici where MahalId=@MahalId and RolId=3" +
                    " if @YetkiliId != 0" +
                    " begin" +
                    " select @YetkiliAdi = dbo.Fn_FulAd(@YetkiliId, '');" +
                    "  set @Msg = 'Zaten mahal yetkili var!(' + @YetkiliAdi + ')'" +
                    " select @RowId RowId, @Msg Mesaj;" +
                    " return;" +
                    " end" +
                    " end " +

                    // query = "declare @RowId int,@Msg varchar(50); set @RowId=0; " +
                    " insert into Kullanici(SinifId,RutbeId,KullaniciAdi,KullaniciSoyadi,Eposta,Tcno,Parola,BirlikId,MahalId,RolId,Aktif,NobettenCikar," +
                     "IslemYapanId,IP,IslemTarihi) values(@SinifId," +
                    "@RutbeId,@userName,@userSurname,@Email,@Tcno,@password,@BirlikId,@MahalId,@rolId,@Aktif,@NobettenCikar,@IslemYapanId,@IP,@IslemTarihi) " +
                    " select top 1 @RowId = KullaniciId from Kullanici where Tcno = @Tcno" +
                    " set @Msg = 'Ekleme Başarılı'" +
                    " end" +
                    " select @RowId RowId, @Msg Mesaj";

                    var userAddResult = mc.Database.SqlQuery<GeneralResult>(query, new SqlParameter("SinifId", Convert.ToInt32(sinifId)),
                        new SqlParameter("RutbeId", Convert.ToInt32(rutbeId)),
                    new SqlParameter("userName", userName)
                   , new SqlParameter("userSurname", userSurname)
                   , new SqlParameter("Email", Email)
                   , new SqlParameter("Tcno", Tcno)
                   , new SqlParameter("password", password)
                   , new SqlParameter("BirlikId", BirlikId)
                   , new SqlParameter("MahalId", MahalId)
                   , new SqlParameter("rolId", rolId)
                   , new SqlParameter("NobettenCikar", NobettenCikarBool)
                   , new SqlParameter("Aktif", aktif)
                       , new SqlParameter("IslemYapanId", islemYapanId)
                   , new SqlParameter("IP", ip)
                   , new SqlParameter("IslemTarihi", islemTarihi)
                   ).FirstOrDefault();
                    if (userAddResult.RowId != 0)
                    {
                        islemId = 15;
                        //log işlemleri 
                       // var ip = Common.GetIpAddress();
                        var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LogKayit.sql"), Encoding.Default);
                        var resultLog = mc.Database.SqlQuery<GeneralResult>(query2, new SqlParameter("SayfaId", 4), new SqlParameter("Aciklama", "rutbeId: " + rutbeId + " userName: " + userName + " userSurname: " + userSurname
                            + " Email: " + Email + " Tcno: " + Tcno + " BirlikId: " + BirlikId + " MahalId: " + MahalId + " rolId: " + rolId)
                            , new SqlParameter("IslemYapanKullaniciId", Convert.ToInt32(Session["KullaniciId"]))
                            , new SqlParameter("IslemSaati", DateTime.Now)
                            , new SqlParameter("Ip", ip)
                            , new SqlParameter("IslemId", islemId)).FirstOrDefault();

                        //kullanıcı bilgilendirme için durum bilgisi eposta ile gönderilir 
                        var mailBaslik = "E-Nöbet Sistemi - Personel Güncelleme İşlemi";
                        var icerik = "Yetkili Tarafından Durum bilgisin:";
                        if (aktif == 1)
                            icerik = " Aktif ";
                        if (aktif == 2)
                            icerik = " Beklemede ";
                        if (aktif == 3)
                            icerik = " Tayin Oldu ";
                        if (aktif == 4)
                            icerik = " Emekli Oldu ";
                        var mailAciklamaHtml = "E-Nöbet Sistemine Nöbet Kıdemlisi Tarafından Kayııt Yapılmıştır. " +// + icerik + " olarak güncellenmiştir."+
                              "<a href='https://www.focaus.dzkk.tsk/enobet/Login/Login' target='_blank'>E-Nöbet Sistemi</a> ";
                        Common.HotmailMailSend3(Email, mailBaslik, mailAciklamaHtml);

                        return Json(new { State = 1, Message = "Ekleme Başarılı!", title = "Kullanıcı Ekleme İşlemi" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { State = 2, Message = userAddResult.Mesaj, title = "Kullanıcı Ekleme İşlemi" }, JsonRequestBehavior.AllowGet);
                }
                else // update 
                {
                    var girisYapanKullaniciId = Convert.ToInt32(Session["KullaniciId"].ToString());
                    var userIdint = Convert.ToInt32(userId);
                    if (girisYapanKullaniciId == userIdint && rolId != 1)
                        rolId = 3;

                    query = "declare @RowId int,@Msg varchar(50),@IsExist tinyint; set @RowId=0; set @IsExist=0 " +

                        " declare @KidemliSayisi int,@YetkiliAdi varchar(max),@YetkiliId int;" +
                        " set @KidemliSayisi=0;set @YetkiliAdi='';set @YetkiliId=0; " +

                   "select top 1 @IsExist=count(KullaniciId) from Kullanici Where (Eposta=@Email or Tcno=@Tcno) and  KullaniciId!=@userId " +
                   " if @IsExist!=0 " +
                   " begin " +
                   "  set @Msg = 'Aynı T.C. Nolu veya E-postalı Personel Zaten Sistemde Mevcut!'; set @RowId=0 " +
                   " end " +
                   " else " +
                   " begin " +


                    " if @rolId=3" +
                    " begin" +
                        " select top 1 @YetkiliId=KullaniciId from Kullanici where MahalId=@MahalId and RolId=3" +
                        " if @YetkiliId != 0" +
                        " begin" +
                            " if @userId!=@YetkiliId" +
                            " begin" +
                                " select @YetkiliAdi = dbo.Fn_FulAd(@YetkiliId, '');" +
                                "  set @Msg = 'Zaten mahal yetkili var!(' + @YetkiliAdi + ')'" +
                            " select @RowId RowId, @Msg Mesaj;" +
                            " return;" +
                        " end" +
                    " end" +
                    " end " +
                       //query = "declare @RowId int,@Msg varchar(50); set @RowId=0; " +
                       " update Kullanici set SinifId=@SinifId,RutbeId=@RutbeId,KullaniciAdi=@userName,KullaniciSoyadi=@userSurname,Eposta=@Email,Tcno=@Tcno,Parola=@password," +
                        " BirlikId=@BirlikId,MahalId=@MahalId,RolId=@rolId,Aktif=@Aktif,NobettenCikar=@NobettenCikar,IslemYapanId=@IslemYapanId,IP=@IP,IslemTarihi=@IslemTarihi where KullaniciId = @userId " +// where KullaniciId = @userId " +
                        " select top 1 @RowId = KullaniciId from Kullanici where KullaniciId = @userId" +
                        " set @Msg = 'Güncelleme Başarılı'" +
                         " end" +
                        " select @RowId RowId, @Msg Mesaj";

                    var userAddResult2 = mc.Database.SqlQuery<GeneralResult>(query, new SqlParameter("SinifId", Convert.ToInt32(sinifId)),
                        new SqlParameter("RutbeId", Convert.ToInt32(rutbeId)),
                    new SqlParameter("userName", userName)
                   , new SqlParameter("userSurname", userSurname)
                   , new SqlParameter("Email", Email)
                   , new SqlParameter("Tcno", Tcno)
                   , new SqlParameter("password", password)
                   , new SqlParameter("BirlikId", BirlikId)
                   , new SqlParameter("MahalId", MahalId)
                   , new SqlParameter("rolId", rolId)
                   , new SqlParameter("Aktif", aktif)
                   , new SqlParameter("NobettenCikar", NobettenCikarBool)
                    , new System.Data.SqlClient.SqlParameter("userId", userIdint)

                       , new SqlParameter("IslemYapanId", islemYapanId)
                   , new SqlParameter("IP", ip)
                   , new SqlParameter("IslemTarihi", islemTarihi)

                   ).FirstOrDefault();
                    if (userAddResult2.RowId != 0)
                    {
                        islemId = 17;
                        //log işlemleri 
                        //var ip = Common.GetIpAddress();
                        var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LogKayit.sql"), Encoding.Default);
                        var resultLog = mc.Database.SqlQuery<GeneralResult>(query2, new SqlParameter("SayfaId", 4), new SqlParameter("Aciklama", "rutbeId: " + rutbeId + " userName: " + userName + " userSurname: " + userSurname
                            + " Email: " + Email + " Tcno: " + Tcno + " BirlikId: " + BirlikId + " MahalId: " + MahalId + " rolId: " + rolId)
                            , new SqlParameter("IslemYapanKullaniciId", Convert.ToInt32(Session["KullaniciId"]))
                            , new SqlParameter("IslemSaati", DateTime.Now)
                            , new SqlParameter("Ip", ip)
                            , new SqlParameter("IslemId", islemId)).FirstOrDefault();

                        //kullanıcı bilgilendirme için durum bilgisi eposta ile gönderilir 
                        var mailBaslik = "E-Nöbet Sistemi - Personel Güncelleme İşlemi";
                        var icerik = "Yetkili Tarafından Durum bilgisi:";
                        if (aktif == 1)
                            icerik = " Aktif ";
                        if (aktif == 2)
                            icerik = " Beklemede ";
                        if (aktif == 3)
                            icerik = " Tayin Oldu ";
                        if (aktif == 4)
                            icerik = " Emekli Oldu ";
                        var mailAciklamaHtml = "E-Nöbet Sisteminde Nöbet Kıdemlisi Tarafından Durum Bilgisi " + icerik + " olarak güncellenmiştir." +
                              "<a href='https://*****/enobet/Login/Login' target='_blank'>E-Nöbet Sistemi</a> ";
                        Common.HotmailMailSend3(Email, mailBaslik, mailAciklamaHtml);

                        return Json(new { State = 1, Message = "Güncelleme Başarılı!", title = "Kullanıcı Güncelleme İşlemi" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(new { State = 2, Message = userAddResult2.Mesaj, title = "Kullanıcı Güncelleme İşlemi" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Obsolete]
        public async Task<JsonResult> KullaniciListele()
        {
            try
            {
                var mahalid = Convert.ToInt32(Session["MahalId"]);
                var query = "";
                if (Session["RolId"].ToString() == "1")
                    query = "DECLARE @result NVARCHAR(max);" +
                        " SET @result = (select KLSinif.Adi as Sinif,KLRutbe.Adi as Rutbe,K.KullaniciAdi,K.KullaniciSoyadi,K.Tcno,K.RolId,K.Eposta,case when ISNULL(M.MahalAdi, '') = '' then '' else M.MahalAdi END AS MahalAdi,K.KullaniciId,case when K.NobettenCikar=0 then 'Dahil' else 'Çıkarılmış' End as NobettenCikar,K.Aktif from Kullanici K left join Mahal M on K.MahalId = m.MahalId " +
                        "left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1 " +
                        "left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 " +
                        " for json path) SELECT @result;";  // where K.Aktif=1 
                if (Session["RolId"].ToString() == "3")
                {
                    query = "DECLARE @result NVARCHAR(max);" +
                            "SET @result = (select KLSinif.Adi as Sinif,KLRutbe.Adi as Rutbe,K.KullaniciAdi,K.KullaniciSoyadi,K.Tcno,K.RolId,K.Eposta,case when ISNULL(M.MahalAdi, '') = '' then '' else M.MahalAdi END AS MahalAdi,K.KullaniciId,case when K.NobettenCikar=0 then 'Dahil' else 'Çıkarılmış' End as NobettenCikar,K.Aktif from Kullanici K left join Mahal M on K.MahalId = m.MahalId " +
                             "left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId " + //and k.Aktif=1
                        "left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId " + //and k.Aktif = 1
                            " where K.Aktif not in(0)  and K.RolId not in(1) " +
                        " and K.MahalId=" +
                        mahalid +
                        //" in(select MahalId from NobetKidemli where KullaniciId=" + Session["KullaniciId"].ToString() + ")" +
                        " for json path) SELECT @result;";
                }

                var userList = string.Empty;
                using (ModelContext mc = new ModelContext())
                {

                    userList = await mc.Database.SqlQuery<string>(query).FirstOrDefaultAsync();
                    //Log Kayit
                    LogKayitEkle(mc, 4, 19, " RolId: " + Session["RolId"].ToString());
                    //_ = Task.Delay(5000);
                    //userList = await response.Content.ReadAsStringAsync();
                }
                JsonResult result = Json(new { State = 1, userList = userList, title = "Kullanıcı Listeleme İşlemi" }, JsonRequestBehavior.AllowGet);
                result.MaxJsonLength = Int32.MaxValue;
                return result;

                /*string result = string.Empty;

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }*/

                //  return Json(Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result));


            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public JsonResult editUserClk(string id)
        {
            try
            {
                var idInt = Convert.ToInt32(id);
                ModelContext mc = new ModelContext();
                var query = "select * from Kullanici where KullaniciId=@Id";// for json path";
                var userData = mc.Database.SqlQuery<KullaniciModel>(query, new SqlParameter("Id", idInt)).FirstOrDefault();

                var Password = Common.DecodeStr(userData.Parola);
                userData.Parola = Password;
                //var strData = JsonConvert.SerializeObject(userData);

                //Log Kayit
                LogKayitEkle(mc, 4, 20, " RolId: " + Session["RolId"].ToString() + " KullaniciId" + id);

                return Json(new { State = 1, userData = userData, title = "Kullanıcı Güncelleme İşlemi" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public JsonResult deleteUserClk(string id)
        {
            try
            {
                var idInt = Convert.ToInt32(id);
                ModelContext mc = new ModelContext();
                var query = "declare @RowId int, @Msg varchar(50); set @RowId = 0; set @Msg = ''; " +
                       "update Kullanici set Aktif=0 where KullaniciId=@Id                 " +
                       "set @Msg = 'Güncelleme Başarılı';" +
                       "select top 1 @RowId = NobetListeJsonId from NobetListeJson where NobetListeJsonId = @Id " +
                       "select cast(@RowId as int)RowId, @Msg Mesaj ";
                // var userData = mc.Database.SqlQuery<string>(query).FirstOrDefault();
                var result = mc.Database.SqlQuery<GeneralResult>(query, new System.Data.SqlClient.SqlParameter("Id", idInt)).FirstOrDefault();
                //Log Kayit
                LogKayitEkle(mc, 4, 16, " RolId: " + Session["RolId"].ToString() + " KullaniciId" + id);

                return Json(new { State = 1, title = "Kullanıcı Silme İşlemi", Message = "Silme Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //[PermissionsFilter]
        [Obsolete]
        public ActionResult NobetKullaniciAylikVeriGiris()
        {
            try
            {

                if (CheckIsPage(5) == false)
                    return RedirectToAction("Login", "Login");

                ModelContext mc = new ModelContext();
                var model = new NobetKullaniciAylikVeriGiris();

                var listOfParam = new List<SelectListItem>();
                /*var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
                listOfParam.Add(item1);*/
                var mahalDataList = GetMahalListDDL();// mc.Database.SqlQuery<MahalModel>("Select * from Mahal", Common.GetDBNullOrValue<int>(5)).ToList();
                //model.MahalList.AddRange(mahalDataList);
                var list = mahalDataList; //RolBLL.Select(exp).ToList();
                listOfParam.AddRange(list.Select(t => new SelectListItem { Text = t.MahalAdi.ToString(), Value = t.MahalId.ToString() }));
                ViewBag.ddlMahalId = listOfParam;

                //Log Kayit
                //LogKayitEkle(mc, 5, 22, " RolId: " + Session["RolId"].ToString());

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("/Shared/Error");
            }
        }

        [Obsolete]
        public async Task<JsonResult> MahalAyDoldur(List<string> dataList)
        {
            try
            {
                var Month = Convert.ToInt32(dataList[0]);
                var MahalId = Convert.ToInt32(dataList[1]);
                var YearStr = "";
                // List<KullaniciAylikVeri> AylikVeriDoldurList = new List<KullaniciAylikVeri>();
                var query = "";
                var userList = new List<KullaniciDdl>();

                var query2 = "";
                var NobetListeGetir = new NobetListe();

                var gecmisNobetListe = "";

                var date = DateTime.Now;
                var year = date.Year;
                var currentMonth = (date.Month);
                var currentDay = date.Day;

                bool isCurrentMonth = false;



                var firstDayOfMonth = new DateTime(year, Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                // hangi gün ile başlıyorsa string
                var firsthDayText = new DateTime(year, Month, 1).ToString("dddd");

                var firstDayNumber = firstDayOfMonth.Day;
                var lastDayNumber = lastDayOfMonth.Day;
                YearStr = year.ToString();
                // son ay ise gelecek yılı al...

                /*        if (Month == 12)
                            YearStr = (year + 1).ToString();
                        else
                        {*/
                // }
                //var MonthStr = Month.ToString();
                string MonthStr = "" + Month;
                /*if (Month < 10)
                {
                    MonthStr = "0" + Month;
                }*/

                //ModelContext mc = new ModelContext();

                //Log Kayit
                //LogKayitEkle(mc, 5, 19, " RolId: " + Session["RolId"].ToString() + " MahalId" + dataList[1]+ " Yıl:"+ YearStr+" Ay:"+ MonthStr);
                using (ModelContext mc = new ModelContext())
                {
                    if (currentMonth == Month || (currentDay >= 25 && Month == currentMonth + 1 )) //|| currentMonth+1 == Month     || Month == 1 || Month == 2 || Month == 3 || Month == 4
                    {
                        isCurrentMonth = true;
                        query = " select KullaniciId,(KLSinif.Adi+' '+ KLRutbe.Adi+' '+KullaniciAdi+' '+KullaniciSoyadi)FullAdi from Kullanici " +
                       "left join KategoriListe KLSinif on KLSinif.KategoriListeId = SinifId and KLSinif.Aktif=1 " +
                       "left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = RutbeId and KLRutbe.Aktif = 1 " +
                       "where MahalId=@MahalId and Kullanici.Aktif=1 and Kullanici.NobettenCikar=0 order by KLRutbe.KategoriListeId desc";
                        userList = mc.Database.SqlQuery<KullaniciDdl>(query, new System.Data.SqlClient.SqlParameter("MahalId", MahalId)
                         ).ToList();

                        // ModelContext mc = new ModelContext();
                        query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetListeSorgula_Yil_Ay_Mahal.sql"), Encoding.Default);
                        NobetListeGetir = mc.Database.SqlQuery<NobetListe>(query2, new System.Data.SqlClient.SqlParameter("Mahal", MahalId)
                            , new System.Data.SqlClient.SqlParameter("Month", Month),
                             new System.Data.SqlClient.SqlParameter("Year", YearStr)
                         ).FirstOrDefault();
                    }
                    else
                    {
                        /*    using (ModelContext mc = new ModelContext())
                            {*/
                        isCurrentMonth = false;
                        query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\gecmisNobetListe_Yil_Ay_Mahal_Getir.sql"), Encoding.Default);
                        gecmisNobetListe = mc.Database.SqlQuery<string>(query2, new SqlParameter("Mahal", MahalId)
                            , new System.Data.SqlClient.SqlParameter("Month", Month),
                             new System.Data.SqlClient.SqlParameter("Year", YearStr)).FirstOrDefault();


                        // JsonResult result = Json(new { State = 1, NobetList = NobetList }, JsonRequestBehavior.AllowGet);

                        /*}*/

                    }

                    //var id = 0;
                    //var data = "";
                    //if(NobetListeGetir != null)
                    //{
                    //    id = NobetListeGetir.Id;
                    //    data = NobetListeGetir.Data;
                    //}

                    var query3 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetYillikKullaniciHesabi.sql"), Encoding.Default);
                    var UserTotalYear = mc.Database.SqlQuery<UserTotalYearData>(query3, new SqlParameter("MahalId", MahalId),
                         new SqlParameter("Year", YearStr)
                     ).ToList();

                    //yıl bazında total hafta analizi
                    var aa = "";
                    var queryHaftalikAnaliz = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetMahalAylik_YillikAnalizHesabi.sql"), Encoding.Default);
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

                    //var array6 = new object[0, 8];

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

                        for (var i = 0; i < KullaniciList.Count; i++)
                        {
                            for (var y = 0; y < 7; y++)
                            {
                                if (array6[i, y] == null)
                                    array6[i, y] = 0;
                            }
                        }

                        list.Add(array6);
                        aa = JsonConvert.SerializeObject(array6);

                        /* JsonResult result2 = Json(new { State = 1, NobetList = NobetList, AnalizList = AnalizList, array6 = aa, UserTotalYear = UserTotalYear }, JsonRequestBehavior.AllowGet);
                         result2.MaxJsonLength = Int32.MaxValue;
                         return result2;*/
                    }

                    JsonResult result = Json(new { State = 1, Message = "", array6 = aa, isCurrentMonth = isCurrentMonth, gecmisNobetListe = gecmisNobetListe, UserTotalYear = UserTotalYear, firsthDayText = firsthDayText, lastDayNumber = lastDayNumber, firstDayOfMonth = firstDayOfMonth, firstDayOfMonthShortStr = firstDayOfMonth.ToShortDateString(), userList = userList, NobetListeGetir = NobetListeGetir }, JsonRequestBehavior.AllowGet);

                    result.MaxJsonLength = Int32.MaxValue;
                    return result;
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

        // total personel mahal yıllık hafta hesabı

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
        public class LastMahalAylik_HaftalikAnaliz
        {
            public int? KullaniciId { get; set; }
            public string GunAdi { get; set; }
            public string fulAdi { get; set; }
            public int Total { get; set; }
        }

        public async Task<JsonResult> NobetMazeretTarihAralikGetir(string mahalId, string Ay, string year)
        {
            try
            {
                var mahalIdInt = Convert.ToInt32(mahalId);
                var AyInt = Convert.ToInt32(Ay);
                var yeaIntr = Convert.ToInt32(year);

                var query = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetMazeretTarihAralik_Kullanici_Mahal_Getir.sql"), Encoding.Default);
                var dataResult = "";
                using (ModelContext mc = new ModelContext())
                {

                    dataResult = await mc.Database.SqlQuery<string>(query
                        , new SqlParameter("Ay", Ay)
                        , new SqlParameter("MahalId", mahalIdInt)
                        , new SqlParameter("Year", yeaIntr)
                        ).FirstOrDefaultAsync();
                    //Log Kayit
                    // LogKayitEkle(mc, 4, 19, " RolId: " + Session["RolId"].ToString());

                }

                JsonResult result = Json(new { State = 1, Message = "", dataResult = dataResult }, JsonRequestBehavior.AllowGet);

                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public JsonResult NobetListeKaydet(List<KullaniciJsonData> dataList, string id)
        {
            try
            {
                var dataListJson = JsonConvert.SerializeObject(dataList);
                ModelContext mc = new ModelContext();
                var query = "";
                var result = new GeneralResult();
                if (dataList.Count != 0) //!string.IsNullOrEmpty( 
                {
                    if (id == "0")
                    {
                        query = "declare @RowId int,@Msg varchar(50); set @RowId=0; set @Msg=''; " +
                  "Insert Into NobetListeJson(Data) Values(@Data);" +
                     " set @Msg = 'Ekleme Başarılı';" +
                                      " select top 1 @RowId = NobetListeJsonId from NobetListeJson " +
                                  " select cast(@RowId as int)RowId, @Msg Mesaj";
                        result = mc.Database.SqlQuery<GeneralResult>(query, new System.Data.SqlClient.SqlParameter("Data", dataListJson)).FirstOrDefault();

                        //Log Kayit
                        LogKayitEkle(mc, 5, 15, " RolId: " + Session["RolId"].ToString() + " dataListJson" + dataListJson);
 
                    }
                    else
                    {
                        query =
                       "    declare @RowId int, @Msg varchar(50); set @RowId = 0; set @Msg = '';                " +
                       "update NobetListeJson set Data=@Data where NobetListeJsonId = @Id                  " +
                       "set @Msg = 'Güncelleme Başarılı';                                                       " +
                       "select top 1 @RowId = NobetListeJsonId from NobetListeJson where NobetListeJsonId = @Id " +
                       "select cast(@RowId as int)RowId, @Msg Mesaj ";
                        var nöbetIdInt = Convert.ToInt32(id);
                        result = mc.Database.SqlQuery<GeneralResult>(query, new System.Data.SqlClient.SqlParameter("Data", dataListJson)
                            , new System.Data.SqlClient.SqlParameter("Id", nöbetIdInt)).FirstOrDefault();

                        //Log Kayit
                        LogKayitEkle(mc, 5, 17, " RolId: " + Session["RolId"].ToString() + " dataListJson" + dataListJson + " NobetListeJsonId:" + id);

                    }
                }
                else
                {
                    return Json(new { State = 2, Message = "Hatalı!" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { State = 1, Message = result.Mesaj }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public ActionResult NobetKidemliIslemleri()
        {
            try
            {
                if (CheckIsPage(4) == false)
                    return RedirectToAction("Login", "Login");

                var model = new MahalModel();
                var listOfParam = new List<SelectListItem>();
                //var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
                //listOfParam.Add(item1);
                var mahalDataList = GetMahalListDDL();// mc.Database.SqlQuery<MahalModel>("Select * from Mahal", Common.GetDBNullOrValue<int>(5)).ToList();
                //model.MahalList.AddRange(mahalDataList);
                var list = mahalDataList; //RolBLL.Select(exp).ToList();
                listOfParam.AddRange(list.Select(t => new SelectListItem { Text = t.MahalAdi.ToString(), Value = t.MahalId.ToString() }));
                ViewBag.ddlMahalId = listOfParam;
                //ViewBag.ddlMahalId = GetMahalListDDL();
                model.MahalKidemli.SelectedValues = new List<string> { "1", "2" };
                //Log Kayit
                var mc = new ModelContext();
                LogKayitEkle(mc, 10, 19, " RolId: " + Session["RolId"].ToString());

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string term)
        {
            try
            {
                var mc = new ModelContext();
                var query = "select cast(KullaniciId as varchar) as value, (KLSinif.Adi+' '+ KLRutbe.Adi +' '+KullaniciAdi+' '+KullaniciSoyadi) as label from Kullanici " +
                    "left join KategoriListe KLSinif on KLSinif.KategoriListeId = SinifId and KLSinif.Aktif=1 " +
                    "left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = RutbeId and KLRutbe.Aktif = 1 " +
                    "Where RolId=3 and (KullaniciAdi like '%" + term + "%' or KullaniciSoyadi like '%" + term + "%') order by KullaniciAdi";
                var result = mc.Database.SqlQuery<UserData>(query).Select(x => new { label = x.label, value = x.value.ToString() }).ToList();
                //    var data = result.Select(m => new { label = m.label,value=m.value }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Hatalı Veri!", JsonRequestBehavior.AllowGet);
            }
            /* var data = names.Where(m => m.Contains(term)).Select(m => new { label = m });

             return Json(data, JsonRequestBehavior.AllowGet);*/
        }

        [Obsolete]
        public ActionResult KidemKullaniciMahalListGetir(string uid)
        {
            try
            {
                ModelContext mc = new ModelContext();
                var uidint = Convert.ToInt32(uid);
                var query2 = "select MahalId from NobetKidemli where KullaniciId=@KullaniciId for json path";//System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetKidemliListe.sql"), Encoding.Default);
                var kidemliMahalList = mc.Database.SqlQuery<string>(query2, new SqlParameter("KullaniciId", uidint)).FirstOrDefault();
                //Log Kayit
                LogKayitEkle(mc, 10, 20, " RolId: " + Session["RolId"].ToString() + " kidemliMahalList:" + kidemliMahalList);

                return Json(new { State = 1, kidemliMahalList = kidemliMahalList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public JsonResult KidemKullaniciMahalEkle(List<string> values, string uid)
        {
            try
            {
                ModelContext mc = new ModelContext();
                var queryLogKayit = "";
                var query2 = "declare @RowId int, @Msg varchar(50); set @RowId = 0; set @Msg = ''; " +
                    "delete from NobetKidemli where KullaniciId=" + uid + " ";
                //var valuesArray = values.Split(',');
                foreach (var value in values)
                {
                    query2 += " insert into NobetKidemli(MahalId, KullaniciId) Values(" + value + ", " + uid + ")";
                    //log işlemleri 
                    var ip = Common.GetIpAddress();
                    queryLogKayit += " insert into LogKayit(SayfaId,Aciklama,IslemYapanKullaniciId,IslemSaati,Ip,IslemId) " +
                        "Values(" + 10 + "," + "'YetkiVerilenMahalId:" + value + "YetkiVerilenKullaniciId:" + uid + "', " + Convert.ToInt32(Session["KullaniciId"]) + ", '" + DateTime.Now.ToString() + "', '" + ip.ToString() + "'," + 15 + ")";
                }
                query2 += " set @Msg = 'Ekleme Başarılı';" +
                                        " select top 1 @RowId = MahalId from NobetKidemli " +  //where MahalId = @PersonelId and DahiliNo = @DahiliNo and Gsm = @Gsm" +
                                   " select cast(@RowId as int)RowId, @Msg Mesaj";

                var result = new GeneralResult();
                result = mc.Database.SqlQuery<GeneralResult>(query2).FirstOrDefault();


                //başarılı
                if (result != null && result.RowId != 0)
                {
                    //log işlemleri 
                    mc.Database.SqlQuery<object>(queryLogKayit).FirstOrDefault();
                    return Json(new { State = 1, Message = result.Mesaj }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { State = 2, Message = result.Mesaj }, JsonRequestBehavior.AllowGet);

                }

                //System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetKidemliListe.sql"), Encoding.Default);

            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public JsonResult deleteMahalClk(string id)
        {
            try
            {
                var idInt = Convert.ToInt32(id);
                ModelContext mc = new ModelContext();
                var query = "declare @RowId int, @Msg varchar(50); set @RowId = 0; set @Msg = ''; " +
                       "update Mahal set Aktif=0 where MahalId=@Id                 " +
                       "set @Msg = 'Silme Başarılı';" +
                       "select top 1 @RowId = MahalId from Mahal where MahalId = @Id " +
                       "select cast(@RowId as int)RowId, @Msg Mesaj ";
                // var userData = mc.Database.SqlQuery<string>(query).FirstOrDefault();
                var result = mc.Database.SqlQuery<GeneralResult>(query, new System.Data.SqlClient.SqlParameter("Id", idInt)).FirstOrDefault();
                if (result != null && result.RowId != 0)
                {
                    //başarılı
                    TempData["Result"] = result.Mesaj;

                    //log işlemleri 
                    var ip = Common.GetIpAddress();
                    var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LogKayit.sql"), Encoding.Default);
                    var resultLog = mc.Database.SqlQuery<GeneralResult>(query2, new SqlParameter("SayfaId", 2), new SqlParameter("Aciklama", "MahalId: " + id)
                        , new SqlParameter("IslemYapanKullaniciId", Convert.ToInt32(Session["KullaniciId"]))
                        , new SqlParameter("IslemSaati", DateTime.Now)
                        , new SqlParameter("Ip", ip)
                        , new SqlParameter("IslemId", 16)).FirstOrDefault();

                    //  ViewBag.Message = result.Mesaj;
                }
                return Json(new { State = 1, title = "Mahal Silme İşlemi", Message = "Silme Başarılı" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public JsonResult editOneListMahalClk(string mid)
        {
            try
            {
                var mc = new ModelContext();
                //log işlemleri 
                var ip = Common.GetIpAddress();
                var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LogKayit.sql"), Encoding.Default);
                var resultLog = mc.Database.SqlQuery<GeneralResult>(query2, new SqlParameter("SayfaId", 2), new SqlParameter("Aciklama", "MahalId: " + mid)
                    , new SqlParameter("IslemYapanKullaniciId", Convert.ToInt32(Session["KullaniciId"]))
                    , new SqlParameter("IslemSaati", DateTime.Now)
                    , new SqlParameter("Ip", ip)
                    , new SqlParameter("IslemId", 20)).FirstOrDefault();

                return Json(new { State = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public JsonResult deleteUserKidemliClk(string id)
        {
            try
            {

                ModelContext mc = new ModelContext();
                //log için sorgu 
                var querySelectKidemliForLog = "select top 1 M.MahalAdi,Sinif.Adi+' '+ Rutbe.Adi + ' '+ K.KullaniciAdi+' '+K.KullaniciSoyadi as FulAdi from NobetKidemli NK join Kullanici K on NK.KullaniciId=K.KullaniciId join KategoriListe Sinif on Sinif.KategoriListeId=K.SinifId join KategoriListe Rutbe on Rutbe.KategoriListeId=K.RutbeId join Mahal M on M.MahalId=NK.MahalId where NK.NobetKidemliId=" + id;

                var query2 = "delete from NobetKidemli where NobetKidemliId=" + id + " select top 1 NobetKidemliId from NobetKidemli where NobetKidemliId=" + id;//System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetKidemliListe.sql"), Encoding.Default);
                var NobetKidemliId = mc.Database.SqlQuery<int?>(query2).FirstOrDefault();
                if (NobetKidemliId == null)
                {
                    //Log Kayit
                    //kullanici adi ve mahal adını bulll
                    var resultDetailJson = mc.Database.SqlQuery<string>(querySelectKidemliForLog + " for json path").FirstOrDefault();
                    LogKayitEkle(mc, 10, 16, " RolId: " + Session["RolId"].ToString() + " NobetKidemliId:" + id);
                    return Json(new { State = 1, Message = "Silme İşlemi Başarılı." }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { State = 2, Message = "Silme İşlemi Hatalı!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [Obsolete]
        public JsonResult KidemKullaniciListele()
        {
            try
            {

                ModelContext mc = new ModelContext();
                var query2 = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\NobetKidemliListe.sql"), Encoding.Default);
                var userList = mc.Database.SqlQuery<string>(query2).FirstOrDefault();
                //Log Kayit
                LogKayitEkle(mc, 10, 19, " RolId: " + Session["RolId"].ToString());

                return Json(new { State = 1, userList = userList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LogKayitSayfa()
        {
            try
            {
                var model = new LogModel();

                //işlem tür listesi
                var islemTurQuery = "select KategoriListeId,Adi from KategoriListe where KategoriValue =3 and KategoriListeId in(15,17) and Aktif=1";
                ModelContext mc = new ModelContext();
                var dataList = new List<KullaniciModel.KullaniciKategoriListe>();
                dataList = mc.Database.SqlQuery<KullaniciModel.KullaniciKategoriListe>(islemTurQuery).ToList();

                var listOfParam = new List<SelectListItem>();
                //var item1 = new SelectListItem { Text = "-Seçiniz-", Value = "" };
                //listOfParam.Add(item1);
                var list = dataList;//mahalDataList; //RolBLL.Select(exp).ToList();
                listOfParam.AddRange(list.Select(t => new SelectListItem { Text = t.Adi.ToString(), Value = t.KategoriListeId.ToString() }));
                ViewBag.ddlIslemId = listOfParam;

                //sayfa listesi
                var sayfaQuery = "select SayfaId,Adi from Sayfa where Aktif=1 and SayfaId in(2,4)";
                var dataListSayfa = new List<KullaniciModel.KullaniciKategoriListe>();
                var dataSayfaList = mc.Database.SqlQuery<KullaniciModel.SayfaModel>(sayfaQuery).ToList();

                var listOfParamSayfa = new List<SelectListItem>();
                var item1Sayfa = new SelectListItem { Text = "-Seçiniz-", Value = "" };
                //listOfParamSayfa.Add(item1);
                var listSayfa = dataSayfaList;//mahalDataList; //RolBLL.Select(exp).ToList();
                listOfParamSayfa.AddRange(listSayfa.Select(t => new SelectListItem { Text = t.Adi.ToString(), Value = t.SayfaId.ToString() }));
                ViewBag.ddlSayfaId = listOfParamSayfa;

                //işlem yapan personel listesi

                //tarihi ara

                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("/Shared/Error");
            }
        }

        public async Task<JsonResult> LogGetir(string Ip, string IslemYapanId, string IslemYapilanId, string SayfaId, string IslemId)
        {
            try
            {
                var dataResult = "";
                var query = "";
                var IslemIdInt = 0;

                if (string.IsNullOrEmpty(IslemId))
                    return null;
                else
                    IslemIdInt = Convert.ToInt32(IslemId);
                if (SayfaId == "2") // mahal işlemleri
                {
                    dataResult = await LogMahalGetir(IslemYapanId, Ip, IslemYapilanId, IslemId);
                }
                if (SayfaId == "4") //personel işlemleri
                {
                    query = "declare @jsonData varchar(max); set @jsonData = (";
                    //query += "Select * from MahalLog Where IslemId=@IslemId ";//
                    query += System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LOG_KullaniciLog.sql"), Encoding.Default);
                    var sartIfadeler = "";
                    if (!string.IsNullOrEmpty(Ip))
                    {
                        sartIfadeler += " and Ip=@Ip ";
                    }
                    if (!string.IsNullOrEmpty(IslemYapanId))
                    {
                        var IslemYapanIdInt = Convert.ToInt32(IslemYapanId);
                        sartIfadeler += " and IslemYapanId=@IslemYapanId ";
                    }
                    if (!string.IsNullOrEmpty(IslemYapilanId))
                    {
                        var IslemYapilanIdInt = Convert.ToInt32(IslemYapilanId);
                        sartIfadeler += " and KullaniciId=@IslemYapilanId ";
                    }
                    //var addWhere = "";
                    query += sartIfadeler;
                    query += " for json path); select @jsonData;";
                    //query += sartIfadeler;
                    //query += " json for path";
                    //query.Replace(addWhere);

                    using (ModelContext mc = new ModelContext())
                    {

                        dataResult = await mc.Database.SqlQuery<string>(query
                            // , new SqlParameter("addWhere", addWhere)
                            , new SqlParameter("IslemId", IslemIdInt)
                            , new SqlParameter("Ip", Ip)
                            ).FirstOrDefaultAsync();
                        //Log Kayit
                        // LogKayitEkle(mc, 4, 19, " RolId: " + Session["RolId"].ToString());

                    }
                }



                JsonResult result = Json(new { State = 1, Message = "", dataResult = dataResult }, JsonRequestBehavior.AllowGet);

                result.MaxJsonLength = Int32.MaxValue;
                return result;
            }
            catch (Exception ex)
            {
                return Json(new { State = 3, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<string> LogMahalGetir(string IslemYapanId,string Ip,string IslemYapilanId,string IslemId)
        {
            var dataResult = "";
            var query = "";
            byte IslemIdByte = Convert.ToByte(IslemId);

            query = "declare @jsonData varchar(max); set @jsonData = (";
            query += System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~"), @"SQL_query\LOG_MahalLog.sql"), Encoding.Default);
            var sartIfadeler = "";
            if (!string.IsNullOrEmpty(Ip))
            {
                sartIfadeler += " and Ip=@Ip ";
            }
            if (!string.IsNullOrEmpty(IslemYapanId))
            {
                var IslemYapanIdInt = Convert.ToInt32(IslemYapanId);
                sartIfadeler += " and IslemYapanId=@IslemYapanId ";
            }
            if (!string.IsNullOrEmpty(IslemYapilanId))
            {
                var IslemYapilanIdInt = Convert.ToInt32(IslemYapilanId);
                sartIfadeler += " and KullaniciId=@IslemYapilanId ";
            }
            //var addWhere = "";
            query += sartIfadeler;
            query += "for json path); select @jsonData;";
            //query += sartIfadeler;
            //query += " json for path";
            //query.Replace(addWhere);

            using (ModelContext mc = new ModelContext())
            {

                dataResult = await mc.Database.SqlQuery<string>(query
                    // , new SqlParameter("addWhere", addWhere)
                    , new SqlParameter("IslemId", IslemIdByte)
                    , new SqlParameter("Ip", Ip)
                    ).FirstOrDefaultAsync();
                //Log Kayit
                // LogKayitEkle(mc, 4, 19, " RolId: " + Session["RolId"].ToString());

            }
            return dataResult;
        }
        public class UserData
        {
            public string value { get; set; }
            public string label { get; set; }
        }
        private List<string> names = new List<string>
     {
         "adana",
         "antalya",
         "ankara",
         "trabzon",
         "tokat",
         "istanbul",
         "izmir",
         "içel",
     };

        [HttpPost]
        public string FillAutoComplete(string value)
        {
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
            dataDictionary.Add("jQuery Validation of Email, Number, Checkbox and More", "https://www.yogihosting.com/using-jquery-to-validate-a-form/");
            dataDictionary.Add("jQuery Uncheck Checkbox Tutorial", "https://www.yogihosting.com/check-uncheck-all-checkbox-using-jquery/");
            dataDictionary.Add("Free WordPress Slider Built In jQuery", "https://www.yogihosting.com/wordpress-image-slider-effect-with-meta-slider/");
            dataDictionary.Add("Creating jQuery Expand Collapse Panels In HTML", "https://www.yogihosting.com/creating-expandable-collapsible-panels-in-jquery/");
            dataDictionary.Add("jQuery AJAX Events Complete Guide for Beginners and Experts", "https://www.yogihosting.com/jquery-ajax-events/");
            dataDictionary.Add("How to Create a Web Scraper in ASP.NET MVC and jQuery", "https://www.yogihosting.com/web-scraper/");
            dataDictionary.Add("CRUD Operations in Entity Framework and ASP.NET MVC", "https://www.yogihosting.com/crud-operations-entity-framework/");
            dataDictionary.Add("Implementing TheMovieDB (TMDB) API in ASP.NET MVC", "https://www.yogihosting.com/implement-themoviedb-api/");
            dataDictionary.Add("ASP.NET MVC Data Annotation – Server Side Validation of Controls", "https://www.yogihosting.com/server-side-validation-asp-net-mvc/");
            dataDictionary.Add("How to use CKEditor in ASP.NET MVC", "https://www.yogihosting.com/ckeditor-tutorial-asp-net-mvc/");

            StringBuilder sb = new StringBuilder();
            sb.Append("<select id=\"autoCompleteSelect\" size=\"5\">");

            foreach (KeyValuePair<string, string> entry in dataDictionary)
            {
                if (entry.Key.IndexOf(value, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    sb.Append("<option value=\"" + entry.Value + "\">" + entry.Key + "</option>");
            }

            sb.Append("</select>");
            return sb.ToString();
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
        /* public bool PermissonCheck()
         {
             return true;
         }*/
        /*public class SessionControl
        {
            public bool SessionCheck
            {
                get
                {
                    var httpContext = new HttpContext(httpRequest, httpResponse);
                    object aa = Session["KullaniciAdi"];
                    if ( == null)
                    {
                        return false;
                    }
                }
            }
        }*/

        public class PermissionsFilter : AuthorizeAttribute
        {
            // private readonly PermissionManager _permissionsManager;
            //   private readonly PermissionManager _permissionsManager;

            // public PermissionsFilter(string permissionName)
            /*   {
                   _permissionName = permissionName;
                   _permissionsManager = new PermissionServiceManager();
               }*/
            /*public PermissionsFilter(int pageId)
            {
                //OnAuthorization(OnAuthorization)
                return 11;
                //return 
             //   _permissionName = permissionName;
            //    _permissionsManager = new PermissionServiceManager();
            }*/
            /* public override void OnAuthorization(AuthorizationContext filterContext)
             {
                 base.OnAuthorization(filterContext);
                 //CheckIfUserIsAuthenticated(filterContext);
             }*/
            /*  public override void OnAuthorization(AuthorizationContext filterContext)
              {
                //  if (!_permissionServiceManager.CanAccessPermission(_permissionName))
                  {
                      var urlHelper = new UrlHelper(filterContext.RequestContext);
                      var url = urlHelper.Action("Unauthorised", "Home");
                      filterContext.Result = new RedirectResult(url);
                  }
              }*/
        }
        private class RedirectController : Controller
        {
            public ActionResult RedirectWhereever()
            {
                return RedirectToAction("Action", "Controller");
            }

        }
        static void CheckIfUserIsAuthenticated(AuthorizationContext filterContext)
        {
            if (filterContext.Result == null)
                return;
            if (filterContext.HttpContext.Session["RolId"] == null)
            {
                string retUrl = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result =
                                 new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                                         {{ "controller", "Main" },
                          { "action", "Home" },
                          { "returnUrl",    retUrl } });
            }
        }
        public ActionResult Istatistik()
        {
            return View();
        }

        public class GeneralResult
        {
            public int RowId { get; set; }
            public string Mesaj { get; set; }
        }
        public class KullaniciAylikVeri
        {
            public int Gun { get; set; }
            public string KullaniciAdi { get; set; }

        }
        public class KullaniciDdl
        {
            public int KullaniciId { get; set; }
            public string FullAdi { get; set; }
        }
        public class KullaniciJsonData
        {
            public int KullaniciId { get; set; }
            public int MahalId { get; set; }
            public string NobetTarihi { get; set; }
        }
        public class NobetListe
        {
            public int Id { get; set; }
            public string Data { get; set; }
        }
    }

}

// 2 döngü olsun ilki hangi günden başlıyorsa o günlere 0 basalım 
//var firstLoopStartNumber = 0;
//if (firstDayOfMonthDay == "Pazartesi") firstLoopStartNumber = 0;
//if (firstDayOfMonthDay == "Salı") firstLoopStartNumber = 1;
//if (firstDayOfMonthDay == "Çarşamba") firstLoopStartNumber = 2;
//if (firstDayOfMonthDay == "Perşembe") firstLoopStartNumber = 3;
//if (firstDayOfMonthDay == "Cuma") firstLoopStartNumber = 4;
//if (firstDayOfMonthDay == "Cumartesi") firstLoopStartNumber = 5;
//if (firstDayOfMonthDay == "Pazar") firstLoopStartNumber = 6;
//var i = 0;
//var y =0;
//var dayNumber = 0;
//for (i=0;i< firstLoopStartNumber; i++)
//{
//    var AylikVeriDoldur = new KullaniciAylikVeri();
//    AylikVeriDoldur.Gun = 0;
//    AylikVeriDoldurList.Add(AylikVeriDoldur);

//}
//y = i;
//for(var x=0; x < 6; x++)
//{
//for (y = 0; y <= lastDayNumber; i++)
//{
//    var AylikVeriDoldur = new KullaniciAylikVeri();
//    AylikVeriDoldur.Gun = dayNumber + 1;
//    AylikVeriDoldurList.Add(AylikVeriDoldur);
//}
//}