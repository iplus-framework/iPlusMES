using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'MixPointLabOrder'}de{'MixPointLabOrder'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class MixPointLabOrder
    {
        public Guid ID { get; set; }

        [ACPropertyInfo(9999, "ACIdentifier", "en{'Entity'}de{'Entität'}")]
        public string ACIdentifier { get; set; }

        [ACPropertyInfo(9999, "ACCaption", "en{'Description'}de{'Beschreibung'}")]
        public string ACCaption { get; set; }


        public Dictionary<LabOrder, List<LabOrderPos>> LabOrderWithItems { get; set; }


        public static MixPointLabOrder Factory(DatabaseApp databaseApp, InOrderPos inOrderPos)
        {
            return new MixPointLabOrder()
            {
                ACIdentifier = inOrderPos.ACIdentifier,
                ACCaption = inOrderPos.ACCaption,
                ID = inOrderPos.InOrderPosID,
                LabOrderWithItems =
                        inOrderPos
                        .LabOrder_InOrderPos
                        .ToDictionary(
                            key => key,
                            val => databaseApp
                                    .LabOrderPos
                                    .Include(c => c.MDLabOrderPosState)
                                    .Include(c => c.MDLabTag)
                                    .Where(c => c.LabOrderID == val.LabOrderID)
                                    .OrderBy(c => c.Sequence)
                                    .ToList())
            };
        }


        public static MixPointLabOrder Factory(DatabaseApp databaseApp, ProdOrderPartslistPos pos)
        {
            return new MixPointLabOrder()
            {
                ACIdentifier = pos.ACIdentifier,
                ACCaption = pos.ACCaption,
                ID = pos.ProdOrderPartslistPosID,
                LabOrderWithItems =
                           pos
                           .LabOrder_ProdOrderPartslistPos
                           .ToDictionary(
                               key => key,
                               val => databaseApp
                                        .LabOrderPos
                                        .Include(c => c.MDLabOrderPosState)
                                        .Include(c => c.MDLabTag)
                                        .Where(c => c.LabOrderID == val.LabOrderID)
                                        .OrderBy(c => c.Sequence)
                                        .ToList())
            };
        }

        // TODO: make version for outorder
    }
}
