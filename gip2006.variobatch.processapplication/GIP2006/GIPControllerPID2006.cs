// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;
using gip.core.communication;
using gip.core.communication.ISOonTCP;
using System.Threading;
using System.Xml;
using System.Runtime.CompilerServices;

namespace gip2006.variobatch.processapplication
{
    #region PID Data
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'PID-Controller data'}de{'PID-Regler Daten'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class GIPControllerPIDData2006 : INotifyPropertyChanged
    {
        public GIPControllerPIDData2006()
        {
        }

        #region PID-Properties
        [IgnoreDataMember]
        private Double _DEADB_W;
        /// <summary>
        /// <para xml:lang="en">Dead band width (= range zero to dead band upper limit) (determines size of dead band)</para>
        /// <para xml:lang="de">Die Regeldifferenz wird über eine Totzone geführt. Der Eingang "Totzonenbreite" bestimmt die Größe der Totzone.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(1, "", "en{'Dead band width'}de{'Totzonenbreite'}")]
        public Double DEADB_W
        {
            get
            {
                return _DEADB_W;
            }
            set
            {
                _DEADB_W = value;
                OnPropertyChanged("DEADB_W");
            }
        }


        [IgnoreDataMember]
        private Double _Gain;
        /// <summary>
        /// <para xml:lang="en">Proportional gain (= controller gain)</para>
        /// <para xml:lang="de">Der Eingang "Proportionalbeiwert" gibt die Reglerverstärkung an</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(2, "", "en{'Gain'}de{'P-Anteil'}")]
        public Double Gain
        {
            get
            {
                return _Gain;
            }
            set
            {
                _Gain = value;
                OnPropertyChanged("Gain");
            }
        }


        [IgnoreDataMember]
        private Double _LMN_HLM;
        /// <summary>
        /// <para xml:lang="en">Manipulated value: high limit (0-100)</para>
        /// <para xml:lang="de">Der Stellwert wird immer auf eine obere und untere Grenze begrenzt. Der Eingang "Stellwert obere Begrenzung" gibt die obere Begrenzung an. (0-100)</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(3, "", "en{'High Limit'}de{'Oberer Grenzwert'}")]
        public Double LMN_HLM
        {
            get
            {
                return _LMN_HLM;
            }
            set
            {
                _LMN_HLM = value;
                OnPropertyChanged("LMN_HLM");
            }
        }


        [IgnoreDataMember]
        private Double _LMN_LLM;
        /// <summary>
        /// <para xml:lang="en">Manipulated value: low limit (0- -100)</para>
        /// <para xml:lang="de">Der Stellwert wird immer auf eine obere und untere Grenze begrenzt. Der Eingang "Stellwert untere Begrenzung" gibt die untere Begrenzung an. (0- -100)</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(4, "", "en{'Low limit'}de{'Unterer Grenzwert'}")]
        public Double LMN_LLM
        {
            get
            {
                return _LMN_LLM;
            }
            set
            {
                _LMN_LLM = value;
                OnPropertyChanged("LMN_LLM");
            }
        }


        [IgnoreDataMember]
        private Double _LMN_FAC;
        /// <summary>
        /// <para xml:lang="en">Manipulated value factor (factor for adapting the manipulated value range)</para>
        /// <para xml:lang="de">Stellfaktor	Der Eingang "Stellwertfaktor" wird mit dem Stellwert multipliziert. Der Eingang dient zur Anpassung des Stellwertbereiches.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(5, "", "en{'Manipulated value factor'}de{'Stellfaktor'}")]
        public Double LMN_FAC
        {
            get
            {
                return _LMN_FAC;
            }
            set
            {
                _LMN_FAC = value;
                OnPropertyChanged("LMN_FAC");
            }
        }


        [IgnoreDataMember]
        private Double _LMN_OFF;
        /// <summary>
        /// <para xml:lang="en">Manipulated value offset (zero point of the manipulated value normalization)</para>
        /// <para xml:lang="de">Der Eingang "Stellwertoffset" wird mit dem Stellwert addiert. Der Eingang dient zur Anpassung des Stellwertbereiches.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(6, "", "en{'Manipulated value offset'}de{'Stellwertoffset'}")]
        public Double LMN_OFF
        {
            get
            {
                return _LMN_OFF;
            }
            set
            {
                _LMN_OFF = value;
                OnPropertyChanged("LMN_OFF");
            }
        }


        [IgnoreDataMember]
        private Double _PV_FAC;
        /// <summary>
        /// <para xml:lang="en">Actual value factor</para>
        /// <para xml:lang="de">Der Eingang "Istwertfaktor" wird mit dem Istwert multipliziert. Der Eingang dient zur Anpassung des Istwertbereiches.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(7, "", "en{'Actual value factor'}de{'Istwertfaktor'}")]
        public Double PV_FAC
        {
            get
            {
                return _PV_FAC;
            }
            set
            {
                _PV_FAC = value;
                OnPropertyChanged("PV_FAC");
            }
        }


        [IgnoreDataMember]
        private Double _PV_OFF;
        /// <summary>
        /// <para xml:lang="en">Actual value offset</para>
        /// <para xml:lang="de">Der Eingang "Istwertoffset" wird mit dem Istwert addiert. Der Eingang dient zur Anpassung des Istwertbereiches.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(8, "", "en{'Actual value offset'}de{'Istwertoffset'}")]
        public Double PV_OFF
        {
            get
            {
                return _PV_OFF;
            }
            set
            {
                _PV_OFF = value;
                OnPropertyChanged("PV_OFF");
            }
        }


        [IgnoreDataMember]
        private Double _SP_RAMP;
        /// <summary>
        /// <para xml:lang="en">SP_RAMP</para>
        /// <para xml:lang="de">SP_RAMP</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(9, "", "en{'SP_RAMP'}de{'SP_RAMP'}")]
        public Double SP_RAMP
        {
            get
            {
                return _SP_RAMP;
            }
            set
            {
                _SP_RAMP = value;
                OnPropertyChanged("SP_RAMP");
            }
        }


        [IgnoreDataMember]
        private Double _HLIM;
        /// <summary>
        /// <para xml:lang="en">HLIM</para>
        /// <para xml:lang="de">HLIM</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(10, "", "en{'HLIM'}de{'HLIM'}")]
        public Double HLIM
        {
            get
            {
                return _HLIM;
            }
            set
            {
                _HLIM = value;
                OnPropertyChanged("HLIM");
            }
        }


        [IgnoreDataMember]
        private Double _HIST;
        /// <summary>
        /// <para xml:lang="en">HIST</para>
        /// <para xml:lang="de">HIST</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(11, "", "en{'HIST'}de{'HIST'}")]
        public Double HIST
        {
            get
            {
                return _HIST;
            }
            set
            {
                _HIST = value;
                OnPropertyChanged("HIST");
            }
        }


        [IgnoreDataMember]
        private TimeSpan _TM_Lag;
        /// <summary>
        /// <para xml:lang="en">Time lag of the D component</para>
        /// <para xml:lang="de">Verzögerungszeit des D-Anteils. Der Algorithmus des D-Anteils beinhaltet eine Verzögerung, die am Eingang "Verzögerungszeit des D-Anteils" parametriert werden kann.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(12, "", "en{'Time lag of the D component'}de{'D Anteil Verzögerung'}")]
        public TimeSpan TM_Lag
        {
            get
            {
                return _TM_Lag;
            }
            set
            {
                _TM_Lag = value;
                OnPropertyChanged("TM_Lag");
            }
        }


        [IgnoreDataMember]
        private TimeSpan _TI;
        /// <summary>
        /// <para xml:lang="en">Reset time</para>
        /// <para xml:lang="de">Der Eingang "Integrationszeit" bestimmt das Zeitverhalten des Integrierers.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(13, "", "en{'Reset time'}de{'I-Anteil'}")]
        public TimeSpan TI
        {
            get
            {
                return _TI;
            }
            set
            {
                _TI = value;
                OnPropertyChanged("TI");
            }
        }


        [IgnoreDataMember]
        private TimeSpan _TD;
        /// <summary>
        /// <para xml:lang="en">Derivative action time</para>
        /// <para xml:lang="de">Der Eingang "Differenzierzeit" bestimmt das Zeitverhalten des Differenzierers.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(14, "", "en{'Derivative action time'}de{'D-Anteil'}")]
        public TimeSpan TD
        {
            get
            {
                return _TD;
            }
            set
            {
                _TD = value;
                OnPropertyChanged("TD");
            }
        }


        [IgnoreDataMember]
        private bool _P_Sel;
        /// <summary>
        /// <para xml:lang="en">P action on</para>
        /// <para xml:lang="de">Im PID-Algorithmus lassen sich die PID-Anteile einzeln zu- und abschalten. Der P-Anteil ist eingeschaltet, wenn der Eingang "P-Anteil einschalten" gesetzt ist.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(15, "", "en{'P action on'}de{'P-Anteil an'}")]
        public bool P_Sel
        {
            get
            {
                return _P_Sel;
            }
            set
            {
                _P_Sel = value;
                OnPropertyChanged("P_Sel");
            }
        }


        [IgnoreDataMember]
        private bool _I_Sel;
        /// <summary>
        /// <para xml:lang="en">I_Sel</para>
        /// <para xml:lang="de">Im PID-Algorithmus lassen sich die PID-Anteile einzeln zu- und abschalten. Der I-Anteil ist eingeschaltet, wenn der Eingang "I-Anteil einschalten" gesetzt ist.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(16, "", "en{'I action on'}de{'I-Anteil an'}")]
        public bool I_Sel
        {
            get
            {
                return _I_Sel;
            }
            set
            {
                _I_Sel = value;
                OnPropertyChanged("I_Sel");
            }
        }


        [IgnoreDataMember]
        private bool _D_Sel;
        /// <summary>
        /// <para xml:lang="en">D_Sel</para>
        /// <para xml:lang="de">Im PID-Algorithmus lassen sich die PID-Anteile einzeln zu- und abschalten. Der D-Anteil ist eingeschaltet, wenn der Eingang "D-Anteil einschalten" gesetzt ist.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(17, "", "en{'D action on'}de{'D-Anteil an'}")]
        public bool D_Sel
        {
            get
            {
                return _D_Sel;
            }
            set
            {
                _D_Sel = value;
                OnPropertyChanged("D_Sel");
            }
        }


        [IgnoreDataMember]
        private bool _Man_On;
        /// <summary>
        /// <para xml:lang="en">Manual mode on (loop opened, LMN set manually)</para>
        /// <para xml:lang="de">Ist der Eingang "Handbetrieb einschalten" gesetzt, ist der Regelkreis unterbrochen. Als Stellwert wird ein Handwert vorgegeben.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(18, "", "en{'Manual mode on'}de{'Handbetrieb ein'}")]
        public bool Man_On
        {
            get
            {
                return _Man_On;
            }
            set
            {
                _Man_On = value;
                OnPropertyChanged("Man_On");
            }
        }


        [IgnoreDataMember]
        private Double _SP_INT;
        /// <summary>
        /// <para xml:lang="en">Internal setpoint (for setting the setpoint with operator interface functions)</para>
        /// <para xml:lang="de">Der Eingang "Interner Sollwert" dient zur Vorgabe eines Sollwertes.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(19, "", "en{'Internal setpoint'}de{'Interner Sollwert'}")]
        public Double SP_INT
        {
            get
            {
                return _SP_INT;
            }
            set
            {
                _SP_INT = value;
                OnPropertyChanged("SP_INT");
            }
        }


        [IgnoreDataMember]
        private Double _PV_IN;
        /// <summary>
        /// <para xml:lang="en">Process variable input (PV in floating-point format)</para>
        /// <para xml:lang="de">Am Eingang "Istwert Eingang" kann ein Inbetriebsetzungswert parametriert oder ein externer Istwert im Gleitpunktformat verschaltet werden.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(20, "", "en{'Process variable input'}de{'Istwert Eingang'}")]
        public Double PV_IN
        {
            get
            {
                return _PV_IN;
            }
            set
            {
                _PV_IN = value;
                OnPropertyChanged("PV_IN");
            }
        }


        [IgnoreDataMember]
        private Double _LMN;
        /// <summary>
        /// <para xml:lang="en">Manipulated value signal (after control algorithm)</para>
        /// <para xml:lang="de">Am Ausgang "Stellwert" wird der effektiv wirkende Stellwert in Gleitpunktformat ausgegeben.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(21, "", "en{'Manipulated value signal'}de{'Stellwert'}")]
        public Double LMN
        {
            get
            {
                return _LMN;
            }
            set
            {
                _LMN = value;
                OnPropertyChanged("LMN");
            }
        }


        [IgnoreDataMember]
        private Double _MAN_VALUE;
        /// <summary>
        /// <para xml:lang="en">Manual setpoint</para>
        /// <para xml:lang="de">Ist der Eingang "Handbetrieb einschalten" gesetzt, ist der Regelkreis unterbrochen. Als Stellwert wird ein Handwert vorgegeben.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(22, "", "en{'Manual setpoint'}de{'Hand Sollwert'}")]
        public Double MAN_VALUE
        {
            get
            {
                return _MAN_VALUE;
            }
            set
            {
                _MAN_VALUE = value;
                OnPropertyChanged("MAN_VALUE");
            }
        }


        [IgnoreDataMember]
        private Double _ITL_VALUE;
        /// <summary>
        /// <para xml:lang="en">Initial value for I action</para>
        /// <para xml:lang="de">Der Ausgang des Integrierers kann am Eingang I_ITL_ON gesetzt werden. Am Eingang "Initialisierungswert für I-Anteil" steht der Initialisierungwert.</para>
        /// </summary>
        [DataMember]
        [ACPropertyInfo(23, "", "en{'Initial value for I action'}de{'Initialisierungswert für I-Anteil'}")]
        public Double ITL_VALUE
        {
            get
            {
                return _ITL_VALUE;
            }
            set
            {
                _ITL_VALUE = value;
                OnPropertyChanged("ITL_VALUE");
            }
        }

        #endregion

        #region Methods

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
    #endregion


    #region Serializer
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'Serializer for PID-Controller'}de{'Serialisierer für PID Regler'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPSerialControllerPID2006 : ACSessionObjSerializer
    {
        #region c´tors
        public GIPSerialControllerPID2006(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        public const int StructLen = 78;
        public const int DBReservLen = 22;

        private readonly string _ConverterTypeName = typeof(GIPControllerPIDData2006).FullName;
        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return typeOrACMethodName == _ConverterTypeName;
        }

        public override bool SendObject(object complexObj, object prevComplexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            S7TCPSession s7Session = ParentACComponent as S7TCPSession;
            if (s7Session == null || complexObj == null)
                return false;
            if (!s7Session.PLCConn.IsConnected)
                return false;
            GIPControllerPIDData2006 request = complexObj as GIPControllerPIDData2006;
            if (request == null)
                return false;
            byte[] sendPackage1 = new byte[StructLen];
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.DEADB_W), 0, sendPackage1, 0, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.Gain), 0, sendPackage1, 4, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.LMN_HLM), 0, sendPackage1, 8, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.LMN_LLM), 0, sendPackage1, 12, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.LMN_FAC), 0, sendPackage1, 16, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.LMN_OFF), 0, sendPackage1, 20, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.PV_FAC), 0, sendPackage1, 24, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.PV_OFF), 0, sendPackage1, 28, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.SP_RAMP), 0, sendPackage1, 32, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.HLIM), 0, sendPackage1, 36, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.HIST), 0, sendPackage1, 40, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(Convert.ToDouble(request.TM_Lag.TotalSeconds)), 0, sendPackage1, 44, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(Convert.ToDouble(request.TI.TotalSeconds)), 0, sendPackage1, 48, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(Convert.ToDouble(request.TD.TotalSeconds)), 0, sendPackage1, 52, 4);
            BitAccessForInt16 bitAccess = new BitAccessForInt16();
            bitAccess.Bit08 = request.P_Sel;
            bitAccess.Bit09 = request.I_Sel;
            bitAccess.Bit10 = request.D_Sel;
            bitAccess.Bit11 = request.Man_On;
            EndianessHelper.Int16ToByteArray((Int16)bitAccess.ValueT, EndianessEnum.BigEndian);
            Array.Copy(EndianessHelper.Int16ToByteArray((Int16)bitAccess.ValueT, EndianessEnum.BigEndian), 0, sendPackage1, 56, 2);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.SP_INT), 0, sendPackage1, 58, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.PV_IN), 0, sendPackage1, 62, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.LMN), 0, sendPackage1, 66, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.MAN_VALUE), 0, sendPackage1, 70, 4);
            Array.Copy(gip.core.communication.ISOonTCP.Types.Real.ToByteArray(request.ITL_VALUE), 0, sendPackage1, 74, 4);

            PLC.Result errCode = s7Session.PLCConn.WriteBytes(DataTypeEnum.DataBlock, dbNo, offset, ref sendPackage1);
            return errCode == null || errCode.IsSucceeded;
        }

        public override object ReadObject(object complexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            S7TCPSession s7Session = ParentACComponent as S7TCPSession;
            if (s7Session == null || complexObj == null)
                return null;
            if (!s7Session.PLCConn.IsConnected)
                return null;
            GIPControllerPIDData2006 response = complexObj as GIPControllerPIDData2006;
            if (response == null)
                return null;

            byte[] readPackage1 = new byte[StructLen];
            PLC.Result errCode = s7Session.PLCConn.ReadBytes(DataTypeEnum.DataBlock, dbNo, offset, StructLen, out readPackage1);
            if (errCode != null && !errCode.IsSucceeded)
                return null;

            response.DEADB_W = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 0);
            response.Gain = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 4);
            response.LMN_HLM = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 8);
            response.LMN_LLM = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 12);
            response.LMN_FAC = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 16);
            response.LMN_OFF = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 20);
            response.PV_FAC = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 24);
            response.PV_OFF = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 28);
            response.SP_RAMP = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 32);
            response.HLIM = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 36);
            response.HIST = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 40);
            response.TM_Lag = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 44));
            response.TI = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 48));
            response.TD = TimeSpan.FromSeconds(gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 52));

            BitAccessForInt16 bitAccess = new BitAccessForInt16();
            bitAccess.ValueT = (UInt16)EndianessHelper.Int16FromByteArray(readPackage1, 56, EndianessEnum.BigEndian);
            response.P_Sel = bitAccess.Bit08;
            response.I_Sel = bitAccess.Bit09;
            response.D_Sel = bitAccess.Bit10;
            response.Man_On = bitAccess.Bit11;

            response.SP_INT = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 58);
            response.PV_IN = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 62);
            response.LMN = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 66);
            response.MAN_VALUE = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 70);
            response.ITL_VALUE = gip.core.communication.ISOonTCP.Types.Real.FromByteArray(readPackage1, 74);

            return response;
        }
    }
    #endregion


    #region Controller
    /// <summary>
    /// Baseclass for converting State and Types between Standard-Model-Components and DataAccess-/Vendor Model
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PID-Controller'}de{'PID-Regler'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class GIPControllerPID2006 : PAClassAlarmingBase
    {
        #region c'tors
        public GIPControllerPID2006(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _DBNo = new ACPropertyConfigValue<UInt16>(this, "DBNo", 0);
            _DBOffset = new ACPropertyConfigValue<UInt16?>(this, "DBOffset", null);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _ = DBNo;
            _ = DBOffset;
            return true;
        }

        public override bool ACPostInit()
        {
            if (CData.ValueT == null)
                CData.ValueT = new GIPControllerPIDData2006();
            AddMeToPollList();
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            StopPollThread();
            lock (_PollLock)
            {
                _ControllerToPoll = null;
            }
            return base.ACDeInit(deleteACClassTask);
        }

        protected void AddMeToPollList()
        {
            lock (_PollLock)
            {
                // If PLC has lost it's battery buffered data, then don't override the values with zero values!
                if (Root.PropPersistenceOff)
                    return;
                if (_ControllerToPoll == null)
                {
                    _ControllerToPoll = new List<ACRef<GIPControllerPID2006>>();
                    _ControllerToPoll.Add(new ACRef<GIPControllerPID2006>(this, ParentACComponent));
                    _PollThread = new ACThread(RunPollCycle);
                    _PollThread.Name = "ACUrl:" + this.GetACUrl() + ";RunPollCycle();";
                    _PollThread.Start();
                }
                else
                    _ControllerToPoll.Add(new ACRef<GIPControllerPID2006>(this, ParentACComponent));
            }
        }

        protected void StopPollThread()
        {
            lock (_PollLock)
            {
                if (_PollThread != null)
                {
                    if (_ShutdownEvent != null && _ShutdownEvent.SafeWaitHandle != null && !_ShutdownEvent.SafeWaitHandle.IsClosed)
                        _ShutdownEvent.Set();
                    if (!_PollThread.Join(10000))
                        _PollThread.Abort();

                    _PollThread = null;
                    _ShutdownEvent = null;
                }
            }
        }
        #endregion

        #region Properties

        #region Configuration
        //[ACPropertyInfo(9999, DefaultValue = 0, ForceBroadcast = true)]
        //public Int16 AggrNo
        //{
        //    get;
        //    set;
        //}

        [ACPropertyBindingSource(9999, "", "en{'Aggregate number'}de{'Aggregatnummer'}", "", false, false)]
        public IACContainerTNet<Int16> AggrNo { get; set; }


        private ACPropertyConfigValue<UInt16> _DBNo;
        [ACPropertyConfig("en{'Datablocknumber'}de{'DB-Nummer'}")]
        public UInt16 DBNo
        {
            get
            {

                UInt16 resultValue = _DBNo.ValueT;
                if (resultValue == 0 && AggrNo.ValueT > 0)
                    resultValue = 888;
                return resultValue;
            }
            set
            {
                _DBNo.ValueT = value;
            }
        }

        private ACPropertyConfigValue<UInt16?> _DBOffset;
        [ACPropertyConfig("en{'Startaddress in DB'}de{'Startadresse im DB'}")]
        public UInt16? DBOffset
        {
            get
            {
                if (!_DBOffset.ValueT.HasValue)
                {
                    if (AggrNo != null && AggrNo.ValueT > 0)
                    {
                        if (AggrNo.ValueT == 1)
                            return 0;
                        else
                            return Convert.ToUInt16((AggrNo.ValueT - 1) * (GIPSerialControllerPID2006.StructLen + GIPSerialControllerPID2006.DBReservLen));
                    }
                    return null;
                }
                else
                    return _DBOffset.ValueT;
            }
            set
            {
                _DBOffset.ValueT = value;
            }
        }

        #endregion

        #region Read-Values from PLC
        [ACPropertyBindingTarget(500, "Read from PLC", "en{'Controller data'}de{'Regler Daten'}", "", false, true)]
        public IACContainerTNet<GIPControllerPIDData2006> CData { get; set; }
        #endregion

        #region internal
        public bool IsReadyForSending
        {
            get
            {
                if (!this.Root.Initialized)
                    return false;
                if (IsSimulationOn)
                    return true;
                bool isReadyForSending = false; 
                if (this.Session != null)
                {
                    isReadyForSending = true;
                    ACSession acSession = this.Session as ACSession;
                    if (acSession != null && !acSession.IsReadyForWriting)
                        isReadyForSending = false;
                    else if (acSession == null && !(bool)this.Session.ACUrlCommand("IsReadyForWriting"))
                        isReadyForSending = false;
                }
                return isReadyForSending;
            }
        }

        private bool _CDataLoaded = false;
        public bool CDataLoaded
        {
            get
            {
                return _CDataLoaded;
            }
        }

        private static object _PollLock = new object();
        private static List<ACRef<GIPControllerPID2006>> _ControllerToPoll = null;
        private static ManualResetEvent _ShutdownEvent = new ManualResetEvent(false);
        private static ACThread _PollThread;
        #endregion


        #endregion

        #region Methods, Range: 800
        [ACMethodInteraction("", "en{'Load controller data'}de{'Reglerdaten laden'}", 500, true, "", Global.ACKinds.MSMethod)]
        public virtual void LoadCData()
        {
            if (!IsEnabledLoadCData())
                return;

            GIPControllerPIDData2006 cData = new GIPControllerPIDData2006();
            cData = this.Session.ACUrlCommand("!ReadObject", cData, DBNo, DBOffset.Value, null) as GIPControllerPIDData2006;
            if (cData != null)
            {
                CData.ValueT = cData;
                _CDataLoaded = true;
            }
            else if (IsSimulationOn)
                _CDataLoaded = true;
            else
            {
                Messages.LogError(this.GetACUrl(), "LoadCData()", "ReadObject failed. Could not read GIPControllerPIDData2006");
            }

            if (CData.ValueT == null)
            {
                // TODO: Alarm?
            }
        }

        public virtual bool IsEnabledLoadCData()
        {
            return IsReadyForSending && DBNo > 0 && DBOffset.HasValue;
        }

        [ACMethodInteraction("", "en{'Write controller data'}de{'Reglerdaten schreiben'}", 501, true, "", Global.ACKinds.MSMethod)]
        public virtual void WriteCData()
        {
            if (!IsEnabledWriteCData())
                return;

            // TODO: Validate Range of Values
            object sended = this.Session.ACUrlCommand("!SendObject", CData.ValueT, null, DBNo, DBOffset.Value, null, null);
            if (sended == null || !((bool)sended))
            {
                // TODO: Alarm?
            }
        }

        public virtual bool IsEnabledWriteCData()
        {
            return IsReadyForSending 
                && DBNo > 0 
                && DBOffset.HasValue 
                && CData.ValueT != null 
                && _CDataLoaded;
        }

        private void RunPollCycle()
        {
            try
            {
                while (!_ShutdownEvent.WaitOne(500, false))
                {
                    _PollThread.StartReportingExeTime();
                    ACRef<GIPControllerPID2006> nextControllerToRead = null;
                    lock (_PollLock)
                    {
                        if (_ControllerToPoll != null && _ControllerToPoll.Any())
                            nextControllerToRead = _ControllerToPoll.FirstOrDefault();
                    }
                    if (nextControllerToRead != null)
                    {
                        bool remove = false;
                        if (!nextControllerToRead.IsAttached || nextControllerToRead.ValueT.Session == null) 
                            remove = true;
                        else if (nextControllerToRead.ValueT.IsReadyForSending)
                        {
                            if (nextControllerToRead.ValueT.IsEnabledLoadCData())
                            {
                                nextControllerToRead.ValueT.LoadCData();
                                if (nextControllerToRead.ValueT.CDataLoaded)
                                    remove = true;
                            }
                            // If AggNo ord DBOffset not set, remove it
                            else
                                remove = true;
                        }
                        if (remove)
                        {
                            lock (_PollLock)
                            {
                                nextControllerToRead.Detach();
                                _ControllerToPoll.Remove(nextControllerToRead);
                                if (!_ControllerToPoll.Any())
                                {
                                    ThreadPool.QueueUserWorkItem((object state) => { StopPollThread(); });
                                }
                            }
                        }
                    }
                    _PollThread.StopReportingExeTime();
                }
            }
            catch (ThreadAbortException e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("GIPControllerPID2006", "RunPollCycle", msg);
            }
        }
        #endregion

        #region Diagnose
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList, ref DumpStats dumpStats)
        {
            base.DumpPropertyList(doc, xmlACPropertyList, ref dumpStats);

            XmlElement xmlChild = xmlACPropertyList["CDataLoaded"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("CDataLoaded");
                if (xmlChild != null)
                    xmlChild.InnerText = CDataLoaded.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

            xmlChild = xmlACPropertyList["IsReadyForSending"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("IsReadyForSending");
                if (xmlChild != null)
                    xmlChild.InnerText = IsReadyForSending.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

        }
        #endregion
    }
    #endregion
}
