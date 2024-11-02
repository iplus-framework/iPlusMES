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

namespace gip.bso.saleswpf
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'BSOInvoiceReportHandler'}de{'BSOInvoiceReportHandler'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOffer.ClassName)]
    public class BSOInvoiceReportHandler : ACComponent
    {
        public BSOInvoiceReportHandler(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override void OnPrintingPhase(object reportEngine, ACPrintingPhase printingPhase)
        {
            if (printingPhase == ACPrintingPhase.Started)
            {
                ReportDocument doc = reportEngine as ReportDocument;
                if (doc != null && doc.ReportData != null && doc.ReportData.Any(c => c.ACClassDesign != null
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
                    BSOInvoice bsoInvoice = ParentACComponent as BSOInvoice;
                    if (bsoInvoice != null)
                        bsoInvoice.BuildInvoicePosData(langCode);
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
            BSOInvoice bsoInvoice = ParentACComponent as BSOInvoice;
            InvoicePos pos = e.ParentDataRow as InvoicePos;
            if (e.FlowDocObj != null
                && (e.FlowDocObj.VBContent == "CurrentInvoice\\IsReverseCharge"
                || e.FlowDocObj.VBContent == "CurrentInvoice\\NotIsReverseCharge"))
            {
                if (bsoInvoice.CurrentInvoice != null
                    && ((!bsoInvoice.CurrentInvoice.IsReverseCharge && e.FlowDocObj.VBContent == "CurrentInvoice\\IsReverseCharge")
                        || (bsoInvoice.CurrentInvoice.IsReverseCharge && e.FlowDocObj.VBContent == "CurrentInvoice\\NotIsReverseCharge")))
                {
                    var inlineCell = e.FlowDocObj as InlineContextValue;
                    if (inlineCell != null)
                    {
                        var tableCell = (inlineCell.Parent as Paragraph)?.Parent as TableCell;
                        if (tableCell != null)
                        {
                            TableRow tableRow = tableCell.Parent as TableRow;
                            if (tableRow != null)
                                tableRow.Cells.Remove(tableCell);
                        }
                    }
                }
            }
            if (e.FlowDocObj != null
                && e.FlowDocObj.VBContent != null
                && (e.FlowDocObj.VBContent.StartsWith("CurrentInvoice\\MDCurrencyExchange\\")
                    || e.FlowDocObj.VBContent.StartsWith("CurrentInvoice\\Foreign"))
                )
            {
                if (bsoInvoice.CurrentInvoice != null && bsoInvoice.CurrentInvoice.MDCurrencyExchange == null)
                {
                    var inlineCell = e.FlowDocObj as InlineContextValue;
                    if (inlineCell != null)
                    {
                        var tableCell = (inlineCell.Parent as Paragraph)?.Parent as TableCell;
                        if (tableCell != null)
                        {
                            TableRow tableRow = tableCell.Parent as TableRow;
                            if (tableRow != null)
                                tableRow.Cells.Remove(tableCell);
                        }
                    }
                }
            }
        }
    }
}
