using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Machine- and containerselection'}de{'Maschinen- und Siloauswahl'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, false, true)]
    public abstract class VBBSOModulesSelector : ACBSOvb
    {
        #region c´tors

        public VBBSOModulesSelector(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            this.ModulesList = new BindingList<POPartslistPosReservation>();
            this.TargetsList = new BindingList<POPartslistPosReservation>();
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            ParentACComponent.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ParentACComponent_PropertyChanged);
            if (!base.ACInit(startChildMode))
                return false;
            _RoutingService = ACRoutingService.ACRefToServiceInstance(this);

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            TargetsList = null;
            ModulesList = null;
            this._Info = null;
            ParentACComponent.PropertyChanged -= ParentACComponent_PropertyChanged;
            this._ModulesList = null;
            this._SelectedModule = null;
            this._SelectedTarget = null;
            this._TargetsList = null;

            ACRoutingService.DetachACRefFromServiceInstance(this, _RoutingService);
            _RoutingService = null;

            return base.ACDeInit(deleteACClassTask);
        }

        abstract protected void ParentACComponent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e);

        #endregion

        #region BSO->ACProperty
        protected ACRef<ACComponent> _RoutingService = null;
        public ACComponent RoutingService
        {
            get
            {
                if (_RoutingService == null)
                    return null;
                return _RoutingService.ValueT;
            }
        }

        public bool IsRoutingServiceAvailable
        {
            get
            {
                return RoutingService != null && RoutingService.ConnectionState != ACObjectConnectionState.DisConnected;
            }
        }

        protected string _Info;
        [ACPropertyInfo(500, "", "en{'Info'}de{'Info'}")]
        public string Info
        {
            get
            {
                return _Info;
            }
            set
            {
                _Info = value;
                OnPropertyChanged("Info");
            }
        }

        protected bool _ShowSelectedCells = false;
        [ACPropertyInfo(501, "ShowSelectedCells", "en{'Show selected targets only'}de{'Nur ausgewählte Ziele anzeigen'}")]
        public bool ShowSelectedCells
        {
            get
            {
                return _ShowSelectedCells;
            }
            set
            {
                if(_ShowSelectedCells != value)
                {
                    _ShowSelectedCells = value;
                    OnPropertyChanged("ShowSelectedCells");
                    RefreshTargets();
                }
            }
        }

        protected bool _ShowEnabledCells = false;
        [ACPropertyInfo(502, "ShowEnabledCells", "en{'Show released targets only'}de{'Nur freie Ziele anzeigen'}")]
        public bool ShowEnabledCells
        {
            get
            {
                return _ShowEnabledCells;
            }
            set
            {
                if(_ShowEnabledCells != value)
                {
                    _ShowEnabledCells = value;
                    OnPropertyChanged("ShowEnabledCells");
                    RefreshTargets();
                }
            }
        }

        protected bool _ShowSameMaterialCells = false;
        [ACPropertyInfo(503, "ShowSameMaterialCells", "en{'Show with same Material'}de{'Mit gleichem Material'}")]
        public bool ShowSameMaterialCells
        {
            get
            {
                return _ShowSameMaterialCells;
            }
            set
            {
                if (_ShowSameMaterialCells != value)
                {
                    _ShowSameMaterialCells = value;
                    OnPropertyChanged("ShowSameMaterialCells");
                    RefreshTargets();
                }
            }
        }

        protected bool _ShowCellsInRoute = true;
        [ACPropertyInfo(504, "ShowCellsInRoute", "en{'Allowed instances'}de{'Erlaubte Instanzen'}")]
        public bool ShowCellsInRoute
        {
            get
            {
                return _ShowCellsInRoute;
            }
            set
            {
                if (_ShowCellsInRoute != value)
                {
                    _ShowCellsInRoute = value;
                    OnPropertyChanged("ShowCellsInRoute");
                    RefreshTargets();
                }
            }
        }


        //ShowCellsInRoute

        public abstract bool IsModuleSelDisabled
        {
            get;
        }


        protected POPartslistPosReservation _SelectedModule;
        [ACPropertySelected(505, "Modules")]
        public POPartslistPosReservation SelectedModule
        {
            get
            {
                return _SelectedModule;
            }
            set
            {
                bool changed = _SelectedModule != value;
                _SelectedModule = value;
                if (changed)
                {
                    RefreshTargets();
                    OnPropertyChanged("SelectedModule");
                }
            }
        }

        protected BindingList<POPartslistPosReservation> _ModulesList;
        [ACPropertyList(506, "Modules")]
        public BindingList<POPartslistPosReservation> ModulesList
        {
            get
            {
                return _ModulesList;
            }
            protected set
            {
                if (_ModulesList != null)
                    _ModulesList.ListChanged -= _Modules_ListChanged;
                _ModulesList = value;
                if (_ModulesList != null)
                    _ModulesList.ListChanged += _Modules_ListChanged;
                OnPropertyChanged("ModulesList");
            }
        }

        protected virtual void _Modules_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.PropertyDescriptor.Name == "IsChecked")
            {
                RefreshTargets();
            }
        }

        protected POPartslistPosReservation _SelectedTarget;
        [ACPropertySelected(507, "Targets")]
        public POPartslistPosReservation SelectedTarget
        {
            get
            {
                return _SelectedTarget;
            }
            set
            {
                if (_SelectedTarget != null)
                    _SelectedTarget.PropertyChanged -= _SelectedTarget_PropertyChanged;
                bool changed = _SelectedTarget != value;
                _SelectedTarget = value;
                if (_SelectedTarget != null)
                    _SelectedTarget.PropertyChanged += _SelectedTarget_PropertyChanged;
                if (changed)
                {
                    OnPropertyChanged("SelectedTarget");
                }
            }
        }


        protected BindingList<POPartslistPosReservation> _TargetsList;
        [ACPropertyList(508, "Targets")]
        public BindingList<POPartslistPosReservation> TargetsList
        {
            get
            {
                return _TargetsList;
            }
            set
            {
                if (_TargetsList != null)
                    _TargetsList.ListChanged -= _Targets_ListChanged;
                _TargetsList = value;
                if (_TargetsList != null)
                    _TargetsList.ListChanged += _Targets_ListChanged;
                OnPropertyChanged();
            }
        }

        protected virtual void _Targets_ListChanged(object sender, ListChangedEventArgs e)
        {
        }

        protected virtual void _SelectedTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }


        #region AppManager Selection
        protected IACComponent _SelectedAppManager;
        [ACPropertySelected(509, "AppManagers")]
        public IACComponent SelectedAppManager
        {
            get
            {
                return _SelectedAppManager;
            }
            set
            {
                _SelectedAppManager = value;
                OnPropertyChanged("SelectedAppManager");
            }
        }

        protected List<IACComponent> _AppManagersList;
        [ACPropertyList(510, "AppManagers")]
        public List<IACComponent> AppManagersList
        {
            get
            {
                return _AppManagersList;
            }
            set
            {
                _AppManagersList = value;
                OnPropertyChanged("AppManagersList");
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod
        abstract protected void RefreshModules();

        abstract protected void RefreshTargets();
        #endregion
    }

}
