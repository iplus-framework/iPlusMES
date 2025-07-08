using gip.core.datamodel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace gip.mes.datamodel
{

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Demands from store stats'}de{'Bedarfe nach Lagerstatistik'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class ConsumptionModel : INotifyPropertyChanged
    {
        #region ctor's
        public ConsumptionModel()
        {
            RecipeSourceList = new List<Partslist>();
            OutOrderPosList = new List<OutOrderPos>();
            ComponentList = new List<ProdOrderPartslistPos>();
            IsSelected = false;
        }
        #endregion

        #region Consumption 

        /// <summary>
        /// Source Property: 
        /// </summary>
        private PlanningMRCons _PlanningMRCons;
        [ACPropertyInfo(999, nameof(PlanningMRCons), "en{'TODO:Consumption'}de{'TODO:Consumption'}")]
        public PlanningMRCons PlanningMRCons
        {
            get
            {
                return _PlanningMRCons;
            }
            set
            {
                if (_PlanningMRCons != value)
                {
                    _PlanningMRCons = value;
                }
            }
        }


        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _MRPProcedure;
        [ACPropertyInfo(999, nameof(MRPProcedure), ConstApp.MRPProcedure)]
        public string MRPProcedure
        {
            get
            {
                return _MRPProcedure;
            }
            set
            {
                if (_MRPProcedure != value)
                {
                    _MRPProcedure = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region RecipeSource

        private Partslist _SelectedRecipeSource;
        /// <summary>
        /// Selected property for Partslist
        /// </summary>
        /// <value>The selected RecipeSource</value>
        [ACPropertySelected(9999, "RecipeSource", "en{'TODO: RecipeSource'}de{'TODO: RecipeSource'}")]
        public Partslist SelectedRecipeSource
        {
            get
            {
                return _SelectedRecipeSource;
            }
            set
            {
                if (_SelectedRecipeSource != value)
                {
                    _SelectedRecipeSource = value;
                    PlanningMRCons.DefaultPartslist = value;
                    OnPropertyChanged(nameof(SelectedRecipeSource));
                }
            }
        }

        private List<Partslist> _RecipeSourceList;
        /// <summary>
        /// List property for Partslist
        /// </summary>
        /// <value>The RecipeSource list</value>
        [ACPropertyList(9999, "RecipeSource")]
        public List<Partslist> RecipeSourceList
        {
            get
            {
                return _RecipeSourceList;
            }
            set
            {
                _RecipeSourceList = value;
                OnPropertyChanged();
            }
        }

        private bool _IsSelected;
        [ACPropertyInfo(999, "IsSelected", "en{'Selected'}de{'Ausgewählt'}")]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion

        #region OutOrderPos

        private OutOrderPos _SelectedOutOrderPos;
        /// <summary>
        /// Selected property for OutOrderPos
        /// </summary>
        /// <value>The selected OutOrderPos</value>
        [ACPropertySelected(9999, nameof(OutOrderPos), "en{'TODO: OutOrderPos'}de{'TODO: OutOrderPos'}")]
        public OutOrderPos SelectedOutOrderPos
        {
            get
            {
                return _SelectedOutOrderPos;
            }
            set
            {
                if (_SelectedOutOrderPos != value)
                {
                    _SelectedOutOrderPos = value;
                    OnPropertyChanged(nameof(SelectedOutOrderPos));
                }
            }
        }


        private List<OutOrderPos> _OutOrderPosList;
        /// <summary>
        /// List property for OutOrderPos
        /// </summary>
        /// <value>The OutOrderPos list</value>
        [ACPropertyList(9999, nameof(OutOrderPos))]
        public List<OutOrderPos> OutOrderPosList
        {
            get
            {
                return _OutOrderPosList;
            }
            set
            {
                _OutOrderPosList = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Component

        private ProdOrderPartslistPos _SelectedComponent;
        /// <summary>
        /// Selected property for ProdOrderPartslistPos
        /// </summary>
        /// <value>The selected Component</value>
        [ACPropertySelected(9999, "PropertyGroupName", "en{'TODO: Component'}de{'TODO: Component'}")]
        public ProdOrderPartslistPos SelectedComponent
        {
            get
            {
                return _SelectedComponent;
            }
            set
            {
                if (_SelectedComponent != value)
                {
                    _SelectedComponent = value;
                    OnPropertyChanged(nameof(SelectedComponent));
                }
            }
        }


        private List<ProdOrderPartslistPos> _ComponentList;
        /// <summary>
        /// List property for ProdOrderPartslistPos
        /// </summary>
        /// <value>The Component list</value>
        [ACPropertyList(9999, "PropertyGroupName")]
        public List<ProdOrderPartslistPos> ComponentList
        {
            get
            {
                return _ComponentList;
            }
            set
            {
                _ComponentList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the INotifyPropertyChanged.PropertyChanged-Event.
        /// </summary>
        /// <param name="name">Name of the property</param>
        [ACMethodInfo("ACComponent", "en{'PropertyChanged'}de{'PropertyChanged'}", 9999)]
        public virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
