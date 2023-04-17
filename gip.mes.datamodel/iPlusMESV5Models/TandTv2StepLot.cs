using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2StepLot : VBEntityObject 
{

    public TandTv2StepLot()
    {
    }

    private TandTv2StepLot(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _TandTv2StepLotID;
    public Guid TandTv2StepLotID 
    {
        get { return _TandTv2StepLotID; }
        set { SetProperty<Guid>(ref _TandTv2StepLotID, value); }
    }

    Guid _TandTv2StepID;
    public Guid TandTv2StepID 
    {
        get { return _TandTv2StepID; }
        set { SetProperty<Guid>(ref _TandTv2StepID, value); }
    }

    string _LotNo;
    public string LotNo 
    {
        get { return _LotNo; }
        set { SetProperty<string>(ref _LotNo, value); }
    }

    Guid _FacilityLotID;
    public Guid FacilityLotID 
    {
        get { return _FacilityLotID; }
        set { SetProperty<Guid>(ref _FacilityLotID, value); }
    }

    private TandTv2Step _TandTv2Step;
    public virtual TandTv2Step TandTv2Step
    { 
        get => LazyLoader.Load(this, ref _TandTv2Step);
        set => _TandTv2Step = value;
    }

    public bool TandTv2Step_IsLoaded
    {
        get
        {
            return TandTv2Step != null;
        }
    }

    public virtual ReferenceEntry TandTv2StepReference 
    {
        get { return Context.Entry(this).Reference("TandTv2Step"); }
    }
    }
