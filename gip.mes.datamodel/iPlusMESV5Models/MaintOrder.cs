using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintOrder : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaintOrder()
    {
    }

    private MaintOrder(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaintOrderID;
    public Guid MaintOrderID 
    {
        get { return _MaintOrderID; }
        set { SetProperty<Guid>(ref _MaintOrderID, value); }
    }

    Guid _VBiPAACClassID;
    public Guid VBiPAACClassID 
    {
        get { return _VBiPAACClassID; }
        set { SetProperty<Guid>(ref _VBiPAACClassID, value); }
    }

    Guid _MaintACClassID;
    public Guid MaintACClassID 
    {
        get { return _MaintACClassID; }
        set { SetProperty<Guid>(ref _MaintACClassID, value); }
    }

    string _MaintOrderNo;
    public string MaintOrderNo 
    {
        get { return _MaintOrderNo; }
        set { SetProperty<string>(ref _MaintOrderNo, value); }
    }

    Guid? _MDMaintOrderStateID;
    public Guid? MDMaintOrderStateID 
    {
        get { return _MDMaintOrderStateID; }
        set { SetProperty<Guid?>(ref _MDMaintOrderStateID, value); }
    }

    Guid? _MDMaintModeID;
    public Guid? MDMaintModeID 
    {
        get { return _MDMaintModeID; }
        set { SetProperty<Guid?>(ref _MDMaintModeID, value); }
    }

    DateTime? _MaintSetDate;
    public DateTime? MaintSetDate 
    {
        get { return _MaintSetDate; }
        set { SetProperty<DateTime?>(ref _MaintSetDate, value); }
    }

    int _MaintSetDuration;
    public int MaintSetDuration 
    {
        get { return _MaintSetDuration; }
        set { SetProperty<int>(ref _MaintSetDuration, value); }
    }

    DateTime? _MaintActStartDate;
    public DateTime? MaintActStartDate 
    {
        get { return _MaintActStartDate; }
        set { SetProperty<DateTime?>(ref _MaintActStartDate, value); }
    }

    int _MaintActDuration;
    public int MaintActDuration 
    {
        get { return _MaintActDuration; }
        set { SetProperty<int>(ref _MaintActDuration, value); }
    }

    DateTime? _MaintActEndDate;
    public DateTime? MaintActEndDate 
    {
        get { return _MaintActEndDate; }
        set { SetProperty<DateTime?>(ref _MaintActEndDate, value); }
    }

    string _DirText;
    public string DirText 
    {
        get { return _DirText; }
        set { SetProperty<string>(ref _DirText, value); }
    }

    string _DirPicture;
    public string DirPicture 
    {
        get { return _DirPicture; }
        set { SetProperty<string>(ref _DirPicture, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
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
    
    private MDMaintOrderState _MDMaintOrderState;
    public virtual MDMaintOrderState MDMaintOrderState
    { 
        get => LazyLoader.Load(this, ref _MDMaintOrderState);
        set => _MDMaintOrderState = value;
    }

    public bool MDMaintOrderState_IsLoaded
    {
        get
        {
            return MDMaintOrderState != null;
        }
    }

    public virtual ReferenceEntry MDMaintOrderStateReference 
    {
        get { return Context.Entry(this).Reference("MDMaintOrderState"); }
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
    
    private ICollection<MaintOrderProperty> _MaintOrderProperty_MaintOrder;
    public virtual ICollection<MaintOrderProperty> MaintOrderProperty_MaintOrder
    {
        get => LazyLoader.Load(this, ref _MaintOrderProperty_MaintOrder);
        set => _MaintOrderProperty_MaintOrder = value;
    }

    public bool MaintOrderProperty_MaintOrder_IsLoaded
    {
        get
        {
            return MaintOrderProperty_MaintOrder != null;
        }
    }

    public virtual CollectionEntry MaintOrderProperty_MaintOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrderProperty_MaintOrder); }
    }

    private ICollection<MaintTask> _MaintTask_MaintOrder;
    public virtual ICollection<MaintTask> MaintTask_MaintOrder
    {
        get => LazyLoader.Load(this, ref _MaintTask_MaintOrder);
        set => _MaintTask_MaintOrder = value;
    }

    public bool MaintTask_MaintOrder_IsLoaded
    {
        get
        {
            return MaintTask_MaintOrder != null;
        }
    }

    public virtual CollectionEntry MaintTask_MaintOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintTask_MaintOrder); }
    }

    private ACClass _VBiPAACClass;
    public virtual ACClass VBiPAACClass
    { 
        get => LazyLoader.Load(this, ref _VBiPAACClass);
        set => _VBiPAACClass = value;
    }

    public bool VBiPAACClass_IsLoaded
    {
        get
        {
            return VBiPAACClass != null;
        }
    }

    public virtual ReferenceEntry VBiPAACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiPAACClass"); }
    }
    }
