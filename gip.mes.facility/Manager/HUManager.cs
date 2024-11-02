// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class HUManager : ACManagerBase
    { 
        #region c´tors
        public HUManager(IACEntityObjectContext db, IRoot root)
            : base(db, root)
        {
        }
        #endregion

        public HandlingUnit CreateHandlingUnit(IPackagingHierarchy packagingHierarchy, int huLevel, string materialNo, MDUnit mdQuantityUnit, Single posWeight)
        {
            int currentPackagingLevel = 0;

            // HandlingUnit anlegen
            HandlingUnit handlingUnit = new HandlingUnit();
            // Innerste HU für Material anlegen
            HU hu = new HU(1, mdQuantityUnit.MDUnitName);

            // Innerste HUPos für Material anlegen
            HUPos huPos = new HUPos(materialNo, mdQuantityUnit, true, "", 1, posWeight);

            hu.HUPosList.Add(huPos);

            if (packagingHierarchy != null && huLevel > 0)
            {
                foreach (IPackaging packaging in packagingHierarchy.PackagingList)
                {
                    currentPackagingLevel++;
                    if (huLevel != 0 && currentPackagingLevel > huLevel)
                        break;
                    hu = new HU(packaging, currentPackagingLevel, hu);
                }
            }

            handlingUnit.HU = hu;
            return handlingUnit;
        }

        /// <summary>
        /// Verpacken von Material mit symetrischer Struktur
        /// </summary>
        public HUData InPacking(HandlingUnit handlingUnit, string materialNo, Single quantity, Single weight, string chargeNo)
        {
            HUData huData = new HUData();
            huData.AddInputHUDataPos(materialNo, quantity, weight, chargeNo, true);

            InPackingIntern(huData, handlingUnit);
            return huData;
        }

        private bool InPackingIntern(HUData huData, HandlingUnit handlingUnit)
        {
            if (handlingUnit.InPackingMaterial(huData))
                huData.Succeded = true;
            return huData.Succeded;
        }

        /// <summary>
        /// Auspacken einer HandlingUnit mit symetrischer Struktur bis zum gewünschten huLevel
        /// </summary>
        public HUData OutPacking(HandlingUnit handlingUnit, int huLevel)
        {
            return handlingUnit.OutPacking(huLevel);
        }

        /// <summary>
        /// Auspacken einer Anzahl von Material HandlingUnit mit symetrischer Struktur
        /// </summary>
        public HUData OutPackingMaterial(HandlingUnit handlingUnit, string materialNo, Single quantity, Single weight, string chargeNo)
        {
            HUData huData = new HUData();
            huData.AddInputHUDataPos(materialNo, quantity, weight, chargeNo, true);

            OutPackingMaterialIntern(huData, handlingUnit);
            return huData;
        }

        private bool OutPackingMaterialIntern(HUData huData, HandlingUnit handlingUnit)
        {
            huData.Succeded = handlingUnit.OutPackingMaterial(huData);
            return huData.Succeded;
        }

        /// <summary>
        /// Umpacken einer HandlingUnit mit symetrischer Struktur
        /// </summary>
        public HUData RePacking(HandlingUnit handlingUnit, IPackagingHierarchy packagingHierarchy, int huLevel)
        {
            return handlingUnit.RePacking(packagingHierarchy, huLevel);
        }

        /// <summary>
        /// Umpacken eines Teils der HandlingUnit mit symetrischer Struktur
        /// </summary>
        public HUData SplitPackaging(HandlingUnit handlingUnit, HandlingUnit handlingUnit2, string materialNo, Single quantity, Single weight, string chargeNo)
        {
            HUData huData = new HUData();
            huData.AddInputHUDataPos(materialNo, quantity, weight, chargeNo, true);

            OutPackingMaterialIntern(huData, handlingUnit);
            if (huData.Succeded)
            {
                // Das Ergebnis der Abbuchung so abwandeln, das es als Eingabeparameter für die Einbuchung verwendet werden kann.
                foreach (var huDataPos in huData.HUDataPosList.ToList())
                {
                    if (!huDataPos.IsResult)
                    {
                        huData.HUDataPosList.Remove(huDataPos);
                    }
                    else
                    {
                        if (huDataPos.IsProduct)
                            huDataPos.IsResult = false;
                    }
                }
                // TODO: InPackingIntern
                InPackingIntern(huData, handlingUnit2/*, materialNo, quantity, weight, chargeNo*/);
            }

            return huData;
        }

        public HandlingUnit DeserializeHU(string handlingUnitXML)
        {
            return HandlingUnit.DeserializeHandlingUnit(handlingUnitXML);
        }

        public string SerializeHU(HandlingUnit handlingUnit)
        {
            return handlingUnit.SerializeHandlingUnit();
        }

    }
}
