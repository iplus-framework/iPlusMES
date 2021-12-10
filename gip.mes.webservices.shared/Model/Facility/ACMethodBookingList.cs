using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [CollectionDataContract(Name = "cBPmL")]
    public class ACMethodBookingList : List<ACMethodBooking>
    {
        public ACMethodBookingList() : base ()
        {

        }
    }
}
