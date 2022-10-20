using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Xml.Linq;
using gip.mes.datamodel;

namespace gip.bso.test
{
    [ACClassInfo(Const.PackName_VarioTest, "en{'Installation Test'}de{'Installation Test'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, false, true)]
    public class VXBSOInstallation : ACBSO
    {
        #region c´tors

        public VXBSOInstallation(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
            _ACEntitySerializer = new ACEntitySerializer();
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }
        #endregion

        ACEntitySerializer _ACEntitySerializer;
        public ACEntitySerializer ACEntitySerializer
        {
            get
            {
                return _ACEntitySerializer;
            }
        }

        #region BSO->ACProperty
        string _CurrentXML;
        [ACPropertyCurrent(1, "")]
        public string CurrentXML
        {
            get
            {
                return _CurrentXML;
            }
            set
            {
                _CurrentXML = value;
                OnPropertyChanged("CurrentXML");
            }
        }

        #endregion

        #region BSO->ACMethod

        [ACMethodInfo("", "en{'Export MDCountry'}de{'Export MDCountry'}", 9999, false, false, true)]
        public void ExportMDCountry()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + MDCountry.ClassName, null);

            queryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDCountryName", Global.LogicalOperators.equal, Global.Operators.and, "GB", false));

            XElement element = ACEntitySerializer.Serialize(database, queryDefinition, "");
            CurrentXML = element != null ? element.ToString() : "";
        }

        [ACMethodInfo("", "en{'Export MDCountryList'}de{'Export MDCountryList'}", 9999, false, false, true)]
        public void ExportMDCountryList()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + MDCountry.ClassName, null);

            XElement element = ACEntitySerializer.Serialize(database, queryDefinition, "");
            CurrentXML = element != null ? element.ToString() : "";
        }

        [ACMethodInfo("", "en{'Import MDCountry'}de{'Import MDCountry'}", 9999, false, false, true)]
        public void ImportMDCountry()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + MDCountry.ClassName, null);
            ACFSItem acFSParentItem = new ACFSItem(null, new ACFSItemContainer(Database), Database, "", ResourceTypeEnum.Folder);
            // ACEntitySerializer.DeserializeXML(Database, acFSParentItem, CurrentXML, queryDefinition);
        }

        [ACMethodInfo("", "en{'Export Company'}de{'Export Company'}", 9999, false, false, true)]
        public void ExportCompany()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Company.ClassName, null);

            queryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "CompanyNo", Global.LogicalOperators.equal, Global.Operators.and, "1004", false));

            XElement element = ACEntitySerializer.Serialize(database, queryDefinition, "");
            CurrentXML = element != null ? element.ToString() : "";
        }

        [ACMethodInfo("", "en{'Export CompanyList'}de{'Export CompanyList'}", 9999, false, false, true)]
        public void ExportCompanyList()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Company.ClassName, null);

            XElement element = ACEntitySerializer.Serialize(database, queryDefinition, "");
            CurrentXML = element != null ? element.ToString() : "";
        }

        [ACMethodInfo("", "en{'Import Company'}de{'Import Company'}", 9999, false, false, true)]
        public void ImportCompany()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Company.ClassName, null);
            ACFSItem acFSParentItem = new ACFSItem(null, new ACFSItemContainer(Database), Database, "", ResourceTypeEnum.Folder);
            // ACEntitySerializer.DeserializeXML(Database, acFSParentItem, CurrentXML, queryDefinition);
        }

        [ACMethodInfo("", "en{'Export CompanyAdress'}de{'Export CompanyAdress'}", 9999, false, false, true)]
        public void ExportCompanyAdress()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + CompanyAddress.ClassName, null);

            queryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Name1", Global.LogicalOperators.equal, Global.Operators.and, "Kundenname 1", false));

            XElement element = ACEntitySerializer.Serialize(database, queryDefinition, "");
            CurrentXML = element != null ? element.ToString() : "";
        }

        [ACMethodInfo("", "en{'Export CompanyAdressList'}de{'Export CompanyAdressList'}", 9999, false, false, true)]
        public void ExportCompanyAdressList()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + CompanyAddress.ClassName, null);

            XElement element = ACEntitySerializer.Serialize(database, queryDefinition, "");
            CurrentXML = element != null ? element.ToString() : "";
        }

        [ACMethodInfo("", "en{'Import CompanyAdress'}de{'Import CompanyAdress'}", 9999, false, false, true)]
        public void ImportCompanyAdress()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + CompanyAddress.ClassName, null);
            ACFSItem acFSParentItem = new ACFSItem(null, new ACFSItemContainer(Database), Database, "", ResourceTypeEnum.Folder);
            // ACEntitySerializer.DeserializeXML(Database, acFSParentItem, CurrentXML, queryDefinition);
        }


        [ACMethodInfo("", "en{'Export MDCountryQuery'}de{'Export MDCountryQuery'}", 9999, false, false, true)]
        public void ExportMDCountryQuery()
        {
            var qryMDCountry = Root.Queries.CreateQuery(null, Const.QueryPrefix + MDCountry.ClassName, null);

            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + ACQueryDefinition.ClassName, null);

            //CurrentXML = qryMDCountry.ConfigXML;
        }

        [ACMethodInfo("", "en{'Export MDCountryQueryFilter'}de{'Export MDCountryQueryFilter'}", 9999, false, false, true)]
        public void ExportMDCountryQueryFilter()
        {
            var qryMDCountry = Root.Queries.CreateQuery(null, Const.QueryPrefix + MDCountry.ClassName, null);
            qryMDCountry.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDCountryName", Global.LogicalOperators.equal, Global.Operators.and, "GB", false));

            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + ACQueryDefinition.ClassName, null);

            //CurrentXML = qryMDCountry.ConfigXML;
        }

        [ACMethodInfo("", "en{'Import MDCountry'}de{'Import MDCountry'}", 9999, false, false, true)]
        public void ImportMDCountryQuery()
        {
            var qryMDCountry = Root.Queries.CreateQuery(null, Const.QueryPrefix + MDCountry.ClassName, null);
            // qryMDCountry.CopyFrom(CurrentXML);
        }

        [ACMethodInfo("", "en{'Export ACClass'}de{'Export ACClass'}", 9999, false, false, true)]
        public void ExportACClass()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + gip.core.datamodel.ACClass.ClassName, null);

            queryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "AssemblyQualifiedName", Global.LogicalOperators.equal, Global.Operators.and, "gip.bso.masterdata.MDBSOMaterialGroup", false));

            XElement element = ACEntitySerializer.Serialize(database, queryDefinition, "");
            CurrentXML = element != null ? element.ToString() : "";
        }

        [ACMethodInfo("", "en{'Import ACClass'}de{'Import ACClass'}", 9999, false, false, true)]
        public void ImportACClass()
        {
            Database database = new Database();

            var queryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + gip.core.datamodel.ACClass.ClassName, null);
            ACFSItem acFSParentItem = new ACFSItem(null, new ACFSItemContainer(Database), Database, "", ResourceTypeEnum.Folder);
            // ACEntitySerializer.DeserializeXML(Database,acFSParentItem, CurrentXML, queryDefinition);
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"ExportMDCountry":
                    ExportMDCountry();
                    return true;
                case"ExportMDCountryList":
                    ExportMDCountryList();
                    return true;
                case"ImportMDCountry":
                    ImportMDCountry();
                    return true;
                case"ExportCompany":
                    ExportCompany();
                    return true;
                case"ExportCompanyList":
                    ExportCompanyList();
                    return true;
                case"ImportCompany":
                    ImportCompany();
                    return true;
                case"ExportCompanyAdress":
                    ExportCompanyAdress();
                    return true;
                case"ExportCompanyAdressList":
                    ExportCompanyAdressList();
                    return true;
                case"ImportCompanyAdress":
                    ImportCompanyAdress();
                    return true;
                case"ExportMDCountryQuery":
                    ExportMDCountryQuery();
                    return true;
                case"ExportMDCountryQueryFilter":
                    ExportMDCountryQueryFilter();
                    return true;
                case"ImportMDCountryQuery":
                    ImportMDCountryQuery();
                    return true;
                case"ExportACClass":
                    ExportACClass();
                    return true;
                case"ImportACClass":
                    ImportACClass();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
