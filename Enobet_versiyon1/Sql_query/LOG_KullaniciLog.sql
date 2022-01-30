
SELECT KL.[KullaniciId]
     -- ,KL.[KullaniciAdi]
     -- ,KL.[KullaniciSoyadi]
      ,KL.[Tcno]
      ,KL.[Eposta]
      ,KL.[BirlikId]
      ,(select MahalAdi from Mahal where KL.MahalId = MahalId) [MahalAdi]
      ,[Parola]
      , case RolId when 0 then 'Kullanıcı' when 1 then 'Yönetici' when 3 then 'Nöbet Kıdemlisi' else '' end as RolAdi
      ,[Aktif]
      ,(select top 1 Adi from KategoriListe where KL.SinifId=KategoriListeId ) as SinifAdi
      ,(select top 1 Adi from KategoriListe where KL.RutbeId=KategoriListeId ) as RutbeAdi-- [RutbeId]
      ,[NobettenCikar]

      ,[IslemYapanId]
      ,[IP]
      ,[IslemId]
      ,IslemYapanPerAdi
      ,[Aciklama]
      ,[IslemTarihi]
      ,[KullaniciAdi]
      ,[KullaniciSoyadi]
      ,case when EskiKullaniciAdi is null or EskiKullaniciAdi='' then '-' else EskiKullaniciAdi  end as EskiKullaniciAdi
      ,case when [EskiKullaniciSoyadi] is null or EskiKullaniciSoyadi='' then '-'  else EskiKullaniciSoyadi end as EskiKullaniciSoyadi
      ,case when [EskiTcno] is null or EskiTcno='' then '-'  else [EskiTcno] end as EskiTcno
      ,case when EskiKullaniciAdi is null or EskiEposta='' then '-'  else [EskiEposta] end as EskiEposta
      ,case when [EskiBirlikAdi] is null or EskiBirlikAdi='' then '-'  else [EskiBirlikAdi] end as EskiBirlikAdi
      ,case when [YeniBirlikAdi] is null or YeniBirlikAdi='' then '-'  else [YeniBirlikAdi] end as YeniBirlikAdi
      ,case when [EskiMahalAdi] is null or EskiMahalAdi='' then '-' else [EskiMahalAdi] end as EskiMahalAdi
      ,case when [YeniMahalAdi] is null or YeniMahalAdi='' then '-' else [YeniMahalAdi] end as YeniMahalAdi
      ,case when [EskiAktif] is null then '-' else [EskiAktif] end as EskiAktif
      ,case when [YeniAktif] is null then '-'  else [YeniAktif] end as YeniAktif
      ,case when [EskiSinifAdi] is null or [EskiSinifAdi]='' then '-' else [EskiSinifAdi] end as EskiSinifAdi
      ,case when [YeniSinifAdi] is null then '-' else [YeniSinifAdi] end as YeniSinifAdi
      ,case when [EskiRutbeAdi] is null then '-' else [EskiRutbeAdi] end as EskiRutbeAdi
      ,case when [YeniRutbeAdi] is null then '-' else [YeniRutbeAdi] end as YeniRutbeAdi
      ,case when [EskiNobettenCikar] is null then '-' else [EskiNobettenCikar] end as EskiNobettenCikar
      ,case when [YeniNobettenCikar] is null then '-' else [YeniNobettenCikar] end as YeniNobettenCikar
      ,case when [YeniKullaniciAdi] is null then '-' else [YeniKullaniciAdi] end as YeniKullaniciAdi
      ,case when [YeniKullaniciSoyadi] is null then '-' else [YeniKullaniciSoyadi] end as YeniKullaniciSoyadi
      ,case when [YeniTcno] is null then '-' else [YeniTcno]  end as YeniTcno
      ,case when [YeniEposta] is null then '-' else [YeniEposta] end as YeniEposta
      ,case when [EskiRolAdi] is null then '-' else [EskiRolAdi] end as EskiRolAdi
      ,case when [YeniRolAdi] is null then '-' else [YeniRolAdi] end as YeniRolAdi
  FROM [Enobet_tsk].[dbo].[KullaniciLog] as KL where IslemId=@IslemId 