// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.bso.sales;
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
                BSOOutOrder bsoOutOrder = ParentACComponent as BSOOutOrder;
                if (bsoOutOrder != null)
                {
                    string langCode = ResolveLangCode(bsoOutOrder.TempReportData);
                    bsoOutOrder.BuildOutOrderPosData(langCode);
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
