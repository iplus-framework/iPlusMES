
create function [dbo].[udf_IsTimeSpanActual]
(
    @startTime datetime,
    @endTime datetime
)
returns bit
AS
begin
    declare @isValid  bit;
	declare @currentTime datetime;
	declare @isAfterStartTime bit;
	declare @isBeforeEndDate bit

	set @isAfterStartTime = 1;
	set @isBeforeEndDate =1;

	set @currentTime = getdate();

	if @startTime is not null
	begin
		set @isAfterStartTime = cast((case when DATEDIFF(SECOND,@startTime, @currentTime)  >= 0 then 1 else 0 end) AS bit);
	end
	if @endTime is not null
	begin
		set @isBeforeEndDate = cast((case when DATEDIFF(SECOND,@endTime, getdate())  < 0 then 1 else 0 end) AS bit);
	end
  	set @isValid = case when  @isAfterStartTime = 1 and  @isBeforeEndDate = 1 then 1 else 0 end;
    return @isValid;
end
