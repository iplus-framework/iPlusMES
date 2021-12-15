using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Text;

namespace gip.mes.datamodel
{
    //[DataContract]
    //[ACSerializeableInfo]
#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Posting behaviour'}de{'Buchungsverhalten'}", Global.ACKinds.TACEnum, QRYConfig = "gip.mes.datamodel.ACValueListPostingBehaviourEnum")]
#endif
    public enum PostingBehaviourEnum : short
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        DoNothing = 0,

        /// <summary>
        /// Blocks the new added quants after a relocation posting
        /// This is used for pickig orders when material is relocated to another store and the receiver has to acknowledg the receipt of goods.
        /// Otherwise the production can't use this material until the material is blocked.
        /// </summary>
        BlockOnRelocation = 1,

        /// <summary>
        /// Sets the stock to zero after a relocation posting. 
        /// This is used for scenarios where a other System is master of a storage location
        /// </summary>
        ZeroStockOnRelocation = 2
    }
}
