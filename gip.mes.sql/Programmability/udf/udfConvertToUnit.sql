IF EXISTS (SELECT * FROM sysobjects WHERE type = 'FN' AND name = 'udfConvertToUnit')
	BEGIN
		DROP  function  dbo.[udfConvertToUnit]
	END
GO
CREATE FUNCTION [dbo].[udfConvertToUnit]
(
	@quantity float,
	@fromMDUnitID uniqueidentifier,
	@toMDUnitID uniqueidentifier,
	@baseMDUnit uniqueidentifier
)
RETURNS float
AS
BEGIN
	declare @convertedQuantity float;
	set @convertedQuantity = 0;
	if @quantity is not null and @quantity > 0 and @fromMDUnitID is not null and @toMDUnitID is not null
	BEGIN
		IF @fromMDUnitID = @toMDUnitID
		BEGIN
			SET @convertedQuantity = @quantity;
		END
		IF @fromMDUnitID <> @toMDUnitID
		BEGIN
			declare @calculationTable table
			(
				ID int NOT NULL identity(1,1),
				Multiplier float,
				Divisor float,
				CalcQuantity float
			);
			insert into @calculationTable(Multiplier,Divisor)
			select top 1
				Mc.Multiplier as Multiplier,
				Mc.Divisor as Divisor
			from MDUnitConversion Mc
			where Mc.MDUnitID = @fromMDUnitID and Mc.ToMDUnitID = @toMDUnitID;
			insert into @calculationTable(Multiplier,Divisor)
			select top 1
				Mc.Divisor as Multiplier,
				Mc.Multiplier as Divisor
			from MDUnitConversion Mc
			where Mc.MDUnitID = @toMDUnitID and Mc.ToMDUnitID = @fromMDUnitID;
			if (select COUNT(ID) from @calculationTable) = 0
			begin
				insert into @calculationTable(Multiplier, Divisor)
				select top 1
					Mc.Multiplier as Multiplier,
					Mc.Divisor as Divisor
				from MDUnitConversion Mc
				where Mc.MDUnitID = @fromMDUnitID and Mc.ToMDUnitID = @baseMDUnit;
				insert into @calculationTable(Multiplier, Divisor)
				select top 1
					Mc.Divisor as Multiplier,
					Mc.Multiplier as Divisor
				from MDUnitConversion Mc
				where Mc.MDUnitID = @baseMDUnit and Mc.ToMDUnitID = @fromMDUnitID;
				insert into @calculationTable(Multiplier, Divisor)
				select top 1
					Mc.Multiplier as Multiplier,
					Mc.Divisor as Divisor
				from MDUnitConversion Mc
				where Mc.MDUnitID = @baseMDUnit and Mc.ToMDUnitID = @toMDUnitID;
				insert into @calculationTable(Multiplier, Divisor)
				select top 1
					Mc.Divisor as Multiplier,
					Mc.Multiplier as Divisor
				from MDUnitConversion Mc
				where Mc.MDUnitID = @toMDUnitID and Mc.ToMDUnitID = @baseMDUnit;
			end
			-- update first step
			update @calculationTable
			set CalcQuantity = @quantity * Divisor / Multiplier
			where ID = 1;
			-- update multiple steps
			UPDATE t1
			SET t1.CalcQuantity = t2.CalcQuantity * t1.Divisor / t1.Multiplier
			FROM @calculationTable AS t1
			INNER JOIN @calculationTable AS t2
			ON t1.ID = t2.ID + 1
			WHERE t1.ID > 1
			set @convertedQuantity = (select top 1 CalcQuantity from @calculationTable order by ID desc);
		END
	END
	RETURN isnull(@convertedQuantity, 0);
END
