using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{
    public interface IPAFuncScaleConfig : IACComponentProcessFunction
    {
        string FuncScaleConfig { get; }

        PAEScaleBase CurrentScaleForWeighing { get; }

        PAScaleMappingHelper<PAEScaleBase> ScaleMappingHelper { get; }
    }

    public class PAScaleMappingHelper<T> where T : IACComponent
    {
        public const string FuncScaleConfigName = "FuncScaleConfig";

        public PAScaleMappingHelper(IACComponent parentComp4Attach, IACComponent pafFunction)
        {
            _ParentComp4Attach = parentComp4Attach;
            if (pafFunction != _ParentComp4Attach)
                _PAProcessFunction = new ACRef<IACComponent>(pafFunction, parentComp4Attach);

            IPAFuncScaleConfig scaleConfig = pafFunction as IPAFuncScaleConfig;
            if (scaleConfig != null)
                AssignScales(scaleConfig.FuncScaleConfig, pafFunction);
            else
            {
                string configValue = pafFunction.ComponentClass[FuncScaleConfigName] as string;
                if (configValue != null)
                    AssignScales(configValue, pafFunction);
            }
        }

        private IACComponent _ParentComp4Attach = null;
        private ACRef<IACComponent> _PAProcessFunction = null;
        public IACComponent PAProcessFunction
        {
            get
            {
                if (_PAProcessFunction != null)
                    return _PAProcessFunction.ValueT;
                return _ParentComp4Attach;
            }
        }

        private void AssignScales(string scaleConfig, IACComponent pafFunction)
        {
            if (String.IsNullOrEmpty(scaleConfig))
                return;
            string[] scaleACUrls = scaleConfig.Split(';');
            foreach (string scaleACUrl in scaleACUrls)
            {
                string acUrl = scaleACUrl.Trim();
                if (string.IsNullOrEmpty(acUrl))
                    continue;

                object scale = pafFunction.ACUrlCommand(acUrl);
                
                if (scale != null && scale is T)
                {
                    _AssignedScales.Add(new ACRef<T>((T)scale, _ParentComp4Attach));
                }
            }
        }

        List<ACRef<T>> _AssignedScales = new List<ACRef<T>>();
        public IEnumerable<T> AssignedScales
        {
            get
            {
                return _AssignedScales.Select(c => c.ValueT);
            }
        }

        public void DetachAndRemove()
        {
            _AssignedScales.ForEach(c => c.Detach());
            _AssignedScales = new List<ACRef<T>>();
            if (_PAProcessFunction != null)
            {
                _PAProcessFunction.Detach();
                _PAProcessFunction = null;
            }
        }
    }
}
