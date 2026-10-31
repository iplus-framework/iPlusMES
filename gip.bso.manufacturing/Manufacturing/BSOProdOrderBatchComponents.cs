// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.media;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Business object or App (dialog) for displaying the components of a batch plan (batch size scaled component list).
    /// It is not a standalone search/edit app: it is opened as a dialog by another component via
    /// ShowDialogComponent(paOrderInfo). The PAOrderInfo must contain a ProdOrderBatchPlan entity; the batch plan is
    /// stored in the BatchPlan property and its root outward components are loaded into the ComponentList property,
    /// with their target quantities scaled from the BOM default quantity to the batch size
    /// (BatchPlan.BatchSize / ProdOrderPartslistPos.TargetQuantityUOM). The selected component is in the
    /// SelectedComponent property, the BOM of the batch plan in the Partslist property and the image of the BOM
    /// material in the PartslistImage property (loaded via LoadMaterialImage()).
    /// The dialog shows three tabs: components (with batch size and production target quantity), recipe description
    /// (Partslist comment) and product image.
    /// This BSO is read-only; there are no New/Save/Delete entity workflows.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOProdOrderBatchComponents&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Components list'}de{'Komponentenliste'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + ProdOrderPartslistPosRelation.ClassName,
        Description = @"Business object or App (dialog) for displaying the components of a batch plan (batch size scaled component list).
        It is not a standalone search/edit app: it is opened as a dialog by another component via ShowDialogComponent(paOrderInfo). The PAOrderInfo must contain a ProdOrderBatchPlan entity; the batch plan is stored in the BatchPlan property and its root outward components are loaded into the ComponentList property, with their target quantities scaled from the BOM default quantity to the batch size (BatchPlan.BatchSize / ProdOrderPartslistPos.TargetQuantityUOM). The selected component is in the SelectedComponent property, the BOM of the batch plan in the Partslist property and the image of the BOM material in the PartslistImage property (loaded via LoadMaterialImage()).
        The dialog shows three tabs: components (with batch size and production target quantity), recipe description (Partslist comment) and product image.
        This BSO is read-only; there are no New/Save/Delete entity workflows.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOProdOrderBatchComponents&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOProdOrderBatchComponents : ACBSOvb
    {

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOProdOrder"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOProdOrderBatchComponents(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            MediaController = ACMediaController.GetServiceInstance(this);
            if (MediaController == null)
            {
                throw new Exception($"{ACMediaController.MediaControllerPath} not found!");
            }

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            bool baseResult = await base.ACDeInit(deleteACClassTask);
            return baseResult;
        }

        #endregion

        #region Properties

        public ACMediaController MediaController { get; set; }  

        #region Properties -> Component

        private ProdOrderPartslistPos _SelectedComponent;
        /// <summary>
        /// Selected property for ProdOrderPartslistPosRelation
        /// </summary>
        /// <value>The selected Component</value>
        [ACPropertySelected(9999, "Component", "en{'Component'}de{'Komponente'}", Description = @"Currently selected component (root outward position of the batch plan) in the component list.")]
        public ProdOrderPartslistPos SelectedComponent
        {
            get
            {
                return _SelectedComponent;
            }
            set
            {
                if (_SelectedComponent != value)
                {
                    _SelectedComponent = value;
                    OnPropertyChanged("SelectedComponent");
                }
            }
        }

        private List<ProdOrderPartslistPos> _ComponentList;
        /// <summary>
        /// List property for ProdOrderPartslistPosRelation
        /// </summary>
        /// <value>The Component list</value>
        [ACPropertyList(9999, "Component", Description = @"Root outward components of the batch plan, ordered by sequence, with target quantities scaled from the BOM default quantity to the batch size. Loaded by ShowDialogComponent().")]
        public List<ProdOrderPartslistPos> ComponentList
        {
            get
            {
                if (_ComponentList == null)
                    _ComponentList = new List<ProdOrderPartslistPos>();
                return _ComponentList;
            }
        }


        #endregion

        /// <summary>
        /// Source Property: 
        /// </summary>
        private ProdOrderBatchPlan _BatchPlan;
        [ACPropertyInfo(999, "BatchPlan", "en{'Batch Size'}de{'Batchgröße'}", Description = @"The batch plan whose components are displayed. Its BatchSize and the target quantity of its production position define the scaling of the component quantities.")]
        public ProdOrderBatchPlan BatchPlan
        {
            get
            {
                return _BatchPlan;
            }
            set
            {
                if (_BatchPlan != value)
                {
                    _BatchPlan = value;
                    OnPropertyChanged(nameof(BatchPlan));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private Partslist _Partslist;
        [ACPropertyInfo(999, "Partslist", "en{'Bill of Materials'}de{'Stückliste'}", Description = @"The bill of material (BOM) of the batch plan; its name, number and comment are shown in the recipe description tab.")]
        public Partslist Partslist
        {
            get
            {
                return _Partslist;
            }
            set
            {
                if (_Partslist != value)
                {
                    _Partslist = value;
                    OnPropertyChanged(nameof(Partslist));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private MediaItemPresentation _PartslistImage;
        [ACPropertyInfo(999, "PartslistImage", "en{'Product image'}de{'Produktbild'}", Description = @"Image of the BOM material (default image of the material media set), shown in the product image tab.")]
        public MediaItemPresentation PartslistImage
        {
            get
            {
                return _PartslistImage;
            }
            set
            {
                if (_PartslistImage != value)
                {
                    _PartslistImage = value;
                    OnPropertyChanged(nameof(PartslistImage));
                }
            }
        }


        #endregion

        #region Methods

        private List<ProdOrderPartslistPos> LoadComponentList(DatabaseApp databaseApp, Guid prodOrderBatchPlanID)
        {
            List<ProdOrderPartslistPos> components = new List<ProdOrderPartslistPos>();
            BatchPlan = null;
            BatchPlan = databaseApp.ProdOrderBatchPlan.FirstOrDefault(c => c.ProdOrderBatchPlanID == prodOrderBatchPlanID);
            if (BatchPlan == null)
            {
                Messages.LogError(this.GetACUrl(), "LoadComponentList()", "batchPlan is null");
                return components;
            }

            components =
                databaseApp
                .ProdOrderPartslistPos
               .Include(x => x.ProdOrderPartslist)
               .Include(x => x.ProdOrderPartslist.ProdOrder)
               .Include(x => x.Material)
               .Include(x => x.Material.BaseMDUnit)
               .Include(x => x.MDUnit)
               .Where(x =>
                       x.ProdOrderPartslistID == BatchPlan.ProdOrderPartslistID
                    && x.AlternativeProdOrderPartslistPosID == null
                    && x.MaterialPosTypeIndex == (short)(GlobalApp.MaterialPosTypes.OutwardRoot)
                    && x.ParentProdOrderPartslistPosID == null
                    && x.AlternativeProdOrderPartslistPosID == null)
                .OrderBy(x => x.Sequence)
                .ToList();

            double quantityIndex = BatchPlan.BatchSize / BatchPlan.ProdOrderPartslistPos.TargetQuantityUOM;
            foreach (ProdOrderPartslistPos pos in components)
            {
                pos.TargetQuantityUOM *= quantityIndex;
                databaseApp.Entry(pos).State = EntityState.Unchanged;
            }

            return components;
        }

        public void LoadMaterialImage(Material material)
        {

            MediaSet imageMediaSet = MediaController.GetMediaSet(material, MediaItemTypeEnum.Image, "", "", true);

            List<MediaItemPresentation> mediaItemPresentations = MediaController.GetFiles(imageMediaSet, 1);

            if (mediaItemPresentations != null && mediaItemPresentations.Any())
            {
                PartslistImage = mediaItemPresentations.Where(c => c.IsDefault).FirstOrDefault();
                if (PartslistImage == null)
                {
                    PartslistImage = mediaItemPresentations.FirstOrDefault();
                }
            }
        }

        [ACMethodInfo("Dialog", "en{'Dialog Lot'}de{'Dialog Los'}", (short)MISort.QueryPrintDlg, Description = @"Opens this BSO as a dialog: reads the ProdOrderBatchPlan entity from the PAOrderInfo, loads the batch-size scaled component list, the BOM and the material image and shows the DialogComponent design. The BSO stops itself after the dialog is closed.")]
        public async Task ShowDialogComponent(PAOrderInfo paOrderInfo)
        {
            if (paOrderInfo == null)
                return;
            PAOrderInfoEntry entry = paOrderInfo.Entities.FirstOrDefault(c => c.EntityName == nameof(ProdOrderBatchPlan));
            if (entry != null)
            {

                try
                {
                    using (DatabaseApp databaseApp = new DatabaseApp())
                    {
                        _ComponentList = LoadComponentList(databaseApp, entry.EntityID);

                        if (BatchPlan != null)
                        {
                            Partslist = BatchPlan.ProdOrderPartslist.Partslist;
                            LoadMaterialImage(Partslist.Material);
                        }
                    }
                }
                catch(Exception ex)
                {
                    this.Messages.LogException(this.GetACUrl(), "ShowDialogComponent(10)", ex);
                }

                OnPropertyChanged(nameof(ComponentList));
                await ShowDialogAsync(this, "DialogComponent");
                await this.ParentACComponent.StopComponent(this);
            }
        }

        #endregion

        #region Overrides

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(ShowDialogComponent):
                    result = ShowDialogComponent((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(ShowDialogComponent):
                    return new string[] { nameof(InitState) };
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion
    }
}
