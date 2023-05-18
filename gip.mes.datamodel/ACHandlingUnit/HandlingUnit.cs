// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="HandlingUnit.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Class HandlingUnit
    /// </summary>
    [DataContract]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'HandlingUnit'}de{'HandlingUnit'}", Global.ACKinds.TACClass)]
    public class HandlingUnit
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="HandlingUnit"/> class.
        /// </summary>
        public HandlingUnit()
        {
            HandlingUnitID = Guid.NewGuid();
            HU = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlingUnit"/> class.
        /// </summary>
        /// <param name="HandlingUnitXML">The handling unit XML.</param>
        public HandlingUnit(string HandlingUnitXML)
        {
        }
        #endregion

        /// <summary>
        /// Gets or sets the handling unit ID.
        /// </summary>
        /// <value>The handling unit ID.</value>
        [DataMember]
        public Guid HandlingUnitID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HU.
        /// </summary>
        /// <value>The HU.</value>
        [DataMember]
        public HU HU
        {
            get;
            set;
        }

        #region Serialization
        /// <summary>
        /// Deserializes the handling unit.
        /// </summary>
        /// <param name="handlingUnitXML">The handling unit XML.</param>
        /// <returns>HandlingUnit.</returns>
        static public HandlingUnit DeserializeHandlingUnit(string handlingUnitXML)
        {
            // Einmal mit DataContractSerializer serialisieren
            using (StringReader ms = new StringReader(handlingUnitXML))
            using (XmlTextReader xmlReader = new XmlTextReader(ms))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(HandlingUnit), new DataContractSerializerSettings() { KnownTypes = ACKnownTypes.GetKnownType(), MaxItemsInObjectGraph = 99999999, IgnoreExtensionDataObject = true, PreserveObjectReferences = true, DataContractResolver = ACConvert.MyDataContractResolver });
                HandlingUnit handlingUnit = (HandlingUnit)serializer.ReadObject(xmlReader);
                return handlingUnit;
            }
        }

        /// <summary>
        /// Serializes the handling unit.
        /// </summary>
        /// <returns>System.String.</returns>
        public string SerializeHandlingUnit()
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(HandlingUnit), new DataContractSerializerSettings() { KnownTypes = ACKnownTypes.GetKnownType(), MaxItemsInObjectGraph = 99999999, IgnoreExtensionDataObject = true, PreserveObjectReferences = true, DataContractResolver = ACConvert.MyDataContractResolver });
                serializer.WriteObject(xmlWriter, this);

                string handlingUnitXML = sw.ToString();
                return handlingUnitXML;
            }
        }
        #endregion

        /// <summary>
        /// Gets a value indicating whether this instance is Double product.
        /// </summary>
        /// <value><c>true</c> if this instance is single product; otherwise, <c>false</c>.</value>
        public bool IsSingleProduct
        {
            get
            {
                String materialNo = null; 
                
                foreach (HUPos huPos in ProductMaterialList)
                {
                    if (string.IsNullOrEmpty(materialNo))
                    {
                        materialNo = huPos.MaterialNo;
                    }
                    else
                    {
                        if (materialNo != huPos.MaterialNo)
                            return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the single product.
        /// </summary>
        /// <value>The single product.</value>
        public string SingleProduct
        {
            get
            {
                String materialNo = null;

                foreach (HUPos huPos in ProductMaterialList)
                {
                    if (string.IsNullOrEmpty(materialNo))
                    {
                        materialNo = huPos.MaterialNo;
                    }
                    else
                    {
                        if (materialNo != huPos.MaterialNo)
                            return "";
                    }
                }
                return materialNo;
            }
        }

        /// <summary>
        /// Gets the product quantity.
        /// </summary>
        /// <param name="materialNo">The material no.</param>
        /// <param name="chargeNo">The charge no.</param>
        /// <returns>Single.</returns>
        public Double GetProductQuantity(string materialNo, string chargeNo)
        {
            Double quantity = 0;
            foreach (var huPos in ProductMaterialList.Where(c=>c.MaterialNo == materialNo))
            {
                if (string.IsNullOrEmpty(chargeNo))
                {
                    quantity += huPos.Quantity;
                }
                else
                {
                    foreach(var huPosCharge in huPos.HUPosChargeList.Where(c=>c.ChargeNo == chargeNo))
                    {
                        quantity += huPosCharge.Quantity;
                    }
                }
            }
            return quantity;
        }

        /// <summary>
        /// Gets the product material list.
        /// </summary>
        /// <value>The product material list.</value>
        public IEnumerable<HUPos> ProductMaterialList
        {
            get
            {
                List<HUPos> huPosList = new List<HUPos>();
                HU.HUPosListRecursive(ref huPosList, true, true, false);
                return huPosList;
            }
        }
        /// <summary>
        /// Gets the package material list.
        /// </summary>
        /// <value>The package material list.</value>
        public IEnumerable<HUPos> PackageMaterialList
        {
            get
            {
                List<HUPos> huPosList = new List<HUPos>();
                HU.HUPosListRecursive(ref huPosList, true, false, true);
                return huPosList;
            }
        }
        /// <summary>
        /// Gets the material list.
        /// </summary>
        /// <value>The material list.</value>
        public IEnumerable<HUPos> MaterialList
        {
            get
            {
                List<HUPos> huPosList = new List<HUPos>();
                HU.HUPosListRecursive(ref huPosList, true, true, true);
                return huPosList;
            }
        }

        /// <summary>
        /// Gets the HU list.
        /// </summary>
        /// <value>The HU list.</value>
        public IEnumerable<HU> HUList
        {
            get
            {
                List<HU> huList = new List<HU>();
                HU.HUListRecursive(ref huList, true);
                return huList;
            }
        }

        /// <summary>
        /// Ins the packing material.
        /// </summary>
        /// <param name="huData">The hu data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool InPackingMaterial(HUData huData)
        {
            foreach (var huDataPos in huData.ProductHUDataPosList.ToList())
            {
                if (!HU.InPackingMaterial(huData, huDataPos))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Outs the packing material.
        /// </summary>
        /// <param name="huData">The hu data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool OutPackingMaterial(HUData huData)
        {
            foreach (var huDataPos in huData.ProductHUDataPosList.ToList())
            {
                Double productQuantity = GetProductQuantity(huDataPos.MaterialNo, huDataPos.ChargeNo);
                if (huDataPos.Quantity > productQuantity)
                {
                    huData.Succeded = false;
                    return huData.Succeded;
                }
                if (!HU.OutPackingMaterial(huData, huDataPos))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Outs the packing.
        /// </summary>
        /// <param name="huLevel">The hu level.</param>
        /// <returns>HUData.</returns>
        public HUData OutPacking(int huLevel)
        {
            HUData huData = new HUData();

            // Prüfen, ob die Struktur symetrisch ist
            HU hu = HU;
            while (hu.HUList.Any())
            {
                if (hu.HUList.Count() != 1)
                    return huData;
                hu = hu.HUList[0];
            }

            hu = HU;
            // Jetzt entpacken bis zum gewünschten HULevel
            while (hu.HULevel > huLevel)
            {
                hu = hu.HUList[0];
            }

            HU = hu;
            huData.Succeded = true;

            return huData;
        }

        /// <summary>
        /// Res the packing.
        /// </summary>
        /// <param name="packagingHierarchy">The packaging hierarchy.</param>
        /// <param name="huLevel">The hu level.</param>
        /// <returns>HUData.</returns>
        public HUData RePacking(IPackagingHierarchy packagingHierarchy, int huLevel)
        {
            HUData huData = new HUData();

            int compatibleHULevel = 0; // Erst mal ist keine Kompatibilität vorhanden
            int checkHULevel = 0;

            HU hu = HU;
            // Solange der zu prüfende "checkHULevel" noch nicht den "huLevel" erreicht hat
            // und die vorherigen Level erfolgreich geprüft wurden, dann weiter prüfen
            while (checkHULevel < huLevel)
            {
                checkHULevel++;
                HU huToCheck = hu.GetHUByHULevel(checkHULevel);
                if (huToCheck == null)
                    break;
                IPackaging packaging = packagingHierarchy.PackagingList.Where(c => c.MaterialUnitPackagingLevel == checkHULevel).First();
                if (!huToCheck.IsCompatiblePacking(packaging))
                    break;

                compatibleHULevel = checkHULevel;
            }

            if (HU.HULevel > compatibleHULevel)
            {
                // Falls nötig, auspacken bis zum compatibleHULevel
                huData = OutPacking(compatibleHULevel);
                // Falls nötig, einpacken bis zum gewünschten huLevel
            }
            if (HU.HULevel < huLevel)
            {
                hu = HU;
                int nextPackagingLevel = hu.HULevel + 1;
                foreach (IPackaging packaging in packagingHierarchy.PackagingList)
                {
                    if (packaging.MaterialUnitPackagingLevel >= nextPackagingLevel)
                    {
                        if (nextPackagingLevel > huLevel)
                            break;

                        if (huLevel != 0 && nextPackagingLevel > huLevel)
                            break;
                        hu = new HU(packaging, nextPackagingLevel, hu);
                        nextPackagingLevel++;
                    }
                }
                HU = hu;
            }
            huData.Succeded = true;
            return huData;
        }


    }

}