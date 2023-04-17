using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class JobTableRecalcActualQuantity : VBEntityObject 
{

    public JobTableRecalcActualQuantity()
    {
    }

    private JobTableRecalcActualQuantity(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    long _ID;
    public long ID 
    {
        get { return _ID; }
        set { SetProperty<long>(ref _ID, value); }
    }

    Guid _JobID;
    public Guid JobID 
    {
        get { return _JobID; }
        set { SetProperty<Guid>(ref _JobID, value); }
    }

    Guid _ItemID;
    public Guid ItemID 
    {
        get { return _ItemID; }
        set { SetProperty<Guid>(ref _ItemID, value); }
    }

    string _ItemType;
    public string ItemType 
    {
        get { return _ItemType; }
        set { SetProperty<string>(ref _ItemType, value); }
    }
}
