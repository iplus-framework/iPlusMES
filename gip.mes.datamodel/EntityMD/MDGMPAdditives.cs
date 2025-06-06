using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.ESGMPAdditives, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOGMPAdditive")]
    [ACPropertyEntity(9999, Const.MDNameTrans, Const.EntityNameTrans, "", "", true)]
    [ACPropertyEntity(2, "MDGMPAdditiveNo", "en{'Additive No.'}de{'Zusatzstoff Nr.'}", "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "DeleteLevel", "en{'Delete Level'}de{'Löschebene'}", "", "", true)]
    [ACPropertyEntity(5, "MDProcessErrorAction", "en{'Action on Process Error'}de{'Aktion bei Prozessfehler'}", Const.ContextDatabase + "\\MDProcessErrorAction" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, "MDQuantityUnit", ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(7, "SafetyFactor", "en{'Safety Factor'}de{'Sicherheitsfaktor'}", "", "", true)]
    [ACPropertyEntity(8, Const.MDKey, Const.EntityKey, "", "", true, MinLength = 1)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MDGMPAdditive.ClassName, ConstApp.ESGMPAdditives, typeof(MDGMPAdditive), MDGMPAdditive.ClassName, "MDGMPAdditiveNo", "MDGMPAdditiveNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDGMPAdditive>) })]
    [NotMapped]
    public partial class MDGMPAdditive
    {
        [NotMapped]
        public const string ClassName = "MDGMPAdditive";

        #region New/Delete
        public static MDGMPAdditive NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDGMPAdditive entity = new MDGMPAdditive();
            entity.MDGMPAdditiveID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDProcessErrorAction = MDProcessErrorAction.DefaultMDProcessErrorAction(dbApp);
            //entity.MDQuantityUnit = MDUnit.DefaultMDQuantityUnit(database);
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return MDGMPAdditiveName;
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
            if (string.IsNullOrEmpty(MDGMPAdditiveName))
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

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "MDGMPAdditiveNo";
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

        #region AdditionalProperties
        [ACPropertyInfo(1, "", "en{'Name'}de{'Bezeichnung'}", MinLength = 1)]
        [NotMapped]
        public String MDGMPAdditiveName
        {
            get
            {
                return Translator.GetTranslation(MDNameTrans);
            }
            set
            {
                MDNameTrans = Translator.SetTranslation(MDNameTrans, value);
                OnPropertyChanged("MDGMPAdditiveName");
            }
        }

        #endregion

    }
}
