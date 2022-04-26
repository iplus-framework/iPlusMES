using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Material Preparation'}de{'Bereitstellung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOProdOrderOverview : ACBSOvb
    {
        #region ctor's

        public BSOProdOrderOverview(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;


            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);


            return b;
        }

        #endregion

        #region Properties

        #region Properties -> Filter

        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime? _FilterStartDate;
        [ACPropertySelected(999, "FilterStartDate", "en{'Prod. start from'}de{'Auftrag Start von'}")]
        public DateTime? FilterStartDate
        {
            get
            {
                return _FilterStartDate;
            }
            set
            {
                if (_FilterStartDate != value)
                {
                    _FilterStartDate = value;
                    OnPropertyChanged(nameof(FilterStartDate));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime? _FilterEndDate;
        [ACPropertySelected(999, "FilterEndDate", "en{'to'}de{'Bis'}")]
        public DateTime? FilterEndDate
        {
            get
            {
                return _FilterEndDate;
            }
            set
            {
                if (_FilterEndDate != value)
                {
                    _FilterEndDate = value;
                    OnPropertyChanged(nameof(FilterEndDate));
                }
            }
        }

        
        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterProgramNo;
        [ACPropertySelected(999, "FilterProgramNo", "en{'Program No.'}de{'AuftragNr.'}")]
        public string FilterProgramNo
        {
            get
            {
                return _FilterProgramNo;
            }
            set
            {
                if (_FilterProgramNo != value)
                {
                    _FilterProgramNo = value;
                    OnPropertyChanged(nameof(FilterProgramNo));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterMaterialNo;
        [ACPropertySelected(999, "FilterMaterialNo", "en{'Material'}de{'Material'}")]
        public string FilterMaterialNo
        {
            get
            {
                return _FilterMaterialNo;
            }
            set
            {
                if (_FilterMaterialNo != value)
                {
                    _FilterMaterialNo = value;
                    OnPropertyChanged("FilterMaterialNo");
                }
            }
        }

        #endregion

        #region Properties -> OverviewProdOrderPartslist

        private OverviewProdOrderPartslist _SelectedOverviewProdOrderPartslist;
        /// <summary>
        /// Selected property for OverviewProdOrderPartslist
        /// </summary>
        /// <value>The selected OverviewProdOrderPartslist</value>
        [ACPropertySelected(9999, "OverviewProdOrderPartslist", "en{'TODO: OverviewProdOrderPartslist'}de{'TODO: OverviewProdOrderPartslist'}")]
        public OverviewProdOrderPartslist SelectedOverviewProdOrderPartslist
        {
            get
            {
                return _SelectedOverviewProdOrderPartslist;
            }
            set
            {
                if (_SelectedOverviewProdOrderPartslist != value)
                {
                    _SelectedOverviewProdOrderPartslist = value;
                    OnPropertyChanged(nameof(SelectedOverviewProdOrderPartslist));
                }
            }
        }

        private List<OverviewProdOrderPartslist> _OverviewProdOrderPartslistList;
        /// <summary>
        /// List property for OverviewProdOrderPartslist
        /// </summary>
        /// <value>The OverviewProdOrderPartslist list</value>
        [ACPropertyList(9999, "OverviewProdOrderPartslist")]
        public List<OverviewProdOrderPartslist> OverviewProdOrderPartslistList
        {
            get
            {
                return _OverviewProdOrderPartslistList;
            }
        }

        private List<OverviewProdOrderPartslist> LoadOverviewProdOrderPartslistList(DatabaseApp databaseApp, DateTime? filterProdStartDate, DateTime? filterProdEndDate,
            DateTime? filterStartBookingDate, DateTime? filterEndBookingDate, string filterProgramNo, string filterMaterialNo)
        {

            List<OverviewProdOrderPartslist> list = s_cQry_OverviewProdOrderPartslist(databaseApp, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo).ToList();
            foreach (OverviewProdOrderPartslist item in list)
                item.RestQuantityUOM = item.TargetActualQuantityUOM - item.SumComponentsActualQuantity;
            return list;
        }

        #endregion

        #region Properties -> OverviewMaterial

        private OverviewMaterial _SelectedOverviewMaterial;
        /// <summary>
        /// Selected property for OverviewMaterial
        /// </summary>
        /// <value>The selected OverviewMaterial</value>
        [ACPropertySelected(9999, "OverviewMaterial", "en{'TODO: OverviewMaterial'}de{'TODO: OverviewMaterial'}")]
        public OverviewMaterial SelectedOverviewMaterial
        {
            get
            {
                return _SelectedOverviewMaterial;
            }
            set
            {
                if (_SelectedOverviewMaterial != value)
                {
                    _SelectedOverviewMaterial = value;
                    OnPropertyChanged(nameof(SelectedOverviewMaterial));
                }
            }
        }

        private List<OverviewMaterial> _OverviewMaterialList;
        /// <summary>
        /// List property for OverviewMaterial
        /// </summary>
        /// <value>The OverviewMaterial list</value>
        [ACPropertyList(9999, "OverviewMaterial")]
        public List<OverviewMaterial> OverviewMaterialList
        {
            get
            {
                return _OverviewMaterialList;
            }
        }

        private List<OverviewMaterial> LoadOverviewMaterialList(DatabaseApp databaseApp, DateTime? filterProdStartDate, DateTime? filterProdEndDate,
            DateTime? filterStartBookingDate, DateTime? filterEndBookingDate, string filterProgramNo, string filterMaterialNo)
        {
            List<OverviewMaterial> list = s_cQry_OverviewMaterial(databaseApp, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo).ToList();
            foreach (OverviewMaterial item in list)
            {
                item.DifferenceOutwardQuantityUOM = item.SumOutwardActualQuantityUOM - item.SumOutwardTargetQuantityUOM;
                item.DifferenceInwardQuantityUOM = item.SumInwardActualQuantityUOM - item.SumInwardTargetQuantityUOM;
                item.RestQuantityUOM = item.SumInwardActualQuantityUOM - item.SumOutwardActualQuantityUOM;
            }
            return list;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Source Property: Search
        /// </summary>
        [ACMethodInfo("Search", "en{'TODO:MethodName'}de{'TODO:MethodName'}", 999)]
        public void Search()
        {
            if (!IsEnabledSearch())
                return;

            _OverviewProdOrderPartslistList = LoadOverviewProdOrderPartslistList(DatabaseApp, FilterStartDate, FilterEndDate, FilterStartDate, FilterEndDate,
                FilterProgramNo, FilterMaterialNo);
            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));

            _OverviewMaterialList = LoadOverviewMaterialList(DatabaseApp, FilterStartDate, FilterEndDate, FilterStartDate, FilterEndDate,
               FilterProgramNo, FilterMaterialNo);
            OnPropertyChanged(nameof(OverviewMaterialList));
        }

        public bool IsEnabledSearch()
        {
            return true;
        }


        #endregion

        #region Precompiled queries

        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, IQueryable<OverviewProdOrderPartslist>> s_cQry_OverviewProdOrderPartslist =
   CompiledQuery.Compile<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, IQueryable<OverviewProdOrderPartslist>>(
       (ctx, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo) =>
           ctx
           .ProdOrderPartslist
           .Include(c => c.Partslist)
           .Include(c => c.Partslist.Material)
           .Include(c => c.ProdOrderPartslistPos_ProdOrderPartslist)
           .Select(c => new OverviewProdOrderPartslist()
           {
               OrderNo = c.ProdOrder.ProgramNo,
               MaterialNo = c.Partslist.Material.MaterialNo,
               MaterialName = c.Partslist.Material.MaterialName1,
               TargetInwardQuantityUOM = c.TargetQuantity,
               TargetActualQuantityUOM = c.ActualQuantity,
               DifferenceQuantityUOM = c.ActualQuantity - c.TargetQuantity,
               SumComponentsActualQuantity = c.ProdOrderPartslistPos_ProdOrderPartslist.Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot).Sum(x => x.ActualQuantityUOM),
               RestQuantityUOM = 0
           })
        );

        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, IQueryable<OverviewMaterial>> s_cQry_OverviewMaterial =
        CompiledQuery.Compile<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, IQueryable<OverviewMaterial>>(
            (ctx, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo) =>
                ctx
                .ProdOrderPartslist
                .Include(c => c.Partslist)
                .Include(c => c.Partslist.Material)
                .Include(c => c.ProdOrderPartslistPos_ProdOrderPartslist)
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.FacilityBooking_ProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation")
                .GroupBy(c => new { c.Partslist.Material.MaterialNo, c.Partslist.Material.MaterialName1 })
                .Select(c => new OverviewMaterial()
                {
                    MaterialNo = c.Key.MaterialNo,
                    MaterialName = c.Key.MaterialName1,
                    SumOutwardTargetQuantityUOM =
                                    c.SelectMany(x => x.ProdOrderPartslistPos_ProdOrderPartslist)
                                    .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                                    .Sum(x => x.TargetQuantityUOM),
                    SumOutwardActualQuantityUOM =
                                    c.SelectMany(x => x.ProdOrderPartslistPos_ProdOrderPartslist)
                                    .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                    .Sum(x => x.OutwardQuantity),
                    DifferenceOutwardQuantityUOM = 0,
                    SumInwardTargetQuantityUOM =
                                    c.SelectMany(x => x.ProdOrderPartslistPos_ProdOrderPartslist)
                                    .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                                    .Sum(x => x.TargetQuantityUOM),
                    SumInwardActualQuantityUOM =
                                    c.SelectMany(x => x.ProdOrderPartslistPos_ProdOrderPartslist)
                                    .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos)
                                    .Sum(x => x.InwardQuantity),
                    DifferenceInwardQuantityUOM = 0,
                    RestQuantityUOM = 0
                })
        );
        #endregion
    }
}
