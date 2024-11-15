using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDSchedulingGroupWF : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MDSchedulingGroupWF()
    {
    }

    private MDSchedulingGroupWF(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDSchedulingGroupWFID;
    public Guid MDSchedulingGroupWFID 
    {
        get { return _MDSchedulingGroupWFID; }
        set { SetProperty<Guid>(ref _MDSchedulingGroupWFID, value); }
    }

    Guid _MDSchedulingGroupID;
    public Guid MDSchedulingGroupID 
    {
        get { return _MDSchedulingGroupID; }
        set { SetProperty<Guid>(ref _MDSchedulingGroupID, value); }
    }

    Guid _VBiACClassWFID;
    public Guid VBiACClassWFID 
    {
        get { return _VBiACClassWFID; }
        set { SetProperty<Guid>(ref _VBiACClassWFID, value); }
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

    private MDSchedulingGroup _MDSchedulingGroup;
    public virtual MDSchedulingGroup MDSchedulingGroup
    { 
        get { return LazyLoader.Load(this, ref _MDSchedulingGroup); } 
        set { SetProperty<MDSchedulingGroup>(ref _MDSchedulingGroup, value); }
    }

    public bool MDSchedulingGroup_IsLoaded
    {
        get
        {
            return _MDSchedulingGroup != null;
        }
    }

    public virtual ReferenceEntry MDSchedulingGroupReference 
    {
        get { return Context.Entry(this).Reference("MDSchedulingGroup"); }
    }
    
    private ACClassWF _VBiACClassWF;
    public virtual ACClassWF VBiACClassWF
    { 
        get { return LazyLoader.Load(this, ref _VBiACClassWF); } 
        set { SetProperty<ACClassWF>(ref _VBiACClassWF, value); }
    }

    public bool VBiACClassWF_IsLoaded
    {
        get
        {
            return _VBiACClassWF != null;
        }
    }

    public virtual ReferenceEntry VBiACClassWFReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassWF"); }
    }
    }
