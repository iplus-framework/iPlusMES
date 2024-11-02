// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOVisitorVoucher.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace gip.bso.logistics
{
    public partial class BSOPicking : ACBSOvbNav
    {
        #region Properties

        #region Weighing
        private core.datamodel.ACClass _SelectedScale;
        [ACPropertySelected(611, "Scales", "en{'Vehicle scale'}de{'Fahrzeugwaage'}")]
        public core.datamodel.ACClass SelectedScale
        {
            get
            {
                return _SelectedScale;
            }
            set
            {
                if (_SelectedScale != value)
                {
                    _SelectedScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<core.datamodel.ACClass> _ScalesList;
        [ACPropertyList(610, "Scales")]
        public List<core.datamodel.ACClass> ScalesList
        {
            get
            {
                if (_ScalesList != null)
                    return _ScalesList;
                _ScalesList = core.datamodel.Database.s_cQry_FindInstancesOfClass(DatabaseApp.ContextIPlus, "PAEScaleCalibratable").ToList();
                return _ScalesList;
            }
            set
            {
                _ScalesList = value;
                OnPropertyChanged();
            }
        }


        Weighing _SelectedWeighing;
        [ACPropertySelected(658, "Weighings")]
        public Weighing SelectedWeighing
        {
            get
            {
                return _SelectedWeighing;
            }
            set
            {
                if (_SelectedWeighing != value)
                {
                    _SelectedWeighing = value;
                    OnPropertyChanged();
                }
            }
        }

        IEnumerable<Weighing> _WeighingList = null;

        [ACPropertyList(659, "Weighings")]
        public IEnumerable<Weighing> WeighingList
        {
            get
            {
                if (this.SelectedPickingPos == null
                    || this.SelectedPickingPos.EntityState == EntityState.Added
                    || this.SelectedPickingPos.EntityState == EntityState.Detached)
                    return null;
                if (_WeighingList != null)
                    return _WeighingList;
                _WeighingList = SelectedPickingPos.Weighing_PickingPos.OrderBy(c => c.StartDate).ToList();
                return _WeighingList;
            }
        }

        protected void RefreshWeighingList(bool forceRefresh = false)
        {
            if (forceRefresh && SelectedPickingPos != null)
            {
                SelectedPickingPos.Weighing_PickingPos.AutoLoad(SelectedPickingPos.Weighing_PickingPosReference, SelectedPickingPos);
            }
            _WeighingList = null;
            OnPropertyChanged(nameof(WeighingList));
        }

        #endregion


        #endregion

        #region Methods

        #region Weighing
        [ACMethodInfo("Dialog", "en{'Register Weight'}de{'Registriere Gewicht'}", (short)500)]
        public void RegisterWeight()
        {
            if (!IsEnabledRegisterWeight())
                return;
            string acUrl = SelectedScale.GetACUrlComponent();
            if (String.IsNullOrEmpty(acUrl))
                return;
            ACComponent scaleComp = Root.ACUrlCommand(acUrl) as ACComponent;
            if (scaleComp == null || scaleComp.ConnectionState == ACObjectConnectionState.DisConnected)
            {
                // TODO Message
                Messages.Error(this, "No connection", true);
                return;
            }
            Msg result = scaleComp.ACUrlCommand("!RegisterAlibiWeightEntity", new PAOrderInfoEntry() { EntityName = nameof(PickingPos), EntityID = SelectedPickingPos.PickingPosID }) as Msg;
            if (result == null)
                return;
            if (result.MessageLevel > eMsgLevel.Info)
            {
                Messages.Msg(result);
                return;
            }

            RefreshWeighingList(true);
        }

        public bool IsEnabledRegisterWeight()
        {
            return SelectedScale != null && SelectedPickingPos != null;
        }


        double _WeighingValue = 0.0;
        [ACPropertyInfo(659, "Weighings", "en{'Weight manually'}de{'Gewicht manuell'}")]
        public double WeighingValue
        {
            get
            {
                return _WeighingValue;
            }
            set
            {
                if (_WeighingValue != value)
                {
                    _WeighingValue = value;
                    OnPropertyChanged();
                }
            }
        }

        [ACMethodInfo("Dialog", "en{'Weight manuell'}de{'Gewicht manuell'}", (short)501)]
        public void AddManualWeighing()
        {
            if (!IsEnabledAddManualWeighing())
                return;
            string secondaryKey = Root.NoManager.GetNewNo(this.Database, typeof(Weighing), Weighing.NoColumnName, Weighing.FormatNewNo, this);
            Weighing weighing = Weighing.NewACObject(this.DatabaseApp, null, secondaryKey);
            weighing.Weight = WeighingValue;
            weighing.IdentNr = DateTime.Now.ToString();
            weighing.PickingPos = SelectedPickingPos;
            SelectedPickingPos.Weighing_PickingPos.Add(weighing);
            //this.DatabaseApp.Weighing.AddObject(weighing);
            RefreshWeighingList(true);
            WeighingValue = 0.0;
            Save();
        }

        public bool IsEnabledAddManualWeighing()
        {
            return SelectedPickingPos != null && Math.Abs(WeighingValue) > FacilityConst.C_ZeroCompare;
        }
        #endregion

        #endregion
    }
}
