-- Parameters

declare	@quantity float;
declare	@fromMDUnitID uniqueidentifier;
declare	@toMDUnitID uniqueidentifier;
declare @baseMDUnit uniqueidentifier;
declare @materialID uniqueidentifier;

-- setup (test) params
set @quantity = 1;
set @fromMDUnitID = (select top 1 MDUnitID from MDUnit where  TechnicalSymbol = 'KOM');
set @toMDUnitID = (select top 1 MDUnitID from MDUnit where  TechnicalSymbol = 'KG');
set @baseMDUnit =  (select top 1 MDUnitID from MDUnit where  TechnicalSymbol = 'kg');
set @materialID = (select top 1 MaterialID from Material where materialno ='1');


-- calculation
declare @convertedQuantity float;
set @convertedQuantity = 0;

declare @calculationTable table
(
	ID int NOT NULL identity(1,1),
	Multiplier float,
	Divisor float,
	CalcQuantity float,
	FromMDUnitID uniqueidentifier,
	ToMDUnitID uniqueidentifier
);

if @fromMDUnitID = @toMDUnitID 
begin
	set @convertedQuantity = @quantity;
end
else
begin
	

	-- if both material unit is SI, not use MaterialUnit table
	if @materialID is not null
		begin
			-- MaterialUnit conversion @fromMDUnitID -> @toMDUnitID exist ?
			insert into @calculationTable(Multiplier, Divisor, CalcQuantity, FromMDUnitID, ToMDUnitID) 
				select
					top 1
					mu.Divisor as Multiplier,
					mu.Multiplier as Divisor,
					@quantity * mu.Divisor / mu.Multiplier as CalcQuantity,
					@fromMDUnitID as FromMDUnitID,
					@toMDUnitID as ToMDUnitID
			from MaterialUnit mu
			inner join Material mt on mt.MaterialID = mu.MaterialID
			where mt.MaterialID = @materialID and  mu.ToMDUnitID = @toMDUnitID and mt.BaseMDUnitID = @fromMDUnitID
			select * from @calculationTable;-- test

			if(@@ROWCOUNT = 0)
			begin
				-- MaterialUnit conversion @toMDUnitID -> @fromMDUnitID exist ?
				insert into @calculationTable(Multiplier, Divisor, CalcQuantity, FromMDUnitID, ToMDUnitID) 
					select
						top 1
						mu.Multiplier as Multiplier,
						mu.Divisor as Divisor,
						@quantity * mu.Multiplier / mu.Divisor as CalcQuantity,
						@fromMDUnitID as FromMDUnitID,
						@toMDUnitID as ToMDUnitID
				from MaterialUnit mu
				inner join Material mt on mt.MaterialID = mu.MaterialID
				where mt.MaterialID = @materialID and  mu.ToMDUnitID = @fromMDUnitID and mt.BaseMDUnitID = @toMDUnitID
				select * from @calculationTable;-- test
			end
		end
	
		-- using MDUnitConversion
		if((select count(*) from @calculationTable) = 0)
		begin

			insert into @calculationTable(Multiplier, Divisor, CalcQuantity, FromMDUnitID, ToMDUnitID) 
			select top 1
				Mc.Divisor as Multiplier,
				Mc.Multiplier as Divisor,
				@quantity * Mc.Divisor / Mc.Multiplier as CalcQuantity,
				@fromMDUnitID as FromMDUnitID,
				@toMDUnitID as ToMDUnitID
			from MDUnitConversion Mc
			where Mc.MDUnitID = @fromMDUnitID and Mc.ToMDUnitID = @toMDUnitID;
			select * from @calculationTable;-- test

			if(@@ROWCOUNT = 0)
			begin
				insert into @calculationTable(Multiplier, Divisor, CalcQuantity, FromMDUnitID, ToMDUnitID) 
				select top 1
					Mc.Multiplier as Multiplier,
					Mc.Divisor as Divisor,
					@quantity * Mc.Multiplier / Mc.Divisor as CalcQuantity,
					@fromMDUnitID as FromMDUnitID,
					@toMDUnitID as ToMDUnitID
				from MDUnitConversion Mc
				where Mc.MDUnitID = @toMDUnitID and Mc.ToMDUnitID = @fromMDUnitID;
				select * from @calculationTable;-- test
			end

			-- Try MDUnitConversion via extra step: @baseMDUnit
			if((select count(*) from @calculationTable) = 0)
			begin
				-- try find path @fromMDUnitID -> @baseMDUnit
				insert into @calculationTable(Multiplier, Divisor, CalcQuantity, FromMDUnitID, ToMDUnitID) 
				select top 1
					Mc.Multiplier as Multiplier,
					Mc.Divisor as Divisor,
					@quantity * Mc.Multiplier / Mc.Divisor as CalcQuantity,
					@fromMDUnitID as FromMDUnitID,
					@baseMDUnit as ToMDUnitID
				from MDUnitConversion Mc
				where Mc.MDUnitID = @fromMDUnitID and Mc.ToMDUnitID = @baseMDUnit;
				select * from @calculationTable;-- test

				if(@@ROWCOUNT = 0)
				begin
					insert into @calculationTable(Multiplier, Divisor, CalcQuantity, FromMDUnitID, ToMDUnitID) 
					select top 1
						Mc.Divisor as Multiplier,
						Mc.Multiplier as Divisor,
						@quantity * Mc.Divisor / Mc.Multiplier as CalcQuantity,
						@fromMDUnitID as FromMDUnitID,
						@baseMDUnit as ToMDUnitID
					from MDUnitConversion Mc
					where Mc.MDUnitID = @baseMDUnit and Mc.ToMDUnitID = @fromMDUnitID;
					select * from @calculationTable;-- test
				end

				if((select count(*) from @calculationTable) = 0)
				begin
					-- try find path @baseMDUnit -> @toMDUnitID
					insert into @calculationTable(Multiplier, Divisor, CalcQuantity, FromMDUnitID, ToMDUnitID) 
					select top 1
						Mc.Multiplier as Multiplier,
						Mc.Divisor as Divisor,
						clc.CalcQuantity * Mc.Multiplier / Mc.Divisor as CalcQuantity,
						@baseMDUnit as FromMDUnitID,
						@toMDUnitID as ToMDUnitID
					from MDUnitConversion Mc
					inner join @calculationTable clc on clc.ToMDUnitID = @baseMDUnit
					where Mc.MDUnitID = @baseMDUnit and Mc.ToMDUnitID = @toMDUnitID
					select * from @calculationTable;-- test

					if(@@ROWCOUNT = 0)
					begin
						insert into @calculationTable(Multiplier, Divisor, CalcQuantity, FromMDUnitID, ToMDUnitID) 
						select top 1
							Mc.Divisor as Multiplier,
							Mc.Multiplier as Divisor,
							clc.CalcQuantity * Mc.Divisor / Mc.Multiplier as CalcQuantity,
							@baseMDUnit as FromMDUnitID,
							@toMDUnitID as ToMDUnitID
						from MDUnitConversion Mc
						inner join @calculationTable clc on clc.ToMDUnitID = @baseMDUnit
						where Mc.MDUnitID = @toMDUnitID and Mc.ToMDUnitID = @baseMDUnit
						select * from @calculationTable;-- test
					end
				end
				
			end
		end
	select 'final test';	
	select * from @calculationTable;-- test
	set @convertedQuantity = (select top 1 CalcQuantity  from @calculationTable where ToMDUnitID = @toMDUnitID order by ID desc)
end

select isnull(@convertedQuantity, 0);