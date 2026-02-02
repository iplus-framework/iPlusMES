// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Data.SqlClient;
using gip.mes.processapplication;

namespace gip2006.variobatch.processapplication
{
    /// <summary>
    /// Baseclass for query old Variobatch-Database
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006QueryVB'}de{'GIPConv2006QueryVB'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, true)]
    public class GIPConv2006QueryVB : ACComponent
    {
        #region c'tors
        public GIPConv2006QueryVB(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
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
            ApplicationManager objectManager = FindParentComponent<ApplicationManager>(c => c is ApplicationManager);
            if (objectManager != null)
            {
                objectManager.ProjectWorkCycleR10sec += ReadFromDB;
            }

            return base.ACPostInit();
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            ApplicationManager objectManager = FindParentComponent<ApplicationManager>(c => c is ApplicationManager);
            if (objectManager != null)
            {
                objectManager.ProjectWorkCycleR10sec -= ReadFromDB;
            }
            return await base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties

        #region Binding Properties
        [ACPropertyInfo(9999)]
        public string ConnectionString
        {
            get;
            set;
        }
        #endregion

        #endregion

        #region Methods
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "ExecuteQuery":
                    result = ExecuteQuery();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }


        #region Private

        private int _ReconnectCounter = 0;
        void ReadFromDB(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ConnectionString))
                return;
            if (_ReconnectCounter > 0)
            {
                _ReconnectCounter--;
                return;
            }

            try
            {
                if (!(bool)ACUrlCommand("!ExecuteQuery"))
                {
                    _ReconnectCounter = 10;
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("GIPConv2006QueryVB", "ReadFromDB", msg);

                _ReconnectCounter = 10;
            }
        }

        [ACMethodInfo("", "en{'Excute query'}de{'Abfrage ausführen'}", 100, false)]
        public virtual bool ExecuteQuery()
        {
            ApplicationManager objectManager = FindParentComponent<ApplicationManager>(c => c is ApplicationManager);
            if (objectManager == null)
                return false;

            SqlConnection con = new SqlConnection(ConnectionString);

            string strSQL = "SELECT g.Nr, g.Name, g.Bem, g.RezeptNr, g.IsMapped, c.AuftragNr, c.LfdProduktionNr, c.ChargenGroesseSoll, r.Name as rName " +
                    "FROM dbo.SystemAggregatGruppe g " +
                    "LEFT OUTER JOIN dbo.Charge c ON g.RezeptNr = c.ChargenNr " +
                    "LEFT OUTER JOIN dbo.Rezepte r ON c.RezeptNr = r.Nr ";
            SqlCommand cmdAGroup = new SqlCommand(strSQL, con);

            strSQL = "SELECT l.Nr, l.Name, l.MaxInhaltKg, l.ArtikelNr, l.Bestand, l.FreigabeProduktion, l.FreigabeAnnahme, l.ReservKey2, a.Bez1 " +
                    "FROM dbo.Lagerplatz l " +
                    "LEFT OUTER JOIN dbo.Artikel a ON l.ArtikelNr = a.Nr " +
                    "WHERE l.Typ = 1";
            SqlCommand cmdLagerplatz = new SqlCommand(strSQL, con);

            try
            {
                con.Open();

                List<PAProcessModule> modules = objectManager.FindChildComponents<PAProcessModule>(c => c is PAProcessModule);
                if (modules.Any())
                {
                    SqlDataReader readerAGroup = cmdAGroup.ExecuteReader();
                    while (readerAGroup.Read())
                    {
                        string keyACIdentifier = readerAGroup["Bem"] as String;

                        if (String.IsNullOrEmpty(keyACIdentifier))
                            continue;
                        PAProcessModule unit = modules.Where(c => c.ACIdentifier == keyACIdentifier).FirstOrDefault();
                        if (unit == null)
                            continue;
                        int fieldId = readerAGroup.GetOrdinal("IsMapped");
                        Boolean isMapped = readerAGroup.IsDBNull(fieldId) ? false : System.Convert.ToBoolean(readerAGroup["IsMapped"]);
                        if (!isMapped)
                        {
                            unit.OrderInfo.ValueT = "";
                        }
                        else
                        {
                            unit.OrderInfo.ValueT = String.Format("{0}\n{1}\n{2}", readerAGroup["rName"], readerAGroup["AuftragNr"], readerAGroup["LfdProduktionNr"]);
                        }
                    }
                    readerAGroup.Close();
                }

                List<PAMSilo> silos = objectManager.FindChildComponents<PAMSilo>(c => c is PAMSilo);
                if (silos.Any())
                {
                    SqlDataReader readerLagerplatz = cmdLagerplatz.ExecuteReader();
                    while (readerLagerplatz.Read())
                    {
                        string keyACIdentifier = readerLagerplatz["Nr"] as String;
                        if (String.IsNullOrEmpty(keyACIdentifier))
                            continue;
                        PAMSilo unit = silos.Where(c => c.ACIdentifier == keyACIdentifier).FirstOrDefault();
                        if (unit == null)
                        {
                            keyACIdentifier = readerLagerplatz["ReservKey2"] as String;
                            if (String.IsNullOrEmpty(keyACIdentifier))
                                continue;
                            unit = silos.Where(c => c.ACIdentifier == keyACIdentifier).FirstOrDefault();
                            if (unit == null)
                                continue;
                        }
                        int fieldId = readerLagerplatz.GetOrdinal("Bestand");
                        double bestand = readerLagerplatz.IsDBNull(fieldId) ? 0 : System.Convert.ToDouble(readerLagerplatz["Bestand"]);
                        //if (bestand != 0)
                        //bestand *= 0.001; // to
                        unit.FillLevel.ValueT = bestand;

                        fieldId = readerLagerplatz.GetOrdinal("MaxInhaltKg");
                        double maxInhaltKg = readerLagerplatz.IsDBNull(fieldId) ? 0 : System.Convert.ToDouble(readerLagerplatz["MaxInhaltKg"]);
                        if (maxInhaltKg > 0.001)
                            unit.MaxWeightCapacity.ValueT = maxInhaltKg;

                        unit.MaterialName.ValueT = readerLagerplatz["Bez1"] as String;

                        fieldId = readerLagerplatz.GetOrdinal("FreigabeAnnahme");
                        Boolean freigabe = readerLagerplatz.IsDBNull(fieldId) ? false : System.Convert.ToBoolean(readerLagerplatz["FreigabeAnnahme"]);
                        unit.InwardEnabled.ValueT = freigabe;

                        fieldId = readerLagerplatz.GetOrdinal("FreigabeProduktion");
                        freigabe = readerLagerplatz.IsDBNull(fieldId) ? false : System.Convert.ToBoolean(readerLagerplatz["FreigabeProduktion"]);
                        unit.OutwardEnabled.ValueT = freigabe;
                    }
                    readerLagerplatz.Close();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Messages.LogDebug(this.GetACUrl(), "GIPConv2006QueryVB.ExecuteQuery()", ConnectionString + ": " + ex.Message + " " + ex.InnerException.Message);
                else
                    Messages.LogDebug(this.GetACUrl(), "GIPConv2006QueryVB.ExecuteQuery()", ConnectionString + ": " + ex.Message);
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
                return false;
            }
            return true;
        }
        #endregion

        #region Public
        #endregion

        #region Protected
        #endregion
        #endregion
    }
}
