using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDLabTag : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDLabTag()
    {
    }

    private MDLabTag(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDLabTagID;
    public Guid MDLabTagID 
    {
        get { return _MDLabTagID; }
        set { SetProperty<Guid>(ref _MDLabTagID, value); }
    }

    short _MDLabTagIndex;
    public short MDLabTagIndex 
    {
        get { return _MDLabTagIndex; }
        set { SetProperty<short>(ref _MDLabTagIndex, value); }
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

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
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

    private ICollection<LabOrderPos> _LabOrderPos_MDLabTag;
    public virtual ICollection<LabOrderPos> LabOrderPos_MDLabTag
    {
        get => LazyLoader.Load(this, ref _LabOrderPos_MDLabTag);
        set => _LabOrderPos_MDLabTag = value;
    }

    public bool LabOrderPos_MDLabTag_IsLoaded
    {
        get
        {
            return LabOrderPos_MDLabTag != null;
        }
    }

    public virtual CollectionEntry LabOrderPos_MDLabTagReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrderPos_MDLabTag); }
    }
}
