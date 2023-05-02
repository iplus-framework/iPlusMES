using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDSchedulingGroupWF : VBEntityObject
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

    private MDSchedulingGroup _MDSchedulingGroup;
    public virtual MDSchedulingGroup MDSchedulingGroup
    { 
        get => LazyLoader.Load(this, ref _MDSchedulingGroup);
        set => _MDSchedulingGroup = value;
    }

    public bool MDSchedulingGroup_IsLoaded
    {
        get
        {
            return MDSchedulingGroup != null;
        }
    }

    public virtual ReferenceEntry MDSchedulingGroupReference 
    {
        get { return Context.Entry(this).Reference("MDSchedulingGroup"); }
    }
    
    private ACClassWF _VBiACClassWF;
    public virtual ACClassWF VBiACClassWF
    { 
        get => LazyLoader.Load(this, ref _VBiACClassWF);
        set => _VBiACClassWF = value;
    }

    public bool VBiACClassWF_IsLoaded
    {
        get
        {
            return VBiACClassWF != null;
        }
    }

    public virtual ReferenceEntry VBiACClassWFReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassWF"); }
    }
    }
