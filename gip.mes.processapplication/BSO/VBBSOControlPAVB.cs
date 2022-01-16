using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using System.Timers;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data.Objects;
using gip.mes.datamodel;
using gip.core.autocomponent;
using gip.mes.maintenance;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Unter-BSO für VBBSOControlDialog
    /// Wird verwendet für PABase (Modelwelt) und Ableitungen
    /// 
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Alarm query VB'}de{'Alarmabfrage VB'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, false)]
    public class VBBSOControlPAVB : VBBSOControlPA
    {
        #region c'tors
        public VBBSOControlPAVB(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            base.PropertyChanged += VBBSOControlPAVB_PropertyChanged;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }


        public override bool ACPostInit()
        {
            ShowMatConfigPoint = true;
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            base.PropertyChanged -= VBBSOControlPAVB_PropertyChanged;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties

        #region Database
        private DatabaseApp _DatabaseApp = null;
        /// <summary>Returns the shared Database-Context for BSO's by calling GetAppContextForBSO()</summary>
        /// <value>Returns the shared Database-Context.</value>
        public virtual DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp == null && this.InitState != ACInitState.Destructed && this.InitState != ACInitState.Destructing && this.InitState != ACInitState.DisposedToPool && this.InitState != ACInitState.DisposingToPool)
                    _DatabaseApp = this.GetAppContextForBSO();
                return _DatabaseApp as DatabaseApp;
            }
        }

        /// <summary>
        /// Overriden: Returns the DatabaseApp-Property.
        /// </summary>
        /// <value>The context as IACEntityObjectContext.</value>
        public override IACEntityObjectContext Database
        {
            get
            {
                return DatabaseApp;
            }
        }
        #endregion


        private bool _ShowMatConfigAll = false;
        [ACPropertyInfo(9999, "", "en{'Route independent (only material)'}de{'Wegeunabhängig (Nur Material)'}")]
        public bool ShowMatConfigAll
        {
            get
            {
                return _ShowMatConfigAll;
            }
            set
            {
                _ShowMatConfigAll = value;
                OnPropertyChanged("ShowMatConfigAll");
                _ShowMatConfigPoint = !_ShowMatConfigAll;
                OnPropertyChanged("ShowMatConfigPoint");
                LoadOutMaterialConfigList();
                LoadInMaterialConfigList();
            }
        }

        private bool _ShowMatConfigPoint = false;
        [ACPropertyInfo(9999, "", "en{'Route dependent (material + route)'}de{'Wegeabhängig (Material + Weg)'}")]
        public bool ShowMatConfigPoint
        {
            get
            {
                return _ShowMatConfigPoint;
            }
            set
            {
                _ShowMatConfigPoint = value;
                OnPropertyChanged("ShowMatConfigPoint");
                _ShowMatConfigAll = !_ShowMatConfigPoint;
                OnPropertyChanged("ShowMatConfigAll");
                LoadOutMaterialConfigList();
                LoadInMaterialConfigList();
            }
        }


        #region In-Material-Config
        private MaterialConfig _SelectedInMaterialConfig;
        [ACPropertySelected(9999, "InMaterialConfig", "en{'InMaterialConfig'}de{'InMaterialConfig'}")]
        public MaterialConfig SelectedInMaterialConfig
        {
            get
            {
                return _SelectedInMaterialConfig;
            }
            set
            {
                _SelectedInMaterialConfig = value;
                OnPropertyChanged("SelectedInMaterialConfig");
            }
        }

        private ObservableCollection<MaterialConfig> _InMaterialConfigList;
        /// <summary>
        /// List property for MaterialConfig
        /// </summary>
        /// <value>The MaterialConfigForIntermediate list</value>
        [ACPropertyList(9999, "InMaterialConfig")]
        public IEnumerable<MaterialConfig> InMaterialConfigList
        {
            get
            {
                return _InMaterialConfigList;
            }
            set
            {

                OnPropertyChanged("InMaterialConfigList");
            }
        }
        #endregion

        #region Out-Material-Config
        private MaterialConfig _SelectedOutMaterialConfig;
        [ACPropertySelected(9999, "OutMaterialConfig", "en{'OutMaterialConfig'}de{'OutMaterialConfig'}")]
        public MaterialConfig SelectedOutMaterialConfig
        {
            get
            {
                return _SelectedOutMaterialConfig;
            }
            set
            {
                _SelectedOutMaterialConfig = value;
                OnPropertyChanged("SelectedOutMaterialConfig");
            }
        }

        private ObservableCollection<MaterialConfig> _OutMaterialConfigList;
        /// <summary>
        /// List property for MaterialConfig
        /// </summary>
        /// <value>The MaterialConfigForIntermediate list</value>
        [ACPropertyList(9999, "OutMaterialConfig")]
        public IEnumerable<MaterialConfig> OutMaterialConfigList
        {
            get
            {
                return _OutMaterialConfigList;
            }
            set
            {

                OnPropertyChanged("OutMaterialConfigList");
            }
        }
        #endregion

        #region Material-Selection

        private Material _SelectedInMaterial;
        [ACPropertySelected(500, "InMaterial", "en{'Material'}de{'Material'}")]
        public Material SelectedInMaterial
        {
            get
            {
                return _SelectedInMaterial;
            }
            set
            {
                _SelectedInMaterial = value;
                OnPropertyChanged("SelectedInMaterial");
            }
        }

        private Material _SelectedOutMaterial;
        [ACPropertySelected(501, "OutMaterial", "en{'Material'}de{'Material'}")]
        public Material SelectedOutMaterial
        {
            get
            {
                return _SelectedOutMaterial;
            }
            set
            {
                _SelectedOutMaterial = value;
                OnPropertyChanged("SelectedOutMaterial");
            }
        }
        #endregion



        #endregion

        #region Methods
        void VBBSOControlPAVB_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedInPointConfig")
            {
                LoadInMaterialConfigList();
            }
            else if (e.PropertyName == "SelectedOutPointConfig")
            {
                LoadOutMaterialConfigList();
            }
            //else if(e.PropertyName == "CurrentACComponent")
            //{
            //    SetMaintACClass();
            //}
        }


        #region InMaterial
        protected void LoadInMaterialConfigList()
        {
            _InMaterialConfigList = null;
            if (CurrentACComponent == null)
            {
                OnPropertyChanged("InMaterialConfigList");
                return;
            }
            gip.core.datamodel.ACClass acClass = CurrentACComponent.ACType as gip.core.datamodel.ACClass;
            if (ShowMatConfigAll)
            {
                if (SelectedInPointConfig != null)
                {
                    _InMaterialConfigList = new ObservableCollection<MaterialConfig>(DatabaseApp.MaterialConfig.Where(c => c.VBiACClassID.HasValue
                                                                && c.VBiACClassID.Value == acClass.ACClassID
                                                                && !c.VBiACClassPropertyRelationID.HasValue
                                                                && c.LocalConfigACUrl == SelectedInPointConfig.ACIdentifier));
                    var query = DatabaseApp.GetAddedEntities<MaterialConfig>();
                    if (query != null && query.Any())
                    {
                        var query2 = query.Where(c => c.VBiACClassID.HasValue
                                    && c.VBiACClassID.Value == acClass.ACClassID
                                    && !c.VBiACClassPropertyRelationID.HasValue
                                    && c.LocalConfigACUrl == SelectedInPointConfig.ACIdentifier);
                        foreach (var matEntity in query2)
                        {
                            if (!_InMaterialConfigList.Contains(matEntity))
                                _InMaterialConfigList.Add(matEntity);
                        }
                    }
                }
            }
            else
            {
                if (SelectedInPoint != null && SelectedInPointConfig != null)
                {
                    _InMaterialConfigList = new ObservableCollection<MaterialConfig>(
                        DatabaseApp.MaterialConfig.Where(c => 
                            !c.VBiACClassID.HasValue
                            && c.LocalConfigACUrl == SelectedInPointConfig.ACIdentifier
                            && c.VBiACClassPropertyRelationID.HasValue
                            && c.VBiACClassPropertyRelation.SourceACClassID == SelectedInPoint.ACClassID
                            && c.VBiACClassPropertyRelation.TargetACClassID == acClass.ACClassID
                            && (c.VBiACClassPropertyRelation.ConnectionTypeIndex == (short)Global.ConnectionTypes.ConnectionPhysical || c.VBiACClassPropertyRelation.ConnectionTypeIndex == (short)Global.ConnectionTypes.DynamicConnection))
                            );
                    var query = DatabaseApp.GetAddedEntities<MaterialConfig>();
                    if (query != null && query.Any())
                    {
                        var query2 = query.Where(c => !c.VBiACClassID.HasValue
                                    && c.LocalConfigACUrl == SelectedInPointConfig.ACIdentifier
                                    && c.VBiACClassPropertyRelationID.HasValue
                                    && c.VBiACClassPropertyRelation.SourceACClassID == SelectedInPoint.ACClassID
                                    && c.VBiACClassPropertyRelation.TargetACClassID == acClass.ACClassID
                                    && c.VBiACClassPropertyRelation.ConnectionTypeIndex == (short)Global.ConnectionTypes.ConnectionPhysical || c.VBiACClassPropertyRelation.ConnectionTypeIndex == (short)Global.ConnectionTypes.DynamicConnection);
                        foreach (var matEntity in query2)
                        {
                            if (!_InMaterialConfigList.Contains(matEntity))
                                _InMaterialConfigList.Add(matEntity);
                        }
                    }
                }
            }
            OnPropertyChanged("InMaterialConfigList");
        }

        [ACMethodInteraction("InMaterialConfig", "en{'add material configuration'}de{'Materialkonfig. hinzufügen'}", (short)MISort.New, true, "SelectedInMaterialConfig", Global.ACKinds.MSMethod)]
        public void AddInMaterialConfig()
        {
            if (!IsEnabledAddInMaterialConfig())
                return;
            MaterialConfig materialConfig = SelectedInMaterial.NewACConfig(null, SelectedInPointConfig.ValueTypeACClass) as MaterialConfig;
            materialConfig.LocalConfigACUrl = SelectedInPointConfig.ACIdentifier;
            if (ShowMatConfigPoint)
            {
                var acClassID = CurrentACComponent.ACType.ACTypeID;
                materialConfig.VBiACClassPropertyRelationID = DatabaseApp.ACClassPropertyRelation.Where(c => c.SourceACClassID == SelectedInPoint.ACClassID
                                                            && c.TargetACClassID == acClassID
                                                            && (c.ConnectionTypeIndex == (short)Global.ConnectionTypes.ConnectionPhysical || c.ConnectionTypeIndex == (short)Global.ConnectionTypes.DynamicConnection))
                                                            .Select(c => c.ACClassPropertyRelationID)
                                                            .FirstOrDefault();
            }
            else
                materialConfig.VBiACClassID = CurrentACComponent.ACType.ACTypeID;
            //SelectedInMaterial.MaterialConfig_Material.Add(materialConfig);
            _InMaterialConfigList.Add(materialConfig);
            SelectedInMaterialConfig = materialConfig;
        }

        public bool IsEnabledAddInMaterialConfig()
        {
            return SelectedInMaterial != null 
                    && SelectedInPointConfig != null
                    && _InMaterialConfigList != null
                    && !_InMaterialConfigList.Where(c => c.MaterialID == SelectedInMaterial.MaterialID).Any()
                    && CurrentACComponent != null;
        }

        [ACMethodInteraction("InMaterialConfig", "en{'Remove material configuration'}de{'Entferne Materialkonfiguration'}", (short)MISort.Delete, true, "SelectedInMaterialConfig", Global.ACKinds.MSMethod)]
        public void RemoveInMaterialConfig()
        {
            if (!IsEnabledRemoveInMaterialConfig())
                return;
            SelectedInMaterialConfig.Material.MaterialConfig_Material.Remove(SelectedInMaterialConfig);
            SelectedInMaterialConfig.DeleteACObject(this.DatabaseApp, false);
            _InMaterialConfigList.Remove(SelectedInMaterialConfig);
            SelectedInMaterialConfig = _InMaterialConfigList.FirstOrDefault();
        }

        public bool IsEnabledRemoveInMaterialConfig()
        {
            return SelectedInMaterialConfig != null && _InMaterialConfigList != null;
        }
        #endregion

        #region OutMaterial
        protected void LoadOutMaterialConfigList()
        {
            _OutMaterialConfigList = null;
            if (CurrentACComponent == null)
            {
                OnPropertyChanged("OutMaterialConfigList");
                return;
            }
            gip.core.datamodel.ACClass acClass = CurrentACComponent.ACType as gip.core.datamodel.ACClass;
            if (ShowMatConfigAll)
            {
                if (SelectedOutPointConfig != null)
                {
                    _OutMaterialConfigList = new ObservableCollection<MaterialConfig>(DatabaseApp.MaterialConfig.Where(c => c.VBiACClassID.HasValue
                                                                && c.VBiACClassID.Value == acClass.ACClassID
                                                                && !c.VBiACClassPropertyRelationID.HasValue
                                                                && c.LocalConfigACUrl == SelectedOutPointConfig.ACIdentifier));
                    var query = DatabaseApp.GetAddedEntities<MaterialConfig>();
                    if (query != null && query.Any())
                    {
                        var query2 = query.Where(c => c.VBiACClassID.HasValue
                                    && c.VBiACClassID.Value == acClass.ACClassID
                                    && !c.VBiACClassPropertyRelationID.HasValue
                                    && c.LocalConfigACUrl == SelectedOutPointConfig.ACIdentifier);
                        foreach (var matEntity in query2)
                        {
                            if (!_OutMaterialConfigList.Contains(matEntity))
                                _OutMaterialConfigList.Add(matEntity);
                        }
                    }
                }
            }
            else
            {
                if (SelectedOutPoint != null && SelectedOutPointConfig != null)
                {
                    _OutMaterialConfigList = new ObservableCollection<MaterialConfig>(
                        DatabaseApp.MaterialConfig.Where(c => !c.VBiACClassID.HasValue
                            && c.LocalConfigACUrl == SelectedOutPointConfig.ACIdentifier
                            && c.VBiACClassPropertyRelationID.HasValue
                            && c.VBiACClassPropertyRelation.SourceACClassID == acClass.ACClassID
                            && c.VBiACClassPropertyRelation.TargetACClassID == SelectedOutPoint.ACClassID
                            && (c.VBiACClassPropertyRelation.ConnectionTypeIndex == (short)Global.ConnectionTypes.ConnectionPhysical || c.VBiACClassPropertyRelation.ConnectionTypeIndex == (short)Global.ConnectionTypes.DynamicConnection))
                            );
                    var query = DatabaseApp.GetAddedEntities<MaterialConfig>();
                    if (query != null && query.Any())
                    {
                        var query2 = query.Where(c => !c.VBiACClassID.HasValue
                                    && c.LocalConfigACUrl == SelectedOutPointConfig.ACIdentifier
                                    && c.VBiACClassPropertyRelationID.HasValue
                                    && c.VBiACClassPropertyRelation.SourceACClassID == acClass.ACClassID
                                    && c.VBiACClassPropertyRelation.TargetACClassID == SelectedOutPoint.ACClassID
                                    && (c.VBiACClassPropertyRelation.ConnectionTypeIndex == (short)Global.ConnectionTypes.ConnectionPhysical || c.VBiACClassPropertyRelation.ConnectionTypeIndex == (short)Global.ConnectionTypes.DynamicConnection));
                        foreach (var matEntity in query2)
                        {
                            if (!_OutMaterialConfigList.Contains(matEntity))
                                _OutMaterialConfigList.Add(matEntity);
                        }
                    }
                }
            }
            OnPropertyChanged("OutMaterialConfigList");
        }
        
        
        [ACMethodInteraction("OutMaterialConfig", "en{'Add material configuration'}de{'Materialkonfig. hinzufügen'}", (short)MISort.New, true, "SelectedOutMaterialConfig", Global.ACKinds.MSMethod)]
        public void AddOutMaterialConfig()
        {
            if (!IsEnabledAddOutMaterialConfig())
                return;
            MaterialConfig materialConfig = SelectedOutMaterial.NewACConfig(null, SelectedOutPointConfig.ValueTypeACClass) as MaterialConfig;
            materialConfig.LocalConfigACUrl = SelectedOutPointConfig.ACIdentifier;
            if (ShowMatConfigPoint)
            {
                var acClassID = CurrentACComponent.ACType.ACTypeID;
                materialConfig.VBiACClassPropertyRelationID = DatabaseApp.ACClassPropertyRelation.Where(c => c.SourceACClassID == acClassID
                                                            && c.TargetACClassID == SelectedOutPoint.ACClassID
                                                            && (c.ConnectionTypeIndex == (short)Global.ConnectionTypes.ConnectionPhysical || c.ConnectionTypeIndex == (short)Global.ConnectionTypes.DynamicConnection))
                                                            .Select(c => c.ACClassPropertyRelationID)
                                                            .FirstOrDefault();
            }
            else
                materialConfig.VBiACClassID = CurrentACComponent.ACType.ACTypeID;
            //SelectedOutMaterial.MaterialConfig_Material.Add(materialConfig);
            _OutMaterialConfigList.Add(materialConfig);
            SelectedOutMaterialConfig = materialConfig;
        }

        public bool IsEnabledAddOutMaterialConfig()
        {
            return SelectedOutMaterial != null 
                    && SelectedOutPointConfig != null
                    && _OutMaterialConfigList != null 
                    && !_OutMaterialConfigList.Where(c => c.MaterialID == SelectedOutMaterial.MaterialID).Any()
                    && CurrentACComponent != null;
        }

        [ACMethodInteraction("OutMaterialConfig", "en{'Remove material configuration'}de{'Entferne Materialkonfiguration'}", (short)MISort.Delete, true, "SelectedOutMaterialConfig", Global.ACKinds.MSMethod)]
        public void RemoveOutMaterialConfig()
        {
            if (!IsEnabledRemoveOutMaterialConfig())
                return;
            SelectedOutMaterialConfig.Material.MaterialConfig_Material.Remove(SelectedOutMaterialConfig);
            SelectedOutMaterialConfig.DeleteACObject(this.DatabaseApp, false);
            _OutMaterialConfigList.Remove(SelectedOutMaterialConfig);
            SelectedOutMaterialConfig = _OutMaterialConfigList.FirstOrDefault();
        }

        public bool IsEnabledRemoveOutMaterialConfig()
        {
            return SelectedOutMaterialConfig != null && _OutMaterialConfigList != null;
        }
        #endregion

        //#region Maintenance

        //private void SetMaintACClass()
        //{
        //    if(ParentACComponent is VBBSOControlDialog)
        //    {
        //        BSOMaintConfig maintConfig = ParentACComponent.FindChildComponents<BSOMaintConfig>(c => c is BSOMaintConfig).FirstOrDefault();
        //        if(maintConfig != null)
        //        {
        //            maintConfig.SetACClass(CurrentACComponent.ACType as gip.core.datamodel.ACClass, true);
        //        }
        //    }
        //}

        //#endregion

        #endregion




        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"AddInMaterialConfig":
                    AddInMaterialConfig();
                    return true;
                case"IsEnabledAddInMaterialConfig":
                    result = IsEnabledAddInMaterialConfig();
                    return true;
                case"RemoveInMaterialConfig":
                    RemoveInMaterialConfig();
                    return true;
                case"IsEnabledRemoveInMaterialConfig":
                    result = IsEnabledRemoveInMaterialConfig();
                    return true;
                case"AddOutMaterialConfig":
                    AddOutMaterialConfig();
                    return true;
                case"IsEnabledAddOutMaterialConfig":
                    result = IsEnabledAddOutMaterialConfig();
                    return true;
                case"RemoveOutMaterialConfig":
                    RemoveOutMaterialConfig();
                    return true;
                case"IsEnabledRemoveOutMaterialConfig":
                    result = IsEnabledRemoveOutMaterialConfig();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
