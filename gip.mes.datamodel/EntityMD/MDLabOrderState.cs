using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESLabOrderState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOLabOrderState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDLabOrderStateIndex", ConstApp.ESLabOrderState, typeof(MDLabOrderState.LabOrderStates), Const.ContextDatabase + "\\LabOrderStatesList", "", true, MinValue = (short)LabOrderStates.New)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDLabOrderState.ClassName, ConstApp.ESLabOrderState, typeof(MDLabOrderState), MDLabOrderState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDLabOrderState>) })]
    [NotMapped]
    public partial class MDLabOrderState
    {
        [NotMapped]
        public const string ClassName = "MDLabOrderState";

        #region New/Delete
        public static MDLabOrderState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDLabOrderState entity = new MDLabOrderState();
            entity.MDLabOrderStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.LabOrderState = LabOrderStates.New;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDLabOrderState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDLabOrderState>>(
            (database) => from c in database.MDLabOrderState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDLabOrderState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDLabOrderState>>(
            (database, index) => from c in database.MDLabOrderState where c.MDLabOrderStateIndex == index select c
        );

        public static MDLabOrderState DefaultMDLabOrderState(DatabaseApp dbApp)
        {
            try
            {
                MDLabOrderState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)LabOrderStates.InProcess).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException(ClassName, "Default" + ClassName, msg);
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
                return MDLabOrderStateName;
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
        public String MDLabOrderStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDLabOrderStateName");
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
        public LabOrderStates LabOrderState
        {
            get
            {
                return (LabOrderStates)MDLabOrderStateIndex;
            }
            set
            {
                MDLabOrderStateIndex = (short)value;
                OnPropertyChanged("LabOrderState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDLabOrderStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'LabStates'}de{'LabStates'}", Global.ACKinds.TACEnum)]
        public enum LabOrderStates : short
        {
            New = 1, //nicht frei
            InProcess = 2, //frei
            Finished = 3, //gesperrt
            Declined = 4 //abgelehnt
        }

        [NotMapped]
        static ACValueItemList _LabOrderStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        [NotMapped]
        public static ACValueItemList LabOrderStatesList
        {
            get
            {
                if (_LabOrderStatesList == null)
                {
                    _LabOrderStatesList = new ACValueItemList("LabOrderStates");
                    _LabOrderStatesList.AddEntry((short)LabOrderStates.New, "en{'New'}de{'Neu'}");
                    _LabOrderStatesList.AddEntry((short)LabOrderStates.InProcess, "en{'In Process'}de{'In Bearbeitung'}");
                    _LabOrderStatesList.AddEntry((short)LabOrderStates.Finished, "en{'Finished'}de{'Beendet'}");
                    _LabOrderStatesList.AddEntry((short)LabOrderStates.Declined, "en{'Declined'}de{'Abgelehnt'}");
                }
                return _LabOrderStatesList;
            }
        }
#endregion

    }
}




