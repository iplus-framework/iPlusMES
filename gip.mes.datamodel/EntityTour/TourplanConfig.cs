using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gipCoreData = gip.core.datamodel;

namespace gip.mes.datamodel
{
    /// <summary>
    /// TourplanConfig
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'TourplanConfig'}de{'TourplanConfig'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, Const.PN_PreConfigACUrl, "en{'Parent WF URL'}de{'WF Eltern-URL'}","", "", true)]
    [ACPropertyEntity(2, Const.PN_LocalConfigACUrl, "en{'Property URL'}de{'Eigenschafts-URL'}","", "", true)]
    [ACPropertyEntity(3, "XMLValue", "en{'Value'}de{'Wert'}")]
    [ACPropertyEntity(4, "Expression", "en{'Expression'}de{'Ausdruck'}","", "", true)]
    [ACPropertyEntity(5, "Comment", "en{'Comment'}de{'Bemerkung'}","", "", true)]
    // 6 Source
    // 7 Value
    [ACPropertyEntity(100, Const.PN_KeyACUrl, Const.EntityKey,"", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACDeleteAction(TourplanConfig.ClassName + "_ParentTourplanConfig", Global.DeleteAction.CascadeManual)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + TourplanConfig.ClassName, "en{'TourplanConfig'}de{'TourplanConfig'}", typeof(TourplanConfig), TourplanConfig.ClassName, Const.PN_LocalConfigACUrl, Const.PN_LocalConfigACUrl)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<TourplanConfig>) })]
    public partial class TourplanConfig : IACConfig
    {
        public const string ClassName = "TourplanConfig";

        #region New/Delete
        public static TourplanConfig NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            TourplanConfig entity = new TourplanConfig();
            entity.TourplanConfigID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.XMLConfig = "";
            if (parentACObject is Tourplan)
            {
                entity.Tourplan = parentACObject as Tourplan;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
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
                if (this.Value is IACObject)
                {
                    return (this.Value as IACObject).ACCaption;
                }
                return this.LocalConfigACUrl;
            }
        }

        /// <summary>
        /// Returns Tourplan
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Tourplan</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Tourplan;
            }
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (ValueTypeACClass == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "Key",
                    Message = "Key",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "Key"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        static public string KeyACIdentifier
        {
            get
            {
                return Const.ACUrlPrefix;
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

        #region Properties
        #endregion

        #region IACConfig

        [ACPropertyInfo(101, Const.PN_ConfigACUrl, "en{'WF Property URL'}de{'WF Eigenschaft URL'}")]
        public string ConfigACUrl
        {
            get
            {
                return ACUrlHelper.BuildConfigACUrl(this);
            }
        }

        /// <summary>Gets or sets the encapsulated value as a boxed type</summary>
        /// <value>The boxed value.</value>
        [ACPropertyInfo(9999, "", "en{'Value'}de{'Wert'}")]
        public object Value
        {
            get
            {
                ACPropertyExt acPropertyExt = ACProperties.GetOrCreateACPropertyExtByName(Const.Value, false, true);
                if (acPropertyExt == null)
                    return null;
                return acPropertyExt.Value;
            }
            set
            {
                ACPropertyExt acPropertyExt = ACProperties.GetOrCreateACPropertyExtByName(Const.Value, false, true);
                if ((acPropertyExt != null && acPropertyExt.Value != value)
                    || (acPropertyExt == null && value != null))
                    ACProperties.SetACPropertyExtValue(ACProperties.GetOrCreateACPropertyExtByName(Const.Value), value);
            }
        }

        [ACPropertyInfo(6, "", "en{'Source']de{'Quelle'}")]
        public IACConfigStore ConfigStore
        {
            get
            {
                return Tourplan;
            }
        }

        /// <summary>Sets the Metadata (iPlus-Type) of the Value-Property.</summary>
        /// <param name="typeOfValue">Metadata (iPlus-Type) of the Value-Property.</param>
        public void SetValueTypeACClass(gip.core.datamodel.ACClass typeOfValue)
        {
            this.ValueTypeACClass = typeOfValue;
        }


        /// <summary>TourplanConfig-Childs</summary>
        /// <value>TourplanConfig-Childs</value>
        public IEnumerable<IACContainerWithItems> Items
        {
            get
            {
                return TourplanConfig_ParentTourplanConfig;
            }
        }

        /// <summary>Gets the parent container.</summary>
        /// <value>The parent container.</value>
        public IACContainerWithItems ParentContainer
        {
            get
            {
                return TourplanConfig1_ParentTourplanConfig;
            }
        }

        /// <summary>Gets the root container.</summary>
        /// <value>The root container.</value>
        public IACContainerWithItems RootContainer
        {
            get
            {
                if (TourplanConfig1_ParentTourplanConfig == null)
                    return this;
                return TourplanConfig1_ParentTourplanConfig.RootContainer;
            }
        }


        /// <summary>Adds the specified child-container</summary>
        /// <param name="child">The child-container</param>
        public void Add(IACContainerWithItems child)
        {
            if (child is TourplanConfig)
            {
                TourplanConfig tourplanConfig = child as TourplanConfig;
                tourplanConfig.TourplanConfig1_ParentTourplanConfig = this;
                TourplanConfig_ParentTourplanConfig.Add(tourplanConfig);
            }
        }

        /// <summary>Removes the specified child-container</summary>
        /// <param name="child">The child-container</param>
        /// <returns>true if removed</returns>
        public bool Remove(IACContainerWithItems child)
        {
            if (child is TourplanConfig)
            {
                return TourplanConfig_ParentTourplanConfig.Remove(child as TourplanConfig);
            }
            return false;
        }
        public gipCoreData.ACClass VBACClass
        {
            get
            {
                if (VBiACClassID == null) return null;
                return VBiACClass.FromIPlusContext<gipCoreData.ACClass>();
            }
        }

        public Guid? ACClassWFID
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region VBIplus-Context
        private gip.core.datamodel.ACClass _ACClass;
        [ACPropertyInfo(9999, "", "en{'Class'}de{'Klasse'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName)]
        public gip.core.datamodel.ACClass ACClass
        {
            get
            {
                if (this.VBiACClassID == null || this.VBiACClassID == Guid.Empty)
                    return null;
                if (_ACClass != null)
                    return _ACClass;
                DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                if (dbApp != null)
                    _ACClass = dbApp.ContextIPlus.GetACType(this.VBiACClassID.Value);
                return _ACClass;
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiACClass == null)
                        return;
                    _ACClass = null;
                    this.VBiACClass = null;
                }
                else
                {
                    if (_ACClass != null && value == _ACClass)
                        return;
                    gip.mes.datamodel.ACClass value2 = value.FromAppContext<gip.mes.datamodel.ACClass>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiACClassID = value.ACClassID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _ACClass = value;
                    if (value2 == this.VBiACClass)
                        return;
                    this.VBiACClass = value2;
                }
            }
        }

        partial void OnVBiACClassIDChanged()
        {
            OnPropertyChanged(gip.core.datamodel.ACClass.ClassName);
        }

        private gip.core.datamodel.ACClass _ValueTypeACClass;
        /// <summary>
        /// Metadata (iPlus-Type) of the Value-Property. ATTENTION: ACClass is a EF-Object. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>Metadata (iPlus-Type) of the Value-Property as ACClass</value>
        [ACPropertyInfo(9999, "", "en{'Datatype'}de{'Datentyp'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName)]
        public gip.core.datamodel.ACClass ValueTypeACClass
        {
            get
            {
                if (this.VBiValueTypeACClassID == null || this.VBiValueTypeACClassID == Guid.Empty)
                    return null;
                if (_ValueTypeACClass != null)
                    return _ValueTypeACClass;
                DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                if (dbApp != null)
                    _ValueTypeACClass = dbApp.ContextIPlus.GetACType(this.VBiValueTypeACClassID);
                return _ValueTypeACClass;
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiValueTypeACClass == null)
                        return;
                    _ValueTypeACClass = null;
                    this.VBiValueTypeACClass = null;
                }
                else
                {
                    if (_ValueTypeACClass != null && value == _ValueTypeACClass)
                        return;
                    gip.mes.datamodel.ACClass value2 = value.FromAppContext<gip.mes.datamodel.ACClass>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiValueTypeACClassID = value.ACClassID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _ValueTypeACClass = value;
                    if (value2 == this.VBiValueTypeACClass)
                        return;
                    this.VBiValueTypeACClass = value2;
                }
            }
        }

        partial void OnVBiValueTypeACClassIDChanged()
        {
            OnPropertyChanged("ValueTypeACClass");
        }

        private gip.core.datamodel.ACClassPropertyRelation _ACClassPropertyRelation;
        [ACPropertyInfo(9999, "", "en{'Relation'}de{'Beziehung'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClassPropertyRelation.ClassName)]
        public gip.core.datamodel.ACClassPropertyRelation ACClassPropertyRelation
        {
            get
            {
                if (this.VBiACClassPropertyRelationID == null || this.VBiACClassPropertyRelationID == Guid.Empty)
                    return null;
                if (_ACClassPropertyRelation != null)
                    return _ACClassPropertyRelation;
                if (this.VBiACClassPropertyRelation == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();

                    using (ACMonitor.Lock(dbApp.ContextIPlus.QueryLock_1X000))
                    {
                        _ACClassPropertyRelation = dbApp.ContextIPlus.ACClassPropertyRelation.Where(c => c.ACClassPropertyRelationID == this.VBiACClassPropertyRelationID).FirstOrDefault();
                    }
                    return _ACClassPropertyRelation;
                }
                else
                {
                    _ACClassPropertyRelation = this.VBiACClassPropertyRelation.FromIPlusContext<gip.core.datamodel.ACClassPropertyRelation>();
                    return _ACClassPropertyRelation;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiACClassPropertyRelation == null)
                        return;
                    _ACClassPropertyRelation = null;
                    this.VBiACClassPropertyRelation = null;
                }
                else
                {
                    if (_ACClassPropertyRelation != null && value == _ACClassPropertyRelation)
                        return;
                    gip.mes.datamodel.ACClassPropertyRelation value2 = value.FromAppContext<gip.mes.datamodel.ACClassPropertyRelation>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiACClassPropertyRelationID = value.ACClassPropertyRelationID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _ACClassPropertyRelation = value;
                    if (value2 == this.VBiACClassPropertyRelation)
                        return;
                    this.VBiACClassPropertyRelation = value2;
                }
            }
        }

        partial void OnVBiACClassPropertyRelationIDChanged()
        {
            OnPropertyChanged("ACClassPropertyRelation");
        }
        #endregion
    }
}
