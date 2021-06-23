using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Workflow-Root für Single dosing
    /// </summary>
    [ACClassConstructorInfo(
        new object[]
        {
            new object[] {gip.core.datamodel.ACProgram.ClassName, Global.ParamOption.Required, typeof(Guid)},
            new object[] {gip.core.datamodel.ACProgramLog.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {FacilityBooking.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {Picking.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {PickingPos.ClassName, Global.ParamOption.Optional, typeof(Guid),
            new object[] {core.datamodel.ACClassWF.ClassName, Global.ParamOption.Optional, typeof(Guid)}}
        }
    )]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Single dosing'}de{'Einzeldosierung'}", Global.ACKinds.TPWMethod, Global.ACStorableTypes.Required, true, true, "", "ACProgram", 40)]
    public class PWMethodSingleDosing : PWMethodRelocation
    {
        public PWMethodSingleDosing(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        public Guid? SelectedSingleDosingACClassWFID
        {
            get
            {
                ACValue acclassWF = CurrentACMethod?.ValueT?.ParameterValueList.GetACValue(core.datamodel.ACClassWF.ClassName);
                if (acclassWF != null && acclassWF.Value != null && ContentACClassWF != null)
                {
                    return acclassWF.ParamAsGuid;
                }
                return null;
            }
        }
    }
}
