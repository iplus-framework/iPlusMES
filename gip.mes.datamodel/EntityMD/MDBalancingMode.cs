using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESBalancingMode, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOBalancingMode")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDBalancingModeIndex", ConstApp.ESBalancingMode, typeof(MDBalancingMode.BalancingModes), Const.ContextDatabase + "\\BalancingModesList" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + MDBalancingMode.ClassName, ConstApp.ESBalancingMode, typeof(MDBalancingMode), MDBalancingMode.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDBalancingMode>) })]
    [NotMapped]
    public partial class MDBalancingMode
    {
        [NotMapped]
        public const string ClassName = "MDBalancingMode";

        #region New/Delete
        public static MDBalancingMode NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDBalancingMode entity = new MDBalancingMode();
            entity.MDBalancingModeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.MDBalancingModeIndex = (Int16)BalancingModes.InwardOn_OutwardOn;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDBalancingMode>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDBalancingMode>>(
            (database) => from c in database.MDBalancingMode where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDBalancingMode>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDBalancingMode>>(
            (database, index) => from c in database.MDBalancingMode where c.MDBalancingModeIndex == index select c
        );

        public static MDBalancingMode DefaultMDBalancingMode(DatabaseApp dbApp)
        {
            try
            {
                MDBalancingMode defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)BalancingModes.InwardOn_OutwardOn).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDBalancingMode", "DefaultMDBalancingMode", msg);
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
                return MDBalancingModeName;
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
        public BalancingModes BalancingMode
        {
            get
            {
                return (BalancingModes)MDBalancingModeIndex;
            }
            set
            {
                MDBalancingModeIndex = (short)value;
                OnPropertyChanged("BalancingMode");
            }
        }

        /// <summary>
        /// Enum für das Feld MDBalancingModeIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'BalancingModes'}de{'BalancingModes'}", Global.ACKinds.TACEnum)]
        public enum BalancingModes : short
        {
            InwardOn_OutwardOn = 0,
            InwardOn_OutwardOff = 1,
            InwardOff_OutwardOn = 2,
            InwardOff_OutwardOff = 3,
            _null = 4, // Wenn nichts an anderer Stelle Konfiguriert ist, wird InwardOn_OutwardOn als Default angenommen
        }

        [NotMapped]
        static ACValueItemList _BalancingModesList = null;

        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        [NotMapped]
        public static ACValueItemList BalancingModesList
        {
            get
            {
                if (_BalancingModesList == null)
                {
                    _BalancingModesList = new ACValueItemList("BalancingModes");
                    _BalancingModesList.AddEntry((short)BalancingModes.InwardOn_OutwardOn, "en{'Inward On, Outward On'}de{'Einwärts an, Auswärts an'}");
                    _BalancingModesList.AddEntry((short)BalancingModes.InwardOn_OutwardOff, "en{'Inward On, Outward Off'}de{'Einwärts an, Auswärts aus'}");
                    _BalancingModesList.AddEntry((short)BalancingModes.InwardOff_OutwardOn, "en{'Inward Off, Outward On'}de{'Einwärts aus, Auswärts an'}");
                    _BalancingModesList.AddEntry((short)BalancingModes.InwardOff_OutwardOff, "en{'Inward Off, Outward Off'}de{'Einwärts aus, Auswärts aus'}");
                }
                return _BalancingModesList;
            }
        }
        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDBalancingModeName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDBalancingModeName");
            }
        }

#endregion
    }
}
