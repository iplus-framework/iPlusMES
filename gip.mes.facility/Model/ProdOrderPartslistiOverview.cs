using gip.core.datamodel;
using gip.mes.datamodel;
using System;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'ProdOrderPartslistiOverview'}de{'ProdOrderPartslistiOverview.'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class ProdOrderPartslistiOverview
    {

        #region Const
        public const string Const_General = "General";
        public const string Const_Input = "Input";
        public const string Const_Output = "Output";
        public const string Const_Usage = "Output";
        #endregion

        #region General

        [ACPropertyInfo(100, Const_General, ConstApp.ProdOrderProgramNo)]
        public string ProgramNo { get; set; }

        [ACPropertyInfo(101, Const_General, "en{'MaterialNo'}de{'MaterialNo.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(102, Const_General, "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(103, Const_General, "en{'Department'}de{'Abteilung.'}")]
        public string DepartmentUserName { get; set; }

        [ACPropertyInfo(104, Const_General, ConstApp.MDUnit)]
        public string MDUnitName { get; set; }

        #endregion

        #region Input

        [ACPropertyInfo(200, Const_Input, "en{'Target Input (I1)'}de{'Soll Einsatz (Ein1)'}")]
        public double OutwardTargetQuantityUOM { get; set; }

        [ACPropertyInfo(201, Const_Input, "en{'Actual Input (I2)'}de{'Ist Einsatz (Ein2)'}")]
        public double OutwardActualQuantityUOM { get; set; }

        [ACPropertyInfo(202, Const_Input, "en{'Diff. Input = I2 - I1 (I3)'}de{'Diff. Einsatz = Ein2 - Ein1 (Ein3)'}")]
        public double OutwardDiffQuantityUOM { get; set; }

        #endregion

        #region Output

        [ACPropertyInfo(300, Const_Output, "en{'Target Output (O1)'}de{'Soll Ergebnis (Erg1)'}")]
        public double InwardTargetQuantityUOM { get; set; }

        [ACPropertyInfo(301, Const_Output, "en{'Actual Output (O2)'}de{'Ist Ergebnis (Erg2)'}")]
        public double InwardActualQuantityUOM { get; set; }

        [ACPropertyInfo(302, Const_Output, "en{'Diff. Output = O2 - O1 (O3)'}de{'Diff. Ergebnis = Erg2 - Erg1 (Erg3)'}")]
        public double InwardDiffQuantityUOM { get; set; }

        [ACPropertyInfo(303, Const_Output, "en{'Diff. Output = O2 - O1 (O3)'}de{'Diff. Erg.-Eins. = Erg2 - Ein2'}")]
        public double ActualRestQuantityUOM { get; set; }

        #endregion

        #region Usage

        [ACPropertyInfo(400, Const_Usage, "en{'Target Usage (U1)'}de{'Soll Verw. (V1)'}")]
        public double UsageTargetQuantityUOM { get; set; }


        [ACPropertyInfo(401, Const_Usage, "en{'Actual Usage (U2)'}de{'Ist Verw. (V2)'}")]
        public double UsageActualQuantityUOM { get; set; }

        [ACPropertyInfo(402, Const_Usage, "en{'Diff. Usage = U2 - U1'}de{'Diff. Verw.  = V2 - V1'}")]
        public double UsageDiffQuantityUOM { get; set; }

        [ACPropertyInfo(403, Const_Usage, "en{'Diff. Usg-Out = U2 - O2'}de{'Diff. Verw.-Erg.= U2 - O2'}")]
        public double UsageInwardDiffQuantityUOM { get; set; }

        #endregion

        #region Others
        public Guid ProdOrderPartslistID { get; set; }
        #endregion

        #region Methods
        public void CalculateDiff()
        {
            OutwardDiffQuantityUOM = OutwardActualQuantityUOM - OutwardTargetQuantityUOM;
            InwardDiffQuantityUOM = InwardActualQuantityUOM - InwardTargetQuantityUOM;
            ActualRestQuantityUOM = InwardActualQuantityUOM - OutwardActualQuantityUOM;
            UsageDiffQuantityUOM = UsageActualQuantityUOM - UsageTargetQuantityUOM;
            UsageInwardDiffQuantityUOM = UsageActualQuantityUOM - InwardActualQuantityUOM;
        }
        #endregion
    }
}
