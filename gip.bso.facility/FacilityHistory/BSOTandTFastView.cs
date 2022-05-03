using gip.core.datamodel;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'ProdOrder Overview'}de{'Zwischenmaterial Übersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOTandTFastView : ACBSOvb
    {


        #region c´tors

        /// <summary>
        /// Konstruktor für ACComponent
        /// (Gleiche Signatur, wie beim ACGenericObject)
        /// </summary>
        /// <param name="acType">ACType anhand dessen die Methoden, Properties und Designs initialisiert werden</param>
        /// <param name="content">Inhalt
        /// Bei Model- oder BSO immer gleich ACClass
        /// Bei WF immer WorkOrderWF</param>
        /// <param name="parentACObject">Lebende ACComponent-Instanz</param>
        /// <param name="parameter">Parameter je nach Ableitungsimplementierung</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOTandTFastView(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion
    }
}
