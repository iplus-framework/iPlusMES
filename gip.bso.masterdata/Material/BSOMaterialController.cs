// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOMaterial.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using static gip.core.datamodel.Global;
using Microsoft.EntityFrameworkCore;
using gip.mes.facility;
using gip.core.media;

namespace gip.bso.masterdata
{
    public partial class BSOMaterial : BSOMaterialExplorer
    {
        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Save):
                    _ = Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter != null && acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(NewMaterialUnit):
                    _ = NewMaterialUnit();
                    return true;
                case nameof(IsEnabledNewMaterialUnit):
                    result = IsEnabledNewMaterialUnit();
                    return true;
                case nameof(DeleteMaterialUnit):
                    DeleteMaterialUnit();
                    return true;
                case nameof(IsEnabledDeleteMaterialUnit):
                    result = IsEnabledDeleteMaterialUnit();
                    return true;
                case nameof(NewMaterialUnitOK):
                    NewMaterialUnitOK();
                    return true;
                case nameof(IsEnabledNewMaterialUnitOK):
                    result = IsEnabledNewMaterialUnitOK();
                    return true;
                case nameof(NewMaterialUnitCancel):
                    NewMaterialUnitCancel();
                    return true;
                case nameof(LoadMaterialCalculation):
                    LoadMaterialCalculation();
                    return true;
                case nameof(IsEnabledLoadMaterialCalculation):
                    result = IsEnabledLoadMaterialCalculation();
                    return true;
                case nameof(NewMaterialCalculation):
                    NewMaterialCalculation();
                    return true;
                case nameof(IsEnabledNewMaterialCalculation):
                    result = IsEnabledNewMaterialCalculation();
                    return true;
                case nameof(DeleteMaterialCalculation):
                    DeleteMaterialCalculation();
                    return true;
                case nameof(IsEnabledDeleteMaterialCalculation):
                    result = IsEnabledDeleteMaterialCalculation();
                    return true;
                case nameof(LoadACConfig):
                    LoadACConfig();
                    return true;
                case nameof(IsEnabledLoadACConfig):
                    result = IsEnabledLoadACConfig();
                    return true;
                case nameof(NewACConfig):
                    NewACConfig();
                    return true;
                case nameof(IsEnabledNewACConfig):
                    result = IsEnabledNewACConfig();
                    return true;
                case nameof(DeleteACConfig):
                    DeleteACConfig();
                    return true;
                case nameof(IsEnabledDeleteACConfig):
                    result = IsEnabledDeleteACConfig();
                    return true;
                case nameof(ConvertTestToBase):
                    ConvertTestToBase();
                    return true;
                case nameof(IsEnabledConvertTestToBase):
                    result = IsEnabledConvertTestToBase();
                    return true;
                case nameof(ConvertTestFromBase):
                    ConvertTestFromBase();
                    return true;
                case nameof(IsEnabledConvertTestFromBase):
                    result = IsEnabledConvertTestFromBase();
                    return true;
                case nameof(ConvertTest):
                    ConvertTest();
                    return true;
                case nameof(IsEnabledConvertTest):
                    result = IsEnabledConvertTest();
                    return true;
                case nameof(AddFacility):
                    AddFacility();
                    return true;
                case nameof(IsEnabledAddFacility):
                    result = IsEnabledAddFacility();
                    return true;
                case nameof(DeleteFacility):
                    DeleteFacility();
                    return true;
                case nameof(IsEnabledDeleteFacility):
                    result = IsEnabledDeleteFacility();
                    return true;
                case nameof(ShowFacility):
                    _ = ShowFacility();
                    return true;
                case nameof(IsEnabledShowFacility):
                    result = IsEnabledShowFacility();
                    return true;
                case nameof(GenerateTestOEEData):
                    GenerateTestOEEData();
                    return true;
                case nameof(IsEnabledGenerateTestOEEData):
                    result = IsEnabledGenerateTestOEEData();
                    return true;
                case nameof(DeleteTestOEEData):
                    DeleteTestOEEData();
                    return true;
                case nameof(IsEnabledDeleteTestOEEData):
                    result = IsEnabledDeleteTestOEEData();
                    return true;
                case nameof(RecalcThroughputAverage):
                    RecalcThroughputAverage();
                    return true;
                case nameof(IsEnabledRecalcThroughputAverage):
                    result = IsEnabledRecalcThroughputAverage();
                    return true;
                case nameof(RecalcThroughputAndOEE):
                    RecalcThroughputAndOEE();
                    return true;
                case nameof(IsEnabledRecalcThroughputAndOEE):
                    result = IsEnabledRecalcThroughputAndOEE();
                    return true;
                case nameof(TranslationNew):
                    TranslationNew();
                    return true;
                case nameof(TranslationDelete):
                    TranslationDelete();
                    return true;
                case nameof(IsEnabledTranslationNew):
                    result = IsEnabledTranslationNew();
                    return true;
                case nameof(IsEnabledTranslationDelete):
                    result = IsEnabledTranslationDelete();
                    return true;
                case nameof(SearchAssociatedPos):
                    SearchAssociatedPos();
                    return true;
                case nameof(IsEnabledSearchAssociatedPos):
                    result = IsEnabledSearchAssociatedPos();
                    return true;
                case nameof(OpenQueryDialog):
                    result = OpenQueryDialog();
                    return true;
                case nameof(AddPWMethodNodeConfig):
                    AddPWMethodNodeConfig();
                    return true;
                case nameof(IsEnabledAddPWMethodNodeConfig):
                    result = IsEnabledAddPWMethodNodeConfig();
                    return true;
                case nameof(DeletePWMethodNodeConfig):
                    DeletePWMethodNodeConfig();
                    return true;
                case nameof(IsEnabledDeletePWMethodNodeConfig):
                    result = IsEnabledDeletePWMethodNodeConfig();
                    return true;
                case nameof(ValidateInput):
                    result = ValidateInput((System.String)acParameter[0], (System.Object)acParameter[1], (System.Globalization.CultureInfo)acParameter[2]);
                    return true;
                case nameof(ConvertAmbientVolToRefVol15):
                    ConvertAmbientVolToRefVol15();
                    return true;
                case nameof(IsEnabledConvertAmbientVolToRefVol15):
                    result = IsEnabledConvertAmbientVolToRefVol15();
                    return true;
                case nameof(ConvertRefVol15ToAmbientVol):
                    ConvertRefVol15ToAmbientVol();
                    return true;
                case nameof(IsEnabledConvertRefVol15ToAmbientVol):
                    result = IsEnabledConvertRefVol15ToAmbientVol();
                    return true;
                case nameof(ConvertAmbVolToMass):
                    ConvertAmbVolToMass();
                    return true;
                case nameof(IsEnabledConvertAmbVolToMass):
                    result = IsEnabledConvertAmbVolToMass();
                    return true;
                case nameof(ConvertMassToAmbVol):
                    ConvertMassToAmbVol();
                    return true;
                case nameof(IsEnabledConvertMassToAmbVol):
                    result = IsEnabledConvertMassToAmbVol();
                    return true;
                case nameof(CalcDensityAndTemp):
                    CalcDensityAndTemp();
                    return true;
                case nameof(IsEnabledCalcDensityAndTemp):
                    result = IsEnabledCalcDensityAndTemp();
                    return true;
                case nameof(CalcDensityAndVol):
                    CalcDensityAndVol();
                    return true;
                case nameof(IsEnabledCalcDensityAndVol):
                    result = IsEnabledCalcDensityAndVol();
                    return true;
                case nameof(ShowMaterialOptions):
                    _ = ShowMaterialOptions();
                    return true;
                case nameof(IsEnabledShowMaterialOptions):
                    result = IsEnabledShowMaterialOptions();
                    return true;
                case nameof(MoveToAnotherIntermediate):
                    MoveToAnotherIntermediate();
                    return true;
                case nameof(IsEnabledMoveToAnotherIntermediate):
                    result = IsEnabledMoveToAnotherIntermediate();
                    return true;
                case nameof(MoveAndReplaceMaterial):
                    MoveAndReplaceMaterial();
                    return true;
                case nameof(IsEnabledMoveAndReplaceMaterial):
                    result = IsEnabledMoveAndReplaceMaterial();
                    return true;
                case nameof(ReplaceMaterial):
                    ReplaceMaterial();
                    return true;
                case nameof(IsEnabledReplaceMaterial):
                    result = IsEnabledReplaceMaterial();
                    return true;
                case nameof(ShowDialogOrderInfo):
                    _ = ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(ShowDialogMaterial):
                    _ = ShowDialogMaterial((String)acParameter[0]);
                    return true;
                case nameof(NavigateToMaterialOverview):
                    NavigateToMaterialOverview();
                    return true;
                case nameof(IsEnabledNavigateToMaterialOverview):
                    result = IsEnabledNavigateToMaterialOverview();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(Load):
                case nameof(IsEnabledLoad):
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(CurrentMaterial), nameof(SelectedMaterial) };
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                case nameof(IsEnabledAddFacility):
                case nameof(IsEnabledDeleteFacility):
                case nameof(IsEnabledShowFacility):
                case nameof(IsEnabledGenerateTestOEEData):
                case nameof(IsEnabledDeleteTestOEEData):
                case nameof(IsEnabledRecalcThroughputAverage):
                case nameof(IsEnabledRecalcThroughputAndOEE):
                case nameof(IsEnabledTranslationNew):
                case nameof(IsEnabledTranslationDelete):
                case nameof(IsEnabledSearchAssociatedPos):
                case nameof(IsEnabledAddPWMethodNodeConfig):
                case nameof(IsEnabledDeletePWMethodNodeConfig):
                case nameof(IsEnabledConvertAmbientVolToRefVol15):
                case nameof(IsEnabledConvertRefVol15ToAmbientVol):
                case nameof(IsEnabledConvertAmbVolToMass):
                case nameof(IsEnabledConvertMassToAmbVol):
                case nameof(IsEnabledCalcDensityAndTemp):
                case nameof(IsEnabledCalcDensityAndVol):
                case nameof(IsEnabledShowMaterialOptions):
                case nameof(IsEnabledMoveToAnotherIntermediate):
                case nameof(IsEnabledMoveAndReplaceMaterial):
                case nameof(IsEnabledReplaceMaterial):
                case nameof(IsEnabledNavigateToMaterialOverview):
                case nameof(IsEnabledSave):
                case nameof(IsEnabledUndoSave):
                case nameof(IsEnabledNew):
                case nameof(IsEnabledConvertTestToBase):
                case nameof(IsEnabledConvertTestFromBase):
                case nameof(IsEnabledConvertTest):
                    return new string[] { nameof(CurrentMaterial) };
                case nameof(IsEnabledLoadMaterialCalculation):
                case nameof(IsEnabledNewMaterialCalculation):
                case nameof(IsEnabledDeleteMaterialCalculation):
                    return new string[] { nameof(CurrentMaterial), nameof(SelectedMaterialCalculation) };
                case nameof(IsEnabledLoadACConfig):
                case nameof(IsEnabledNewACConfig):
                case nameof(IsEnabledDeleteACConfig):
                    return new string[] { nameof(CurrentMaterial), nameof(SelectedACConfig) };
                case nameof(IsEnabledDeleteMaterialUnit):
                case nameof(IsEnabledNewMaterialUnit):
                case nameof(IsEnabledNewMaterialUnitOK):
                    return new string[] { nameof(CurrentMaterial), nameof(CurrentNewMaterialUnit) };
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}