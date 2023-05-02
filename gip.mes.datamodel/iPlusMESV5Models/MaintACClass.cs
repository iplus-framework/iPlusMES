using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintACClass : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaintACClass()
    {
    }

    private MaintACClass(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaintACClassID;
    public Guid MaintACClassID 
    {
        get { return _MaintACClassID; }
        set { SetProperty<Guid>(ref _MaintACClassID, value); }
    }

    Guid _VBiACClassID;
    public Guid VBiACClassID 
    {
        get { return _VBiACClassID; }
        set { SetProperty<Guid>(ref _VBiACClassID, value); }
    }

    Guid _MDMaintModeID;
    public Guid MDMaintModeID 
    {
        get { return _MDMaintModeID; }
        set { SetProperty<Guid>(ref _MDMaintModeID, value); }
    }

    bool _IsActive;
    public bool IsActive 
    {
        get { return _IsActive; }
        set { SetProperty<bool>(ref _IsActive, value); }
    }

    int? _MaintInterval;
    public int? MaintInterval 
    {
        get { return _MaintInterval; }
        set { SetProperty<int?>(ref _MaintInterval, value); }
    }

    DateTime? _LastMaintTerm;
    public DateTime? LastMaintTerm 
    {
        get { return _LastMaintTerm; }
        set { SetProperty<DateTime?>(ref _LastMaintTerm, value); }
    }

    bool _IsWarningActive;
    public bool IsWarningActive 
    {
        get { return _IsWarningActive; }
        set { SetProperty<bool>(ref _IsWarningActive, value); }
    }

    int? _WarningDiff;
    public int? WarningDiff 
    {
        get { return _WarningDiff; }
        set { SetProperty<int?>(ref _WarningDiff, value); }
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

    private MDMaintMode _MDMaintMode;
    public virtual MDMaintMode MDMaintMode
    { 
        get => LazyLoader.Load(this, ref _MDMaintMode);
        set => _MDMaintMode = value;
    }

    public bool MDMaintMode_IsLoaded
    {
        get
        {
            return MDMaintMode != null;
        }
    }

    public virtual ReferenceEntry MDMaintModeReference 
    {
        get { return Context.Entry(this).Reference("MDMaintMode"); }
    }
    
    private ICollection<MaintACClassProperty> _MaintACClassProperty_MaintACClass;
    public virtual ICollection<MaintACClassProperty> MaintACClassProperty_MaintACClass
    {
        get => LazyLoader.Load(this, ref _MaintACClassProperty_MaintACClass);
        set => _MaintACClassProperty_MaintACClass = value;
    }

    public bool MaintACClassProperty_MaintACClass_IsLoaded
    {
        get
        {
            return MaintACClassProperty_MaintACClass != null;
        }
    }

    public virtual CollectionEntry MaintACClassProperty_MaintACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintACClassProperty_MaintACClass); }
    }

    private ICollection<MaintACClassVBGroup> _MaintACClassVBGroup_MaintACClass;
    public virtual ICollection<MaintACClassVBGroup> MaintACClassVBGroup_MaintACClass
    {
        get => LazyLoader.Load(this, ref _MaintACClassVBGroup_MaintACClass);
        set => _MaintACClassVBGroup_MaintACClass = value;
    }

    public bool MaintACClassVBGroup_MaintACClass_IsLoaded
    {
        get
        {
            return MaintACClassVBGroup_MaintACClass != null;
        }
    }

    public virtual CollectionEntry MaintACClassVBGroup_MaintACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintACClassVBGroup_MaintACClass); }
    }

    private ICollection<MaintOrder> _MaintOrder_MaintACClass;
    public virtual ICollection<MaintOrder> MaintOrder_MaintACClass
    {
        get => LazyLoader.Load(this, ref _MaintOrder_MaintACClass);
        set => _MaintOrder_MaintACClass = value;
    }

    public bool MaintOrder_MaintACClass_IsLoaded
    {
        get
        {
            return MaintOrder_MaintACClass != null;
        }
    }

    public virtual CollectionEntry MaintOrder_MaintACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrder_MaintACClass); }
    }

    private ACClass _VBiACClass;
    public virtual ACClass VBiACClass
    { 
        get => LazyLoader.Load(this, ref _VBiACClass);
        set => _VBiACClass = value;
    }

    public bool VBiACClass_IsLoaded
    {
        get
        {
            return VBiACClass != null;
        }
    }

    public virtual ReferenceEntry VBiACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiACClass"); }
    }
    }
