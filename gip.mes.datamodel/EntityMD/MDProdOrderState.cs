using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, ConstApp.ESProdOrderState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOProdOrderState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDProdOrderStateIndex", ConstApp.ESProdOrderState, typeof(MDProdOrderState.ProdOrderStates), Const.ContextDatabase + "\\ProdOrderStatesList", "", true, MinValue = (short)MDProdOrderState.ProdOrderStates.NewCreated)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + MDProdOrderState.ClassName, ConstApp.ESProdOrderState, typeof(MDProdOrderState), MDProdOrderState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDProdOrderState>) })]
    [NotMapped]
    public partial class MDProdOrderState
    {
        [NotMapped]
        public const string ClassName = "MDProdOrderState";

        #region New/Delete
        public static MDProdOrderState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDProdOrderState entity = new MDProdOrderState();
            entity.MDProdOrderStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.ProdOrderState = ProdOrderStates.NewCreated;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IQueryable<MDProdOrderState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IQueryable<MDProdOrderState>>(
            (database) => from c in database.MDProdOrderState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IQueryable<MDProdOrderState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IQueryable<MDProdOrderState>>(
            (database, index) => from c in database.MDProdOrderState where c.MDProdOrderStateIndex == index select c
        );

        public static MDProdOrderState DefaultMDProdOrderState(DatabaseApp dbApp)
        {
            try
            {
                MDProdOrderState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)ProdOrderStates.NewCreated).FirstOrDefault();
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
                return MDProdOrderStateName;
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
        public String MDProdOrderStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDProdOrderStateName");
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
        public ProdOrderStates ProdOrderState
        {
            get
            {
                return (ProdOrderStates)MDProdOrderStateIndex;
            }
            set
            {
                MDProdOrderStateIndex = (short)value;
                OnPropertyChanged("ProdOrderState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDProdOrderStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'ProdOrderStates'}de{'ProdOrderStates'}", Global.ACKinds.TACEnum)]
        public enum ProdOrderStates : short
        {
            NewCreated = 1,         //Neu angelegt
            PrepareBatch = 2,       //Vorbereitung Produktionschargen
            BatchPrepared = 3,      //Produktionsbereit
            InProduction = 4,       //In Produktion
            ProdFinished = 5,       //Produktion beendet
            WorkOrderFinished = 6,  //Auftrag erledigt
            Rated = 7,              //Bewertet
            Cancelled = 8           //Storniert
        }

        [NotMapped]
        static ACValueItemList _ProdOrderStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück.
        /// </summary>
        [NotMapped]
        public static ACValueItemList ProdOrderStatesList
        {
            get
            {
                if (_ProdOrderStatesList == null)
                {
                    _ProdOrderStatesList = new ACValueItemList("ProdOrderStates");
                    _ProdOrderStatesList.AddEntry((short)ProdOrderStates.NewCreated, "en{'New Created'}de{'Neu angelegt'}");
                    _ProdOrderStatesList.AddEntry((short)ProdOrderStates.PrepareBatch, "en{'Prepare Batch'}de{'Vorbereitung Produktionschargen'}");
                    _ProdOrderStatesList.AddEntry((short)ProdOrderStates.BatchPrepared, "en{'Batch Prepared'}de{'Produktionsbereit'}");
                    _ProdOrderStatesList.AddEntry((short)ProdOrderStates.InProduction, "en{'In Production'}de{'In Produktion'}");
                    _ProdOrderStatesList.AddEntry((short)ProdOrderStates.ProdFinished, "en{'Production Finished'}de{'Produktion beendet'}");
                    _ProdOrderStatesList.AddEntry((short)ProdOrderStates.WorkOrderFinished, "en{'Work Order Finished'}de{'Auftrag erledigt'}");
                    _ProdOrderStatesList.AddEntry((short)ProdOrderStates.Rated, "en{'Rated'}de{'Bewertet'}");
                    _ProdOrderStatesList.AddEntry((short)ProdOrderStates.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return _ProdOrderStatesList;
            }
        }
#endregion

    }
}
