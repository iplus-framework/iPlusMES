using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance ACClassProperty'}de{'Maintenance ACClassProperty'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "VBiACClassProperty", "en{'ACClassProperty'}de{'ACClassProperty'}", Const.ContextDatabase + "\\" + gip.core.datamodel.ACClassProperty.ClassName, "", true)]
    [ACPropertyEntity(5, MaintACClass.ClassName, "en{'Maintenance rule'}de{'Wartungsregel'}", Const.ContextDatabase + "\\" + MaintACClass.ClassName, "", true)]
    [ACPropertyEntity(3, "MaxValue", "en{'Maximum value'}de{'Maximaler Wert'}", "", "", true)]
    [ACPropertyEntity(5, "IsActive","en{'Is Active'}de{'Ist Aktiv'}","","",true)]
    [ACPropertyEntity(6, "IsWarningActive", "en{'Warning'}de{'Warnung'}", "", "", true)]
    [ACPropertyEntity(7, "WarningValueDiff", "en{'Warning at'}de{'Warnung bei'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintACClassProperty.ClassName, "en{'Maintenance ACClassProperty'}de{'Maintenance ACClassProperty'}", typeof(MaintACClassProperty), MaintACClassProperty.ClassName, "", "")]
    public partial class MaintACClassProperty
    {
        public const string ClassName = "MaintACClassProperty";

        public static MaintACClassProperty NewACObject(DatabaseApp dbApp, IACObject parentObject)
        {
            var entity = new MaintACClassProperty();
            entity.MaintACClassPropertyID = Guid.NewGuid();
            if (parentObject is MaintACClass)
                entity.MaintACClass = ((MaintACClass)parentObject);
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        private ACValueItem _MaxValueShow;
        [ACPropertyInfo(999, "","en{'Maximum value'}de{'Maximaler Wert'}")]
        public ACValueItem MaxValueShow
        {
            get
            {
                if (_MaxValueShow != null)
                    return _MaxValueShow;
           
                else
                {
                    _MaxValueShow = new ACValueItem("MaxValue", null, VBiACClassProperty.ValueTypeACClass.FromIPlusContext<core.datamodel.ACClass>());
                    _MaxValueShow.ParentACObject = this;
                    _MaxValueShow.OnPropertyChangedNameWithACIdentifier = true;
                    _MaxValueShow.SetValueFromString(MaxValue);
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
        [ACPropertyInfo(999, "", "en{'Warning value difference'}de{'Warnungswertdifferenz'}")]
        public ACValueItem WarningValueShow
        {
            get
            {
                if (_WarningValueShow != null)
                    return _WarningValueShow;
                else
                {
                    _WarningValueShow = new ACValueItem("WarningValue", null, VBiACClassProperty.ValueTypeACClass.FromIPlusContext<core.datamodel.ACClass>());
                    _WarningValueShow.ParentACObject = this;
                    _WarningValueShow.OnPropertyChangedNameWithACIdentifier = true;
                    _WarningValueShow.SetValueFromString(WarningValueDiff);
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
        public new void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == "ACValueItem(MaxValue)\\Value")
            {
                string maxValue = _MaxValueShow.GetStringValue();
                if(MaxValue != maxValue)
                    MaxValue = maxValue;
            }
            else if(propertyName == "ACValueItem(WarningValue)\\Value")
            {
                string warningValue = _WarningValueShow.GetStringValue();
                if (WarningValueDiff != warningValue)
                    WarningValueDiff = warningValue;
            }
            else if (propertyName == nameof(VBiACClassPropertyID))
            {
                OnPropertyChanged("ACClassProperty");
            }
        }

        private gip.core.datamodel.ACClassProperty _ACClassProperty;
        [ACPropertyInfo(9999, "", "en{'Property'}de{'Eigenschaft'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClassProperty.ClassName)]
        public gip.core.datamodel.ACClassProperty ACClassProperty
        {
            get
            {
                if (this.VBiACClassPropertyID == null || this.VBiACClassPropertyID == Guid.Empty)
                    return null;
                if (_ACClassProperty != null)
                    return _ACClassProperty;
                if (this.VBiACClassProperty == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                    _ACClassProperty = dbApp.ContextIPlus.ACClassProperty.Where(c => c.ACClassPropertyID == this.VBiACClassPropertyID).FirstOrDefault();
                    return _ACClassProperty;
                }
                else
                {
                    _ACClassProperty = this.VBiACClassProperty.FromIPlusContext<gip.core.datamodel.ACClassProperty>();
                    return _ACClassProperty;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiACClassProperty == null)
                        return;
                    _ACClassProperty = null;
                    this.VBiACClassProperty = null;
                }
                else
                {
                    if (_ACClassProperty != null && value == _ACClassProperty)
                        return;
                    gip.mes.datamodel.ACClassProperty value2 = value.FromAppContext<gip.mes.datamodel.ACClassProperty>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiACClassPropertyID = value.ACClassPropertyID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _ACClassProperty = value;
                    if (value2 == this.VBiACClassProperty)
                        return;
                    this.VBiACClassProperty = value2;
                }
            }
        }

    }
}
