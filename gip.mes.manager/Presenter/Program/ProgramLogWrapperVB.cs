using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.manager;

namespace gip.mes.manager
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'ProgramLogWrapper'}de{'ProgramLogWrapper'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, true)]
    public class ProgramLogWrapperVB : ProgramLogWrapper
    {
        public ProgramLogWrapperVB() : base()
        {

        }

        private static DatabaseApp _DatbaseApp;
        static DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatbaseApp == null)
                    _DatbaseApp = new datamodel.DatabaseApp();
                return _DatbaseApp;
            }
        }

        private datamodel.ACProgramLog RootProgramLog
        {
            get
            {
                datamodel.ACProgramLog temp = DatabaseApp.ACProgramLog.FirstOrDefault(c => c.ACProgramLogID == ACProgramLog.ACProgramLogID);
                if (temp == null)
                    return null;
                while (temp.ParentACProgramLogID != null)
                    temp = DatabaseApp.ACProgramLog.FirstOrDefault(c => c.ACProgramLogID == temp.ParentACProgramLogID);
                return temp;
            }
        }

        [ACPropertyInfo(999,"","en{'ProdOrderNo'}de{'ProdOrderNo'}")]
        public string ProdOrderInfo
        {
            get 
            {
                if (RootProgramLog == null)
                    return "";
                OrderLog orderlog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == RootProgramLog.ACProgramLogID);
                if (orderlog == null)
                    return "";
                if (orderlog.ProdOrderPartslistPos != null)
                    return orderlog.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                else if (orderlog.ProdOrderPartslistPosRelation != null)
                    return orderlog.ProdOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                else if (orderlog.PickingPos != null)
                    return orderlog.PickingPos.Picking.PickingNo;
                else if (orderlog.FacilityBooking != null)
                    return orderlog.FacilityBooking.FacilityBookingNo;
                else if (orderlog.DeliveryNotePos != null)
                    return orderlog.DeliveryNotePos.DeliveryNote.DeliveryNoteNo;
                return "";
            }
            set
            { 
            }
        }
    }
}
