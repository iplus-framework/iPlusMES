using gip.core.datamodel;
using System;
using System.Security.Cryptography.X509Certificates;

namespace gip.bso.sales
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'CertifcatePreview'}de{'CertifcatePreview'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class CertifcatePreview
    {
        [ACPropertyInfo(1, nameof(Subject), "en{'Subject'}de{'Sn'}")]
        public string Subject { get; set; }

        [ACPropertyInfo(2, nameof(Issuer), "en{'Issuer'}de{'Emittent'}")]
        public string Issuer { get; set; }

        [ACPropertyInfo(3, nameof(NotBefore), "en{'Not before '}de{'Nicht vorher'}")]
        public DateTime NotBefore { get; set; }

        [ACPropertyInfo(4, nameof(NotAfter), "en{'Not after'}de{'Nicht nach'}")]
        public DateTime NotAfter { get; set; }

        [ACPropertyInfo(5, nameof(HasPrivateKey), "en{'Has Private Key'}de{'Hat einen privaten Schlüssel'}")]
        public bool HasPrivateKey { get; set; }

        public X509Certificate2 Certificate { get; set; }
    }
}
