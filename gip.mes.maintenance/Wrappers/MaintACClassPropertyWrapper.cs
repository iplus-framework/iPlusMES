using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using vd = gip.mes.datamodel;
using System.ComponentModel;

namespace gip.mes.maintenance
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance ACClassProperty'}de{'Maintenance ACClassProperty'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, false, true)]
    [ACQueryInfo(Const.PackName_VarioAutomation, "MaintACClassProperty", "", typeof(MaintACClassPropertyWrapper), "en{'Maintenance ACClassProperty'}de{'Maintenance ACClassProperty'}", "", "")]
    public class MaintACClassPropertyWrapper : IACObject, INotifyPropertyChanged
    {
        [ACPropertyInfo(999)]
        public ACClassProperty ACClassProperty
        {
            get;
            set;
        }

        [ACPropertyInfo(999)]
        public vd.MaintACClassProperty MaintACClassProperty
        {
            get;
            set;
        }

        private ACValueItem _MaxValueShow;
        [ACPropertyInfo(999, "", "en{'Maximum value'}de{'Maximum value'}")]
        public ACValueItem MaxValueShow
        {
            get
            {
                if (_MaxValueShow != null)
                    return _MaxValueShow;

                else
                {

                    using (ACMonitor.Lock(Database.GlobalDatabase.QueryLock_1X000))
                        _MaxValueShow = new ACValueItem("MaxValue", null, Database.GlobalDatabase.ACClass.FirstOrDefault(c => c.ACClassID == MaintACClassProperty.VBiACClassProperty.ValueTypeACClassID));
                    _MaxValueShow.ParentACObject = this;
                    _MaxValueShow.OnPropertyChangedNameWithACIdentifier = true;
                    _MaxValueShow.SetValueFromString(MaintACClassProperty.MaxValue);
                }
                return _MaxValueShow;
            }
            set
            {
                _MaxValueShow = value;
                OnPropertyChanged("MaxValueShow");
            }
        }

        private ACValueItem _WarningValueShow;
        [ACPropertyInfo(999, "", "en{'Warning value at'}de{'Warning value at'}")]
        public ACValueItem WarningValueShow
        {
            get
            {
                if (_WarningValueShow != null)
                    return _WarningValueShow;
                else
                {

                    using (ACMonitor.Lock(Database.GlobalDatabase.QueryLock_1X000))
                        _WarningValueShow = new ACValueItem("WarningValue", null, Database.GlobalDatabase.ACClass.FirstOrDefault(c => c.ACClassID == MaintACClassProperty.VBiACClassProperty.ValueTypeACClassID));
                    _WarningValueShow.ParentACObject = this;
                    _WarningValueShow.OnPropertyChangedNameWithACIdentifier = true;
                    _WarningValueShow.SetValueFromString(MaintACClassProperty.WarningValueDiff);
                }
                return _WarningValueShow;
            }
            set
            {
                _WarningValueShow = value;
                OnPropertyChanged("WarningValueShow");
            }
        }

        [ACMethodInfo("", "", 999)]
        public void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "ACValueItem(MaxValue)\\Value")
            {
                string maxValue = _MaxValueShow.GetStringValue();
                if (MaintACClassProperty.MaxValue != maxValue)
                    MaintACClassProperty.MaxValue = maxValue;
            }
            else if (propertyName == "ACValueItem(WarningValue)\\Value")
            {
                string warningValue = _WarningValueShow.GetStringValue();
                if (MaintACClassProperty.WarningValueDiff != warningValue)
                    MaintACClassProperty.WarningValueDiff = warningValue;
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region IACObject

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        public string ACIdentifier
        {
            get { return this.ReflectGetACIdentifier(); }
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(999)]
        public string ACCaption
        {
            get { return this.ACClassProperty.ACCaption; }
        }

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
            get { return this.ReflectGetACContentList(); }
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
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }

        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get { return null; }
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
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

#endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
