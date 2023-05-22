using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, ConstApp.ESDemandOrderState, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSODemandOrderState")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDDemandOrderStateIndex", ConstApp.ESDemandOrderState, typeof(MDDemandOrderState.DemandOrderStates), Const.ContextDatabase + "\\DemandOrderStatesList", "", true, MinValue = (short)DemandOrderStates.Proposal)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + MDDemandOrderState.ClassName, ConstApp.ESDemandOrderState, typeof(MDDemandOrderState), MDDemandOrderState.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDDemandOrderState>) })]
    [NotMapped]
    public partial class MDDemandOrderState
    {
        [NotMapped]
        public const string ClassName = "MDDemandOrderState";

        #region New/Delete
        public static MDDemandOrderState NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDDemandOrderState entity = new MDDemandOrderState();
            entity.MDDemandOrderStateID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.DemandOrderState = DemandOrderStates.Proposal;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDDemandOrderState>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDDemandOrderState>>(
            (database) => from c in database.MDDemandOrderState where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDDemandOrderState>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDDemandOrderState>>(
            (database, index) => from c in database.MDDemandOrderState where c.MDDemandOrderStateIndex == index select c
        );

        public static MDDemandOrderState DefaultMDDemandOrderState(DatabaseApp dbApp)
        {
            try
            {
                MDDemandOrderState defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)DemandOrderStates.Proposal).FirstOrDefault();
                return defaultObj;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MDDemandOrderState", "DefaultMDDemandOrderState", msg);
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
                return MDDemandOrderStateName;
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

        /*
         * Onlinehelp: Definition von hartcodierten Wertebereichen von Entities
         * 
         * Vergangenheit:
         * Im alte System wurden Wertebereiche in einem Stringfeld in Tabelle XFelder gespeichert 
         * (z.B. :Absolut:Prozent:) und der Numerische Index wurde einfach durchnummeriert
         * An diesem Index hängt aber in den meisten Fällen eine hartcodierte Programmlogik,
         * die je Nach Wertebereich sich in viele Quellcodedateien befindet.
         * Wird in der Datenbank ein Stringfeld geändert, kann dies gravierende Auswirkungen
         * auf die Stabilität oder die Programmlogik haben, bis hin zum Absturz.
         * 
         * Gegenwart und Zukunft:
         * Im VarioBatch 2008 erfolgt sind auch die Wertebereich hartcodiert, wobei die 
         * Bezeichnungen und die Zuordnung weiterhin in der Datenbank, nun aber in normalisierten
         * Tabellen, wie in diesem Fall "MDAbsoluteOrPercent" gespeichert. Die Tabelle hat immer
         * einen Const.SortIndex zur Sortierung der Einträge innerhalb einer Combobox und den fachlichen 
         * Indexe "MDAbsoluteOrPercentIndex", für den ein Enum bereitgestellt wird. 
         * 
         * Grundsätzlich können für einen fachlichen Index auch mehrere Datensätze gespeichert sein.
         * Ein Anwendungsfäll wäre zum Beispiel bei den Lagerbuchungsarten (MDFacilityBookingType), wo
         * eine Einbuchung "Anlieferung durch Fremdunternehmen" und "Anlieferung durch Tochterunternehmen" 
         * für den fachlichen Index "Einbuchung" definierbar wäre. Die Programmlogik verhält sich
         * bei beiden Buchungsarten identisch, aber es kann auch bei Auswertungen differenziert werden.
         * 
         * Die Abfrage des fachlichen Index MUSS im Quelltext immer explizit (== oder Case) und 
         * unter Verwendung des enum´s erfolgen, da eine Abfrage > 1 dazu führt, das die 
         * Erweiterung eines fachlichen Index zu einem Fehlverhalten führen kann. Bei einer Erweiterung
         * den gesamten Quelltext nach dem enum durchsuchen und ALLE betroffenen Stellen anpassen.
         * 
         * Eine kundenspezifische Erweiterung des fachlichen Index darf es NICHT geben, da hiervon
         * Standardquellcode betroffen ist. Erweiterungen sind immer so zu planen, das diese 
         * auch in den Standard einfließen können.
         * 
         * Die enums beginnen IMMER mit dem Wert "1" und besitzen nie den Wert "0", welcher
         * IMMER für "Undefiniert" steht.
         * 
         * In EntityCheckAdded und EntityCheckModified wird jeweils die Prüffunktion
         * (IndexList.CheckIndex(MDAbsoluteOrPercentIndex);) aufgerufen, damit keine 
         * falschen Werte gespeichert werden können.
         */
        #endregion

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDDemandOrderStateName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDDemandOrderStateName");
            }
        }

#endregion

#region enums
        [NotMapped]
        public DemandOrderStates DemandOrderState
        {
            get
            {
                return (DemandOrderStates)MDDemandOrderStateIndex;
            }
            set
            {
                MDDemandOrderStateIndex = (short)value;
                OnPropertyChanged("DemandOrderState");
            }
        }

        /// <summary>
        /// Enum für das Feld MDDemandOrderStateIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'DemandOrderStates'}de{'DemandOrderStates'}", Global.ACKinds.TACEnum)]
        public enum DemandOrderStates : short
        {
            Proposal = 1,
            Active = 2,
            InActive = 3,
            Finished = 4,
        }

        [NotMapped]
        static ACValueItemList _DemandOrderStatesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        [NotMapped]
        public static ACValueItemList DemandOrderStatesList
        {
            get
            {
                if (_DemandOrderStatesList == null)
                {
                    _DemandOrderStatesList = new ACValueItemList("DemandOrderStates");
                    _DemandOrderStatesList.AddEntry((short)DemandOrderStates.Proposal, "en{'Proposal'}de{'Vorschlag'}");
                    _DemandOrderStatesList.AddEntry((short)DemandOrderStates.Active, "en{'Active'}de{'Aktiv'}");
                    _DemandOrderStatesList.AddEntry((short)DemandOrderStates.InActive, "en{'Inactive'}de{'Inaktiv'}");
                    _DemandOrderStatesList.AddEntry((short)DemandOrderStates.Finished, "en{'Finished'}de{'Beendet'}");
                }
                return _DemandOrderStatesList;
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

    }
}
