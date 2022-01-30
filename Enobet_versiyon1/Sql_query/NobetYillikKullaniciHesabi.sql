 declare @jData varchar(max);Set @jData = '';
	select @jData= (SELECT  userLists.KullaniciId, count(userLists.KullaniciId) as TotalCount
FROM NobetListeJson NL
CROSS APPLY OPENJSON (NL.Data, N'$')
  WITH (
    KullaniciId int)
      AS userLists where  JSON_VALUE(NL.Data, '$[0].MahalId') =@MahalId and JSON_VALUE(NL.Data, '$[0].NobetTarihi') like @Year+'%' 
	  group by userLists.KullaniciId for json path) select JData.value As UserTotalYear, KLSinif.Adi+' '+ KLRutbe.Adi + ' ' + K.KullaniciAdi + ' ' + K.KullaniciSoyadi As FullName from openjson(@jData) as JData
	  inner join Kullanici K On K.KullaniciId=Json_value(JData.value,'$.KullaniciId')
	  left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 
	  --where K.Aktif=1 
	  where K.NobettenCikar=0
	