using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class LabOrder : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public LabOrder()
    {
    }

    private LabOrder(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _LabOrderID;
    public Guid LabOrderID 
    {
        get { return _LabOrderID; }
        set { SetProperty<Guid>(ref _LabOrderID, value); }
    }

    string _LabOrderNo;
    public string LabOrderNo 
    {
        get { return _LabOrderNo; }
        set { SetProperty<string>(ref _LabOrderNo, value); }
    }

    Guid? _BasedOnTemplateID;
    public Guid? BasedOnTemplateID 
    {
        get { return _BasedOnTemplateID; }
        set { SetProperty<Guid?>(ref _BasedOnTemplateID, value); }
    }

    string _TemplateName;
    public string TemplateName 
    {
        get { return _TemplateName; }
        set { SetProperty<string>(ref _TemplateName, value); }
    }

    DateTime _SampleTakingDate;
    public DateTime SampleTakingDate 
    {
        get { return _SampleTakingDate; }
        set { SetProperty<DateTime>(ref _SampleTakingDate, value); }
    }

    DateTime? _TestDate;
    public DateTime? TestDate 
    {
        get { return _TestDate; }
        set { SetProperty<DateTime?>(ref _TestDate, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    Guid _MDLabOrderStateID;
    public Guid MDLabOrderStateID 
    {
        get { return _MDLabOrderStateID; }
        set { SetProperty<Guid>(ref _MDLabOrderStateID, value); }
    }

    Guid? _InOrderPosID;
    public Guid? InOrderPosID 
    {
        get { return _InOrderPosID; }
        set { SetProperty<Guid?>(ref _InOrderPosID, value); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    Guid? _OutOrderPosID;
    public Guid? OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetProperty<Guid?>(ref _OutOrderPosID, value); }
    }

    Guid? _FacilityLotID;
    public Guid? FacilityLotID 
    {
        get { return _FacilityLotID; }
        set { SetProperty<Guid?>(ref _FacilityLotID, value); }
    }

    short _LabOrderTypeIndex;
    public short LabOrderTypeIndex 
    {
        get { return _LabOrderTypeIndex; }
        set { SetProperty<short>(ref _LabOrderTypeIndex, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
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

    private LabOrder _LabOrder1_BasedOnTemplate;
    public virtual LabOrder LabOrder1_BasedOnTemplate
    { 
        get => LazyLoader.Load(this, ref _LabOrder1_BasedOnTemplate);
        set => _LabOrder1_BasedOnTemplate = value;
    }

    public bool LabOrder1_BasedOnTemplate_IsLoaded
    {
        get
        {
            return LabOrder1_BasedOnTemplate != null;
        }
    }

    public virtual ReferenceEntry LabOrder1_BasedOnTemplateReference 
    {
        get { return Context.Entry(this).Reference("LabOrder1_BasedOnTemplate"); }
    }
    
    private FacilityLot _FacilityLot;
    public virtual FacilityLot FacilityLot
    { 
        get => LazyLoader.Load(this, ref _FacilityLot);
        set => _FacilityLot = value;
    }

    public bool FacilityLot_IsLoaded
    {
        get
        {
            return FacilityLot != null;
        }
    }

    public virtual ReferenceEntry FacilityLotReference 
    {
        get { return Context.Entry(this).Reference("FacilityLot"); }
    }
    
    private InOrderPos _InOrderPos;
    public virtual InOrderPos InOrderPos
    { 
        get => LazyLoader.Load(this, ref _InOrderPos);
        set => _InOrderPos = value;
    }

    public bool InOrderPos_IsLoaded
    {
        get
        {
            return InOrderPos != null;
        }
    }

    public virtual ReferenceEntry InOrderPosReference 
    {
        get { return Context.Entry(this).Reference("InOrderPos"); }
    }
    
    private ICollection<LabOrder> _LabOrder_BasedOnTemplate;
    public virtual ICollection<LabOrder> LabOrder_BasedOnTemplate
    {
        get => LazyLoader.Load(this, ref _LabOrder_BasedOnTemplate);
        set => _LabOrder_BasedOnTemplate = value;
    }

    public bool LabOrder_BasedOnTemplate_IsLoaded
    {
        get
        {
            return LabOrder_BasedOnTemplate != null;
        }
    }

    public virtual CollectionEntry LabOrder_BasedOnTemplateReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrder_BasedOnTemplate); }
    }

    private ICollection<LabOrderPos> _LabOrderPo_LabOrder;
    public virtual ICollection<LabOrderPos> LabOrderPo_LabOrder
    {
        get => LazyLoader.Load(this, ref _LabOrderPo_LabOrder);
        set => _LabOrderPo_LabOrder = value;
    }

    public bool LabOrderPo_LabOrder_IsLoaded
    {
        get
        {
            return LabOrderPo_LabOrder != null;
        }
    }

    public virtual CollectionEntry LabOrderPo_LabOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrderPo_LabOrder); }
    }

    private MDLabOrderState _MDLabOrderState;
    public virtual MDLabOrderState MDLabOrderState
    { 
        get => LazyLoader.Load(this, ref _MDLabOrderState);
        set => _MDLabOrderState = value;
    }

    public bool MDLabOrderState_IsLoaded
    {
        get
        {
            return MDLabOrderState != null;
        }
    }

    public virtual ReferenceEntry MDLabOrderStateReference 
    {
        get { return Context.Entry(this).Reference("MDLabOrderState"); }
    }
    
    private Material _Material;
    public virtual Material Material
    { 
        get => LazyLoader.Load(this, ref _Material);
        set => _Material = value;
    }

    public bool Material_IsLoaded
    {
        get
        {
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get => LazyLoader.Load(this, ref _OutOrderPos);
        set => _OutOrderPos = value;
    }

    public bool OutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos"); }
    }
    
    private ProdOrderPartslistPos _ProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos ProdOrderPartslistPos
    { 
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos);
        set => _ProdOrderPartslistPos = value;
    }

    public bool ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPos"); }
    }
    }
