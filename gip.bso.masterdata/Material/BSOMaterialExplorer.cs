﻿using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;
using static gip.core.datamodel.Global;
using System.ComponentModel;
using System.Data.Objects;
using System.Windows.Input;
using System;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Material'}de{'Material'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Material.ClassName)]
    public class BSOMaterialExplorer : ACBSOvbNav
    {
        #region helper properties

        public BSOMaterialExplorer _CloneFrom = null;

        public string MediaRootFolder { get; set; }

        public MediaSettings MediaSettings { get; private set; }

        public string MaterialTypeRootFolder { get; private set; }
        #endregion

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOMaterial"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOMaterialExplorer(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            MediaSettings = new MediaSettings();
            Material dummyMaterial = DatabaseApp.Material.FirstOrDefault();
            MediaSettings.LoadTypeFolder(dummyMaterial);
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessAssociatedPartslistPos != null)
            {
                _AccessAssociatedPartslistPos.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;
                if (_AccessAssociatedPartslistPos.NavACQueryDefinition != null)
                    _AccessAssociatedPartslistPos.NavACQueryDefinition.PropertyChanged -= NavACQueryDefinition_PropertyChanged;
                _AccessAssociatedPartslistPos.ACDeInit(false);
                _AccessAssociatedPartslistPos = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty
        #region BSO->ACProperty->Material
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Material> _AccessAssociatedPartslistPos;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, Material.ClassName)]
        public ACAccessNav<Material> AccessPrimary
        {
            get
            {
                if (_AccessAssociatedPartslistPos == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                        _AccessAssociatedPartslistPos = navACQueryDefinition.NewAccessNav<Material>(Material.ClassName, this);
                        _AccessAssociatedPartslistPos.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                        navACQueryDefinition.PropertyChanged += NavACQueryDefinition_PropertyChanged;
                    }
                }
                return _AccessAssociatedPartslistPos;
            }
        }

        protected virtual IQueryable<Material> _AccessPrimary_NavSearchExecuting(IQueryable<Material> result)
        {
            ObjectQuery<Material> query = result as ObjectQuery<Material>;
            if (query != null)
            {
                query
                .Include(c => c.BaseMDUnit)
                .Include(c => c.MDMaterialGroup)
                .Include(c => c.MDMaterialType)
                .Include(c => c.MDGMPMaterialGroup)
                .Include(c => c.MDInventoryManagementType)
                .Include(c => c.MDFacilityManagementType);
            }

            if (FilterMDSchedulingGroupID != null)
            {
                Guid[] linieMaterials = DatabaseApp
                   .MDSchedulingGroup
                   .Where(c => c.MDSchedulingGroupID == FilterMDSchedulingGroupID)
                   .SelectMany(c => c.MDSchedulingGroupWF_MDSchedulingGroup)
                   .Select(c => c.VBiACClassWF.ACClassMethod)
                   .SelectMany(c => c.MaterialWFACClassMethod_ACClassMethod)
                   .SelectMany(c => c.PartslistACClassMethod_MaterialWFACClassMethod)
                   //.Where(c => c.Partslist.IsEnabled)
                   .Select(c => c.Partslist.MaterialID)
                    .Distinct()
                    .ToArray();

                //Guid[] linieMaterials = DatabaseApp
                //                       .MDSchedulingGroup
                //                       .Where(c => c.MDSchedulingGroupID == FilterMDSchedulingGroupID)
                //                       .SelectMany(c => c.MDSchedulingGroupWF_MDSchedulingGroup)
                //                       .Select(c => c.VBiACClassWF)
                //                       .SelectMany(c => c.MaterialWFConnection_ACClassWF)
                //                       .Select(c => c.Material)
                //                       .SelectMany(c => c.PartslistPos_Material)
                //                       .Select(c => c.Partslist)
                //                       .Select(c => c.MaterialID)
                //                       .Distinct()
                //                       .ToArray();
                query = query
                    .Where(c =>
                        c.Partslist_Material.Any(x => x.IsEnabled)
                        //&& x.PartslistACClassMethod_Partslist.FirstOrDefault().MaterialWFACClassMethod.ACClassMethod.ACClassWF_ACClassMethod)
                        && linieMaterials.Contains(c.MaterialID)
                        ) as ObjectQuery<Material>;
            }

            if (FilterIsNotDeleted != null)
            {
                query = query
                    .Where(c => 
                                c.Partslist_Material
                                .Any(x => (x.DeleteDate == null) == (FilterIsNotDeleted ?? false))
                           ) as ObjectQuery<Material>;
            }

            if (FilterIsConnectedWithEnabledPartslist != null)
            {
                query = query.Where(c => c.Partslist_Material.Any(x => x.DeleteDate == null && x.IsEnabled == (FilterIsConnectedWithEnabledPartslist ?? false))) as ObjectQuery<Material>;
            }

            return query;
        }

        private List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem phOpen = new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phOpen);

                ACFilterItem partslistNoFilter = new ACFilterItem(FilterTypes.filter, "MaterialNo", LogicalOperators.contains, Operators.or, "", true, true);
                aCFilterItems.Add(partslistNoFilter);

                ACFilterItem filterPartslistName = new ACFilterItem(FilterTypes.filter, "MaterialName1", LogicalOperators.contains, Operators.or, "", true, true);
                aCFilterItems.Add(filterPartslistName);

                ACFilterItem phClose = new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phClose);

                return aCFilterItems;
            }
        }

        private void NavACQueryDefinition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SearchWord")
            {
                OnPropertyChanged("SearchWord");
            }
        }

        private void OnPropertyChangedFromFilter(string filterPropertyName)
        {
            switch (filterPropertyName)
            {
                case "MaterialNo":
                    OnPropertyChanged("FilterMaterialNo");
                    break;
                case "MaterialName1":
                    OnPropertyChanged("FilterMaterialName");
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the current material.
        /// </summary>
        /// <value>The current material.</value>
        [ACPropertyCurrent(9999, Material.ClassName)]
        public virtual Material CurrentMaterial
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                if (AccessPrimary.Current != value)
                {
                    AccessPrimary.Current = value;
                    OnPropertyChanged("CurrentMaterial");
                }
            }
        }

        public override object Clone()
        {

            BSOMaterialExplorer clone = base.Clone() as BSOMaterialExplorer;
            clone._CloneFrom = this;
            clone.SelectedMaterial = this.SelectedMaterial;
            clone.CurrentMaterial = this.CurrentMaterial;
            return clone;
        }

        /// <summary>
        /// Gets the material list.
        /// </summary>
        /// <value>The material list.</value>
        [ACPropertyList(9999, Material.ClassName)]
        public IList<Material> MaterialList
        {
            get
            {
                List<Material> materials = AccessPrimary.NavList.ToList();
                foreach (var material in materials)
                {
                    MediaSettings.LoadImage(material);
                }
                return materials;
            }
        }

        /// <summary>
        /// Gets or sets the selected material.
        /// </summary>
        /// <value>The selected material.</value>
        [ACPropertySelected(9999, Material.ClassName)]
        public Material SelectedMaterial
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
                    //ChangedSelectedMaterial(value);
                    OnPropertyChanged("SelectedMaterial");
                }
            }
        }

        public virtual void ChangedSelectedMaterial(Material material)
        {

        }

        #endregion

        #endregion

        #region Properties

        public Guid? FilterMDSchedulingGroupID { get; set; }

        public bool? FilterIsConnectedWithEnabledPartslist { get; set; }
        public bool? FilterIsNotDeleted { get; set; }

        [ACPropertyInfo(5, "", "en{'Filter'}de{'Filter'}")]
        public string SearchWord
        {
            get
            {
                if (_AccessAssociatedPartslistPos == null || _AccessAssociatedPartslistPos.NavACQueryDefinition == null) return null;
                return _AccessAssociatedPartslistPos.NavACQueryDefinition.SearchWord;
            }
            set
            {
                if (_AccessAssociatedPartslistPos != null && _AccessAssociatedPartslistPos.NavACQueryDefinition != null)
                {
                    if (_AccessAssociatedPartslistPos.NavACQueryDefinition.SearchWord != value)
                    {
                        _AccessAssociatedPartslistPos.NavACQueryDefinition.SearchWord = value;
                        OnPropertyChanged("SearchWord");
                        if (string.IsNullOrEmpty(value))
                            ClearSearch();
                        else
                            Search();
                    }
                }
            }
        }

        #endregion

        #region BSO->ACMethods

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Search material'}de{'Materialsuche'}", (short)MISort.Search)]
        public virtual void Search()
        {
            AccessPrimary.NavSearch(DatabaseApp, DatabaseApp.RecommendedMergeOption);
            OnPropertyChanged("MaterialList");
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Partslist.ClassName, "en{'Remove'}de{'Entfernen'}", (short)MISort.Search)]
        public void ClearSearch()
        {
            if (!PreExecute("ClearSearch")) return;
            if (AccessPrimary == null) return;
            AccessPrimary.NavList.Clear();
            SelectedMaterial = null;
            CurrentMaterial = null;
            OnPropertyChanged("MaterialList");
            PostExecute("ClearSearch");
            //SearchWord = "";
            //Search();
        }


        [ACMethodInfo("", "en{'Key event'}de{'Tastatur Ereignis'}", 9999, false)]
        public void OnKeyEvent(KeyEventArgs e)
        {
            IVBContent control = e.Source as IVBContent;
            if (control != null && control.VBContent == "SearchWord")
            {
                if (e.Key == Key.Enter)
                {
                    Search();
                }
            }
        }

        #endregion
    }
}