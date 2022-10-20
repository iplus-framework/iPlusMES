using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Diese partielle Klasse beenhaltet Methoden zum Geneireren und Manipulieren
    /// von Bewegungssätzen FacilityBooking und FacilityBookingCharge
    /// </summary>
    public partial class FacilityManager
    {
        public static ACComponent GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACComponent>(requester, HelperIFacilityManager.C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACComponent> ACRefToServiceInstance(ACComponent requester)
        {
            ACComponent serviceInstance = GetServiceInstance(requester) as ACComponent;
            if (serviceInstance != null)
                return new ACRef<ACComponent>(serviceInstance, requester);
            return null;
        }
    }
}
