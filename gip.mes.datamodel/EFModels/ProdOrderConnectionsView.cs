using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrderConnectionsView : VBEntityObject
{

    public ProdOrderConnectionsView()
    {
    }

    private ProdOrderConnectionsView(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    string _InwardProgramNo;
    public string InwardProgramNo 
    {
        get { return _InwardProgramNo; }
        set { SetProperty<string>(ref _InwardProgramNo, value); }
    }

    int _InwardPartslistSequence;
    public int InwardPartslistSequence 
    {
        get { return _InwardPartslistSequence; }
        set { SetProperty<int>(ref _InwardPartslistSequence, value); }
    }

    string _InwardLotNo;
    public string InwardLotNo 
    {
        get { return _InwardLotNo; }
        set { SetProperty<string>(ref _InwardLotNo, value); }
    }

    string _OutwardProgramNo;
    public string OutwardProgramNo 
    {
        get { return _OutwardProgramNo; }
        set { SetProperty<string>(ref _OutwardProgramNo, value); }
    }

    int? _OutwardPartslistSequence;
    public int? OutwardPartslistSequence 
    {
        get { return _OutwardPartslistSequence; }
        set { SetProperty<int?>(ref _OutwardPartslistSequence, value); }
    }
}
