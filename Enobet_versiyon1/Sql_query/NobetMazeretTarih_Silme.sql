
/****** Script for SelectTopNRows command from SSMS  ******/
/*SELECT TOP (1000) [NobetListeJsonId]
      ,[Data]
  FROM [Enobet_tsk].[dbo].[NobetListeJson]
  */
  
  declare @Sayi int,@Mesaj varchar(max),@Year varchar(4), @Month varchar(2),@MahalId int,@BaslangicTarihi date,@BitisTarihi date;
  /*
  set @Year='2021';
  set @Month='03';
  --set @MahalId=2;
  set @BaslangicTarihi='2020-03-21';
  set @BitisTarihi='2021-04-07';*/
  set @Sayi=0;
  set @Mesaj='';

  select @BaslangicTarihi=BaslangicTarihi,@BitisTarihi=BitisTarihi,@MahalId=MahalId from NobetMazeretTarih where Id=@Id

  select @Sayi=count(*)
  from 
  NobetListeJson as NL where (MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi'))>= MONTH(@BaslangicTarihi) and (MONTH(JSON_VALUE(NL.Data, '$[0].NobetTarihi'))<= MONTH(@BitisTarihi)))
  and (YEAR(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) =YEAR(@BaslangicTarihi)
  and YEAR(JSON_VALUE(NL.Data, '$[0].NobetTarihi')) =YEAR(@BitisTarihi))
  and JSON_VALUE(NL.Data, '$[0].MahalId') = @MahalId
  --select @Sayi as Sayi

  --JSON_VALUE(NL.Data, '$[0].NobetTarihi') like @Year+'-'+@Month+'-%' and





  if @Sayi=0
    begin
        update NobetMazeretTarih set Durum=0 where Id=@Id
        set @Mesaj ='Silme Başarılı.';
    end
    else
    begin
    set @Mesaj ='Nöbet Listesi Yayında Olduğundan Dolayı Silemezsiniz!';
    end
    select @Mesaj as Mesaj ;