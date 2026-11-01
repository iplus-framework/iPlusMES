using gip.core.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.purchasing
{
    public class BSOInDeliveryNoteTrackingMenuBuilder
    {
        /// <summary>
        /// Helper class (not a BSO) that builds the Tracking and Tracing context menu for the BSOInDeliveryNote app.
        /// It is instantiated from the BSOInDeliveryNote design when a control bound to one of the InDeliveryNote_TrackingPropertiesEnum
        /// properties (e.g. SelectedFacilityBooking) is right-clicked. If the bound VBContent matches a tracking property and a
        /// corresponding entity is selected (currently only SelectedFacilityBooking), the TrackingCommonStart service is used to
        /// collect the tracking and tracing menu items and add them to the passed ACMenuItemList. Other tracking properties
        /// (SelectedDeliveryNotePos, SelectedInOrderPos, SelectedInOrderPosFromPicking) are currently commented out.
        /// </summary>
        /// <param name="bSOInDeliveryNote"></param>
        /// <param name="aCMenuItems"></param>
        /// <param name="vbContent"></param>
        /// <param name="vbControl"></param>
        public BSOInDeliveryNoteTrackingMenuBuilder(BSOInDeliveryNote bSOInDeliveryNote, ACMenuItemList aCMenuItems, string vbContent, string vbControl)
        {
            Dictionary<InDeliveryNote_TrackingPropertiesEnum, string> trackingVBContents = ((InDeliveryNote_TrackingPropertiesEnum[])Enum.GetValues(typeof(InDeliveryNote_TrackingPropertiesEnum))).ToDictionary(key => key, val => val.ToString());
            if (!string.IsNullOrEmpty(vbContent) && trackingVBContents.Values.Contains(vbContent))
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                InDeliveryNote_TrackingPropertiesEnum trackingProperty = (InDeliveryNote_TrackingPropertiesEnum)Enum.Parse(typeof(InDeliveryNote_TrackingPropertiesEnum), vbContent);
                ACMenuItemList trackingAndTracingMenuItems = null;
                switch (trackingProperty)
                {
                    case InDeliveryNote_TrackingPropertiesEnum.SelectedFacilityBooking:
                        if (bSOInDeliveryNote.SelectedFacilityBooking != null)
                        {
                            trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(bSOInDeliveryNote, bSOInDeliveryNote.SelectedFacilityBooking);
                        }
                        break;
                        //case InDeliveryNote_TrackingPropertiesEnum.SelectedDeliveryNotePos:
                        //    if (bSOInDeliveryNote.SelectedDeliveryNotePos != null)
                        //    {
                        //        trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(bSOInDeliveryNote, bSOInDeliveryNote.SelectedDeliveryNotePos);
                        //    }
                        //    break;
                        //case InDeliveryNote_TrackingPropertiesEnum.SelectedInOrderPos:
                        //    if (bSOInDeliveryNote.SelectedInOrderPos != null)
                        //    {
                        //        trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(bSOInDeliveryNote, bSOInDeliveryNote.SelectedInOrderPos.TopParentInOrderPos);
                        //    }
                        //    break;
                        //case InDeliveryNote_TrackingPropertiesEnum.SelectedInOrderPosFromPicking:
                        //    if (bSOInDeliveryNote.SelectedInOrderPosFromPicking != null)
                        //    {
                        //        trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(bSOInDeliveryNote, bSOInDeliveryNote.SelectedInOrderPosFromPicking.TopParentInOrderPos);
                        //    }
                        //    break;
                }
                if (trackingAndTracingMenuItems != null)
                    aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }
        }
    }
}
