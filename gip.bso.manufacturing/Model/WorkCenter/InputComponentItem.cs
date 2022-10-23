using System.Runtime.CompilerServices;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Input component item'}de{'Input component item'}", Global.ACKinds.TACSimpleClass)]
    public class InputComponentItem : INotifyPropertyChanged
    {
        public InputComponentItem(ProdOrderPartslistPosRelation relation)
        {
            ProdOrderPartslistPos sourcePos = relation.SourceProdOrderPartslistPos;
            MaterialNo = sourcePos.Material.MaterialNo;
            MaterialName = sourcePos.Material.MaterialName1;
            TargetQuantityUOM = relation.TargetQuantityUOM;
            TargetQuantity = relation.TargetQuantity;
            ActualQuantityUOM = relation.ActualQuantityUOM;
            ActualQuantity = relation.ActualQuantity;
            DifferenceQuantityUOM = relation.DifferenceQuantityUOM;
            DifferenceQuantity = relation.DifferenceQuantity;
            State = relation.MDProdOrderPartslistPosState;
        }

        public InputComponentItem(PickingPos pickingPos)
        {
            if (pickingPos.Material != null)
            {
                MaterialNo = pickingPos.Material.MaterialNo;
                MaterialName = pickingPos.Material.MaterialName1;
            }

            TargetQuantityUOM = pickingPos.TargetQuantity;
            ActualQuantityUOM = pickingPos.ActualQuantity;
            DifferenceQuantityUOM = pickingPos.DiffQuantityUOM;

            if (pickingPos.MDDelivPosLoadState != null && pickingPos.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck)
            {
                State = new MDProdOrderPartslistPosState() { MDProdOrderPartslistPosStateIndex = (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed };
            }
            else
            {
                State = new MDProdOrderPartslistPosState() { MDProdOrderPartslistPosStateIndex = (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created };
            }

        }

        [ACPropertyInfo(100, "", ConstApp.MaterialNo)]
        public string MaterialNo
        {
            get;
            set;
        }

        [ACPropertyInfo(100, "", ConstApp.MaterialName1)]
        public string MaterialName
        {

            get;
            set;
        }

        private double _TargetQuantityUOM;
        [ACPropertyInfo(100, "", "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}")]
        public double TargetQuantityUOM
        {
            get => _TargetQuantityUOM;
            set
            {
                _TargetQuantityUOM = value;
                OnPropertyChanged("TargetQuantityUOM");
            }
        }

        private double _TargetQuantity;
        [ACPropertyInfo(100, "", "en{'Target Quantity'}de{'Sollmenge'}")]
        public double TargetQuantity
        {
            get => _TargetQuantity;
            set
            {
                _TargetQuantity = value;
                OnPropertyChanged();
            }
        }

        private double _ActualQuantityUOM;
        [ACPropertyInfo(100, "", "en{'Actual Quantity (UOM)'}de{'Istmenge (BME)'}")]
        public double ActualQuantityUOM
        {
            get => _ActualQuantityUOM;
            set
            {
                _ActualQuantityUOM = value;
                OnPropertyChanged("ActualQuantityUOM");
            }
        }

        private double _ActualQuantity;
        [ACPropertyInfo(100, "", "en{'Actual Quantity'}de{'Istmenge'}")]
        public double ActualQuantity
        {
            get => _ActualQuantity;
            set
            {
                _ActualQuantity = value;
                OnPropertyChanged();
            }
        }

        private double _DifferenceQuantityUOM;
        [ACPropertyInfo(100, "", "en{'Difference Quantity UOM'}de{'Differenzmenge (BME)'}")]
        public double DifferenceQuantityUOM
        {
            get => _DifferenceQuantityUOM;
            set
            {
                _DifferenceQuantityUOM = value;
                OnPropertyChanged("DifferenceQuantityUOM");
            }
        }

        private double _DifferenceQuantity;
        [ACPropertyInfo(100, "", "en{'Difference Quantity'}de{'Differenzmenge'}")]
        public double DifferenceQuantity
        {
            get => _DifferenceQuantity;
            set
            {
                _DifferenceQuantity = value;
                OnPropertyChanged();
            }
        }

        private MDProdOrderPartslistPosState _State;
        [ACPropertyInfo(100, "", "en{'State'}de{'Status'}")]
        public MDProdOrderPartslistPosState State
        {
            get => _State;
            set
            {
                _State = value;
                OnPropertyChanged("State");
            }
        }

        private string _AdditionalParam1;
        [ACPropertyInfo(100, "", "en{'Param1'}de{'Param1'}")]
        public string AdditionalParam1
        {
            get => _AdditionalParam1;
            set
            {
                _AdditionalParam1 = value;
                OnPropertyChanged("AdditionalParam1");
            }
        }

        private string _AdditionalParam2;
        [ACPropertyInfo(100, "", "en{'Param2'}de{'Param2'}")]
        public string AdditionalParam2
        {
            get => _AdditionalParam2;
            set
            {
                _AdditionalParam2 = value;
                OnPropertyChanged("AdditionalParam2");
            }
        }

        private string _AdditionalParam3;
        [ACPropertyInfo(100, "", "en{'Param3'}de{'Param3'}")]
        public string AdditionalParam3
        {
            get => _AdditionalParam3;
            set
            {
                _AdditionalParam3 = value;
                OnPropertyChanged("AdditionalParam3");
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
