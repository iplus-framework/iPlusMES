// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.bso.sales;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.reporthandler.avui;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.sales.avui
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
                BSOInvoice bsoInvoice = ParentACComponent as BSOInvoice;
                if (bsoInvoice != null)
                {
                    string langCode = ResolveLangCode(bsoInvoice.TempReportData);
                    bsoInvoice.BuildInvoicePosData(langCode);
                }
            }

            base.OnPrintingPhase(reportEngine, printingPhase);
        }

        private static string ResolveLangCode(ReportData reportData)
        {
            gip.core.datamodel.ACClassDesign design = reportData?.ACClassDesign;

            if (design?.ACIdentifier == null)
                return "de";

            if (design.ACIdentifier.EndsWith("Hr"))
                return "hr";

            if (design.ACIdentifier.EndsWith("En"))
                return "en";

            return "de";
        }
    }
}
