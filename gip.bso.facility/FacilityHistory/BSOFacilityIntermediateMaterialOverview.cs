using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{

    [ACClassInfo(Const.PackName_VarioFacility, "en{'Intermediate Material Overview'}de{'Zwischenmaterial Übersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOFacilityIntermediateMaterialOverview : ACBSOvbNav
    {

        #region constants

        public virtual string ExcludedFacilityBins
        {
            get
            {
                return "not-defined";
            }
        }

        #endregion

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityMaterialOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityIntermediateMaterialOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._SelectedIntermediateMaterialOverview = null;
            this._IntermediateMaterialOverviewList = null;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region PrecompiledQueries

        static readonly Func<DatabaseApp, string, IQueryable<IntermediateMaterialOverview>> s_cQry_IntermediateMaterialOverview =
        CompiledQuery.Compile<DatabaseApp, string, IQueryable<IntermediateMaterialOverview>>(
            (ctx, excludedBins) =>
                ctx.MaterialWFRelation
                .Select(x => x.TargetMaterial)
                .Where(x => !x.MaterialWFRelation_SourceMaterial.Any())
                .Select(x => x.MaterialID)
                .Distinct()
                .Join(ctx.Material, Id => Id, Mt => Mt.MaterialID, (Id, Mt) => new { Material = Mt })
                .Join(ctx.MaterialWFRelation, Mt => Mt.Material.MaterialID, Rel => Rel.TargetMaterialID, (Mt, Rel) => new { Material = Mt.Material, MaterialWFID = Rel.MaterialWFID })
                .Join(ctx.Partslist, Mt => Mt.MaterialWFID, Pl => Pl.MaterialWFID, (Mt, Pl) => new { Material = Mt.Material, Partslist = Pl })
                .GroupBy(x => new { x.Material.MaterialNo, x.Material.MaterialName1, VMaterialNo = x.Partslist.Material.MaterialNo, VMaterialName1 = x.Partslist.Material.MaterialName1, Pt = x.Partslist.PartslistID })
                 .Join(ctx.FacilityCharge,
                         Jo => new { a = Jo.Key.MaterialNo, b = Jo.Key.Pt },
                         Fc => new { a = Fc.Material.MaterialNo, b = (Fc.PartslistID ?? Guid.Empty) },
                         (Jo, Fc) => new { MaterialNo = Jo.Key.MaterialNo, MaterialName1 = Jo.Key.MaterialName1, VMaterialNo = Jo.Key.VMaterialNo, VMaterialName1 = Jo.Key.VMaterialName1, Fc = Fc })
                 .Where(x => !x.Fc.NotAvailable && x.Fc.StockQuantity > 0 && x.Fc.Facility.FacilityNo != excludedBins)
                 .GroupBy(x => new { x.MaterialNo, x.MaterialName1, VMaterialNo = x.VMaterialNo, VMaterialName1 = x.VMaterialName1 })
                  .OrderBy(x => x.Key.VMaterialName1)
                .Select(x =>
                    new IntermediateMaterialOverview()
                    {
                        MaterialName1 = x.Key.MaterialName1,
                        MaterialNo = x.Key.MaterialNo,
                        VMaterialName1 = x.Key.VMaterialName1,
                        VMaterialNo = x.Key.VMaterialNo,
                        StockQuantity = x.Sum(a => (double?)a.Fc.StockQuantity),
                        DayInward = x.SelectMany(fbc => fbc.Fc.FacilityBookingCharge_InwardFacilityCharge).Where(a => a.InsertDate >= DayFilterStart && a.InsertDate < DayFilterEnd).Sum(a => (double?)a.InwardQuantity),
                        DayOutward = x.SelectMany(fbc => fbc.Fc.FacilityBookingCharge_OutwardFacilityCharge).Where(a => a.InsertDate >= DayFilterStart && a.InsertDate < DayFilterEnd).Sum(a => (double?)a.OutwardQuantity),
                    }));

        #endregion

        #region IAccessNav

        public override IAccessNav AccessNav
        {
            get { return null; }
        }

        #endregion

        #region Properties

        #region Properties -> Filter -> Date

        public static DateTime DayFilterStart
        {
            get
            {
                return DateTime.Now.Date;
            }
        }

        public static DateTime DayFilterEnd
        {
            get
            {
                return DateTime.Now.Date.AddDays(1);
            }
        }

        #endregion

        #endregion

        #region IntermediateMaterialOverview -> Select, (Current,) List

        private IntermediateMaterialOverview _SelectedIntermediateMaterialOverview;
        /// <summary>
        /// Selected property for IntermediateMaterialOverview
        /// </summary>
        /// <value>The selected IntermediateMaterialOverview</value>
        [ACPropertySelected(601, "IntermediateMaterialOverview", "en{'TODO: IntermediateMaterialOverview'}de{'TODO: IntermediateMaterialOverview'}")]
        public IntermediateMaterialOverview SelectedIntermediateMaterialOverview
        {
            get
            {
                return _SelectedIntermediateMaterialOverview;
            }
            set
            {
                _SelectedIntermediateMaterialOverview = value;
                OnPropertyChanged("SelectedIntermediateMaterialOverview");
            }
        }

        private IEnumerable<IntermediateMaterialOverview> _IntermediateMaterialOverviewList;
        /// <summary>
        /// List property for IntermediateMaterialOverview
        /// </summary>
        /// <value>The IntermediateMaterialOverview list</value>
        [ACPropertyList(602, "IntermediateMaterialOverview")]
        public IEnumerable<IntermediateMaterialOverview> IntermediateMaterialOverviewList
        {
            get
            {
                if (_IntermediateMaterialOverviewList == null)
                    _IntermediateMaterialOverviewList = new List<IntermediateMaterialOverview>();
                return _IntermediateMaterialOverviewList;
            }
        }

        #endregion

        #region methods

        [ACMethodCommand("IntermediateMaterialOverview", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            _IntermediateMaterialOverviewList = GetIntermediateMaterialOverviewList();
            if (_IntermediateMaterialOverviewList != null)
                SelectedIntermediateMaterialOverview = _IntermediateMaterialOverviewList.FirstOrDefault();
            OnPropertyChanged("IntermediateMaterialOverviewList");

        }


        [ACMethodInteraction("IntermediateMaterialOverview", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaterialStock")]
        public void Load(bool requery = false)
        {
            if (requery)
                Search();
        }

        private List<IntermediateMaterialOverview> GetIntermediateMaterialOverviewList()
        {
            List<IntermediateMaterialOverview> model = s_cQry_IntermediateMaterialOverview(DatabaseApp,ExcludedFacilityBins).ToList();
            int sn = 0;
            model.ForEach(x => x.Sn = ++sn);
            return model;
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"Search":
                    Search();
                    return true;
                case"Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
