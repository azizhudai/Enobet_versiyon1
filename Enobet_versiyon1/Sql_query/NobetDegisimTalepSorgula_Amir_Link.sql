select NDT.TalepEttigiPersonelId as TalepEttigiPersonelId,
(select Eposta from Kullanici K where K.KullaniciId=NDT.AmirId) as AmirEspota,
NDT.YeniTalepEttigiKisininTarihi as YeniTalepEttigiKisininTarihi,
NDT.YeniTalepEdenKisininTarihi as YeniTalepEdenKisininTarihi,
(select dbo.Fn_FulAd(NDT.TalepEdenPersonelId,'')) as KendiAdi,
(select dbo.Fn_FulAd(NDT.TalepEttigiPersonelId,'')) as BaskaAdi,
NDT.Mazeret as Mazeret
from NobetDegisimTalep NDT where @Link=NDT.Link
