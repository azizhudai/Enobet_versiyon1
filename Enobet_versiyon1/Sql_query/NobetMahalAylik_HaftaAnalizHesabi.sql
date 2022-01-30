
 declare-- @KullaniciId int,@Month int,@MahalId int,
 @jData varchar(max),@result varchar(max);
 /*
 set @Month=2;
 set @KullaniciId=20014;
 set @MahalId = 2;*/

select @jData=(NL.Data)
  from 
  NobetListeJson as NL where MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) =@Month
   and YEAR(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) =@Year
  and JSON_VALUE(NL.Data, '$[0].MahalId')=@MahalId;

  select 
   cast(FORMAT(cast(NobetTarihi as Date),'dddd','tr') as varchar) as GunAdi,
  dbo.Fn_FulAd(KullaniciId,'') fulAdi,
  count(KullaniciId) as Total,
  KullaniciId
 -- (select MahalAdi from Mahal where MahalId=@MahalId) as MahalAdi,
  
 -- CAST(DATEPART(d, NobetTarihi) AS VARCHAR) as GunSayi,
 -- FORMAT(cast(NobetTarihi as date),'M') as GunAy
  from OPENJSON(@jData)WITH (KullaniciId int,
       NobetTarihi nvarchar(max)) as NLTableList
	   group by 
	   cast(FORMAT(cast(NobetTarihi as Date),'dddd','tr') as varchar),
	   KullaniciId order by count(KullaniciId) desc