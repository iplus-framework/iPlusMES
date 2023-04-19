using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDMaintMode : VBEntityObject , IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDMaintMode()
    {
    }

    private MDMaintMode(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDMaintModeID;
    public Guid MDMaintModeID 
    {
        get { return _MDMaintModeID; }
        set { SetProperty<Guid>(ref _MDMaintModeID, value); }
    }

    short _MDMaintModeIndex;
    public short MDMaintModeIndex 
    {
        get { return _MDMaintModeIndex; }
        set { SetProperty<short>(ref _MDMaintModeIndex, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    private ICollection<MaintACClass> _MaintACClass_MDMaintMode;
    public virtual ICollection<MaintACClass> MaintACClass_MDMaintMode
    {
        get => LazyLoader.Load(this, ref _MaintACClass_MDMaintMode);
        set => _MaintACClass_MDMaintMode = value;
    }

    public bool MaintACClass_MDMaintMode_IsLoaded
    {
        get
        {
            return MaintACClass_MDMaintMode != null;
        }
    }

    public virtual CollectionEntry MaintACClass_MDMaintModeReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintACClass_MDMaintMode); }
    }

    private ICollection<MaintOrder> _MaintOrder_MDMaintMode;
    public virtual ICollection<MaintOrder> MaintOrder_MDMaintMode
    {
        get => LazyLoader.Load(this, ref _MaintOrder_MDMaintMode);
        set => _MaintOrder_MDMaintMode = value;
    }

    public bool MaintOrder_MDMaintMode_IsLoaded
    {
        get
        {
            return MaintOrder_MDMaintMode != null;
        }
    }

    public virtual CollectionEntry MaintOrder_MDMaintModeReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrder_MDMaintMode); }
    }
}
