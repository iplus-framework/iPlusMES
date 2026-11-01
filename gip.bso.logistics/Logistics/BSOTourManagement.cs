// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOTourManagement.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.mes.datamodel; using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.logistics
{
    /// <summary>
    /// Businessobject or App for the tour management overview (tour planning board).
    /// This BSO is a lightweight planning overview for tour plans (Tourplan) in a date range:
    /// The CurrentFromDate and CurrentToDate properties define the displayed date range (week).
    /// MovePrevWeek() moves the date range one week back and MoveNextWeek() one week forward.
    /// The tour plans of the date range are displayed in the connected designs (tour planning board).
    /// The detailed editing of tour plans and their tour positions is done in the tour plan dialog (opened from the visitor voucher or the tour plan app).
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOTourManagement&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Tourmanagement'}de{'Tourmanagement'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Tourplan.ClassName,
        Description = @"Businessobject or App for the tour management overview (tour planning board).
        This BSO is a lightweight planning overview for tour plans (Tourplan) in a date range:
        The CurrentFromDate and CurrentToDate properties define the displayed date range (week).
        MovePrevWeek() moves the date range one week back and MoveNextWeek() one week forward.
        The tour plans of the date range are displayed in the connected designs (tour planning board).
        The detailed editing of tour plans and their tour positions is done in the tour plan dialog (opened from the visitor voucher or the tour plan app).
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOTourManagement&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOTourManagement : ACBSOvb 
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOTourManagement"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOTourManagement(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
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
            //Search();
            return true;
        }
        #endregion

        #region BSO->ACProperty
        /// <summary>
        /// The _ current from date
        /// </summary>
        DateTime _CurrentFromDate;
        /// <summary>
        /// Gets or sets the start of the displayed date range.
        /// </summary>
        /// <value>The current from date.</value>
        [ACPropertyCurrent(500, "Date", "en{'From date'}de{'Von Datum'}", Description = @"Start of the displayed date range (week) of the tour planning board.")]
        public DateTime CurrentFromDate
        {
            get
            {
                return _CurrentFromDate;
            }
            set
            {
                _CurrentFromDate = value;
                OnPropertyChanged("CurrentFromDate");
            }
        }

        /// <summary>
        /// The _ current to date
        /// </summary>
        DateTime _CurrentToDate;
        /// <summary>
        /// Gets or sets the end of the displayed date range.
        /// </summary>
        /// <value>The current to date.</value>
        [ACPropertyCurrent(501, "Date", "en{'To date'}de{'Bis Datum'}", Description = @"End of the displayed date range (week) of the tour planning board.")]
        public DateTime CurrentToDate
        {
            get
            {
                return _CurrentToDate;
            }
            set
            {
                _CurrentToDate = value;
                OnPropertyChanged("CurrentToDate");
            }
        }
        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Moves the displayed date range one week back.
        /// </summary>
        [ACMethodCommand("Date", "en{'-'}de{'-'}", 500, Description = @"Moves the displayed date range (CurrentFromDate/CurrentToDate) of the tour planning board one week back. Currently a placeholder without implementation.")]
        public void MovePrevWeek()
        {
        }

        /// <summary>
        /// Moves the displayed date range one week forward.
        /// </summary>
        [ACMethodCommand("Date", "en{'+'}de{'+'}", 501, Description = @"Moves the displayed date range (CurrentFromDate/CurrentToDate) of the tour planning board one week forward. Currently a placeholder without implementation.")]
        public void MoveNextWeek()
        {
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"MovePrevWeek":
                    MovePrevWeek();
                    return true;
                case"MoveNextWeek":
                    MoveNextWeek();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(MovePrevWeek):
                case nameof(MoveNextWeek):
                    return new string[] { nameof(CurrentFromDate), nameof(CurrentToDate) };
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion


    }
}
