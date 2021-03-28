using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Country Tax'}de{'Staat MwStr'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDCountrySalesTax.ClassName)]

    public class MDBSOCountrySalesTax : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOCostCenter"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOCountrySalesTax(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            _MDCountryList = LoadMDCountryList();
            OnPropertyChanged("MDCountryList");
            _MDMaterialGroupList = LoadMDMaterialGroupList();
            OnPropertyChanged("MDMaterialGroupList");
            Search();
            LoadTaxPositions();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);
            return b;
        }

        #endregion

        #region BSO->ACProperty

        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        ACAccessNav<MDCountrySalesTax> _AccessPrimary;

        [ACPropertyAccessPrimary(690, MDCountrySalesTax.ClassName)]
        public ACAccessNav<MDCountrySalesTax> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDCountrySalesTax>(MDCountrySalesTax.ClassName, this);
                }
                return _AccessPrimary;
            }
        }


        [ACPropertyList(601, MDCountrySalesTax.ClassName)]
        public IEnumerable<MDCountrySalesTax> MDCountrySalesTaxList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, MDCountrySalesTax.ClassName)]
        public MDCountrySalesTax SelectedMDCountrySalesTax
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return;
                if (AccessPrimary.Selected != value)
                {
                    AccessPrimary.Selected = value;
                    OnPropertyChanged("SelectedMDCountrySalesTax");
                    LoadTaxPositions();
                }
            }
        }

        private void LoadTaxPositions()
        {
            MDCountrySalesTaxMaterialList = LoadMDCountrySalesTaxMaterial();
            if (MDCountrySalesTaxMaterialList == null)
                SelectedMDCountrySalesTaxMaterial = null;
            else
                SelectedMDCountrySalesTaxMaterial = MDCountrySalesTaxMaterialList.FirstOrDefault();

            MDCountrySalesTaxMDMaterialGroupList = LoadMDCountrySalesTaxMDMaterialGroup();
            if (MDCountrySalesTaxMDMaterialGroupList == null)
                SelectedMDCountrySalesTaxMDMaterialGroup = null;
            else
                SelectedMDCountrySalesTaxMDMaterialGroup = MDCountrySalesTaxMDMaterialGroupList.FirstOrDefault();
        }

        #endregion

        #region MDCountrySalesTaxMaterial


        #region MDCountrySalesTaxMaterial -> Properties

        private MDCountrySalesTaxMaterial _SelectedMDCountrySalesTaxMaterial;
        /// <summary>
        /// Selected property for TaxMDMaterialGroup
        /// </summary>
        /// <value>The selected TaxMDMaterialGroup</value>
        [ACPropertySelected(9999, MDCountrySalesTaxMaterial.ClassName, "en{'TODO: TaxMDMaterialGroup'}de{'TODO: TaxMDMaterialGroup'}")]
        public MDCountrySalesTaxMaterial SelectedMDCountrySalesTaxMaterial
        {
            get
            {
                return _SelectedMDCountrySalesTaxMaterial;
            }
            set
            {
                if (_SelectedMDCountrySalesTaxMaterial != value)
                {
                    _SelectedMDCountrySalesTaxMaterial = value;
                    OnPropertyChanged("SelectedMDCountrySalesTaxMaterial");
                }
            }
        }


        private List<MDCountrySalesTaxMaterial> _MDCountrySalesTaxMaterialList;
        /// <summary>
        /// List property for TaxMDMaterialGroup
        /// </summary>
        /// <value>The TaxMDMaterialGroup list</value>
        [ACPropertyList(9999, MDCountrySalesTaxMaterial.ClassName)]
        public List<MDCountrySalesTaxMaterial> MDCountrySalesTaxMaterialList
        {
            get
            {
                if (_MDCountrySalesTaxMaterialList == null)
                    _MDCountrySalesTaxMaterialList = new List<MDCountrySalesTaxMaterial>();
                return _MDCountrySalesTaxMaterialList;
            }
            set
            {
                _MDCountrySalesTaxMaterialList = value;
                OnPropertyChanged("MDCountrySalesTaxMaterialList");
            }
        }
        #endregion

        #region MDCountrySalesTaxMaterial -> Methods

        private List<MDCountrySalesTaxMaterial> LoadMDCountrySalesTaxMaterial()
        {
            if (SelectedMDCountrySalesTax == null) return null;
            return SelectedMDCountrySalesTax.MDCountrySalesTaxMaterial_MDCountrySalesTax.OrderBy(c => c.Material.MaterialNo).ToList();
        }

        [ACMethodInfo(MDCountrySalesTaxMaterial.ClassName, "en{'New Position'}de{'Neue Position'}", 999)]
        public void AddMDCountrySalesTaxMaterial()
        {
            MDCountrySalesTaxMaterial entity = MDCountrySalesTaxMaterial.NewACObject(DatabaseApp, SelectedMDCountrySalesTax);
            entity.Material = BSOMaterialExplorer_Child.Value.SelectedMaterial;
            MDCountrySalesTaxMaterialList.Add(entity);
            OnPropertyChanged("MDCountrySalesTaxMaterialList");
            SelectedMDCountrySalesTaxMaterial = entity;
        }

        [ACMethodInfo(MDCountrySalesTaxMaterial.ClassName, "en{'Delete Position'}de{'Position löschen'}", 999)]
        public void DeleteMDCountrySalesTaxMaterial()
        {
            MDCountrySalesTaxMaterialList.Remove(SelectedMDCountrySalesTaxMaterial);
            Msg msg = SelectedMDCountrySalesTaxMaterial.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                MDCountrySalesTaxMaterialList.Add(SelectedMDCountrySalesTaxMaterial);
                return;
            }
            else
            {
                SelectedMDCountrySalesTaxMaterial = MDCountrySalesTaxMaterialList != null ? MDCountrySalesTaxMaterialList.FirstOrDefault() : null;
                OnPropertyChanged("MDCountrySalesTaxMaterialList");
            }
        }

        public bool IsEnabledAddAddMDCountrySalesTaxMaterial()
        {
            return SelectedMDCountrySalesTax != null
                && BSOMaterialExplorer_Child != null
                && BSOMaterialExplorer_Child.Value != null
                && BSOMaterialExplorer_Child.Value.SelectedMaterial != null
                && (MDCountrySalesTaxMaterialList == null || !MDCountrySalesTaxMaterialList.Where(c => c.MaterialID == BSOMaterialExplorer_Child.Value.SelectedMaterial.MaterialID).Any());
        }

        public bool IsEnabledDeleteMDCountrySalesTaxMaterial()
        {
            return SelectedMDCountrySalesTax != null && SelectedMDCountrySalesTaxMaterial != null;
        }

        #endregion

        #endregion

        #region MDCountrySalesTaxMDMaterialGroup

        #region MDCountrySalesTaxMDMaterialGroup -> Properties

        private MDCountrySalesTaxMDMaterialGroup _SelectedMDCountrySalesTaxMDMaterialGroup;
        /// <summary>
        /// Selected property for TaxMaterial
        /// </summary>
        /// <value>The selected TaxMaterial</value>
        [ACPropertySelected(9999, MDCountrySalesTaxMDMaterialGroup.ClassName, "en{'TODO: TaxMaterial'}de{'TODO: TaxMaterial'}")]
        public MDCountrySalesTaxMDMaterialGroup SelectedMDCountrySalesTaxMDMaterialGroup
        {
            get
            {
                return _SelectedMDCountrySalesTaxMDMaterialGroup;
            }
            set
            {
                if (_SelectedMDCountrySalesTaxMDMaterialGroup != value)
                {
                    _SelectedMDCountrySalesTaxMDMaterialGroup = value;
                    OnPropertyChanged("SelectedMDCountrySalesTaxMDMaterialGroup");
                }
            }
        }


        private List<MDCountrySalesTaxMDMaterialGroup> _MDCountrySalesTaxMDMaterialGroupList;
        /// <summary>
        /// List property for TaxMaterial
        /// </summary>
        /// <value>The TaxMaterial list</value>
        [ACPropertyList(9999, MDCountrySalesTaxMDMaterialGroup.ClassName)]
        public List<MDCountrySalesTaxMDMaterialGroup> MDCountrySalesTaxMDMaterialGroupList
        {
            get
            {
                if (_MDCountrySalesTaxMDMaterialGroupList == null)
                    _MDCountrySalesTaxMDMaterialGroupList = new List<MDCountrySalesTaxMDMaterialGroup>();
                return _MDCountrySalesTaxMDMaterialGroupList;
            }
            set
            {
                _MDCountrySalesTaxMDMaterialGroupList = value;
            }
        }

        #endregion

        #region MDCountrySalesTaxMDMaterialGroup -> Methods

        private List<MDCountrySalesTaxMDMaterialGroup> LoadMDCountrySalesTaxMDMaterialGroup()
        {
            if (SelectedMDCountrySalesTax == null) return null;
            return SelectedMDCountrySalesTax.MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax.OrderBy(c => c.MDMaterialGroup.MDKey).ToList();
        }

        [ACMethodInfo(MDCountrySalesTaxMDMaterialGroup.ClassName, "en{'New Position'}de{'Neue Position'}", 999)]
        public void AddMDCountrySalesTaxMDMaterialGroup()
        {
            MDCountrySalesTaxMDMaterialGroup entity = MDCountrySalesTaxMDMaterialGroup.NewACObject(DatabaseApp, SelectedMDCountrySalesTax);
            entity.MDMaterialGroup = SelectedMDMaterialGroup;
            MDCountrySalesTaxMDMaterialGroupList.Add(entity);
            OnPropertyChanged("MDCountrySalesTaxMDMaterialGroupList");
            SelectedMDCountrySalesTaxMDMaterialGroup = entity;
        }

        [ACMethodInfo(MDCountrySalesTaxMDMaterialGroup.ClassName, "en{'Delete Position'}de{'Position löschen'}", 999)]
        public void DeleteMDCountrySalesTaxMDMaterialGroup()
        {
            MDCountrySalesTaxMDMaterialGroupList.Remove(SelectedMDCountrySalesTaxMDMaterialGroup);
            Msg msg = SelectedMDCountrySalesTaxMDMaterialGroup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                MDCountrySalesTaxMDMaterialGroupList.Add(SelectedMDCountrySalesTaxMDMaterialGroup);
                return;
            }
            else
            {
                SelectedMDCountrySalesTaxMDMaterialGroup = MDCountrySalesTaxMDMaterialGroupList != null ? MDCountrySalesTaxMDMaterialGroupList.FirstOrDefault() : null;
                OnPropertyChanged("MDCountrySalesTaxMDMaterialGroupList");
            }
        }

        public bool IsEnabledAddMDCountrySalesTaxMDMaterialGroup()
        {
            return SelectedMDCountrySalesTax != null
                && SelectedMDMaterialGroup != null
                && (MDCountrySalesTaxMDMaterialGroupList == null || !MDCountrySalesTaxMDMaterialGroupList.Where(c => c.MDMaterialGroupID == SelectedMDMaterialGroup.MDMaterialGroupID).Any());
        }

        public bool IsEnabledDeleteMDCountrySalesTaxMDMaterialGroup()
        {
            return SelectedMDCountrySalesTax != null && SelectedMDCountrySalesTaxMDMaterialGroup != null;
        }

        #endregion

        #endregion

        #region Messages

        #region Messages -> IMsgObserver

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged("MsgList");
        }

        #endregion

        #region Messages -> Properties

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(9999, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged("CurrentMsg");
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(9999, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        #endregion

        #endregion

        #region Material
        ACChildItem<BSOMaterialExplorer> _BSOMaterialExplorer_Child;
        [ACPropertyInfo(9999)]
        [ACChildInfo("BSOMaterialExplorer_Child", typeof(BSOMaterialExplorer))]
        public ACChildItem<BSOMaterialExplorer> BSOMaterialExplorer_Child
        {
            get
            {
                if (_BSOMaterialExplorer_Child == null)
                    _BSOMaterialExplorer_Child = new ACChildItem<BSOMaterialExplorer>(this, "BSOMaterialExplorer_Child");
                return _BSOMaterialExplorer_Child;
            }
        }
        #endregion

        #region MDMaterialGroup
        private MDMaterialGroup _SelectedMDMaterialGroup;
        /// <summary>
        /// Selected property for MDMaterialGroup
        /// </summary>
        /// <value>The selected MDMaterialGroup</value>
        [ACPropertySelected(9999, "MDMaterialGroup", "en{'TODO: MDMaterialGroup'}de{'TODO: MDMaterialGroup'}")]
        public MDMaterialGroup SelectedMDMaterialGroup
        {
            get
            {
                return _SelectedMDMaterialGroup;
            }
            set
            {
                if (_SelectedMDMaterialGroup != value)
                {
                    _SelectedMDMaterialGroup = value;
                    OnPropertyChanged("SelectedMDMaterialGroup");
                }
            }
        }


        private List<MDMaterialGroup> _MDMaterialGroupList;
        /// <summary>
        /// List property for MDMaterialGroup
        /// </summary>
        /// <value>The MDMaterialGroup list</value>
        [ACPropertyList(9999, "MDMaterialGroup")]
        public List<MDMaterialGroup> MDMaterialGroupList
        {
            get
            {
                if (_MDMaterialGroupList == null)
                    _MDMaterialGroupList = LoadMDMaterialGroupList();
                return _MDMaterialGroupList;
            }
        }

        private List<MDMaterialGroup> LoadMDMaterialGroupList()
        {
            return DatabaseApp.MDMaterialGroup.OrderBy(c => c.SortIndex).ToList();
        }
        #endregion

        #region MDCountry
        private MDCountry _SelectedMDCountry;
        /// <summary>
        /// Selected property for MDCountry
        /// </summary>
        /// <value>The selected MDCountry</value>
        [ACPropertySelected(9999, MDCountry.ClassName, ConstApp.ESCountry)]
        public MDCountry SelectedMDCountry
        {
            get
            {
                return _SelectedMDCountry;
            }
            set
            {
                if (_SelectedMDCountry != value)
                {
                    _SelectedMDCountry = value;
                    OnPropertyChanged("SelectedMDCountry");
                }
            }
        }


        private List<MDCountry> _MDCountryList;
        /// <summary>
        /// List property for MDCountry
        /// </summary>
        /// <value>The MDCountry list</value>
        [ACPropertyList(9999, MDCountry.ClassName)]
        public List<MDCountry> MDCountryList
        {
            get
            {
                if (_MDCountryList == null)
                    _MDCountryList = LoadMDCountryList();
                return _MDCountryList;
            }
        }

        private List<MDCountry> LoadMDCountryList()
        {
            return DatabaseApp.MDCountry.OrderBy(c => c.SortIndex).ToList();
        }
        #endregion

        #region Methods
        [ACMethodCommand(MDCountrySalesTax.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(MDCountrySalesTax.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(MDCountrySalesTax.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedTax", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDCountrySalesTax>(requery, () => SelectedMDCountrySalesTax, () => SelectedMDCountrySalesTax, c => SelectedMDCountrySalesTax = c,
                        DatabaseApp.MDCountrySalesTax
                        .Include(c => c.MDCountrySalesTaxMaterial_MDCountrySalesTax)
                        .Include(c => c.MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax)
                        .Where(c => c.MDCountrySalesTaxID == SelectedMDCountrySalesTax.MDCountrySalesTaxID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedMDCountrySalesTax != null;
        }

        [ACMethodInteraction(MDCountrySalesTax.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedTax", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            SelectedMDCountrySalesTax = MDCountrySalesTax.NewACObject(DatabaseApp, null);
            DatabaseApp.MDCountrySalesTax.AddObject(SelectedMDCountrySalesTax);
            AccessPrimary.NavList.Add(SelectedMDCountrySalesTax);
            OnPropertyChanged("MDCountrySalesTaxList");
            ACState = Const.SMNew;
            PostExecute("New");
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(MDCountrySalesTax.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedTax", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            SelectedMDCountrySalesTax.MDCountrySalesTaxMaterial_MDCountrySalesTax.Clear();
            SelectedMDCountrySalesTax.MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax.Clear();
            Msg msg = SelectedMDCountrySalesTax.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(SelectedMDCountrySalesTax);
            SelectedMDCountrySalesTax = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        public bool IsEnabledDelete()
        {
            return true;
        }

        [ACMethodCommand(OutOffer.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("TaxList");
        }

        protected override Msg OnPreSave()
        {
            Msg result = null;
            if (SelectedMDCountrySalesTax != null)
            {
                // Error50400. StartTime, EndTime
                bool isInvalidPeriod = SelectedMDCountrySalesTax.DateTo != null && SelectedMDCountrySalesTax.DateTo <= SelectedMDCountrySalesTax.DateFrom;
                if (isInvalidPeriod)
                {
                    result = new Msg()
                    {
                        Message = Root.Environment.TranslateMessage(this, "Error50403", SelectedMDCountrySalesTax.DateFrom, SelectedMDCountrySalesTax.DateTo),
                        MessageLevel = eMsgLevel.Error
                    };
                }
                else
                {
                    // Error50398. TaxNo
                    MDCountrySalesTax concurentTax =
                        DatabaseApp
                        .MDCountrySalesTax
                        .Where(c =>
                                    c.MDCountrySalesTaxID != SelectedMDCountrySalesTax.MDCountrySalesTaxID
                                    && c.MDCountryID == SelectedMDCountrySalesTax.MDCountryID
                                    &&
                                    (
                                        c.DateFrom <= SelectedMDCountrySalesTax.DateFrom && (c.DateTo ?? DateTime.Now) >= SelectedMDCountrySalesTax.DateFrom
                                        || c.DateFrom >= (SelectedMDCountrySalesTax.DateTo ?? DateTime.Now) && (c.DateTo ?? DateTime.Now) <= (SelectedMDCountrySalesTax.DateTo ?? DateTime.Now)
                                    )
                                )
                        .FirstOrDefault();
                    if (concurentTax != null)
                    {
                        result = new Msg()
                        {
                            Message = Root.Environment.TranslateMessage(this, "Error50402", SelectedMDCountrySalesTax.MDCountrySalesTaxName),
                            MessageLevel = eMsgLevel.Error
                        };
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
