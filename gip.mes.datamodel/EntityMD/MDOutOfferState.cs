using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESOutOfferState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOOutOfferState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDOutOfferStateIndex", ConstApp.ESOutOfferState, typeof(MDOutOfferState.OutOfferStates), Const.ContextDatabase + "\\OutOfferStatesList", "", true, MinValue = (short)MDOutOfferState.OutOfferStates.OfferingNew)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDOutOfferState.ClassName, ConstApp.ESOutOfferState, typeof(MDOutOfferState), MDOutOfferState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDOutOfferState>) })]
    [NotMapped]
    public partial class MDOutOfferState
    {
        [NotMapped]
        public const string ClassName = "MDOutOfferState";

        #region New/Delete
        public static MDOutOfferState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDOutOfferState entity = new MDOutOfferState();
            entity.MDOutOfferStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.OutOfferState = OutOfferStates.OfferingNew;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDOutOfferState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDOutOfferState>>(
            (database) => from c in database.MDOutOfferState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDOutOfferState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDOutOfferState>>(
            (database, index) => from c in database.MDOutOfferState where c.MDOutOfferStateIndex == index select c
        );

        public static MDOutOfferState DefaultMDOutOfferState(DatabaseApp dbApp)
        {
            try
            {
                MDOutOfferState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)OutOfferStates.OfferingNew).FirstOrDefault();
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
                return MDOutOfferStateName;
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
        public String MDOutOfferStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDOutOfferStateName");
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
        public OutOfferStates OutOfferState
        {
            get
            {
                return (OutOfferStates)MDOutOfferStateIndex;
            }
            set
            {
                MDOutOfferStateIndex = (short)value;
                OnPropertyChanged("OutOfferState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDOutOfferStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'OutOfferStates'}de{'OutOfferStates'}", Global.ACKinds.TACEnum)]
        public enum OutOfferStates : short
        {
            OfferingNew = 1,
            OfferingSended = 2,
            OfferingCompleted = 3,
            OfferingCancelled = 4
        }

        [NotMapped]
        static ACValueItemList _OutOfferStatesList;

        [NotMapped]
        public static ACValueItemList OutOfferStatesList
        {
            get
            {
                if (_OutOfferStatesList == null)
                {
                    _OutOfferStatesList = new ACValueItemList("OutOfferStates");
                    _OutOfferStatesList.AddEntry((short)OutOfferStates.OfferingNew, "en{'New'}de{'Neu'}");
                    _OutOfferStatesList.AddEntry((short)OutOfferStates.OfferingSended, "en{'Sended'}de{'Gesendet'}");
                    _OutOfferStatesList.AddEntry((short)OutOfferStates.OfferingCompleted, "en{'Completed'}de{'Fertiggestellt'}");
                    _OutOfferStatesList.AddEntry((short)OutOfferStates.OfferingCancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _OutOfferStatesList;
            }
        }
#endregion

    }
}








