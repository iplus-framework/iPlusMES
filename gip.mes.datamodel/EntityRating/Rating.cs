using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Rating'}de{'Bewertung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSORating")]
    [ACPropertyEntity(1, "Score", "en{'Score'}de{'Punktestand'}","", "", true)]
    [ACPropertyEntity(2, Company.ClassName, "en{'Company'}de{'Unternehmen'}", Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(3, CompanyPerson.ClassName, "en{'Person'}de{'Person'}", Const.ContextDatabase + "\\" + CompanyPerson.ClassName, "", true)]
    [ACPropertyEntity(4, DeliveryNote.ClassName, "en{'Indeliverynote'}de{'Eingangslieferschein'}", Const.ContextDatabase + "\\" + DeliveryNote.ClassName, "", true)]
    [ACPropertyEntity(5, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + Rating.ClassName, "en{'Rating'}de{'Bewertung'}", typeof(Rating), Rating.ClassName, Const.EntityInsertDate, Const.EntityInsertDate, new object[]
        {
                new object[] {Const.QueryPrefix + RatingComplaint.ClassName, "en{'Rating Complaint'}de{'Beanstandung'}", typeof(TourplanPos), RatingComplaint.ClassName + "_" + Rating.ClassName, Const.EntityInsertDate, Const.EntityInsertDate}
        }
    )]
    public partial class Rating
    {
        public const string ClassName = "Rating";

        #region New/Delete
        public static Rating NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            Rating entity = new Rating();
            entity.RatingID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        
        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            string toString = @"Rating for {0} {1} {2} is {3}";
            toString = string.Format(
                toString,
                Company != null ? Company.ACCaption : "-",
                CompanyPerson != null ? CompanyPerson.ACCaption : "",
                DeliveryNote != null ? DeliveryNote.ACCaption : "", 
                Score);
            return toString;
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

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == RatingComplaint.ClassName)
            {
                DateTime insertDate = DateTime.MinValue;
                if (DateTime.TryParse(filterValues[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out insertDate))
                    return this.RatingComplaint_Rating.Where(c => c.InsertDate == insertDate).FirstOrDefault();
            }
            return null;
        }

        #endregion
  
    }
}
