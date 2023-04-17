using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintACClassProperty : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MaintACClassProperty()
    {
    }

    private MaintACClassProperty(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MaintACClassPropertyID;
    public Guid MaintACClassPropertyID 
    {
        get { return _MaintACClassPropertyID; }
        set { SetProperty<Guid>(ref _MaintACClassPropertyID, value); }
    }

    Guid _MaintACClassID;
    public Guid MaintACClassID 
    {
        get { return _MaintACClassID; }
        set { SetProperty<Guid>(ref _MaintACClassID, value); }
    }

    Guid _VBiACClassPropertyID;
    public Guid VBiACClassPropertyID 
    {
        get { return _VBiACClassPropertyID; }
        set { SetProperty<Guid>(ref _VBiACClassPropertyID, value); }
    }

    string _MaxValue;
    public string MaxValue 
    {
        get { return _MaxValue; }
        set { SetProperty<string>(ref _MaxValue, value); }
    }

    bool _IsActive;
    public bool IsActive 
    {
        get { return _IsActive; }
        set { SetProperty<bool>(ref _IsActive, value); }
    }

    bool _IsWarningActive;
    public bool IsWarningActive 
    {
        get { return _IsWarningActive; }
        set { SetProperty<bool>(ref _IsWarningActive, value); }
    }

    string _WarningValueDiff;
    public string WarningValueDiff 
    {
        get { return _WarningValueDiff; }
        set { SetProperty<string>(ref _WarningValueDiff, value); }
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
    
    private ICollection<MaintACClassVBGroup> _MaintACClassVBGroup_MaintACClassProperty;
    public virtual ICollection<MaintACClassVBGroup> MaintACClassVBGroup_MaintACClassProperty
    {
        get => LazyLoader.Load(this, ref _MaintACClassVBGroup_MaintACClassProperty);
        set => _MaintACClassVBGroup_MaintACClassProperty = value;
    }

    public bool MaintACClassVBGroup_MaintACClassProperty_IsLoaded
    {
        get
        {
            return MaintACClassVBGroup_MaintACClassProperty != null;
        }
    }

    public virtual CollectionEntry MaintACClassVBGroup_MaintACClassPropertyReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintACClassVBGroup_MaintACClassProperty); }
    }

    private ICollection<MaintOrderProperty> _MaintOrderProperty_MaintACClassProperty;
    public virtual ICollection<MaintOrderProperty> MaintOrderProperty_MaintACClassProperty
    {
        get => LazyLoader.Load(this, ref _MaintOrderProperty_MaintACClassProperty);
        set => _MaintOrderProperty_MaintACClassProperty = value;
    }

    public bool MaintOrderProperty_MaintACClassProperty_IsLoaded
    {
        get
        {
            return MaintOrderProperty_MaintACClassProperty != null;
        }
    }

    public virtual CollectionEntry MaintOrderProperty_MaintACClassPropertyReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrderProperty_MaintACClassProperty); }
    }

    private ACClassProperty _VBiACClassProperty;
    public virtual ACClassProperty VBiACClassProperty
    { 
        get => LazyLoader.Load(this, ref _VBiACClassProperty);
        set => _VBiACClassProperty = value;
    }

    public bool VBiACClassProperty_IsLoaded
    {
        get
        {
            return VBiACClassProperty != null;
        }
    }

    public virtual ReferenceEntry VBiACClassPropertyReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassProperty"); }
    }
    }
