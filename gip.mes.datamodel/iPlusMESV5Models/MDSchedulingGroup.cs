using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDSchedulingGroup : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDSchedulingGroup()
    {
    }

    private MDSchedulingGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDSchedulingGroupID;
    public Guid MDSchedulingGroupID 
    {
        get { return _MDSchedulingGroupID; }
        set { SetProperty<Guid>(ref _MDSchedulingGroupID, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    short _MDSchedulingGroupIndex;
    public short MDSchedulingGroupIndex 
    {
        get { return _MDSchedulingGroupIndex; }
        set { SetProperty<short>(ref _MDSchedulingGroupIndex, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
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

    private ICollection<FacilityMDSchedulingGroup> _FacilityMDSchedulingGroup_MDSchedulingGroup;
    public virtual ICollection<FacilityMDSchedulingGroup> FacilityMDSchedulingGroup_MDSchedulingGroup
    {
        get => LazyLoader.Load(this, ref _FacilityMDSchedulingGroup_MDSchedulingGroup);
        set => _FacilityMDSchedulingGroup_MDSchedulingGroup = value;
    }

    public bool FacilityMDSchedulingGroup_MDSchedulingGroup_IsLoaded
    {
        get
        {
            return FacilityMDSchedulingGroup_MDSchedulingGroup != null;
        }
    }

    public virtual CollectionEntry FacilityMDSchedulingGroup_MDSchedulingGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityMDSchedulingGroup_MDSchedulingGroup); }
    }

    private ICollection<MDSchedulingGroupWF> _MDSchedulingGroupWF_MDSchedulingGroup;
    public virtual ICollection<MDSchedulingGroupWF> MDSchedulingGroupWF_MDSchedulingGroup
    {
        get => LazyLoader.Load(this, ref _MDSchedulingGroupWF_MDSchedulingGroup);
        set => _MDSchedulingGroupWF_MDSchedulingGroup = value;
    }

    public bool MDSchedulingGroupWF_MDSchedulingGroup_IsLoaded
    {
        get
        {
            return MDSchedulingGroupWF_MDSchedulingGroup != null;
        }
    }

    public virtual CollectionEntry MDSchedulingGroupWF_MDSchedulingGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.MDSchedulingGroupWF_MDSchedulingGroup); }
    }
}
