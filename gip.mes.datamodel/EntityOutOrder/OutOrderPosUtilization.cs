using gip.core.datamodel;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    // OutOrderPosUtilizationNo,OutOrderPos\\Sequence,User,TimeFrom,TimeTo,Material\\MaterialNo,Material\\MaterialName1,ActualQuantity,ActualWeight,Comment,IsToBill,IsBilled"; }
    [ACClassInfo(Const.PackName_VarioSales, "en{'Outorderposutilization'}de{'Auftragspositionaufwand'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "OutOrderPosUtilizationNo", "en{'OutOrderPosUtilizationNo'}de{'de-OutOrderPosUtilizationNo'}", "", "", true)]
    [ACPropertyEntity(2, "ActualQuantity", ConstApp.ActualQuantity, "", "", true)]
    [ACPropertyEntity(3, "ActualWeight", "en{'ActualWeight'}de{'de-ActualWeight'}", "", "", true)]
    [ACPropertyEntity(4, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(5, "IsBilled", "en{'Billed'}de{'Berechnet'}", "", "", true)]
    [ACPropertyEntity(6, "IsToBill", "en{'To Bill'}de{'Zu berechnen'}", "", "", true)]
    [ACPropertyEntity(7, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(8, "TimeFrom", "en{'Time From'}de{'Von Zeit'}", "", "", true)]
    [ACPropertyEntity(9, "TimeTo", "en{'Time To'}de{'Bis Zeit'}", "", "", true)]
    [ACPropertyEntity(9999, OutOrderPos.ClassName, "en{'Outorderpos'}de{'Auftragsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + OutOrderPosUtilization.ClassName, "en{'Outorderposutilization'}de{'Auftragspositionaufwand'}", typeof(OutOrderPosUtilization), OutOrderPosUtilization.ClassName, "OutOrderPosUtilizationNo", "OutOrderPosUtilizationNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OutOrderPosUtilization>) })]
    public partial class OutOrderPosUtilization
    {
        public const string ClassName = "OutOrderPosUtilization";
        public const string NoColumnName = "OutOrderPosUtilizationNo";
        public const string FormatNewNo = "OPU{0}";


        #region New/Delete
        public static OutOrderPosUtilization NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            OutOrderPosUtilization entity = new OutOrderPosUtilization();
            entity.OutOrderPosUtilizationID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is OutOrderPos)
            {
                entity.OutOrderPos = parentACObject as OutOrderPos;
            }
            entity.OutOrderPosUtilizationNo = secondaryKey;
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
                return OutOrderPosUtilizationNo;
            }
        }

        /// <summary>
        /// Returns OutOrderPos
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to OutOrderPos</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return OutOrderPos;
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
            if (string.IsNullOrEmpty(Comment))
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
            return null;
        }

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "OutOrderPosUtilizationNo";
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
