using System;

namespace gip.bso.manufacturing
{
    public class OperationLogParam
    {
        public Guid OperationLogID { get; set; }

        #region Method and Parameter information
        public string ParameterName { get; set; }

        public bool IsParam { get;set;}
        public bool IsResult { get;set;}

        public string ValueStr { get; set; }
        public double? ValueDouble { get; set; }
        public int? ValueInt { get; set; }
        #endregion
    }
}
