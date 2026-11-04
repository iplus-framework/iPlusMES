IF COL_LENGTH('dbo.Material', 'MRPProcedureIndex') IS NULL
    ALTER TABLE dbo.Material ADD MRPProcedureIndex smallint NULL;
GO
ALTER TABLE dbo.Material ALTER COLUMN MRPProcedureIndex smallint NULL;