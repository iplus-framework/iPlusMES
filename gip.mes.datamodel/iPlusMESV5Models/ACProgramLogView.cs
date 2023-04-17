using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACProgramLogView : VBEntityObject 
{

    public ACProgramLogView()
    {
    }

    private ACProgramLogView(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _ACProgramLogID;
    public Guid ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid>(ref _ACProgramLogID, value); }
    }

    Guid _ACClassID;
    public Guid ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid>(ref _ACClassID, value); }
    }

    Guid? _BasedOnACClassID;
    public Guid? BasedOnACClassID 
    {
        get { return _BasedOnACClassID; }
        set { SetProperty<Guid?>(ref _BasedOnACClassID, value); }
    }

    string _ACClassACIdentifier;
    public string ACClassACIdentifier 
    {
        get { return _ACClassACIdentifier; }
        set { SetProperty<string>(ref _ACClassACIdentifier, value); }
    }

    string _ACClassACCaptionTranslation;
    public string ACClassACCaptionTranslation 
    {
        get { return _ACClassACCaptionTranslation; }
        set { SetProperty<string>(ref _ACClassACCaptionTranslation, value); }
    }

    string _ACUrl;
    public string ACUrl 
    {
        get { return _ACUrl; }
        set { SetProperty<string>(ref _ACUrl, value); }
    }

    string _ACProgramProgramNo;
    public string ACProgramProgramNo 
    {
        get { return _ACProgramProgramNo; }
        set { SetProperty<string>(ref _ACProgramProgramNo, value); }
    }

    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
    }

    string _ProdOrderBatchNo;
    public string ProdOrderBatchNo 
    {
        get { return _ProdOrderBatchNo; }
        set { SetProperty<string>(ref _ProdOrderBatchNo, value); }
    }

    int? _PartslistSequence;
    public int? PartslistSequence 
    {
        get { return _PartslistSequence; }
        set { SetProperty<int?>(ref _PartslistSequence, value); }
    }

    string _MaterialNo;
    public string MaterialNo 
    {
        get { return _MaterialNo; }
        set { SetProperty<string>(ref _MaterialNo, value); }
    }

    string _MaterialName;
    public string MaterialName 
    {
        get { return _MaterialName; }
        set { SetProperty<string>(ref _MaterialName, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    string _PosACUrl;
    public string PosACUrl 
    {
        get { return _PosACUrl; }
        set { SetProperty<string>(ref _PosACUrl, value); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    double? _InwardTargetQuantityUOM;
    public double? InwardTargetQuantityUOM 
    {
        get { return _InwardTargetQuantityUOM; }
        set { SetProperty<double?>(ref _InwardTargetQuantityUOM, value); }
    }

    double? _InwardActualQuantityUOM;
    public double? InwardActualQuantityUOM 
    {
        get { return _InwardActualQuantityUOM; }
        set { SetProperty<double?>(ref _InwardActualQuantityUOM, value); }
    }

    string _RelACUrl;
    public string RelACUrl 
    {
        get { return _RelACUrl; }
        set { SetProperty<string>(ref _RelACUrl, value); }
    }

    Guid? _ProdOrderPartslistPosRelationID;
    public Guid? ProdOrderPartslistPosRelationID 
    {
        get { return _ProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosRelationID, value); }
    }

    double? _OutwardTargetQuantityUOM;
    public double? OutwardTargetQuantityUOM 
    {
        get { return _OutwardTargetQuantityUOM; }
        set { SetProperty<double?>(ref _OutwardTargetQuantityUOM, value); }
    }

    double? _OutwardActualQuantityUOM;
    public double? OutwardActualQuantityUOM 
    {
        get { return _OutwardActualQuantityUOM; }
        set { SetProperty<double?>(ref _OutwardActualQuantityUOM, value); }
    }
}
