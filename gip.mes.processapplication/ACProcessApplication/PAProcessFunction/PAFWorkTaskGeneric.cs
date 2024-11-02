// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Generic work task'}de{'Allgemeine Arbeitsaufgabe'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, nameof(PWWorkTaskGeneric), true)]
    public class PAFWorkTaskGeneric : PAFWorkTaskScanBase
    {
        #region Constructors

        static PAFWorkTaskGeneric()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFWorkTaskGeneric), ACStateConst.TMStart, CreateVirtualMethod("Work", "en{'Generic work task'}de{'Allgemeine Arbeitsaufgabe'}", typeof(PWWorkTaskGeneric)));
            RegisterExecuteHandler(typeof(PAFWorkTaskGeneric), HandleExecuteACMethod_PAFWorkTaskGeneric);
        }

        public PAFWorkTaskGeneric(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion

        #region Properties
        #endregion

        #region override methods
        protected override bool PWWorkTaskScanSelector(IACComponent c)
        {
            return c is PWWorkTaskGeneric;
        }

        protected override bool PWWorkTaskScanDeSelector(IACComponent c)
        {
            return c is PWWorkTaskGeneric;
        }

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAFWorkTaskGeneric(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAFWorkTaskScanBase(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        public override void InitializeRouteAndConfig(Database dbIPlus)
        {
        }

        protected override CompleteResult AnalyzeACMethodResult(ACMethod acMethod, out MsgWithDetails msg, CompleteResult completeResult)
        {
            msg = null;
            return CompleteResult.Succeeded;
        }

        protected override MsgWithDetails CompleteACMethodOnSMStarting(ACMethod acMethod, ACMethod previousParams)
        {
            return null;
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("Description", typeof(string), "", Global.ParamOption.Optional));
            paramTranslation.Add("Description", "en{'Description'}de{'Bechreibung'}");
            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        //protected override PAProdOrderPartslistWFInfo OnCreateNewWFInfo(List<PAProdOrderPartslistWFInfo> infoList, PWMethodProduction activeWorkflow, PWWorkTaskScanBase pwNode, bool forRelease, Guid intermediatePosID, Guid intermediateChildPosID)
        //{
        //    PAProdOrderPartslistWFInfo wfInfo = base.OnCreateNewWFInfo(infoList, activeWorkflow, pwNode, forRelease, intermediatePosID, intermediateChildPosID);
        //    PWWorkTaskGeneric pwGenericTask = pwNode as PWWorkTaskGeneric;
        //    // TODO: Remove after resolving json-serialization problem and replacement of newer version on mobile devices
        //    if (pwGenericTask != null)
        //    {
        //        wfInfo.PostingQSuggestionMode = pwGenericTask.PostingQuantitySuggestionMode.HasValue ? pwGenericTask.PostingQuantitySuggestionMode.Value : PostingQuantitySuggestionMode.OrderQuantity;
        //        wfInfo.PostingQSuggestionMode2 = pwGenericTask.PostingQuantitySuggestionMode2.HasValue ? pwGenericTask.PostingQuantitySuggestionMode2.Value : PostingQuantitySuggestionMode.None;
        //    }
        //    return wfInfo;
        //}

        #endregion
    }
}
