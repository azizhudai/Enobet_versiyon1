
SELECT 
      [MahalAdi]
      ,[EskiMahalAdi]
      ,[DahiliNo]
      ,[EskiDahiliNo]
      ,[Aktif]

      ,[YeniMahalAmirAdi]
      ,[EskiMahalAmirAdi]

      ,[IslemYapanPerAdi]
      ,[IP]
      ,[IslemTarihi]

      ,[Aciklama]
  FROM [Enobet_tsk].[dbo].[MahalLog] where IslemId=@IslemId 
