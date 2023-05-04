using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Number provider for Invoice numbers'}de{'Nummern generator für Rechnungen'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class ACNoProviderInvoice : PARole, IACVBNoProvider
    {
        #region c´tors
        public ACNoProviderInvoice(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        #endregion

        #region Public Methods
        public string GetNewNo(IACVBNoManager parentManager, Database iplusContext, IACEntityObjectContext appContext, 
                                Type type, string entityNoFieldName, string formatNewNo, core.datamodel.VBNoConfiguration vbNoConfiguration, IACComponent invoker = null)
        {
            DatabaseApp dbApp = appContext as DatabaseApp;
            if (dbApp == null)
                return (parentManager as ACVBNoManager).GetNextNo(vbNoConfiguration, formatNewNo);

            UserSettings userSettings = dbApp.UserSettings
                                        .Include(c => c.InvoiceCompanyAddress)
                                        .Include(c => c.InvoiceCompanyAddress.MDCountry)
                                        .FirstOrDefault(c => c.VBUserID == Root.Environment.User.VBUserID);
            if (userSettings != null 
                && userSettings.InvoiceCompanyAddress != null 
                && userSettings.InvoiceCompanyAddress.MDCountry != null)
            {
                if (userSettings.InvoiceCompanyAddress.MDCountry.MDKey.StartsWith("HR"))
                {
                    DateTime now = DateTime.Now;
                    DateTime from = new DateTime(now.Year, 1, 1);
                    DateTime to = new DateTime(now.Year, 12, 31);
                    string maxInvoiceNo = dbApp.Invoice
                        .Where(c => c.InvoiceDate > from && c.InvoiceDate < to)
                        .OrderByDescending(c => c.InsertDate)
                        .Select(c => c.InvoiceNo)
                        .FirstOrDefault();
                    int lastInvoiceNoThisYear = 0;
                    if (!String.IsNullOrEmpty(maxInvoiceNo))
                    {
                        string[] parts = maxInvoiceNo.Split('/');
                        if (parts != null && parts.Count() == 4)
                        {
                            if (!String.IsNullOrEmpty(parts[1]))
                            {
                                if (!Int32.TryParse(parts[1], out lastInvoiceNoThisYear))
                                    lastInvoiceNoThisYear = 0;
                            }
                        }
                    }
                    lastInvoiceNoThisYear++;
                    // Invoice-No in Croatia  "Unique Sequence number per year" + "/" + "Id of selling point" + "/" + "Id of billing device"
                    return String.Format("{0}/{1}/{2}/{3}", now.Year, lastInvoiceNoThisYear, "1", "1");
                }
            }
            return (parentManager as ACVBNoManager).GetNextNo(vbNoConfiguration, formatNewNo);
        }

        private readonly Type _InvoiceType = typeof(Invoice);
        public bool IsHandlerForType(Type type, string entityNoFieldName)
        {
            return type == _InvoiceType;
        }

        #endregion

    }
}


