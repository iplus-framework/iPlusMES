using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv2ItemType : VBEntityObject 
{

    public TandTv2ItemType()
    {
    }

    private TandTv2ItemType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    string _TandTv2ItemTypeID;
    public string TandTv2ItemTypeID 
    {
        get { return _TandTv2ItemTypeID; }
        set { SetProperty<string>(ref _TandTv2ItemTypeID, value); }
    }

    string _ACCaptionTranslation;
    public string ACCaptionTranslation 
    {
        get { return _ACCaptionTranslation; }
        set { SetProperty<string>(ref _ACCaptionTranslation, value); }
    }

    private ICollection<TandTv2Job> _TandTv2Job_TandTv2ItemType;
    public virtual ICollection<TandTv2Job> TandTv2Job_TandTv2ItemType
    {
        get => LazyLoader.Load(this, ref _TandTv2Job_TandTv2ItemType);
        set => _TandTv2Job_TandTv2ItemType = value;
    }

    public bool TandTv2Job_TandTv2ItemType_IsLoaded
    {
        get
        {
            return TandTv2Job_TandTv2ItemType != null;
        }
    }

    public virtual CollectionEntry TandTv2Job_TandTv2ItemTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2Job_TandTv2ItemType); }
    }

    private ICollection<TandTv2StepItem> _TandTv2StepItem_TandTv2ItemType;
    public virtual ICollection<TandTv2StepItem> TandTv2StepItem_TandTv2ItemType
    {
        get => LazyLoader.Load(this, ref _TandTv2StepItem_TandTv2ItemType);
        set => _TandTv2StepItem_TandTv2ItemType = value;
    }

    public bool TandTv2StepItem_TandTv2ItemType_IsLoaded
    {
        get
        {
            return TandTv2StepItem_TandTv2ItemType != null;
        }
    }

    public virtual CollectionEntry TandTv2StepItem_TandTv2ItemTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv2StepItem_TandTv2ItemType); }
    }
}
