
declare @IslemYapanPersonelId int,@TalepId int,@SystemTime datetime;

select @IslemYapanPersonelId=KullaniciId from Kullanici where Eposta=@Eposta;

select @TalepId=Id from NobetDegisimTalep where Link=@Link;

set @Systemtime = GETDATE();
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
		   ,@Aciklama
           ,5)