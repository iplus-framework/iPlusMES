// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="HUData.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Class HUData
    /// </summary>
    public class HUData
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="HUData"/> class.
        /// </summary>
        public HUData()
        {
            Succeded = false;
        }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="HUData"/> is succeded.
        /// </summary>
        /// <value><c>true</c> if succeded; otherwise, <c>false</c>.</value>
        public bool Succeded { get; set; }

        /// <summary>
        /// Adds the input HU data pos.
        /// </summary>
        /// <param name="materialNo">The material no.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="chargeNo">The charge no.</param>
        /// <param name="isProduct">if set to <c>true</c> [is product].</param>
        public void AddInputHUDataPos(string materialNo, Double quantity, Double weight, String chargeNo, bool isProduct)
        {
            HUDataPos huDataPos = new HUDataPos { MaterialNo = materialNo, Quantity = quantity, Weight = weight, ChargeNo = chargeNo, IsProduct = isProduct, IsResult = false };
            HUDataPosList.Add(huDataPos);
        }

        /// <summary>
        /// Verbrauch an Verpackungsmaterial protollieren
        /// </summary>
        /// <param name="materialNo">The material no.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="chargeNo">The charge no.</param>
        /// <param name="isProduct">if set to <c>true</c> [is product].</param>
        public void AddInputResultHUDataPos(string materialNo, Double quantity, Double weight, String chargeNo, bool isProduct)
        {
            HUDataPos huDataPos = null;
            if ( string.IsNullOrEmpty(chargeNo))
            {
                var query = PackageHUDataPosList.Where(c=>c.MaterialNo == materialNo);
                if ( query.Any())
                    huDataPos = query.First();
            }
            else
            {
                var query = PackageHUDataPosList.Where(c=>c.MaterialNo == materialNo && c.ChargeNo == chargeNo);
                if (query.Any())
                    huDataPos = query.First();
            }
            if ( huDataPos == null)
            {
                huDataPos = new HUDataPos { MaterialNo = materialNo, Quantity = 0, Weight = 0, ChargeNo = chargeNo, IsProduct = isProduct, IsResult = true };
                HUDataPosList.Add(huDataPos);
            }
            huDataPos.Quantity += quantity;
            huDataPos.Weight += weight;
        }

        /// <summary>
        /// Material
        /// </summary>
        /// <value>The product HU data pos list.</value>
        public IEnumerable<HUDataPos> ProductHUDataPosList
        {
            get
            {
                return this.HUDataPosList.Where(c => c.IsProduct && !c.IsResult);
            }
        }


        /// <summary>
        /// Material
        /// </summary>
        /// <value>The result product HU data pos list.</value>
        public IEnumerable<HUDataPos> ResultProductHUDataPosList
        {
            get
            {
                return this.HUDataPosList.Where(c => c.IsProduct && c.IsResult);
            }
        }

        /// <summary>
        /// Verpackungsmaterial
        /// </summary>
        /// <value>The package HU data pos list.</value>
        public IEnumerable<HUDataPos> PackageHUDataPosList
        {
            get
            {
                return this.HUDataPosList.Where(c => !c.IsProduct);
            }
        }

        /// <summary>
        /// The _ HU data pos list
        /// </summary>
        HUDataPosList _HUDataPosList = new HUDataPosList();
        /// <summary>
        /// Gets the HU data pos list.
        /// </summary>
        /// <value>The HU data pos list.</value>
        public HUDataPosList HUDataPosList
        {
            get
            {
                return _HUDataPosList;
            }
        }
    }

    /// <summary>
    /// Class HUDataPos
    /// </summary>
    public class HUDataPos
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HUDataPos"/> class.
        /// </summary>
        public HUDataPos()
        {
        }

        /// <summary>
        /// Gets or sets the material no.
        /// </summary>
        /// <value>The material no.</value>
        public string MaterialNo { get; set; }
        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public Double Quantity { get; set; }
        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        public Double Weight { get; set; }
        /// <summary>
        /// Gets or sets the charge no.
        /// </summary>
        /// <value>The charge no.</value>
        public String ChargeNo { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is product.
        /// </summary>
        /// <value><c>true</c> if this instance is product; otherwise, <c>false</c>.</value>
        public bool IsProduct { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is result.
        /// </summary>
        /// <value><c>true</c> if this instance is result; otherwise, <c>false</c>.</value>
        public bool IsResult { get; set; }
    }

    /// <summary>
    /// Class HUDataPosList
    /// </summary>
    public class HUDataPosList : List<HUDataPos>
    {
    }
}
