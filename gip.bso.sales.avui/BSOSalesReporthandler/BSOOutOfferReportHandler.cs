// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.reporthandler.avui;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.bso.sales;

namespace gip.bso.sales.avui
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'BSOOutOfferReportHandler'}de{'BSOOutOfferReportHandler'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOffer.ClassName)]
    public class BSOOutOfferReportHandler : ACComponent
    {
        public BSOOutOfferReportHandler(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override void OnPrintingPhase(object reportEngine, ACPrintingPhase printingPhase)
        {
            if (printingPhase == ACPrintingPhase.Started)
            {
                BSOOutOffer bsoOutOffer = ParentACComponent as BSOOutOffer;
                if (bsoOutOffer != null)
                {
                    string langCode = ResolveLangCode(bsoOutOffer.TempReportData);
                    bsoOutOffer.BuildOutOfferPosData(langCode);
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
