using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace gip.mes.facility
{
    /// <summary>
    /// This object is used to make constraints in search and structure navigation
    /// </summary>
    [DataObject]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTFilter'}de{'TandTFilter'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class TandTFilter
    {

        [DataObjectField(false)]
        [ACPropertyInfo(9999, "Deposit", "en{'From'}de{'von'}")]
        public DateTime? StartTime { get; set; }


        [DataObjectField(false)]
        [ACPropertyInfo(9999, "Deposit", "en{'To'}de{'bis'}")]
        public DateTime? EndTime { get; set; }

        private List<Func<IACObjectEntity, TandTFilter, bool>> _FilterFunctions;
        public List<Func<IACObjectEntity, TandTFilter, bool>> FilterFunctions
        {
            get
            {
                if (_FilterFunctions == null)
                    _FilterFunctions = new List<Func<IACObjectEntity, TandTFilter, bool>>();
                return _FilterFunctions;
            }
        }

    }
}

