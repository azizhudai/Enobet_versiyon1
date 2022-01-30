  declare @jData varchar(max), @result NVARCHAR(max);Set @jData = '';

  SET LANGUAGE turkish;

  -- declare @Year varchar(10),@Month varchar(10),@Mahal varchar(10);set @Year =2020;set @Month=11;set @Mahal=2;
  SET @result = (
 SELECT  userLists.KullaniciId,KLSinif.Adi+' '+ KLRutbe.Adi+' '+ K.KullaniciAdi+' '+K.KullaniciSoyadi as FullName,
 
 --userLists.NobetTarihi,
 FORMAT(userLists.NobetTarihi,'d MMMM yyyy dddd') as NobetTarihi,
 
 M.MahalAdi
FROM NobetListeJson NL
CROSS APPLY OPENJSON (NL.Data, N'$')
  WITH (
    KullaniciId int,
	MahalId int,
	NobetTarihi Date)
      AS userLists
	    left join Kullanici K on K.KullaniciId=userLists.KullaniciId
		left join Mahal M on M.MahalId=userLists.MahalId
left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId
	  where 
	  cast(JSON_VALUE(NL.Data,'$[0].NobetTarihi') as date) = cast(@Year as varchar)+'-'+cast(@Month as varchar)+'-1'  --+cast((@Day+1) as varchar)
	  --JSON_VALUE(NL.Data, '$[0].NobetTarihi') =@Year+'-'+@Month+'-1'
	  
	  and JSON_VALUE(NL.Data, '$[0].MahalId') = @Mahal order by userLists.NobetTarihi
	for json path) SELECT @result;