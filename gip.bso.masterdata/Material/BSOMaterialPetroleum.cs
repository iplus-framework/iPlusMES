// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOMaterial.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
//using gip.core.manager;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Globalization;

namespace gip.bso.masterdata
{
    public partial class BSOMaterial : BSOMaterialExplorer
    {
        #region BSO->ACProperty

        #region Conversion-Test 1
        private double _ConvertTestAmbVol;
        [ACPropertyInfo(9999, "Petroleum", "en{'Ambient Volume [l]'}de{'Volumen Ambient [l]'}")]
        public double ConvertTestAmbVol
        {
            get
            {
                return _ConvertTestAmbVol;
            }
            set
            {
                _ConvertTestAmbVol = value;
                OnPropertyChanged("ConvertTestAmbVol");
            }
        }

        private double _ConvertTestRef15Vol;
        [ACPropertyInfo(9999, "Petroleum", "en{'Standard Volume [l]'}de{'Standard Volumen [l]'}")]
        public double ConvertTestRef15Vol
        {
            get
            {
                return _ConvertTestRef15Vol;
            }
            set
            {
                _ConvertTestRef15Vol = value;
                OnPropertyChanged("ConvertTestRef15Vol");
            }
        }

        private double _ConvertTestMass;
        [ACPropertyInfo(9999, "Petroleum", "en{'Weight in Air [kg]'}de{'Gewicht an der Luft [kg]'}")]
        public double ConvertTestMass
        {
            get
            {
                return _ConvertTestMass;
            }
            set
            {
                _ConvertTestMass = value;
                OnPropertyChanged("ConvertTestMass");
            }
        }

        private double _ConvertTestMassVac;
        [ACPropertyInfo(9999, "Petroleum", "en{'Weight in Vacuum [kg]'}de{'Gewicht im Vakuum [kg]'}")]
        public double ConvertTestMassVac
        {
            get
            {
                return _ConvertTestMassVac;
            }
            set
            {
                _ConvertTestMassVac = value;
                OnPropertyChanged("ConvertTestMassVac");
            }
        }

        private Nullable<double> _AmbientTemperature;
        [ACPropertyInfo(9999, "Petroleum", "en{'Ambient temperature [°C]'}de{'Temperatur Ambient [°C]'}")]
        public Nullable<double> AmbientTemperature
        {
            get
            {
                return _AmbientTemperature;
            }
            set
            {
                _AmbientTemperature = value;
                OnPropertyChanged("AmbientTemperature");
            }
        }

        private Nullable<double> _ConvertTestDensity;
        [ACPropertyInfo(9999, "Petroleum", "en{'Standard density [kg/dm³]'}de{'Normdichte [kg/dm³]'}")]
        public Nullable<double> ConvertTestDensity
        {
            get
            {
                return _ConvertTestDensity;
            }
            set
            {
                _ConvertTestDensity = value;
                OnPropertyChanged("ConvertTestDensity");
            }
        }

        private bool? _CalcWithASTM = true;
        [ACPropertyInfo(9999, "Petroleum", "en{'Calculation over ASTM 1250'}de{'Umrechnung nach ASTM 1250'}")]
        public bool? CalcWithASTM
        {
            get
            {
                return _CalcWithASTM;
            }
            set
            {
                _CalcWithASTM = value;
                OnPropertyChanged("CalcWithASTM");
            }
        }

        #endregion

        #region Conversion-Test 2
        private Nullable<double> _ConvertTest2AmbVol;
        [ACPropertyInfo(9999, "Petroleum", "en{'Ambient Volume [l]'}de{'Volumen Ambient [l]'}")]
        public Nullable<double> ConvertTest2AmbVol
        {
            get
            {
                return _ConvertTest2AmbVol;
            }
            set
            {
                _ConvertTest2AmbVol = value;
                OnPropertyChanged("ConvertTest2AmbVol");
            }
        }

        private Nullable<double> _ConvertTest2Ref15Vol;
        [ACPropertyInfo(9999, "Petroleum", "en{'Standard Volume [l]'}de{'Standard Volumen [l]'}")]
        public Nullable<double> ConvertTest2Ref15Vol
        {
            get
            {
                return _ConvertTest2Ref15Vol;
            }
            set
            {
                _ConvertTest2Ref15Vol = value;
                OnPropertyChanged("ConvertTest2Ref15Vol");
            }
        }

        private Nullable<double> _ConvertTest2Mass;
        [ACPropertyInfo(9999, "Petroleum", "en{'Weight in Air [kg]'}de{'Gewicht an der Luft [kg]'}")]
        public Nullable<double> ConvertTest2Mass
        {
            get
            {
                return _ConvertTest2Mass;
            }
            set
            {
                _ConvertTest2Mass = value;
                OnPropertyChanged("ConvertTest2Mass");
            }
        }

        private double _ConvertTest2MassVac;
        [ACPropertyInfo(9999, "Petroleum", "en{'Weight in Vacuum [kg]'}de{'Geiwcht im Vakuum [kg]'}")]
        public double ConvertTest2MassVac
        {
            get
            {
                return _ConvertTest2MassVac;
            }
            set
            {
                _ConvertTest2MassVac = value;
                OnPropertyChanged("ConvertTest2MassVac");
            }
        }

        private double _AmbientTemperature2;
        [ACPropertyInfo(9999, "Petroleum", "en{'Ambient Temperature [°C]'}de{'Temperatur Ambient [°C]'}")]
        public double AmbientTemperature2
        {
            get
            {
                return _AmbientTemperature2;
            }
            set
            {
                _AmbientTemperature2 = value;
                OnPropertyChanged("AmbientTemperature2");
            }
        }

        private double _ConvertTest2Density;
        [ACPropertyInfo(9999, "Petroleum", "en{'Standard Density [kg/dm³]'}de{'Standard Dichte [kg/dm³]'}")]
        public double ConvertTest2Density
        {
            get
            {
                return _ConvertTest2Density;
            }
            set
            {
                _ConvertTest2Density = value;
                OnPropertyChanged("ConvertTest2Density");
            }
        }

        private double _ConvertTest2DensityAmb;
        [ACPropertyInfo(9999, "Petroleum", "en{'Density ambient [kg/dm³]'}de{'Dichte Ambient [kg/dm³]'}")]
        public double ConvertTest2DensityAmb
        {
            get
            {
                return _ConvertTest2DensityAmb;
            }
            set
            {
                _ConvertTest2DensityAmb = value;
                OnPropertyChanged("ConvertTest2DensityAmb");
            }
        }
        #endregion

        #region Conversion-Test 3
        private double? _AmbientTemperature3;
        [ACPropertyInfo(9999, "Petroleum", "en{'Ambient Temperature [°C]'}de{'Temperatur Ambient [°C]'}")]
        public double? AmbientTemperature3
        {
            get
            {
                return _AmbientTemperature3;
            }
            set
            {
                _AmbientTemperature3 = value;
                OnPropertyChanged("AmbientTemperature3");
            }
        }

        private double? _ConvertTestRef15Vol3;
        [ACPropertyInfo(9999, "Petroleum", "en{'Standard Volume [l]'}de{'Standard Volumen [l]'}")]
        public double? ConvertTestRef15Vol3
        {
            get
            {
                return _ConvertTestRef15Vol3;
            }
            set
            {
                _ConvertTestRef15Vol3 = value;
                OnPropertyChanged("ConvertTestRef15Vol3");
            }
        }

        private double? _ConvertTestMass3;
        [ACPropertyInfo(9999, "Petroleum", "en{'Weight in Air [kg]'}de{'Gewicht an der Luft [kg]'}")]
        public double? ConvertTestMass3
        {
            get
            {
                return _ConvertTestMass3;
            }
            set
            {
                _ConvertTestMass3 = value;
                OnPropertyChanged("ConvertTestMass3");
            }
        }

        private double _ConvertTest3Density;
        [ACPropertyInfo(9999, "Petroleum", "en{'Standard Density [kg/dm³]'}de{'Standard Dichte [kg/dm³]'}")]
        public double ConvertTest3Density
        {
            get
            {
                return _ConvertTest3Density;
            }
            set
            {
                _ConvertTest3Density = value;
                OnPropertyChanged("ConvertTest3Density");
            }
        }

        private double _ConvertTest3AmbVol;
        [ACPropertyInfo(9999, "Petroleum", "en{'Ambient Volume [l]'}de{'Volumen Ambient [l]'}")]
        public double ConvertTest3AmbVol
        {
            get
            {
                return _ConvertTest3AmbVol;
            }
            set
            {
                _ConvertTest3AmbVol = value;
                OnPropertyChanged("ConvertTest3AmbVol");
            }
        }
        #endregion

        #region Methods

        [ACMethodInfo("", "en{'Validate Input'}de{'Überprüfe Eingabe'}", 9999, false)]
        public Msg ValidateInput(string vbContent, object value, CultureInfo cultureInfo)
        {
            //if (value == null)
            //{
            //    return new Msg() { MessageLevel = eMsgLevel.Info };
            //}
            //else if (value is String)
            //{
            //    if (vbContent == "ConvertTestDensity")
            //    {
            //        String strValue = value as String;
            //        if (String.IsNullOrEmpty(strValue))
            //        {
            //            ConvertTestDensity = 0;
            //        }
            //    }
            //}
            return new Msg() { MessageLevel = eMsgLevel.Info };
        }


        #region Conversion 1
        private void RefreshFields1()
        {
            OnPropertyChanged("ConvertTestAmbVol");
            OnPropertyChanged("ConvertTestRef15Vol");
            OnPropertyChanged("ConvertTestMass");
            OnPropertyChanged("ConvertTestMassVac");
            OnPropertyChanged("AmbientTemperature");
            OnPropertyChanged("ConvertTestDensity");
        }

        private short _InvocationLock1 = 0;
        [ACMethodCommand("Petroleum", "en{'Volume to 15°C'}de{'Volumen auf 15°C'}", 9999)]
        public void ConvertAmbientVolToRefVol15()
        {
            if (!IsEnabledConvertAmbientVolToRefVol15())
                return;
            if (_InvocationLock1 == 0)
                _InvocationLock1 = 1;
            RefreshFields1();
            try
            {
                ConvertTestRef15Vol = CurrentMaterial.ConvertAmbientVolToRefVol15(ConvertTestAmbVol, AmbientTemperature.Value, ConvertTestDensity, CalcWithASTM);
                //Double Den15 = ConvertTestDensity.Value * 1000;
                //Double DegC = AmbientTemperature.Value;
                //double Vcf_c = ConvertTestAmbVol; 
                //double vcf_p = 0;
                //long iFlag = 0;
                //_VB_Table_54B(Den15, DegC, ref Vcf_c, ref vcf_p, ref iFlag);
            }
            catch (Exception e)
            {
                Messages.Msg(new Msg() { Message = e.Message });
                ConvertTestRef15Vol = 0;
            }
            if (_InvocationLock1 == 1)
            {
                ConvertAmbVolToMass();
                _InvocationLock1 = 0;
            }
        }
        public bool IsEnabledConvertAmbientVolToRefVol15()
        {
            if (!AmbientTemperature.HasValue || !ConvertTestDensity.HasValue)
                return false;
            return true;
        }

        [ACMethodCommand("Petroleum", "en{'Volume to Ambient'}de{'Volumen auf Umgebung'}", 9999)]
        public void ConvertRefVol15ToAmbientVol()
        {
            if (!IsEnabledConvertRefVol15ToAmbientVol())
                return;
            if (_InvocationLock1 == 0)
                _InvocationLock1 = 2;

            RefreshFields1();
            try
            {
                ConvertTestAmbVol = CurrentMaterial.ConvertRefVol15ToAmbientVol(ConvertTestRef15Vol, AmbientTemperature.Value, ConvertTestDensity, CalcWithASTM);
            }
            catch (Exception e)
            {
                Messages.Msg(new Msg() { Message = e.Message });
                ConvertTestAmbVol = 0;
            }
            if (_InvocationLock1 == 2)
            {
                ConvertAmbVolToMass();
                _InvocationLock1 = 0;
            }
        }
        public bool IsEnabledConvertRefVol15ToAmbientVol()
        {
            if (!AmbientTemperature.HasValue || !ConvertTestDensity.HasValue)
                return false;
            return true;
        }

        [ACMethodCommand("Petroleum", "en{'Volume to mass'}de{'Volumen nach Masse'}", 9999)]
        public void ConvertAmbVolToMass()
        {
            if (!IsEnabledConvertAmbVolToMass())
                return;
            if (_InvocationLock1 == 0)
                _InvocationLock1 = 3;
            RefreshFields1();
            try
            {
                ConvertTestMass = CurrentMaterial.ConvertAmbVolToMass(ConvertTestAmbVol, AmbientTemperature.Value, ConvertTestDensity);
                ConvertTestMassVac = CurrentMaterial.ConvertAmbVolToMassVac(ConvertTestAmbVol, AmbientTemperature.Value, ConvertTestDensity);
            }
            catch (Exception e)
            {
                Messages.Msg(new Msg() { Message = e.Message });
                ConvertTestMass = 0;
            }
            if (_InvocationLock1 == 3)
            {
                ConvertAmbientVolToRefVol15();
                _InvocationLock1 = 0;
            }
        }
        public bool IsEnabledConvertAmbVolToMass()
        {
            if (!AmbientTemperature.HasValue || !ConvertTestDensity.HasValue)
                return false;
            return true;
        }

        [ACMethodCommand("Petroleum", "en{'Mass to volume'}de{'Masse nach Volumen'}", 9999)]
        public void ConvertMassToAmbVol()
        {
            if (!IsEnabledConvertMassToAmbVol())
                return;
            if (_InvocationLock1 == 0)
                _InvocationLock1 = 4;
            RefreshFields1();
            try
            {
                if (Math.Abs(ConvertTestMass) > Double.Epsilon)
                {
                    ConvertTestAmbVol = CurrentMaterial.ConvertMassToAmbVol(ConvertTestMass, AmbientTemperature.Value, ConvertTestDensity);
                }
                else if (Math.Abs(ConvertTestMassVac) > Double.Epsilon)
                {
                    ConvertTestAmbVol = CurrentMaterial.ConvertMassVacToAmbVol(ConvertTestMassVac, AmbientTemperature.Value, ConvertTestDensity);
                }
            }
            catch (Exception e)
            {
                Messages.Msg(new Msg() { Message = e.Message });
                ConvertTestAmbVol = 0;
            }
            if (_InvocationLock1 == 4)
            {
                ConvertAmbientVolToRefVol15();
                ConvertAmbVolToMass();
                _InvocationLock1 = 0;
            }
        }

        public bool IsEnabledConvertMassToAmbVol()
        {
            if (!AmbientTemperature.HasValue || !ConvertTestDensity.HasValue)
                return false;
            return true;
        }
        #endregion

        #region Conversion 2
        private void RefreshFields2()
        {
            OnPropertyChanged("ConvertTest2AmbVol");
            OnPropertyChanged("ConvertTest2Ref15Vol");
            OnPropertyChanged("ConvertTest2Mass");
            OnPropertyChanged("ConvertTest2MassVac");
            OnPropertyChanged("AmbientTemperature2");
            OnPropertyChanged("ConvertTest2Density");
            OnPropertyChanged("ConvertTest2DensityAmb");
        }

        [ACMethodCommand("Petroleum", "en{'Volume to 15°C'}de{'Volumen auf 15°C'}", 9999)]
        public void CalcDensityAndTemp()
        {
            if (!IsEnabledCalcDensityAndTemp())
                return;
            try
            {
                RefreshFields2();
                ConvertTest2Density = Material.CalcDensity15(ConvertTest2AmbVol.Value, ConvertTest2Ref15Vol.Value, ConvertTest2Mass.Value);
                double vFactor = ConvertTest2Ref15Vol.Value / ConvertTest2AmbVol.Value;
                AmbientTemperature2 = Material.CalcTemperature(vFactor, ConvertTest2Density, 7);
                ConvertTest2DensityAmb = Material.CalcDensityAmbient(ConvertTest2Density * 1000, AmbientTemperature2, 0);
                //Double Den15 = ConvertTestDensity.Value * 1000;
                //Double DegC = AmbientTemperature.Value;
                //double Vcf_c = ConvertTestAmbVol; 
                //double vcf_p = 0;
                //long iFlag = 0;
                //_VB_Table_54B(Den15, DegC, ref Vcf_c, ref vcf_p, ref iFlag);
            }
            catch (Exception e)
            {
                Messages.Msg(new Msg() { Message = e.Message });
                ConvertTestRef15Vol = 0;
            }
        }
        public bool IsEnabledCalcDensityAndTemp()
        {
            if (!ConvertTest2AmbVol.HasValue || !ConvertTest2Ref15Vol.HasValue || !ConvertTest2Mass.HasValue)
                return false;
            return true;
        }
        #endregion

        #region Conversion 3
        private void RefreshFields3()
        {
            OnPropertyChanged("AmbientTemperature3");
            OnPropertyChanged("ConvertTestRef15Vol3");
            OnPropertyChanged("ConvertTestMass3");
            OnPropertyChanged("ConvertTest3Density");
            OnPropertyChanged("ConvertTest3AmbVol");
        }

        [ACMethodCommand("Petroleum", "en{'Volume to 15°C'}de{'Volumen auf 15°C'}", 9999)]
        public void CalcDensityAndVol()
        {
            if (!IsEnabledCalcDensityAndVol())
                return;
            try
            {
                RefreshFields3();

                ConvertTest3Density = (ConvertTestMass3.Value / ConvertTestRef15Vol3.Value) + Material.DensityInAirOffset;
                ConvertTest3AmbVol = CurrentMaterial.ConvertRefVol15ToAmbientVol(ConvertTestRef15Vol3.Value, AmbientTemperature3.Value, ConvertTest3Density, CalcWithASTM);
            }
            catch (Exception e)
            {
                Messages.Msg(new Msg() { Message = e.Message });
                ConvertTest3Density = 0;
                ConvertTest3AmbVol = 0;
            }
        }
        public bool IsEnabledCalcDensityAndVol()
        {
            if (!AmbientTemperature3.HasValue || !ConvertTestRef15Vol3.HasValue || !ConvertTestMass3.HasValue)
                return false;
            if (ConvertTestRef15Vol3.Value == 0 || ConvertTestMass3.Value == 0)
                return false;
            return true;
        }
        #endregion

        //[DllImport("pmsvb32.dll",EntryPoint="#12",CallingConvention=CallingConvention.StdCall)]
        //public static extern void _VB_Table_54B(Double Den15, Double DegC, ref double Vcf_c, ref double vcf_p, ref long iFlag);

        #endregion

        #endregion
    }
}
