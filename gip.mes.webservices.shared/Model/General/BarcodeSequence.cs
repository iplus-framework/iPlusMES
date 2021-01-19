// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="FacilityChargeSumFacilityHelper.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using gip.mes.facility;
using gip.mes.datamodel;

namespace gip.mes.webservices
{
    [DataContract]
    public class BarcodeSequence : BarcodeSequenceBase
    {
        [DataMember]
        public string CurrentBarcode
        {
            get; set;
        }

        [DataMember]
        public bool PreviousLotConsumed
        {
            get; set;
        }

        [DataMember]
        public List<BarcodeEntity> Sequence
        {
            get; set;
        }

        public void AddQuestion(core.datamodel.Msg question)
        {
            if (question.MessageLevel != core.datamodel.eMsgLevel.Question && question.MessageLevel != core.datamodel.eMsgLevel.QuestionPrompt)
                return;

            Message = question;
            Sequence.Add(new BarcodeEntity() { MsgResult = core.datamodel.Global.MsgResult.Cancel });
            State = ActionState.Question;
        }
    }
}