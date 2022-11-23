using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    public static class LabOrderToExcel
    {

        #region Create Excel Table
        public static void DoLabOrderToExcel(Database database, string fileName, LabOrder[] labOrders)
        {
            XLWorkbook workBook = new XLWorkbook();
            List<String> sheetNames = new List<String>();
            int indexOfMaterial = 0;

            foreach (LabOrder labOrder in labOrders)
            {
                String sheetName = labOrder.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1;
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

        public static void DoLabOrderToExcel(Database database, IXLWorksheet worksheet, LabOrder labOrder)
        {
            worksheet.Cell("A1").Value = "Laboratory Order No";
            worksheet.Cell("B1").Value = labOrder.LabOrderNo;
            worksheet.Cell("A2").Value = "Material Name";
            worksheet.Cell("B2").Value = labOrder.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1;
            worksheet.Cell("A3").Value = "Production Order";
            worksheet.Cell("B3").Value = labOrder.ProdOrderPartslistPos?.ProdOrderPartslist.ProdOrder.ProgramNo;
            worksheet.Cell("A4").Value = "Update Dates";
            worksheet.Cell("B4").Value = labOrder.SampleTakingDate;
            worksheet.Cell("A5").Value = "Proizvodna linija";

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

            worksheet.Cell("B5").Value = machine?.ACCaption;

            worksheet.Cell("A7").Value = "Reference Value";
            worksheet.Cell("A8").Value = "Lowest value";
            worksheet.Cell("A9").Value = "Maximum value";
            worksheet.Cell("A10").Value = "Average Value";

            worksheet.Cell("A12").Value = "Broj vaganja";
            worksheet.Cell("A13").Value = "Broj vaganja u toleranciji";
            worksheet.Cell("A14").Value = "Broj vaganja izvan tolerancije";

            int tolCounter = 0;
            int counter = 0;

            foreach (LabOrderPos labOrderPosItem in labOrder.LabOrderPos_LabOrder.ToArray())
            {
                double refValue = (double)labOrderPosItem.ReferenceValue;
                int rowPiStats = 17;

                worksheet.Cell("B7").Value = labOrderPosItem.ReferenceValue;
                worksheet.Cell("B8").Value = labOrderPosItem.ValueMin;
                worksheet.Cell("B9").Value = labOrderPosItem.ValueMax;
                worksheet.Cell("B10").Value = labOrderPosItem.ActualValue;


                worksheet.Cell("A16").Value = "Date";
                worksheet.Cell("B16").Value = "Weight";
                worksheet.Cell("C16").Value = "Tolerancy Status";
                worksheet.Cell("D16").Value = "Deviation from Reference Weight";

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

                worksheet.Cell("B12").Value = counter;
                worksheet.Cell("B13").Value = counter - tolCounter;
                worksheet.Cell("B14").Value = tolCounter;

                #region Excel Styling

                var bottomTableHeader = worksheet.Range(16, 1, 16, 4);
                bottomTableHeader.Style.Font.Bold = true;
                bottomTableHeader.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;

                var rngTable = worksheet.Range(16, 1, rowPiStats, 4);
                rngTable.Sort("4 Desc");

                worksheet.Cell("B4").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

                #endregion
            }



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
