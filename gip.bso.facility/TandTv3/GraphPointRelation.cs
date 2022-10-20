using gip.core.datamodel;
using System;

namespace gip.bso.facility
{
    public class GraphPointRelation<T> where T : IACObject
    {
        #region ctor's

        public GraphPointRelation(GraphPoint<T> source, GraphPoint<T> target, GraphPointRelationTypeEnum relationSource)
        {
            Source = source;
            Target = target;
            RelationType = relationSource;
        }

        #endregion

        #region Properties

        public GraphPoint<T> Source { get; set; }
        public GraphPoint<T> Target { get; set; }

        public bool IsPassed { get; set; }


        public GraphPointRelationTypeEnum RelationType { get; set; }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return RelationType.GetHashCode() ^ Source.ItemID.GetHashCode() ^ Target.ItemID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (obj != null && obj is GraphPointRelation<T>)
            {
                GraphPointRelation<T> graphPointRelation = obj as GraphPointRelation<T>;
                isEqual = Source.ItemID == graphPointRelation.Source.ItemID && Target.ItemID == graphPointRelation.Target.ItemID && RelationType == graphPointRelation.RelationType;
            }
            return isEqual;
        }

        public override string ToString()
        {
            string sourceVirtual = "";
            if (Source.IsVirtual)
                sourceVirtual = @"[IsVirutal]";
            string targetVirtual = "";
            if (Target.IsVirtual)
                targetVirtual = @"[IsVirutal]";
            return string.Format(@"[{0}] {1} - {2} {3} => {4} - {5} {6}", RelationType, Source.ItemType, Source.ItemNo, sourceVirtual, Target.ItemType, Target.ItemNo, targetVirtual);
        }

        #endregion
    }
}
