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
    /// Class BSOTourManagement
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Tourmanagement'}de{'Tourmanagement'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Tourplan.ClassName)]
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
        /// Gets or sets the current from date.
        /// </summary>
        /// <value>The current from date.</value>
        [ACPropertyCurrent(500, "Date", "en{'From date'}de{'Von Datum'}")]
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
        /// Gets or sets the current to date.
        /// </summary>
        /// <value>The current to date.</value>
        [ACPropertyCurrent(501, "Date", "en{'To date'}de{'Von Datum'}")]
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
        /// Moves the prev week.
        /// </summary>
        [ACMethodCommand("Date", "en{'-'}de{'-'}", 500)]
        public void MovePrevWeek()
        {
        }

        /// <summary>
        /// Moves the next week.
        /// </summary>
        [ACMethodCommand("Date", "en{'+'}de{'+'}", 501)]
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

        #endregion


    }
}
