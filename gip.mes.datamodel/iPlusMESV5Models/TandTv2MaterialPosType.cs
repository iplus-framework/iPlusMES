using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2MaterialPosType : VBEntityObject 
{

    public TandTv2MaterialPosType()
    {
    }

    private TandTv2MaterialPosType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    int _TandTv2MaterialPosTypeID;
    public int TandTv2MaterialPosTypeID 
    {
        get { return _TandTv2MaterialPosTypeID; }
        set { SetProperty<int>(ref _TandTv2MaterialPosTypeID, value); }
    }

    string _Name;
    public string Name 
    {
        get { return _Name; }
        set { SetProperty<string>(ref _Name, value); }
    }
}
