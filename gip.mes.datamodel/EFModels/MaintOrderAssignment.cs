using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintOrderAssignment : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaintOrderAssignment()
    {
    }

    private MaintOrderAssignment(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaintOrderAssignmentID;
    public Guid MaintOrderAssignmentID 
    {
        get { return _MaintOrderAssignmentID; }
        set { SetProperty<Guid>(ref _MaintOrderAssignmentID, value); }
    }

    Guid _MaintOrderID;
    public Guid MaintOrderID 
    {
        get { return _MaintOrderID; }
        set { SetForeignKeyProperty<Guid>(ref _MaintOrderID, value, "MaintOrder", _MaintOrder, MaintOrder != null ? MaintOrder.MaintOrderID : default(Guid)); }
    }

    Guid? _VBUserID;
    public Guid? VBUserID 
    {
        get { return _VBUserID; }
        set { SetForeignKeyProperty<Guid?>(ref _VBUserID, value, "VBUser", _VBUser, VBUser != null ? VBUser.VBUserID : default(Guid?)); }
    }

    Guid? _VBGroupID;
    public Guid? VBGroupID 
    {
        get { return _VBGroupID; }
        set { SetForeignKeyProperty<Guid?>(ref _VBGroupID, value, "VBGroup", _VBGroup, VBGroup != null ? VBGroup.VBGroupID : default(Guid?)); }
    }

    Guid? _CompanyID;
    public Guid? CompanyID 
    {
        get { return _CompanyID; }
        set { SetForeignKeyProperty<Guid?>(ref _CompanyID, value, "Company", _Company, Company != null ? Company.CompanyID : default(Guid?)); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    bool _IsActive;
    public bool IsActive 
    {
        get { return _IsActive; }
        set { SetProperty<bool>(ref _IsActive, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    private Company _Company;
    public virtual Company Company
    { 
        get { return LazyLoader.Load(this, ref _Company); } 
        set { SetProperty<Company>(ref _Company, value); }
    }

    public bool Company_IsLoaded
    {
        get
        {
            return _Company != null;
        }
    }

    public virtual ReferenceEntry CompanyReference 
    {
        get { return Context.Entry(this).Reference("Company"); }
    }
    
    private MaintOrder _MaintOrder;
    public virtual MaintOrder MaintOrder
    { 
        get { return LazyLoader.Load(this, ref _MaintOrder); } 
        set { SetProperty<MaintOrder>(ref _MaintOrder, value); }
    }

    public bool MaintOrder_IsLoaded
    {
        get
        {
            return _MaintOrder != null;
        }
    }

    public virtual ReferenceEntry MaintOrderReference 
    {
        get { return Context.Entry(this).Reference("MaintOrder"); }
    }
    
    private VBGroup _VBGroup;
    public virtual VBGroup VBGroup
    { 
        get { return LazyLoader.Load(this, ref _VBGroup); } 
        set { SetProperty<VBGroup>(ref _VBGroup, value); }
    }

    public bool VBGroup_IsLoaded
    {
        get
        {
            return _VBGroup != null;
        }
    }

    public virtual ReferenceEntry VBGroupReference 
    {
        get { return Context.Entry(this).Reference("VBGroup"); }
    }
    
    private VBUser _VBUser;
    public virtual VBUser VBUser
    { 
        get { return LazyLoader.Load(this, ref _VBUser); } 
        set { SetProperty<VBUser>(ref _VBUser, value); }
    }

    public bool VBUser_IsLoaded
    {
        get
        {
            return _VBUser != null;
        }
    }

    public virtual ReferenceEntry VBUserReference 
    {
        get { return Context.Entry(this).Reference("VBUser"); }
    }
    }
