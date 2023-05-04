using gip.core.datamodel;
using System.Collections.Generic;
using gip.mes.facility;
using System.ComponentModel;
using System;
using dbMes = gip.mes.datamodel;
using System.ComponentModel.Design;

namespace gip.bso.masterdata
{

    [ACClassInfo(Const.PackName_VarioMaterial, "en{'RuleSelection'}de{'RuleSelection'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleSelection : INotifyPropertyChanged
    {
        #region DI
        public RuleGroup RuleGroup { get; set; }

        #endregion

        #region Properies

        [ACPropertyInfo(100, "", Const.Workflow)]
        public MachineItem MachineItem { get; set; }

        [ACPropertyInfo(101, "", Const.Workflow)]
        public ACClass Target { get; set; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            string material = "-";
            if (MachineItem != null && MachineItem.Material != null)
            {
                material = MachineItem.Material.ToString();
            }
            string isSelected = "";
            if (MachineItem != null && MachineItem.IsSelected)
            {
                isSelected = "x";
            }
            return $"[{isSelected}] {material} | {MachineItem?.Machine} -> {Target}";
        }

        #endregion

    }
}
