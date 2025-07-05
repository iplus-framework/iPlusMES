// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TrackingAndTracingPoint'}de{'TrackingAndTracingPoint'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class TrackingAndTracingPoint : IVBDataCheckbox, IACContainerWithItemsT<TrackingAndTracingPoint, IACObjectEntity>, IACObject, ICloneable
    {
        #region ctor's

        public TrackingAndTracingPoint(TrackingAndTracingResult result, IACObjectEntity item)
        {
            ParentResultObject = result;
            Related = item;
        }

        public TrackingAndTracingPoint(TrackingAndTracingResult result, TandTPoint basicPoint)
        {
            ParentResultObject = result;
            Related = basicPoint.Item;
            foreach (var item in basicPoint.Children)
            {
                TrackingAndTracingPoint childPoint = new TrackingAndTracingPoint(result, item);
                childPoint.ParentACObject = this;
                AddWithCheck(childPoint, item.ID);
            }
            IconName = basicPoint.IconName;
            IsExternLot = (Related as FacilityLot) != null && !string.IsNullOrEmpty((Related as FacilityLot).ExternLotNo);
        }

        #endregion

        #region IVBDataCheckbox
        [ACPropertyInfo(999)]
        public string DataContentCheckBox
        {
            get { return "IsChecked"; }
        }

        [ACPropertyInfo(999)]
        public bool IsChecked { get; set; }

        [ACPropertyInfo(999)]
        public bool IsEnabled { get; set; }
        #endregion

        #region IACContainerWithItems


        private List<TrackingAndTracingPoint> _Items;
        /// <summary>Container-Childs</summary>
        /// <value>Container-Childs</value>
        [ACPropertyInfo(999)]
        public IEnumerable<IACContainerWithItems> Items
        {
            get
            {
                return ItemsT;
            }
        }

        /// <summary>Container-Childs</summary>
        /// <value>Container-Childs</value>
        public IEnumerable<TrackingAndTracingPoint> ItemsT
        {
            get
            {
                if (_Items == null)
                    _Items = new List<TrackingAndTracingPoint>();
                return _Items;
            }
        }

        /// <summary>Visible Container-Childs</summary>
        /// <value>Visible Container-Childs</value>
        public IEnumerable<TrackingAndTracingPoint> VisibleItemsT
        {
            get
            {
                return ItemsT;
            }
        }

        /// <summary>Gets the parent container.</summary>
        /// <value>The parent container.</value>
        public IACContainerWithItems ParentContainer
        {
            get
            {
                return ParentACObject as IACContainerWithItems;
            }
        }

        /// <summary>Gets the parent container T.</summary>
        /// <value>The parent container T.</value>
        public TrackingAndTracingPoint ParentContainerT
        {
            get
            {
                return ParentACObject as TrackingAndTracingPoint;
            }
        }

        /// <summary>Gets the root container T.</summary>
        /// <value>The root container T.</value>
        public TrackingAndTracingPoint RootContainerT
        {
            get
            {
                return RootContainer as TrackingAndTracingPoint;
            }
        }


        /// <summary>Gets the root container.</summary>
        /// <value>The root container.</value>
        public IACContainerWithItems RootContainer
        {
            get
            {
                if (ParentContainer == null)
                    return this;
                return ParentContainer.RootContainer;
            }
        }

        /// <summary>Adds the specified child-container</summary>
        /// <param name="child">The child-container</param>
        public void Add(IACContainerWithItems child)
        {
            Add(child as TrackingAndTracingPoint);
        }

        public bool AddWithCheck(TrackingAndTracingPoint child, Guid id)
        {
            if (ParentResultObject.ProcessedItems.Any(x => x == id))
                return false;
            child._ParentACObject = this;
            if (_Items == null)
                _Items = new List<TrackingAndTracingPoint>();
            _Items.Add(child);
            ParentResultObject.ProcessedItems.Add(id);
            return true;
        }


        /// <summary>Adds the specified child-container</summary>
        /// <param name="child">The child-container</param>
        public void Add(TrackingAndTracingPoint child)
        {
            if (child == null)
                return;
            child._ParentACObject = this;
            child.ParentResultObject = this.ParentResultObject;
            if (_Items == null)
                _Items = new List<TrackingAndTracingPoint>();
            _Items.Add(child);
        }

        /// <summary>Removes the specified child-container</summary>
        /// <param name="child">The child-container</param>
        /// <returns>true if removed</returns>
        public bool Remove(TrackingAndTracingPoint child)
        {
            if (_Items != null || child == null)
                return false;
            return _Items.Remove(child);
        }


        /// <summary>Removes the specified child-container</summary>
        /// <param name="child">The child-container</param>
        /// <returns>true if removed</returns>
        public bool Remove(IACContainerWithItems child)
        {
            return Remove(child as TrackingAndTracingPoint);
        }

        /// <summary>Gets or sets the encapsulated value as a boxed type</summary>
        /// <value>The boxed value.</value>
        public object Value
        {
            get
            {
                return Related;
            }
            set
            {
                if (value is IACObjectEntity)
                    Related = value as IACObjectEntity;
            }
        }

        public IACObjectEntity ValueT
        {
            get
            {
                return Related;
            }
            set
            {
                Related = value;
            }
        }

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                _IsVisible = value;
            }
        }

        /// <summary>
        /// Metadata (iPlus-Type) of the Value-Property. ATTENTION: ACClass is a EF-Object. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>Metadata (iPlus-Type) of the Value-Property as ACClass</value>
        public core.datamodel.ACClass ValueTypeACClass
        {
            get
            {
                IACObject iObject = Value as IACObject;
                return iObject == null ? null : iObject.ACType as core.datamodel.ACClass;
            }
        }

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        [ACPropertyInfo(999)]
        public string ACIdentifier
        {
            get { return ""; }
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(999)]
        public string ACCaption
        {
            get
            {
                return Related.ACCaption;
            }
        }

        #endregion

        #region IACObject
        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType
        {
            get { return this.ReflectACType(); }
        }

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList
        {
            get
            {
                return ItemsT;
            }
        }

        /// <summary>
        /// The ACUrlCommand is a universal method that can be used to query the existence of an instance via a string (ACUrl) to:
        /// 1. get references to components,
        /// 2. query property values,
        /// 3. execute method calls,
        /// 4. start and stop Components,
        /// 5. and send messages to other components.
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>Result if a property was accessed or a method was invoked. Void-Methods returns null.</returns>
        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return false;
        }

        private IACObject _ParentACObject;
        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get { return _ParentACObject; }
            set { _ParentACObject = value; }
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        public string GetACUrl(IACObject rootACObject = null)
        {
            return ACIdentifier;
        }

        /// <summary>
        /// Method that returns a source and path for WPF-Bindings by passing a ACUrl.
        /// </summary>
        /// <param name="acUrl">ACUrl of the Component, Property or Method</param>
        /// <param name="acTypeInfo">Reference to the iPlus-Type (ACClass)</param>
        /// <param name="source">The Source for WPF-Databinding</param>
        /// <param name="path">Relative path from the returned source for WPF-Databinding</param>
        /// <param name="rightControlMode">Information about access rights for the requested object</param>
        /// <returns><c>true</c> if binding could resolved for the passed ACUrl<c>false</c> otherwise</returns>
        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            throw new NotImplementedException();
        }

        public bool ACUrlTypeInfo(string acUrl, ref ACUrlTypeInfo acUrlTypeInfo)
        {
            return this.ReflectACUrlTypeInfo(acUrl, ref acUrlTypeInfo);
        }
        #endregion

        #region Properties

        [ACPropertyInfo(999)]
        public IACObjectEntity Related { get; set; }

        public TrackingAndTracingResult ParentResultObject { get; set; }

        public string IconName { get; set; }

        [ACPropertyInfo(999)]
        public string IconDesign
        {
            get
            {
                gip.core.datamodel.ACClassDesign acClassDesign = ACType.GetDesign(this, Global.ACUsages.DUMain, Global.ACKinds.DSDesignLayout, IconName);
                string layoutXAML = null;
                if (acClassDesign != null && acClassDesign.ACIdentifier != "UnknowMainlayout")
                {
                    layoutXAML = acClassDesign.XMLDesign;
                }
                else
                {
                    layoutXAML = "<vb:VBDockPanel><vb:VBTextBox ACCaption=\"Unknown:\" Text=\"" + IconName + "\"></vb:VBTextBox></vb:VBDockPanel>";
                }
                return layoutXAML;
            }
        }

        [ACPropertyInfo(999)]
        public bool IsExternLot { get; set; }


        #endregion

        #region Helper methods

        public bool IsSatisfyFilter(List<string> filter)
        {
            return filter.Contains(this.Related.GetType().Name);
        }

        /// <summary>
        ///  Apply filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TrackingAndTracingPoint ApplyFilter(TrackingAndTracingPoint rootPoint, List<string> filter)
        {
            TrackingAndTracingPoint point = this;
            if (this.ParentACObject != null)
            {
                if (!IsSatisfyFilter(filter))
                {
                    Items.ToList().ForEach(x => (x as TrackingAndTracingPoint)._ParentACObject = rootPoint);
                    (ParentACObject as TrackingAndTracingPoint)._Items.Remove(this);
                }
                else
                {
                    if (!rootPoint.Items.Contains(this))
                        rootPoint.Add(this);
                    rootPoint = this;
                }
            }
            else
            {
                rootPoint = this;
            }
            Items.ToList().ForEach(x => (x as TrackingAndTracingPoint).ApplyFilter(rootPoint, filter));
            return point;
        }

        private void SetParentObject(IACObject parent)
        {
            _ParentACObject = parent;
        }

        /// <summary>
        /// Šro
        /// </summary>
        internal void ProcessStatistics()
        {
            ParentResultObject.ProcessStatistics(this);
            Items.ToList().ForEach(x => (x as TrackingAndTracingPoint).ProcessStatistics());
        }

        public override string ToString()
        {
            return Related.ToString();
        }

        /// <summary>
        ///  HTML tree presentation
        /// </summary>
        /// <returns></returns>
        public string ToHTMLString()
        {
            string allChildrenString = "";
            if (Items != null && _Items.Any())
            {
                foreach (IACObject point in Items)
                {
                    TrackingAndTracingPoint childPoint = point as TrackingAndTracingPoint;
                    allChildrenString += childPoint.ToHTMLString();
                }
            }
            if (!string.IsNullOrEmpty(allChildrenString))
            {
                allChildrenString = "<ul>" + allChildrenString + "</ul>";
            }
            return string.Format("<li>[{0}]{1} {2}</li>", Related.GetType().Name, ToString(), allChildrenString);
        }

        #endregion

        #region IClonable

        public object Clone()
        {
            TrackingAndTracingPoint pointCloned = this.MemberwiseClone() as TrackingAndTracingPoint;
            if (_Items != null)
            {
                List<TrackingAndTracingPoint> itemsList = _Items.ToList();
                _Items.Clear();
                itemsList = itemsList.Select(x => x.MemberwiseClone() as TrackingAndTracingPoint).ToList();
                _Items = itemsList;
            }
            return pointCloned;
        }
        #endregion
    }
}
