using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;
using System.Data;

namespace gip.bso.logistics
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Order BOM Plan'}de{'Auftrag Stückliste Plan'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class PickingPlanWrapper : INotifyPropertyChanged, IScheduledOrder
    {


        private static ACValueItemList _PickingStateList;
        [ACPropertyInfo(9999)]
        public static ACValueItemList PickingStateList
        {
            get
            {
                if (_PickingStateList == null)
                    _PickingStateList = new ACValueListPickingStateEnum();
                return _PickingStateList;
            }
        }

        public PickingPlanWrapper(Picking picking)
        {
            Picking = picking;
        }

        #region event

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        [ACPropertyInfo(1, "", "en{'Picking Order'}de{'Kommissionierauftrag'}")]
        public Picking Picking
        {
            get;
            set;
        }

        Material _Material;
        [ACPropertyInfo(2, "", ConstApp.Material)]
        public Material Material
        {
            get
            {
                if (_Material != null)
                    return _Material;
                if (   Picking == null
                    || !Picking.PickingPos_Picking.Any())
                {
                    return null;
                }
                _Material = Picking.PickingPos_Picking.FirstOrDefault().Material;
                return _Material;
            }
        }

        public MaterialUnit AltMaterialUnit
        {
            get
            {
                return Material != null ? Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).FirstOrDefault() : null;
            }
        }

        [ACPropertyInfo(3, "", "en{'Base UOM'}de{'Basiseinheit'}")]
        public MDUnit MDUnit
        {
            get
            {
                if (Material == null)
                    return null;
                return AltMaterialUnit != null ? AltMaterialUnit.ToMDUnit : Material.BaseMDUnit;
            }
        }

        [ACPropertyInfo(4, "", "en{'Alt. Unit of Measure'}de{'Alt. Einheit'}")]
        public MDUnit AltMDUnit
        {
            get
            {
                if (Material == null)
                    return null;
                return AltMaterialUnit != null ? AltMaterialUnit.ToMDUnit : Material.BaseMDUnit;
            }
        }

        #region Exposed Properties

        [ACPropertyInfo(999)]
        public EntityState EntityState
        {
            get
            {
                return Picking.EntityState;
            }
        }

        public Guid PickingID
        {
            get => Picking.PickingID;
            set { Picking.PickingID = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(1, "PickingNo", ConstApp.PickingNo)]
        public string PickingNo
        {
            get => Picking.PickingNo;
            set { Picking.PickingNo = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(2, "PickingTypeIndex", "en{'Picking Type'}de{'Kommissioniertyp'}")]
        public GlobalApp.PickingType PickingType
        {
            get => Picking.PickingType;
        }

        [ACPropertyInfo(3, "PickingState", "en{'Picking Status'}de{'Status'}", Const.ContextDatabase + "\\PickingStateList", true)]
        public PickingStateEnum PickingState
        {
            get => Picking.PickingState;
            set { Picking.PickingState = value; OnPropertyChanged(); }
        }


        [ACPropertyInfo(3, "PickingState", "en{'Picking Status'}de{'Status'}", Const.ContextDatabase + "\\PickingStateList", true)]
        public short PickingStateIndex
        {
            get => Picking.PickingStateIndex;
            set { Picking.PickingStateIndex = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(3, "", "en{'State'}de{'Status'}")]
        public string PickingIndexName
        {
            get
            {
                ACValueItem acValueItem = PickingStateList.FirstOrDefault(c => ((short)c.Value) == PickingStateIndex);
                return acValueItem.ACCaption;
            }
        }

        [ACPropertyInfo(4, "DeliveryDateFrom", "en{'Date from'}de{'Datum von'}", "", true)]
        public DateTime DeliveryDateFrom
        {
            get => Picking.DeliveryDateFrom;
            set { Picking.DeliveryDateFrom = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(5, "DeliveryDateTo", "en{'Date to'}de{'Datum bis'}", "", true)]
        public DateTime DeliveryDateTo
        {
            get => Picking.DeliveryDateTo;
            set { Picking.DeliveryDateTo = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(6, "Comment", ConstApp.Comment, "",  true)]
        public string Comment
        {
            get => Picking.Comment;
            set { Picking.Comment = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(7, "Comment2", ConstApp.Comment2, "", true)]
        public string Comment2
        {
            get => Picking.Comment2;
            set { Picking.Comment2 = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(8, VisitorVoucher.ClassName, "en{'Visitor Voucher'}de{'Besucherbeleg'}", Const.ContextDatabase + "\\" + VisitorVoucher.ClassName, true)]
        public VisitorVoucher VisitorVoucher
        {
            get => Picking.VisitorVoucher;
            set { Picking.VisitorVoucher = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(9, gip.core.datamodel.ACClassMethod.ClassName, "en{'Workflow'}de{'Workflow'}", Const.ContextDatabase + "\\" + gip.core.datamodel.ACClassMethod.ClassName, true)]
        public mes.datamodel.ACClassMethod ACClassMethod
        {
            get => Picking.ACClassMethod;
            set { Picking.ACClassMethod = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(10, MDPickingType.ClassName, "en{'Picking type'}de{'Kommissionierung Typ'}", Const.ContextDatabase + "\\" + MDPickingType.ClassName, true)]
        public MDPickingType MDPickingType
{
            get => Picking.MDPickingType;
            set { Picking.MDPickingType = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(11, "DeliveryCompanyAddress", "en{'Delivery Address'}de{'Lieferadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName, true)]
        public CompanyAddress DeliveryCompanyAddress
{
            get => Picking.DeliveryCompanyAddress;
            set { Picking.DeliveryCompanyAddress = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(12, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", true)]
        public string KeyOfExtSys
{
            get => Picking.KeyOfExtSys;
            set { Picking.KeyOfExtSys = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(17, "ScheduledStartDate", "en{'Planned Start Date'}de{'Geplante Startzeit'}", "", true)]
        public DateTime? ScheduledStartDate
        {
            get => Picking.ScheduledStartDate;
            set { Picking.ScheduledStartDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(18, "ScheduledEndDate", "en{'Planned End Date'}de{'Geplante Endezeit'}", "", true)]
        public DateTime? ScheduledEndDate
        {
            get => Picking.ScheduledEndDate;
            set { Picking.ScheduledEndDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(19, "CalculatedStartDate", "en{'Calculated Start Date'}de{'Berechnete Startzeit'}", "", true)]
        public DateTime? CalculatedStartDate
        {
            get => Picking.CalculatedStartDate;
            set { Picking.CalculatedStartDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(20, "CalculatedEndDate", "en{'Calculated End Date'}de{'Berechnete Endezeit'}", "", true)]
        public DateTime? CalculatedEndDate
        {
            get => Picking.CalculatedEndDate;
            set { Picking.CalculatedEndDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(10, "IsSelected", "en{'Select'}de{'Auswahl'}")]
        public bool IsSelected
        {
            get
            {
                return Picking.IsSelected;
            }
            set
            {
                Picking.IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        [ACPropertyInfo(16, "ScheduledOrder", "en{'Scheduled Order'}de{'Reihenfolge Plan'}")]
        public int? ScheduledOrder
        {
            get => Picking.ScheduledOrder;
            set { Picking.ScheduledOrder = value; OnPropertyChanged(); }
        }


        [ACPropertyInfo(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
        public DateTime InsertDate
        {
            get => Picking.InsertDate;
            set { Picking.InsertDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(497, Const.EntityInsertName, Const.EntityTransInsertName)]
        public string InsertName
        {
            get => Picking.InsertName;
            set { Picking.InsertName = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
        public DateTime UpdateDate
        {
            get => Picking.UpdateDate;
            set { Picking.UpdateDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
        public string UpdateName
        {
            get => Picking.UpdateName;
            set { Picking.UpdateName = value; OnPropertyChanged(); }
        }


        #endregion

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


