using System;

namespace gip.mes.cmdlet.DBSync
{
    public class DbSyncerInfo
    {
        public int DbSyncerInfoID { get; set; }
        public string DbSyncerInfoContextID { get; set; }
        public DateTime ScriptDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateAuthor { get; set; }
    }
}
