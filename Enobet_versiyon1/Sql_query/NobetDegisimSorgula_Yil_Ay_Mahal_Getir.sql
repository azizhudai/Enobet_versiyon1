/*declare @nowDate Date,@jData varchar(max),@NLJsonId int;set @NLJsonId=0;--SELECT @nowDate=FORMAT (getdate(), 'yyyy-MM-1') ; --select @nowDate;
select @jData=(NL.Data),@NLJsonId=NL.NobetListeJsonId from NobetListeJson NL where (JSON_VALUE(NL.Data, '$[0].NobetTarihi') like (cast(@Yil as varchar(4))+'-'+cast(@Ay as varchar(2))+'-%')) and JSON_VALUE(NL.Data, '$[0].MahalId')=@MahalId
select NLTableList.KullaniciId,NLTableList.NobetTarihi,KLSinif.Adi+' '+KLRutbe.Adi+' '+K.KullaniciAdi+' '+K.KullaniciSoyadi as KullaniciFulAdi from OPENJSON(@jData)WITH (KullaniciId int,
       NobetTarihi nvarchar(max)) as NLTableList
	   inner join Kullanici K On K.KullaniciId=NLTableList.KullaniciId
	  left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 */


   /*declare @nowDate Date,@jData varchar(max),@JNobetListe varchar(max),@NLJsonId int;set @NLJsonId=0;--@Yil varchar(4),@Ay varchar(2),@MahalId int;set @MahalId=2;set @Yil='2021';set @Ay='02';set @NLJsonId=0;--SELECT @nowDate=FORMAT (getdate(), 'yyyy-MM-1') ; --select @nowDate;
select @jData=(NL.Data),@NLJsonId=NL.NobetListeJsonId from NobetListeJson NL where (JSON_VALUE(NL.Data, '$[0].NobetTarihi') like (cast(@Yil as varchar(4))+'-'+cast(@Ay as varchar(2))+'-%')) and JSON_VALUE(NL.Data, '$[0].MahalId')=@MahalId
set @JNobetListe=(select NLTableList.KullaniciId,NLTableList.NobetTarihi,KLSinif.Adi+' '+KLRutbe.Adi+' '+K.KullaniciAdi+' '+K.KullaniciSoyadi as KullaniciFulAdi from OPENJSON(@jData)WITH (KullaniciId int,
       NobetTarihi nvarchar(max)) as NLTableList
	   inner join Kullanici K On K.KullaniciId=NLTableList.KullaniciId
	  left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 for json path)

select @JNobetListe as NobetListeJson, @NLJsonId as NobetListeId*/

 declare @nowDate Date,@jData varchar(max),@JNobetListe varchar(max),@NLJsonId int,@MahalAmirId int,@AmirFulAdi varchar(max),@Eposta varchar(max),@NobetKidemliId int,@NobetKidemliAdi varchar(50);--@Yil varchar(4),@Ay varchar(2),@MahalId int;

	   set @MahalAmirId=0;set @NLJsonId=0;--set @MahalId=2;set @Yil='2021';set @Ay='02';--SELECT @nowDate=FORMAT (getdate(), 'yyyy-MM-1') ; --select @nowDate;
select @jData=(NL.Data),@NLJsonId=NL.NobetListeJsonId from NobetListeJson NL where 

MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) =@Ay and Year(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) =@Yil
--(JSON_VALUE(NL.Data, '$[0].NobetTarihi') like (cast(@Yil as varchar(4))+'-'+cast(@Ay as varchar(2))+'-%')) 

and JSON_VALUE(NL.Data, '$[0].MahalId')=@MahalId
set @JNobetListe=(select NLTableList.KullaniciId,NLTableList.NobetTarihi,KLSinif.Adi+' '+KLRutbe.Adi+' '+K.KullaniciAdi+' '+K.KullaniciSoyadi as KullaniciFulAdi from OPENJSON(@jData)WITH (KullaniciId int,
       NobetTarihi nvarchar(max)) as NLTableList
	   inner join Kullanici K On K.KullaniciId=NLTableList.KullaniciId
	  left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 
--değil edildi (nöbetten çıkarılmış ise listede gözükmez)
where K.NobettenCikar=0 
order by cast(NLTableList.NobetTarihi AS date)
for json path)

select top 1 @MahalAmirId=MahalAmirId from Mahal where MahalId=@MahalId --where Ma

select @AmirFulAdi=KLSinif.Adi+' '+KLRutbe.Adi+' '+K.KullaniciAdi+' '+K.KullaniciSoyadi,@Eposta=K.Eposta  from Kullanici K
  left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 
where KullaniciId=@MahalAmirId;

select @NobetKidemliId=KullaniciId from Kullanici where RolId=3 and MahalId=@MahalId
select @NobetKidemliAdi=dbo.Fn_FulAd(@NobetKidemliId,'');

select @JNobetListe as NobetListeJson, @NLJsonId as NobetListeId, (select top 1 MahalAdi from Mahal where MahalId=@MahalId) as MahalAdi,
@AmirFulAdi as AmirFulAdi,@MahalAmirId as MahalAmirId,@Eposta as AmirEposta,@NobetKidemliId as NobetKidemliId,@NobetKidemliAdi as NobetKidemliAdi