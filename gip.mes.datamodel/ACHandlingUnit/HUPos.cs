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
// <copyright file="HUPos.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Class HUPos
    /// </summary>
    [DataContract]
    public class HUPos
    {
        /// <summary>
        /// Konstruktor für neue HUPos ohne Bestand
        /// </summary>
        /// <param name="materialNo">The material no.</param>
        /// <param name="mdQuantityUnit">The md quantity unit.</param>
        /// <param name="mdWeightUnit">The md weight unit.</param>
        /// <param name="isProduct">if set to <c>true</c> [is product].</param>
        /// <param name="usingRuleAttribute">The using rule attribute.</param>
        /// <param name="huPosQuantity">The hu pos quantity.</param>
        /// <param name="huPosWeight">The hu pos weight.</param>
        public HUPos(string materialNo, MDUnit mdQuantityUnit, bool isProduct, String usingRuleAttribute, Double huPosQuantity, Double huPosWeight)
        {
            MaterialNo = materialNo;
            Quantity = 0;
            MDQuantityUnitName = mdQuantityUnit.MDUnitName;
            Weight = 0;
            //MDWeightUnitName = mdWeightUnit.MDUnitName;
            IsProduct = isProduct;
            HUPosQuantity = huPosQuantity;
            HUPosWeight = huPosWeight;
            UsingRuleAttribute = usingRuleAttribute;
        }

        /// <summary>
        /// Konstruktor für eine HUPos mit Bestand
        /// </summary>
        /// <param name="materialNo">The material no.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="mdQuantityUnit">The md quantity unit.</param>
        /// <param name="weight">The weight.</param>
        /// <param name="mdWeightUnit">The md weight unit.</param>
        /// <param name="isProduct">if set to <c>true</c> [is product].</param>
        /// <param name="chargeNo">The charge no.</param>
        /// <param name="usingRuleAttribute">The using rule attribute.</param>
        /// <param name="huPosQuantity">The hu pos quantity.</param>
        public HUPos(string materialNo, Double quantity, MDUnit mdQuantityUnit, Double weight, MDUnit mdWeightUnit, bool isProduct, string chargeNo, String usingRuleAttribute, Double huPosQuantity)
        {
            MaterialNo = materialNo;
            Quantity = quantity;
            MDQuantityUnitName = mdQuantityUnit.MDUnitName;
            Weight = weight;
            MDWeightUnitName = mdWeightUnit.MDUnitName;
            IsProduct = isProduct;
            HUPosQuantity = huPosQuantity;
            UsingRuleAttribute = usingRuleAttribute;
            if (!string.IsNullOrEmpty(chargeNo))
            {
                HUPosChargeList.Add(new HUPosCharge(chargeNo, quantity, weight));
            }
        }

        /// <summary>
        /// Artikelnnummer
        /// </summary>
        /// <value>The material no.</value>
        [DataMember]
        public string MaterialNo { get; set; }

        /// <summary>
        /// Mengenangabe der Verpackung entsprechend der Packvorschrift
        /// </summary>
        /// <value>The HU pos quantity.</value>
        [DataMember]
        public Double HUPosQuantity { get; set; }

        /// <summary>
        /// Gewichtangabe je 1 Quantity
        /// </summary>
        /// <value>The HU pos weight.</value>
        [DataMember]
        public Double HUPosWeight { get; set; }

        /// <summary>
        /// Mengenangabe des Materials
        /// </summary>
        /// <value>The quantity.</value>
        [DataMember]
        public Double Quantity { get; set; }

        /// <summary>
        /// Mengeneinheitsname
        /// </summary>
        /// <value>The name of the MD quantity unit.</value>
        [DataMember]
        public string MDQuantityUnitName { get; set; }

        /// <summary>
        /// Gewichtangabe
        /// a) IsProduct == false, dann Menge je Verpackungseinheit
        /// b) IsProduct == true,  dann komplette Menge des Packguts
        /// </summary>
        /// <value>The weight.</value>
        [DataMember]
        public Double Weight { get; set; }

        /// <summary>
        /// Gewichteinheitsname
        /// </summary>
        /// <value>The name of the MD weight unit.</value>
        [DataMember]
        public string MDWeightUnitName { get; set; }

        /// <summary>
        /// Angabe, ob die HUPos den Packgut-Artikel entspricht
        /// </summary>
        /// <value><c>true</c> if this instance is product; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool IsProduct { get; set; }

        /// <summary>
        /// The _ HU pos charge list
        /// </summary>
        HUPosChargeList _HUPosChargeList;
        /// <summary>
        /// Gets the HU pos charge list.
        /// </summary>
        /// <value>The HU pos charge list.</value>
        [DataMember]
        public HUPosChargeList HUPosChargeList
        {
            get
            {
                if (_HUPosChargeList == null)
                    _HUPosChargeList = new HUPosChargeList();
                return _HUPosChargeList;
            }
        }

        /// <summary>
        /// Gets or sets the using rule attribute.
        /// </summary>
        /// <value>The using rule attribute.</value>
        [DataMember]
        public String UsingRuleAttribute { get; set; }

        /// <summary>
        /// Determines whether the specified material no contains material.
        /// </summary>
        /// <param name="materialNo">The material no.</param>
        /// <param name="chargeNo">The charge no.</param>
        /// <returns><c>true</c> if the specified material no contains material; otherwise, <c>false</c>.</returns>
        public bool ContainsMaterial(string materialNo, string chargeNo)
        {
            if (MaterialNo != materialNo)
                return false;
            if (string.IsNullOrEmpty(chargeNo))
                return true;
            if (HUPosChargeList.Where(c => c.ChargeNo == chargeNo).Any())
                return true;
            return false;
        }

    }
}