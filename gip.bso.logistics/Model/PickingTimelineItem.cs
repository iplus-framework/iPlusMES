using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.logistics
{
    public class PickingTimelineItem : IACObject
    {
        #region c'tors

        public PickingTimelineItem()
        {

        }

        #endregion

        #region Properties

        [ACPropertyInfo(100)]
        public int DisplayOrder { get; set; }

        internal List<PickingTimelineItem> _Items;

        /// <summary>
        /// Gets the child items.
        /// </summary>
        public List<PickingTimelineItem> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new List<PickingTimelineItem>();
                return _Items;
            }
        }

        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        [ACPropertyInfo(101, "", "en{'Start date'}de{'Startzeitpunkt'}")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the EndDate.
        /// </summary>
        [ACPropertyInfo(102, "", "en{'End date'}de{'Endzeitpunkt'}")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets the Duration. (EndDate - StartDate)
        /// </summary>
        [ACPropertyInfo(103, "", "en{'Duration'}de{'Dauer'}")]
        public TimeSpan Duration
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue)
                    return EndDate.Value - StartDate.Value;
                return new TimeSpan();
            }
        }

        /// <summary>
        /// Gets or sets the duration of all child items.
        /// </summary>
        [ACPropertyInfo(104, "", "en{'Total Duration'}de{'Gesamtdauer'}")]
        public TimeSpan TotalDuration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the duration of all child items.
        /// </summary>
        [ACPropertyInfo(105, "", "en{'Order Number'}de{'Auftragsnummer'}")]
        public string ProgramNo
        {
            get;
            set;
        }

        [ACPropertyInfo(106, "", "en{'Required quantity (UOM)'}de{'Bedarfsmenge (UOM)'}")]
        public double TargetQuantityUOM
        {
            get;
            set;
        }

        public PickingTimelineItemType TimelineItemType
        {
            get;
            set;
        }

        #endregion

        #region Enum

        public enum PickingTimelineItemType : short
        {
            TimelineItem = 10,
            ContainerItem = 20
        }

        #endregion

        #region IACObject members

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        public string ACIdentifier { get; set; }

        private string _ACCaption;

        ///// <summary>
        ///// The PropertyChanged event.
        ///// </summary>
        //public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(999)]
        public string ACCaption
        {
            get => _ACCaption;
            set => _ACCaption = value;
        }

        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType => this.ReflectACType();

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get;
            set;
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
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        public bool ACUrlTypeInfo(string acUrl, ref ACUrlTypeInfo acUrlTypeInfo)
        {
            return this.ReflectACUrlTypeInfo(acUrl, ref acUrlTypeInfo);
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
        /// Determins is enabled ACUrlCommand.
        /// </summary>
        /// <param name="acUrl">The acUrl.</param>
        /// <param name="acParameter">The acParameter.</param>
        /// <returns>True if is enabled, otherwise returns false.</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
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
        #endregion
    }
}
