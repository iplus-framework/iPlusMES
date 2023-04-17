using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2Operation : VBEntityObject 
{

    public TandTv2Operation()
    {
    }

    private TandTv2Operation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    string _TandTv2OperationID;
    public string TandTv2OperationID 
    {
        get { return _TandTv2OperationID; }
        set { SetProperty<string>(ref _TandTv2OperationID, value); }
    }

    private ICollection<TandTv2StepItem> _TandTv2StepItem_TandTv2Operation;
    public virtual ICollection<TandTv2StepItem> TandTv2StepItem_TandTv2Operation
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItem_TandTv2Operation);
        set => _TandTv2StepItem_TandTv2Operation = value;
    }

    public bool TandTv2StepItem_TandTv2Operation_IsLoaded
    {
        get
        {
            return TandTv2StepItem_TandTv2Operation != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItem_TandTv2OperationReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItem_TandTv2Operation); }
    }
}
