// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.processapplication;
using gip.mes.datamodel;

namespace gip.mes.webservices
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan-Controller for PAFBinSelection'}de{'Scan-Controller für PAFBinSelection'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    public class PAFBinSelectionSC : PAScannedCompContrBase
    {
        #region c'tors
        new public const string ClassName = "PAFBinSelectionSC";

        public PAFBinSelectionSC(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion


        #region Properties
        protected override Type OnGetControlledType()
        {
            return typeof(PAFBinSelection);
        }
        #endregion 


        #region Methods
        public override void HandleBarcodeSequence(ACComponent component, BarcodeSequence sequence)
        {
            BarcodeEntity entityFacility, entityCharge;
            ParentDecoder.GetFacilityEntitiesFromSequence(sequence, out entityFacility, out entityCharge);
            Guid facilityID, facilityChargeID;
            ParentDecoder.GetGuidFromFacilityEntities(entityFacility, entityCharge, out facilityID, out facilityChargeID);

            BarcodeSequenceBase result = component.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + PAFWorkTaskScanBase.MN_OnScanEvent, facilityID, facilityChargeID, sequence.Sequence.Count) as BarcodeSequenceBase;
            if (result != null)
            {
                sequence.State = result.State;
                sequence.Message = result.Message;
            }
            else
            {
                // Error50352: No response from process function!
                sequence.Message = new Msg(this, eMsgLevel.Error, ClassName, "HandleBarcodeSequence", 10, "Error50352");
                sequence.State = BarcodeSequence.ActionState.Cancelled;
            }
        }
        #endregion
    }

}
