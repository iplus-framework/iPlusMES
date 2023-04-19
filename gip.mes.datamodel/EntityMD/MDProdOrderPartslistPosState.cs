using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESProdOrderPartslistPosState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOProdOrderPartslistPosState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDProdOrderPartslistPosStateIndex", ConstApp.ESProdOrderPartslistPosState, typeof(MDProdOrderPartslistPosState.ProdOrderPartslistPosStates), Const.ContextDatabase + "\\ProdOrderPartslistPosStatesList", "", true, MinValue = (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + MDProdOrderPartslistPosState.ClassName, ConstApp.ESProdOrderPartslistPosState, typeof(MDProdOrderPartslistPosState), MDProdOrderPartslistPosState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDProdOrderPartslistPosState>) })]
    public partial class MDProdOrderPartslistPosState
    {
        public const string ClassName = "MDProdOrderPartslistPosState";

        #region New/Delete
        public static MDProdOrderPartslistPosState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDProdOrderPartslistPosState entity = new MDProdOrderPartslistPosState();
            entity.MDProdOrderPartslistPosStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.ProdOrderPartslistPosState = ProdOrderPartslistPosStates.Created;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDProdOrderPartslistPosState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDProdOrderPartslistPosState>>(
            (database) => from c in database.MDProdOrderPartslistPosState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDProdOrderPartslistPosState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDProdOrderPartslistPosState>>(
            (database, index) => from c in database.MDProdOrderPartslistPosState where c.MDProdOrderPartslistPosStateIndex == index select c
        );

        public static MDProdOrderPartslistPosState DefaultMDProdOrderPartslistPosState(DatabaseApp dbApp)
        {
            try
            {
                MDProdOrderPartslistPosState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)ProdOrderPartslistPosStates.Created).FirstOrDefault();
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
        public override string ACCaption
        {
            get
            {
                return MDProdOrderPartslistPosStateName;
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
        public String MDProdOrderPartslistPosStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDProdOrderPartslistPosStateName");
            }
        }

#endregion

#region IEntityProperty Members

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
        public ProdOrderPartslistPosStates ProdOrderPartslistPosState
        {
            get
            {
                return (ProdOrderPartslistPosStates)MDProdOrderPartslistPosStateIndex;
            }
            set
            {
                MDProdOrderPartslistPosStateIndex = (short)value;
                OnPropertyChanged("ProdOrderPartslistPosState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDProdOrderPartslistPosStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'ProdOrderPartslistPosStates'}de{'ProdOrderPartslistPosStates'}", Global.ACKinds.TACEnum)]
        public enum ProdOrderPartslistPosStates : short
        {
            Created = 1, //Neu Angelegt
            InProcess = 2, //In Bearbeitung
            Completed = 3, //Fertiggestellt
            Blocked = 4, //Gesperrt
            Cancelled = 5, //Storniert
            AutoStart = 6,
            PartialCompleted = 7
        }

        static ACValueItemList _ProdOrderPartlistPosStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList ProdOrderPartslistPosStatesList
        {
            get
            {
                if (_ProdOrderPartlistPosStatesList == null)
                {
                    _ProdOrderPartlistPosStatesList = new ACValueItemList("ProdOrderPartslistPosStates");
                    _ProdOrderPartlistPosStatesList.AddEntry((short)ProdOrderPartslistPosStates.Created, "en{'Created'}de{'Neu angelegt'}");
                    _ProdOrderPartlistPosStatesList.AddEntry((short)ProdOrderPartslistPosStates.InProcess, "en{'In Process'}de{'In Bearbeitung'}");
                    _ProdOrderPartlistPosStatesList.AddEntry((short)ProdOrderPartslistPosStates.Completed, "en{'Completed'}de{'Fertiggestellt'}");
                    _ProdOrderPartlistPosStatesList.AddEntry((short)ProdOrderPartslistPosStates.Blocked, "en{'Blocked'}de{'Gesperrt'}");
                    _ProdOrderPartlistPosStatesList.AddEntry((short)ProdOrderPartslistPosStates.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                    _ProdOrderPartlistPosStatesList.AddEntry((short)ProdOrderPartslistPosStates.AutoStart, "en{'Autostart'}de{'Autostart'}");
                    _ProdOrderPartlistPosStatesList.AddEntry((short)ProdOrderPartslistPosStates.PartialCompleted, "en{'Partially Completed'}de{'Teilweise abgeschlossen'}");
                }
                return _ProdOrderPartlistPosStatesList;
            }
        }
#endregion

    }
}
