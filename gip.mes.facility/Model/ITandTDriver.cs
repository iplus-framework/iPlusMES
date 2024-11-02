// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public interface ITandTDriver
    {

        #region Facility(Pre)Booking
        TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, FacilityBooking facilityBooking, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter);

        TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, FacilityPreBooking facilitiyPreBooking, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter);

        #endregion

        #region In(Out)OrderPos
        TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, InOrderPos inOrderPos, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter);

        TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, OutOrderPos outOrderPos, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter);

        TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, DeliveryNotePos deliveryNotePos, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter);

        #endregion

        #region Facility

        TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, FacilityCharge fc, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter);

        TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, FacilityLot fl, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter);

        #endregion

        #region Productions

        TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, ProdOrderPartslistPos pos, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter);

        #endregion

        void SearchLot(gip.core.autocomponent.ACBSO bso, IACObjectEntity fb, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter, bool searchIntermediately = false);

        string TandTBSOName { get; }
    }
}
