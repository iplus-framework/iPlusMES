using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'ApplyMatConnectionToOtherWF'}de{'ApplyMatConnectionToOtherWF'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class ApplyMatConnectionToOtherWF
    {
        public ApplyMatConnectionToOtherWF()
        {
            WFConnection = new List<MaterialWFConnection>();
        }

        #region Material
        public MaterialWFACClassMethod MaterialWFACClassMethod { get; set; }

        [ACPropertyInfo(100, "Material", ConstApp.MaterialNo)]
        public string Material
        {
            get
            {
                if (WFConnection == null || !WFConnection.Any())
                    return "";
                string materialNo = "";
                try
                {
                    materialNo = string.Join(",", WFConnection.Select(c => c.Material.MaterialNo).Distinct());

                }
                catch (Exception)
                { 
                }
                return materialNo;
            }
        }

        #endregion

        #region Source

        [ACPropertyInfo(200, "WFConnection", "en{'WFConnection'}de{'WFConnection'}")]
        public List<MaterialWFConnection> WFConnection { get; set; }

        [ACPropertyInfo(300, "WF", "WF")]
        public gip.mes.datamodel.ACClassWF WF { get; set; }


        [ACPropertyInfo(400, "ACUrl", "en{'ACUrl'}de{'ACUrl'}")]
        public string ACUrl { get; set; }

        #endregion


    }
}
