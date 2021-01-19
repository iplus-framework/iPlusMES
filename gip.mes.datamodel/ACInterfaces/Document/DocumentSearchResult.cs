using System.Collections.Generic;
using System.IO;

namespace gip.mes.datamodel
{
    public class DocumentSearchResult
    {
        public DoumentFilter Filter { get; set; }

        public int ItemsCount { get; set; }
        public List<FileInfo> FileInfos { get; set; }
    }
}
