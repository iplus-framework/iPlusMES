using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'BSOCompanyMaterialOverview'}de{'BSOCompanyMaterialOverview'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]

    public class BSOCompanyMaterialOverview : ACBSOvb
    {

        #region ctor's

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOCompany"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOCompanyMaterialOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit =  base.ACInit(startChildMode);
            FilterCompanyMaterialOverview = CompanyMaterialOverviewTypeEnum.ViewPartnerStockOfUniqueMaterials;
            return baseACInit;
        }

        #endregion

        #region ChildBSO

        ACChildItem<BSOMaterialDetails> _BSOMaterialDetails_Child_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo(nameof(BSOMaterialDetails_Child), typeof(BSOMaterialDetails))]
        public ACChildItem<BSOMaterialDetails> BSOMaterialDetails_Child
        {
            get
            {
                if (_BSOMaterialDetails_Child_Child == null)
                    _BSOMaterialDetails_Child_Child = new ACChildItem<BSOMaterialDetails>(this, nameof(BSOMaterialDetails_Child));
                return _BSOMaterialDetails_Child_Child;
            }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Source Property: 
        /// </summary>
        private Company _CurrentCompany;
        [ACPropertyInfo(999, nameof(CurrentCompany), "en{'TODO:CurrentCompany'}de{'TODO:CurrentCompany'}")]
        public Company CurrentCompany
        {
            get
            {
                return _CurrentCompany;
            }
            set
            {
                if (_CurrentCompany != value)
                {
                    _CurrentCompany = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Properties -> Filter

        public CompanyMaterialOverviewTypeEnum? FilterCompanyMaterialOverview
        {
            get
            {
                if (SelectedFilterCompanyMaterialOverviewType == null)
                {
                    return null;
                }
                else
                {
                    return (CompanyMaterialOverviewTypeEnum)((short)SelectedFilterCompanyMaterialOverviewType.Value);
                }
            }
            set
            {
                if (value != null)
                {
                    _SelectedFilterCompanyMaterialOverviewType = FilterCompanyMaterialOverviewTypeList.FirstOrDefault(c => ((short)c.Value) == (short)value);
                }
                else
                {
                    _SelectedFilterCompanyMaterialOverviewType = null;
                }
                OnPropertyChanged(nameof(SelectedFilterCompanyMaterialOverviewType));
            }
        }

        private const string CompanyMaterialOverviewType = "CompanyMaterialOverviewType";
        //ACValueItem
        // FilterCompanyMaterialOverviewType
        #region FilterCompanyMaterialOverviewType
        private ACValueItem _SelectedFilterCompanyMaterialOverviewType;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected FilterCompanyMaterialOverviewType</value>
        [ACPropertySelected(9999, nameof(CompanyMaterialOverviewType), "en{'Material overview type'}de{'Materialübersichtstyp'}")]
        public ACValueItem SelectedFilterCompanyMaterialOverviewType
        {
            get
            {
                return _SelectedFilterCompanyMaterialOverviewType;
            }
            set
            {
                if (_SelectedFilterCompanyMaterialOverviewType != value)
                {
                    _SelectedFilterCompanyMaterialOverviewType = value;
                    OnPropertyChanged(nameof(SelectedFilterCompanyMaterialOverviewType));
                    Search();
                }
            }
        }


        private List<ACValueItem> _FilterCompanyMaterialOverviewTypeList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The FilterCompanyMaterialOverviewType list</value>
        [ACPropertyList(9999, nameof(CompanyMaterialOverviewType))]
        public List<ACValueItem> FilterCompanyMaterialOverviewTypeList
        {
            get
            {
                if (_FilterCompanyMaterialOverviewTypeList == null)
                {
                    _FilterCompanyMaterialOverviewTypeList = new ACValueListCompanyMaterialOverviewTypeEnum();
                }
                return _FilterCompanyMaterialOverviewTypeList;
            }
        }

        #endregion


        #region Properties -> CompanyMaterialOverviewModel


        #region MaterialOverview
        private CompanyMaterialOverviewModel _SelectedMaterialOverview;
        /// <summary>
        /// Selected property for CompanyMaterialOverviewModel
        /// </summary>
        /// <value>The selected MaterialOverview</value>
        [ACPropertySelected(9999, "MaterialOverview", "en{'TODO: MaterialOverview'}de{'TODO: MaterialOverview'}")]
        public CompanyMaterialOverviewModel SelectedMaterialOverview
        {
            get
            {
                return _SelectedMaterialOverview;
            }
            set
            {
                if (_SelectedMaterialOverview != value)
                {
                    _SelectedMaterialOverview = value;
                    OnPropertyChanged(nameof(SelectedMaterialOverview));

                    if(BSOMaterialDetails_Child != null && BSOMaterialDetails_Child.Value != null)
                    {
                        
                        if (value!= null)
                        {
                            BSOMaterialDetails_Child.Value.CurrentMaterialID = value.MaterialID;
                            BSOMaterialDetails_Child.Value.CurrentMaterialStock = value.CurrentMaterialStock;
                        }
                        else
                        {
                            BSOMaterialDetails_Child.Value.CurrentMaterialID = null;
                            BSOMaterialDetails_Child.Value.CurrentMaterialStock = null;
                        }
                    }
                }
            }
        }

        private List<CompanyMaterialOverviewModel> _MaterialOverviewList;
        /// <summary>
        /// List property for CompanyMaterialOverviewModel
        /// </summary>
        /// <value>The MaterialOverview list</value>
        [ACPropertyList(9999, "MaterialOverview")]
        public List<CompanyMaterialOverviewModel> MaterialOverviewList
        {
            get
            {
                return _MaterialOverviewList;
            }
            set
            {
                _MaterialOverviewList = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #endregion

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodInfo(nameof(Search), ConstApp.Search, 100)]
        public void Search()
        {
            if (FilterCompanyMaterialOverview != null)
            {
                MaterialOverviewList = GetMaterialOverviewList(DatabaseApp, CurrentCompany, FilterCompanyMaterialOverview ?? CompanyMaterialOverviewTypeEnum.ViewSupplierStock);
                SelectedMaterialOverview = MaterialOverviewList.FirstOrDefault();
            }
            else
            {
                MaterialOverviewList = null;
                SelectedMaterialOverview = null;
            }
        }

        [ACMethodInfo(nameof(Clear), Const.Clear, 200)]
        public void Clear()
        {
            MaterialOverviewList = null;
        }

        #region Methods -> Private methods

        private List<CompanyMaterialOverviewModel> GetMaterialOverviewList(DatabaseApp databaseApp, Company company, CompanyMaterialOverviewTypeEnum overviewType)
        {
            List<CompanyMaterialOverviewModel> result = new List<CompanyMaterialOverviewModel>();
            switch (overviewType)
            {
                case CompanyMaterialOverviewTypeEnum.ViewSupplierStock:
                    result = GetViewSupplierStock(databaseApp, company);
                    break;
                case CompanyMaterialOverviewTypeEnum.ViewPartnerStock:
                    result = GetViewPartnerStock(databaseApp, company);
                    break;
                case CompanyMaterialOverviewTypeEnum.ViewPartnerStockOfUniqueMaterials:
                    result = GetViewPartnerStockOfUniqueMaterials(databaseApp, company);
                    break;
                default:
                    break;
            }

            int nr = 0;
            foreach(CompanyMaterialOverviewModel item in result)
            {
                nr ++;
                item.Sn = nr;
            }     
            return result;
        }

        private List<CompanyMaterialOverviewModel> GetViewSupplierStock(DatabaseApp databaseApp, Company company)
        {
            return
               s_cQry_ViewSupplierStock(databaseApp, company.CompanyID)
                .ToList();
        }

        private List<CompanyMaterialOverviewModel> GetViewPartnerStock(DatabaseApp databaseApp, Company company)
        {
            List<CompanyMaterialOverviewModel> lotManaged = GetViewPartnerStockLotManaged(databaseApp, company);
            List<CompanyMaterialOverviewModel> notLotManaged = GetViewPartnerStockNotLotManaged(databaseApp, company);

            IEnumerable<CompanyMaterialOverviewModel> union = lotManaged.Union(notLotManaged);

            return
                union
                .OrderBy(c => c.MaterialNo)
                .ThenBy(c => c.CompanyMaterialNo)
                .ToList();
        }

        private List<CompanyMaterialOverviewModel> GetViewPartnerStockLotManaged(DatabaseApp databaseApp, Company company)
        {
            return
               s_cQry_ViewPartnerStockLotManaged(databaseApp, company.CompanyID)
                .ToList();
        }

        private List<CompanyMaterialOverviewModel> GetViewPartnerStockNotLotManaged(DatabaseApp databaseApp, Company company)
        {
            return
               s_cQry_ViewPartnerStockNotLotManaged(databaseApp, company.CompanyID)
                .ToList();
        }


        private List<CompanyMaterialOverviewModel> GetViewPartnerStockOfUniqueMaterials(DatabaseApp databaseApp, Company company)
        {
            return
               s_cQry_ViewPartnerStockOfUniqueMaterials(databaseApp, company.CompanyID)
                .ToList();
        }


        #endregion

        #endregion

        #region Methods -> Overrides

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Search):
                    Search();
                    return true;
                case nameof(Clear):
                    Clear();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Queries

        public static readonly Func<DatabaseApp, Guid, IQueryable<CompanyMaterialOverviewModel>> s_cQry_ViewSupplierStock =
           EF.CompileQuery<DatabaseApp, Guid, IQueryable<CompanyMaterialOverviewModel>>(
         (databaseApp, companyID) =>
         databaseApp
          .FacilityCharge
         .Include(c => c.CompanyMaterial)
         .Include(c => c.CompanyMaterial.Material)
          .Where(c =>
              !c.NotAvailable
              && c.CompanyMaterial.CompanyID == companyID
          )
          .Select(c => c.CompanyMaterial)
          .GroupBy(c => c.CompanyMaterialNo)
          .Select(c => c.FirstOrDefault() ?? null)
          .Select(c =>
                  new CompanyMaterialOverviewModel
                  {
                      MaterialID = c.MaterialID,
                      MaterialNo = c.Material.MaterialNo,
                      MaterialName = c.Material.MaterialName1,
                      CompanyMaterialNo = c.CompanyMaterialNo,
                      CompanyMaterialName = c.CompanyMaterialName,
                      CurrentMaterialStock = c.Material.MaterialStock_Material.OrderBy(x=>x.InsertDate).FirstOrDefault()
                  })
          .OrderBy(c => c.MaterialNo)
          .ThenBy(c => c.CompanyMaterialNo)

      );

        public static readonly Func<DatabaseApp, Guid, IQueryable<CompanyMaterialOverviewModel>> s_cQry_ViewPartnerStockLotManaged =
           EF.CompileQuery<DatabaseApp, Guid, IQueryable<CompanyMaterialOverviewModel>>(
               (databaseApp, companyID) =>
               databaseApp
                .FacilityCharge
               .Include(c => c.CPartnerCompanyMaterial)
               .Include(c => c.CPartnerCompanyMaterial.Material)
               .Include(c => c.CPartnerCompanyMaterial.Material.MDFacilityManagementType)
                .Where(c =>
                    !c.NotAvailable
                    && c.CPartnerCompanyMaterial.CompanyID == companyID
                    && (
                        c.Material.MDFacilityManagementType != null
                        && (
                            c.Material.MDFacilityManagementType.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.FacilityCharge
                            || c.Material.MDFacilityManagementType.MDFacilityManagementTypeIndex == (int)MDFacilityManagementType.FacilityManagementTypes.FacilityChargeReservation
                            )
                        )
                )
                .Select(c => c.CPartnerCompanyMaterial)
                .GroupBy(c => c.CompanyMaterialNo)
                .Select(c => c.FirstOrDefault() ?? null)
                .Select(c =>
                        new CompanyMaterialOverviewModel
                        {
                            MaterialID = c.MaterialID,
                            MaterialNo = c.Material.MaterialNo,
                            MaterialName = c.Material.MaterialName1,
                            CompanyMaterialNo = c.CompanyMaterialNo,
                            CompanyMaterialName = c.CompanyMaterialName,
                            CurrentMaterialStock = c.Material.MaterialStock_Material.OrderBy(x => x.InsertDate).FirstOrDefault()
                        })
                .OrderBy(c => c.MaterialNo)
                .ThenBy(c => c.CompanyMaterialNo)

            );

        public static readonly Func<DatabaseApp, Guid, IQueryable<CompanyMaterialOverviewModel>> s_cQry_ViewPartnerStockNotLotManaged =
           EF.CompileQuery<DatabaseApp, Guid, IQueryable<CompanyMaterialOverviewModel>>(
               (databaseApp, companyID) =>
                databaseApp
                .CompanyMaterialStock
                .Include(c => c.CompanyMaterial)
                .Include(c => c.CompanyMaterial.Material)
                .Include(c => c.CompanyMaterial.Material.MDFacilityManagementType)
                .Where(c =>
                        c.CompanyMaterial.CompanyID == companyID
                        &&
                            (c.CompanyMaterial.Material.MDFacilityManagementType == null
                                ||
                                    (
                                        c.CompanyMaterial.Material.MDFacilityManagementType.MDFacilityManagementTypeIndex != (int)MDFacilityManagementType.FacilityManagementTypes.FacilityCharge
                                        && c.CompanyMaterial.Material.MDFacilityManagementType.MDFacilityManagementTypeIndex != (int)MDFacilityManagementType.FacilityManagementTypes.FacilityChargeReservation
                                    )
                            )
                )
                .Select(c => c.CompanyMaterial)
                .GroupBy(c => c.CompanyMaterialNo)
                .Select(c => c.FirstOrDefault() ?? null)
                .Select(c =>
                        new CompanyMaterialOverviewModel
                        {
                            MaterialID = c.MaterialID,
                            MaterialNo = c.Material.MaterialNo,
                            MaterialName = c.Material.MaterialName1,
                            CompanyMaterialNo = c.CompanyMaterialNo,
                            CompanyMaterialName = c.CompanyMaterialName,
                            CurrentMaterialStock = c.Material.MaterialStock_Material.OrderBy(x => x.InsertDate).FirstOrDefault()
                        })
                .OrderBy(c => c.MaterialNo)
                .ThenBy(c => c.CompanyMaterialNo)

            );


        public static readonly Func<DatabaseApp, Guid, IQueryable<CompanyMaterialOverviewModel>> s_cQry_ViewPartnerStockOfUniqueMaterials =
     EF.CompileQuery<DatabaseApp, Guid, IQueryable<CompanyMaterialOverviewModel>>(
         (databaseApp, companyID) =>
         databaseApp
          .CompanyMaterial
          .Where(c=>c.CompanyID == companyID)
          .Select(c =>
                  new CompanyMaterialOverviewModel
                  {
                      MaterialID = c.MaterialID,
                      MaterialNo = c.Material.MaterialNo,
                      MaterialName = c.Material.MaterialName1,
                      CompanyMaterialNo = c.CompanyMaterialNo,
                      CompanyMaterialName = c.CompanyMaterialName,
                      CurrentMaterialStock = c.Material.MaterialStock_Material.OrderBy(x => x.InsertDate).FirstOrDefault()
                  })
          .OrderBy(c => c.MaterialNo)
          .ThenBy(c => c.CompanyMaterialNo)

      );

        #endregion
    }
}
