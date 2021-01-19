CREATE view [dbo].[VBSystemColumns] as
select 
    ISNULL(t.name, 'idx1') as 'tablename',
	ISNULL(c.name, 'idx2') as 'columnname',
	NULLIF (type_name(c.xusertype), '') as 'columntype',
    NULLIF (c.length, -9999) as 'columnlength',
    NULLIF (c.isnullable, -9999) as 'columnnullable'
from SYSOBJECTS t inner join syscolumns c on t.id = c.id and t.type in ('u', 'v');