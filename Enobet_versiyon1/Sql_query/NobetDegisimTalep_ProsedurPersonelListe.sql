/*
select @NobetKidemliAdi=KLSinif.Adi+' '+KLRutbe.Adi+' '+NobetKidemli.KullaniciAdi+' '+NobetKidemli.KullaniciSoyadi,@NobetKidemliEposta=NobetKidemli.Eposta  from Kullanici NobetKidemli
  left join KategoriListe KLSinif on KLSinif.KategoriListeId = NobetKidemli.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = NobetKidemli.RutbeId and KLRutbe.Aktif = 1 
--left join Kullanici as KendiAmirId on KendiAmirId.KullaniciId=NobetKidemli.KullaniciId
where NobetKidemli.KullaniciId=@NobetKidemliId

select @KendiAmirAdi=KLSinif.Adi+' '+KLRutbe.Adi+' '+K.KullaniciAdi+' '+K.KullaniciSoyadi,@KendiAmirEposta=K.Eposta  from Kullanici K
  left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 
where K.KullaniciId=@KendiAmirId

select @TalepAmirAdi=KLSinif.Adi+' '+KLRutbe.Adi+' '+K.KullaniciAdi+' '+K.KullaniciSoyadi,@TalepAmirEposta=K.Eposta  from Kullanici K
  left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 
where K.KullaniciId=@TalepAmirId

select @OnayMakamAdi=KLSinif.Adi+' '+KLRutbe.Adi+' '+K.KullaniciAdi+' '+K.KullaniciSoyadi,@OnayMakamEposta=K.Eposta  from Kullanici K
  left join KategoriListe KLSinif on KLSinif.KategoriListeId = K.SinifId and KLSinif.Aktif=1
left join KategoriListe KLRutbe on KLRutbe.KategoriListeId = K.RutbeId and KLRutbe.Aktif = 1 
where K.KullaniciId=@OnayMakamId

select @NobetKidemliAdi as NobetKidemliAdi,@NobetKidemliEposta as NobetKidemliEposta,@KendiAmirAdi as KendiAmirAdi, @KendiAmirEposta as KendiAmirEposta,
@TalepAmirAdi as TalepAmirAdi, @TalepAmirEposta as TalepAmirEposta,@OnayMakamAdi as OnayMakamAdi,@OnayMakamEposta as OnayMakamEposta*/


select dbo.Fn_FulAd(NDT.TalepEdenPersonelId,'') as TalepEdenPerAdi,K.Eposta TalepEdenPerEposta,NobetKidemli.Eposta NobetKidemliPerEposta,KendiAmir.Eposta KendiAmirEposta,
		dbo.Fn_FulAd(TalepEttigiPer.KullaniciId,'') as TalepEttigiPerAdi,TalepEttigiPer.Eposta as TalepEttigiPerEposta,
		case when NDT.TalepEttigiPersonelOnayladiMi = 0 or NDT.TalepEttigiPersonelOnayladiMi is null then 'Beklemede' when  NDT.TalepEttigiPersonelOnayladiMi=1 then 'Onaylandı' when NDT.TalepEttigiPersonelOnayladiMi=2 then 'Reddedildi' else '' end as TalepEttigiPersonelOnayAdi,
		dbo.Fn_FulAd(NobetKidemli.KullaniciId,'') as NobetKidemliAdi,NobetKidemli.Eposta as NobetKidemliEposta,
		case when NDT.NobetKidemliOnayladiMi = 0 or  NDT.NobetKidemliOnayladiMi is null then 'Beklemede' when  NDT.NobetKidemliOnayladiMi=1 then 'Onaylandı' when NDT.NobetKidemliOnayladiMi=2 then 'Reddedildi' else '' end as NobetKidemliOnayAdi,
		dbo.Fn_FulAd(KendiAmir.KullaniciId,'') as KendiAmirAdi,NobetKidemli.Eposta as KendiAmirEposta,
		case  when NDT.KendiAmirOnayladiMi=0 or NDT.KendiAmirOnayladiMi is null then 'Beklemede' when  NDT.KendiAmirOnayladiMi=1 then 'Onaylandı' when NDT.KendiAmirOnayladiMi=2 then 'Reddedildi' else '' end as KendiAmirOnayAdi,
		dbo.Fn_FulAd(TalepAmir.KullaniciId,'') as TalepAmirAdi,TalepAmir.Eposta as TalepAmirEposta,
		case when NDT.TalepEttigiAmirOnayladiMi = 0 or NDT.TalepEttigiAmirOnayladiMi is null then 'Beklemede' when  NDT.TalepEttigiAmirOnayladiMi=1 then 'Onaylandı' when NDT.TalepEttigiAmirOnayladiMi=2 then 'Reddedildi' else '' end as TalepEttigiAmirOnayAdi,
		dbo.Fn_FulAd(OnayMakam.KullaniciId,'') as OnayMakamAdi,NobetKidemli.Eposta as OnayMakamEposta,
		case when NDT.OnayMakamiOnayladiMi = 0 or NDT.OnayMakamiOnayladiMi is null then 'Beklemede' when  NDT.OnayMakamiOnayladiMi=1 then 'Onaylandı' when NDT.OnayMakamiOnayladiMi=2 then 'Reddedildi' else '' end as OnayMakamiOnayAdi
		,NDT.EskiTalepEdenKisininTarihi, NDT.YeniTalepEdenKisininTarihi,NDT.Mazeret
from NobetDegisimTalep NDT

left join Kullanici K on K.KullaniciId=NDT.TalepEdenPersonelId
left join Kullanici TalepEttigiPer on TalepEttigiPer.KullaniciId=NDT.TalepEttigiPersonelId
left join Kullanici NobetKidemli on NobetKidemli.KullaniciId=NDT.NobetKidemliId
left join Kullanici KendiAmir on KendiAmir.KullaniciId=NDT.KendiAmirId
left join Kullanici TalepAmir on TalepAmir.KullaniciId=NDT.TalepEttigiAmirId
left join Kullanici OnayMakam on OnayMakam.KullaniciId=NDT.OnayMakamiId

 where NDT.Id =@NobetDegisimTalepId