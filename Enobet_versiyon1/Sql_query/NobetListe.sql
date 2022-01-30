  --declare @Day int,@Month int,@Year int;set @Day =12;set @Month =10;set @Year=2020

   SET LANGUAGE turkish;

  DECLARE @result NVARCHAR(max);
  SET @result = (SELECT
 KLSinif.Adi+' '+ KLRutbe.Adi+' '+ K.KullaniciAdi+' '+K.KullaniciSoyadi As KullaniciFulAdi,
 isnull(M.MahalAdi,'') As MahalAdi,
 M.DahiliNo As DahiliNo,
 JSON_VALUE(N.Data,'$['+cast(@Day as varchar)+'].KullaniciId') As KullaniciId,

 --JSON_VALUE(N.Data,'$['+cast(@Day as varchar)+'].NobetTarihi') As NobetTarihi
  FORMAT(cast(JSON_VALUE(N.Data,'$['+cast(@Day as varchar)+'].NobetTarihi') as date),'d MMMM yyyy dddd') as NobetTarihi
 --cast(FORMAT(cast(JSON_VALUE(N.Data,'$['+cast(@Day as varchar)+'].NobetTarihi') as Date),'dddd','tr') as varchar) As NobetTarihi

FROM [NobetListeJson] N inner join Kullanici K on JSON_VALUE(N.Data,'$['+cast(@Day as varchar)+'].KullaniciId') = K.KullaniciId
left join Mahal M on JSON_VALUE(N.Data,'$['+cast(@Day as varchar)+'].MahalId') = M.MahalId
left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 
where cast(JSON_VALUE(Data,'$['+cast(@Day as varchar)+'].NobetTarihi')as date) = cast(@Year as varchar)+'-'+cast(@Month as varchar)+'-'+cast((@Day+1) as varchar)
and M.Aktif=1 order by M.Sira
for json path) SELECT @result;