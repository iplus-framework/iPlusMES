using System;
using System.Collections.Generic;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_DeliveryNotePos : DoBackward_DeliveryNotePos
    {

        #region ctor's

        public DoForward_DeliveryNotePos(DatabaseApp databaseApp, TandTv2Result result, DeliveryNotePos item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region overriden methodes
        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            return new List<IDoItem>() { new DoForward_DeliveryNote(databaseApp, result, Item.DeliveryNote, jobFilter) };
        }


        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> doItems = new List<IDoItem>();
            if (Item.InOrderPosID != null)
                doItems.Add(new DoForward_InOrderPos(databaseApp, result, Item.InOrderPos, jobFilter));
            return doItems;
        }

        #endregion

    }
}
