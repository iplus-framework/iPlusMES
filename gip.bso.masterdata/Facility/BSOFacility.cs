using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Storage Location'}de{'Lagerplatz'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName)]
    public class BSOFacility : BSOFacilityExplorer
    {

        #region consts
        public const string ClassName = "BSOFacility";
        public const string ParameterName_ParentFacilityID = "ParentFacilityID";
        #endregion

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityLocation"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacility(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }


        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _FacilityOEEManager = ACFacilityOEEManager.ACRefToServiceInstance(this);
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_FacilityOEEManager != null)
                ACFacilityOEEManager.DetachACRefFromServiceInstance(this, _FacilityOEEManager);
            _FacilityOEEManager = null;
            return base.ACDeInit(deleteACClassTask);
        }

        public override object Clone()
        {
            object clonedObject = base.Clone();
            BSOFacility clonedBSOFacility = clonedObject as BSOFacility;
            if (CurrentFacility != null && CurrentFacility.ACObject != null)
            {
                Facility facility = CurrentFacility.ACObject as Facility;
                clonedBSOFacility.CurrentFacility = new ACFSItem(null, CurrentFacility.Container, facility, FacilityTree.FacilityACCaption(facility), ResourceTypeEnum.IACObject);
            }
            return clonedBSOFacility;
        }

        #endregion


        #region Properties

        #region Managers
        protected ACRef<ACFacilityOEEManager> _FacilityOEEManager = null;
        public ACFacilityOEEManager FacilityOEEManager
        {
            get
            {
                if (_FacilityOEEManager == null)
                    return null;
                return _FacilityOEEManager.ValueT;
            }
        }
        #endregion

        #region FacilityMaterial
        private FacilityMaterial _SelectedFacilityMaterial;
        /// <summary>
        /// Selected property for FacilityMaterial
        /// </summary>
        /// <value>The selected FacilityMaterial</value>
        [ACPropertySelected(9999, "FacilityMaterial", "en{'TODO: FacilityMaterial'}de{'TODO: FacilityMaterial'}")]
        public FacilityMaterial SelectedFacilityMaterial
        {
            get
            {
                return _SelectedFacilityMaterial;
            }
            set
            {
                if (_SelectedFacilityMaterial != value)
                {
                    _SelectedFacilityMaterial = value;
                    OnPropertyChanged(nameof(SelectedFacilityMaterial));
                }
            }
        }


        private List<FacilityMaterial> _FacilityMaterialList;
        /// <summary>
        /// List property for FacilityMaterial
        /// </summary>
        /// <value>The FacilityMaterial list</value>
        [ACPropertyList(9999, "FacilityMaterial")]
        public List<FacilityMaterial> FacilityMaterialList
        {
            get
            {
                if (_FacilityMaterialList == null)
                    _FacilityMaterialList = LoadFacilityMaterialList();
                return _FacilityMaterialList;
            }
            set
            {
                _FacilityMaterialList = value;
                OnPropertyChanged(nameof(FacilityMaterialList));
            }
        }

        private List<FacilityMaterial> LoadFacilityMaterialList()
        {
            if (SelectedFacility == null)
                return new List<FacilityMaterial>();
            return
                SelectedFacility
                .FacilityMaterial_Facility
                .OrderBy(c => c.Material.MaterialNo)
                .ToList();
        }
        #endregion

        #endregion

        #region Methods

        #region BSO

        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            if (name == nameof(SelectedFacility))
            {
                FacilityMaterialList = LoadFacilityMaterialList();
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
            DatabaseApp.OnPropertyChanged(Facility.ClassName);
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(nameof(Facility), "en{'Load'}de{'Laden'}", (short)MISort.Load, false, nameof(SelectedFacility), Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            LoadEntity<Facility>(requery, () => SelectedFacility, () => SelectedFacility, c => SelectedFacility = c,
                        DatabaseApp.Facility
                         .Include(c => c.Material)
                         .Include(c => c.MDFacilityType)
                       
                        .Where(c => c.FacilityID == SelectedFacility.FacilityID));
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedFacility != null;
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
            Load();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }


        #region Mehtods ->  BSO -> ACMethod -> Tree operation

        [ACMethodInteraction(Facility.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "CurrentFacility", Global.ACKinds.MSMethodPrePost)]
        public virtual void AddFacility()
        {
            if (!IsEnabledAddFacility()) return;
            ACFSItem currentFacilityACFSItem = CurrentFacility;

            Facility parentFacility = null;
            if (CurrentFacility.ACObject != null)
                parentFacility = CurrentFacility.ACObject as Facility;
            Facility facility = Facility.NewACObject(DatabaseApp, parentFacility);

            MDFacilityType mDFacilityType = DatabaseApp.MDFacilityType.FirstOrDefault(C => C.IsDefault);
            if (mDFacilityType == null)
                mDFacilityType = DatabaseApp.MDFacilityType.OrderBy(c => c.SortIndex).FirstOrDefault();
            int nr = 0;
            Action<ACFSItem> countNewItems = null;
            countNewItems = (ACFSItem item) =>
            {
                if (item.ACObject != null && (item.ACObject as Facility).FacilityNo.StartsWith("NewFacility"))
                    nr++;
                foreach (var childItem in item.Items)
                    countNewItems(childItem as ACFSItem);
            };
            countNewItems(CurrentFacilityRoot);

            facility.FacilityNo = string.Format(@"NewFacility{0}", nr + 1);
            facility.FacilityName = "-";
            facility.MDFacilityType = mDFacilityType;
            facility.MDFacilityTypeID = mDFacilityType.MDFacilityTypeID;
            DatabaseApp.Facility.AddObject(facility);
            if (parentFacility != null)
                parentFacility.Facility_ParentFacility.Add(facility);

            ACFSItem newFacilityACFSItem = new ACFSItem(null, CurrentFacility.Container, facility, FacilityTree.FacilityACCaption(facility), ResourceTypeEnum.IACObject);
            newFacilityACFSItem.OnACFSItemChange += FacilityTree.CFSItem_OnACFSItemChange;
            currentFacilityACFSItem.Add(newFacilityACFSItem);

            CurrentFacilityRootChangeInfo = new ChangeInfo(currentFacilityACFSItem, newFacilityACFSItem, Const.CmdAddChildData);

            OnPropertyChanged("CurrentFacility");
            OnPropertyChanged("SelectedFacility");
        }

        public virtual bool IsEnabledAddFacility()
        {
            return CurrentFacility != null;
        }

        [ACMethodInteraction(Facility.ClassName, "en{'Delete'}de{'Loschen'}", (short)MISort.Delete, true, "CurrentFacility", Global.ACKinds.MSMethodPrePost)]
        public virtual void DeleteFacility()
        {
            if (!IsEnabledDeleteFacility()) return;
            ACFSItem parent = CurrentFacility.ParentACObject as ACFSItem;
            ACFSItem current = CurrentFacility;

            parent.Remove(current);

            Facility dbFacility = current.ACObject as Facility;
            MsgWithDetails msg = dbFacility.DeleteACObject(DatabaseApp, false);

            CurrentFacility = parent;

            CurrentFacilityRootChangeInfo = new ChangeInfo(null, CurrentFacility, Const.CmdDeleteData);


            OnPropertyChanged("CurrentFacility");
            OnPropertyChanged("SelectedFacility");
        }

        public virtual bool IsEnabledDeleteFacility()
        {
            return CurrentFacility != null && CurrentFacility.ACObject != null && !CurrentFacility.Items.Any();
        }

        #endregion


        //[ACMethodInteraction("", "en{'Print labels for child facilites'}de{'Print labels for child facilites'}", 800, true)]
        //public void PrintLabelsForChildFacilities()
        //{
        //    var items = CurrentFacility.VisibleItemsT;

        //    foreach(var item in items)
        //    {

        //    }
        //}

        //public bool IsEnabledPrintLabelsForChildFacilities()
        //{
        //    return CurrentFacility != null;
        //}


        #endregion

        #region FacilityMaterial and OEE
        [ACMethodInfo("", "en{'Add'}de{'Neu'}", 700)]
        public void AddFacilityMaterial()
        {
            if (!IsEnabledAddFacilityMaterial())
                return;

            FacilityMaterial facilityMaterial = FacilityMaterial.NewACObject(DatabaseApp, null);
            facilityMaterial.Facility = SelectedFacility;
            SelectedFacility.FacilityMaterial_Facility.Add(facilityMaterial);
            FacilityMaterialList.Add(facilityMaterial);
            OnPropertyChanged(nameof(FacilityMaterialList));

            SelectedFacilityMaterial = facilityMaterial;
        }

        public bool IsEnabledAddFacilityMaterial()
        {
            return SelectedFacility != null;
        }


        /// <summary>
        /// Source Property: DeleteFacility
        /// </summary>
        [ACMethodInfo("", "en{'Delete'}de{'Löschen'}", 701)]
        public void DeleteFacilityMaterial()
        {
            if (!IsEnabledDeleteFacilityMaterial())
                return;

            SelectedFacility.FacilityMaterial_Facility.Remove(SelectedFacilityMaterial);
            FacilityMaterialList.Remove(SelectedFacilityMaterial);

            SelectedFacilityMaterial.DeleteACObject(DatabaseApp, false);

            OnPropertyChanged(nameof(FacilityMaterialList));
        }

        public bool IsEnabledDeleteFacilityMaterial()
        {
            return SelectedFacilityMaterial != null;
        }


        [ACMethodInfo("", "en{'Generate OEE test data'}de{'OEE Testdaten generieren'}", 703)]
        public void GenerateTestOEEData()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                Msg msg = FacilityOEEManager.GenerateTestOEEData(dbApp, facilityMaterial);
                if (msg != null)
                    Messages.Msg(msg);
            }
        }

        public bool IsEnabledGenerateTestOEEData()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }

        [ACMethodInfo("", "en{'Delete OEE test data'}de{'OEE Testdaten löschen'}", 704)]
        public void DeleteTestOEEData()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                FacilityOEEManager.DeleteTestOEEData(dbApp, facilityMaterial);
            }
        }

        public bool IsEnabledDeleteTestOEEData()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }


        [ACMethodInfo("", "en{'Recalc average throughput'}de{'Aktualisiere Mittelwert Durchsatz'}", 705)]
        public void RecalcThroughputAverage()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;

            Msg msg = FacilityOEEManager.RecalcThroughputAverage(this.DatabaseApp, SelectedFacilityMaterial, false);
            if (msg != null)
                Messages.Msg(msg);
        }

        public bool IsEnabledRecalcThroughputAverage()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }

        [ACMethodInfo("", "en{'Correct throuhputs and OEE'}de{'Korrigiere Durchsätze und OEE'}", 706)]
        public void RecalcThroughputAndOEE()
        {
            if (SelectedFacilityMaterial == null || FacilityOEEManager == null)
                return;

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                FacilityMaterial facilityMaterial = SelectedFacilityMaterial.FromAppContext<FacilityMaterial>(dbApp);
                Msg msg = FacilityOEEManager.RecalcThroughputAndOEE(dbApp, facilityMaterial, null, null);
                if (msg != null)
                    Messages.Msg(msg);
            }
        }

        public bool IsEnabledRecalcThroughputAndOEE()
        {
            return SelectedFacilityMaterial != null && FacilityOEEManager != null;
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Save):
                    Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(AddFacility):
                    AddFacility();
                    return true;
                case nameof(IsEnabledAddFacility):
                    result = IsEnabledAddFacility();
                    return true;
                case nameof(DeleteFacility):
                    DeleteFacility();
                    return true;
                case nameof(IsEnabledDeleteFacility):
                    result = IsEnabledDeleteFacility();
                    return true;
                case nameof(AddFacilityMaterial):
                    AddFacilityMaterial();
                    return true;
                case nameof(IsEnabledAddFacilityMaterial):
                    result = IsEnabledAddFacilityMaterial();
                    return true;
                case nameof(DeleteFacilityMaterial):
                    DeleteFacilityMaterial();
                    return true;
                case nameof(IsEnabledDeleteFacilityMaterial):
                    result = IsEnabledDeleteFacilityMaterial();
                    return true;
                case nameof(GenerateTestOEEData):
                    GenerateTestOEEData();
                    return true;
                case nameof(IsEnabledGenerateTestOEEData):
                    result = IsEnabledGenerateTestOEEData();
                    return true;
                case nameof(DeleteTestOEEData):
                    DeleteTestOEEData();
                    return true;
                case nameof(IsEnabledDeleteTestOEEData):
                    result = IsEnabledDeleteTestOEEData();
                    return true;
                case nameof(RecalcThroughputAverage):
                    RecalcThroughputAverage();
                    return true;
                case nameof(IsEnabledRecalcThroughputAverage):
                    result = IsEnabledRecalcThroughputAverage();
                    return true;
                case nameof(RecalcThroughputAndOEE):
                    RecalcThroughputAndOEE();
                    return true;
                case nameof(IsEnabledRecalcThroughputAndOEE):
                    result = IsEnabledRecalcThroughputAndOEE();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #endregion
    }
}
