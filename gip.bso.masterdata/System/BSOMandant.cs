using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Mandant'}de{'Mandant'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + VBUser.ClassName)]
    public class BSOMandant : ACBSOvb
    {

          #region c'tors

        public BSOMandant(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACIntit =  base.ACInit(startChildMode);
            Mandant = GetMandant(Root.Environment.User);
            Operator = GetOperator(Root.Environment.User);
            return baseACIntit;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Mandandt & Operator properties

        private CompanyAddress _Mandant;
        [ACPropertyInfo(1, "Mandant", "en{'Mandant'}de{'Mandant'}")]
        public CompanyAddress Mandant
        {
            get
            {
                return _Mandant;
            }
            set
            {
                if(_Mandant != value)
                {
                    _Mandant = value;
                    OnPropertyChanged("Mandant");
                }
            }
        }


        private CompanyPerson _Operator;
        [ACPropertyInfo(2, "Operator", "en{'Operator'}de{'Operator'}")]
        public CompanyPerson Operator
        {
            get
            {
                return _Operator;
            }
            set
            {
                if(_Operator != value)
                {
                    _Operator = value;
                    OnPropertyChanged("Operator");
                }
            }
        }

        #endregion

        #region Mandant & Operator Methods

        public void SetMandant(gip.core.datamodel.VBUser vBUser, CompanyAddress companyAddress)
        {
            CompanyAddress otherMandant = GetMandant(vBUser);
            if(otherMandant != null && otherMandant.CompanyAddressID != companyAddress.CompanyAddressID)
                otherMandant.VBUser = null;
            Mandant = companyAddress;
            gip.mes.datamodel.VBUser mesVBUser = vBUser.FromAppContext<gip.mes.datamodel.VBUser>(DatabaseApp);
            companyAddress.VBUser = mesVBUser;
             Mandant = companyAddress;
        }

        public void SetOperator(gip.core.datamodel.VBUser vBUser, CompanyPerson companyPerson)
        {
            CompanyPerson otherOperater = GetOperator(vBUser);
            if(otherOperater != null && otherOperater.CompanyPersonID != otherOperater.CompanyPersonID)
                otherOperater.VBUser = null;
            gip.mes.datamodel.VBUser mesVBUser = vBUser.FromAppContext<gip.mes.datamodel.VBUser>(DatabaseApp);
            companyPerson.VBUser = mesVBUser;
            Operator = companyPerson;
        }

        public CompanyAddress GetMandant(gip.core.datamodel.VBUser vBUser)
        {
            return DatabaseApp.CompanyAddress.FirstOrDefault(c=>c.VBUserID == vBUser.VBUserID);
        }

        public CompanyPerson GetOperator(gip.core.datamodel.VBUser vBUser)
        {
            return DatabaseApp.CompanyPerson.FirstOrDefault(c=>c.VBUserID == vBUser.VBUserID);
        }
        #endregion

        #region Company

        #endregion

        #region Company Address

        #endregion

        #region Company 

    }
}
