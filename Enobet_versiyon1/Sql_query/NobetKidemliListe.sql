select NobKid.NobetKidemliId,Kul.KullaniciId as KullaniciId, KLSinif.Adi+' '+ KLRutbe.Adi+' '+KullaniciAdi+' '+KullaniciSoyadi FullAdi, 
(select top 1 MahalAdi from Mahal where MahalId=NobKid.MahalId) as YetkiMahalAdi from NobetKidemli NobKid inner join Kullanici Kul on NobKid.KullaniciId=Kul.KullaniciId 
left join KategoriListe KLSinif on KLSinif.KategoriListeId = Kul.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = Kul.RutbeId and KLRutbe.Aktif = 1 
where Kul.Aktif=1 for json path