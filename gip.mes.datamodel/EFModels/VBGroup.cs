using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBGroup : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public VBGroup()
    {
    }

    private VBGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _VBGroupID;
    public Guid VBGroupID 
    {
        get { return _VBGroupID; }
        set { SetProperty<Guid>(ref _VBGroupID, value); }
    }

    string _VBGroupName;
    public string VBGroupName 
    {
        get { return _VBGroupName; }
        set { SetProperty<string>(ref _VBGroupName, value); }
    }

    string _Description;
    public string Description 
    {
        get { return _Description; }
        set { SetProperty<string>(ref _Description, value); }
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

    private ICollection<MaintOrderAssignment> _MaintOrderAssignment_VBGroup;
    public virtual ICollection<MaintOrderAssignment> MaintOrderAssignment_VBGroup
    {
        get { return LazyLoader.Load(this, ref _MaintOrderAssignment_VBGroup); }
        set { _MaintOrderAssignment_VBGroup = value; }
    }

    public bool MaintOrderAssignment_VBGroup_IsLoaded
    {
        get
        {
            return _MaintOrderAssignment_VBGroup != null;
        }
    }

    public virtual CollectionEntry MaintOrderAssignment_VBGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrderAssignment_VBGroup); }
    }

    private ICollection<VBGroupRight> _VBGroupRight_VBGroup;
    public virtual ICollection<VBGroupRight> VBGroupRight_VBGroup
    {
        get { return LazyLoader.Load(this, ref _VBGroupRight_VBGroup); }
        set { _VBGroupRight_VBGroup = value; }
    }

    public bool VBGroupRight_VBGroup_IsLoaded
    {
        get
        {
            return _VBGroupRight_VBGroup != null;
        }
    }

    public virtual CollectionEntry VBGroupRight_VBGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.VBGroupRight_VBGroup); }
    }

    private ICollection<VBUserGroup> _VBUserGroup_VBGroup;
    public virtual ICollection<VBUserGroup> VBUserGroup_VBGroup
    {
        get { return LazyLoader.Load(this, ref _VBUserGroup_VBGroup); }
        set { _VBUserGroup_VBGroup = value; }
    }

    public bool VBUserGroup_VBGroup_IsLoaded
    {
        get
        {
            return _VBUserGroup_VBGroup != null;
        }
    }

    public virtual CollectionEntry VBUserGroup_VBGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.VBUserGroup_VBGroup); }
    }
}
