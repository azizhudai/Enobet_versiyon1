
declare @size int;--,@bastarih date,@bitistarih date;
/*
set @size=0;
set @bastarih='2021-02-03';
set @bitistarih ='2021-02-03';*/

--bitiş dışarda ama başlangç içinde
select @size=count(*) from NobetMazeretTarih where KullaniciId=@KullaniciId and MahalId=@MahalId and Durum=1 

and  cast(BaslangicTarihi as date) <= cast(@BaslangicTarihi as date) 
and cast(BitisTarihi as date)  <=cast(@BitisTarihi as date) 
and cast(BitisTarihi as date) >= cast(@BaslangicTarihi as date)

--
if @size=0
begin
--bitiş içirde başlangıç dışarda
select @size=count(*) from NobetMazeretTarih where KullaniciId=@KullaniciId and MahalId=@MahalId and Durum=1 

and  cast(BaslangicTarihi as date) >= cast(@BaslangicTarihi as date) 
and cast(BitisTarihi as date)  >=cast(@BitisTarihi as date) 
and cast(BaslangicTarihi as date) <= cast(@BitisTarihi as date)
end 

if @size=0
begin
--bitişte içirde başlangıçta içerde
select @size=count(*) from NobetMazeretTarih where KullaniciId=@KullaniciId and MahalId=@MahalId and Durum=1 

and  cast(BaslangicTarihi as date) <= cast(@BaslangicTarihi as date) 
and cast(BitisTarihi as date)  >=cast(@BitisTarihi as date) 
and cast(BaslangicTarihi as date) <= cast(@BitisTarihi as date)
end

select @size AS Sayi;