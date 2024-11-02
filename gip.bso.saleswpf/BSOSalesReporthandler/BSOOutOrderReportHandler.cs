// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.bso.sales;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.reporthandlerwpf.Flowdoc;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace gip.bso.saleswpf
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'BSOOutOrderReportHandler'}de{'BSOOutOrderReportHandler'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOffer.ClassName)]
    public class BSOOutOrderReportHandler : ACComponent
    {
        public BSOOutOrderReportHandler(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override void OnPrintingPhase(object reportEngine, ACPrintingPhase printingPhase)
        {
            if (printingPhase == ACPrintingPhase.Started)
            {
                ReportDocument doc = reportEngine as ReportDocument;
                if (
                    doc != null
                    && doc.ReportData != null
                    && doc.ReportData.Any(c => c.ACClassDesign != null
                                               && (c.ACClassDesign.ACIdentifier.EndsWith("De")) || c.ACClassDesign.ACIdentifier.EndsWith("En") || c.ACClassDesign.ACIdentifier.EndsWith("Hr")))
                {
                    doc.SetFlowDocObjValue += Doc_SetFlowDocObjValue;
                    gip.core.datamodel.ACClassDesign design = doc.ReportData.Select(c => c.ACClassDesign).FirstOrDefault();
                    string langCode = "de";
                    if (design != null)
                    {
                        if (design.ACIdentifier.EndsWith("Hr"))
                            langCode = "hr";
                        if (design.ACIdentifier.EndsWith("En"))
                            langCode = "en";
                    }
                    BSOOutOrder bsoOutOrder = ParentACComponent as BSOOutOrder;
                    if (bsoOutOrder != null)
                        bsoOutOrder.BuildOutOrderPosData(langCode);
                }
            }
            else
            {
                ReportDocument doc = reportEngine as ReportDocument;
                if (doc != null)
                {
                    doc.SetFlowDocObjValue -= Doc_SetFlowDocObjValue;
                }
            }

            base.OnPrintingPhase(reportEngine, printingPhase);
        }

        private void Doc_SetFlowDocObjValue(object sender, PaginatorOnSetValueEventArgs e)
        {
            OutOrderPos pos = e.ParentDataRow as OutOrderPos;
            if (pos != null && pos.GroupSum && pos.OutOrderPosID == new Guid())
            {
                var inlineCell = e.FlowDocObj as InlineTableCellValue;
                if (inlineCell != null)
                {
                    var tableCell = (inlineCell.Parent as Paragraph)?.Parent as TableCell;
                    if (tableCell != null)
                    {
                        if (inlineCell.VBContent == "MaterialNo")
                        {
                            TableRow tableRow = tableCell.Parent as TableRow;
                            if (tableRow != null && tableRow.Cells.Count > 6)
                            {
                                tableRow.Cells.RemoveAt(2);
                                tableRow.Cells.RemoveAt(2);
                                tableRow.Cells.RemoveAt(2);
                                tableRow.Cells.RemoveAt(2);
                            }
                            tableCell.ColumnSpan = 2;
                        }

                        else if (inlineCell.VBContent == "TotalPricePrinted")
                        {
                            tableCell.ColumnSpan = 4;
                            tableCell.BorderBrush = Brushes.Black;
                            tableCell.BorderThickness = new System.Windows.Thickness(0, 1, 0, 1);
                            tableCell.TextAlignment = System.Windows.TextAlignment.Right;
                        }
                        tableCell.FontWeight = System.Windows.FontWeights.Bold;
                    }
                }
            }
        }
    }
}
