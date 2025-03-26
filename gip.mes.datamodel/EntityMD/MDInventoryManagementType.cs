using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESInventoryManagementType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOInventoryManagementType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDInventoryManagementTypeIndex", "en{'Inventory Mgmnt. Type'}de{'Bestandsführungsart'}", typeof(MDInventoryManagementType.InventoryManagementTypes), "", "", true, MinValue = (short)InventoryManagementTypes.NoManagement)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDInventoryManagementType.ClassName, ConstApp.ESInventoryManagementType, typeof(MDInventoryManagementType), MDInventoryManagementType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDInventoryManagementType>) })]
    public partial class MDInventoryManagementType
    {
        public const string ClassName = "MDInventoryManagementType";

        #region New/Delete
        public static MDInventoryManagementType NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDInventoryManagementType entity = new MDInventoryManagementType();
            entity.MDInventoryManagementTypeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.InventoryManagementType = InventoryManagementTypes.NoManagement;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }



        static readonly Func<DatabaseApp, IQueryable<MDInventoryManagementType>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDInventoryManagementType>>(
            (database) => from c in database.MDInventoryManagementType where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDInventoryManagementType>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDInventoryManagementType>>(
            (database, index) => from c in database.MDInventoryManagementType where c.MDInventoryManagementTypeIndex == index select c
        );

        public static MDInventoryManagementType DefaultMDInventoryManagementType(DatabaseApp dbApp)
        {
            try
            {
                MDInventoryManagementType defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)InventoryManagementTypes.NoManagement).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException(ClassName, "Default"+ClassName, msg);
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
        public override string ACCaption
        {
            get
            {
                return MDInventoryManagementTypeName;
            }
        }

        #endregion

        #region IACObjectEntity Members

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
        public String MDInventoryManagementTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDInventoryManagementTypeName");
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

#region enums
        public InventoryManagementTypes InventoryManagementType
        {
            get
            {
                return (InventoryManagementTypes)MDInventoryManagementTypeIndex;
            }
            set
            {
                MDInventoryManagementTypeIndex = (short)value;
                OnPropertyChanged("InventoryManagementType");
            }
        }

        /// <summary>
        /// Enum für das Feld MDInventoryManagementTypeIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'InventoryManagementTypes'}de{'InventoryManagementTypes'}", Global.ACKinds.TACEnum)]
        public enum InventoryManagementTypes : short
        {
            NoManagement = 1,   // Keine automatische Bestandsführung
            MinStock = 2,       // Bestandsführung nach MinStock
            InfiniteStock = 3       // unable to set quant on null for material with this
        }

        static ACValueItemList _InventoryManagementTypesList = null;

        public static ACValueItemList InventoryManagementTypesList
        {
            get
            {
                if (_InventoryManagementTypesList == null)
                {
                    _InventoryManagementTypesList = new ACValueItemList("InventoryManagementTypes");
                    _InventoryManagementTypesList.AddEntry((short)InventoryManagementTypes.NoManagement, "en{'Manual Inventory Management'}de{'Manuelle Bestandsführung'}");
                    _InventoryManagementTypesList.AddEntry((short)InventoryManagementTypes.MinStock, "en{'Signal Stock'}de{'Signalbestand'}");
                }
                return _InventoryManagementTypesList;
            }
        }
#endregion
        
    }
}
