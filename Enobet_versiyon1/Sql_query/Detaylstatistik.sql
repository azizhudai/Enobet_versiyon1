/*declare @KullaniciId int,@Month varchar(100),@MahalId int,@jData varchar(max),@jDataSubat varchar(max),@result varchar(max),@Year varchar(4);

 set @Month='2';
 set @KullaniciId='0';
 set @MahalId = '2';
 set @Year = '2021';
 */
 declare @jData varchar(max);

 /*if @Month='0'
 begin
 set @Month='(1,2,3,4,5,6,7,8,9,10,11,12)';
 end*/
 /*
  if @KullaniciId='0'
 begin
 set @KullaniciId='(1,2,3,4,5,6,7,8,9,10,11,12)';
 end
 */
 set @jData=(
 select
(select MahalAdi from Mahal Where MahalId=NLTableList.MahalId) as MahalAdi,
  cast(FORMAT(cast(NLTableList.NobetTarihi as Date),'d MMMM yyyy dddd','tr') as varchar) as GunAdi,
  --cast(FORMAT(cast(NLTableList.NobetTarihi as Date),'yyyy','tr') as varchar) as Yil,
  dbo.Fn_FulAd(NLTableList.KullaniciId,'') fulAdi,
  --count(NLTableList.KullaniciId) as Total,
  NLTableList.KullaniciId

  from
   NobetListeJson as NL 
  cross apply 
  OPENJSON(NL.Data)WITH (
		KullaniciId int,
		NobetTarihi nvarchar(max),
		MahalId int
	   )
	   as NLTableList
	   
	   where 
	   --cast(MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) as varchar(2)) in(@Month)
	   --','+@Month+',' like '%,'+ cast(MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) as varchar(2)) +',%' and
	   cast(@Month as varchar(4)) like case when @Month = 0 then '%%' else cast(MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) as varchar(4)) end and

	    cast(@KullaniciId as varchar(50)) like case when @KullaniciId = 0 then '%%' else cast(NLTableList.KullaniciId as varchar(50)) end and

		cast(@Year as varchar(4)) like case when @Year = 0 then '%%' else cast(YEAR(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) as varchar(4)) end and

		cast(@MahalId as varchar(25)) like case when @MahalId = 0 then '%%' else cast(JSON_VALUE(NL.Data, '$[0].MahalId') as varchar(25)) end

		--YEAR(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) = @Year and
		--JSON_VALUE(NL.Data, '$[0].MahalId')=@MahalId
	   
	   order by cast (JSON_VALUE(NL.Data, '$[0].NobetTarihi') as date)
	   for json path)
	   select @jData;
	  /* group by 
	   cast(FORMAT(cast(NLTableList.NobetTarihi as Date),'dddd','tr') as varchar)
	   ,NLTableList.KullaniciId */