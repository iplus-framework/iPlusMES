using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CompanyPersonRole : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public CompanyPersonRole()
    {
    }

    private CompanyPersonRole(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _CompanyPersonRoleID;
    public Guid CompanyPersonRoleID 
    {
        get { return _CompanyPersonRoleID; }
        set { SetProperty<Guid>(ref _CompanyPersonRoleID, value); }
    }

    Guid _CompanyPersonID;
    public Guid CompanyPersonID 
    {
        get { return _CompanyPersonID; }
        set { SetProperty<Guid>(ref _CompanyPersonID, value); }
    }

    Guid _VBiRoleACClassID;
    public Guid VBiRoleACClassID 
    {
        get { return _VBiRoleACClassID; }
        set { SetProperty<Guid>(ref _VBiRoleACClassID, value); }
    }

    Guid? _CompanyAddressDepartmentID;
    public Guid? CompanyAddressDepartmentID 
    {
        get { return _CompanyAddressDepartmentID; }
        set { SetProperty<Guid?>(ref _CompanyAddressDepartmentID, value); }
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

    private CompanyAddressDepartment _CompanyAddressDepartment;
    public virtual CompanyAddressDepartment CompanyAddressDepartment
    { 
        get { return LazyLoader.Load(this, ref _CompanyAddressDepartment); } 
        set { SetProperty<CompanyAddressDepartment>(ref _CompanyAddressDepartment, value); }
    }

    public bool CompanyAddressDepartment_IsLoaded
    {
        get
        {
            return CompanyAddressDepartment != null;
        }
    }

    public virtual ReferenceEntry CompanyAddressDepartmentReference 
    {
        get { return Context.Entry(this).Reference("CompanyAddressDepartment"); }
    }
    
    private CompanyPerson _CompanyPerson;
    public virtual CompanyPerson CompanyPerson
    { 
        get { return LazyLoader.Load(this, ref _CompanyPerson); } 
        set { SetProperty<CompanyPerson>(ref _CompanyPerson, value); }
    }

    public bool CompanyPerson_IsLoaded
    {
        get
        {
            return CompanyPerson != null;
        }
    }

    public virtual ReferenceEntry CompanyPersonReference 
    {
        get { return Context.Entry(this).Reference("CompanyPerson"); }
    }
    
    private ACClass _VBiRoleACClass;
    public virtual ACClass VBiRoleACClass
    { 
        get { return LazyLoader.Load(this, ref _VBiRoleACClass); } 
        set { SetProperty<ACClass>(ref _VBiRoleACClass, value); }
    }

    public bool VBiRoleACClass_IsLoaded
    {
        get
        {
            return VBiRoleACClass != null;
        }
    }

    public virtual ReferenceEntry VBiRoleACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiRoleACClass"); }
    }
    }
