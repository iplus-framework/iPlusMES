using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESFacilityManagementType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOFacilityManagementType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDFacilityManagementTypeIndex", ConstApp.ESFacilityManagementType, typeof(MDFacilityManagementType.FacilityManagementTypes), Const.ContextDatabase + "\\FacilityManagementTypesList", "", true, MinValue = (short)FacilityManagementTypes.NoFacility)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDFacilityManagementType.ClassName, ConstApp.ESFacilityManagementType, typeof(MDFacilityManagementType), MDFacilityManagementType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDFacilityManagementType>) })]
    [NotMapped]
    public partial class MDFacilityManagementType
    {
        [NotMapped]
        public const string ClassName = "MDFacilityManagementType";

        #region New/Delete
        public static MDFacilityManagementType NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDFacilityManagementType entity = new MDFacilityManagementType();
            entity.MDFacilityManagementTypeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.FacilityManagementType = FacilityManagementTypes.NoFacility;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDFacilityManagementType>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDFacilityManagementType>>(
            (database) => from c in database.MDFacilityManagementType where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDFacilityManagementType>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDFacilityManagementType>>(
            (database, index) => from c in database.MDFacilityManagementType where c.MDFacilityManagementTypeIndex == index select c
        );

        public static MDFacilityManagementType DefaultMDFacilityManagementType(DatabaseApp dbApp)
        {
            try
            {
                MDFacilityManagementType defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)FacilityManagementTypes.NoFacility).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDFacilityManagementType", "DefaultMDFacilityManagementType", msg);
                return null;
            }
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
                return MDFacilityManagementTypeName;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return Const.MDKey;
            }
        }
        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDFacilityManagementTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDFacilityManagementTypeName");
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

#region enums
        [NotMapped]
        public FacilityManagementTypes FacilityManagementType
        {
            get
            {
                return (FacilityManagementTypes)MDFacilityManagementTypeIndex;
            }
            set
            {
                MDFacilityManagementTypeIndex = (short)value;
                OnPropertyChanged("FacilityManagementType");
            }
        }

        /// <summary>
        /// Enum für das Feld MDFacilityManagementTypeIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'FacilityManagementTypes'}de{'FacilityManagementTypes'}", Global.ACKinds.TACEnum)]
        public enum FacilityManagementTypes : short
        {
            NoFacility = 1,             // Keine Lagerverwaltung
            Facility = 2,               // Lagerführung ohne Chargen
            FacilityCharge = 3,         // Lagerführung mit Chargen
            FacilityChargeReservation = 4 // Lagerführung mit Chargen und reservierungspflichtig
        }

        [NotMapped]
        static ACValueItemList _FacilityManagementTypesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        [NotMapped]
        public static ACValueItemList FacilityManagementTypesList
        {
            get
            {
                if (_FacilityManagementTypesList == null)
                {
                    _FacilityManagementTypesList = new ACValueItemList("FacilityManagementTypes");
                    _FacilityManagementTypesList.AddEntry((short)FacilityManagementTypes.NoFacility, "en{'No Inventory Management'}de{'Keine Lagerverwaltung'}");
                    _FacilityManagementTypesList.AddEntry((short)FacilityManagementTypes.Facility, "en{'Inventory Management Without Batches'}de{'Lagerführung ohne Chargen'}");
                    _FacilityManagementTypesList.AddEntry((short)FacilityManagementTypes.FacilityCharge, "en{'Inventory Management with Batches'}de{'Lagerführung mit Chargen'}");
                    _FacilityManagementTypesList.AddEntry((short)FacilityManagementTypes.FacilityChargeReservation, "en{'Inventory Management with Batches + Reservation'}de{'Lagerführung mit Chargen Reservierungspflichtig'}");
                }
                return _FacilityManagementTypesList;
            }
        }
#endregion

    }
}




