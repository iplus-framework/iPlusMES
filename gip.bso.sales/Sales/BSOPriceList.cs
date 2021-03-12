using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using gip.bso.masterdata;

namespace gip.bso.sales.Sales
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Price list'}de{'Preisliste'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + PriceList.ClassName)]
    public class BSOPriceList : ACBSOvbNav
    {
        #region c´tors

        public BSOPriceList(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;


            Search();
            LoadPriceListPositions();
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
        ACAccessNav<PriceList> _AccessPrimary;

        [ACPropertyAccessPrimary(690, PriceList.ClassName)]
        public ACAccessNav<PriceList> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<PriceList>(PriceList.ClassName, this);
                }
                return _AccessPrimary;
            }
        }


        [ACPropertyList(601, PriceList.ClassName)]
        public IEnumerable<PriceList> PriceListList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, PriceList.ClassName)]
        public PriceList SelectedPriceList
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
                    OnPropertyChanged("SelectedPriceList");

                    LoadPriceListPositions();
                }
            }
        }

        private void LoadPriceListPositions()
        {
            PriceListMaterialList = LoadPriceListMaterialList();
            if (PriceListMaterialList == null)
                SelectedPriceListMaterial = null;
            else
                SelectedPriceListMaterial = PriceListMaterialList.FirstOrDefault();
        }

        #endregion

        #region PriceListMaterial

        #region PriceListMaterial -> Properties

        private PriceListMaterial _SelectedPriceListMaterial;
        /// <summary>
        /// Selected property for PriceListMaterial
        /// </summary>
        /// <value>The selected PriceListMaterial</value>
        [ACPropertySelected(9999, PriceListMaterial.ClassName, "en{'TODO: PriceListMaterial'}de{'TODO: PriceListMaterial'}")]
        public PriceListMaterial SelectedPriceListMaterial
        {
            get
            {
                return _SelectedPriceListMaterial;
            }
            set
            {
                if (_SelectedPriceListMaterial != value)
                {
                    _SelectedPriceListMaterial = value;
                    OnPropertyChanged("SelectedPriceListMaterial");
                }
            }
        }

        private List<PriceListMaterial> _PriceListMaterialList;
        /// <summary>
        /// List property for PriceListMaterial
        /// </summary>
        /// <value>The PriceListMaterial list</value>
        [ACPropertyList(9999, PriceListMaterial.ClassName)]
        public List<PriceListMaterial> PriceListMaterialList
        {
            get
            {
                return _PriceListMaterialList;
            }
            set
            {
                _PriceListMaterialList = value;
            }
        }

        private List<PriceListMaterial> LoadPriceListMaterialList()
        {
            if (SelectedPriceList == null) return null;
            return SelectedPriceList.PriceListMaterial_PriceList.OrderBy(c => c.Material.MaterialNo).ToList();
        }

        #endregion

        #region PriceListMaterial -> Methods
        [ACMethodInfo(MDCountrySalesTaxMDMaterialGroup.ClassName, "en{'New Position'}de{'Neue Position'}", 999)]
        public void AddPriceListMaterial()
        {
            PriceListMaterial entity = PriceListMaterial.NewACObject(DatabaseApp, SelectedPriceList);
            entity.Material = BSOMaterialExplorer_Child.Value.SelectedMaterial;
            PriceListMaterialList.Add(entity);
            OnPropertyChanged("PriceListMaterialList");
            SelectedPriceListMaterial = entity;
        }

        [ACMethodInfo(MDCountrySalesTaxMDMaterialGroup.ClassName, "en{'Delete Position'}de{'Position löschen'}", 999)]
        public void DeletePriceListMaterial()
        {
            PriceListMaterialList.Remove(SelectedPriceListMaterial);
            Msg msg = SelectedPriceListMaterial.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                PriceListMaterialList.Add(SelectedPriceListMaterial);
                return;
            }
            else
            {
                SelectedPriceListMaterial = PriceListMaterialList != null ? PriceListMaterialList.FirstOrDefault() : null;
                OnPropertyChanged("PriceListMaterialList");
            }
        }

        public bool IsEnabledAddPriceListMaterial()
        {
            return SelectedPriceList != null
                && BSOMaterialExplorer_Child != null
                && BSOMaterialExplorer_Child.Value != null
                && BSOMaterialExplorer_Child.Value.SelectedMaterial != null
                && (PriceListMaterialList == null || !PriceListMaterialList.Where(c => c.MaterialID == BSOMaterialExplorer_Child.Value.SelectedMaterial.MaterialID).Any());
        }

        public bool IsEnabledDeletePriceListMaterial()
        {
            return SelectedPriceList != null && SelectedPriceListMaterial != null;
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
        [ACMethodCommand(PriceList.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(PriceList.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(PriceList.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPriceList", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<PriceList>(requery, () => SelectedPriceList, () => SelectedPriceList, c => SelectedPriceList = c,
                        DatabaseApp.PriceList
                        .Include(c => c.PriceListMaterial_PriceList)
                        .Where(c => c.PriceListID == SelectedPriceList.PriceListID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedPriceList != null;
        }

        [ACMethodInteraction(PriceList.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedPriceList", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(PriceList), PriceList.NoColumnName, PriceList.FormatNewNo, this);
            SelectedPriceList = PriceList.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.PriceList.AddObject(SelectedPriceList);
            OnPropertyChanged("PriceListList");
            ACState = Const.SMNew;
            PostExecute("New");
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(PriceList.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedPriceList", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            SelectedPriceList.PriceListMaterial_PriceList.Clear();
            Msg msg = SelectedPriceList.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(SelectedPriceList);
            SelectedPriceList = AccessPrimary.NavList.FirstOrDefault();
            Load();
        }

        public bool IsEnabledDelete()
        {
            return true;
        }

        [ACMethodCommand(OutOffer.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("PriceList");
        }


        protected override Msg OnPreSave()
        {
            Msg result = null;
            if (SelectedPriceList != null)
            {
                // Error50401. StartTime EndTime

                bool isInvalidPeriod = SelectedPriceList.DateTo != null && SelectedPriceList.DateTo <= SelectedPriceList.DateFrom;
                if (isInvalidPeriod)
                {
                    result = new Msg()
                    {
                        Message = Root.Environment.TranslateMessage(this, "Error50401", SelectedPriceList.DateFrom, SelectedPriceList.DateTo),
                        MessageLevel = eMsgLevel.Error
                    };
                }
                else
                {
                    // Error50399. PriceListNo, MaterialNos, MateralGroupMDkeys
                    string[] currentMaterialNos = SelectedPriceList.PriceListMaterial_PriceList.Select(c => c.Material.MaterialNo).ToArray();
                    PriceList concurentPriceList =
                        DatabaseApp
                        .PriceList
                        .Where(c =>
                                    (
                                        c.DateFrom <= SelectedPriceList.DateFrom && (c.DateTo ?? DateTime.Now) >= SelectedPriceList.DateFrom
                                        || c.DateFrom >= (SelectedPriceList.DateTo ?? DateTime.Now) && (c.DateTo ?? DateTime.Now) <= (SelectedPriceList.DateTo ?? DateTime.Now)
                                    )
                                    && c.PriceListMaterial_PriceList.Select(x => x.Material.MaterialNo).Intersect(currentMaterialNos).Any()
                                )
                       .FirstOrDefault();

                    if (concurentPriceList != null)
                    {
                        List<string> materialNos = concurentPriceList.PriceListMaterial_PriceList.Select(c => c.Material.MaterialNo).Intersect(currentMaterialNos).ToList();
                        result = new Msg()
                        {
                            Message = Root.Environment.TranslateMessage(this, "Error50399", concurentPriceList.PriceListNo, string.Join(",", materialNos)),
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
