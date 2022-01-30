DECLARE @id int;
set @id=0;

INSERT INTO NobetMazeretTarih
           (KullaniciId
           ,MahalId
           ,BaslangicTarihi
           ,BitisTarihi
           ,MazeretAciklama
           ,MazeretBildiriTarihi
           ,Durum)
     VALUES
           (@KullaniciId
           ,@MahalId
           ,@BaslangicTarihi
           ,@BitisTarihi
           ,@MazeretAciklama
           ,GETDATE()
           ,@Durum)
            SELECT @id=SCOPE_IDENTITY(); select @id;
