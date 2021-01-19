using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Lab Report'}de{'Laborauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOLabOrder")]
    [ACPropertyEntity(1, "LabOrderNo", "en{'Laboratory Order No.'}de{'Laborauftragsnummer'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, InOrderPos.ClassName, "en{'Purchase Order Pos.'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACPropertyEntity(4, OutOrderPos.ClassName, "en{'Sales Order Pos.'}de{'Auftragsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(5, ProdOrderPartslistPos.ClassName, "en{'Bill of Material Pos.'}de{'Stücklistenposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(6, FacilityLot.ClassName, "en{'Lot/Charge'}de{'Los/Charge'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName, "", true)]
    [ACPropertyEntity(7, "SampleTakingDate", "en{'Sample Extraction'}de{'Probenentnahme'}", "", "", true)]
    [ACPropertyEntity(8, "TestDate", "en{'Test Date'}de{'Testzeitpunkt'}", "", "", true)]
    [ACPropertyEntity(9, "MDLabOrderState", ConstApp.ESLabOrderState, Const.ContextDatabase + "\\MDLabOrderState", "", true)]
    [ACPropertyEntity(10, "LabOrderTypeIndex", "en{'Type of Order'}de{'Auftragstyp'}", typeof(GlobalApp.LabOrderType), Const.ContextDatabase + "\\LabOrderTypeList", "", true)]
    [ACPropertyEntity(11, "BasedOnTemplate", "en{'Based on Template'}de{'Basierend auf Vorlage'}", Const.ContextDatabase + "\\LabOrder", "", true)]
    [ACPropertyEntity(495, "TemplateName", "en{'Template Name'}de{'Name der Vorlage'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + LabOrder.ClassName, "en{'Lab Order'}de{'Laborauftrag'}", typeof(LabOrder), LabOrder.ClassName, "LabOrderNo", "LabOrderNo", new object[]
        {
                new object[] {Const.QueryPrefix + LabOrderPos.ClassName, "en{'Lab Values'}de{'Laborwerte'}", typeof(LabOrderPos), LabOrderPos.ClassName + "_" + LabOrder.ClassName, "Sequence", "Sequence"}
        })
    ]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + "LabOrderTemplate", "en{'Lab Order Template'}de{'Laborauftrag Vorlage'}", typeof(LabOrder), LabOrder.ClassName, "LabOrderNo", "LabOrderNo", new object[]
        {
                new object[] {Const.QueryPrefix + LabOrderPos.ClassName, "en{'Lab Values'}de{'Laborwerte'}", typeof(LabOrderPos), LabOrderPos.ClassName + "_" + LabOrder.ClassName, "Sequence", "Sequence"}
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<LabOrder>) })]
    public partial class LabOrder
    {
        public const string ClassName = "LabOrder";
        public const string NoColumnName = "LabOrderNo";
        public const string FormatNewNo = "LO{0}";

        #region New/Delete
        public static LabOrder NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            LabOrder entity = new LabOrder();
            entity.LabOrderID = Guid.NewGuid();
            if (parentACObject != null && parentACObject is LabOrder)
            {
                LabOrder template = parentACObject as LabOrder;
                entity.Material = template.Material;
                entity.SampleTakingDate = DateTime.Now;
                entity.MDLabOrderState = template.MDLabOrderState;
                entity.LabOrderTypeIndex = 1;
                entity.BasedOnTemplateID = template.LabOrderID;
            }
            else
            {
                entity.DefaultValuesACObject();
                entity.LabOrderTypeIndex = (short)GlobalApp.LabOrderType.Order;
                entity.MDLabOrderState = MDLabOrderState.DefaultMDLabOrderState(dbApp);
            }
            entity.LabOrderNo = secondaryKey;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }

        public MsgWithDetails DeleteACObject(IACEntityObjectContext database, bool withCheck)
        {
            if (withCheck)
            {
                MsgWithDetails msg = IsEnabledDeleteACObject(database);
                if (msg != null)
                    return msg;
            }
            database.DeleteObject(this);
            return null;
        }

        #endregion

        #region IACUrl Member
        public override string ToString()
        {
            return ACCaption;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return LabOrderNo;
            }
            set
            {
                throw new NotImplementedException();
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
            if (filterValues.Any())
            {
                switch (className)
                {
                    case LabOrderPos.ClassName:
                        Int16 sequence = 0;
                        if (Int16.TryParse(filterValues[0], out sequence))
                            return this.LabOrderPos_LabOrder.FirstOrDefault(x => x.Sequence == sequence);
                        break;
                }
            }
            return null;
        }

        #endregion

        #region IACObjectEntity Members
        static public string KeyACIdentifier
        {
            get
            {
                return "LabOrderNo";
            }
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

        #region Additional Properties
        public GlobalApp.LabOrderType LabOrderType
        {
            get
            {
                return (GlobalApp.LabOrderType)LabOrderTypeIndex;
            }
            set
            {
                LabOrderTypeIndex = (Int16)value;
            }
        }

        #endregion
    }
}




