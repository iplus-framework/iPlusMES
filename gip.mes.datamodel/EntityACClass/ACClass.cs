using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    //class gip.core.datamodel.ACClass
    //{

    //    /// <summary>
    //    /// Gets or sets the facility.
    //    /// </summary>
    //    /// <value>The facility.</value>
    //    [ACPropertyInfo(9999, "", "en{'Facility'}de{'Facility'}", "", false)]
    //    public Facility Facility
    //    {
    //        get
    //        {
    //            if (this.Facility_FacilityACClass.Any())
    //                return this.Facility_FacilityACClass.First();
    //            return null;
    //        }
    //        set
    //        {
    //            Facility oldFacility = null;
    //            Facility newFacility = value;
    //            if (this.Facility_FacilityACClass.Any())
    //            {
    //                oldFacility = this.Facility_FacilityACClass.First();
    //            }
    //            if (oldFacility != newFacility)
    //            {
    //                if (newFacility.FacilityACClass != null && newFacility.FacilityACClass.ACKind == Global.ACKinds.TACFacility)
    //                {
    //                    newFacility.FacilityACClass.DeleteACObject(Database, false);
    //                }
    //                newFacility.FacilityACClass = this;
    //                OnPropertyChanged(Facility.ClassName);
    //            }
    //            if (oldFacility != null)
    //            {
    //                gip.core.datamodel.ACClass.NewACObjectForFacility(this.Database(), oldFacility);
    //            }
    //        }
    //    }

    //    //            [ACPropertyEntity(9999, Facility.ClassName, "en{'Facility'}de{'Lagerplatz'}", Const.ContextDatabase + "\\" + Facility.ClassName)]

    //    /// <summary>
    //    /// Gets or sets the facility.
    //    /// </summary>
    //    /// <value>The facility.</value>
    //    [ACPropertyInfo(9999, "", "en{'Facility'}de{'Facility'}", "", false)]
    //    public Facility Facility
    //    {
    //        get
    //        {
    //            if (this.Facility_FacilityACClass.Any())
    //                return this.Facility_FacilityACClass.First();
    //            return null;
    //        }
    //        set
    //        {
    //            Facility oldFacility = null;
    //            Facility newFacility = value;
    //            if (this.Facility_FacilityACClass.Any())
    //            {
    //                oldFacility = this.Facility_FacilityACClass.First();
    //            }
    //            if (oldFacility != newFacility)
    //            {
    //                if (newFacility.FacilityACClass != null && newFacility.FacilityACClass.ACKind == Global.ACKinds.TACFacility)
    //                {
    //                    newFacility.FacilityACClass.DeleteACObject(Database, false);
    //                }
    //                newFacility.FacilityACClass = this;
    //                OnPropertyChanged(Facility.ClassName);
    //            }
    //            if (oldFacility != null)
    //            {
    //                gip.core.datamodel.ACClass.NewACObjectForFacility(this.Database(), oldFacility);
    //            }
    //        }
    //    }

    //}
}
