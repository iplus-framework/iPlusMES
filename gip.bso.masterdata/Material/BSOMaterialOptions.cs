using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using System.Collections.ObjectModel;

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
                    if (value != null)
                    {
                        IntermediateProductsList.Clear();
                        foreach (Material mat in value.MaterialWFRelation_MaterialWF.Select(c => c.TargetMaterial)
                                                               .Concat(SelectedMaterialWF.MaterialWFRelation_MaterialWF.Select(x => x.SourceMaterial)).Distinct())
                        {
                            IntermediateProductsList.Add(mat);
                        }

                        OnPropertyChanged(nameof(SelectedIntermediateProduct));
                    }
                    else
                    {
                        IntermediateProductsList.Clear();
                        foreach (Material mat in DatabaseApp.Material.Where(c => c.IsIntermediate))
                        {
                            IntermediateProductsList.Add(mat);
                        }

                    }
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

        private ObservableCollection<Material> _IntermediateProductsList;

        [ACPropertyList(9999, "IntermediateProduct")]
        public ObservableCollection<Material> IntermediateProductsList
        {
            get
            {
                return _IntermediateProductsList;
            }
            set
            {
                _IntermediateProductsList = value;
                OnPropertyChanged();
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
                return IntermediateProductsList;
            }
        }

        [ACMethodInfo("", "en{'Show material options'}de{'Materialoptionen anzeigen'}", 9999, true)]
        public void ShowMaterialOptions()
        {

        }

        public bool IsEnabledShowMaterialOptions()
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
