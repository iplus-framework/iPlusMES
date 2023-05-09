using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESLabOrderPosState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOLabOrderPosState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDLabOrderPosStateIndex", ConstApp.ESLabOrderPosState, typeof(MDLabOrderPosState.LabOrderPosStates), Const.ContextDatabase + "\\LabOrderPosStatesList", "", true, MinValue = (short)LabOrderPosStates.NotFree)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDLabOrderPosState.ClassName, ConstApp.ESLabOrderPosState, typeof(MDLabOrderPosState), MDLabOrderPosState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDLabOrderPosState>) })]
    public partial class MDLabOrderPosState
    {
        public const string ClassName = "MDLabOrderPosState";

        #region New/Delete
        public static MDLabOrderPosState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDLabOrderPosState entity = new MDLabOrderPosState();
            entity.MDLabOrderPosStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.LabOrderPosState = LabOrderPosStates.NotFree;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDLabOrderPosState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDLabOrderPosState>>(
            (database) => from c in database.MDLabOrderPosState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDLabOrderPosState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDLabOrderPosState>>(
            (database, index) => from c in database.MDLabOrderPosState where c.MDLabOrderPosStateIndex == index select c
        );

        public static MDLabOrderPosState DefaultMDLabOrderPosState(DatabaseApp dbApp)
        {
            try
            {
                MDLabOrderPosState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)LabOrderPosStates.Free).FirstOrDefault();
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
                return MDLabOrderPosStateName;
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
        public String MDLabOrderPosStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDLabOrderPosStateName");
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
        public LabOrderPosStates LabOrderPosState
        {
            get
            {
                return (LabOrderPosStates)MDLabOrderPosStateIndex;
            }
            set
            {
                MDLabOrderPosStateIndex = (short)value;
                OnPropertyChanged("LabOrderPosState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDLabOrderPosStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'LabOrderPosStates'}de{'LabOrderPosStates'}", Global.ACKinds.TACEnum)]
        public enum LabOrderPosStates : short
        {
            NotFree = 1, //unfrei
            Free = 2, //frei
            Blocked = 3, //gesperrt
            Declined = 4 //abgelehnt
        }

        [NotMapped]
        static ACValueItemList _LabOrderPosStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        [NotMapped]
        public static ACValueItemList LabOrderPosStatesList
        {
            get
            {
                if (_LabOrderPosStatesList == null)
                {
                    _LabOrderPosStatesList = new ACValueItemList("LabOrderPosStates");
                    _LabOrderPosStatesList.AddEntry((short)LabOrderPosStates.NotFree, "en{'Not Free'}de{'Nicht frei'}");
                    _LabOrderPosStatesList.AddEntry((short)LabOrderPosStates.Free, "en{'Free'}de{'Frei'}");
                    _LabOrderPosStatesList.AddEntry((short)LabOrderPosStates.Blocked, "en{'Blocked'}de{'Blockiert'}");
                    _LabOrderPosStatesList.AddEntry((short)LabOrderPosStates.Declined, "en{'Declined'}de{'Abgelehnt'}");
                }
                return _LabOrderPosStatesList;
            }
        }

#endregion
        
    }
}
