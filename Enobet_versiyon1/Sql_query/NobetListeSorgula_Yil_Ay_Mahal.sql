
 --SET LANGUAGE turkish;

select N.NobetListeJsonId As Id, N.Data from NobetListeJson N
  where
  
  cast(JSON_VALUE(N.Data,'$[0].NobetTarihi') as date) = cast(@Year as varchar)+'-'+cast(@Month as varchar)+'-1'
  --JSON_VALUE(N.Data, '$[0].NobetTarihi') =@Year+'-'+@Month+'-1' 
  
  and JSON_VALUE(N.Data, '$[0].MahalId') = @Mahal