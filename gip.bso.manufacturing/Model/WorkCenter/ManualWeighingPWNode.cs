using gip.core.autocomponent;
using gip.core.datamodel;

namespace gip.bso.manufacturing
{
    public class ManualWeighingPWNode
    {
        public ManualWeighingPWNode(ACRef<IACComponentPWNode> componentPWNode)
        {
            ComponentPWNode = componentPWNode;
        }

        private ACRef<IACComponentPWNode> _ComponentPWNode;
        public ACRef<IACComponentPWNode> ComponentPWNode
        {
            get => _ComponentPWNode;
            set
            {
                if (value != null)
                {
                    ComponentPWNodeACState = value.ValueT.GetPropertyNet(Const.ACState) as IACContainerTNet<ACStateEnum>;
                }
                _ComponentPWNode = value;
            }
        }

        public IACContainerTNet<ACStateEnum> ComponentPWNodeACState
        {
            get;
            set;
        }

        public void Deinit()
        {
            ComponentPWNodeACState = null;
            ComponentPWNode?.Detach();
            ComponentPWNode = null;
        }

    }
}
