using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility.TandTv3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.facility
{
    public class TandTGraphModel
    {

        public TandTGraphModel()
        {
            GraphEdges = new List<TandTEdge>();
            JobID = Guid.NewGuid();
            RegistredRelations = new List<MixPointRelation>();
        }

        public Guid JobID { get; private set; }

        public List<TandTEdge> GraphEdges { get; set; }

        public List<IACObject> ActiveGraphComponents
        {
            get
            {
                return AllGeneratedTandTPointPresenterComponents.Where(c => c.JobIds.Contains(JobID)).Select(c => c as IACObject).ToList();
            }
        }

        public List<TandTPointPresenter> AllGeneratedTandTPointPresenterComponents { get; set; }

        public gip.core.datamodel.ACClass TandTPointPresenterClass { get; set; }
        public gip.core.datamodel.ACClass DummyClass { get; set; }

        public List<MixPointRelation> RegistredRelations { get; set; }


        public Exception Error { get; set; }
        public bool Success { get; set; }

        public Msg GetDetailedMessage()
        {
            if (Error == null) return null;
            return new Msg()
            {
                MessageLevel = eMsgLevel.Error,
                Message = string.Format(@"Graph generaring error! Message:{0}", Error.Message)
            };
        }
    }
}
