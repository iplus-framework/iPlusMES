using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    public static class LabOrderToExcel
    {

        #region Create Excel Table
        public static void DoLabOrderToExcel(string fileName, LabOrder[] labOrders)
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
                DoLabOrderToExcel(workSheet, labOrder);
                workSheet.Columns().AdjustToContents();
            }

            workBook.SaveAs(fileName);
        }

        #endregion

        #region Excel Cell Printing

        public static void DoLabOrderToExcel(IXLWorksheet worksheet, LabOrder labOrder)
        {
            worksheet.Cell("A1").Value = "Laboratory Order No";
            worksheet.Cell("B1").Value = labOrder.LabOrderNo;
            worksheet.Cell("A2").Value = "Production Order";
            worksheet.Cell("B2").Value = labOrder.ProdOrderPartslistPos?.ProdOrderPartslist.ProdOrder.ProgramNo;
            worksheet.Cell("A3").Value = "Update Dates";
            worksheet.Cell("B3").Value = labOrder.SampleTakingDate;
            worksheet.Cell("D1").Value = "Material Name";
            worksheet.Cell("E1").Value = labOrder.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1;


            worksheet.Cell("A5").Value = "Sequence";
            worksheet.Cell("B5").Value = "Key";
            worksheet.Cell("C5").Value = "Reference Value";
            worksheet.Cell("D5").Value = "Average Value";
            worksheet.Cell("E5").Value = "Lab Order Pos. Status";
            worksheet.Cell("F5").Value = "Comment";
            worksheet.Cell("G5").Value = "Lowest value for alarm";
            worksheet.Cell("H5").Value = "Lowest value";
            worksheet.Cell("I5").Value = "Maximum value";
            worksheet.Cell("J5").Value = "Maximum value for alarm";

            int rowPos = 6;

            foreach (LabOrderPos labOrderPosItem in labOrder.LabOrderPos_LabOrder.ToArray())
            {
                double refValue = (double)labOrderPosItem.ReferenceValue;
                int rowPiStats = 11;

                worksheet.Cell(rowPos, 1).Value = labOrderPosItem.Sequence;
                if (labOrderPosItem.MDLabTag != null) 
                {
                    worksheet.Cell(rowPos, 2).Value = labOrderPosItem.MDLabTag.MDKey;
                }

                worksheet.Cell(rowPos, 3).Value = labOrderPosItem.ReferenceValue;
                worksheet.Cell(rowPos, 4).Value = labOrderPosItem.ActualValue;

                if (labOrderPosItem.MDLabOrderPosState != null) 
                { 
                    worksheet.Cell(rowPos, 5).Value = labOrderPosItem.MDLabOrderPosState.MDKey; 
                }

                worksheet.Cell(rowPos, 6).Value = labOrderPosItem.Comment;
                worksheet.Cell(rowPos, 7).Value = labOrderPosItem.ValueMinMin;
                worksheet.Cell(rowPos, 8).Value = labOrderPosItem.ValueMin;
                worksheet.Cell(rowPos, 9).Value = labOrderPosItem.ValueMax;
                worksheet.Cell(rowPos, 10).Value = labOrderPosItem.ValueMaxMax;
                rowPos++;



                worksheet.Cell("A10").Value = "Date";
                worksheet.Cell("B10").Value = "Weight";
                worksheet.Cell("C10").Value = "Tolerancy Status";
                worksheet.Cell("D10").Value = "Deviation from Reference Weight";

                if (labOrderPosItem.XMLConfig != null) 
                {
                    SamplePiStats piStats = null;
                    try
                    {
                        piStats = labOrderPosItem[PWSamplePiLightBox.C_LabOrderExtFieldStats] as SamplePiStats;
                    } catch { }

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


                            rowPiStats++;
                        }

                    }
                }

                #region Excel Styling

                var bottomTableHeader = worksheet.Range(10, 1, 10, 4);
                bottomTableHeader.Style.Font.Bold = true;
                bottomTableHeader.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;

                var rngTable = worksheet.Range(10, 1, rowPiStats, 4);
                rngTable.Sort("4 Desc");

                #endregion
            }

        }

        #endregion

        #region Helper Functions
        public static string TruncateAtWord(this string value, int length)
        {
            if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
                return value;

            return value.Substring(0, value.IndexOf(" ", length));
        }

        #endregion
    }
}
