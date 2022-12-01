using ClosedXML.Excel;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.processapplication
{
    public static class LabOrderToExcel
    {

        #region Create Excel Table
        public static void DoLabOrderToExcel(Database database, string fileName, LabOrder[] labOrders)
        {
            XLWorkbook workBook = new XLWorkbook();
            List<string> sheetNames = new List<string>();
            int indexOfMaterial = 0;

            labOrders = labOrders.Where(c => c.ProdOrderPartslistPos != null).ToArray();

            if (labOrders != null && labOrders.Count() > 1)
            {
                DoSummaryWorksheet(database, workBook, labOrders);
            }

            foreach (LabOrder labOrder in labOrders)
            {
                string sheetName = labOrder.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1;
                sheetName = TruncateAtWord(sheetName, 20);
                sheetNames.Add(sheetName);
                indexOfMaterial = sheetNames.Where(s => s != null && s.Equals(sheetName)).Count();

                IXLWorksheet workSheet = workBook.Worksheets.Add(sheetName + " - " + indexOfMaterial);
                DoLabOrderToExcel(database, workSheet, labOrder);
                workSheet.Columns().AdjustToContents();
            }

            workBook.SaveAs(fileName);
        }

        #endregion

        #region Excel Cell Printing

        public static void DoSummaryWorksheet(Database database, XLWorkbook workBook, LabOrder[] labOrders)
        {
            IXLWorksheet worksheet = workBook.Worksheets.Add("Summary");
            worksheet.Cell(1, 1).Value = "Laboratory Order No";
            worksheet.Cell(1, 2).Value = "Material No.";
            worksheet.Cell(1, 3).Value = "Material Name";
            worksheet.Cell(1, 4).Value = "Production Order";
            worksheet.Cell(1, 5).Value = "Update Dates";
            worksheet.Cell(1, 6).Value = "Proizvodna linija";
            worksheet.Cell(1, 7).Value = "Reference Value";
            worksheet.Cell(1, 8).Value = "Lowest value";
            worksheet.Cell(1, 9).Value = "Maximum value";
            worksheet.Cell(1, 10).Value = "Average Value";
            worksheet.Cell(1, 11).Value = "Broj vaganja";
            worksheet.Cell(1, 12).Value = "Broj vaganja u toleranciji";
            worksheet.Cell(1, 13).Value = "Broj vaganja izvan tolerancije";

            int row = 2;
            foreach (LabOrder labOrder in labOrders)
            {
                DoSummaryWorksheetRow(database, row, worksheet, labOrder);
                row++;
            }

            var rngTable = worksheet.Range(1, 1, row, 13);
            rngTable.CreateTable("summary");
        }

        private static void DoSummaryWorksheetRow(Database database, int row, IXLWorksheet worksheet, LabOrder labOrder)
        {
            worksheet.Cell(row, 1).Value = labOrder.LabOrderNo;
            worksheet.Cell(row, 2).Value = labOrder.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialNo;
            worksheet.Cell(row, 3).Value = labOrder.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1;
            worksheet.Cell(row, 4).Value = labOrder.ProdOrderPartslistPos?.ProdOrderPartslist.ProdOrder.ProgramNo;
            worksheet.Cell(row, 5).Value = labOrder.SampleTakingDate;

            core.datamodel.ACClass machine = GetLabOrderMachine(database, labOrder);
            worksheet.Cell(row, 6).Value = machine?.ACCaption;

            int tolCounter = 0;
            int counter = 0;

            foreach (LabOrderPos labOrderPosItem in labOrder.LabOrderPos_LabOrder.ToArray())
            {
                double refValue = (double)labOrderPosItem.ReferenceValue;
                worksheet.Cell(row, 7).Value = labOrderPosItem.ReferenceValue;
                worksheet.Cell(row, 8).Value = labOrderPosItem.ValueMin;
                worksheet.Cell(row, 9).Value = labOrderPosItem.ValueMax;
                worksheet.Cell(row, 10).Value = labOrderPosItem.ActualValue;

                if (labOrderPosItem.XMLConfig != null)
                {
                    SamplePiStats piStats = null;
                    try
                    {
                        piStats = labOrderPosItem[PWSamplePiLightBox.C_LabOrderExtFieldStats] as SamplePiStats;
                    }
                    catch { }

                    if (piStats != null)
                    {
                        foreach (SamplePiValue item in piStats.Values)
                        {
                            if (item.TolState == 1 || item.TolState == -1)
                                tolCounter++;
                            counter++;
                        }
                    }
                }
            }

            worksheet.Cell(row, 11).Value = counter;
            worksheet.Cell(row, 12).Value = counter - tolCounter;
            worksheet.Cell(row, 13).Value = tolCounter;
        }

        public static void DoLabOrderToExcel(Database database, IXLWorksheet worksheet, LabOrder labOrder)
        {

            worksheet.Cell("A1").Value = "Laboratory Order No";
            worksheet.Cell("B1").Value = labOrder.LabOrderNo;
            worksheet.Cell("A2").Value = "Material No.";
            worksheet.Cell("B2").Value = labOrder.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialNo;
            worksheet.Cell("A3").Value = "Material Name";
            worksheet.Cell("B3").Value = labOrder.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1;
            worksheet.Cell("A4").Value = "Production Order";
            worksheet.Cell("B4").Value = labOrder.ProdOrderPartslistPos?.ProdOrderPartslist.ProdOrder.ProgramNo;
            worksheet.Cell("A5").Value = "Update Dates";
            worksheet.Cell("B5").Value = labOrder.SampleTakingDate;
            worksheet.Cell("A6").Value = "Proizvodna linija";

            core.datamodel.ACClass machine = GetLabOrderMachine(database, labOrder);
            worksheet.Cell("B6").Value = machine?.ACCaption;

            worksheet.Cell("A7").Value = "Reference Value";
            worksheet.Cell("A9").Value = "Lowest value";
            worksheet.Cell("A10").Value = "Maximum value";
            worksheet.Cell("A11").Value = "Average Value";

            worksheet.Cell("A13").Value = "Broj vaganja";
            worksheet.Cell("A14").Value = "Broj vaganja u toleranciji";
            worksheet.Cell("A15").Value = "Broj vaganja izvan tolerancije";

            int tolCounter = 0;
            int counter = 0;

            foreach (LabOrderPos labOrderPosItem in labOrder.LabOrderPos_LabOrder.ToArray())
            {
                double refValue = (double)labOrderPosItem.ReferenceValue;
                int rowPiStats = 18;

                worksheet.Cell("B7").Value = labOrderPosItem.ReferenceValue;
                worksheet.Cell("B9").Value = labOrderPosItem.ValueMin;
                worksheet.Cell("B10").Value = labOrderPosItem.ValueMax;
                worksheet.Cell("B11").Value = labOrderPosItem.ActualValue;


                worksheet.Cell("A17").Value = "Date";
                worksheet.Cell("B17").Value = "Weight";
                worksheet.Cell("C17").Value = "Tolerancy Status";
                worksheet.Cell("D17").Value = "Deviation from Reference Weight";

                if (labOrderPosItem.XMLConfig != null)
                {
                    SamplePiStats piStats = null;
                    try
                    {
                        piStats = labOrderPosItem[PWSamplePiLightBox.C_LabOrderExtFieldStats] as SamplePiStats;
                    }
                    catch { }

                    if (piStats != null)
                    {
                        foreach (SamplePiValue item in piStats.Values)
                        {
                            worksheet.Cell(rowPiStats, 1).Value = item.DTStamp;
                            worksheet.Cell(rowPiStats, 2).Value = item.Value;
                            worksheet.Cell(rowPiStats, 3).Value = item.TolState;

                            if (item.Value > refValue)
                                worksheet.Cell(rowPiStats, 4).Value = item.Value - refValue;
                            else
                                worksheet.Cell(rowPiStats, 4).Value = refValue - item.Value;

                            if (item.TolState == 1 || item.TolState == -1)
                                tolCounter++;

                            counter++;
                            rowPiStats++;
                        }

                    }
                }

                worksheet.Cell("B13").Value = counter;
                worksheet.Cell("B14").Value = counter - tolCounter;
                worksheet.Cell("B15").Value = tolCounter;

                #region Excel Styling

                var bottomTableHeader = worksheet.Range(17, 1, 17, 4);
                bottomTableHeader.Style.Font.Bold = true;
                bottomTableHeader.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;

                var rngTable = worksheet.Range(17, 1, rowPiStats, 4);
                rngTable.Sort("4 Desc");

                worksheet.Cell("B2").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                worksheet.Cell("B4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                #endregion
            }



        }

        private static core.datamodel.ACClass GetLabOrderMachine(Database database, LabOrder labOrder)
        {
            gip.core.datamodel.ACClass machine = null;

            if (labOrder.RefACClassID == null)
            {
                machine = GetMachine(database, labOrder);
                if (machine != null)
                {
                    labOrder.RefACClassID = machine.ACClassID;
                }
            }
            else if (labOrder.RefACClassID != null)
            {
                machine = database.ACClass.FirstOrDefault(c => c.ACClassID == labOrder.RefACClassID);
            }

            return machine;
        }

        #endregion

        #region provide machines

        private static gip.core.datamodel.ACClass GetMachine(Database database, LabOrder labOrder)
        {

            List<OrderLog> orderLogs = labOrder.ProdOrderPartslistPos.OrderLog_ProdOrderPartslistPos.ToList();

            if (!orderLogs.Any())
            {
                orderLogs =
                    labOrder
                    .ProdOrderPartslistPos
                    .ProdOrderPartslist
                    .ProdOrderPartslistPos_ProdOrderPartslist
                    .SelectMany(c => c.OrderLog_ProdOrderPartslistPos)
                    .ToList();

            }
            Guid[] programLogIDs = orderLogs.Select(x => x.VBiACProgramLogID).ToArray();
            gip.core.datamodel.ACProgramLog[] programLogs = database.ACProgramLog.Where(c => programLogIDs.Contains(c.ACProgramLogID)).ToArray();
            return GetMachine(database, programLogs);
        }

        private static gip.core.datamodel.ACClass GetMachine(Database database, gip.core.datamodel.ACProgramLog[] programLogs)
        {
            gip.core.datamodel.ACClass machine = null;

            foreach (gip.core.datamodel.ACProgramLog programLog in programLogs)
            {
                machine = GetMachine(database, programLog);
                if (machine != null)
                    break;
            }

            return machine;
        }

        private static gip.core.datamodel.ACClass GetMachine(Database database, gip.core.datamodel.ACProgramLog programLog)
        {
            if (programLog.RefACClassID != null)
                return database.ACClass.FirstOrDefault(c => c.ACClassID == programLog.RefACClassID);
            else
                return GetMachine(database, programLog.ACProgramLog_ParentACProgramLog.ToArray());
        }

        #endregion

        #region Helper Functions
        public static string TruncateAtWord(this string value, int length)
        {

            if (value == null || value.Length < length)
                return value;

            if (value.IndexOf(" ", length) < length && value.IndexOf(" ", length) > 0)
                return value.Substring(0, value.IndexOf(" ", length));
            else
                return value.Substring(0, length);
        }

        #endregion
    }
}
