declare @RowId int,@Msg varchar(100),@IsExist tinyint,@NobetKidemliEposta varchar(max),@DetaiMessage varchar(max),@KullaniciId int,@PerAdi varchar(max); 
        
set @RowId=0; set @IsExist=0

select top 1 @IsExist=count(KullaniciId) from Kullanici Where Eposta=@Email or Tcno=@Tcno 

if @IsExist!=0 
begin 
    set @Msg = 'Aynı T.C. Nolu veya E-Postalı Personel Zaten Sistemde Mevcut!'; 
    set @RowId=0;
end 
else 
begin 
    insert into Kullanici(SinifId,RutbeId,KullaniciAdi,KullaniciSoyadi,Eposta,Tcno,Parola,RolId,Aktif,NobettenCikar,MahalId,IslemYapanId,IP,IslemTarihi) values(@SinifId,
    @RutbeId,@userName,@userSurname,@Email,@Tcno,@password,0,@Aktif,@NobettenCikar,@MahalId,@IslemYapanId,@IP,@IslemTarihi) 
    
    SELECT @KullaniciId=SCOPE_IDENTITY(); --select @KullaniciId; 

    select top 1 @NobetKidemliEposta=Eposta from Kullanici where MahalId=2 and RolId=3; 
    select @PerAdi=dbo.Fn_FulAd(@KullaniciId,'') --from Kullanici where  KullaniciId=@KullaniciId;
    select top 1 @RowId = KullaniciId from Kullanici where Tcno = @Tcno;
    
    set @Msg = 'Ekleme Başarılı';
    set @DetaiMessage = 'Talep Eden Personel: '+@PerAdi;
end

select @RowId RowId, @Msg Mesaj,@DetaiMessage DetaiMessage,@NobetKidemliEposta NobetKidemliEposta