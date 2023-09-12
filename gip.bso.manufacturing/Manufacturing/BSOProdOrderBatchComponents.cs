using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.media;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Components list'}de{'Komponentenliste'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + ProdOrderPartslistPosRelation.ClassName)]
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

            MediaController = ACUrlCommand(ACMediaController.MediaControllerPath) as ACMediaController;
            if (MediaController == null)
            {
                throw new Exception($"{ACMediaController.MediaControllerPath} not found!");
            }

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool baseResult = base.ACDeInit(deleteACClassTask);
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
        [ACPropertySelected(9999, "Component", "en{'TODO: Component'}de{'TODO: Component'}")]
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
        [ACPropertyList(9999, "Component")]
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
        [ACPropertyInfo(999, "BatchPlan", "en{'Batch Size'}de{'Batchgröße'}")]
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
        [ACPropertyInfo(999, "Partslist", "en{'TODO:Partslist'}de{'TODO:Partslist'}")]
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
        [ACPropertyInfo(999, "PartslistImage", "en{'TODO:PartslistImage'}de{'TODO:PartslistImage'}")]
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
                databaseApp.ObjectStateManager.ChangeObjectState(pos, System.Data.EntityState.Unchanged);
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

        [ACMethodInfo("Dialog", "en{'Dialog Lot'}de{'Dialog Los'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogComponent(PAOrderInfo paOrderInfo)
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
                ShowDialog(this, "DialogComponent");
                this.ParentACComponent.StopComponent(this);
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
                    ShowDialogComponent((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
