using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using gip.bso.masterdata;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.sales.Sales
{
    /// <summary>
    /// Businessobject for managing price lists. A price list (PriceList) defines the sales price of materials
    /// for a validity period (DateFrom / DateTo) and a currency (MDCurrency).
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the PriceListList property.
    /// Then call NavigateFirst() method to set SelectedPriceList with the first record in the list.
    /// SelectedPriceList is used to display and edit the currently selected record. When the selection changes,
    /// the positions of the price list are reloaded into the PriceListMaterialList property (ordered by material number)
    /// and the first position becomes SelectedPriceListMaterial.
    /// Property changes should always be made on SelectedPriceList and when all field values have been changed,
    /// the Save() method should be called to save the changes in the database before navigating to the next record
    /// or creating a new record. Save() validates that DateTo is later than DateFrom (Error50401) and that no other
    /// price list with an overlapping validity period contains the same materials (Error50399).
    /// The UndoSave() method discards all unsaved changes.
    /// The New() method creates a new price list with a new price list number and assigns the new entity object
    /// to the SelectedPriceList property. Enter the number in SelectedPriceList.PriceListNo, the name in
    /// SelectedPriceList.PriceListName, the currency in SelectedPriceList.MDCurrency, the validity period in
    /// SelectedPriceList.DateFrom / SelectedPriceList.DateTo and optionally a comment in SelectedPriceList.Comment.
    /// The Delete() method deletes the selected price list with all its positions and selects the first remaining record.
    /// The Load() method reloads the selected price list with its positions from the database (for background changes by other users).
    /// To define the material prices of a price list the PriceListMaterialList property is used.
    /// Example how to add a material price: call the NewPriceListMaterial() method which creates a new PriceListMaterial,
    /// adds it to the PriceListMaterialList and selects it in SelectedPriceListMaterial. Then assign the material via the
    /// SelectedQueueMaterial property (the QueueMaterialList combo box) and enter the price in SelectedPriceListMaterial.Price.
    /// Alternatively, on the "MaterialAssign" tab select a material in the embedded material explorer
    /// (BSOMaterialExplorer_Child), enter the price in the AssignedMaterialPrice property and call the
    /// AssignMaterialToPriceList() method which creates the position with material and price in one step
    /// (only enabled if the material is not yet in the price list and the price is greater than zero).
    /// The DeletePriceListMaterial() method deletes the position selected in SelectedPriceListMaterial.
    /// Now the Save() method must be called to save the changes in the database.
    /// Visit https://github.com/search?q=org%3Aiplus-framework%20BSOPriceList&type=code for more information.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Price list'}de{'Preisliste'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + PriceList.ClassName,
                 Description = @"Businessobject for managing price lists. A price list (PriceList) defines the sales price of materials
                                for a validity period (DateFrom / DateTo) and a currency (MDCurrency).
                                To search records enter the search string in the SearchWord property.
                                The database result is copied to the PriceListList property.
                                Then call NavigateFirst() method to set SelectedPriceList with the first record in the list.
                                SelectedPriceList is used to display and edit the currently selected record. When the selection changes,
                                the positions of the price list are reloaded into the PriceListMaterialList property (ordered by material number)
                                and the first position becomes SelectedPriceListMaterial.
                                Property changes should always be made on SelectedPriceList and when all field values have been changed,
                                the Save() method should be called to save the changes in the database before navigating to the next record
                                or creating a new record. Save() validates that DateTo is later than DateFrom (Error50401) and that no other
                                price list with an overlapping validity period contains the same materials (Error50399).
                                The UndoSave() method discards all unsaved changes.
                                The New() method creates a new price list with a new price list number and assigns the new entity object
                                to the SelectedPriceList property. Enter the number in SelectedPriceList.PriceListNo, the name in
                                SelectedPriceList.PriceListName, the currency in SelectedPriceList.MDCurrency, the validity period in
                                SelectedPriceList.DateFrom / SelectedPriceList.DateTo and optionally a comment in SelectedPriceList.Comment.
                                The Delete() method deletes the selected price list with all its positions and selects the first remaining record.
                                The Load() method reloads the selected price list with its positions from the database (for background changes by other users).
                                To define the material prices of a price list the PriceListMaterialList property is used.
                                Example how to add a material price: call the NewPriceListMaterial() method which creates a new PriceListMaterial,
                                adds it to the PriceListMaterialList and selects it in SelectedPriceListMaterial. Then assign the material via the
                                SelectedQueueMaterial property (the QueueMaterialList combo box) and enter the price in SelectedPriceListMaterial.Price.
                                Alternatively, on the MaterialAssign tab select a material in the embedded material explorer
                                (BSOMaterialExplorer_Child), enter the price in the AssignedMaterialPrice property and call the
                                AssignMaterialToPriceList() method which creates the position with material and price in one step
                                (only enabled if the material is not yet in the price list and the price is greater than zero).
                                The DeletePriceListMaterial() method deletes the position selected in SelectedPriceListMaterial.
                                Now the Save() method must be called to save the changes in the database.
                                Visit https://github.com/search?q=org%3Aiplus-framework%20BSOPriceList&type=code for more information.")]
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

            if (BSOMaterialExplorer_Child != null && BSOMaterialExplorer_Child.Value != null)
                BSOMaterialExplorer_Child.Value.Search();

            Search();
            LoadPriceListPositions();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            var b = await base.ACDeInit(deleteACClassTask);
            return b;
        }

        #endregion

        #region BSO->ACProperty

        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        ACAccessNav<PriceList> _AccessPrimary;

        [ACPropertyAccessPrimary(690, PriceList.ClassName, Description = "Primary navigation access to the PriceList entities. The search result is available as PriceListList and the selected record as SelectedPriceList.")]
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


        [ACPropertyList(601, PriceList.ClassName, Description = "List of all price lists loaded by the search. Filled by Search() / NavigateFirst().")]
        public IEnumerable<PriceList> PriceListList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, PriceList.ClassName, Description = "Currently selected price list. Changing the selection reloads the positions into PriceListMaterialList. Edit the price list data (PriceListNo, PriceListName, MDCurrency, DateFrom, DateTo, Comment) on this entity and call Save().")]
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
        [ACPropertySelected(9999, PriceListMaterial.ClassName, "en{'Price list position'}de{'Preislistenposition'}", Description = "Currently selected price list position (PriceListMaterial). Assign the material via SelectedQueueMaterial and enter the price in Price.")]
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

                    if (_SelectedPriceListMaterial != null)
                    {
                        AccessQueueMaterial.Selected = _SelectedPriceListMaterial.Material;
                    }
                    else
                        AccessQueueMaterial.Selected = null;
                    OnPropertyChanged("SelectedQueueMaterial");
                }
            }
        }

        private void _SelectedPriceListMaterial_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private List<PriceListMaterial> _PriceListMaterialList;
        /// <summary>
        /// List property for PriceListMaterial
        /// </summary>
        /// <value>The PriceListMaterial list</value>
        [ACPropertyList(9999, PriceListMaterial.ClassName, Description = "All material positions of the selected price list, ordered by material number. Managed with NewPriceListMaterial(), AssignMaterialToPriceList() and DeletePriceListMaterial().")]
        public List<PriceListMaterial> PriceListMaterialList
        {
            get
            {
                return _PriceListMaterialList;
            }
            set
            {
                _PriceListMaterialList = value;
                OnPropertyChanged("PriceListMaterialList");
            }
        }

        private List<PriceListMaterial> LoadPriceListMaterialList()
        {
            if (SelectedPriceList == null) return null;
            return SelectedPriceList.PriceListMaterial_PriceList.OrderBy(c => c.Material.MaterialNo).ToList();
        }

        #endregion

        #region PriceListMaterial -> Methods

        [ACMethodInfo(MDCountrySalesTaxMDMaterialGroup.ClassName, "en{'New Position'}de{'Neue Position'}", 999, Description = "Creates a new empty price list position, adds it to PriceListMaterialList and selects it in SelectedPriceListMaterial.")] 
        public void NewPriceListMaterial()
        {
            PriceListMaterial entity = PriceListMaterial.NewACObject(DatabaseApp, SelectedPriceList);
            PriceListMaterialList.Add(entity);
            OnPropertyChanged("PriceListMaterialList");
            SelectedPriceListMaterial = entity;
        }

        public bool IsEnabledPriceListMaterial()
        {
            return SelectedPriceList != null;
        }

        [ACMethodInfo(MDCountrySalesTaxMDMaterialGroup.ClassName, "en{'New Position'}de{'Neue Position'}", 999, Description = "Creates a new price list position from the material selected in the embedded material explorer and the AssignedMaterialPrice value. Only enabled if the material is not yet in the price list and the price is greater than zero.")] 
        public void  AssignMaterialToPriceList()
        {
            PriceListMaterial entity = PriceListMaterial.NewACObject(DatabaseApp, SelectedPriceList);
            entity.Material = BSOMaterialExplorer_Child.Value.SelectedMaterial;
            entity.Price = AssignedMaterialPrice.Value;
            PriceListMaterialList.Add(entity);
            OnPropertyChanged("PriceListMaterialList");
            SelectedPriceListMaterial = entity;
        }
        public bool IsEnabledAssignMaterialToPriceList()
        {
            return SelectedPriceList != null
                && BSOMaterialExplorer_Child != null
                && BSOMaterialExplorer_Child.Value != null
                && BSOMaterialExplorer_Child.Value.SelectedMaterial != null
                && AssignedMaterialPrice != null
                && AssignedMaterialPrice.Value > 0
                && (PriceListMaterialList == null || !PriceListMaterialList.Where(c => c.MaterialID == BSOMaterialExplorer_Child.Value.SelectedMaterial.MaterialID).Any());
        }

        [ACMethodInfo(MDCountrySalesTaxMDMaterialGroup.ClassName, "en{'Delete Position'}de{'Position löschen'}", 999, Description = "Deletes the price list position selected in SelectedPriceListMaterial from the database.")]
        public void DeletePriceListMaterial()
        {
            PriceListMaterialList.Remove(SelectedPriceListMaterial);
            Msg msg = SelectedPriceListMaterial.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                PriceListMaterialList.Add(SelectedPriceListMaterial);
                return;
            }
            else
            {
                SelectedPriceListMaterial = PriceListMaterialList != null ? PriceListMaterialList.FirstOrDefault() : null;
                OnPropertyChanged("PriceListMaterialList");

                AccessQueueMaterial.NavSearch();
                OnPropertyChanged("QueueMaterialList");
            }
        }

        public bool IsEnabledDeletePriceListMaterial()
        {
            return SelectedPriceList != null && SelectedPriceListMaterial != null;
        }
        #endregion

        #endregion

        #region Material
        #region Material -> BSO
        ACChildItem<BSOMaterialExplorer> _BSOMaterialExplorer_Child;
        [ACPropertyInfo(9999, Description = "Embedded material explorer child component used on the MaterialAssign tab to select a material for AssignMaterialToPriceList().")]
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

        #region Material -> AccessNav

        ACAccessNav<Material> _AccessQueueMaterial;
        [ACPropertyAccess(100, "QueueMaterial", Description = "Access for the material combo box of the price list position.")]
        public ACAccessNav<Material> AccessQueueMaterial
        {
            get
            {
                if (_AccessQueueMaterial == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Material", Material.ClassName);
                    _AccessQueueMaterial = navACQueryDefinition.NewAccessNav<Material>("QueueMaterial", this);
                    _AccessQueueMaterial.NavSearchExecuting += _AccessQueueMaterial_NavSearchExecuting;
                    _AccessQueueMaterial.AutoSaveOnNavigation = false;
                    _AccessQueueMaterial.NavSearch();
                }
                return _AccessQueueMaterial;
            }
        }

        private IQueryable<Material> _AccessQueueMaterial_NavSearchExecuting(IQueryable<Material> result)
        {
            string[] usedMaterials = PriceListMaterialList.Where(c => c.Material != null).Select(x => x.Material.MaterialNo).ToArray();
            return result.Where(c => !usedMaterials.Contains(c.MaterialNo));
        }

        /// <summary>
        /// Gets or sets the selected QueueMaterial.
        /// </summary>
        /// <value>The selected QueueMaterial.</value>
        [ACPropertySelected(101, "QueueMaterial", ConstApp.Material, Description = "Material combo box for the selected price list position. Setting this property assigns the material to SelectedPriceListMaterial.")]
        public Material SelectedQueueMaterial
        {
            get
            {
                if (AccessQueueMaterial == null)
                    return null;
                return AccessQueueMaterial.Selected;
            }
            set
            {
                if (AccessQueueMaterial == null)
                    return;
                if (AccessQueueMaterial.Selected != value)
                {
                    if (SelectedPriceListMaterial != null)
                    {
                        SelectedPriceListMaterial.Material = value;
                        OnPropertyChanged("SelectedPriceListMaterial");
                    }
                    AccessQueueMaterial.Selected = value;
                    OnPropertyChanged("SelectedQueueMaterial");

                    AccessQueueMaterial.NavSearch();
                    OnPropertyChanged("QueueMaterialList");
                }
            }
        }

        /// <summary>
        /// Gets the QueueMaterial list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(102, "QueueMaterial", Description = "List of materials offered in the material combo box for the selected price list position.")]
        public IEnumerable<Material> QueueMaterialList
        {
            get
            {
                if (AccessQueueMaterial == null)
                    return null;
                return AccessQueueMaterial.NavList;
            }
        }

        private decimal? _AssignedMaterialPrice;
        [ACPropertyInfo(750, "AssignedMaterialPrice", "en{'Price'}de{'Preis'}", Description = "Price input for the AssignMaterialToPriceList() method on the MaterialAssign tab. Must be greater than zero.")]
        public decimal? AssignedMaterialPrice
        {
            get
            {
                return _AssignedMaterialPrice;
            }
            set
            {
                if (_AssignedMaterialPrice != value)
                {
                    _AssignedMaterialPrice = value;
                    OnPropertyChanged("AssignedMaterialPrice");
                }
            }
        }

        #endregion

        #endregion

        #region Messages

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged("MsgList");
        }

        #region Messages -> Properties

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(9999, "Message", "en{'Message'}de{'Meldung'}", Description = "Currently displayed message.")]
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
        [ACPropertyList(9999, "Message", "en{'Messagelist'}de{'Meldungsliste'}", Description = "List of messages (e.g. validation or delete errors) collected by the BSO.")]
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
        [ACMethodCommand(PriceList.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves the price list and its positions to the database. Validates that DateTo is after DateFrom (Error50401) and that no other price list with overlapping validity contains the same materials (Error50399).")]
        public void Save()
        {
            OnSave();
            var temp = ACSaveChanges();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(PriceList.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards all unsaved changes on the selected price list and its positions.")]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(PriceList.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPriceList", Global.ACKinds.MSMethodPrePost, Description = "Reloads the selected price list with its positions from the database (for background changes by other users).")]
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

        [ACMethodInteraction(PriceList.ClassName, Const.New, (short)MISort.New, true, "SelectedPriceList", Global.ACKinds.MSMethodPrePost, Description = "Creates a new price list with a new price list number, adds it to PriceListList and selects it in SelectedPriceList.")]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(PriceList), PriceList.NoColumnName, PriceList.FormatNewNo, this);
            SelectedPriceList = PriceList.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.PriceList.Add(SelectedPriceList);
            OnPropertyChanged("PriceListList");
            ACState = Const.SMNew;
            PostExecute("New");
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(PriceList.ClassName, Const.Delete, (short)MISort.Delete, true, "SelectedPriceList", Global.ACKinds.MSMethodPrePost, Description = "Deletes the selected price list with all its positions and selects the first remaining price list.")]
        public void Delete()
        {
            SelectedPriceList.PriceListMaterial_PriceList.Clear();
            Msg msg = SelectedPriceList.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
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

        [ACMethodCommand(OutOffer.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the search with the current query definition and refreshes the PriceListList property.")]
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
                                    c.PriceListID != SelectedPriceList.PriceListID
                                    &&
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
