using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBUserACProject : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public VBUserACProject()
    {
    }

    private VBUserACProject(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _VBUserACProjectID;
    public Guid VBUserACProjectID 
    {
        get { return _VBUserACProjectID; }
        set { SetProperty<Guid>(ref _VBUserACProjectID, value); }
    }

    Guid _ACProjectID;
    public Guid ACProjectID 
    {
        get { return _ACProjectID; }
        set { SetForeignKeyProperty<Guid>(ref _ACProjectID, value, "ACProject", _ACProject, ACProject != null ? ACProject.ACProjectID : default(Guid)); }
    }

    Guid _VBUserID;
    public Guid VBUserID 
    {
        get { return _VBUserID; }
        set { SetForeignKeyProperty<Guid>(ref _VBUserID, value, "VBUser", _VBUser, VBUser != null ? VBUser.VBUserID : default(Guid)); }
    }

    bool _IsClient;
    public bool IsClient 
    {
        get { return _IsClient; }
        set { SetProperty<bool>(ref _IsClient, value); }
    }

    bool _IsServer;
    public bool IsServer 
    {
        get { return _IsServer; }
        set { SetProperty<bool>(ref _IsServer, value); }
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

    private ACProject _ACProject;
    public virtual ACProject ACProject
    { 
        get { return LazyLoader.Load(this, ref _ACProject); } 
        set { SetProperty<ACProject>(ref _ACProject, value); }
    }

    public bool ACProject_IsLoaded
    {
        get
        {
            return _ACProject != null;
        }
    }

    public virtual ReferenceEntry ACProjectReference 
    {
        get { return Context.Entry(this).Reference("ACProject"); }
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
