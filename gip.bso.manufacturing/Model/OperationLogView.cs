// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;

namespace gip.bso.manufacturing
{
    public class OperationLogView
    {

        public Guid OperationLogID { get; set; }
        public string UserName { get; set; }

        public List<OperationLogParam> Params { get; set; } = new List<OperationLogParam>();


        #region Material and order data
        public string MaterialNo { get; set; }
        public string MaterialName { get; set; }
        public string ProgramNo { get; set; }
        public string FacilityLotNo { get; set; }
        public int SplitNo { get; set; }

        #endregion

        #region Machine Information
        public string ACCaptionInstance { get; set; }
        public string ACUrlInstance { get; set; }

        public string ACCaptionTypeModel { get; set; }
        public string ACUrlTypeModel { get; set; }
        public string MethodeName { get; set; }

        #endregion

        #region OperationLog Times
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now;
        #endregion

        #region Method and Parameter information
        public string ParameterName { get; set; }


        public string ValueStr { get; set; }
        public double? ValueDouble { get; set; }
        #endregion
    }
}
