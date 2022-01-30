
declare @jData varchar(max);

 SET LANGUAGE turkish;

set @jData = 
  (select *,DATEDIFF(DAY,BaslangicTarihi,BitisTarihi)+1 as GunFarki 
  from NobetMazeretTarih
  where (Month(BaslangicTarihi) <=@Ay and Month(BitisTarihi) >=@Ay) and Year(BaslangicTarihi) =@Year and Year(BitisTarihi) =@Year
  and Durum=1 and KullaniciId in(select KullaniciId from Mahal where MahalId=@MahalId)
   for json path) select @jData jdata;