using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace gip.bso.sales
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Tax'}de{'MwStr'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Tax.ClassName)]
    public class BSOTax : ACBSOvbNav
    {
        #region c´tors

        public BSOTax(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;


            Search();
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
        ACAccessNav<Tax> _AccessPrimary;

        [ACPropertyAccessPrimary(690, Tax.ClassName)]
        public ACAccessNav<Tax> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Tax>(Tax.ClassName, this);
                }
                return _AccessPrimary;
            }
        }


        [ACPropertyList(601, Tax.ClassName)]
        public IEnumerable<Tax> TaxList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, Tax.ClassName)]
        public Tax SelectedTax
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
                    OnPropertyChanged("SelectedTax");

                    TaxMDMaterialGroupList = LoadTaxMDMaterialGroupList();
                    if (TaxMDMaterialGroupList == null)
                        SelectedTaxMDMaterialGroup = null;
                    else
                        SelectedTaxMDMaterialGroup = TaxMDMaterialGroupList.FirstOrDefault();

                    TaxMaterialList = LoadTaxMaterialList();
                    if (TaxMaterialList == null)
                        SelectedTaxMaterial = null;
                    else
                        SelectedTaxMaterial = TaxMaterialList.FirstOrDefault();
                }
            }
        }

        #endregion


        #region TaxMDMaterialGroup


        #region TaxMDMaterialGroup -> Properties

        private TaxMDMaterialGroup _SelectedTaxMDMaterialGroup;
        /// <summary>
        /// Selected property for TaxMDMaterialGroup
        /// </summary>
        /// <value>The selected TaxMDMaterialGroup</value>
        [ACPropertySelected(9999, TaxMDMaterialGroup.ClassName, "en{'TODO: TaxMDMaterialGroup'}de{'TODO: TaxMDMaterialGroup'}")]
        public TaxMDMaterialGroup SelectedTaxMDMaterialGroup
        {
            get
            {
                return _SelectedTaxMDMaterialGroup;
            }
            set
            {
                if (_SelectedTaxMDMaterialGroup != value)
                {
                    _SelectedTaxMDMaterialGroup = value;
                    OnPropertyChanged("SelectedTaxMDMaterialGroup");
                }
            }
        }


        private List<TaxMDMaterialGroup> _TaxMDMaterialGroupList;
        /// <summary>
        /// List property for TaxMDMaterialGroup
        /// </summary>
        /// <value>The TaxMDMaterialGroup list</value>
        [ACPropertyList(9999, TaxMDMaterialGroup.ClassName)]
        public List<TaxMDMaterialGroup> TaxMDMaterialGroupList
        {
            get
            {
                return _TaxMDMaterialGroupList;
            }
            set
            {
                _TaxMDMaterialGroupList = value;
                OnPropertyChanged("TaxMDMaterialGroupList");
            }
        }
        #endregion

        #region TaxMDMaterialGroup -> Methods

        private List<TaxMDMaterialGroup> LoadTaxMDMaterialGroupList()
        {
            if (SelectedTax == null) return null;
            return SelectedTax.TaxMDMaterialGroup_Tax.OrderBy(c => c.MDMaterialGroup.MDKey).ToList();
        }

        [ACMethodInfo(TaxMDMaterialGroup.ClassName, "en{'New Position'}de{'Neue Position'}", 999)]
        public void AddTaxMDMaterialGroup()
        {
            TaxMDMaterialGroup entity = TaxMDMaterialGroup.NewACObject(DatabaseApp, SelectedTax);
            TaxMDMaterialGroupList.Add(entity);
            OnPropertyChanged("TaxMDMaterialGroupList");
            SelectedTaxMDMaterialGroup = entity;
        }

        [ACMethodInfo(TaxMDMaterialGroup.ClassName, "en{'Delete Position'}de{'Position löschen'}", 999)]
        public void DeleteTaxMDMaterialGroup()
        {
            TaxMDMaterialGroupList.Remove(SelectedTaxMDMaterialGroup);
            Msg msg = SelectedTaxMDMaterialGroup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                TaxMDMaterialGroupList.Add(SelectedTaxMDMaterialGroup);
                return;
            }
            else
            {
                SelectedTaxMDMaterialGroup = TaxMDMaterialGroupList != null ? TaxMDMaterialGroupList.FirstOrDefault() : null;
                OnPropertyChanged("TaxMDMaterialGroupList");
            }
        }


        public bool IsEnabledAddTaxMDMaterialGroup()
        {
            return SelectedTax != null;
        }

        public bool IsEnabledDeleteTaxMDMaterialGroup()
        {
            return SelectedTax != null && SelectedTaxMDMaterialGroup != null;
        }

        #endregion

        #endregion

        #region TaxMaterial

        #region TaxMaterial -> Properties

        private TaxMaterial _SelectedTaxMaterial;
        /// <summary>
        /// Selected property for TaxMaterial
        /// </summary>
        /// <value>The selected TaxMaterial</value>
        [ACPropertySelected(9999, TaxMaterial.ClassName, "en{'TODO: TaxMaterial'}de{'TODO: TaxMaterial'}")]
        public TaxMaterial SelectedTaxMaterial
        {
            get
            {
                return _SelectedTaxMaterial;
            }
            set
            {
                if (_SelectedTaxMaterial != value)
                {
                    _SelectedTaxMaterial = value;
                    OnPropertyChanged("SelectedTaxMaterial");
                }
            }
        }


        private List<TaxMaterial> _TaxMaterialList;
        /// <summary>
        /// List property for TaxMaterial
        /// </summary>
        /// <value>The TaxMaterial list</value>
        [ACPropertyList(9999, TaxMaterial.ClassName)]
        public List<TaxMaterial> TaxMaterialList
        {
            get
            {
                return _TaxMaterialList;
            }
            set
            {
                _TaxMaterialList = value;
            }
        }

        #endregion

        #region TaxMaterial -> Methods

        private List<TaxMaterial> LoadTaxMaterialList()
        {
            if (SelectedTax == null) return null;
            return SelectedTax.TaxMaterial_Tax.OrderBy(c => c.Material.MaterialNo).ToList();
        }

        [ACMethodInfo(TaxMaterial.ClassName, "en{'New Position'}de{'Neue Position'}", 999)]
        public void AddTaxMaterial()
        {
            TaxMaterial entity = TaxMaterial.NewACObject(DatabaseApp, SelectedTax);
            TaxMaterialList.Add(entity);
            OnPropertyChanged("TaxMaterialList");
            SelectedTaxMaterial = entity;
        }

        [ACMethodInfo(TaxMaterial.ClassName, "en{'Delete Position'}de{'Position löschen'}", 999)]
        public void DeleteTaxMaterial()
        {
            TaxMaterialList.Remove(SelectedTaxMaterial);
            Msg msg = SelectedTaxMaterial.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                TaxMaterialList.Add(SelectedTaxMaterial);
                return;
            }
            else
            {
                SelectedTaxMaterial = TaxMaterialList != null ? TaxMaterialList.FirstOrDefault() : null;
                OnPropertyChanged("TaxMaterialList");
            }
        }

        public bool IsEnabledAddTaxMaterial()
        {
            return SelectedTax != null;
        }

        public bool IsEnabledDeleteTaxMaterial()
        {
            return SelectedTax != null && SelectedTaxMaterial != null;
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

        #region Methods
        [ACMethodCommand(Tax.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(Tax.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(Tax.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedTax", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Tax>(requery, () => SelectedTax, () => SelectedTax, c => SelectedTax = c,
                        DatabaseApp.Tax
                        .Include(c => c.TaxMDMaterialGroup_Tax)
                        .Include(c => c.TaxMaterial_Tax)
                        .Where(c => c.TaxID == SelectedTax.TaxID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedTax != null;
        }

        [ACMethodInteraction(Tax.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedTax", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Tax), Tax.NoColumnName, Tax.FormatNewNo, this);
            SelectedTax = Tax.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.Tax.AddObject(SelectedTax);
            OnPropertyChanged("TaxList");
            ACState = Const.SMNew;
            PostExecute("New");
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(Tax.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedTax", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            SelectedTax.TaxMaterial_Tax.Clear();
            SelectedTax.TaxMDMaterialGroup_Tax.Clear();
            Msg msg = SelectedTax.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(SelectedTax);
            SelectedTax = AccessPrimary.NavList.FirstOrDefault();
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
            if (SelectedTax != null)
            {
                // Error50400. StartTime, EndTime
                bool isInvalidPeriod = SelectedTax.DateTo != null && SelectedTax.DateTo <= SelectedTax.DateFrom;
                if (isInvalidPeriod)
                {
                    result = new Msg()
                    {
                        Message = Root.Environment.TranslateMessage(this, "Error50401", SelectedTax.DateFrom, SelectedTax.DateTo),
                        MessageLevel = eMsgLevel.Error
                    };
                }
                else
                {
                    // Error50398. TaxNo
                    Tax concurentTax =
                        DatabaseApp
                        .Tax
                        .Where(c =>
                                    c.DateFrom <= SelectedTax.DateFrom && (c.DateTo ?? DateTime.Now) >= SelectedTax.DateFrom
                                    || c.DateFrom >= (SelectedTax.DateTo ?? DateTime.Now) && (c.DateTo ?? DateTime.Now) <= (SelectedTax.DateTo ?? DateTime.Now)
                                )
                        .FirstOrDefault();
                    if (concurentTax != null)
                    {
                        result = new Msg()
                        {
                            Message = Root.Environment.TranslateMessage(this, "Error50398", concurentTax.TaxNo),
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
