using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESMaterialGroup, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOMaterialGroup")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true, MinLength = 1)]
    [ACPropertyEntity(5, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "IsNotPercantage", "en{'Not Percantage'}de{'Keine Prozente'}", "", "", true)]
    [ACPropertyEntity(3, Const.SortIndex, Const.EntitySortSequence, "", "", true)]
    [ACPropertyEntity(4, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(5, "MDMaterialGroupIndex", ConstApp.ESMaterialGroup, typeof(MDMaterialGroup.MaterialGroupTypes), Const.ContextDatabase + "\\MaterialGroupTypesList", "", true, MinValue = (short)MaterialGroupTypes.Undefined)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDMaterialGroup.ClassName, ConstApp.ESMaterialGroup, typeof(MDMaterialGroup), MDMaterialGroup.ClassName, Const.MDNameTrans, Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDMaterialGroup>) })]
    [NotMapped]
    public partial class MDMaterialGroup : IImageInfo
    {
        [NotMapped]
        public const string ClassName = "MDMaterialGroup";

        #region New/Delete
        public static MDMaterialGroup NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDMaterialGroup entity = new MDMaterialGroup();
            entity.MDMaterialGroupID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.IsDefault = false;
            entity.MaterialGroupType = MaterialGroupTypes.Undefined;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        static readonly Func<DatabaseApp, IEnumerable<MDMaterialGroup>> s_cQry_Default =
            EF.CompileQuery<DatabaseApp, IEnumerable<MDMaterialGroup>>(
            (database) => from c in database.MDMaterialGroup where c.IsDefault select c
        );

        static readonly Func<DatabaseApp, short, IEnumerable<MDMaterialGroup>> s_cQry_Index =
            EF.CompileQuery<DatabaseApp, short, IEnumerable<MDMaterialGroup>>(
            (database, index) => from c in database.MDMaterialGroup where c.MDMaterialGroupIndex == index select c
        );

        public static MDMaterialGroup DefaultMDMaterialGroup(DatabaseApp dbApp)
        {
            try
            {
                MDMaterialGroup defaultObj = s_cQry_Default(dbApp).FirstOrDefault();
                if (defaultObj == null)
                    defaultObj = s_cQry_Index(dbApp, (short)MaterialGroupTypes.Textarticle).FirstOrDefault();
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
                return MDMaterialGroupName;
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
        public String MDMaterialGroupName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDMaterialGroupName");
            }
        }

        #endregion

        #region enums
        [NotMapped]
        public MaterialGroupTypes MaterialGroupType
        {
            get
            {
                return (MaterialGroupTypes)MDMaterialGroupIndex;
            }
            set
            {
                MDMaterialGroupIndex = (short)value;
                OnPropertyChanged("MaterialGroupType");
            }
        }

        /// <summary>
        /// Enum für das Feld MDMaterialGroupIndex
        /// </summary>
        [ACClassInfo(Const.PackName_VarioSystem, "en{'MaterialGroupTypes'}de{'MaterialGroupTypes'}", Global.ACKinds.TACEnum)]
        public enum MaterialGroupTypes : short
        {
            Undefined = 0,
            RawMaterial = 1,
            IntermediateProduct = 2,
            SemiFinishedProduct = 3,
            FinishedProduct = 4,
            PackagingMeans = 5,
            MeansOfTransport = 6,
            MiscGoods = 7,
            Merchandise = 8,
            Service = 9,
            Rework = 20,
            /// <summary>
            /// The purpose of this group is to find material in:
            /// 1. BSOWorkCenter - PickingByMaterial: that material which contains a configuration of the combination ACClass(machine or workplace) and planning workflow node
            /// 2. BSOPicking: that intermediate material in the Material-Workflow which is intended for the Picking
            /// </summary>
            Picking = 30,
            //Services = 100,
            Textarticle = 110,
            Licence = 120       // Nur für Softwarelizenzen
        }

        [NotMapped]
        static ACValueItemList _MaterialGroupTypesList = null;

        [NotMapped]
        public static ACValueItemList MaterialGroupTypesList
        {
            get
            {
                if (_MaterialGroupTypesList == null)
                {
                    _MaterialGroupTypesList = new ACValueItemList("MaterialGroupTypes");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.Undefined, "en{'Undefined'}de{'Nicht definiert'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.RawMaterial, "en{'Raw Material'}de{'Rohmaterial'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.IntermediateProduct, "en{'Intermediate Product'}de{'Zwischenprodukt'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.SemiFinishedProduct, "en{'Semi Finished Product'}de{'Halbfertigware'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.FinishedProduct, "en{'Finished Product'}de{'Fertigware'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.PackagingMeans, "en{'Packaging Means'}de{'Verpackungsmittel'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.MeansOfTransport, "en{'Means of Transport'}de{'Transportmittel'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.MiscGoods, "en{'Misc. Goods'}de{'Sonstige Waren'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.Merchandise, "en{'Merchandise'}de{'Handelsware'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.Service, "en{'Service'}de{'Dienstleistung'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.Rework, "en{'Rework'}de{'Rework'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.Textarticle, "en{'Text Article'}de{'Textartikel'}");
                    _MaterialGroupTypesList.AddEntry((short)MaterialGroupTypes.Licence, "en{'License'}de{'Lizenz'}");

                }
                return _MaterialGroupTypesList;
            }
        }

        #endregion

        /* Onlinehelp: Definition und Verwendung kundenspezifischer Spalten in Enitäten
         * Damit kundRenspezifische Spalten nicht immer zu Datenbankänderungen führen, gibt
         * es bei den meisten Tabellen eine Spalte "XMLConfig", welche beliebige Daten im 
         * XML-Format aufnehmen kann.
         * 
         * 1. Arbeiten im Quellcode
         * 
         * MDMaterialGroup MDMaterialGroup = MDMaterialGroup.NewMDMaterialGroup(Database);
         * 
         * // Definition und/oder Wertzuweisung
         * MDMaterialGroup["Kundenwert"] = "100";
         * 
         * // Zugriff auf einen Wert
         * string wert = MDMaterialGroup["Kundenwert"];
         *
         * // Damit auch Typsicher gearbeitet werden kann gibt es auch die Möglichkeit
         * // über den DataHelper auf den Wert zuzugreifen.
         * int doppelterWert = MDMaterialGroup.DataHelper.Int["Kundenwert"] *2;
         * 
         * 2. Verwendung in XAML
         * Anstelle wie bei einem normalen Datenbankfeld "CurrentArticleType.Kundenwert", ist beim
         * VBContent das Feld mit [] zu definieren:
         *           
         * <vb:VBTextBox Grid.Column="0" Grid.Row="2"  VBContent="CurrentArticleType[Kundenwert]"></vb:VBTextBox>
         * 
         * Wie bei normalen Entitätsfeldern wird auch die Feldeigenschaften in der Tabelle SolutionProperty abgelegt,
         * so das auch hier die Captions zentral definierbar sind.
         */
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

        #region Image

        [NotMapped]
        public bool IsImageLoaded;

        [NotMapped]
        private string _DefaultImage;
        /// <summary>
        /// Doc  DefaultImage
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "DefaultImage", "en{'DefaultImage'}de{'DefaultImage'}")]
        [NotMapped]
        public string DefaultImage
        {
            get
            {
                return _DefaultImage;
            }
            set
            {
                if (_DefaultImage != value)
                {
                    _DefaultImage = value;
                    OnPropertyChanged("DefaultImage");
                }
            }
        }


        [NotMapped]
        private string _DefaultThumbImage;
        /// <summary>
        /// Doc  DefaultThumbImage
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "DefaultThumbImage", "en{'DefaultThumbImage'}de{'DefaultThumbImage'}")]
        [NotMapped]
        public string DefaultThumbImage
        {
            get
            {
                return _DefaultThumbImage;
            }
            set
            {
                if (_DefaultThumbImage != value)
                {
                    _DefaultThumbImage = value;
                    OnPropertyChanged("DefaultThumbImage");
                }
            }
        }


        #endregion


    }
}




