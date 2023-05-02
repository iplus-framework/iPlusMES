using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintACClassVBGroup : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaintACClassVBGroup()
    {
    }

    private MaintACClassVBGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaintACClassVBGroupID;
    public Guid MaintACClassVBGroupID 
    {
        get { return _MaintACClassVBGroupID; }
        set { SetProperty<Guid>(ref _MaintACClassVBGroupID, value); }
    }

    Guid? _MaintACClassID;
    public Guid? MaintACClassID 
    {
        get { return _MaintACClassID; }
        set { SetProperty<Guid?>(ref _MaintACClassID, value); }
    }

    Guid? _MaintACClassPropertyID;
    public Guid? MaintACClassPropertyID 
    {
        get { return _MaintACClassPropertyID; }
        set { SetProperty<Guid?>(ref _MaintACClassPropertyID, value); }
    }

    Guid _VBGroupID;
    public Guid VBGroupID 
    {
        get { return _VBGroupID; }
        set { SetProperty<Guid>(ref _VBGroupID, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    bool _IsActive;
    public bool IsActive 
    {
        get { return _IsActive; }
        set { SetProperty<bool>(ref _IsActive, value); }
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

    private MaintACClass _MaintACClass;
    public virtual MaintACClass MaintACClass
    { 
        get => LazyLoader.Load(this, ref _MaintACClass);
        set => _MaintACClass = value;
    }

    public bool MaintACClass_IsLoaded
    {
        get
        {
            return MaintACClass != null;
        }
    }

    public virtual ReferenceEntry MaintACClassReference 
    {
        get { return Context.Entry(this).Reference("MaintACClass"); }
    }
    
    private MaintACClassProperty _MaintACClassProperty;
    public virtual MaintACClassProperty MaintACClassProperty
    { 
        get => LazyLoader.Load(this, ref _MaintACClassProperty);
        set => _MaintACClassProperty = value;
    }

    public bool MaintACClassProperty_IsLoaded
    {
        get
        {
            return MaintACClassProperty != null;
        }
    }

    public virtual ReferenceEntry MaintACClassPropertyReference 
    {
        get { return Context.Entry(this).Reference("MaintACClassProperty"); }
    }
    
    private ICollection<MaintTask> _MaintTask_MaintACClassVBGroup;
    public virtual ICollection<MaintTask> MaintTask_MaintACClassVBGroup
    {
        get => LazyLoader.Load(this, ref _MaintTask_MaintACClassVBGroup);
        set => _MaintTask_MaintACClassVBGroup = value;
    }

    public bool MaintTask_MaintACClassVBGroup_IsLoaded
    {
        get
        {
            return MaintTask_MaintACClassVBGroup != null;
        }
    }

    public virtual CollectionEntry MaintTask_MaintACClassVBGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintTask_MaintACClassVBGroup); }
    }

    private VBGroup _VBGroup;
    public virtual VBGroup VBGroup
    { 
        get => LazyLoader.Load(this, ref _VBGroup);
        set => _VBGroup = value;
    }

    public bool VBGroup_IsLoaded
    {
        get
        {
            return VBGroup != null;
        }
    }

    public virtual ReferenceEntry VBGroupReference 
    {
        get { return Context.Entry(this).Reference("VBGroup"); }
    }
    }
