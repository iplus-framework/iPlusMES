// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOFacilityEndClosing.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    /// <summary>
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Monats-/Tagesabschluß und Bestandsabgleich
    /// Neue Masken:
    /// 1. Monats-/Tagesabschluß und Bestandsabgleich
    /// ALLE Lagerbuchungen erfolgen immer nur über den StoreBookingManager.
    /// Dieser ist auch in anderen buchenden Anwendungen zu verwenden.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Closings and Inventory Sync.'}de{'Abschlüsse und Bestandsabgl.'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOFacilityEndClosing : ACBSOvb
    {

        #region const
        public const string BGWorkerMehtod_Matching = @"Matching";
        public const string BGWorkerMehtod_DayClosing = @"DayClosing";
        public const string BGWorkerMehtod_WeekClosing = @"WeekClosing";
        public const string BGWorkerMehtod_MonthClosing = @"MonthClosing";
        public const string BGWorkerMehtod_YearClosing = @"YearClosing";

        #endregion

        #region c´tors

        /// <summary>
        /// Konstruktor für ACComponent
        /// (Gleiche Signatur, wie beim ACGenericObject)
        /// </summary>
        /// <param name="acType">ACType anhand dessen die Methoden, Properties und Designs initialisiert werden</param>
        /// <param name="content">Inhalt
        /// Bei Model- oder BSO immer gleich ACClass
        /// Bei WF immer WorkOrderWF</param>
        /// <param name="parentACObject">Lebende ACComponent-Instanz</param>
        /// <param name="parameter">Parameter je nach Ableitungsimplementierung</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityEndClosing(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;
            this._BookingParameter = null;
            return await base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Managers
        /// <summary>
        /// The _ facility manager
        /// </summary>
        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        /// <summary>
        /// The _ booking parameter
        /// </summary>
        ACMethodBooking _BookingParameter;
        /// <summary>
        /// The _ progress info
        /// </summary>
        #endregion

        #region BSO->ACProperty
        public override DatabaseApp DatabaseApp
        {
            get
            {
                return base.DatabaseApp;
            }
        }

        /// <summary>
        /// Gets the current booking parameter.
        /// </summary>
        /// <value>The current booking parameter.</value>
        [ACPropertyCurrent(501, "BookingParameter")]
        public ACMethodBooking CurrentBookingParameter
        {
            get
            {
                return _BookingParameter;
            }
        }
        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Starts the matching.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Cell / Material Adjustment'}de{'Zellen- / Artikelabgleich'}", 501, true, Global.ACKinds.MSMethodPrePost)]
        public void StartMatching()
        {
            if (!IsEnabledStartMatching()) return;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_Matching);
            ShowDialog(this, DesignNameProgressBar);
        }

        /// <summary>
        /// Determines whether [is enabled start matching].
        /// </summary>
        /// <returns><c>true</c> if [is enabled start matching]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledStartMatching()
        {
            return !BackgroundWorker.IsBusy;
        }


        /// <summary>
        /// Starts the day closing.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Daily Closing'}de{'Tagesabschluss'}", 502, true, Global.ACKinds.MSMethodPrePost)]
        public void StartDayClosing()
        {
            if (BackgroundWorker.IsBusy)
                return;
            // Starte Hintergrundthread
            if (!PreExecute("StartDayClosing")) return;
            BackgroundWorker.RunWorkerAsync("DayClosing");
            ShowDialog(this, DesignNameProgressBar);
            PostExecute("StartDayClosing");
        }

        /// <summary>
        /// Determines whether [is enabled start day closing].
        /// </summary>
        /// <returns><c>true</c> if [is enabled start day closing]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledStartDayClosing()
        {
            return !BackgroundWorker.IsBusy;
        }

        /// <summary>
        /// Starts the week closing.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Weekly Closing'}de{'Wochenabschluss'}", 503, true, Global.ACKinds.MSMethodPrePost)]
        public void StartWeekClosing()
        {
            if (BackgroundWorker.IsBusy)
                return;
            // Starte Hintergrundthread
            if (!PreExecute("StartWeekClosing")) return;
            BackgroundWorker.RunWorkerAsync("WeekClosing");
            ShowDialog(this, DesignNameProgressBar);
            PostExecute("StartWeekClosing");
        }

        /// <summary>
        /// Determines whether [is enabled start week closing].
        /// </summary>
        /// <returns><c>true</c> if [is enabled start week closing]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledStartWeekClosing()
        {
            return !BackgroundWorker.IsBusy;
        }

        /// <summary>
        /// Starts the month closing.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Monthly Closing'}de{'Monatsabschluss'}", 504, false, Global.ACKinds.MSMethodPrePost)]
        public void StartMonthClosing()
        {
            if (BackgroundWorker.IsBusy)
                return;
            // Starte Hintergrundthread
            if (!PreExecute("StartMonthClosing")) return;
            BackgroundWorker.RunWorkerAsync("MonthClosing");
            ShowDialog(this, DesignNameProgressBar);
            PostExecute("StartMonthClosing");
        }

        /// <summary>
        /// Determines whether [is enabled start month closing].
        /// </summary>
        /// <returns><c>true</c> if [is enabled start month closing]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledStartMonthClosing()
        {
            return !BackgroundWorker.IsBusy;
        }

        /// <summary>
        /// Starts the year closing.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Year-End Closing'}de{'Jahresabschluss'}", 505, true, Global.ACKinds.MSMethodPrePost)]
        public void StartYearClosing()
        {
            if (BackgroundWorker.IsBusy)
                return;
            // Starte Hintergrundthread
            if (!PreExecute("StartYearClosing")) return;
            BackgroundWorker.RunWorkerAsync("YearClosing");
            ShowDialog(this, DesignNameProgressBar);
            PostExecute("StartYearClosing");
        }

        /// <summary>
        /// Determines whether [is enabled start year closing].
        /// </summary>
        /// <returns><c>true</c> if [is enabled start year closing]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledStartYearClosing()
        {
            return !BackgroundWorker.IsBusy;
        }


        /// <summary>
        /// Cancels this instance.
        /// </summary>
        [ACMethodCommand("BookingParameter", "en{'Cancel Closing / Adjustment'}de{'Abbruch Abschluss / Abgleich'}", 506, true, Global.ACKinds.MSMethodPrePost)]
        public void Cancel()
        {
            base.CancelBackgroundWorker();
        }

        /// <summary>
        /// Determines whether [is enabled cancel].
        /// </summary>
        /// <returns><c>true</c> if [is enabled cancel]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledCancel()
        {
            return BackgroundWorker.IsBusy;
        }
        #endregion

        #region Background-Worker
        /// <summary>
        /// Eventhandler für Events, die vom Backgroundworker ausgelöst werden
        /// Der Bachgroundworker generiert einen Hintergrundthread
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            // Hole Backgroundworker, der das Event ausgelöst hat
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();

            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            // Führe Buchungsfunktion durch durch durch
            // Info: e.Argument beeinhaltet Übergabeparameter
            
            switch (command)
            {
                case BGWorkerMehtod_Matching:
                    _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!MatchingStockAll", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                    break;
                case BGWorkerMehtod_DayClosing:
                    _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!ClosingDay", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                    break;
                case BGWorkerMehtod_WeekClosing:
                    _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!ClosingWeek", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                    break;
                case BGWorkerMehtod_MonthClosing:
                    _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!ClosingMonth", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                    break;
                case BGWorkerMehtod_YearClosing:
                    _BookingParameter = ACFacilityManager.ACUrlACTypeSignature("!ClosingYear", gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
                    break;
                default:
                    return;
            }

            if (_BookingParameter != null)
            {
                _BookingParameter.VBProgress = worker.ProgressInfo;
                e.Result = ACFacilityManager.BookFacility(_BookingParameter, this.DatabaseApp);
            }
        }

        /// <summary>
        /// 2. Dieser Eventhandler wird aufgerufen, wenn Hintergrundjob erledigt ist
        /// Methode läuft im Benutzerthread
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                
            }
            else if (e.Cancelled)
            {
            }
            else
            {
            }

            CloseTopDialog();

            if (_BookingParameter != null)
            {
                if (!_BookingParameter.ValidMessage.IsSucceded() || _BookingParameter.ValidMessage.HasWarnings())
                    Messages.MsgAsync(_BookingParameter.ValidMessage);
                else
                    _BookingParameter = null;
            }
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "StartMatching":
                    StartMatching();
                    return true;
                case "IsEnabledStartMatching":
                    result = IsEnabledStartMatching();
                    return true;
                case "StartDayClosing":
                    StartDayClosing();
                    return true;
                case "IsEnabledStartDayClosing":
                    result = IsEnabledStartDayClosing();
                    return true;
                case "StartWeekClosing":
                    StartWeekClosing();
                    return true;
                case "IsEnabledStartWeekClosing":
                    result = IsEnabledStartWeekClosing();
                    return true;
                case "StartMonthClosing":
                    StartMonthClosing();
                    return true;
                case "IsEnabledStartMonthClosing":
                    result = IsEnabledStartMonthClosing();
                    return true;
                case "StartYearClosing":
                    StartYearClosing();
                    return true;
                case "IsEnabledStartYearClosing":
                    result = IsEnabledStartYearClosing();
                    return true;
                case "Cancel":
                    Cancel();
                    return true;
                case "IsEnabledCancel":
                    result = IsEnabledCancel();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
