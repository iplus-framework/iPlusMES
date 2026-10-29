using System;

namespace gip.mes.cmdlet.Barcode.Model
{
    public class BarcodeResult
    {
        public Guid ID { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string ACUrl { get; set; }
    }
}
