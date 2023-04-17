using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrderConnectionsDetailView : VBEntityObject 
{

    public ProdOrderConnectionsDetailView()
    {
    }

    private ProdOrderConnectionsDetailView(ILazyLoader lazyLoader)
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

    int _InwardIntermediateSequence;
    public int InwardIntermediateSequence 
    {
        get { return _InwardIntermediateSequence; }
        set { SetProperty<int>(ref _InwardIntermediateSequence, value); }
    }

    string _InwardProdOrderBatchNo;
    public string InwardProdOrderBatchNo 
    {
        get { return _InwardProdOrderBatchNo; }
        set { SetProperty<string>(ref _InwardProdOrderBatchNo, value); }
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

    int? _OutwardIntermediateSequence;
    public int? OutwardIntermediateSequence 
    {
        get { return _OutwardIntermediateSequence; }
        set { SetProperty<int?>(ref _OutwardIntermediateSequence, value); }
    }

    string _OutwardProdOrderBatchNo;
    public string OutwardProdOrderBatchNo 
    {
        get { return _OutwardProdOrderBatchNo; }
        set { SetProperty<string>(ref _OutwardProdOrderBatchNo, value); }
    }
}
