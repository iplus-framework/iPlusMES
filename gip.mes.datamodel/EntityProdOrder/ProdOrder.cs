using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Production Order'}de{'Produktionsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOProdOrder")]
    [ACPropertyEntity(1, "ProgramNo", "en{'Order Number'}de{'Auftragsnummer'}", "", "", true)]
    [ACPropertyEntity(3, MDProdOrderState.ClassName, "en{'Production Status'}de{'Produktionsstatus'}", Const.ContextDatabase + "\\" + MDProdOrderState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, "CPartnerCompany", "en{'Contractual Partner'}de{'Vertragspartner'}", Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + ProdOrder.ClassName, "en{'Production Order'}de{'Produktionsauftrag'}", typeof(ProdOrder), ProdOrder.ClassName, "ProgramNo", "ProgramNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<ProdOrder>) })]
    public partial class ProdOrder
    {
        public const string ClassName = "ProdOrder";
        public const string NoColumnName = "ProgramNo";
        public const string FormatNewNo = "P{0}";

        #region new/Delete
        public static ProdOrder NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            ProdOrder entity = new ProdOrder();
            entity.DefaultValuesACObject();
            entity.ProdOrderID = Guid.NewGuid();
            entity.MDProdOrderState = MDProdOrderState.DefaultMDProdOrderState(dbApp);
            entity.ProgramNo = secondaryKey;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl

        public override string ToString()
        {
            return "(P)" + ProgramNo;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return ProgramNo;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "ProgramNo";
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && filterValues[0] != null)
            {
                string[] filterParams = filterValues[0].Split(',');
                switch (className)
                {
                    case ProdOrderPartslist.ClassName:
                        int sequence = int.Parse(filterParams[2]);
                        return this.ProdOrderPartslist_ProdOrder
                            .Where(c =>
                                c.Partslist.PartslistNo == filterParams[0]
                                && c.Partslist.PartslistVersion == filterParams[1]
                                && c.Sequence == sequence
                                ).FirstOrDefault();

                }
            }

            return null;
        }
        #endregion

        #region IEntityProperty Members

        [NotMapped]
        bool bRefreshConfig = false;
        protected override void OnPropertyChanging<T>(T newValue, string propertyName, bool afterChange)
        {
            if (propertyName == nameof(XMLConfig))
            {
                string xmlConfig = newValue as string;
                if (afterChange)
                {
                    if (bRefreshConfig)
                        ACProperties.Refresh();
                }
                else
                {
                    bRefreshConfig = false;
                    if (this.EntityState != EntityState.Detached && (!(String.IsNullOrEmpty(xmlConfig) && String.IsNullOrEmpty(XMLConfig)) && xmlConfig != XMLConfig))
                        bRefreshConfig = true;
                }
            }
            base.OnPropertyChanging(newValue, propertyName, afterChange);
        }

        #endregion

        #region Properties

        [NotMapped]
        private string _ProducedMaterialNos;
        [ACPropertyInfo(9999, "ProducedMaterialNos", "en{'Produced materials No'}de{'Material Nummers'}")]
        [NotMapped]
        public string ProducedMaterialNos
        {
            get
            {
                if (_ProducedMaterialNos == null)
                {
                    List<string> materialNos = new List<string>();
                    if (this.ProdOrderPartslist_ProdOrder.Any())
                        foreach (var item in this.ProdOrderPartslist_ProdOrder)
                            materialNos.Add(item.Partslist.Material.MaterialNo);
                    _ProducedMaterialNos = string.Join(",", materialNos.ToArray());
                }
                return _ProducedMaterialNos;
            }
        }

        [NotMapped]
        private string _ProducedMaterialNames;
        [ACPropertyInfo(9999, "ProducedMaterialNames", "en{'Product names'}de{'Produktnamen'}")]
        [NotMapped]
        public string ProducedMaterialNames
        {
            get
            {
                if (_ProducedMaterialNames == null)
                {
                    List<string> materialNos = new List<string>();
                    if (ProdOrderPartslist_ProdOrder.Any())
                        foreach (var item in ProdOrderPartslist_ProdOrder.OrderBy(c => c.Sequence))
                            materialNos.Add(item.Partslist.Material.MaterialName1);
                    _ProducedMaterialNames = string.Join(",", materialNos.ToArray());
                }
                return _ProducedMaterialNames;
            }
        }



        [NotMapped]
        private string _PartslistName1;
        [ACPropertyInfo(9999, "PartslistName1", "en{'1.Product'}de{'1.Produkt'}")]
        [NotMapped]
        public string PartslistName1
        {
            get
            {
                if (string.IsNullOrEmpty(_PartslistName1))
                    _PartslistName1 = LoadPartslistName(1);
                return _PartslistName1;
            }
        }

        [NotMapped]
        private string _PartslistName2;
        [ACPropertyInfo(9999, "PartslistName2", "en{'2.Product'}de{'2.Produkt'}")]
        [NotMapped]
        public string PartslistName2
        {
            get
            {
                if (string.IsNullOrEmpty(_PartslistName2))
                    _PartslistName2 = LoadPartslistName(2);
                return _PartslistName2;
            }
        }

        [NotMapped]
        private string _PartslistName3;
        [ACPropertyInfo(9999, "PartslistName3", "en{'3.Product'}de{'3.Produkt'}")]
        [NotMapped]
        public string PartslistName3
        {
            get
            {
                if (string.IsNullOrEmpty(_PartslistName3))
                    _PartslistName3 = LoadPartslistName(3);
                return _PartslistName3;
            }
        }

        private string LoadPartslistName(int sequence)
        {
            string name = "-";
            ProdOrderPartslist pl = null;
            // 1
            pl = ProdOrderPartslist_ProdOrder.FirstOrDefault(c => c.Sequence == sequence);
            if (pl != null)
                name = pl.Partslist.Material.MaterialName1;
            return name;
        }

        [NotMapped]
        private bool _IsSelected;
        [ACPropertyInfo(999, nameof(IsSelected), Const.Select)]
        [NotMapped]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [NotMapped]
        private string _DepartmentUserNames;
        [ACPropertyInfo(505, nameof(DepartmentUserNames), ConstApp.DepartmentUserName)]
        [NotMapped]
        public string DepartmentUserNames
        {
            get
            {
                if (_DepartmentUserNames == null)
                {
                    _DepartmentUserNames = "";
                    List<string> departmentUserNames = new List<string>();
                    if (ProdOrderPartslist_ProdOrder.Where(c => !string.IsNullOrEmpty(c.DepartmentUserName)).Any())
                    {
                        foreach (var item in ProdOrderPartslist_ProdOrder.Where(c => !string.IsNullOrEmpty(c.DepartmentUserName)).OrderBy(c => c.Sequence))
                        {
                            departmentUserNames.Add(item.DepartmentUserName);
                        }
                        _DepartmentUserNames = string.Join(",", departmentUserNames.ToArray());
                    }
                }
                return _DepartmentUserNames;
            }
        }

        [NotMapped]
        private List<DateTime> _ProductionStartTimes;
        [NotMapped]
        [ACPropertyInfo(506, nameof(ProductionStartTimes), ConstApp.ProductionStart)]
        public List<DateTime> ProductionStartTimes
        {
            get
            {
                if (_ProductionStartTimes == null)
                {
                    _ProductionStartTimes =
                        this
                        .ProdOrderPartslist_ProdOrder
                        .Select(c => c.StartDate)
                        .Where(c => c != null)
                        .Select(c => c.Value.Date)
                        .Distinct()
                        .ToList();
                }
                return _ProductionStartTimes;
            }
        }

        [NotMapped]
        private string _ProductionStartTimesStr;
        [NotMapped]
        [ACPropertyInfo(507, nameof(ProductionStartTimesStr), ConstApp.ProductionStart)]
        public string ProductionStartTimesStr
        {
            get
            {
                if (_ProductionStartTimesStr == null)
                {
                    _ProductionStartTimesStr = "";
                    if (ProductionStartTimes != null && ProductionStartTimes.Any())
                    {
                        _ProductionStartTimesStr = string.Join(",", ProductionStartTimes.Select(c => c.ToString("dd.MM.yyyy")));
                    }
                }
                return _ProductionStartTimesStr;
            }
        }


        [NotMapped]
        private bool _IsSetupMaxProdUserEndDate;
        [NotMapped]
        private DateTime? _MaxProdUserEndDate;
        [NotMapped]
        [ACPropertyInfo(508, nameof(MaxProdUserEndDate), "en{'Delivery time'}de{'Faelligkeitsdatum'}")]
        public DateTime? MaxProdUserEndDate
        {
            get
            {
                if (!_IsSetupMaxProdUserEndDate)
                {
                    _MaxProdUserEndDate = ProdOrderPartslist_ProdOrder.Where(c => c.ProdUserEndDate != null).Max(c => c.ProdUserEndDate);
                    _IsSetupMaxProdUserEndDate = true;
                }
                return _MaxProdUserEndDate;
            }
        }

        #endregion

        #region Methods
        public void RecalcActualQuantity(Nullable<MergeOption> mergeOption = null)
        {
            foreach (ProdOrderPartslist pOPl in this.ProdOrderPartslist_ProdOrder)
            {
                pOPl.RecalcActualQuantity(true);
            }
        }

        /// <summary>
        /// Recalc via Stored Procedure (cca 2 sec for ProgNo=3163)
        /// </summary>
        /// <param name="dbApp"></param>
        public void RecalcActualQuantitySP(DatabaseApp dbApp, bool callRefresh = true)
        {
            dbApp.Database.ExecuteSql(FormattableStringFactory.Create("udpRecalcActualQuantity @p0, @p1" ,ProgramNo, null));
            if (callRefresh)
                RefreshAfterRecalcActualQuantity(dbApp);
        }

        public void RefreshAfterRecalcActualQuantity(DatabaseApp dbApp)
        {
            this.Refresh(dbApp);
            foreach (var pl in ProdOrderPartslist_ProdOrder)
            {
                pl.RefreshAfterRecalcActualQuantity(dbApp);
            }
        }


        #endregion
    }
}
