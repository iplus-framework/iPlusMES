// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 12-14-2012
// ***********************************************************************
// <copyright file="Material.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using gip.core.datamodel;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    public partial class Material : IACConfigStore
    {
        [NotMapped]
        public const double DensityInAirOffset = 0.0011; // = Dichte in Trockenluft 1.092 / 1000;
        /// <summary>
        /// Gets or sets the Dimension
        /// </summary>
        /// <value>Dimension</value>
        [NotMapped]
        public GlobalApp.PetroleumGroups PetroleumGroup
        {
            get
            {
                return (GlobalApp.PetroleumGroups)PetroleumGroupIndex;
            }
            set
            {
                PetroleumGroupIndex = (Int16)value;
            }
        }

        [NotMapped]
        private static Nullable<bool> _CalcWithACTMTables;
        [NotMapped]
        public static bool CalcWithACTMTables
        {
            get
            {
                if (_CalcWithACTMTables.HasValue)
                    return _CalcWithACTMTables.Value;
                string MainDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                _CalcWithACTMTables = File.Exists(MainDir + "\\pmsvb32.dll");
                return _CalcWithACTMTables.Value;
            }
        }

        /// <summary>
        /// Converts a volume at ambient temperature to the reference Volume at 15°C for this material
        /// </summary>
        /// <param name="ambientVolume">in liter (dm³)</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns></returns>
        public Double ConvertAmbientVolToRefVol15(Double ambientVolume, Double ambientTemperature, double? density, bool? withASTMTables = null)
        {
            double calcDensity = density.HasValue ? density.Value : this.Density;
            CheckPetroleumProperties(calcDensity);
            return ConvertAmbientVolToRefVol15(this.PetroleumGroup, calcDensity, ambientVolume, ambientTemperature, withASTMTables);
        }

        /// <summary>
        /// Converts the reference Volume at 15°C to the volume at ambient temperature for this material
        /// </summary>
        /// <param name="ref15Volume">in liter (dm³)</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns></returns>
        public Double ConvertRefVol15ToAmbientVol(Double ref15Volume, Double ambientTemperature, double? density, bool? withASTMTables = null)
        {
            double calcDensity = density.HasValue ? density.Value : this.Density;
            CheckPetroleumProperties(calcDensity);
            return ConvertRefVol15ToAmbientVol(this.PetroleumGroup, calcDensity, ref15Volume, ambientTemperature, withASTMTables);
        }

        /// <summary>
        /// Converts a volume at ambient temperature to mass for this material
        /// </summary>
        /// <param name="ambientVolume">in liter (dm³)</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns></returns>
        public Double ConvertAmbVolToMass(Double ambientVolume, Double ambientTemperature, double? density)
        {
            double calcDensity = density.HasValue ? density.Value : this.Density;
            CheckPetroleumProperties(calcDensity);
            return ConvertAmbVolToMass(this.PetroleumGroup, calcDensity, ambientVolume, ambientTemperature, true);
        }

        /// <summary>
        /// Converts a mass to a volume at ambient temperature for this material
        /// </summary>
        /// <param name="ambientMass">in kg</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns></returns>
        public Double ConvertMassToAmbVol(Double ambientMass, Double ambientTemperature, double? density)
        {
            double calcDensity = density.HasValue ? density.Value : this.Density;
            CheckPetroleumProperties(calcDensity);
            return ConvertMassToAmbVol(this.PetroleumGroup, calcDensity, ambientMass, ambientTemperature, true);
        }

        /// <summary>
        /// Converts a volume at ambient temperature to MassVac for this material
        /// </summary>
        /// <param name="ambientVolume">in liter (dm³)</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns></returns>
        public Double ConvertAmbVolToMassVac(Double ambientVolume, Double ambientTemperature, double? density)
        {
            double calcDensity = density.HasValue ? density.Value : this.Density;
            CheckPetroleumProperties(calcDensity);
            return ConvertAmbVolToMass(this.PetroleumGroup, calcDensity, ambientVolume, ambientTemperature, false);
        }

        /// <summary>
        /// Converts a MassVac to a volume at ambient temperature for this material
        /// </summary>
        /// <param name="ambientMassVac">in kg</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns></returns>
        public Double ConvertMassVacToAmbVol(Double ambientMassVac, Double ambientTemperature, double? density)
        {
            double calcDensity = density.HasValue ? density.Value : this.Density;
            CheckPetroleumProperties(calcDensity);
            return ConvertMassToAmbVol(this.PetroleumGroup, calcDensity, ambientMassVac, ambientTemperature, false);
        }

        /// <summary>
        /// Converts a volume at ambient temperature to the reference Volume at 15°C
        /// </summary>
        /// <param name="petroleumGroup">PetroleumGroups</param>
        /// <param name="density15">in kg/dm³</param>
        /// <param name="ambientVolume">in liter (dm³)</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns>Volume in liter at 15°C</returns>
        public static Double ConvertAmbientVolToRefVol15(GlobalApp.PetroleumGroups petroleumGroup, Double density15, Double ambientVolume, Double ambientTemperature, bool? withASTMTables = null)
        {
            CheckPetroleumProperties(petroleumGroup, density15, ambientTemperature);
            if (Math.Abs(ambientVolume - 0) <= Double.Epsilon)
                return ambientVolume;

            bool bASTM = false;
            if (withASTMTables.HasValue)
                bASTM = withASTMTables.Value;
            else
                bASTM = CalcWithACTMTables;

            density15 *= 1000;
            if (bASTM)
            {
                double VCFtemp = 0;  //Volume Correction factor to temperature
                double VCFpress = 0;  //Volume Correction factor to pressure
                long iFlag = 0;
                try
                {
                    _VB_Table_54B(density15, ambientTemperature, ref VCFtemp, ref VCFpress, ref iFlag);
                    //double densityAmb = density15 * VCFtemp;
                    double ref15Volume = ambientVolume * VCFtemp;
                    return ref15Volume;
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (Database.Root != null && Database.Root.Messages != null)
                        Database.Root.Messages.LogException(ClassName, "ConvertAmbientVolToRefVol15", msg);

                    return ambientVolume;
                }
            }
            else
            {
                if (petroleumGroup == GlobalApp.PetroleumGroups.A
                    || petroleumGroup == GlobalApp.PetroleumGroups.B1
                    || petroleumGroup == GlobalApp.PetroleumGroups.B2
                    || petroleumGroup == GlobalApp.PetroleumGroups.B3
                    || petroleumGroup == GlobalApp.PetroleumGroups.B4
                    || petroleumGroup == GlobalApp.PetroleumGroups.C
                    || petroleumGroup == GlobalApp.PetroleumGroups.D)
                {
                    double expCoeff = GetExpansionCoefficient(petroleumGroup, density15, ambientTemperature);
                    double deltaTemp = ambientTemperature - 15;
                    double lambda = (expCoeff * deltaTemp * (1 + expCoeff * 0.8 * deltaTemp)) * -1;
                    double ref15Volume = ambientVolume * Math.Exp(lambda);
                    return ref15Volume;
                }
                else
                {
                    double vcfP1, vcfP2, vcfP3, vcfP4;
                    double deltaTemp = ambientTemperature - 15;
                    GetVCF(petroleumGroup, out vcfP1, out vcfP2, out vcfP3, out vcfP4);
                    double ptp15 = 1 + ((((vcfP1 * -1) / density15) + vcfP2) * deltaTemp) + ((((vcfP3 * -1) / density15) + vcfP4) * Math.Pow(deltaTemp, 2));
                    double ref15Volume = ambientVolume * ptp15;
                    return ref15Volume;
                }
            }
        }

        /// <summary>
        /// Converts the reference Volume at 15°C to the volume at ambient temperature
        /// </summary>
        /// <param name="petroleumGroup">PetroleumGroups</param>
        /// <param name="density15">in kg/dm³</param>
        /// <param name="ambientVolume">in liter (dm³)</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns>Volume in liter at ambient temperature</returns>
        public static Double ConvertRefVol15ToAmbientVol(GlobalApp.PetroleumGroups petroleumGroup, Double density15, Double ref15Volume, Double ambientTemperature, bool? withASTMTables = null)
        {
            CheckPetroleumProperties(petroleumGroup, density15, ambientTemperature);
            if (Math.Abs(ref15Volume - 0) <= Double.Epsilon)
                return ref15Volume;

            bool bASTM = false;
            if (withASTMTables.HasValue)
                bASTM = withASTMTables.Value;
            else
                bASTM = CalcWithACTMTables;

            density15 *= 1000;
            if (bASTM)
            {
                double VCFtemp = 0;  //Volume Correction factor to temperature
                double VCFpress = 0;  //Volume Correction factor to pressure
                long iFlag = 0;
                try
                {
                    _VB_Table_54B(density15, ambientTemperature, ref VCFtemp, ref VCFpress, ref iFlag);
                    //double densityAmb = density15 * VCFtemp;
                    double ambientVolume = (double)(ref15Volume / VCFtemp);
                    return ambientVolume;
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (Database.Root != null && Database.Root.Messages != null)
                        Database.Root.Messages.LogException(ClassName, "ConvertRefVol15ToAmbientVol", msg);

                    return ref15Volume;
                }
            }
            else
            {
                if (petroleumGroup == GlobalApp.PetroleumGroups.A
                    || petroleumGroup == GlobalApp.PetroleumGroups.B1
                    || petroleumGroup == GlobalApp.PetroleumGroups.B2
                    || petroleumGroup == GlobalApp.PetroleumGroups.B3
                    || petroleumGroup == GlobalApp.PetroleumGroups.B4
                    || petroleumGroup == GlobalApp.PetroleumGroups.C
                    || petroleumGroup == GlobalApp.PetroleumGroups.D)
                {
                    double expCoeff = GetExpansionCoefficient(petroleumGroup, density15, ambientTemperature);
                    double deltaTemp = ambientTemperature - 15;
                    double lambda = (expCoeff * deltaTemp * (1 + expCoeff * 0.8 * deltaTemp)) * -1;
                    double ambientVolume = ref15Volume / Math.Exp(lambda);
                    return ambientVolume;
                }
                else
                {
                    double vcfP1, vcfP2, vcfP3, vcfP4;
                    double deltaTemp = ambientTemperature - 15;
                    GetVCF(petroleumGroup, out vcfP1, out vcfP2, out vcfP3, out vcfP4);
                    double ptp15 = 1 + ((((vcfP1 * -1) / density15) + vcfP2) * deltaTemp) + ((((vcfP3 * -1) / density15) + vcfP4) * Math.Pow(deltaTemp, 2));
                    double ambientVolume = ref15Volume / ptp15;
                    return ambientVolume;
                }
            }
        }

        /// <summary>
        /// Converts a volume at ambient temperature to mass
        /// </summary>
        /// <param name="petroleumGroup">PetroleumGroups</param>
        /// <param name="density15">in kg/dm³</param>
        /// <param name="ambientVolume">in liter (dm³)</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns></returns>
        public static Double ConvertAmbVolToMass(GlobalApp.PetroleumGroups petroleumGroup, Double density15, Double ambientVolume, Double ambientTemperature, bool inAir = false)
        {
            CheckPetroleumProperties(petroleumGroup, density15, ambientTemperature);
            if (Math.Abs(ambientVolume - 0) <= Double.Epsilon)
                return ambientVolume;

            double ref15Volume = ConvertAmbientVolToRefVol15(petroleumGroup, density15, ambientVolume, ambientTemperature);

            double densityVac = density15;
            //density15 += DensityInAirOffset;
            if (inAir)
                densityVac = density15 - DensityInAirOffset;
            double mass = ref15Volume * densityVac;
            return mass;
        }

        /// <summary>
        /// Converts a mass to a volume at ambient temperature 
        /// </summary>
        /// <param name="petroleumGroup">PetroleumGroups</param>
        /// <param name="density15">in kg/dm³</param>
        /// <param name="ref15Mass">Mass at 15°C in kg</param>
        /// <param name="ambientTemperature">in °C</param>
        /// <returns></returns>
        public static Double ConvertMassToAmbVol(GlobalApp.PetroleumGroups petroleumGroup, Double density15, Double ref15Mass, Double ambientTemperature, bool inAir = false)
        {
            CheckPetroleumProperties(petroleumGroup, density15, ambientTemperature);
            if (Math.Abs(ref15Mass - 0) <= Double.Epsilon)
                return ref15Mass;

            double densityVac = density15;
            //density15 += DensityInAirOffset;
            if (inAir)
                densityVac = density15 - DensityInAirOffset;

            double ref15Volume = ref15Mass / densityVac;
            double ambientVolume = ConvertRefVol15ToAmbientVol(petroleumGroup, density15, ref15Volume, ambientTemperature);
            return ambientVolume;
        }

        public static Double CalcDensity15(Double ambientVolume, Double ref15Volume, Double weightInAir)
        {
            Double facDichte = 0;
            Double facDichte15 = 0;
            Double gewVac = 0;
            Double astm15;
            Double astmAmb;

            if (Math.Abs(ambientVolume - 0) <= Double.Epsilon)
                return ambientVolume;
            if (Math.Abs(ref15Volume - 0) <= Double.Epsilon)
                return ref15Volume;
            if (Math.Abs(weightInAir - 0) <= Double.Epsilon)
                return weightInAir;

            facDichte = weightInAir / ambientVolume;
            facDichte15 = weightInAir / ref15Volume;

            gewVac = (DensityInAirOffset + facDichte) * ambientVolume;
            //GewVac = Math.Round(GewVac, 2);
            astm15 = gewVac / ref15Volume;
            //Astm15 = Math.Round(Astm15, 4);  
            astmAmb = gewVac / ambientVolume;
            return astm15;
        }

        public static Double CalcTemperature(double vFactor, Double density15, Int32 precision = 1)
        {
            Double mTemp = 0;
            Double VCFtemp = 0;
            Double VCFpress = 0;
            long iFlag = 0;
            vFactor = Math.Round(vFactor, precision);

            //Int32 noNachkomma;
            //Int32 iPos;
            //String mZw;
            Double mVonValue = 0;
            Double mBisValue = 0;
            Double mOldValue = 0;
            //Int32 i;

            //Korrektur der Dichte wenn in 1/1000 Dichte angegeben
            if (density15 < 100)
                density15 = density15 * 1000;

            //Errechne Nachkommastellen von vFactor

            //mZw = CStr(vFactor)
            //iPos = InStr(mZw, ".")
            //noNachkomma = Len(mZw) - iPos

            //mZw = "0."
            //For i = 1 To noNachkomma - 1
            //    mZw = mZw & "0"
            //Next i
            //mZw = mZw & "1"

            mTemp = -20;
            try
            {
                while (mTemp <= 50)
                {
                    //calFakt = iso54b(density15 * 1000, mTemp)

                    _VB_Table_54B(density15, mTemp, ref VCFtemp, ref VCFpress, ref iFlag);

                    if (Math.Abs(Math.Round(VCFtemp, precision) - vFactor) <= Double.Epsilon)
                    {
                        break;
                    }
                    else
                    {
                        //Liegt Wert zwischen altem und neuen Wert?
                        if (VCFtemp > mOldValue)
                        {
                            mVonValue = mOldValue;
                            mBisValue = VCFtemp;
                        }
                        else
                        {
                            mVonValue = VCFtemp;
                            mBisValue = mOldValue;
                        }
                        mOldValue = VCFtemp;
                        if (vFactor >= mVonValue && vFactor <= mBisValue && mVonValue != 0)
                            break;
                        else
                            mTemp = Math.Round(mTemp, 2) + 0.1;
                    }
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException(ClassName, "CalcTemperature", msg);
                return 0;
            }
            return Math.Round(mTemp, 1);
        }

        public static double CalcDensityAmbient(Double density15, Double ambientTemperature, double vFactor)
        {
            Double VCFtemp = 0;
            Double VCFpress = 0;
            long iFlag = 0;

            if (Math.Abs(density15 - 0) <= Double.Epsilon)
                return density15;
            if (Math.Abs(ambientTemperature - 0) <= Double.Epsilon)
                return ambientTemperature;
            //if (Math.Abs(vFactor - 0) <= Double.Epsilon)
            //    return vFactor;

            try
            {
                _VB_Table_54B(density15, ambientTemperature, ref VCFtemp, ref VCFpress, ref iFlag);
                double densityAmb = (double)((density15 * VCFtemp) / 1000);
                return densityAmb;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null)
                    Database.Root.Messages.LogException("MaterialPetroleum", "CalcDensityAmbient", msg);

                return 0;
            }
        }

        private void CheckPetroleumProperties()
        {
            CheckPetroleumProperties(this.Density);
        }

        private void CheckPetroleumProperties(Double density15)
        {
            if (this.PetroleumGroup == GlobalApp.PetroleumGroups.None)
                throw new Exception("Material is not a assigned zo a petroleum group");
            if (Math.Abs(density15 - 0) <= Double.Epsilon)
                throw new Exception("Density of Material is invalid");
        }

        private static void CheckPetroleumProperties(GlobalApp.PetroleumGroups petroleumGroup, Double density15, Double ambientTemperature)
        {
            if (petroleumGroup == GlobalApp.PetroleumGroups.None)
                throw new ArgumentException("GlobalApp.PetroleumGroups.None not allowed");
            if (Math.Abs(density15 - 0) <= Double.Epsilon)
                throw new Exception("Density ois invalid");
            if ((petroleumGroup == GlobalApp.PetroleumGroups.A && (density15 < 610.5 || density15 > 1075.0))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.B1 && (density15 < 600.0 || density15 > 770.5))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.B2 && (density15 < 770.5 || density15 > 787.6))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.B3 && (density15 < 778.6 || density15 > 838.6))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.B4 && (density15 < 838.6 || density15 > 1200.0))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.C && (density15 < 1 || density15 > 20000))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.D && (density15 < 838.6 || density15 > 1200.0))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XG1 && (density15 < 500 || density15 > 600))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XG2 && (density15 < 600 || density15 > 620))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XG3 && (density15 < 620 || density15 > 640))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XG4 && (density15 < 640 || density15 > 650))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XB1 && (density15 < 950 || density15 > 1000))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XB2 && (density15 < 1000 || density15 > 1100))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XB3 && (density15 < 1100 || density15 > 1200))
                )
            {
                throw new ArgumentOutOfRangeException("Density is out of range");
            }
            if ((petroleumGroup == GlobalApp.PetroleumGroups.A && (ambientTemperature < -18 || ambientTemperature > 150))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.B1 && (ambientTemperature < -18 || ambientTemperature > 95))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.B2 && (ambientTemperature < -18 || ambientTemperature > 125))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.B3 && (ambientTemperature < -18 || ambientTemperature > 150))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.B4 && (ambientTemperature < -18 || ambientTemperature > 150))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.C && (ambientTemperature < -18 || ambientTemperature > 150))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.D && (ambientTemperature < -20 || ambientTemperature > 170))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XG1 && (ambientTemperature < -50 || ambientTemperature > 50))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XG2 && (ambientTemperature < -50 || ambientTemperature > 50))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XG3 && (ambientTemperature < -50 || ambientTemperature > 50))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XG4 && (ambientTemperature < -50 || ambientTemperature > 50))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XB1 && (ambientTemperature < 0 || ambientTemperature > 250))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XB2 && (ambientTemperature < 0 || ambientTemperature > 250))
                 && (petroleumGroup == GlobalApp.PetroleumGroups.XB3 && (ambientTemperature < 0 || ambientTemperature > 250))
                )
            {
                throw new ArgumentOutOfRangeException("Tmeperature is out of range");
            }
        }

        private static double GetExpansionCoefficient(GlobalApp.PetroleumGroups petroleumGroup, Double density15, Double ambientTemperature)
        {
            if (petroleumGroup == GlobalApp.PetroleumGroups.A)
                return 613.9723 / Math.Pow(density15, 2);
            else if (petroleumGroup == GlobalApp.PetroleumGroups.B1)
                return (346.4228 / Math.Pow(density15, 2)) + (0.4388 / density15);
            else if (petroleumGroup == GlobalApp.PetroleumGroups.B2)
                return -0.00336312 + (2680.3206 / Math.Pow(density15, 2));
            else if (petroleumGroup == GlobalApp.PetroleumGroups.B3)
                return 594.5418 / Math.Pow(density15, 2);
            else if (petroleumGroup == GlobalApp.PetroleumGroups.B4)
                return (186.9696 / Math.Pow(density15, 2)) + (0.4862 / density15);
            else if (petroleumGroup == GlobalApp.PetroleumGroups.C && ambientTemperature < 95)
                return 0.955;
            else if (petroleumGroup == GlobalApp.PetroleumGroups.C && ambientTemperature < 125)
                return 0.919;
            else if (petroleumGroup == GlobalApp.PetroleumGroups.C && ambientTemperature < 150)
                return 0.486;
            return 0;
        }

        private static void GetVCF(GlobalApp.PetroleumGroups petroleumGroup, out double vcfP1, out double vcfP2, out double vcfP3, out double vcfP4)
        {
            switch (petroleumGroup)
            {
                case GlobalApp.PetroleumGroups.XG1:
                    vcfP1 = 4075.0 * 0.001;
                    vcfP2 = 5050.0 * 0.000001;
                    vcfP3 = 27.5 * 0.001;
                    vcfP4 = 45.0 * 0.000001;
                    break;
                case GlobalApp.PetroleumGroups.XG2:
                    vcfP1 = 2448.9 * 0.001;
                    vcfP2 = 2340.9 * 0.000001;
                    vcfP3 = 1.589 * 0.001;
                    vcfP4 = 1.947 * 0.000001;
                    break;
                case GlobalApp.PetroleumGroups.XG3:
                    vcfP1 = 2225.1 * 0.001;
                    vcfP2 = 1980.0 * 0.000001;
                    vcfP3 = 1.588 * 0.001;
                    vcfP4 = 1.946 * 0.000001;
                    break;
                case GlobalApp.PetroleumGroups.XG4:
                    vcfP1 = 1936.6 * 0.001;
                    vcfP2 = 1529.1 * 0.000001;
                    vcfP3 = 1.588 * 0.001;
                    vcfP4 = 1.946 * 0.000001;
                    break;
                case GlobalApp.PetroleumGroups.XB1:
                    vcfP1 = 708.2 * 0.001;
                    vcfP2 = 51.8 * 0.000001;
                    vcfP3 = 1.587 * 0.001;
                    vcfP4 = 1.944 * 0.000001;
                    break;
                case GlobalApp.PetroleumGroups.XB2:
                    vcfP1 = 984.2 * 0.001;
                    vcfP2 = 328.0 * 0.000001;
                    vcfP3 = -7.481 * 0.001;
                    vcfP4 = -7.129 * 0.000001;
                    break;
                case GlobalApp.PetroleumGroups.XB3:
                    vcfP1 = 890.0 * 0.001;
                    vcfP2 = 242.3 * 0.000001;
                    vcfP3 = -7.830 * 0.001;
                    vcfP4 = -7.453 * 0.000001;
                    break;
                default:
                    vcfP1 = 0;
                    vcfP2 = 0;
                    vcfP3 = 0;
                    vcfP4 = 0;
                    break;
            }
        }

        [DllImport("pmsvb32.dll", EntryPoint = "#12", CallingConvention = CallingConvention.StdCall)]
        public static extern void _VB_Table_54B(Double Den15, Double DegC, ref double Vcf_c, ref double vcf_p, ref long iFlag);

    }
}
