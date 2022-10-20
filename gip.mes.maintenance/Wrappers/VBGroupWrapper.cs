using gip.core.datamodel;
using vd = gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.maintenance
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance role'}de{'Maintenance role'}", Global.ACKinds.TACSimpleClass)]
    public class VBGroupWrapper : ACObjectItem
    {
        public VBGroupWrapper()
            : base("")
        {

        }

        public vd.VBGroup VBGroup
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
