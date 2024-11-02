// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class BackwardSortedPointInOrder : TandTPointSorted
    {
        public BackwardSortedPointInOrder(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint parentPoint, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema, TandTFilter filter)
            : base(dbApp, result, sortedResult, parentPoint, originalPoint, sortingShema, filter)
        {

        }

        public override void ProcessSortedRelated(DatabaseApp dbApp, TandTResult result, TandTResult sortedResult, TandTPoint originalPoint, TandTSortingShemaEnum sortingShema)
        {
            List<Guid> iopIDs = (originalPoint.Item as InOrder).InOrderPos_InOrder.Select(x => x.InOrderPosID).ToList();
            List<TandTPoint> pointsInOrderPos = result
                .Results
                .Where(x => iopIDs.Contains(x.ID))
                .OrderBy(x => (x.Item as InOrderPos).Sequence)
                .ToList();
            foreach (TandTPoint originalChildPoint in pointsInOrderPos)
                new BackwardSortedPointInOrderPos(dbApp, result, sortedResult, this, originalChildPoint, sortingShema, Filter);
        }
    }
}
