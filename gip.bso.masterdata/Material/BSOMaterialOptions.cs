using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using System.Collections.ObjectModel;
using gip.mes.facility;

namespace gip.bso.masterdata
{
    public partial class BSOMaterial
    {
        private MaterialWF _SelectedMaterialWF;
        [ACPropertySelected(9999, MaterialWF.ClassName, "en{'Material workflow'}de{'Material workflow'}")]
        public MaterialWF SelectedMaterialWF
        {
            get
            {
                return _SelectedMaterialWF;
            }
            set
            {
                if (_SelectedMaterialWF != value)
                {
                    _SelectedMaterialWF = value;
                    OnPropertyChanged();
                    if (value != null)
                    {
                        IntermediateProductsList.Clear();
                        foreach (Material mat in value.MaterialWFRelation_MaterialWF.Select(c => c.TargetMaterial)
                                                               .Concat(SelectedMaterialWF.MaterialWFRelation_MaterialWF.Select(x => x.SourceMaterial)).Distinct())
                        {
                            IntermediateProductsList.Add(mat);
                        }

                        OnPropertyChanged(nameof(SelectedIntermediateProduct));
                    }
                    else
                    {
                        IntermediateProductsList.Clear();
                        foreach (Material mat in DatabaseApp.Material.Where(c => c.IsIntermediate))
                        {
                            IntermediateProductsList.Add(mat);
                        }

                    }
                }
            }
        }

        [ACPropertyList(9999, MaterialWF.ClassName)]
        public IEnumerable<MaterialWF> MaterialWFList
        {
            get
            {
                return DatabaseApp.MaterialWF.OrderBy(x => x.Name);
            }
        }

        private Material _SelectedIntermediateProduct;
        [ACPropertySelected(9999, "IntermediateProduct", "en{'Intermediate product'}de{'Zwischenprodukt'}")]
        public Material SelectedIntermediateProduct
        {
            get => _SelectedIntermediateProduct;
            set
            {
                _SelectedIntermediateProduct = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Material> _IntermediateProductsList;

        [ACPropertyList(9999, "IntermediateProduct")]
        public ObservableCollection<Material> IntermediateProductsList
        {
            get
            {
                return _IntermediateProductsList;
            }
            set
            {
                _IntermediateProductsList = value;
                OnPropertyChanged();
            }
        }

        private Material _SelectedNewIntermediateProduct;
        [ACPropertySelected(9999, "IntermediateProduct", "en{'Move to intermediate product'}de{'Umzug nach Zwischenprodukt'}")]
        public Material SelectedNewIntermediateProduct
        {
            get => _SelectedNewIntermediateProduct;
            set
            {
                _SelectedNewIntermediateProduct = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyList(9999, "IntermediateProduct")]
        public IEnumerable<Material> IntermediateNewProductList
        {
            get
            {
                return IntermediateProductsList;
            }
        }

        ACAccessNav<Material> _AccessReplacementMaterial;
        [ACPropertyAccess(200, "ReplacementMaterial")]
        public ACAccessNav<Material> AccessReplacementMaterial
        {
            get
            {
                if (_AccessReplacementMaterial == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Material", Material.ClassName);
                    navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, nameof(Material.IsIntermediate), Global.LogicalOperators.equal, Global.Operators.and, "False", false));
                    _AccessReplacementMaterial = navACQueryDefinition.NewAccessNav<Material>("ReplacementMaterial", this);
                    _AccessReplacementMaterial.AutoSaveOnNavigation = false;
                }
                return _AccessReplacementMaterial;
            }
        }

        /// <summary>
        /// Gets or sets the selected FilterInventoryPosMaterial.
        /// </summary>
        /// <value>The selected FilterInventoryPosMaterial.</value>
        [ACPropertySelected(201, "ReplacementMaterial", "en{'and replace with another material'}de{'and replace with another material'}")]
        public Material SelectedReplacementMaterial
        {
            get
            {
                if (AccessReplacementMaterial == null)
                    return null;
                return AccessReplacementMaterial.Selected;
            }
            set
            {
                if (AccessReplacementMaterial == null)
                    return;
                AccessReplacementMaterial.Selected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the FilterInventoryPosMaterial list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(202, "ReplacementMaterial")]
        public IEnumerable<Material> ReplacementMaterialList
        {
            get
            {
                if (AccessReplacementMaterial == null)
                    return null;
                return AccessReplacementMaterial.NavList;
            }
        }

        private bool _PerformOpertionsOnProductionOrders;
        [ACPropertyInfo(9999,"", "en{'Perform operations on the production orders'}de{'Führen Sie Operationen auch an den Produktionsaufträgen durch'}")]
        public bool PerformOpertionsOnProductionOrders
        {
            get => _PerformOpertionsOnProductionOrders;
            set
            {
                _PerformOpertionsOnProductionOrders = value;
                OnPropertyChanged();
            }
        }

        private bool _SelectUnSelectAll = false;
        [ACPropertyInfo(9999)]
        public bool SelectUnSelectAll
        {
            get => _SelectUnSelectAll;
            set
            {
                _SelectUnSelectAll = value;

                if (value)
                {
                    SelectAll();
                }
                else
                {
                    UnSelectAll();
                }
                OnPropertyChanged();
            }
        }

        [ACMethodInfo("", "en{'Show material options'}de{'Materialoptionen anzeigen'}", 9999, true)]
        public void ShowMaterialOptions()
        {
            SelectedTargetMaterial = null;
            SelectedReplacementMaterial = null;
            ShowDialog(this, "MaterialOptionsDialog");
        }

        public bool IsEnabledShowMaterialOptions()
        {
            return
                CurrentMaterial != null
                && AssociatedPartslistPosList != null
                && AssociatedPartslistPosList.Any();
        }


        [ACMethodInfo("", "en{'Move'}de{'Move'}", 9999, true)]
        public void MoveToAnotherIntermediate()
        {
            if (Messages.Question(this, "Question50088") == Global.MsgResult.Yes)
            {
                var msgDetails = RunMaterialOperations(true);

                if (msgDetails.MsgDetailsCount > 0)
                {
                    Messages.Msg(msgDetails);
                    return;
                }

                if (PerformOpertionsOnProductionOrders)
                    RunMaterialOperationsOnProdOrders(true);
            }
        }

        public bool IsEnabledMoveToAnotherIntermediate()
        {
            return CurrentMaterial != null
                && !CurrentMaterial.IsIntermediate
                && AssociatedPartslistPosList != null
                && AssociatedPartslistPosList.Any()
                && SelectedNewIntermediateProduct != null;
        }

        [ACMethodInfo("", "en{'Move and replace material'}de{'Move and replace material'}", 9999, true)]
        public void MoveAndReplaceMaterial()
        {
            if (Messages.Question(this, "Question50088") == Global.MsgResult.Yes)
            {
                var msgDetails = RunMaterialOperations(false);

                if (msgDetails.MsgDetailsCount > 0)
                {
                    Messages.Msg(msgDetails);
                    return;
                }

                if (PerformOpertionsOnProductionOrders)
                    RunMaterialOperationsOnProdOrders(false);
            }
        }

        public bool IsEnabledMoveAndReplaceMaterial()
        {
            return CurrentMaterial != null
                && !CurrentMaterial.IsIntermediate
                && AssociatedPartslistPosList != null
                && AssociatedPartslistPosList.Any()
                && SelectedNewIntermediateProduct != null
                && SelectedReplacementMaterial != null && !SelectedReplacementMaterial.IsIntermediate;
        }

        private MsgWithDetails RunMaterialOperations(bool isOnlyChangeIntermediate)
        {
            Material matNew = isOnlyChangeIntermediate ? null : DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == SelectedReplacementMaterial.MaterialNo);
            Material intMatNew = DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == SelectedNewIntermediateProduct.MaterialNo); ;

            var partslistToSwitch = AssociatedPartslistPosList.Where(c => c.IsChecked).Select(c => c.Partslist).Distinct().ToList();

            ACPartslistManager plManager = null;

            MsgWithDetails msgDetails = new MsgWithDetails();

            foreach (Partslist pl in partslistToSwitch)
            {
                var pos = pl.PartslistPos_Partslist.FirstOrDefault(c => c.Material.MaterialNo == CurrentMaterial.MaterialNo);
                if (pos == null)
                    continue;

                if (matNew != null)
                {
                    pos.Material = matNew;
                }

                PartslistPosRelation[] relations;

                string matNoForCompare = matNew != null ? matNew.MaterialNo : CurrentMaterial.MaterialNo;

                if (SelectedIntermediateProduct != null)
                {
                    relations = pos.PartslistPosRelation_SourcePartslistPos.Where(c => c.TargetPartslistPos.Material.MaterialNo == SelectedIntermediateProduct.MaterialNo
                                                                                           && c.SourcePartslistPos.Material.MaterialNo == matNoForCompare).ToArray();
                }
                else
                {
                    relations = pos.PartslistPosRelation_SourcePartslistPos.Where(c => c.SourcePartslistPos.Material.MaterialNo == matNoForCompare).ToArray();
                }

                foreach (var relation in relations)
                {
                    if (intMatNew != null)
                    {
                        var intPos = pl.PartslistPos_Partslist.FirstOrDefault(c => c.MaterialID == intMatNew.MaterialID);
                        if (intPos == null)
                        {
                            continue;
                        }

                        relation.TargetPartslistPos = intPos;
                    }
                }

                Msg msg = DatabaseApp.ACSaveChanges();
                if (msg != null)
                {
                    msgDetails.AddDetailMessage(msg);
                }

                if (plManager == null)
                {
                    plManager = ACPartslistManager.GetServiceInstance(this);
                }
                if (plManager != null)
                {
                    plManager.RecalcIntermediateSum(pl);
                }

                msg = DatabaseApp.ACSaveChanges();
                if (msg != null)
                {
                    msgDetails.AddDetailMessage(msg);
                }
            }

            return msgDetails;


            // }
        }

        private void RunMaterialOperationsOnProdOrders(bool isOnlyChangeIntermediate)
        {
            Material matNew = isOnlyChangeIntermediate ? null : DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == SelectedReplacementMaterial.MaterialNo);
            Material intMatNew = DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == SelectedNewIntermediateProduct.MaterialNo); ;

            var partslistToSwitch = AssociatedPartslistPosList.Where(c => c.IsChecked).Select(c => c.Partslist).Distinct().ToArray();

            var prodOrders = partslistToSwitch.SelectMany(c => c.ProdOrderPartslist_Partslist)
                                              .Where(x => x.MDProdOrderState.ProdOrderState == MDProdOrderState.ProdOrderStates.NewCreated
                                                       && x.ProdOrder.MDProdOrderState.ProdOrderState == MDProdOrderState.ProdOrderStates.NewCreated).ToArray();

            MsgWithDetails msgDetails = new MsgWithDetails();
            ACProdOrderManager prodOrderManager = null;

            foreach (ProdOrderPartslist poPls in prodOrders)
            {
                var pos = poPls.ProdOrderPartslistPos_ProdOrderPartslist .FirstOrDefault(c => c.Material.MaterialNo == CurrentMaterial.MaterialNo);
                if (pos == null)
                    continue;

                if (matNew != null)
                {
                    pos.Material = matNew;
                }

                ProdOrderPartslistPosRelation[] relations;

                string matNoForCompare = matNew != null ? matNew.MaterialNo : CurrentMaterial.MaterialNo;

                if (SelectedIntermediateProduct != null)
                {
                    relations = pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                                   .Where(c => c.TargetProdOrderPartslistPos.Material.MaterialNo == SelectedIntermediateProduct.MaterialNo
                                            && c.SourceProdOrderPartslistPos.Material.MaterialNo == matNoForCompare)
                                   .Select(c => c.TopParentPartslistPosRelation).ToArray();
                }
                else
                {
                    relations = pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                                   .Where(c => c.SourceProdOrderPartslistPos.Material.MaterialNo == matNoForCompare)
                                   .Select(c => c.TopParentPartslistPosRelation).ToArray();
                }

                ProdOrderPartslistPos intPos = null;
                if (intMatNew != null)
                    intPos = poPls.ProdOrderPartslistPos_ProdOrderPartslist.FirstOrDefault(c => c.MaterialID == intMatNew.MaterialID);

                foreach (var relation in relations)
                {
                    if (intPos != null)
                    {
                        relation.TargetProdOrderPartslistPos = intPos;
                    }
                }

                Msg msg = DatabaseApp.ACSaveChanges();
                if (msg != null)
                {
                    msgDetails.AddDetailMessage(msg);
                }

                if (intPos != null)
                {
                    if (prodOrderManager == null)
                    {
                        prodOrderManager = ACProdOrderManager.GetServiceInstance(this);
                    }

                    if (prodOrderManager != null)
                    {
                        prodOrderManager.RecalcIntermediateItem(intPos, true);
                    }

                    msg = DatabaseApp.ACSaveChanges();
                    if (msg != null)
                    {
                        msgDetails.AddDetailMessage(msg);
                    }
                }

                if (msgDetails.MsgDetailsCount > 0)
                {
                    Messages.Msg(msgDetails);
                    return;
                }

            }
        }

        private void SelectAll()
        {
            if (AssociatedPartslistPosList == null)
                return;

            foreach(var item in AssociatedPartslistPosList)
            {
                item.IsChecked = true;
            }
        }

        private void UnSelectAll()
        {
            if (AssociatedPartslistPosList == null)
                return;

            foreach (var item in AssociatedPartslistPosList)
            {
                item.IsChecked = false;
            }
        }

        #region AssociatedPartslistPos -> Material replacement

        #region AssociatedPartslistPos -> Material replacement-> Material (TargetMaterial)

        ACAccessNav<Material> _AccessTargetMaterial;
        [ACPropertyAccess(200, "TargetMaterial")]
        public ACAccessNav<Material> AccessTargetMaterial
        {
            get
            {
                if (_AccessTargetMaterial == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Material", Material.ClassName);
                    _AccessTargetMaterial = navACQueryDefinition.NewAccessNav<Material>("TargetMaterial", this);
                    _AccessTargetMaterial.AutoSaveOnNavigation = false;
                }
                return _AccessTargetMaterial;
            }
        }

        /// <summary>
        /// Gets or sets the selected FilterInventoryPosMaterial.
        /// </summary>
        /// <value>The selected FilterInventoryPosMaterial.</value>
        [ACPropertySelected(201, "TargetMaterial", "en{'Replace with another material'}de{'Replace with another material'}")]
        public Material SelectedTargetMaterial
        {
            get
            {
                if (AccessTargetMaterial == null)
                    return null;
                return AccessTargetMaterial.Selected;
            }
            set
            {
                if (AccessTargetMaterial == null)
                    return;
                AccessTargetMaterial.Selected = value;
                OnPropertyChanged("SelectedFilterInventoryPosMaterial");
            }
        }

        /// <summary>
        /// Gets the FilterInventoryPosMaterial list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(202, "TargetMaterial")]
        public IEnumerable<Material> TargetMaterialList
        {
            get
            {
                if (AccessTargetMaterial == null)
                    return null;
                return AccessTargetMaterial.NavList;
            }
        }

        #endregion


        /// <summary>
        /// Source Property: ReplaceMaterial
        /// </summary>
        [ACMethodInfo("ReplaceMaterial", "en{'Replace material'}de{'Material ersetzen'}", 999)]
        public void ReplaceMaterial()
        {
            if (!IsEnabledReplaceMaterial())
                return;
            if (Root.Messages.Question(this, "Question50080", Global.MsgResult.No, false, SelectedTargetMaterial.MaterialNo, SelectedTargetMaterial.MaterialName1) == Global.MsgResult.Yes)
            {
                Msg msg = null;

                PartslistPos[] positions = AssociatedPartslistPosList.Where(c => c.IsChecked).ToArray();
                if (positions.Any())
                {
                    foreach (PartslistPos pos in positions)
                        pos.Material = SelectedTargetMaterial;

                    msg = DatabaseApp.ACSaveChanges();
                    SearchAssociatedPos();

                    if (msg == null && PerformOpertionsOnProductionOrders)
                    {
                        var POPos = positions.SelectMany(c => c.ProdOrderPartslistPos_BasedOnPartslistPos)
                                             .Where(c => c.ProdOrderPartslist.MDProdOrderState.ProdOrderState == MDProdOrderState.ProdOrderStates.NewCreated
                                                     &&  c.ProdOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState == MDProdOrderState.ProdOrderStates.NewCreated).ToArray();

                        foreach (ProdOrderPartslistPos pos in POPos)
                            pos.Material = SelectedTargetMaterial;

                        msg = DatabaseApp.ACSaveChanges();
                    }
                }
            }
        }

        public bool IsEnabledReplaceMaterial()
        {
            return
                CurrentMaterial != null
                && SelectedTargetMaterial != null
                && CurrentMaterial.MaterialID != SelectedTargetMaterial.MaterialID
                && AssociatedPartslistPosList != null
                && AssociatedPartslistPosList.Any();
        }


        #endregion
    }
}
