declare @jData varchar(max);
set @jData=(
select 
dbo.Fn_FulAd(TalepEdenPersonelId,'') as TalepEdenFulAdi,
dbo.Fn_FulAd(TalepEttigiPersonelId,'') as TalepEttigiFulAdi,
YeniTalepEdenKisininTarihi,
YeniTalepEttigiKisininTarihi,
case
when Durum ='0' then 'Beklemede' 
when Durum ='1' then 'Beklemede(Amir)' 
when Durum ='2' then 'Onaylanmış' 
when Durum ='3' then 'Reddedilmiş' 
end as Durum,
(select MahalAdi from Mahal where MahalId=NobetDegisimTalep.MahalId) as MahalAdi,
case
when DegisimTipi='2' then 'Karşılıklı Değişim' 
when DegisimTipi='1' then 'Tek Gün Değişim' 
end as DegisimTipi,
dbo.Fn_FulAd(AmirId,'') as OnaylayanAmirFulAdi
 from NobetDegisimTalep where TalepEdenPersonelId= @TalepEdenPersonelId order by Id desc
 for json path)
 select @jData jdata;