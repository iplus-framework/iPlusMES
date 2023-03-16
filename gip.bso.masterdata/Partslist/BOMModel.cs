using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'BOMModel'}de{'BOMModel'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class BOMModel
    {

        [ACPropertyInfo(100, "", ConstApp.ProdOrderPartslist)]
        public string PartslistNo
        {
            get;
            set;
        }

        [ACPropertyInfo(101, "", Const.EntitySortSequence)]
        public int Sequence
        {
            get;
            set;
        }

        [ACPropertyInfo(102, "", ConstApp.MaterialNo)]
        public string MaterialNo
        {
            get;
            set;
        }

        [ACPropertyInfo(103, "", ConstApp.MaterialName1)]
        public string MaterialName
        {

            get;
            set;
        }

        [ACPropertyInfo(104, "", ConstApp.TargetQuantityUOM)]
        public double TargetQuantityUOM
        {

            get;
            set;
        }

        [ACPropertyInfo(105, "", ConstApp.TechnicalSymbol)]
        public string BaseMDUnit
        {

            get;
            set;
        }

        [ACPropertyInfo(106, "", ConstApp.TargetQuantity)]
        public double TargetQuantity
        {

            get;
            set;
        }

        [ACPropertyInfo(107, "", ConstApp.TechnicalSymbol)]
        public string MDUnit
        {

            get;
            set;
        }

        [ACPropertyInfo(108, "", "en{'T.Q per unit (UOM)'}de{'Sollm. pro Einheit (BME)'}")]
        public double TargetQuantityPerUnitUOM
        {

            get;
            set;
        }

        [ACPropertyInfo(109, "", "en{'T.Q per unit'}de{'Sollm. pro Einheit'}")]
        public double TargetQuantityPerUnit
        {

            get;
            set;
        }
    }
}
