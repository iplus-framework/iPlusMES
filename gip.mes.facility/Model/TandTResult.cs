// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    public class TandTResult
    {

        #region Constatns
        public static string ToHtmlStringTemplate = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN""
        ""http://www.w3.org/TR/html4/loose.dtd"">
            <html lang=""en"">
            <head>
	            <meta http-equiv=""content-type"" content=""text/html; charset=utf-8"">
	            <title>Tracking and tracking result</title>
            </head>
            <body>
            <h1>T&T export - filtered by {0}</h1>
            <ul>{1}</ul>
            </body>
            </html>
            ";
        #endregion

        #region Properties
        private List<FacilityCharge> _FacilityChargeList;

        public List<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (_FacilityChargeList == null)
                    _FacilityChargeList = new List<FacilityCharge>();
                return _FacilityChargeList;
            }
        }


        private List<string> lots;
        public List<string> Lots
        {
            get
            {
                if (lots == null)
                    lots = new List<string>();
                return lots;
            }
        }

        private List<string> fbcNos;
        public List<string> FBCNos
        {
            get
            {
                if (fbcNos == null)
                    fbcNos = new List<string>();
                return fbcNos;
            }
        }

        private List<TandTPoint> results;
        public List<TandTPoint> Results
        {
            get
            {
                if (results == null)
                    results = new List<TandTPoint>();
                return results;
            }
        }

        private List<TandTPointConnection> connectionPoints;
        public List<TandTPointConnection> ConnectionPoints
        {
            get
            {
                if (connectionPoints == null)
                    connectionPoints = new List<TandTPointConnection>();
                return connectionPoints;
            }
        }

        #endregion

        #region Methods

        public bool Add(TandTPoint item)
        {
            if (!Results.Where(x => x.ID == item.ID).Any())
            {
                Results.Add(item);
                return true;
            }
            return false;
        }

        public void AddFacilityCharge(FacilityCharge charge)
        {
            if (!FacilityChargeList.Contains(charge))
                FacilityChargeList.Add(charge);
        }

        public void AddFacilityCharge(IEnumerable<FacilityCharge> charges)
        {
            foreach (var item in charges)
                AddFacilityCharge(item);
        }

        public string ToHTMLString()
        {
            var rootPoint = Results.Where(x => x.Parent == null).FirstOrDefault();
            return string.Format(ToHtmlStringTemplate, "", rootPoint.ToHTMLString());
        }

        #endregion
    }
}
