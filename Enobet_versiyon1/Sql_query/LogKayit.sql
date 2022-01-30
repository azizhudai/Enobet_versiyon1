
declare @RowId int, @Msg varchar(50); set @RowId = 0; set @Msg = '';
insert into LogKayit(SayfaId,Aciklama,IslemYapanKullaniciId,IslemSaati,Ip,IslemId) Values(@SayfaId,@Aciklama,@IslemYapanKullaniciId,@IslemSaati,@Ip,@IslemId)
 set @Msg = 'Ekleme Başarılı';
 select top 1 @RowId = Id from LogKayit;
 select cast(@RowId as int)RowId, @Msg Mesaj