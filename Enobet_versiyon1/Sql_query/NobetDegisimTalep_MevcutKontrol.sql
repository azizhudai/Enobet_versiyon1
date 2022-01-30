

select top 1 dbo.Fn_FulAd(TalepEdenPersonelId,'') as IslemYapanPerAdi,IslemTarihi  from NobetDegisimTalep 
where (YeniTalepEdenKisininTarihi=@Tarih or YeniTalepEttigiKisininTarihi=@Tarih) and Durum not in(1,3) and MahalId=@MahalId
