 declare --@KullaniciId int,@Ay int,@MahalId int,
 @jData varchar(max),@result NVARCHAR(max);
 /*
 set @Ay=2;
 set @KullaniciId=20014;
 set @MahalId = 2;*/

 set LANGUAGE turkish;

select @jData=(NL.Data)
  from 
  NobetListeJson as NL where MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) =@Month 
  and JSON_VALUE(NL.Data, '$[0].MahalId')=@MahalId 
  and Year(JSON_VALUE(NL.Data, '$[0].NobetTarihi'))=@Year;

  set @result=(select dbo.Fn_FulAd(KullaniciId,'') fulAdi,
  (select MahalAdi from Mahal where MahalId=@MahalId) as MahalAdi,
 
  --CAST(DATEPART(d, NobetTarihi) AS VARCHAR) as GunSayi,
  FORMAT(cast(NobetTarihi as date),'M')+' '+ FORMAT(cast(NobetTarihi as date),'dddd')+ ' '+cast(Year(cast(NobetTarihi as date))as varchar(4)) as TarihDetay
  from OPENJSON(@jData)WITH (KullaniciId int,
       NobetTarihi nvarchar(max)) as NLTableList

	   where KullaniciId = @KullaniciId order by cast(NobetTarihi as date) for json path)
	     select @result;
	   /*inner join Kullanici K On K.KullaniciId=NLTableList.KullaniciId
	  left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 */

--yıl,mahal, filter