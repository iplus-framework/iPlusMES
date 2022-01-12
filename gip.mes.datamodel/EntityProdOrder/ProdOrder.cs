using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Production Order'}de{'Produktionsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOProdOrder")]
    [ACPropertyEntity(1, "ProgramNo", "en{'Order Number'}de{'Auftragsnummer'}", "", "", true)]
    [ACPropertyEntity(3, MDProdOrderState.ClassName, "en{'Production Status'}de{'Produktionsstatus'}", Const.ContextDatabase + "\\" + MDProdOrderState.ClassName, "", true)]
    [ACPropertyEntity(4, "CPartnerCompany", "en{'Contractual Partner'}de{'Vertragspartner'}", Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(9999, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
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
        public override string ACCaption
        {
            get
            {
                return ProgramNo;
            }
        }

        #endregion

        #region IACObjectEntity Members

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

        bool bRefreshConfig = false;
        partial void OnXMLConfigChanging(global::System.String value)
        {
            bRefreshConfig = false;
            if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
                bRefreshConfig = true;
        }

        partial void OnXMLConfigChanged()
        {
            if (bRefreshConfig)
                ACProperties.Refresh();
        }

        #endregion

        #region properties

        private string _ProducedMaterialNos;
        [ACPropertyInfo(9999, "ProducedMaterialNos", "en{'Produced materials No'}de{'Material Nummers'}")]
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

        private string _ProducedMaterialNames;
        [ACPropertyInfo(9999, "ProducedMaterialNames", "en{'Product names'}de{'Produktnamen'}")]
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



        private string _PartslistName1;
        [ACPropertyInfo(9999, "PartslistName1", "en{'1.Product'}de{'1.Produkt'}")]
        public string PartslistName1
        {
            get
            {
                if (string.IsNullOrEmpty(_PartslistName1))
                    _PartslistName1 = LoadPartslistName(1);
                return _PartslistName1;
            }
        }

        private string _PartslistName2;
        [ACPropertyInfo(9999, "PartslistName2", "en{'2.Product'}de{'2.Produkt'}")]
        public string PartslistName2
        {
            get
            {
                if (string.IsNullOrEmpty(_PartslistName2))
                    _PartslistName2 = LoadPartslistName(2);
                return _PartslistName2;
            }
        }

        private string _PartslistName3;
        [ACPropertyInfo(9999, "PartslistName3", "en{'3.Product'}de{'3.Produkt'}")]
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

        #endregion

        #region Methods
        public void RecalcActualQuantity(Nullable<MergeOption> mergeOption = null)
        {
            foreach (ProdOrderPartslist pOPl in this.ProdOrderPartslist_ProdOrder)
            {
                pOPl.RecalcActualQuantity(mergeOption, true);
            }
        }

        /// <summary>
        /// Recalc via Stored Procedure (cca 2 sec for ProgNo=3163)
        /// </summary>
        /// <param name="dbApp"></param>
        public void RecalcActualQuantitySP(DatabaseApp dbApp, bool callRefresh = true)
        {
            dbApp.udpRecalcActualQuantity(ProgramNo, null);
            if (callRefresh)
                RefreshAfterRecalcActualQuantity(dbApp);
        }

        public void RefreshAfterRecalcActualQuantity(DatabaseApp dbApp)
        {
            this.Refresh(RefreshMode.StoreWins, dbApp);
            foreach (var pl in ProdOrderPartslist_ProdOrder)
            {
                pl.RefreshAfterRecalcActualQuantity(dbApp);
            }
        }


        #endregion
    }
}
