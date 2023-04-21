// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="HU.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Die Klasse HU stellt eine hierarchich geschachtel Verpackungseinheit dar.
    /// </summary>
    [DataContract]
    public class HU
    {
        /// <summary>
        /// Verpacken des untersten Packguts (Material) in die HU mit HULevel = 0
        /// </summary>
        /// <param name="innerHUQuantity">The inner HU quantity.</param>
        /// <param name="mdQuantityUnitName">Name of the md quantity unit.</param>
        public HU(Double innerHUQuantity, string mdQuantityUnitName)
        {
            HULevel = 0;
            HUQuantity = 0;
            InnerHUQuantity = innerHUQuantity;
            HUMDQuantityUnitName = mdQuantityUnitName;
        }

        /// <summary>
        /// Verpacken einer HU mit einer übergeordneten HU
        /// </summary>
        /// <param name="packaging">The packaging.</param>
        /// <param name="huLevel">The hu level.</param>
        /// <param name="hu">The hu.</param>
        public HU(IPackaging packaging, int huLevel, HU hu)
        {
            HULevel = huLevel;
            HUQuantity = (Double)Math.Ceiling(hu.HUQuantity / packaging.ProductQuantity);
            InnerHUQuantity = packaging.ProductQuantity;
            HUMDQuantityUnitName = packaging.PackagingMDQuantityUnit.MDUnitName;
            HUList.Add(hu);
            foreach (var packagingPos in packaging.PackagingPosList)
            {
                HUPosList.Add(new HUPos(packagingPos.Material.MaterialNo, packagingPos.MDQuantityUnit, false, packagingPos.UsingRuleAttribute, packagingPos.Quantity, packagingPos.Material.NetWeight));
            }
        }

        /// <summary>
        /// Verpackungstiefe
        /// </summary>
        /// <value>The HU level.</value>
        [DataMember]
        public int HULevel { get; set; }

        /// <summary>
        /// Anzahl der physikalischen Menge der Handlingunit (Verpackung)
        /// </summary>
        /// <value>The HU quantity.</value>
        [DataMember]
        public Double HUQuantity { get; set; }

        /// <summary>
        /// Mengeneinheit der Handlingunit (Verpackung)
        /// </summary>
        /// <value>The name of the HUMD quantity unit.</value>
        [DataMember]
        public string HUMDQuantityUnitName { get; set; }

        /// <summary>
        /// Anzahl der physikalischen Menge Packgut je Handlingunit
        /// </summary>
        /// <value>The inner HU quantity.</value>
        [DataMember]
        public Double InnerHUQuantity { get; set; }

        /// <summary>
        /// The _ HU list
        /// </summary>
        HUList _HUList;
        /// <summary>
        /// Untergeordnerte Handlingunit (Verpackung)
        /// </summary>
        /// <value>The HU list.</value>
        [DataMember]
        public HUList HUList
        {
            get
            {
                if (_HUList == null)
                    _HUList = new HUList();
                return _HUList;
            }
        }

        /// <summary>
        /// The _ HU pos list
        /// </summary>
        HUPosList _HUPosList;
        /// <summary>
        /// Verwendetes Packgut und Verpackungsmaterial
        /// </summary>
        /// <value>The HU pos list.</value>
        [DataMember]
        public HUPosList HUPosList
        {
            get
            {
                if (_HUPosList == null)
                    _HUPosList = new HUPosList();
                return _HUPosList;
            }
        }

        #region Packaginginformationen
        /// <summary>
        /// Gibt den HU für den übergebenen "huLevel" zurück
        /// Funktioniert nur für HUPacking mit symetrischer Struktur
        /// </summary>
        /// <param name="huLevel">The hu level.</param>
        /// <returns>HU.</returns>
        public HU GetHUByHULevel(int huLevel)
        {
            if (HULevel < huLevel) // Ist nicht vorhanden
                return null;
            if (HULevel == huLevel) // Treffer huLevel erreicht
                return this;
            // Weiter unten suchen
            return HUList[0].GetHUByHULevel(huLevel);
        }
        #endregion

        #region Hilfsmethoden
        /// <summary>
        /// HUs the pos list recursive.
        /// </summary>
        /// <param name="huPosList">The hu pos list.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="withProductMaterial">if set to <c>true</c> [with product material].</param>
        /// <param name="withPackageMaterial">if set to <c>true</c> [with package material].</param>
        public void HUPosListRecursive(ref List<HUPos> huPosList, bool recursive, bool withProductMaterial, bool withPackageMaterial)
        {
            foreach (var huPos in HUPosList)
            {
                if ((huPos.IsProduct && withProductMaterial) || (!huPos.IsProduct && withPackageMaterial))
                {
                    huPosList.Add(huPos);
                }
            }
            if (recursive)
            {
                foreach (var hu in HUList)
                {
                    hu.HUPosListRecursive(ref huPosList, recursive, withProductMaterial, withPackageMaterial);
                }
            }
        }

        /// <summary>
        /// HUs the list recursive.
        /// </summary>
        /// <param name="huList">The hu list.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public void HUListRecursive(ref List<HU> huList, bool recursive)
        {
            huList.Add(this);
            foreach (var hu in HUList)
            {
                hu.HUListRecursive(ref huList, recursive);
            }
        }

        /// <summary>
        /// Prüft ob MaterialNo und ChargenNo vorhanden ist
        /// </summary>
        /// <param name="materialNo">The material no.</param>
        /// <param name="chargeNo">The charge no.</param>
        /// <returns><c>true</c> if the specified material no contains material; otherwise, <c>false</c>.</returns>
        public bool ContainsMaterial(string materialNo, string chargeNo)
        {
            foreach(var huPos in HUPosList)
            {
                if (huPos.ContainsMaterial(materialNo, chargeNo))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// !!! Rekursive Methode !!!
        /// </summary>
        /// <param name="huData">The hu data.</param>
        /// <param name="huDataPos">The hu data pos.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool InPackingMaterial(HUData huData, HUDataPos huDataPos/* string materialNo, string chargeNo, Double quantity, Double weight*/)
        {
            if (ContainsMaterial(huDataPos.MaterialNo, ""))
            {
                Double restQuantity = huDataPos.Quantity;
                HUPos huPos = null;
                HUPosCharge huPosCharge = null;
                if (!string.IsNullOrEmpty(huDataPos.ChargeNo))
                {
                    foreach(HUPos huPosTemp in HUPosList)
                    {
                        huPosCharge = huPosTemp.HUPosChargeList.HUPosChargeContainsCharge(huDataPos.ChargeNo);
                        if ( huPosCharge != null )
                        {
                            huPos = huPosTemp;
                            break;
                        }
                    }

                }
                if (huPos == null)
                {
                    huPos = HUPosList[0];
                }
                huPos.Quantity += huDataPos.Quantity;
                if (huDataPos.Weight == 0)
                {
                    huPos.Weight += huDataPos.Quantity * huPos.HUPosWeight;
                }
                else
                {
                    huPos.Weight += huDataPos.Weight;
                }

                if (!string.IsNullOrEmpty(huDataPos.ChargeNo))
                {
                    if (huPosCharge == null)
                    {
                        huPosCharge = new HUPosCharge(huDataPos.ChargeNo, huDataPos.Quantity, huDataPos.Weight);
                        huPos.HUPosChargeList.Add(huPosCharge);
                    }
                    else
                    {
                        huPosCharge.Quantity += huDataPos.Quantity;
                        if (huDataPos.Weight == 0)
                        {
                            huPosCharge.Weight += huDataPos.Quantity * huPos.HUPosWeight;
                        }
                        else
                        {
                            huPosCharge.Weight += huDataPos.Weight;
                        }
                    }
                }
                huDataPos.Quantity = 0;
                huDataPos.Weight = 0;

                Double sumQuantity = 0;
                foreach (var huPos1 in HUPosList)
                {
                    sumQuantity += huPos1.Quantity;
                }
                HUQuantity = sumQuantity;

                return true;
            }
            else
            {
                bool ok = false;
                foreach (var hu in HUList)
                {
                    if (hu.InPackingMaterial(huData, huDataPos))
                    {

                        ok = true;
                        break;
                    }
                }
                if (ok)
                {
                    RecalcHU(huData);
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// !!! Rekursive Methode !!!
        /// </summary>
        /// <param name="huData">The hu data.</param>
        /// <param name="huDataPos">The hu data pos.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool OutPackingMaterial(HUData huData, HUDataPos huDataPos)
        {
            if (ContainsMaterial(huDataPos.MaterialNo, huDataPos.ChargeNo))
            {
                Double restQuantity = huDataPos.Quantity;
                Double restWeight = huDataPos.Weight;
                HUPos huPos = null;
                for (int i = HUPosList.Count; i > 0; i--)
                {
                    huPos = HUPosList[i-1];
                    if (huPos.ContainsMaterial(huDataPos.MaterialNo, huDataPos.ChargeNo))
                    {
                        if (!huPos.HUPosChargeList.Any())
                        {
                            // Ohne Chargenführung
                            if (huPos.Quantity > restQuantity)
                            {
                                Double weight2 = huPos.Weight / huPos.Quantity;
                                huPos.Quantity -= restQuantity;
                                huPos.Weight = huPos.Quantity * weight2;
                                huData.AddInputResultHUDataPos(huPos.MaterialNo, restQuantity, restQuantity * weight2, "", huPos.IsProduct);
                                break;
                            }
                            else
                            {
                                restQuantity = -huPos.Quantity;
                                huData.AddInputResultHUDataPos(huPos.MaterialNo, huPos.Quantity, huPos.Weight, "", huPos.IsProduct);
                                huPos.Quantity = 0;
                                huPos.Weight = 0;
                            }
                        }
                        else
                        {
                            // Mit Chargenführung
                            List<HUPosCharge> huPosChargeList;
                            if (string.IsNullOrEmpty(huDataPos.ChargeNo))
                            {
                                // ChargeNo wurde nicht übergeben, also einfach abbuchen von nächster Charge
                                huPosChargeList = huPos.HUPosChargeList.ToList();
                            }
                            else
                            {
                                // ChargeNo wurde nicht übergeben, also nur von konkreten Chargen abbuchen
                                huPosChargeList = huPos.HUPosChargeList.Where(c => c.ChargeNo == huDataPos.ChargeNo).ToList();
                            }

                            foreach (var huPosCharge in huPosChargeList)
                            {
                                if (huPosCharge.Quantity > restQuantity)
                                {
                                    Double weight2 = huPosCharge.Weight / huPosCharge.Quantity;
                                    huPosCharge.Quantity -= restQuantity;
                                    huPosCharge.Weight = huPosCharge.Quantity * weight2;
                                    huData.AddInputResultHUDataPos(huPos.MaterialNo, restQuantity, restQuantity * weight2, huPosCharge.ChargeNo, huPos.IsProduct);
                                    break;
                                }
                                else
                                {
                                    restQuantity -= huPosCharge.Quantity;
                                    huData.AddInputResultHUDataPos(huPos.MaterialNo, huPosCharge.Quantity, huPosCharge.Weight, huPosCharge.ChargeNo, huPos.IsProduct);
                                    huPos.HUPosChargeList.Remove(huPosCharge);
                                }
                            }

                            huPos.Quantity = 0;
                            huPos.Weight = 0;
                            foreach (var huPosCharge in huPos.HUPosChargeList)
                            {
                                huPos.Quantity += huPosCharge.Quantity;
                                huPos.Weight += huPosCharge.Weight;
                            }
                        }
                    }
                }
                Double sumQuantity = 0;
                foreach(var huPos1 in HUPosList)
                {
                    sumQuantity += huPos1.Quantity;
                }
                HUQuantity = sumQuantity;

                return true;
            }
            else
            {
                bool ok = false;
                foreach (var hu in HUList)
                {
                    if (hu.OutPackingMaterial(huData, huDataPos))
                    {
                       
                        ok = true;
                        break;
                    }
                }
                if (ok)
                {
                    RecalcHU(huData);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Recalcs the HU.
        /// </summary>
        /// <param name="huData">The hu data.</param>
        void RecalcHU(HUData huData)
        {
            Double sumQuantity = 0;
            foreach (var hu in HUList)
            {
                sumQuantity += hu.HUQuantity;
            }

            // Gesamtmenge von der Verpackung ermitteln
            HUQuantity = (Double)Math.Ceiling(sumQuantity / InnerHUQuantity);

            foreach (HUPos huPos in HUPosList)
            {
                Double weight = huPos.Weight / huPos.Quantity;
                Double diffQuantity = -(HUQuantity * huPos.HUPosQuantity - huPos.Quantity);
                
                huData.AddInputResultHUDataPos(huPos.MaterialNo, diffQuantity, 0, "", false);

                huPos.Quantity = HUQuantity * huPos.HUPosQuantity;
                huPos.Weight = huPos.Quantity * weight;
            }
        }

        /// <summary>
        /// Vergleicht, ob die Struktur der "hu" mit der Struktur der "packaging" übereinstimmt.
        /// Betrachtet wird nur das aktuell huLevel. Tiefere huLevel werden nicht betrachtet.
        /// </summary>
        /// <param name="packaging">The packaging.</param>
        /// <returns><c>true</c> if [is compatible packing] [the specified packaging]; otherwise, <c>false</c>.</returns>
        public bool IsCompatiblePacking(IPackaging packaging)
        {
            if (HULevel != packaging.MaterialUnitPackagingLevel)
                return false;
            if (InnerHUQuantity != packaging.ProductQuantity)
                return false;
            if (HUMDQuantityUnitName != packaging.PackagingMDQuantityUnit.MDUnitName)
                return false;

            // Alle HUPos raussuchen, die kein Packgut sind.
            var huPosList = HUPosList.Where(c => !c.IsProduct);

            if (huPosList.Count() != packaging.PackagingPosList.Count())
                return false;
            foreach (var huPos in huPosList)
            {
                var query = packaging.PackagingPosList.Where(c => c.Material.MaterialNo == huPos.MaterialNo);
                if (!query.Any())
                    return false;

                IPackagingPos packagingPos = query.First();

                if (huPos.HUPosQuantity != packagingPos.Quantity)
                    return false;

                if (huPos.MDQuantityUnitName != packagingPos.MDQuantityUnit.MDUnitName)
                    return false;
            }

            return true;
        }

        #endregion
    }
}

