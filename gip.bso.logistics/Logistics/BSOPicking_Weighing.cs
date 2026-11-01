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
    /// <summary>
    /// Partial class of BSOPicking that manages vehicle/position weighings for a picking position.
    /// This partial adds the scale selection (ScalesList contains all instances of the class "PAEScaleCalibratable",
    /// SelectedScale is the currently selected vehicle scale) and the weighing list of the selected picking position
    /// (WeighingList with SelectedWeighing, sorted by start date).
    /// RegisterWeight() registers an alibi weight on the selected scale for the selected picking position (via the scale component's RegisterAlibiWeightEntity command).
    /// WeighingValue holds a manually entered weight; AddManualWeighing() creates a new Weighing record with this weight for the selected picking position and saves it.
    /// RefreshWeighingList() reloads the weighing list of the selected picking position.
    /// </summary>
    public partial class BSOPicking : ACBSOvbNav
    {
        #region Properties

        #region Weighing
        private core.datamodel.ACClass _SelectedScale;
        /// <summary>
        /// Gets or sets the currently selected vehicle scale.
        /// </summary>
        [ACPropertySelected(611, "Scales", "en{'Vehicle scale'}de{'Fahrzeugwaage'}", Description = @"Currently selected vehicle scale (instance of the class PAEScaleCalibratable). RegisterWeight() registers the alibi weight on this scale.")]
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
        /// <summary>
        /// Gets the list of available vehicle scales (all instances of the class "PAEScaleCalibratable").
        /// </summary>
        [ACPropertyList(610, "Scales", "en{'Vehicle Scales'}de{'Fahrzeugwaagen'}", Description = @"List of available vehicle scales: all instances of the iPlus class PAEScaleCalibratable found in the system.")]
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
        /// <summary>
        /// Gets or sets the selected weighing in the weighing list of the selected picking position.
        /// </summary>
        [ACPropertySelected(658, "Weighings", "en{'Weighing'}de{'Wägung'}", Description = @"Selected weighing in the weighing list of the selected picking position.")]
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

        /// <summary>
        /// Gets the list of weighings of the selected picking position, sorted by start date.
        /// </summary>
        [ACPropertyList(659, "Weighings", "en{'Weighings'}de{'Wägungen'}", Description = @"List of weighings (Weighing) of the selected picking position, sorted by start date. Null if no picking position is selected or the position is not yet saved.")]
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
        /// <summary>
        /// Registers an alibi weight on the selected vehicle scale for the selected picking position.
        /// </summary>
        [ACMethodInfo("Dialog", "en{'Register Weight'}de{'Registriere Gewicht'}", (short)500, Description = @"Registers an alibi weight on the selected vehicle scale (SelectedScale) for the selected picking position. The scale component's RegisterAlibiWeightEntity command is invoked with a reference to the PickingPos; afterwards the weighing list is refreshed. Enabled only if a scale and a saved picking position are selected and the scale is connected.")]
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
                Messages.ErrorAsync(this, "No connection", true);
                return;
            }
            Msg result = scaleComp.ACUrlCommand("!RegisterAlibiWeightEntity", new PAOrderInfoEntry() { EntityName = nameof(PickingPos), EntityID = SelectedPickingPos.PickingPosID }) as Msg;
            if (result == null)
                return;
            if (result.MessageLevel > eMsgLevel.Info)
            {
                Messages.MsgAsync(result);
                return;
            }

            RefreshWeighingList(true);
        }

        public bool IsEnabledRegisterWeight()
        {
            return SelectedScale != null && SelectedPickingPos != null;
        }


        double _WeighingValue = 0.0;
        /// <summary>
        /// Gets or sets the manually entered weight for AddManualWeighing().
        /// </summary>
        [ACPropertyInfo(659, "Weighings", "en{'Weight manually'}de{'Gewicht manuell'}", Description = @"Manually entered weight value. AddManualWeighing() creates a weighing record with this weight for the selected picking position.")]
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

        /// <summary>
        /// Creates a manual weighing record with the entered WeighingValue for the selected picking position and saves it.
        /// </summary>
        [ACMethodInfo("Dialog", "en{'Weight manuell'}de{'Gewicht manuell'}", (short)501, Description = @"Creates a new Weighing record with the manually entered WeighingValue for the selected picking position (a new weighing number is generated), adds it to the position and saves it to the database. Enabled only if a picking position is selected and WeighingValue is not zero.")]
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
            _= Save();
        }

        public bool IsEnabledAddManualWeighing()
        {
            return SelectedPickingPos != null && Math.Abs(WeighingValue) > FacilityConst.C_ZeroCompare;
        }
        #endregion

        #endregion
    }
}
