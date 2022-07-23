DECLARE @name VARCHAR(50)
DECLARE db_cursor CURSOR FOR 
SELECT ProgramNo 
FROM ProdOrder

OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @name  

WHILE @@FETCH_STATUS = 0  
BEGIN  
      exec udpRecalcActualQuantity @name,null

      FETCH NEXT FROM db_cursor INTO @name 
END 

CLOSE db_cursor  
DEALLOCATE db_cursor 