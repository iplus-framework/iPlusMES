using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Rule'}de{'Wartungsregel'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOMaintConfig")]
    [ACPropertyEntity(1, "VBiACClass", "en{'Object'}de{'Objekt'}", "", "", true)]
    [ACPropertyEntity(2, "MDMaintMode", "en{'Maintenance Mode'}de{'Wartungsmodus'}", Const.ContextDatabase + "\\MDMaintMode", "", true)]
    [ACPropertyEntity(3, "IsActive", ConstApp.IsActive, "", "", true)]
    [ACPropertyEntity(4, "MaintInterval", "en{'Maintenance Interval [days]'}de{'Wartungsintervall [Tage]'}", "", "", true)]
    [ACPropertyEntity(5, "LastMaintTerm", "en{'Last Maintenance on'}de{'Letzte Wartung am'}", "", "", true)]
    [ACPropertyEntity(6, "IsWarningActive", "en{'Maintenance Notification'}de{'Wartungsvorankündigung'}", "", "", true)]
    [ACPropertyEntity(7, "WarningDiff", "en{'Advance Notice Days'}de{'Vorankündigung Tage'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    public partial class MaintACClass
    {
        public const string ClassName = "MaintACClass";

        public static MaintACClass NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            var entity = new MaintACClass();
            entity.MaintACClassID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            dbApp.MaintACClass.Add(entity);
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        protected override void OnPropertyChanged(string property)
        {
            base.OnPropertyChanged(property);
            if (property == "MaintInterval" || property == "LastMaintTerm")
            {
                //if (MaintInterval != null)
                //    NextMaintTerm = LastMaintTerm + TimeSpan.FromDays(MaintInterval.Value);
                //else
                //    NextMaintTerm = null;
                OnPropertyChanged("NextMaintTerm");
            }
            else if (property == nameof(VBiACClassID))
            {
                OnPropertyChanged("FacilityACClass");
            }
            //else if (property == "MDMaintModeID")
            //{
            //    //SetConfigCM();
            //}
        }

        //public void SetConfigCM()
        //{
        //    if (MDMaintMode == null)
        //    {
        //        TimeConfigCM = Global.ControlModes.Collapsed;
        //        EventConfigCM = Global.ControlModes.Collapsed;
        //    }
        //    else
        //    {
        //        switch (MDMaintMode.MDMaintModeIndex)
        //        {
        //            case 1:
        //                TimeConfigCM = Global.ControlModes.Enabled;
        //                EventConfigCM = Global.ControlModes.Collapsed;
        //                break;
        //            case 2:
        //                TimeConfigCM = Global.ControlModes.Collapsed;
        //                EventConfigCM = Global.ControlModes.Enabled;
        //                break;
        //            case 3:
        //                TimeConfigCM = Global.ControlModes.Enabled;
        //                EventConfigCM = Global.ControlModes.Enabled;
        //                break;
        //        }
        //    }
        //}

        //private Global.ControlModes _TimeConfigCM;
        //[ACPropertyInfo(999)]
        //public Global.ControlModes TimeConfigCM
        //{
        //    get
        //    {
        //        return _TimeConfigCM;
        //    }
        //    set
        //    {
        //        _TimeConfigCM = value;
        //        OnPropertyChanged("TimeConfigCM");
        //    }
        //}

        //private Global.ControlModes _EventConfigCM;
        //[ACPropertyInfo(999)]
        //public Global.ControlModes EventConfigCM
        //{
        //    get
        //    {
        //        return _EventConfigCM;
        //    }
        //    set
        //    {
        //        _EventConfigCM = value;
        //        OnPropertyChanged("EventConfigCM");
        //    }
        //}

        //private DateTime? _NextMaintTerm;
        [ACPropertyInfo(999, "", "en{'Next Maintenance on'}de{'Nächste Wartung am'}")]
        public DateTime? NextMaintTerm
        {
            get
            {
                if (LastMaintTerm.HasValue && MaintInterval.HasValue)
                    return LastMaintTerm + TimeSpan.FromDays(MaintInterval.Value);
                return null;
            }
            set
            {
                //_NextMaintTerm = value;
                //OnPropertyChanged("NextMaintTerm");
            }
        }

        private string _ACClassACUrl;
        [ACPropertyInfo(999, "", "en{'Component URL'}de{'Component url'}")]
        public string ACClassACUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_ACClassACUrl) && this.ACClass != null)
                {
                    _ACClassACUrl = this.ACClass.GetACUrl();
                }
                return _ACClassACUrl;
            }
        }

        private IEnumerable<MaintACClassProperty> _LocalPropertyCache = null;
        public IEnumerable<MaintACClassProperty> LocalPropertyCache
        {
            get
            {
                return _LocalPropertyCache;
            }
        }

        public void CopyMaintACClassPropertiesToLocalCache()
        {
            _LocalPropertyCache = this.MaintACClassProperty_MaintACClass.ToArray();
        }

        #region VBIplus-Context
        private gip.core.datamodel.ACClass _ACClass;
        [ACPropertyInfo(9999, "", "en{'Maintenance objekt'}de{'Wartungsobjekt'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName)]
        public gip.core.datamodel.ACClass ACClass
        {
            get
            {
                if (this.VBiACClassID == null || this.VBiACClassID == Guid.Empty)
                    return null;
                if (_ACClass != null)
                    return _ACClass;
                DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                if (dbApp != null)
                    _ACClass = dbApp.ContextIPlus.GetACType(this.VBiACClassID);
                return _ACClass;
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiACClass == null)
                        return;
                    _ACClass = null;
                    this.VBiACClass = null;
                }
                else
                {
                    if (_ACClass != null && value == _ACClass)
                        return;
                    gip.mes.datamodel.ACClass value2 = value.FromAppContext<gip.mes.datamodel.ACClass>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiACClassID = value.ACClassID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _ACClass = value;
                    if (value2 == this.VBiACClass)
                        return;
                    this.VBiACClass = value2;
                }
            }
        }

        public gip.core.datamodel.ACClass GetACClass(Database db)
        {
            if (this.VBiACClassID == null || this.VBiACClassID == Guid.Empty)
                return null;
            if (this.VBiACClass == null)
            {

                using (ACMonitor.Lock(db.QueryLock_1X000))
                {
                    return db.ACClass.Where(c => c.ACClassID == this.VBiACClassID).FirstOrDefault();
                }
            }
            else
            {

                using (ACMonitor.Lock(db.QueryLock_1X000))
                {
                    return this.VBiACClass.FromIPlusContext<gip.core.datamodel.ACClass>(db);
                }
            }
        }
        #endregion

    }
}
