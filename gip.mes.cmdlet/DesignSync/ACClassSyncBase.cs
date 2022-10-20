using gip.bso.iplus;
using gip.core.datamodel;

namespace gip.mes.cmdlet.DesignSync
{
    public delegate void ImportMessage(string msg);

    public class ACClassSyncBase
    {
        public event ImportMessage OnImportMessage;

        public string RootFolder { get; set; }
        public ACProjectManager ProjectManager { get; set; }
        public Database Database { get; set; }

        public void SendImportMessage(string msg)
        {
            if (OnImportMessage != null)
                OnImportMessage(msg);
        }
    }
}
