using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Lab Report'}de{'Laborauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOLabOrder")]
    [ACPropertyEntity(1, "LabOrderNo", "en{'Laboratory Order No.'}de{'Laborauftragsnummer'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, InOrderPos.ClassName, "en{'Purchase Order Pos.'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, OutOrderPos.ClassName, "en{'Sales Order Pos.'}de{'Auftragsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, ProdOrderPartslistPos.ClassName, "en{'Bill of Material Pos.'}de{'Stücklistenposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, FacilityLot.ClassName, "en{'Lot/Charge'}de{'Los/Charge'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(7, "SampleTakingDate", "en{'Sample taken at'}de{'Probe am'}", "", "", true)]
    [ACPropertyEntity(8, "TestDate", "en{'Test Date'}de{'Testzeitpunkt'}", "", "", true)]
    [ACPropertyEntity(9, "MDLabOrderState", ConstApp.ESLabOrderState, Const.ContextDatabase + "\\MDLabOrderState" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(10, "LabOrderTypeIndex", "en{'Type of Order'}de{'Auftragstyp'}", typeof(GlobalApp.LabOrderType), Const.ContextDatabase + "\\LabOrderTypeList", "", true)]
    [ACPropertyEntity(11, "BasedOnTemplate", "en{'Based on Template'}de{'Basierend auf Vorlage'}", Const.ContextDatabase + "\\LabOrder" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(495, "TemplateName", "en{'Template Name'}de{'Name der Vorlage'}", "", "", true)]
    [ACPropertyEntity(13, PickingPos.ClassName, "en{'Picking line'}de{'Kommissionierposition'}", Const.ContextDatabase + "\\" + PickingPos.ClassName, "", true)]
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
    [NotMapped]
    public partial class LabOrder
    {
        [NotMapped]
        public const string ClassName = "LabOrder";
        [NotMapped]
        public const string NoColumnName = "LabOrderNo";
        [NotMapped]
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
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
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
            database.Remove(this);
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
        [NotMapped]
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
        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "LabOrderNo";
            }
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

        #region Additional Properties
        [NotMapped]
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
        private string _OrderNo;
        [ACPropertyInfo(999, "DocumentNo", ConstApp.OrderNo)]
        [NotMapped]
        public string OrderNo
        {
            get
            {
                if (_OrderNo == null)
                {
                    if (OutOrderPos != null)
                    {
                        _OrderNo = OutOrderPos.OutOrder.OutOrderNo;
                    }
                    if (InOrderPos != null)
                    {
                        _OrderNo = InOrderPos.InOrder.InOrderNo;
                    }
                    else if (ProdOrderPartslistPos != null)
                    {
                        _OrderNo = ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    }
                    else if (FacilityLot != null)
                    {
                        _OrderNo = FacilityLot.LotNo;
                    }
                }
                return _OrderNo;
            }
        }

        #endregion
    }

}




