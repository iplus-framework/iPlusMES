using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Rating Complaint'}de{'Beanstandung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSORatingComplaint")]
    [ACPropertyEntity(1, "Score", "en{'Score'}de{'Punktestand'}","", "", true)]
    [ACPropertyEntity(2, "MDRatingComplaintType", "en{'Rating Complaint Type'}de{'Beanstandungstyp'}", Const.ContextDatabase + "\\" + MDRatingComplaintType.ClassName, "", true)]
    [ACPropertyEntity(4, "Rating", "en{'Rating'}de{'Rating'}", Const.ContextDatabase + "\\" + Rating.ClassName, "", true)]
    [ACPropertyEntity(3, "Comment", "en{'Comment'}de{'Kommentar'}","", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + RatingComplaint.ClassName, "en{'Rating Complaint'}de{'Beanstandung'}", typeof(RatingComplaint), RatingComplaint.ClassName, Const.EntityInsertDate, Const.EntityInsertDate)]
    public partial class RatingComplaint
    {
        public const string ClassName = "RatingComplaint";

        #region New/Delete
        public static RatingComplaint NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            RatingComplaint entity = new RatingComplaint();
            
            entity.RatingComplaintID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            if (Rating == null) 
                return "";
            return string.Format(@"{0} comment {1} : {2}", Rating.ACCaption, MDRatingComplaintType != null ? MDRatingComplaintType.ACCaption : "-", Comment ?? "");
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return ToString();
            }
        }

        #endregion

    }
}
