using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    public class WorkCenterItemFunction : INotifyPropertyChanged
    {
        public WorkCenterItemFunction(ACComponent parentProcessModule, string PAFACIdentifier, BSOWorkCenterSelector bso, ACComposition[] bsos)
        {
            ACComponent paf = parentProcessModule.ACUrlCommand(PAFACIdentifier) as ACComponent;
            if (paf != null)
            {
                _ProcessFunction = new ACRef<ACComponent>(paf, bso);
                ACStateProperty = ProcessFunction.GetPropertyNet("ACState") as IACContainerTNet<ACStateEnum>;
                NeedWorkProperty = ProcessFunction.GetPropertyNet("NeedWork") as IACContainerTNet<bool>;
                RelatedBSOs = bsos;

                if (RelatedBSOs.Any(c => BSOWorkCenterMessages._WCSMessagesType.IsAssignableFrom((c.ValueT as core.datamodel.ACClass)?.ObjectType)))
                {
                    IsReponsibleForUserAck = true;
                }
            }
        }

        private bool _IsFunctionActive;
        public bool IsFunctionActive
        {
            get
            {
                return _IsFunctionActive;
            }
            private set
            {
                _IsFunctionActive = value;
                OnPropertyChanged("IsFunctionActive");
            }
        }

        private bool _IsReponsibleForUserAck = false;
        public bool IsReponsibleForUserAck
        {
            get => _IsReponsibleForUserAck;
            private set
            {
                _IsReponsibleForUserAck = value;
            }
        }

        public ACComposition[] RelatedBSOs
        {
            get;
            private set;
        }

        internal ACRef<ACComponent> _ProcessFunction;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ACComponent ProcessFunction
        {
            get => _ProcessFunction?.ValueT;
        }

        public IACContainerTNet<ACStateEnum> ACStateProperty
        {
            get;
            set;
        }

        public IACContainerTNet<bool> NeedWorkProperty
        {
            get;
            set;
        }

        public bool CheckIsFunctionActive()
        {
            return (ACStateProperty != null && ACStateProperty.ValueT == ACStateEnum.SMRunning || ACStateProperty.ValueT == ACStateEnum.SMStarting)
                || (NeedWorkProperty != null && NeedWorkProperty.ValueT);
        }

        public bool SetIsFunctionActive()
        {
            IsFunctionActive = CheckIsFunctionActive();
            return IsFunctionActive;
        }

        public void SetFunctionActive(bool anyUserAckNodesActive)
        {
            IsFunctionActive = anyUserAckNodesActive;
        }
    }
}
