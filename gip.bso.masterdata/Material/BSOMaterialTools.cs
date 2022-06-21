using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;

namespace gip.bso.masterdata
{
    public partial class BSOMaterial
    {
        private MaterialWF _selectedMaterialWF;
        [ACPropertySelected(9999, MaterialWF.ClassName, "en{'Material workflow'}de{'Material workflow'}")]
        public MaterialWF SelectedMaterialWF
        {
            get
            {
                return _selectedMaterialWF;
            }
            set
            {
                if (_selectedMaterialWF != value)
                {
                    _selectedMaterialWF = value;
                    OnPropertyChanged();
                }
            }
        }


        [ACPropertyList(9999, MaterialWF.ClassName)]
        public IEnumerable<MaterialWF> MaterialWFList
        {
            get
            {
                return DatabaseApp.MaterialWF.OrderBy(x => x.Name);
            }
        }

        private Material _SelectedIntermediateProduct;
        [ACPropertySelected(9999, "IntermediateProduct", "en{'Intermediate product'}de{'Zwischenprodukt'}")]
        public Material SelectedIntermediateProduct
        {
            get => _SelectedIntermediateProduct;
            set
            {
                _SelectedIntermediateProduct = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyList(9999, "IntermediateProduct")]
        public IEnumerable<Material> IntermediateProductList
        {
            get
            {
                if (SelectedMaterialWF != null)
                {
                    return SelectedMaterialWF.MaterialWFRelation_MaterialWF.Select(c => c.TargetMaterial).Concat(SelectedMaterialWF.MaterialWFRelation_MaterialWF.Select(x => x.SourceMaterial)).Distinct();
                }
                else
                {
                    return DatabaseApp.Material.Where(c => c.IsIntermediate);
                }
            }
        }

        private Material _SelectedNewIntermediateProduct;
        [ACPropertySelected(9999, "IntermediateProduct", "en{'New intermediate product'}de{'Neue Zwischenprodukt'}")]
        public Material SelectedNewIntermediateProduct
        {
            get => _SelectedNewIntermediateProduct;
            set
            {
                _SelectedNewIntermediateProduct = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyList(9999, "IntermediateProduct")]
        public IEnumerable<Material> IntermediateNewProductList
        {
            get
            {
                return IntermediateProductList;
            }
        }

        public void ShowMaterialTools()
        {

        }

        public bool IsEnabledShowMaterialTools()
        {
            return true;
        }


        public void ReplaceComponent()
        {

        }

        public void SwitchComponentToAnotherIntermediate()
        {

        }

        public void SplitComponentToAnotherIntermediate()
        {

        }
    }
}
