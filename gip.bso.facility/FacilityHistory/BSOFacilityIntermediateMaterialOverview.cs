// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.facility
{

    /// <summary>
    /// Businessobject or App for displaying an overview of intermediate materials in stock.
    /// For every intermediate material (a target material of a material workflow relation that is not itself a source material) the overview shows the material, the parent material of the partslist (VMaterial), the current stock quantity and the inward/outward booking quantities of the current day.
    /// The list is read-only and is built by a precompiled query over MaterialWFRelation, Partslist and FacilityCharge; bins whose facility number equals the ExcludedFacilityBins value are excluded.
    /// On start the Search() method is called automatically, which fills the IntermediateMaterialOverviewList property and selects the first row in SelectedIntermediateMaterialOverview.
    /// The Load() method re-executes the search when the requery parameter is true.
    /// This BSO has no New(), Save(), Delete() or navigation methods, because the overview is a pure read-only report without editable records.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityIntermediateMaterialOverview&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Intermediate Material Overview'}de{'Zwischenmaterial Übersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"Businessobject or App for displaying an overview of intermediate materials in stock.
        For every intermediate material (a target material of a material workflow relation that is not itself a source material) the overview shows the material, the parent material of the partslist (VMaterial), the current stock quantity and the inward/outward booking quantities of the current day.
        The list is read-only and is built by a precompiled query over MaterialWFRelation, Partslist and FacilityCharge; bins whose facility number equals the ExcludedFacilityBins value are excluded.
        On start the Search() method is called automatically, which fills the IntermediateMaterialOverviewList property and selects the first row in SelectedIntermediateMaterialOverview.
        The Load() method re-executes the search when the requery parameter is true.
        This BSO has no New(), Save(), Delete() or navigation methods, because the overview is a pure read-only report without editable records.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityIntermediateMaterialOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
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

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._SelectedIntermediateMaterialOverview = null;
            this._IntermediateMaterialOverviewList = null;
            return await base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region PrecompiledQueries

        static readonly Func<DatabaseApp, string, IEnumerable<IntermediateMaterialOverview>> s_cQry_IntermediateMaterialOverview =
            (ctx, excludedBins) =>
                ctx.MaterialWFRelation
                .AsEnumerable()
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
                    }).AsEnumerable();

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
        [ACPropertySelected(601, "IntermediateMaterialOverview", "en{'Intermediate Material'}de{'Zwischenmaterial'}", Description = "The row currently selected in the overview grid; automatically preset with the first row after Search().")]
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
        [ACPropertyList(602, "IntermediateMaterialOverview", Description = "The read-only overview rows; filled by Search() with one row per intermediate material in stock (material, parent material, stock quantity, day inward/outward).")]
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

        [ACMethodCommand("IntermediateMaterialOverview", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Rebuilds the IntermediateMaterialOverviewList from the database and selects the first row. Called automatically on start.")]
        public void Search()
        {
            _IntermediateMaterialOverviewList = GetIntermediateMaterialOverviewList();
            if (_IntermediateMaterialOverviewList != null)
                SelectedIntermediateMaterialOverview = _IntermediateMaterialOverviewList.FirstOrDefault();
            OnPropertyChanged("IntermediateMaterialOverviewList");

        }


        [ACMethodInteraction("IntermediateMaterialOverview", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaterialStock", Description = "Refreshes the overview; re-executes the search when the requery parameter is true.")]
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

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(Search):
                case nameof(Load):
                    return new string[] { };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}
