using gip.core.autocomponent;
using gip.core.communication;
using gip.core.datamodel;
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

        public PAESamplePiLightBox(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACPostInit()
        {
            (SetPoint as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            (TolPlus as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            (TolMinus as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            (ActualValue as ACPropertyNetServerBase<double>).ForceCreatePropertyValueLog();
            if (ApplicationManager != null)
                ApplicationManager.ProjectWorkCycleR5min += ApplicationManager_ProjectWorkCycleR5min;
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (ApplicationManager != null)
                ApplicationManager.ProjectWorkCycleR5min -= ApplicationManager_ProjectWorkCycleR5min;
            return base.ACDeInit(deleteACClassTask);
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
        public IACContainerTNet<bool> IsRecording { get; set; }

        [ACPropertyBindingSource(201, "Values", "en{'Setpoint'}de{'Sollwert'}", "", false, true)]
        public IACContainerTNet<double> SetPoint { get; set; }

        [ACPropertyBindingSource(202, "Values", "en{'Tolerance +'}de{'Toleranz +'}", "", false, true)]
        public IACContainerTNet<double> TolPlus { get; set; }

        [ACPropertyBindingSource(203, "Values", "en{'Tolerance -'}de{'Toleranz -'}", "", false, true)]
        public IACContainerTNet<double> TolMinus { get; set; }

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

        [ACPropertyBindingSource(207, "Values", "en{'Average Value'}de{'Mittelwert'}", "", false, true)]
        public IACContainerTNet<double> AverageValue { get; set; }

        [ACPropertyBindingSource(208, "Values", "en{'Average-State'}de{'Mittelwert-Status'}", "", false, true)]
        public IACContainerTNet<short> AverageState { get; set; }

        #endregion

        #region Methods

        private void ApplicationManager_ProjectWorkCycleR5min(object sender, EventArgs e)
        {
            if (IsRecording.ValueT)
            {
                this.ApplicationManager.ApplicationQueue.Add(() => { GetValues(); });
            }
        }

        [ACMethodInfo("", "en{'Send paramas an start order'}de{'Sende Parameter und starte Auftrag'}", 200)]
        public bool SetParamsAndStartOrder(double setPoint, double tolPlus, double tolMinus)
        {
            if (!IsEnabledStartOrder())
                return false;
            if (setPoint <= 0.000001 || tolPlus <= 0.000001 || tolMinus <= 0.000001)
                return false;
            if (IsRecording.ValueT)
            {
                GetValues();
                StopOrder();
            }
            SetPoint.ValueT = setPoint;
            TolPlus.ValueT = tolPlus;
            TolMinus.ValueT = tolMinus;
            return StartOrder();
        }

        [ACMethodCommand("", "en{'Start order'}de{'Auftrag starten'}", 201, true)]
        public bool StartOrder()
        {
            if (!IsEnabledStartOrder())
                return false;
            if (IsRecording.ValueT)
            {
                GetValues();
                StopOrder();
            }

            AverageValue.ValueT = 0;
            AverageState.ValueT = 0;
            UriBuilder uriBuilder = new UriBuilder(C_ActivateOrderString + "?");
            NameValueCollection parameters = HttpUtility.ParseQueryString(uriBuilder.Query);
            parameters[C_WeightSetpointString] = SetPoint.ValueT.ToString("0.######").Replace(",",".");
            parameters[C_ToleranceAboveString] = TolPlus.ValueT.ToString("0.######").Replace(",", ".");
            parameters[C_ToleranceBelowString] = TolMinus.ValueT.ToString("0.######").Replace(",", ".");
            uriBuilder.Query = parameters.ToString();
            string relativeRequestUrl = String.Format("{0}{1}", C_ActivateOrderString, uriBuilder.Query.ToString());
            WSResponse<string> response = this.Client.Get(relativeRequestUrl);
            if (!response.Suceeded)
                return false;
            IsRecording.ValueT = true;
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
            IsRecording.ValueT = false;
            return true;
        }

        public bool IsEnabledStopOrder()
        {
            if (!CanSend())
                return false;
            return true;
        }

        [ACMethodInfo("", "en{'Read collected values'}de{'Lese protokollierte Werte'}", 203, true)]
        public SamplePiStats GetValues()
        {
            if (!IsEnabledGetValues())
                return null;
            WSResponse<string> response = this.Client.Get(C_GetToleranceWeighDataString);
            if (!response.Suceeded)
                return null;
            LastResultValues = response.Data;
            SamplePiStats result = ParseValues(response.Data, SetPoint.ValueT, TolPlus.ValueT, TolMinus.ValueT);
            if (result != null && result.Any())
            {
                ACPropertyNetSource<double> actValueProp = ActualValue as ACPropertyNetSource<double>;
                if (actValueProp.PropertyLog != null)
                {
                    foreach (SamplePiValue value in result)
                    {
                        actValueProp.PropertyLog.AddValue(value.Value, value.DTStamp);
                    }
                    actValueProp.PropertyLog.SaveChanges();
                }
                var logProp = (SetPoint as ACPropertyNetServerBase<double>);
                if (logProp != null && logProp.PropertyLog != null)
                    logProp.PropertyLog.AddValue(this.SetPoint.ValueT, DateTime.Now);

                logProp = (TolPlus as ACPropertyNetServerBase<double>);
                if (logProp != null && logProp.PropertyLog != null)
                    logProp.PropertyLog.AddValue(this.SetPoint.ValueT + this.TolPlus.ValueT, DateTime.Now);

                logProp = (TolMinus as ACPropertyNetServerBase<double>);
                if (logProp != null && logProp.PropertyLog != null)
                    logProp.PropertyLog.AddValue(this.SetPoint.ValueT - this.TolMinus.ValueT, DateTime.Now);

                this.AverageValue.ValueT = result.AverageValue;
                if (result.CountInTol > result.CountAbove + result.CountBelow)
                    this.AverageState.ValueT = 0;
                else if (result.CountAbove > result.CountBelow)
                    this.AverageState.ValueT = 1;
                else if (result.CountAbove <= result.CountBelow)
                    this.AverageState.ValueT = -1;
            }
            return result;
        }

        public static SamplePiStats ParseValues(string values, double setPoint, double tolPlus, double tolMinus)
        {
            //0.240;Below;19/03/2022 07:07:54
            //0.505;Above;19/03/2022 07:08:33
            //0.240;Within;19/03/2022 07:22:54
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
                    double weight = 0;
                    if (!Double.TryParse(valArr[0], NumberStyles.AllowDecimalPoint, cultureInfo, out weight))
                        continue;
                    DateTime stamp;
                    if (!DateTime.TryParseExact(valArr[2], "dd/MM/yyyy HH:mm:ss", cultureInfo, System.Globalization.DateTimeStyles.None, out stamp))
                        continue;
                    stamp = stamp.ToLocalTime();
                    SamplePiValue piValue = new SamplePiValue() { DTStamp = stamp, Value = weight };
                    if (valArr[1] == "Above")
                        piValue.TolState = 1;
                    else if (valArr[1] == "Below")
                        piValue.TolState = -1;
                    else
                        piValue.TolState = 0;
                    result.Add(piValue);
                }
            }
            
            return result;
        }

        [ACMethodInfo("", "en{'Read archived values'}de{'Lese archivierte Werte'}", 209, true)]
        public SamplePiStats GetArchivedValues(DateTime from, DateTime to, bool useCurrentSetPoints)
        {
            double setPoint = 0.0;
            double tolPlus = 0.0;
            double tolMinus = 0.0;
            if (useCurrentSetPoints)
            {
                setPoint = SetPoint.ValueT;
                tolPlus = TolPlus.ValueT;
                tolMinus = TolMinus.ValueT;
            }
            if (!useCurrentSetPoints || setPoint <= double.Epsilon || tolPlus <= double.Epsilon || tolMinus <= double.Epsilon)
            {
                var setPointSeries = (SetPoint as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
                if (setPointSeries == null || setPointSeries.PropertyLogList == null || !setPointSeries.PropertyLogList.Any())
                    return null;
                setPoint = (double) setPointSeries.PropertyLogList.FirstOrDefault().Value;

                var tolPlusSeries = (TolPlus as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
                if (tolPlusSeries == null || tolPlusSeries.PropertyLogList == null || !tolPlusSeries.PropertyLogList.Any())
                    return null;
                tolPlus = (double)tolPlusSeries.PropertyLogList.FirstOrDefault().Value;

                var tolMinusSeries = (TolMinus as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
                if (tolMinusSeries == null || tolMinusSeries.PropertyLogList == null || !tolMinusSeries.PropertyLogList.Any())
                    return null;
                tolMinus = (double)tolMinusSeries.PropertyLogList.FirstOrDefault().Value;
            }

            SamplePiStats stats = new SamplePiStats(setPoint, tolPlus, tolMinus);
            var actValueSeries =  (ActualValue as ACPropertyNetServerBase<double>).GetArchiveLog(from, to);
            if (actValueSeries != null && actValueSeries.PropertyLogList != null && actValueSeries.PropertyLogList.Any())
                stats.AddRange(actValueSeries.PropertyLogList.Select(c => new SamplePiValue() { Value = (double)c.Value, DTStamp = c.Time }));
            return stats;
        }

        public bool IsEnabledGetValues()
        {
            if (!CanSend())
                return false;
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
            return Client != null && !String.IsNullOrEmpty(Client.ServiceUrl);
        }

        public static bool HandleExecuteACMethod_PAESamplePiLightBox(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
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

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(SetParamsAndStartOrder):
                    result = SetParamsAndStartOrder((double)acParameter[0], (double)acParameter[1], (double)acParameter[2]);
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
    public class SamplePiValue : INotifyPropertyChanged
    {
        double _Value;
        [DataMember(Name = "V")]
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


        DateTime _DTStamp;
        [DataMember(Name = "DT")]
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'SamplePiStats'}de{'SamplePiStats'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class SamplePiStats : List<SamplePiValue>, INotifyPropertyChanged
    {
        public SamplePiStats() : base()
        {
        }

        public SamplePiStats(double setPoint, double tolPlus, double tolMinus)
        {
            SetPoint = setPoint;
            TolMinus = tolPlus;
            TolMinus = tolMinus;
        }


        [IgnoreDataMember]
        double _SetPoint;
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

        private int? _CountInTol;
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

        public IEnumerable<SamplePiValue> ValidValues
        {
            get
            {
                double filterMinValue = SetPoint * 0.5;
                double filterMaxValue = SetPoint * 1.5;
                return this.Where(c => c.Value > filterMinValue && c.Value < filterMaxValue);
            }
        }

        public void RecalcStatistics()
        {
            if (ValidValues != null && ValidValues.Any())
            {
                SamplePiValue[] validValues = ValidValues.ToArray();
                _CountValid = validValues.Count();
                _CountInvalid = Count - _CountValid.Value;
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

}
