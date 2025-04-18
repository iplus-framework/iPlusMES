﻿using gip.core.datamodel;
using VD = gip.mes.datamodel;

namespace gip.mes.maintenance
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance role'}de{'Maintenance role'}", Global.ACKinds.TACSimpleClass)]
    public class VBGroupWrapper : ACObjectItem
    {
        public VBGroupWrapper()
            : base("")
        {

        }

        public VD.VBGroup VBGroup
        {
            get;
            set;
        }

        private bool _IsChecked;
        [ACPropertyInfo(999)]
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                _IsChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
}
