using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    public interface IFactoryTandTPointPresenterComponent
    {
        List<TandTPointPresenter> AllGeneratedTandTPointPresenterComponents { get; set; }
        TandTPointPresenter FactoryComponent(Guid jobID, MDTrackingStartItemTypeEnum itemType, TandTv3Point mixPoint, Guid? representMixPointID, Guid acObjectID, IACObject content);
    }
}
