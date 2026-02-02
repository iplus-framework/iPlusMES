// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.communication;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static gip.core.communication.ISOonTCP.PLC;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sample light box RaspPi'}de{'Stichproben Ampelbox RaspPi'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAESamplePiLightBox : PAModule
    {
        #region c'tors

        static PAESamplePiLightBox()
        {
            RegisterExecuteHandler(typeof(PAESamplePiLightBox), HandleExecuteACMethod_PAESamplePiLightBox);
        }

        public PAESamplePiLightBox(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            InitializeExtPropInLabOrderPos(this);
            return true;
        }

        public override bool ACPostInit()
        {
            (SetPoint as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            SetPoint.ForceBroadcast = true;
            (TolPlus as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolPlus.ForceBroadcast = true;
            (TolMinus as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolMinus.ForceBroadcast = true;
            (ActualValue as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();

            (SetPoint2 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            SetPoint2.ForceBroadcast = true;
            (TolPlus2 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolPlus2.ForceBroadcast = true;
            (TolMinus2 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolMinus2.ForceBroadcast = true;
            (ActualValue2 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();

            (SetPoint3 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            SetPoint3.ForceBroadcast = true;
            (TolPlus3 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolPlus3.ForceBroadcast = true;
            (TolMinus3 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolMinus3.ForceBroadcast = true;
            (ActualValue3 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();

            (SetPoint4 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            SetPoint4.ForceBroadcast = true;
            (TolPlus4 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolPlus4.ForceBroadcast = true;
            (TolMinus4 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolMinus4.ForceBroadcast = true;
            (ActualValue4 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();

            (SetPoint5 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            SetPoint5.ForceBroadcast = true;
            (TolPlus5 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolPlus5.ForceBroadcast = true;
            (TolMinus5 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            TolMinus5.ForceBroadcast = true;
            (ActualValue5 as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();

            if (ApplicationManager != null)
                ApplicationManager.ProjectWorkCycleR5min += ApplicationManager_ProjectWorkCycleR5min;
            return base.ACPostInit();
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            if (ApplicationManager != null)
                ApplicationManager.ProjectWorkCycleR5min -= ApplicationManager_ProjectWorkCycleR5min;
            return await base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        #region HTTP-Const
        private const string C_ActivateOrderString = "ActivateOrder";
        private const string C_StopOrderString = "StopOrder";
        private const string C_GetToleranceWeighDataString = "GetValues";
        private const string C_GetLogFileString = "GetLogFile";
        private const string C_WeightSetpointString = "SetP";
        private const string C_ToleranceAboveString = "TolA";
        private const string C_ToleranceBelowString = "TolB";
        private const string C_UpdateDateTime = "UpdateTime";
        private const string C_CurrrentTime = "CurrentTime";
        private const string C_ResetToleranceCycle = "ResetToleranceCycle";

        private ACRef<ACRestClient> _Client;
        public ACRestClient Client
        {
            get
            {
                if (_Client != null)
                    return _Client.ValueT;
                var client = FindChildComponents<ACRestClient>(c => c is ACRestClient).FirstOrDefault();
                if (client != null)
                {
                    _Client = new ACRef<ACRestClient>(client, this);
                    return _Client.ValueT;
                }
                return null;
            }
        }
        #endregion

        [ACPropertyBindingSource(200, "Values", "en{'Racording is active'}de{'Aufnahme aktiv'}", "", false, true)]
        public IACContainerTNet<int> IsRecording { get; set; }

        [ACPropertyBindingSource(200, "Values", "en{'Setpoint sequence'}de{'Sollwertfolge'}", "", false, true)]
        public IACContainerTNet<int> SetPointSequence { get; set; }

        [ACPropertyBindingSource(201, "Values", "en{'Setpoint'}de{'Sollwert'}", "", false, true)]
        public IACContainerTNet<double> SetPoint { get; set; }

        [ACPropertyBindingSource(201, "Values", "en{'Setpoint 2'}de{'Sollwert 2'}", "", false, true)]
        public IACContainerTNet<double> SetPoint2 { get; set; }

        [ACPropertyBindingSource(201, "Values", "en{'Setpoint 3'}de{'Sollwert 3'}", "", false, true)]
        public IACContainerTNet<double> SetPoint3 { get; set; }

        [ACPropertyBindingSource(201, "Values", "en{'Setpoint 4'}de{'Sollwert 4'}", "", false, true)]
        public IACContainerTNet<double> SetPoint4 { get; set; }

        [ACPropertyBindingSource(201, "Values", "en{'Setpoint 5'}de{'Sollwert 5'}", "", false, true)]
        public IACContainerTNet<double> SetPoint5 { get; set; }

        [ACPropertyBindingSource(202, "Values", "en{'Tolerance +'}de{'Toleranz +'}", "", false, true)]
        public IACContainerTNet<double> TolPlus { get; set; }

        [ACPropertyBindingSource(202, "Values", "en{'Tolerance 2 +'}de{'Toleranz 2 +'}", "", false, true)]
        public IACContainerTNet<double> TolPlus2 { get; set; }

        [ACPropertyBindingSource(202, "Values", "en{'Tolerance 3 +'}de{'Toleranz 3 +'}", "", false, true)]
        public IACContainerTNet<double> TolPlus3 { get; set; }

        [ACPropertyBindingSource(202, "Values", "en{'Tolerance 4 +'}de{'Toleranz 4 +'}", "", false, true)]
        public IACContainerTNet<double> TolPlus4 { get; set; }

        [ACPropertyBindingSource(202, "Values", "en{'Tolerance 5 +'}de{'Toleranz 5 +'}", "", false, true)]
        public IACContainerTNet<double> TolPlus5 { get; set; }


        [ACPropertyBindingSource(203, "Values", "en{'Tolerance -'}de{'Toleranz -'}", "", false, true)]
        public IACContainerTNet<double> TolMinus { get; set; }

        [ACPropertyBindingSource(203, "Values", "en{'Tolerance 2 -'}de{'Toleranz 2 -'}", "", false, true)]
        public IACContainerTNet<double> TolMinus2 { get; set; }

        [ACPropertyBindingSource(203, "Values", "en{'Tolerance 3 -'}de{'Toleranz 3 -'}", "", false, true)]
        public IACContainerTNet<double> TolMinus3 { get; set; }

        [ACPropertyBindingSource(203, "Values", "en{'Tolerance 4 -'}de{'Toleranz 4 -'}", "", false, true)]
        public IACContainerTNet<double> TolMinus4 { get; set; }

        [ACPropertyBindingSource(203, "Values", "en{'Tolerance 5 -'}de{'Toleranz 5 -'}", "", false, true)]
        public IACContainerTNet<double> TolMinus5 { get; set; }

        private string _LastResultValues;
        [ACPropertyInfo(true, 204, "", "en{'Last results'}de{'Letztes Ergebnis'}", "", false)]
        public string LastResultValues
        {
            get
            {
                return _LastResultValues;
            }
            set
            {
                _LastResultValues = value;
                OnPropertyChanged("LastResultValues");
            }
        }


        private string _LastLogs;
        [ACPropertyInfo(true, 205, "", "en{'Last logs'}de{'Letztes Logs'}", "", false)]
        public string LastLogs
        {
            get
            {
                return _LastLogs;
            }
            set
            {
                _LastLogs = value;
                OnPropertyChanged("LastLogs");
            }
        }

        [ACPropertyBindingSource(206, "Values", "en{'Actual Value'}de{'Aktueller Wert'}", "", false, true)]
        public IACContainerTNet<double> ActualValue { get; set; }

        [ACPropertyBindingSource(206, "Values", "en{'Actual Value 2'}de{'Aktueller Wert 2'}", "", false, true)]
        public IACContainerTNet<double> ActualValue2 { get; set; }

        [ACPropertyBindingSource(206, "Values", "en{'Actual Value 3'}de{'Aktueller Wert 3'}", "", false, true)]
        public IACContainerTNet<double> ActualValue3 { get; set; }

        [ACPropertyBindingSource(206, "Values", "en{'Actual Value 4'}de{'Aktueller Wert 4'}", "", false, true)]
        public IACContainerTNet<double> ActualValue4 { get; set; }

        [ACPropertyBindingSource(206, "Values", "en{'Actual Value 5'}de{'Aktueller Wert 5'}", "", false, true)]
        public IACContainerTNet<double> ActualValue5 { get; set; }

        [ACPropertyBindingSource(207, "Values", "en{'Average Value'}de{'Mittelwert'}", "", false, true)]
        public IACContainerTNet<double> AverageValue { get; set; }

        [ACPropertyBindingSource(207, "Values", "en{'Average Value 2'}de{'Mittelwert 2'}", "", false, true)]
        public IACContainerTNet<double> AverageValue2 { get; set; }

        [ACPropertyBindingSource(207, "Values", "en{'Average Value 3'}de{'Mittelwert 3'}", "", false, true)]
        public IACContainerTNet<double> AverageValue3 { get; set; }

        [ACPropertyBindingSource(207, "Values", "en{'Average Value 4'}de{'Mittelwert 4'}", "", false, true)]
        public IACContainerTNet<double> AverageValue4 { get; set; }

        [ACPropertyBindingSource(207, "Values", "en{'Average Value 5'}de{'Mittelwert 5'}", "", false, true)]
        public IACContainerTNet<double> AverageValue5 { get; set; }

        [ACPropertyBindingSource(208, "Values", "en{'Average-State'}de{'Mittelwert-Status'}", "", false, true)]
        public IACContainerTNet<short> AverageState { get; set; }

        [ACPropertyBindingSource(208, "Values", "en{'Average-State 2'}de{'Mittelwert-Status 2'}", "", false, true)]
        public IACContainerTNet<short> AverageState2 { get; set; }

        [ACPropertyBindingSource(208, "Values", "en{'Average-State 3'}de{'Mittelwert-Status 3'}", "", false, true)]
        public IACContainerTNet<short> AverageState3 { get; set; }

        [ACPropertyBindingSource(208, "Values", "en{'Average-State 4'}de{'Mittelwert-Status 4'}", "", false, true)]
        public IACContainerTNet<short> AverageState4 { get; set; }

        [ACPropertyBindingSource(208, "Values", "en{'Average-State 5'}de{'Mittelwert-Status 5'}", "", false, true)]
        public IACContainerTNet<short> AverageState5 { get; set; }

        [ACPropertyBindingSource(208, "Values", "en{'Update time'}de{'Updatezeit'}", "", true, true)]
        public IACContainerTNet<bool> UpdateTime { get; set; }

        private string _PWSampleNode;
        [ACPropertyInfo(true, 201, "", "en{'Actual Workflownode'}de{'Aktueller Workflowknoten'}", "", false)]
        public string PWSampleNode
        {
            get
            {
                return _PWSampleNode;
            }
            set
            {
                _PWSampleNode = value;
                OnPropertyChanged("PWSampleNode");
            }
        }

        #endregion

        #region Methods

        private void ApplicationManager_ProjectWorkCycleR5min(object sender, EventArgs e)
        {
            if (IsRecording.ValueT > 0)
            {
                this.ApplicationManager.ApplicationQueue.Add(() => { GetValues(); });
            }
        }

        [ACMethodInfo("", "en{'Send paramas an start order'}de{'Sende Parameter und starte Auftrag'}", 200)]
        public bool SetParamsAndStartOrder(double setPoint, double tolPlus, double tolMinus, string pwSampleNode, int sequence)
        {
            if (!IsEnabledStartOrder())
                return false;
            if (setPoint <= 0.000001 || tolPlus <= 0.000001 || tolMinus <= 0.000001)
                return false;
            if (sequence < IsRecording.ValueT )
            {
                GetValues();
                StopOrder();
            }

            SetPointSequence.ValueT = sequence;

            if (sequence == 1)
            {
                SetPoint.ValueT = setPoint;
                TolPlus.ValueT = tolPlus;
                TolMinus.ValueT = tolMinus;
            }
            else if (sequence == 2)
            {
                SetPoint2.ValueT = setPoint;
                TolPlus2.ValueT = tolPlus;
                TolMinus2.ValueT = tolMinus;
            }
            else if (sequence == 3)
            {
                SetPoint3.ValueT = setPoint;
                TolPlus3.ValueT = tolPlus;
                TolMinus3.ValueT = tolMinus;
            }
            else if (sequence == 4)
            {
                SetPoint4.ValueT = setPoint;
                TolPlus4.ValueT = tolPlus;
                TolMinus4.ValueT = tolMinus;
            }
            else if (sequence == 5)
            {
                SetPoint5.ValueT = setPoint;
                TolPlus5.ValueT = tolPlus;
                TolMinus5.ValueT = tolMinus;
            }

            bool started = StartOrder();
            if (started)
                PWSampleNode = pwSampleNode;
            return started;
        }

        [ACMethodCommand("", "en{'Start order'}de{'Auftrag starten'}", 201, true)]
        public bool StartOrder()
        {
            if (!IsEnabledStartOrder())
                return false;
            if ((SetPointSequence.ValueT == 1 && IsRecording.ValueT > 0))
            {
                GetValues();
                StopOrder();
            }

            if (UpdateTime.ValueT)
            {
                UriBuilder uriBuilderDT = new UriBuilder(C_UpdateDateTime + "?");
                NameValueCollection parametersDT = HttpUtility.ParseQueryString(uriBuilderDT.Query);
                parametersDT[C_CurrrentTime] = DateTime.Now.ToString();
                uriBuilderDT.Query = parametersDT.ToString();
                string relativeReqUrl = String.Format("{0}{1}", C_UpdateDateTime, uriBuilderDT.Query.ToString());

                WSResponse<string> responseDT = this.Client.Get(relativeReqUrl);
                if (!responseDT.Suceeded)
                {
                    Messages.LogError(this.GetACUrl(), nameof(StartOrder), "Update DateTime is not performed!");
                    return false;
                }
            }

            if (SetPointSequence.ValueT == 1)
            {
                AverageValue.ValueT = 0;
                AverageState.ValueT = 0;
            }
            else if (SetPointSequence.ValueT == 2)
            {
                AverageValue2.ValueT = 0;
                AverageState2.ValueT = 0;
            }
            else if (SetPointSequence.ValueT == 3)
            {
                AverageValue3.ValueT = 0;
                AverageState3.ValueT = 0;
            }
            else if (SetPointSequence.ValueT == 4)
            {
                AverageValue4.ValueT = 0;
                AverageState4.ValueT = 0;
            }
            else if (SetPointSequence.ValueT == 5)
            {
                AverageValue5.ValueT = 0;
                AverageState5.ValueT = 0;
            }

            UriBuilder uriBuilder = new UriBuilder(C_ActivateOrderString + "?");
            NameValueCollection parameters = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (SetPointSequence.ValueT == 1)
            {
                parameters[C_WeightSetpointString] = SetPoint.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceAboveString] = TolPlus.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceBelowString] = TolMinus.ValueT.ToString("0.######").Replace(",", ".");
            }
            else if (SetPointSequence.ValueT == 2)
            {
                parameters[C_WeightSetpointString] = SetPoint2.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceAboveString] = TolPlus2.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceBelowString] = TolMinus2.ValueT.ToString("0.######").Replace(",", ".");
            }
            else if (SetPointSequence.ValueT == 3)
            {
                parameters[C_WeightSetpointString] = SetPoint3.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceAboveString] = TolPlus3.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceBelowString] = TolMinus3.ValueT.ToString("0.######").Replace(",", ".");
            }
            else if (SetPointSequence.ValueT == 4)
            {
                parameters[C_WeightSetpointString] = SetPoint4.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceAboveString] = TolPlus4.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceBelowString] = TolMinus4.ValueT.ToString("0.######").Replace(",", ".");
            }
            else if (SetPointSequence.ValueT == 5)
            {
                parameters[C_WeightSetpointString] = SetPoint5.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceAboveString] = TolPlus5.ValueT.ToString("0.######").Replace(",", ".");
                parameters[C_ToleranceBelowString] = TolMinus5.ValueT.ToString("0.######").Replace(",", ".");
            }

            uriBuilder.Query = parameters.ToString();
            string relativeRequestUrl = String.Format("{0}{1}", C_ActivateOrderString, uriBuilder.Query.ToString());

            WSResponse<string> response = this.Client.Get(relativeRequestUrl);
            if (!response.Suceeded)
                return false;

            int recordingSequence = 1;
            if (SetPointSequence.ValueT > 0)
                recordingSequence = SetPointSequence.ValueT;

            IsRecording.ValueT = recordingSequence;
            return true;
        }

        public bool IsEnabledStartOrder()
        {
            if (!CanSend())
                return false;
            return true;
        }

        [ACMethodInteraction("", "en{'Stop order'}de{'Auftrag stoppen'}", 202, true)]
        public bool StopOrder()
        {
            if (!IsEnabledStopOrder())
                return false;
            WSResponse<string> response = this.Client.Get(C_StopOrderString);
            if (!response.Suceeded)
                return false;
            IsRecording.ValueT = 0;
            PWSampleNode = null;
            return true;
        }

        public bool IsEnabledStopOrder()
        {
            if (!CanSend())
                return false;
            return true;
        }

        [ACMethodInfo("", "en{'Read collected values'}de{'Lese protokollierte Werte'}", 203, true)]
        public SamplePiStatsCollection GetValues()
        {
            if (!IsEnabledGetValues())
                return null;
            WSResponse<string> response = this.Client.Get(C_GetToleranceWeighDataString);
            if (!response.Suceeded)
                return null;
            LastResultValues = response.Data;

            SamplePiStatsCollection result = new SamplePiStatsCollection();

            SamplePiStats resultStats = ParseValues(response.Data, SetPoint.ValueT, TolPlus.ValueT, TolMinus.ValueT);

            if (resultStats == null)
                return null;

            var resultsBySetPoint = resultStats.Values.GroupBy(c => c.SetPoint);

            foreach (var resultBySetPoint in resultsBySetPoint)
            {
                if (resultBySetPoint.Any())
                {
                    SamplePiStats resStats = new SamplePiStats();

                    if (resultBySetPoint.Key == SetPoint.ValueT || resultBySetPoint.Key <= double.Epsilon)
                    {
                        LogPropertyValues(resStats, resultBySetPoint.ToList(), ActualValue, SetPoint, TolPlus, TolMinus, AverageValue, AverageState);
                    }
                    else if (resultBySetPoint.Key == SetPoint2.ValueT)
                    {
                        LogPropertyValues(resStats, resultBySetPoint.ToList(), ActualValue2, SetPoint2, TolPlus2, TolMinus2, AverageValue2, AverageState2);
                    }
                    else if (resultBySetPoint.Key == SetPoint3.ValueT)
                    {
                        LogPropertyValues(resStats, resultBySetPoint.ToList(), ActualValue3, SetPoint3, TolPlus3, TolMinus3, AverageValue3, AverageState3);
                    }
                    else if (resultBySetPoint.Key == SetPoint4.ValueT)
                    {
                        LogPropertyValues(resStats, resultBySetPoint.ToList(), ActualValue4, SetPoint4, TolPlus4, TolMinus4, AverageValue4, AverageState4);
                    }
                    else if (resultBySetPoint.Key == SetPoint5.ValueT)
                    {
                        LogPropertyValues(resStats, resultBySetPoint.ToList(), ActualValue5, SetPoint5, TolPlus5, TolMinus5, AverageValue5, AverageState5);
                    }

                    result.Add(resStats);
                }
            }
            return result;
        }

        private void LogPropertyValues(SamplePiStats stats, List<SamplePiValue> tolValues, IACContainerTNet<double> actualValue, IACContainerTNet<double> setPoint, IACContainerTNet<double> tolPlus, IACContainerTNet<double> tolMinus,
                                       IACContainerTNet<double> averageValue, IACContainerTNet<short> averageState)
        {
            stats.SetPoint = setPoint.ValueT;
            stats.TolPlus = tolPlus.ValueT;
            stats.TolMinus = tolMinus.ValueT;
            stats.Values = tolValues;

            ACPropertyNetSource<double> actValueProp = actualValue as ACPropertyNetSource<double>;
            if (actValueProp.PropertyLog != null)
            {
                foreach (SamplePiValue value in tolValues)
                {
                    actValueProp.PropertyLog.AddValue(value.Value, value.DTStamp);
                }
                actValueProp.PropertyLog.SaveChanges();
            }
            var logProp = (setPoint as ACPropertyNetServerBase<double>);
            if (logProp != null && logProp.PropertyLog != null)
            {
                logProp.PropertyLog.AddValue(setPoint.ValueT, DateTime.Now);
                logProp.PropertyLog.SaveChanges();
            }

            logProp = (tolPlus as ACPropertyNetServerBase<double>);
            if (logProp != null && logProp.PropertyLog != null)
            {
                logProp.PropertyLog.AddValue(setPoint.ValueT + tolPlus.ValueT, DateTime.Now);
                logProp.PropertyLog.SaveChanges();
            }

            logProp = (tolMinus as ACPropertyNetServerBase<double>);
            if (logProp != null && logProp.PropertyLog != null)
            {
                logProp.PropertyLog.AddValue(setPoint.ValueT - tolMinus.ValueT, DateTime.Now);
                logProp.PropertyLog.SaveChanges();
            }

            averageValue.ValueT = stats.AverageValue;
            if (stats.CountInTol > stats.CountAbove + stats.CountBelow)
                averageState.ValueT = 0;
            else if (stats.CountAbove > stats.CountBelow)
                averageState.ValueT = 1;
            else if (stats.CountAbove <= stats.CountBelow)
                averageState.ValueT = -1;
        }

        public static SamplePiStats ParseValues(string values, double setPoint, double tolPlus, double tolMinus)
        {
            //0.240;Below;19/03/2022 07:07:54
            //0.505;Above;19/03/2022 07:08:33
            //0.240;Within;19/03/2022 07:22:54
            //0.240;Within;19/03/2022 07:22:55;0.250

            SamplePiStats result = new SamplePiStats(setPoint, tolPlus, tolMinus);
            if (String.IsNullOrEmpty(values))
                return result;

            CultureInfo cultureInfo = new CultureInfo("en-US");
            using (var reader = new StringReader(values))
            {
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    string[] valArr = line.Split(';');
                    if (valArr == null || valArr.Length < 3)
                        continue;

                    double setPointRes = 0;

                    if (valArr.Length == 4)
                    {
                        if (!Double.TryParse(valArr[3], NumberStyles.AllowDecimalPoint, cultureInfo, out setPointRes))
                            continue;
                    }

                    double weight = 0;
                    if (!Double.TryParse(valArr[0], NumberStyles.AllowDecimalPoint, cultureInfo, out weight))
                        continue;
                    DateTime stamp;
                    if (!DateTime.TryParseExact(valArr[2], "dd/MM/yyyy HH:mm:ss", cultureInfo, System.Globalization.DateTimeStyles.None, out stamp))
                        continue;
                    //stamp = stamp.ToLocalTime();
                    SamplePiValue piValue = new SamplePiValue() { DTStamp = stamp, Value = weight, SetPoint = setPointRes };
                    if (valArr[1] == "Above")
                        piValue.TolState = 1;
                    else if (valArr[1] == "Below")
                        piValue.TolState = -1;
                    else
                        piValue.TolState = 0;
                    result.Values.Add(piValue);
                }
            }
            
            return result;
        }

        [ACMethodInfo("", "en{'Read archived values'}de{'Lese archivierte Werte'}", 209, true)]
        public SamplePiStatsCollection GetArchivedValues(DateTime from, DateTime to, bool useCurrentSetPoints)
        {
            double setPoint = 0.0;
            double tolPlus = 0.0;
            double tolMinus = 0.0;

            double setPoint2 = 0.0;
            double tolPlus2 = 0.0;
            double tolMinus2 = 0.0;

            double setPoint3 = 0.0;
            double tolPlus3 = 0.0;
            double tolMinus3 = 0.0;

            double setPoint4 = 0.0;
            double tolPlus4 = 0.0;
            double tolMinus4 = 0.0;

            double setPoint5 = 0.0;
            double tolPlus5 = 0.0;
            double tolMinus5 = 0.0;

            if (useCurrentSetPoints)
            {
                setPoint = SetPoint.ValueT;
                tolPlus = TolPlus.ValueT;
                tolMinus = TolMinus.ValueT;

                setPoint2 = SetPoint2.ValueT;
                tolPlus2 = TolPlus2.ValueT;
                tolMinus2 = TolMinus2.ValueT;

                setPoint3 = SetPoint3.ValueT;
                tolPlus3 = TolPlus3.ValueT;
                tolMinus3 = TolMinus3.ValueT;

                setPoint4 = SetPoint4.ValueT;
                tolPlus4 = TolPlus4.ValueT;
                tolMinus4 = TolMinus4.ValueT;

                setPoint5 = SetPoint5.ValueT;
                tolPlus5 = TolPlus5.ValueT;
                tolMinus5 = TolMinus5.ValueT;
            }
            if (!useCurrentSetPoints || setPoint <= double.Epsilon || tolPlus <= double.Epsilon || tolMinus <= double.Epsilon)
            {
                bool result = GetValuesFromArchive(SetPoint, TolPlus, TolMinus, from, to, ref setPoint, ref tolPlus, ref tolMinus);
                if (!result)
                    return null;
            }

            if (!useCurrentSetPoints || setPoint2 <= double.Epsilon || tolPlus2 <= double.Epsilon || tolMinus2 <= double.Epsilon)
            {
                GetValuesFromArchive(SetPoint2, TolPlus2, TolMinus2, from, to, ref setPoint2, ref tolPlus2, ref tolMinus2);
            }

            if (!useCurrentSetPoints || setPoint3 <= double.Epsilon || tolPlus3 <= double.Epsilon || tolMinus3 <= double.Epsilon)
            {
                GetValuesFromArchive(SetPoint3, TolPlus3, TolMinus3, from, to, ref setPoint3, ref tolPlus3, ref tolMinus3);
            }

            if (!useCurrentSetPoints || setPoint4 <= double.Epsilon || tolPlus4 <= double.Epsilon || tolMinus4 <= double.Epsilon)
            {
                GetValuesFromArchive(SetPoint4, TolPlus4, TolMinus4, from, to, ref setPoint4, ref tolPlus4, ref tolMinus4);
            }

            if (!useCurrentSetPoints || setPoint5 <= double.Epsilon || tolPlus5 <= double.Epsilon || tolMinus5 <= double.Epsilon)
            {
                GetValuesFromArchive(SetPoint5, TolPlus5, TolMinus5, from, to, ref setPoint5, ref tolPlus5, ref tolMinus5);
            }

            SamplePiStatsCollection resultStats = new SamplePiStatsCollection();

            SamplePiStats stats = new SamplePiStats(setPoint, tolPlus, tolMinus);
            var actValueSeries =  (ActualValue as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
            if (actValueSeries != null && actValueSeries.PropertyLogList != null && actValueSeries.PropertyLogList.Any())
                stats.Values.AddRange(actValueSeries.PropertyLogList.Select(c => new SamplePiValue() { Value = (double)c.Value, DTStamp = c.Time }));
            resultStats.Add(stats);

            if (setPoint2 > double.Epsilon)
            {
                SamplePiStats stats2 = new SamplePiStats(setPoint2, tolPlus2, tolMinus2);
                var actValueSeries2 = (ActualValue2 as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
                if (actValueSeries2 != null && actValueSeries2.PropertyLogList != null && actValueSeries2.PropertyLogList.Any())
                    stats2.Values.AddRange(actValueSeries2.PropertyLogList.Select(c => new SamplePiValue() { Value = (double)c.Value, DTStamp = c.Time }));
                resultStats.Add(stats2);
            }

            if (setPoint3 > double.Epsilon)
            {
                SamplePiStats stats3 = new SamplePiStats(setPoint3, tolPlus3, tolMinus3);
                var actValueSeries3 = (ActualValue3 as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
                if (actValueSeries3 != null && actValueSeries3.PropertyLogList != null && actValueSeries3.PropertyLogList.Any())
                    stats3.Values.AddRange(actValueSeries3.PropertyLogList.Select(c => new SamplePiValue() { Value = (double)c.Value, DTStamp = c.Time }));
                resultStats.Add(stats3);
            }

            if (setPoint4 > double.Epsilon)
            {
                SamplePiStats stats4 = new SamplePiStats(setPoint4, tolPlus4, tolMinus4);
                var actValueSeries4 = (ActualValue4 as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
                if (actValueSeries4 != null && actValueSeries4.PropertyLogList != null && actValueSeries4.PropertyLogList.Any())
                    stats4.Values.AddRange(actValueSeries4.PropertyLogList.Select(c => new SamplePiValue() { Value = (double)c.Value, DTStamp = c.Time }));
                resultStats.Add(stats4);
            }

            if (setPoint5 > double.Epsilon)
            {
                SamplePiStats stats5 = new SamplePiStats(setPoint5, tolPlus5, tolMinus5);
                var actValueSeries5 = (ActualValue5 as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
                if (actValueSeries5 != null && actValueSeries5.PropertyLogList != null && actValueSeries5.PropertyLogList.Any())
                    stats5.Values.AddRange(actValueSeries5.PropertyLogList.Select(c => new SamplePiValue() { Value = (double)c.Value, DTStamp = c.Time }));
                resultStats.Add(stats5);
            }

            return resultStats;
        }

        public bool IsEnabledGetValues()
        {
            if (!CanSend())
                return false;
            return true;
        }

        private bool GetValuesFromArchive(IACContainerTNet<double> setPointProp, IACContainerTNet<double> tolPlusProp, IACContainerTNet<double> tolMinusProp, DateTime from, DateTime to, ref double setPoint, ref double tolPlus, ref double tolMinus)
        {
            var setPointSeries = (setPointProp as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
            if (setPointSeries == null || setPointSeries.PropertyLogList == null || !setPointSeries.PropertyLogList.Any())
                return false;
            setPoint = (double)setPointSeries.PropertyLogList.FirstOrDefault().Value;

            var tolPlusSeries = (tolPlusProp as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
            if (tolPlusSeries == null || tolPlusSeries.PropertyLogList == null || !tolPlusSeries.PropertyLogList.Any())
                return false;
            tolPlus = (double)tolPlusSeries.PropertyLogList.FirstOrDefault().Value;

            var tolMinusSeries = (tolMinusProp as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
            if (tolMinusSeries == null || tolMinusSeries.PropertyLogList == null || !tolMinusSeries.PropertyLogList.Any())
                return false;
            tolMinus = (double)tolMinusSeries.PropertyLogList.FirstOrDefault().Value;

            return true;
        }

        [ACMethodInteraction("", "en{'Read logfile'}de{'Logfile lesen'}", 204, true)]
        public string GetLogs()
        {
            if (!IsEnabledGetLogs())
                return null;
            WSResponse<string> response = this.Client.Get(C_GetLogFileString);
            if (!response.Suceeded)
                return null;
            LastLogs = response.Data;
            this.Messages.LogDebug(this.GetACUrl(), "GetLogs()", LastLogs);
            return response.Data;
        }

        public bool IsEnabledGetLogs()
        {
            if (!CanSend())
                return false;
            return true;
        }

        private bool CanSend()
        {
            return     Client != null 
                    && !String.IsNullOrEmpty(Client.ServiceUrl) 
                    && !Client.ConnectionDisabled;
        }

        private static bool _ExtPropInitialized = false;
        internal static void InitializeExtPropInLabOrderPos(ACComponent invoker)
        {
            if (_ExtPropInitialized)
                return;
            _ExtPropInitialized = true;
            gip.core.datamodel.Database typeDB = gip.core.datamodel.Database.GlobalDatabase;
            if (typeDB != null)
            {
                try
                {
                    core.datamodel.ACClass pickingPosType = typeDB.GetACType(typeof(LabOrderPos));
                    if (pickingPosType != null)
                    {
                        core.datamodel.ACClassProperty extPropStats = pickingPosType.GetProperty(PWSamplePiLightBox.C_LabOrderExtFieldStats);
                        if (extPropStats == null)
                        {
                            core.datamodel.ACClass dataTypeSamplePi = typeDB.GetACType(typeof(SamplePiStats));
                            if (dataTypeSamplePi != null)
                            {
                                using (ACMonitor.Lock(typeDB.QueryLock_1X000))
                                {
                                    extPropStats = core.datamodel.ACClassProperty.NewACObject(typeDB, pickingPosType);
                                    extPropStats.ACIdentifier = PWSamplePiLightBox.C_LabOrderExtFieldStats;
                                    extPropStats.ACCaptionTranslation = "en{'SamplePiStats'}de{'SamplePiStats'}";
                                    extPropStats.ValueTypeACClass = dataTypeSamplePi;
                                    extPropStats.ACKind = Global.ACKinds.PSPropertyExt;
                                    extPropStats.ACPropUsage = Global.ACPropUsages.Property;
                                    extPropStats.IsNullable = true;
                                    pickingPosType.ACClassProperty_ACClass.Add(extPropStats);
                                    typeDB.ACSaveChanges();
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    invoker.Messages.LogException("PAESamplePiLightBox", "InitializeExtPropInLabOrderPos()", e);
                }
            }
        }

        [ACMethodInfo("","",9999)]
        public void ResetWeighingCycle()
        {
            WSResponse<string> response = this.Client.Get(C_ResetToleranceCycle);
            if (!response.Suceeded)
            {
                //TODO alarm
            }
        }


        public static bool HandleExecuteACMethod_PAESamplePiLightBox(out object result, IACComponent acComponent, string acMethodName, core.datamodel.ACClassMethod acClassMethod, object[] acParameter)
        {
            //result = null;
            //switch(acMethodName)
            //{
            //    //case "EmptySampleMagazine":
            //    //    EmptySampleMagazine(acComponent);
            //    //    return true;
            //    //case "IsEnabledEmptySampleMagazine":
            //    //    result = IsEnabledEmptySampleMagazine(acComponent);
            //    //    return true;
            //}
            return HandleExecuteACMethod_PAModule(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(SetParamsAndStartOrder):
                    result = SetParamsAndStartOrder((double)acParameter[0], (double)acParameter[1], (double)acParameter[2], (string)acParameter[3], (int)acParameter[4]);
                    return true;
                case nameof(StartOrder):
                    result = StartOrder();
                    return true;
                case nameof(IsEnabledStartOrder):
                    result = IsEnabledStartOrder();
                    return true;
                case nameof(StopOrder):
                    result = StopOrder();
                    return true;
                case nameof(IsEnabledStopOrder):
                    result = IsEnabledStopOrder();
                    return true;
                case nameof(GetValues):
                    result = GetValues();
                    return true;
                case nameof(IsEnabledGetValues):
                    result = IsEnabledGetValues();
                    return true;
                case nameof(GetArchivedValues):
                    result = GetArchivedValues((DateTime)acParameter[0], (DateTime)acParameter[1], (bool)acParameter[2]);
                    return true;
                case nameof(GetLogs):
                    result = GetLogs();
                    return true;
                case nameof(IsEnabledGetLogs):
                    result = IsEnabledGetLogs();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

    [DataContract(Name = "PiV")]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'SamplePiValue'}de{'SamplePiValue'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class SamplePiValue : INotifyPropertyChanged, IVBChartTupleT<DateTime, double>
    {
        double _SetPoint;
        [DataMember(Name = "SP")]
        [ACPropertyInfo(200, "", "en{'SetPoint'}de{'Sollwert'}", "", false)]
        public double SetPoint
        {
            get
            {
                return _SetPoint;
            }
            set
            {
                if (_SetPoint != value)
                {
                    _SetPoint = value;
                    OnPropertyChanged();
                }
            }
        }

        double _Value;
        [DataMember(Name = "V")]
        [ACPropertyInfo(201, "", "en{'Weight'}de{'Gewicht'}", "", false)]
        public double Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged();
                }
            }
        }

        short _TolState;
        [DataMember(Name = "S")]
        [ACPropertyInfo(202, "", "en{'Tolerance state'}de{'Toleranzstatus'}", "", false)]
        public short TolState
        {
            get
            {
                return _TolState;
            }
            set
            {
                if (_TolState != value)
                {
                    _TolState = value;
                    OnPropertyChanged();
                }
            }
        }

        public void RecalcTolSate(double setPoint, double tolPlus, double tolMinus)
        {
            if (Value < setPoint - tolMinus)
                TolState = -1;
            else if (Value > setPoint + tolPlus)
                TolState = 1;
            else
                TolState = 0;
        }


        DateTime _DTStamp;
        [DataMember(Name = "DT")]
        [ACPropertyInfo(203, "", "en{'Time'}de{'Uhrzeit'}", "", false)]
        public DateTime DTStamp
        {
            get
            {
                return _DTStamp;
            }
            set
            {
                if (_DTStamp != value)
                {
                    _DTStamp = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime ValueT1
        {
            get 
            {
                return DTStamp;
            }
        }

        public double ValueT2
        {
            get
            {
                return Value;
            }
        }

        public object Value1
        {
            get
            {
                return DTStamp;
            }
        }

        public object Value2
        {
            get
            {
                return Value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    [DataContract(Name = "PiS")]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'SamplePiStats'}de{'SamplePiStats'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class SamplePiStats : INotifyPropertyChanged
    {
        public SamplePiStats() : base()
        {
        }

        public SamplePiStats(double setPoint, double tolPlus, double tolMinus)
        {
            SetToleranceAndRecalc(setPoint, tolPlus, tolMinus);
        }

        public void SetToleranceAndRecalc(double setPoint, double tolPlus, double tolMinus, bool recalc = false)
        {
            SetPoint = setPoint;
            TolPlus = tolPlus;
            TolMinus = tolMinus;
            if (recalc)
                Values.ForEach(c => c.RecalcTolSate(setPoint, tolPlus, tolMinus));
        }

        [IgnoreDataMember]
        double _SetPoint;
        [ACPropertyInfo(201, "", "en{'Setpoint'}de{'Sollwert'}", "", false)]
        [DataMember(Name = "SP")]
        public double SetPoint
        {
            get
            {
                return _SetPoint;
            }
            set
            {
                if (_SetPoint != value)
                {
                    _SetPoint = value;
                    OnPropertyChanged();
                }
            }
        }


        [IgnoreDataMember]
        double _TolPlus;
        [ACPropertyInfo(202, "", "en{'Tolerance +'}de{'Toleranz +'}", "", false)]
        [DataMember(Name = "P")]
        public double TolPlus
        {
            get
            {
                return _TolPlus;
            }
            set
            {
                if (_TolPlus != value)
                {
                    _TolPlus = value;
                    OnPropertyChanged();
                }
            }
        }

        [IgnoreDataMember]
        double _TolMinus;
        [ACPropertyInfo(203, "", "en{'Tolerance -'}de{'Toleranz -'}", "", false)]
        [DataMember(Name = "M")]
        public double TolMinus
        {
            get
            {
                return _TolMinus;
            }
            set
            {
                if (_TolMinus != value)
                {
                    _TolMinus = value;
                    OnPropertyChanged();
                }
            }
        }

        [IgnoreDataMember]
        private double? _AverageValue;
        [ACPropertyInfo(204, "", "en{'Average weight'}de{'Durchschnittsgewicht'}", "", false)]
        [IgnoreDataMember]
        public double AverageValue
        {
            get
            {
                if (!_AverageValue.HasValue)
                    RecalcStatistics();
                return _AverageValue.Value;
            }
            protected set
            {
                _AverageValue = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        private int? _CountAbove;
        [ACPropertyInfo(205, "", "en{'Count above'}de{'Anzahl Überhalb'}", "", false)]
        [IgnoreDataMember]
        public int CountAbove
        {
            get
            {
                if (!_CountAbove.HasValue)
                    RecalcStatistics();
                return _CountAbove.Value;
            }
            protected set
            {
                _CountAbove = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        private int? _CountBelow;
        [ACPropertyInfo(206, "", "en{'Count below'}de{'Anzahl Unterhalb'}", "", false)]
        [IgnoreDataMember]
        public int CountBelow
        {
            get
            {
                if (!_CountBelow.HasValue)
                    RecalcStatistics();
                return _CountBelow.Value;
            }
            protected set
            {
                _CountBelow = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        private int? _CountInTol;
        [ACPropertyInfo(207, "", "en{'Count in tolerance'}de{'Anzahl in Toleranz'}", "", false)]
        [IgnoreDataMember]
        public int CountInTol
        {
            get
            {
                if (!_CountInTol.HasValue)
                    RecalcStatistics();
                return _CountInTol.Value;
            }
            protected set
            {
                _CountInTol = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        private int? _CountValid;
        [ACPropertyInfo(208, "", "en{'Valid measurements'}de{'Gültige Messwerte'}", "", false)]
        [IgnoreDataMember]
        public int CountValid
        {
            get
            {
                if (!_CountValid.HasValue)
                    RecalcStatistics();
                return _CountValid.Value;
            }
            protected set
            {
                _CountValid = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        private int? _CountInvalid;
        [ACPropertyInfo(209, "", "en{'Inalid measurements'}de{'Ungültige Messwerte'}", "", false)]
        [IgnoreDataMember]
        public int CountInvalid
        {
            get
            {
                if (!_CountInvalid.HasValue)
                    RecalcStatistics();
                return _CountInvalid.Value;
            }
            protected set
            {
                _CountInvalid = value;
                OnPropertyChanged();
            }
        }

        [IgnoreDataMember]
        private List<SamplePiValue> _Values;
        [ACPropertyInfo(210, "", "en{'Measurements'}de{'Messwerte'}", "", false)]
        [DataMember(Name = "VS")]
        public List<SamplePiValue> Values
        {
            get
            {
                if (_Values == null)
                    _Values = new List<SamplePiValue>();
                return _Values;
            }
            set
            {
                _Values = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<SamplePiValue> ValidValues
        {
            get
            {
                double filterMinValue = SetPoint * 0.5;
                double filterMaxValue = SetPoint * 1.5;
                return Values.Where(c => c.Value > filterMinValue && c.Value < filterMaxValue);
            }
        }

        public void RecalcStatistics()
        {
            Values.ForEach(c => c.RecalcTolSate(SetPoint, TolPlus, TolMinus));
            if (ValidValues != null && ValidValues.Any())
            {
                SamplePiValue[] validValues = ValidValues.ToArray();
                _CountValid = validValues.Count();
                _CountInvalid = Values.Count - _CountValid.Value;
                _CountInTol = validValues.Count(c => c.TolState == 0);
                _CountBelow = validValues.Count(c => c.TolState < 0);
                _CountAbove = validValues.Count(c => c.TolState > 0);
                _AverageValue = validValues.Average(c => c.Value);
            }
            else
            {
                _CountValid = 0;
                _CountInvalid = 0;
                _CountInTol = 0;
                _CountBelow = 0;
                _CountAbove = 0;
                _AverageValue = 0;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }


    [CollectionDataContract(Name = "PiSC")]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'SamplePiStatsCollection'}de{'SamplePiStatsCollection'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class SamplePiStatsCollection : List<SamplePiStats>
    {

    }

}
