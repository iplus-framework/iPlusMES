using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.facility
{
    public class GraphPointResult<T> where T : IACObject
    {

        #region ctor's

        public GraphPointResult()
        {
            Points = new List<GraphPoint<T>>();
            Relations = new List<GraphPointRelation<T>>();
            Msgs = new List<Msg>();
        }

        #endregion

        #region Properties
        public List<GraphPoint<T>> Points { get; set; }

        public List<GraphPointRelation<T>> Relations { get; set; }

        public List<Msg> Msgs { get; set; }

        #endregion

        #region Method

        public GraphPoint<T> FactoryPoint(Guid itemID, string itemNo, MDTrackingStartItemTypeEnum itemType, MDBookingDirectionEnum direction, Guid? mixPointID, T item)
        {
            GraphPoint<T> point = GetPoint(itemID, itemType, direction, mixPointID);
            if (point == null)
            {
                point = new GraphPoint<T>()
                {
                    Direcion = direction,
                    Item = item,
                    ItemID = itemID,
                    ItemNo = itemNo,
                    ItemType = itemType,
                    MixPointID = mixPointID
                };
                if (!Points.Contains(point))
                    Points.Add(point);
            }

            return point;
        }

        public GraphPoint<T> GetPoint(Guid itemID, MDTrackingStartItemTypeEnum itemType, MDBookingDirectionEnum direction, Guid? mixPointID)
        {
            return Points.FirstOrDefault(c =>
                        c.ItemID == itemID &&
                        c.ItemType == itemType &&
                        (c.MixPointID ?? Guid.Empty) == (mixPointID ?? Guid.Empty) &&
                        c.Direcion == direction);
        }

        public void AddRelation(GraphPoint<T> source, GraphPoint<T> target, GraphPointRelationTypeEnum relationType)
        {
            GraphPointRelation<T> relation = new GraphPointRelation<T>(source,target, relationType);
            if (!Relations.Contains(relation))
                Relations.Add(relation);
        }

        public bool ContainsRelation(GraphPoint<T> source, GraphPoint<T> target, GraphPointRelationTypeEnum relationType)
        {
            GraphPointRelation<T> relation = new GraphPointRelation<T>(source, target, relationType);
            return Relations.Contains(relation);
        }

        #endregion
    }
}
