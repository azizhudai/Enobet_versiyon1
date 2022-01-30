/*BEGIN TRANSACTION [Tran1]

  BEGIN TRY*/

declare @IslemYapanPersonelId int,@SystemTime datetime,@Mesaj varchar(max),@TalepId int,@KullaniciId int,@TalepEdenPersonelId int,
@AmirMi tinyint,@DBDurum tinyint,@StateId int,@MahalId int,@DegisimMahalId int, @jData varchar(max),@NobetListeId int,@NLJsonId int,@JNobetListe varchar(max),@EskiTalepNobetId int,@YeniTalepNobetId int;

declare @TalepEttigiPersonelId int,@NobetKidemliId int,@TalepEdenAmirId int,@TalepEttigiAmirId int,@OnayMakamId int,
@TalepEttigiOnayMi tinyint, @NobetKidemliOnayMi tinyint,@TalepEdenAmirOnayMi tinyint,@TalepEttigiAmirOnayMi tinyint,@OnayMakamOnayMi tinyint,
@TalepEttigiOnayMiDB tinyint, @NobetKidemliOnayMiDB tinyint,@TalepEdenAmirOnayMiDB tinyint,@TalepEttigiAmirOnayMiDB tinyint,@OnayMakamOnayMiDB tinyint
,@SiradakiEpostaGonderilecekPer varchar(50),@SiradakiOnaylayacakPerId int,@DurumAdi varchar(50),@isReturn bit,@TalepEdenPerEposta varchar(50);

set @isReturn=0;
set @Systemtime = GETDATE();

--@Eposta varchar,@Link nvarchar(max);
--declare @Durum int,@Eposta varchar(max),@Link nvarchar(max);
If(OBJECT_ID('tempdb..#TempNL') Is Not Null)
Begin
    Drop Table #TempNL
End

create table #TempNL
(
Id int identity,
KullaniciId int,
MahalId int,
NobetTarihi nvarchar(max)
)

--set @Link='M1dETUM5UDAzQ0pOSUdGTw=='; set @Durum=1;set @Eposta='karatas.a10@dzkk.tsk';
set @NLJsonId=0;
set @Mesaj='';
set @AmirMi=0;
 --select @AmirMi=COUNT(MahalId) from Mahal where  @DegisimMahalId=2 MahalAmirId=(select KullaniciId from Kullanici where Eposta=@Eposta)

 select top 1 @IslemYapanPersonelId=KullaniciId  from Kullanici where Eposta=@Eposta;

select @TalepId=Id,@NobetListeId=NobetListeJsonId,@DegisimMahalId=MahalId,@DBDurum=Durum,
@TalepEdenPersonelId=TalepEdenPersonelId,@TalepEttigiPersonelId=TalepEttigiPersonelId,
@NobetKidemliId=NobetKidemliId,@TalepEdenAmirId=KendiAmirId,@TalepEttigiAmirId=TalepEttigiAmirId,@OnayMakamId=OnayMakamiId
,@TalepEttigiOnayMiDB =TalepEttigiPersonelOnayladiMi, @NobetKidemliOnayMiDB =NobetKidemliOnayladiMi
,@TalepEdenAmirOnayMiDB =KendiAmirOnayladiMi,@TalepEttigiAmirOnayMiDB =TalepEttigiAmirOnayladiMi,@OnayMakamOnayMiDB=OnayMakamiOnayladiMi
from NobetDegisimTalep where Link=@Link;

 select @TalepEdenPerEposta=Eposta from Kullanici where KullaniciId =@TalepEdenPersonelId;

if @DBDurum=1 --onay makamı tarafından
begin
set @Mesaj='Nöbet Değişim Talebi önceden onaylanmıştır.Tekrar işlem yapılamaz!';
set @StateId=2;
select @Mesaj Mesaj,@StateId StateId;
return;
end
if @DBDurum=2
begin
set @Mesaj='Nöbet Değişim Talebi önceden reddedilmiş. Tekrar başvuru yapılması gerekmektedir!';
set @StateId=3;
select @Mesaj Mesaj,@StateId StateId;
return;
end
/*
--select @NobetListeId
if @Durum=2
begin
	update NobetDegisimTalep set Durum=@Durum where Id=@TalepId;
	set @Mesaj='1';
end	
if @Durum=1 --amir mi gelmiş kontrol et
begin
update NobetDegisimTalep set Durum=@Durum where Id=@TalepId;
set @Mesaj='Nöbet Değişim Talebi Personel Tarafından Onaylanmıştır!';
end
if @Durum=0

begin*/


	--şuan işlem yapan kişi önceden işlem yapmış mı bak
	/*if @IslemYapanPersonelId=@TalepEttigiPersonelId
	begin 
		if @TalepEttigiOnayMiDB is not null  and  @TalepEttigiOnayMiDB!=0
		begin
		set @Mesaj='Tekrar İşlem Yapamazsınız!';
		--set @StateId=4;
		set @isReturn=1;
		--return;
		end
	end*/
	if @IslemYapanPersonelId=@NobetKidemliId
	begin 
		if @NobetKidemliOnayMiDB!=0 and @NobetKidemliOnayMiDB is not null
		begin
		set @Mesaj='Tekrar İşlem Yapamazsınız!';
		set @isReturn=1;
		--return;
		end
	end
	if @IslemYapanPersonelId=@TalepEdenAmirId
	begin 
		if @TalepEdenAmirOnayMiDB is not null   and  @TalepEdenAmirOnayMiDB!=0
		begin
		set @Mesaj='Tekrar İşlem Yapamazsınız!';
		set @isReturn=1;
		--return;
		end
	end
	if @IslemYapanPersonelId=@TalepEttigiAmirId
	begin 
		if @TalepEttigiAmirOnayMiDB is not null   and  @TalepEttigiAmirOnayMiDB!=0
		begin
		set @Mesaj='Tekrar İşlem Yapamazsınız!';
		set @isReturn=1;
		--return;
		end
	end
	if @IslemYapanPersonelId=@OnayMakamId
	begin 
		if @OnayMakamOnayMiDB is not null   and  @OnayMakamOnayMiDB!=0
		begin
		set @Mesaj='Tekrar İşlem Yapamazsınız!';
		set @isReturn=1;
		--return;
		end
	end

	if @isReturn=1
	begin
	set @StateId=4;
	select @Mesaj Mesaj, @StateId StateId;
	return;
	end
	--kontrolleri yap
	--ilk kontrol yani talep ettiği kişinin onayı

	-----------mail gönderilecek personeli belirleeee

	if @Durum=1
	set @DurumAdi='Onaylandı.';
	if @Durum=2
	begin
	set @DurumAdi='Reddedildi.';
	/*set @Mesaj = 'Nöbet Değişim Talep İşlemi Reddedilmiştir.';
	set @SiradakiOnaylayacakPerId=@TalepEdenPerEposta;
	update NobetDegisimTalep set Durum=@Durum,[IslemYapanPersonelId]=@IslemYapanPersonelId where Id=@TalepId ;
	select @Mesaj Mesaj,@SiradakiOnaylayacakPerId SiradakiOnaylayacakPerId,@TalepId TalepId;
	--update NobetDegisimTalep set Durum=@Durum where Id=@TalepId;
	return;*/
	end

	/*if @IslemYapanPersonelId = @TalepEttigiPersonelId-- and @TalepEttigiPersonelId = @NobetKidemliId
	begin
	set @TalepEttigiOnayMi=@Durum;
	set @Mesaj='Talep Ettiği Personel Tarafından '+@DurumAdi;
	--aynı zamanda nöbet kıdemlisi mi bak
		if @TalepEttigiPersonelId = @NobetKidemliId
		begin
		set @TalepEttigiOnayMi=@Durum;
		set @NobetKidemliOnayMi=@Durum;
		set @Mesaj='Nöbet Kıdemlisi Tarafından '+@DurumAdi;
		set @SiradakiOnaylayacakPerId=@TalepEdenAmirId;
		--talep eden personel nöbet kıdemlisi olduğunda

			/*if @Durum=1
			begin
			set @Mesaj='Nöbet Kıdemlisi Tarafından Onaylanmıştır.';
			end
			else if @Durum=2
			begin
			set @Mesaj='Nöbet Kıdemlisi Tarafından Reddedilmiştir!';
			end*/
		end*/
			 if @TalepEdenPersonelId=@NobetKidemliId
			begin
			--set @TalepEdenAmirOnayMi=@Durum;
			set @NobetKidemliOnayMi=1;
			set @IslemYapanPersonelId=@TalepEdenAmirId;
			end
		--aynı zamanda talep eden personelin amiri mi
		/*else if @TalepEttigiPersonelId = @TalepEdenAmirId
		begin
		set @TalepEdenAmirId=@Durum;
		end*/
	--end
	else if @IslemYapanPersonelId=@NobetKidemliId
	begin
	set @NobetKidemliOnayMi=@Durum;
	set @SiradakiOnaylayacakPerId=@TalepEdenAmirId;
	set @Mesaj='Nöbet Kıdemlisi Tarafından '+@DurumAdi+'Talep Eden Personnele E-Posta Gönderilecektir.';
	end
	if @IslemYapanPersonelId = @TalepEdenAmirId
	begin
		if @IslemYapanPersonelId=@TalepEttigiAmirId
		begin
		set @TalepEdenAmirOnayMi=@Durum;
		set @TalepEttigiAmirOnayMi=@Durum;
		set @Mesaj='Nöbetçi Amir Tarafından '+@DurumAdi;
		--talep eden ve edilen amir aynı ve onay makamıda aynı ise 
			if @IslemYapanPersonelId=@OnayMakamId
			begin
			set @OnayMakamOnayMi=@Durum;
			set @Mesaj='Onay Makamı Tarafından '+@DurumAdi;
			end
			else --sadece onay makamı kalmış
			begin
			set @SiradakiOnaylayacakPerId=@OnayMakamId;
			end
		end
		else
		begin
		set @TalepEdenAmirOnayMi=@Durum;
		set @SiradakiOnaylayacakPerId=@TalepEttigiAmirId;
		end
		

	end
	--talep eden amiri değil talep ettiği amir ise
	else if @IslemYapanPersonelId=@TalepEttigiAmirId
	begin
	set @TalepEttigiAmirOnayMi=@Durum;
		set @Mesaj='Talep Ettiği Personelin Nöbetçi Amiri Tarafından '+@DurumAdi;
		if @IslemYapanPersonelId=@OnayMakamId
			begin
			set @OnayMakamOnayMi=@Durum;
			set @Mesaj='Onay Makamı Tarafından '+@DurumAdi;
			end
			else --sadece onay makamı kalmış
			begin
			set @SiradakiOnaylayacakPerId=@OnayMakamId;
			end
	end

	--if @TalepEdenPersonelId=

	if @IslemYapanPersonelId!=@OnayMakamId
	begin
	set @StateId=5;
	select @SiradakiEpostaGonderilecekPer=Eposta from Kullanici where KullaniciId=@SiradakiOnaylayacakPerId;

	update NobetDegisimTalep set 
	--Durum=@Durum,
	[NobetKidemliOnayladiMi] = @NobetKidemliOnayMi
      ,[KendiAmirOnayladiMi] = @TalepEdenAmirOnayMi
      ,[TalepEttigiAmirOnayladiMi] = @TalepEttigiAmirOnayMi
      ,[OnayMakamiOnayladiMi] = @OnayMakamOnayMi
      ,[TalepEttigiPersonelOnayladiMi] = @TalepEttigiOnayMi
	,[IslemYapanPersonelId]=@IslemYapanPersonelId
	
	where Id=@TalepId;
	select @Mesaj Mesaj,@SiradakiEpostaGonderilecekPer SiradakiEpostaGonderilecekPer,@StateId StateId,@TalepId TalepId; 
	end

	if @IslemYapanPersonelId=@OnayMakamId
	begin
	set @OnayMakamOnayMi=@Durum;
		if @Durum=1
		-- nöbetleri değiştir
			begin
		--kullanıcının bilgilerinin güncellenmesi
		set @StateId=6;
		select @jData=(NL.Data),@NLJsonId=NL.NobetListeJsonId from NobetListeJson NL where NL.NobetListeJsonId=@NobetListeId;

insert into #TempNL
select NLTableList.KullaniciId,NLTableList.MahalId,NLTableList.NobetTarihi from OPENJSON(@jData)WITH (KullaniciId int,MahalId int,
       NobetTarihi nvarchar(max)) as NLTableList;
	   
	   --select * from #TempNL;
	   --select @EskiTalepNobetId=Id,@DegisimMahalId=MahalId from #TempNL T1 where KullaniciId=(select TalepEdenPersonelId from NobetDegisimTalep T2 where T2.Link=@Link) and NobetTarihi=(select EskiTalepEttigiKisininTarihi/*EskiTalepEdenKisininTarihi */ from NobetDegisimTalep T2 where T2.Link=@Link)

	   select @YeniTalepNobetId=Id from #TempNL T1 where KullaniciId=(select TalepEttigiPersonelId from NobetDegisimTalep T2 where T2.Link=@Link) and NobetTarihi=(select YeniTalepEdenKisininTarihi /*EskiTalepEttigiKisininTarihi*/ from NobetDegisimTalep T2 where T2.Link=@Link)

	   update #TempNL set KullaniciId=(select TalepEdenPersonelId from NobetDegisimTalep T2 where T2.Link=@Link) where Id=@YeniTalepNobetId;
	   --update #TempNL set KullaniciId=(select TalepEttigiPersonelId from NobetDegisimTalep T2 where T2.Link=@Link) where Id=@EskiTalepNobetId;

	   ALTER TABLE #TempNL
		DROP COLUMN Id;

		 select * from #TempNL;
		 --en sonda bu olacak
	   ----update NobetListeJson set Data=(select * from #TempNL for json path) where @NobetListeId=NobetListeJsonId --gerçek tablo güncellenir
	   -- log kayıtları tutulacak
	   
	   --select @Mesaj=count(*) from #TempNL;
	  -- select @Mesaj;
	   If(OBJECT_ID('tempdb..#TempNL') Is Not Null)
			Begin
				Drop Table #TempNL
			End


		--talep bilgisinin onaylandığı bilgisi
		--update NobetDegisimTalep set Durum=@Durum where Id=@TalepId;

	

		-- select @Mesaj,@Durum,@TalepId,@NobetListeId ;
		set @Mesaj='Nöbet Değişim Talebi Onay Makamı Tarafından Onaylanmıştır!';
		set @StateId=1;
		select @Mesaj Mesaj, @StateId StateId;

		end
		update NobetDegisimTalep set 
	Durum=@Durum,
	[NobetKidemliOnayladiMi] = @NobetKidemliOnayMi
      ,[KendiAmirOnayladiMi] = @TalepEdenAmirOnayMi
      ,[TalepEttigiAmirOnayladiMi] = @TalepEttigiAmirOnayMi
      ,[OnayMakamiOnayladiMi] = @OnayMakamOnayMi
      ,[TalepEttigiPersonelOnayladiMi] = @TalepEttigiOnayMi
	,[IslemYapanPersonelId]=@IslemYapanPersonelId
	
	where Id=@TalepId;
	end
	
	/*--talep eden kişi nöbet kıdemlisi ise direk olarak amirine geç
	--nöbet kıdemlisi işlemi yapan kişini
	if @IslemYapanPersonelId = @NobetKidemliId
	*/



	/* select @AmirMi=COUNT(MahalId) from Mahal where MahalId=@DegisimMahalId and MahalAmirId=(select KullaniciId from Kullanici where Eposta=@Eposta)

	if @AmirMi<1
	set @Mesaj='Onaylayacak Personelin Nöbetçi Amir Olması Gerekmektedir!';
	else --nöbetçi amirdir
		begin
		--kullanıcının bilgilerinin güncellenmesi
		select @jData=(NL.Data),@NLJsonId=NL.NobetListeJsonId from NobetListeJson NL where NL.NobetListeJsonId=@NobetListeId;

insert into #TempNL
select NLTableList.KullaniciId,NLTableList.MahalId,NLTableList.NobetTarihi from OPENJSON(@jData)WITH (KullaniciId int,MahalId int,
       NobetTarihi nvarchar(max)) as NLTableList;
	   
	   --select * from #TempNL;
	   select @EskiTalepNobetId=Id,@DegisimMahalId=MahalId from #TempNL T1 where KullaniciId=(select TalepEdenPersonelId from NobetDegisimTalep T2 where T2.Link=@Link) and NobetTarihi=(select EskiTalepEttigiKisininTarihi from NobetDegisimTalep T2 where T2.Link=@Link)

	   select @YeniTalepNobetId=Id from #TempNL T1 where KullaniciId=(select TalepEttigiPersonelId from NobetDegisimTalep T2 where T2.Link=@Link) and NobetTarihi=(select EskiTalepEdenKisininTarihi from NobetDegisimTalep T2 where T2.Link=@Link)

	   update #TempNL set KullaniciId=(select TalepEdenPersonelId from NobetDegisimTalep T2 where T2.Link=@Link) where Id=@YeniTalepNobetId;
	   update #TempNL set KullaniciId=(select TalepEttigiPersonelId from NobetDegisimTalep T2 where T2.Link=@Link) where Id=@EskiTalepNobetId;

	   ALTER TABLE #TempNL
		DROP COLUMN Id;

		-- select * from #TempNL;
		 --en sonda bu olacak
	   update NobetListeJson set Data=(select * from #TempNL for json path) where @NobetListeId=NobetListeJsonId --gerçek tablo güncellenir
	   -- log kayıtları tutulacak
	   --select @Mesaj=count(*) from #TempNL;
	  -- select @Mesaj;
	   If(OBJECT_ID('tempdb..#TempNL') Is Not Null)
			Begin
				Drop Table #TempNL
			End


		--talep bilgisinin onaylandığı bilgisi
		update NobetDegisimTalep set Durum=@Durum where Id=@TalepId;
		-- select @Mesaj,@Durum,@TalepId,@NobetListeId ;
		set @Mesaj='Nöbet Değişim Talebi Amir Tarafından Onaylanmıştır!';
		
		end*/
--end


--set @Mesaj='Daha Önce İşlem Yapılmıştır!';
/*if @Durum=3
begin
update NobetDegisimTalep set Durum=@Durum where Id=@TalepId;
set @Mesaj='Nöbet Değişim Talebi Reddedilmiştir!';
end*/


		--talep detay bilgisi ekleme
		INSERT INTO TalepDetay
           (NobetDegisimTalepId
           ,IslemYapanPersonelId
           ,IslemSaati
		   ,Aciklama
           ,Durum)
     VALUES
           (@TalepId
           ,@IslemYapanPersonelId
           ,@Systemtime
		   ,@Mesaj
           ,@Durum)


--select @Mesaj as Mesaj;
/*
COMMIT TRANSACTION [Tran1]

  END TRY

  BEGIN CATCH
  set @Mesaj='0';
  SELECT
    ERROR_NUMBER() AS ErrorNumber,
    ERROR_STATE() AS ErrorState,
    ERROR_SEVERITY() AS ErrorSeverity,
    ERROR_PROCEDURE() AS ErrorProcedure,
    ERROR_LINE() AS ErrorLine,
    ERROR_MESSAGE() AS ErrorMessage,
	@Mesaj Mesaj;
      ROLLBACK TRANSACTION [Tran1]
	--  select @Mesaj;
  END CATCH  
  */