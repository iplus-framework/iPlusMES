// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Linq;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Contains the FacilityPreBooking data together with extra data 
    /// to backtrace open FacilityPreBookings in the BSOProdOrder.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'POOpenPostingsWrapper'}de{'POOpenPostingsWrapper'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable)]
    public class OpenPostingsWrapper
    {
        public const string ClassName = "OpenPostingsWrapper";
        public OpenPostingsWrapper() { }

        public FacilityPreBooking CurrentFacilityPreBooking { get; set; }

        #region Columns

        [ACPropertyInfo(1, "Entity", "en{'Posting No.'}de{'Buchungsnummer'}")]
        public string PreBookingNo { get; set; }

        [ACPropertyInfo(2, "Entity", "en{'Posting date'}de{'Buchungsdatum'}")]
        public DateTime PreBookingDate { get; set; }

        [ACPropertyInfo(3, "Entity", "en{'Material'}de{'Material'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(4, "Entity", "en{'Sequence BOM'}de{'Reihenfolge Stückliste'}")]
        public int SequenceBOM { get; set; }

        [ACPropertyInfo(5, "Entity", "en{'Interm. prod.'}de{'ZW.-Prod.'}")]
        public string IntermediateProd { get; set; }

        [ACPropertyInfo(6, "Entity", "en{'Sequence batch'}de{'Reihenfolge Batch'}")]
        public int SequenceBatchNo { get; set; }

        public PostingTypeEnum PostingEnum { get; set; }

        [ACPropertyInfo(7, "Entity", "en{'Posting type'}de{'Buchungstyp'}")]
        public string PostingType
        {
            get
            {
                return PostingTypeList.Where(c => (PostingTypeEnum)c.Value == PostingEnum).FirstOrDefault().ACCaption;
            }
        }

        #endregion

        public enum PostingTypeEnum : short
        {
            Outward = 0,
            Inward = 1,
        }

        static ACValueItemList _PostingTypeList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList PostingTypeList
        {
            get
            {
                if (_PostingTypeList == null)
                {
                    _PostingTypeList = new ACValueItemList("MaterialProcessStateIndex");
                    _PostingTypeList.AddEntry((short)PostingTypeEnum.Outward, "en{'Input'}de{'Einsatz'}");
                    _PostingTypeList.AddEntry((short)PostingTypeEnum.Inward, "en{'Output'}de{'Ergebnis'}");
                }
                return _PostingTypeList;
            }
        }

    }
}
