using gip.core.datamodel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace gip.mes.datamodel
{
    public class InputQForActual : INotifyPropertyChanged
    {

        #region Properties

        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForActualOutput;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForActualOutput)]
        public double InputQForActualOutput
        {
            get
            {
                return _InputQForActualOutput;
            }
            set
            {
                if (_InputQForActualOutput != value)
                {
                    _InputQForActualOutput = value;
                    OnPropertyChanged(nameof(InputQForActualOutput));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForActualOutputDiff;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForActualOutputDiff)]
        public double InputQForActualOutputDiff
        {
            get
            {
                return _InputQForActualOutputDiff;
            }
            set
            {
                if (_InputQForActualOutputDiff != value)
                {
                    _InputQForActualOutputDiff = value;
                    OnPropertyChanged(nameof(InputQForActualOutputDiff));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForActualOutputPer;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForActualOutputPer)]
        public double InputQForActualOutputPer
        {
            get
            {
                return _InputQForActualOutputPer;
            }
            set
            {
                if (_InputQForActualOutputPer != value)
                {
                    _InputQForActualOutputPer = value;
                    OnPropertyChanged(nameof(InputQForActualOutputPer));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForGoodActualOutput;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForGoodActualOutput)]
        public double InputQForGoodActualOutput
        {
            get
            {
                return _InputQForGoodActualOutput;
            }
            set
            {
                if (_InputQForGoodActualOutput != value)
                {
                    _InputQForGoodActualOutput = value;
                    OnPropertyChanged(nameof(InputQForGoodActualOutput));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForGoodActualOutputDiff;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForGoodActualOutputDiff)]
        public double InputQForGoodActualOutputDiff
        {
            get
            {
                return _InputQForGoodActualOutputDiff;
            }
            set
            {
                if (_InputQForGoodActualOutputDiff != value)
                {
                    _InputQForGoodActualOutputDiff = value;
                    OnPropertyChanged(nameof(InputQForGoodActualOutputDiff));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForGoodActualOutputPer;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForGoodActualOutputPer)]
        public double InputQForGoodActualOutputPer
        {
            get
            {
                return _InputQForGoodActualOutputPer;
            }
            set
            {
                if (_InputQForGoodActualOutputPer != value)
                {
                    _InputQForGoodActualOutputPer = value;
                    OnPropertyChanged(nameof(InputQForGoodActualOutputPer));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForScrapActualOutput;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForScrapActualOutput)]
        public double InputQForScrapActualOutput
        {
            get
            {
                return _InputQForScrapActualOutput;
            }
            set
            {
                if (_InputQForScrapActualOutput != value)
                {
                    _InputQForScrapActualOutput = value;
                    OnPropertyChanged(nameof(InputQForScrapActualOutput));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForScrapActualOutputDiff;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForScrapActualOutputDiff)]
        public double InputQForScrapActualOutputDiff
        {
            get
            {
                return _InputQForScrapActualOutputDiff;
            }
            set
            {
                if (_InputQForScrapActualOutputDiff != value)
                {
                    _InputQForScrapActualOutputDiff = value;
                    OnPropertyChanged(nameof(InputQForScrapActualOutputDiff));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForScrapActualOutputPer;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForScrapActualOutputPer)]
        public double InputQForScrapActualOutputPer
        {
            get
            {
                return _InputQForScrapActualOutputPer;
            }
            set
            {
                if (_InputQForScrapActualOutputPer != value)
                {
                    _InputQForScrapActualOutputPer = value;
                    OnPropertyChanged(nameof(InputQForScrapActualOutputPer));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForFinalActualOutput;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForFinalActualOutput)]
        public double InputQForFinalActualOutput
        {
            get
            {
                return _InputQForFinalActualOutput;
            }
            set
            {
                if (_InputQForFinalActualOutput != value)
                {
                    _InputQForFinalActualOutput = value;
                    OnPropertyChanged(nameof(InputQForFinalActualOutput));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForFinalActualOutputDiff;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForFinalActualOutputDiff)]
        public double InputQForFinalActualOutputDiff
        {
            get
            {
                return _InputQForFinalActualOutputDiff;
            }
            set
            {
                if (_InputQForFinalActualOutputDiff != value)
                {
                    _InputQForFinalActualOutputDiff = value;
                    OnPropertyChanged(nameof(InputQForFinalActualOutputDiff));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForFinalActualOutputPer;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForFinalActualOutputPer)]
        public double InputQForFinalActualOutputPer
        {
            get
            {
                return _InputQForFinalActualOutputPer;
            }
            set
            {
                if (_InputQForFinalActualOutputPer != value)
                {
                    _InputQForFinalActualOutputPer = value;
                    OnPropertyChanged(nameof(InputQForFinalActualOutputPer));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForFinalGoodActualOutput;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForFinalGoodActualOutput)]
        public double InputQForFinalGoodActualOutput
        {
            get
            {
                return _InputQForFinalGoodActualOutput;
            }
            set
            {
                if (_InputQForFinalGoodActualOutput != value)
                {
                    _InputQForFinalGoodActualOutput = value;
                    OnPropertyChanged(nameof(InputQForFinalGoodActualOutput));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForFinalGoodActualOutputDiff;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForFinalGoodActualOutputDiff)]
        public double InputQForFinalGoodActualOutputDiff
        {
            get
            {
                return _InputQForFinalGoodActualOutputDiff;
            }
            set
            {
                if (_InputQForFinalGoodActualOutputDiff != value)
                {
                    _InputQForFinalGoodActualOutputDiff = value;
                    OnPropertyChanged(nameof(InputQForFinalGoodActualOutputDiff));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForFinalGoodActualOutputPer;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForFinalGoodActualOutputPer)]
        public double InputQForFinalGoodActualOutputPer
        {
            get
            {
                return _InputQForFinalGoodActualOutputPer;
            }
            set
            {
                if (_InputQForFinalGoodActualOutputPer != value)
                {
                    _InputQForFinalGoodActualOutputPer = value;
                    OnPropertyChanged(nameof(InputQForFinalGoodActualOutputPer));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForFinalScrapActualOutput;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForFinalScrapActualOutput)]
        public double InputQForFinalScrapActualOutput
        {
            get
            {
                return _InputQForFinalScrapActualOutput;
            }
            set
            {
                if (_InputQForFinalScrapActualOutput != value)
                {
                    _InputQForFinalScrapActualOutput = value;
                    OnPropertyChanged(nameof(InputQForFinalScrapActualOutput));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForFinalScrapActualOutputDiff;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForFinalScrapActualOutputDiff)]
        public double InputQForFinalScrapActualOutputDiff
        {
            get
            {
                return _InputQForFinalScrapActualOutputDiff;
            }
            set
            {
                if (_InputQForFinalScrapActualOutputDiff != value)
                {
                    _InputQForFinalScrapActualOutputDiff = value;
                    OnPropertyChanged(nameof(InputQForFinalScrapActualOutputDiff));
                }
            }
        }
        /// <summary>
        /// Source Property: 
        /// </summary>
        private double _InputQForFinalScrapActualOutputPer;
        [ACPropertyInfo(999, "", ConstIInputQForActual.InputQForFinalScrapActualOutputPer)]
        public double InputQForFinalScrapActualOutputPer
        {
            get
            {
                return _InputQForFinalScrapActualOutputPer;
            }
            set
            {
                if (_InputQForFinalScrapActualOutputPer != value)
                {
                    _InputQForFinalScrapActualOutputPer = value;
                    OnPropertyChanged(nameof(InputQForFinalScrapActualOutputPer));
                }
            }
        }

        #endregion


        #region Property-Changed
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
