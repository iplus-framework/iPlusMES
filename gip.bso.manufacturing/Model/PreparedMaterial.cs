using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'PreparedMaterial'}de{'PreparedMaterial.'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class PreparedMaterial
    {
        public Material Material { get; set; }

        [ACPropertyInfo(100, "Sn", "en{'No'}de{'Nr'}")]
        public int Sn { get; set; }

        [ACPropertyInfo(101, "MaterialNo", "en{'Material-No.'}de{'Material-Nr.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(102, "MaterialName", "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(103, "MaterialName", "en{'Image'}de{'Bild'}")]
        public string DefaultThumbImage { get; set; }

        [ACPropertyInfo(104, "TargetQuantityUOM", "en{'Required quantity'}de{'Bedarfsmenge'}")]
        public double TargetQuantityUOM { get; set; }

        [ACPropertyInfo(105, "AvailableQuantity", "en{'Stock Quantity'}de{'Lagermenge'}")]
        public double AvailableQuantityUOM { get; set; }

        [ACPropertyInfo(106, "AvailableQuantityDestinationUOM", "en{'Destination Quantity'}de{'Bestimmungsmenge'}")]
        public double AvailableQuantityDestinationUOM { get; set; }

        [ACPropertyInfo(107, "PickingPosQuantity", "en{'Planned'}de{'Geplant'}")]
        public double? PickingPosQuantityUOM { get; set; }

        [ACPropertyInfo(109, "MissingQuantity", "en{'Missing Planned'}de{'Fehlende Planmenge'}")]
        public double? MissingQuantityUOM { get; set; }

        public Guid[] RelatedIDs { get; set; }

        public PickingRelationTypeEnum PickingRelationType { get; set; }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid[] MDSchedulingGroupIDs { get; set; }
    }
}
