// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.iplus
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-15-2012
// ***********************************************************************
// <copyright file="BSOProcessControl.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.bso.iplus;
using gip.mes.processapplication;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Class BSOPropertyLogPresenterVB
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Analysis of plant logs'}de{'Auswertung der Anlagenprotokolle'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOPropertyLogPresenterVB : VBBSOPropertyLogPresenter
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOProcessControl"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOPropertyLogPresenterVB(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region BSO->ACProperty

        #region Database
        private DatabaseApp _DatabaseApp = null;
        /// <summary>Returns the shared Database-Context for BSO's by calling GetAppContextForBSO()</summary>
        /// <value>Returns the shared Database-Context.</value>
        public virtual DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp == null && this.InitState != ACInitState.Destructed && this.InitState != ACInitState.Destructing && this.InitState != ACInitState.DisposedToPool && this.InitState != ACInitState.DisposingToPool)
                    _DatabaseApp = ACObjectContextManager.GetOrCreateContext<DatabaseApp>(this.GetACUrl(),"", new core.datamodel.Database());
                return _DatabaseApp as DatabaseApp;
            }
        }

        /// <summary>
        /// Overriden: Returns the DatabaseApp-Property.
        /// </summary>
        /// <value>The context as IACEntityObjectContext.</value>
        public override IACEntityObjectContext Database
        {
            get
            {
                return DatabaseApp;
            }
        }
        #endregion

        #endregion

        #region Methods
        [ACMethodInteraction("", "en{'Show Order'}de{'Show Order'}", 781, true, nameof(SelectedPropertyLog))]
        public void ShowOrder()
        {
            if (SelectedItemInTimeline == null || SelectedItemInTimeline.ProgramLog == null || !SelectedItemInTimeline.ProgramLog.Any())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                foreach (core.datamodel.ACProgramLog programLog in SelectedItemInTimeline.ProgramLog)
                {
                    OrderLog orderLog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == programLog.ACProgramLogID);
                    if (orderLog == null)
                    {
                        var pLog = programLog.FromAppContext<gip.mes.datamodel.ACProgramLog>(DatabaseApp);
                        while (orderLog == null && programLog.ACProgramLog1_ParentACProgramLog != null)
                        {
                            orderLog = pLog.OrderLog_VBiACProgramLog;
                            pLog = pLog.ACProgramLog1_ParentACProgramLog;
                        }
                    }
                    if (orderLog == null)
                        continue;

                    if (orderLog.ProdOrderPartslistPosID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.ProdOrderPartslistPosID.Value,
                            EntityName = ProdOrderPartslistPos.ClassName
                        });
                    }
                    if (orderLog.ProdOrderPartslistPosRelationID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.ProdOrderPartslistPosRelationID.Value,
                            EntityName = ProdOrderPartslistPosRelation.ClassName
                        });
                    }
                    if (orderLog.PickingPosID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.PickingPosID.Value,
                            EntityName = PickingPos.ClassName
                        });
                    }
                    if (orderLog.FacilityBookingID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.FacilityBookingID.Value,
                            EntityName = FacilityBooking.ClassName
                        });
                    }
                    if (orderLog.DeliveryNotePosID.HasValue)
                    {
                        info.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = orderLog.DeliveryNotePosID.Value,
                            EntityName = DeliveryNotePos.ClassName
                        });
                    }
                    if (!info.Entities.Any())
                        info.Entities.Add(new PAOrderInfoEntry(OrderLog.ClassName, orderLog.VBiACProgramLogID));
                }
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledShowOrder()
        {
            return SelectedItemInTimeline != null && SelectedItemInTimeline.ProgramLog != null;
        }

        #endregion


        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(ShowOrder):
                    ShowOrder();
                    return true;
                case nameof(IsEnabledShowOrder):
                    result = IsEnabledShowOrder();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }

}
