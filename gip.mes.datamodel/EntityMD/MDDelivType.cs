using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Data.Objects;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESDelivType, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSODelivType")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDDelivTypeIndex", ConstApp.ESDelivType, typeof(MDDelivType.DelivTypes), Const.ContextDatabase + "\\DelivTypesList", "", true, MinValue = (short)DelivTypes.Supplier)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDDelivType.ClassName, ConstApp.ESDelivType, typeof(MDDelivType), MDDelivType.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDDelivType>) })]
    public partial class MDDelivType
    {
        public const string ClassName = "MDDelivType";

        #region New/Delete
        public static MDDelivType NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDDelivType entity = new MDDelivType();
            entity.MDDelivTypeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.DelivType = DelivTypes.Supplier;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDDelivType>> s_cQry_Default =
            CompiledQuery.Compile<DatabaseApp, IQueryable<MDDelivType>>(
            (database) => from c in database.MDDelivType where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDDelivType>> s_cQry_Index =
            CompiledQuery.Compile<DatabaseApp, short, IQueryable<MDDelivType>>(
            (database, index) => from c in database.MDDelivType where c.MDDelivTypeIndex == index select c
        );

        public static MDDelivType DefaultMDDelivType(DatabaseApp dbApp)
        {
            try
            {
                MDDelivType defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)DelivTypes.Supplier).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDDelivType", "DefaultMDDelivType", msg);
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
                return MDDelivTypeName;
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
        public String MDDelivTypeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDDelivTypeName");
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
        public DelivTypes DelivType
        {
            get
            {
                return (DelivTypes)MDDelivTypeIndex;
            }
            set
            {
                MDDelivTypeIndex = (short)value;
                OnPropertyChanged("DelivType");
            }
        }

        /// <summary>
        /// Enum für das Feld MDDelivTypeIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'DelivTypes'}de{'DelivTypes'}", Global.ACKinds.TACEnum)]
        public enum DelivTypes : short
        {
            Supplier = 1, //Zulieferer
            Collector = 2, //Abholer
        }

        static ACValueItemList _DelivTypesList = null;

        public static ACValueItemList DelivTypesList
        {
            get
            {
                if (_DelivTypesList == null)
                {
                    _DelivTypesList = new ACValueItemList("DelivTypes");
                    _DelivTypesList.AddEntry((short)DelivTypes.Supplier, "en{'Supplier'}de{'Zulieferer'}");
                    _DelivTypesList.AddEntry((short)DelivTypes.Collector, "en{'Collector'}de{'Abholer'}");
                }
                return _DelivTypesList;
            }
        }
#endregion
    }
}
