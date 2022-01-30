 --declare @KullaniciId int,@Month int,@MahalId int,@jData varchar(max),@jDataSubat varchar(max),@result varchar(max);

 /*set @Month=3;
 set @KullaniciId=20014;
 set @MahalId = 2;*/
 /*
select @jData=(NL.Data)
  from 
  NobetListeJson as NL where MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) in (3) and JSON_VALUE(NL.Data, '$[0].MahalId')=@MahalId;

  select @jDataSubat=(NL.Data)
  from 
  NobetListeJson as NL where MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) in (2) and JSON_VALUE(NL.Data, '$[0].MahalId')=@MahalId;
  */
  --'d MMMM dddd yyy'

  select 
  cast(FORMAT(cast(NLTableList.NobetTarihi as Date),'dddd','tr') as varchar) as GunAdi,
  dbo.Fn_FulAd(NLTableList.KullaniciId,'') fulAdi,
  count(NLTableList.KullaniciId) as Total,
  NLTableList.KullaniciId

  from
   NobetListeJson as NL 
  cross apply 
  OPENJSON(NL.Data)WITH (KullaniciId int,
       NobetTarihi nvarchar(max))
	   as NLTableList

	   where MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) in (1,2,3,4,5,6,7,8,9,10,11,12)
	   and YEAR(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) =@Year
	   and JSON_VALUE(NL.Data, '$[0].MahalId')=@MahalId
	   
	   group by 
	   cast(FORMAT(cast(NLTableList.NobetTarihi as Date),'dddd','tr') as varchar)
	   ,NLTableList.KullaniciId 
	   
	   
	   
	   --order by GunAdi desc