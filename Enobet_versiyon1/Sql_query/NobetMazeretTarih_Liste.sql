declare @jData varchar(max);
 SET LANGUAGE turkish;
set @jData = 
    (
    --select NMT.*,
    select 
    NMT.Id,
    NMT.KullaniciId,
	NMT.MahalId,
	FORMAT(cast(NMT.BaslangicTarihi as date),'d MMMM dddd yyyy') as BaslangicTarihi,
	FORMAT(cast(NMT.BitisTarihi as date),'d MMMM dddd yyyy') as BitisTarihi,
	NMT.MazeretAciklama,
	NMT.Durum,
    (select MahalAdi from Mahal where MahalId=NMT.MahalId) as MahalAdi,
    dbo.Fn_FulAd(NMT.KullaniciId,'') as KullaniciFulAdi
    from NobetMazeretTarih NMT Where KullaniciId=@KullaniciId and MahalId=@MahalId and Durum=@Durum order by Id desc for json path) select @jData jdata;