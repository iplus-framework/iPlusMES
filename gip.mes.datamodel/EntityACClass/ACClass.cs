using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public partial class ACClass
    {
        public void CopyFrom(ACClass from, bool withReferences)
        {
            if (withReferences)
            {
                ACProjectID = ACProjectID;
                BasedOnACClassID = BasedOnACClassID;
                ParentACClassID = ParentACClassID;
                PWMethodACClassID = PWMethodACClassID;
                ACPackageID = ACPackageID;
                PWACClassID = PWACClassID;
            }

            ACIdentifier = ACIdentifier;
            ACIdentifierKey = ACIdentifierKey;
            ACCaptionTranslation = ACCaptionTranslation;
            ACKindIndex = ACKindIndex;
            SortIndex = SortIndex;
            AssemblyQualifiedName = AssemblyQualifiedName;
            Comment = Comment;
            IsAutostart = IsAutostart;
            IsAbstract = IsAbstract;
            ACStartTypeIndex = ACStartTypeIndex;
            ACStorableTypeIndex = ACStorableTypeIndex;
            IsAssembly = IsAssembly;
            IsMultiInstance = IsMultiInstance;
            IsRightmanagement = IsRightmanagement;
            ACSortColumns = ACSortColumns;
            ACFilterColumns = ACFilterColumns;
            XMLConfig = XMLConfig;
            XMLACClass = XMLACClass;
            BranchNo = BranchNo;
            InsertName = InsertName;
            InsertDate = InsertDate;
            UpdateName = UpdateName;
            UpdateDate = UpdateDate;
            ChangeLogMax = ChangeLogMax;
            ACURLCached = ACURLCached;
            ACURLComponentCached = ACURLComponentCached;
            IsStatic = IsStatic;
        }

        public void CopyTo(gip.core.datamodel.ACClass to, bool withReferences)
        {
            if (withReferences)
            {
                to.ACProjectID = ACProjectID;
                to.BasedOnACClassID = BasedOnACClassID;
                to.ParentACClassID = ParentACClassID;
                to.PWMethodACClassID = PWMethodACClassID;
                to.ACPackageID = ACPackageID;
                to.PWACClassID = PWACClassID;
            }

            to.ACIdentifier = ACIdentifier;
            to.ACIdentifierKey = ACIdentifierKey;
            to.ACCaptionTranslation = ACCaptionTranslation;
            to.ACKindIndex = ACKindIndex;
            to.SortIndex = SortIndex;
            to.AssemblyQualifiedName = AssemblyQualifiedName;
            to.Comment = Comment;
            to.IsAutostart = IsAutostart;
            to.IsAbstract = IsAbstract;
            to.ACStartTypeIndex = ACStartTypeIndex;
            to.ACStorableTypeIndex = ACStorableTypeIndex;
            to.IsAssembly = IsAssembly;
            to.IsMultiInstance = IsMultiInstance;
            to.IsRightmanagement = IsRightmanagement;
            to.ACSortColumns = ACSortColumns;
            to.ACFilterColumns = ACFilterColumns;
            to.XMLConfig = XMLConfig;
            to.XMLACClass = XMLACClass;
            to.BranchNo = BranchNo;
            to.InsertName = InsertName;
            to.InsertDate = InsertDate;
            to.UpdateName = UpdateName;
            to.UpdateDate = UpdateDate;
            to.ChangeLogMax = ChangeLogMax;
            to.ACURLCached = ACURLCached;
            to.ACURLComponentCached = ACURLComponentCached;
            to.IsStatic = IsStatic;
        }
    }
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
