DECLARE @RC int
DECLARE @programNo nvarchar(50)
DECLARE @prodOrderPartslistID uniqueidentifier

-- TODO: Set parameter values here.
set @programNo = 'P00412182';


EXECUTE @RC = [dbo].[udpRecalcActualQuantity] 
   @programNo
  ,@prodOrderPartslistID
GO