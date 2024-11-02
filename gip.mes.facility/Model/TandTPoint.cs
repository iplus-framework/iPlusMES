// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{

    /// <summary>
    /// @aagincic
    /// Basic class for tracking and tracing
    /// Contains only element they are relevant in data system
    /// </summary>
    public class TandTPoint
    {
        public TandTPoint(DatabaseApp dbApp, IACObjectEntity item, TandTFilter filter)
        {
            Item = item;
            ID = (Guid)((VBEntityObject)item).EntityKey.EntityKeyValues[0].Value;
            Filter = filter;
        }

        public TandTPoint(DatabaseApp dbApp, TandTResult rs, TandTPoint parentPoint, IACObjectEntity item, TandTFilter filter)
            : this(dbApp, item, filter)
        {
            bool isFunctionSatisfied = true;
            if (filter.FilterFunctions != null)
            {

                foreach (var func in filter.FilterFunctions)
                {
                    isFunctionSatisfied = isFunctionSatisfied && func(this.Item, this.Filter);
                }
            }
            if (isFunctionSatisfied)
            {
                if (rs.Add(this))
                {
                    Parent = parentPoint;
                    if (parentPoint != null)
                        parentPoint.AddChild(this);
                    ProcessRelated(dbApp, rs, item);
                }
                IconName = string.Format(@"Icon_{0}", item.GetType().Name);
                if ((item as ProdOrderPartslistPos) != null && (item as ProdOrderPartslistPos).ParentProdOrderPartslistPosID != null)
                    IconName = "Icon_ProdOrderPartslistPosBatch";
            }
        }

        public virtual void ProcessRelated(DatabaseApp dbApp, TandTResult rs, IACObjectEntity item)
        {

        }

        /// <summary>
        /// Usage for filter in navigation
        /// </summary>
        public TandTFilter Filter { get; set; }


        public string IconName { get; set; }


        public Guid ID { get; set; }

        public TandTPoint Parent { get; set; }

        public IACObjectEntity Item { get; set; }

        private List<TandTPoint> children;
        public List<TandTPoint> Children
        {
            get
            {
                if (children == null)
                    children = new List<TandTPoint>();
                return children;
            }
        }

        public void AddChild(TandTPoint item)
        {
            Children.Add(item);
        }

        /// <summary>
        ///  HTML tree presentation
        /// </summary>
        /// <returns></returns>
        public string ToHTMLString()
        {
            string allChildrenString = "";
            if (Children != null && Children.Any())
            {
                foreach (TandTPoint childPoint in Children)
                {
                    allChildrenString += childPoint.ToHTMLString();
                }
            }
            if (!string.IsNullOrEmpty(allChildrenString))
            {
                allChildrenString = "<ul>" + allChildrenString + "</ul>";
            }
            return string.Format("<li>{0} {1}</li>", Item.ACCaption, allChildrenString);
        }
    }
}
