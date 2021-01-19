using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    public class GraphPoint<T> where T : IACObject
    {

        #region Properties

        public MDTrackingStartItemTypeEnum ItemType { get; set; }
        public MDBookingDirectionEnum Direcion { get; set; }

        public Guid ItemID { get; set; }
        public string ItemNo { get; set; }
        public T Item { get; set; }

        public Guid? MixPointID { get; set; }

        public bool IsVirtual { get; set; }
        public bool IsPassed { get; set; }

        #endregion

        #region overrides 

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (obj != null && obj is GraphPoint<T>)
            {
                GraphPoint<T> second = obj as GraphPoint<T>;
                isEqual =
                    ItemType == second.ItemType &&
                    Direcion == second.Direcion &&
                    ItemID == second.ItemID &&
                    ItemNo == second.ItemNo;
            }
            return isEqual;
        }

        #endregion


        public TandTPointPresenter ProducedComponent { get; set; }

        #region overrides
        public override string ToString()
        {
            string isVirtualStr = "";
            if(IsVirtual)
                isVirtualStr = "[IsVirtual]";
            return string.Format(@"{0} - {1} [{2}]{3}", ItemType.ToString(), ItemNo, Item.ToString(), isVirtualStr);
        }
        #endregion

    }
}
