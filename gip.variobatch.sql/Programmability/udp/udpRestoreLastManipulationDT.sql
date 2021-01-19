CREATE PROCEDURE [dbo].[udpRestoreLastManipulationDT] @DifferenceDays int
AS
update ACClassPropertyRelation set LastManipulationDT = DATEADD(D, @DifferenceDays, LastManipulationDT);