using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CompanyAddressDepartment : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public CompanyAddressDepartment()
    {
    }

    private CompanyAddressDepartment(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _CompanyAddressDepartmentID;
    public Guid CompanyAddressDepartmentID 
    {
        get { return _CompanyAddressDepartmentID; }
        set { SetProperty<Guid>(ref _CompanyAddressDepartmentID, value); }
    }

    Guid _CompanyAddressID;
    public Guid CompanyAddressID 
    {
        get { return _CompanyAddressID; }
        set { SetProperty<Guid>(ref _CompanyAddressID, value); }
    }

    string _DepartmentName;
    public string DepartmentName 
    {
        get { return _DepartmentName; }
        set { SetProperty<string>(ref _DepartmentName, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    private CompanyAddress _CompanyAddress;
    public virtual CompanyAddress CompanyAddress
    { 
        get { return LazyLoader.Load(this, ref _CompanyAddress); } 
        set { SetProperty<CompanyAddress>(ref _CompanyAddress, value); }
    }

    public bool CompanyAddress_IsLoaded
    {
        get
        {
            return _CompanyAddress != null;
        }
    }

    public virtual ReferenceEntry CompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("CompanyAddress"); }
    }
    
    private ICollection<CompanyPersonRole> _CompanyPersonRole_CompanyAddressDepartment;
    public virtual ICollection<CompanyPersonRole> CompanyPersonRole_CompanyAddressDepartment
    {
        get { return LazyLoader.Load(this, ref _CompanyPersonRole_CompanyAddressDepartment); }
        set { _CompanyPersonRole_CompanyAddressDepartment = value; }
    }

    public bool CompanyPersonRole_CompanyAddressDepartment_IsLoaded
    {
        get
        {
            return _CompanyPersonRole_CompanyAddressDepartment != null;
        }
    }

    public virtual CollectionEntry CompanyPersonRole_CompanyAddressDepartmentReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyPersonRole_CompanyAddressDepartment); }
    }
}
