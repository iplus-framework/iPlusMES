using gip.core.datamodel;
using System.Collections.Generic;

namespace gip.mes.datamodel
{
    public interface IOutOrderPosBSO: IACComponent
    {
        MDUnit CurrentMDUnit { get; set; }

        void OnPricePropertyChanged();

        List<PriceListMaterial> PriceListMaterialItems { get; set; }
        PriceListMaterial SelectedPriceListMaterial { get; set; }

    }
}
