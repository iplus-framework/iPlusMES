﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "", Global.ACKinds.TACSimpleClass)]
    public class OperationLogGroupItem : EntityBase, IACObject
    {
        [ACPropertyInfo(9999)]
        public Material Material
        { 
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public FacilityLot Lot
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public double StockQuantity
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public MDUnit Unit
        {
            get;
            set;
        }

        [ACPropertyInfo(101, "", ConstApp.StockUnitA)]
        public double StockUnitA { get; set; }

        [ACPropertyInfo(102, "", ConstApp.UnitOfStockA)]
        public string UnitA { get; set; }

        [ACPropertyInfo(103, "", ConstApp.StockUnitB)]
        public double StockUnitB { get; set; }

        [ACPropertyInfo(104, "", ConstApp.UnitOfStockB)]
        public string UnitB { get; set; }

        [ACPropertyInfo(105, "", ConstApp.StockUnitC)]
        public double StockUnitC { get; set; }

        [ACPropertyInfo(106, "", ConstApp.UnitOfStockC)]
        public string UnitC { get; set; }

        public IACObject ParentACObject => null;

        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        public string ACIdentifier => this.ReflectGetACIdentifier();

        public string ACCaption => "";

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        public bool ACUrlTypeInfo(string acUrl, ref ACUrlTypeInfo acUrlTypeInfo)
        {
            return this.ReflectACUrlTypeInfo(acUrl, ref acUrlTypeInfo);
        }

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }
    }
}
