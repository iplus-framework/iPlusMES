﻿using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_System, ConstApp.ESUserSettings, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "TenantCompanyID", ConstApp.Company, Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(2, "InvoiceCompanyAddressID", ConstApp.BillingCompanyAddress, Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(3, "TenantCompanyID", ConstApp.IssuerCompanyPerson, Const.ContextDatabase + "\\" + CompanyPerson.ClassName, "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + UserSettings.ClassName, ConstApp.ESInOrderState, typeof(UserSettings), UserSettings.ClassName, "", Const.SortIndex)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<UserSettings>) })]
    public partial class UserSettings
    {
        public const string ClassName = "UserSettings";

        #region New/Delete
        public static UserSettings NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            UserSettings entity = new UserSettings();
            entity.UserSettingsID = Guid.NewGuid();
            if(parentACObject != null)
            {
                gip.core.datamodel.VBUser vBUser = parentACObject as gip.core.datamodel.VBUser;
                if(vBUser != null)
                {
                    VBUser mesVBUser = vBUser.FromAppContext<VBUser>(dbApp);
                    entity.VBUser = mesVBUser;
                }
            }
            entity.DefaultValuesACObject();
            return entity;
        }
        #endregion


    }
}