using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Picking : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public Picking()
    {
    }

    private Picking(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PickingID;
    public Guid PickingID 
    {
        get { return _PickingID; }
        set { SetProperty<Guid>(ref _PickingID, value); }
    }

    Guid? _ACClassMethodID;
    public Guid? ACClassMethodID 
    {
        get { return _ACClassMethodID; }
        set { SetProperty<Guid?>(ref _ACClassMethodID, value); }
    }

    Guid? _VisitorVoucherID;
    public Guid? VisitorVoucherID 
    {
        get { return _VisitorVoucherID; }
        set { SetProperty<Guid?>(ref _VisitorVoucherID, value); }
    }

    Guid? _TourplanID;
    public Guid? TourplanID 
    {
        get { return _TourplanID; }
        set { SetProperty<Guid?>(ref _TourplanID, value); }
    }

    string _PickingNo;
    public string PickingNo 
    {
        get { return _PickingNo; }
        set { SetProperty<string>(ref _PickingNo, value); }
    }

    short _PickingStateIndex;
    public short PickingStateIndex 
    {
        get { return _PickingStateIndex; }
        set { SetProperty<short>(ref _PickingStateIndex, value); }
    }

    DateTime _DeliveryDateFrom;
    public DateTime DeliveryDateFrom 
    {
        get { return _DeliveryDateFrom; }
        set { SetProperty<DateTime>(ref _DeliveryDateFrom, value); }
    }

    DateTime _DeliveryDateTo;
    public DateTime DeliveryDateTo 
    {
        get { return _DeliveryDateTo; }
        set { SetProperty<DateTime>(ref _DeliveryDateTo, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
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

    Guid _MDPickingTypeID;
    public Guid MDPickingTypeID 
    {
        get { return _MDPickingTypeID; }
        set { SetProperty<Guid>(ref _MDPickingTypeID, value); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    Guid? _DeliveryCompanyAddressID;
    public Guid? DeliveryCompanyAddressID 
    {
        get { return _DeliveryCompanyAddressID; }
        set { SetProperty<Guid?>(ref _DeliveryCompanyAddressID, value); }
    }

    Guid? _MirroredFromPickingID;
    public Guid? MirroredFromPickingID 
    {
        get { return _MirroredFromPickingID; }
        set { SetProperty<Guid?>(ref _MirroredFromPickingID, value); }
    }

    string _Comment2;
    public string Comment2 
    {
        get { return _Comment2; }
        set { SetProperty<string>(ref _Comment2, value); }
    }

    private ACClassMethod _ACClassMethod;
    public virtual ACClassMethod ACClassMethod
    { 
        get { return LazyLoader.Load(this, ref _ACClassMethod); } 
        set { SetProperty<ACClassMethod>(ref _ACClassMethod, value); }
    }

    public bool ACClassMethod_IsLoaded
    {
        get
        {
            return ACClassMethod != null;
        }
    }

    public virtual ReferenceEntry ACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("ACClassMethod"); }
    }
    
    private CompanyAddress _DeliveryCompanyAddress;
    public virtual CompanyAddress DeliveryCompanyAddress
    { 
        get { return LazyLoader.Load(this, ref _DeliveryCompanyAddress); } 
        set { SetProperty<CompanyAddress>(ref _DeliveryCompanyAddress, value); }
    }

    public bool DeliveryCompanyAddress_IsLoaded
    {
        get
        {
            return DeliveryCompanyAddress != null;
        }
    }

    public virtual ReferenceEntry DeliveryCompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("DeliveryCompanyAddress"); }
    }
    
    private MDPickingType _MDPickingType;
    public virtual MDPickingType MDPickingType
    { 
        get { return LazyLoader.Load(this, ref _MDPickingType); } 
        set { SetProperty<MDPickingType>(ref _MDPickingType, value); }
    }

    public bool MDPickingType_IsLoaded
    {
        get
        {
            return MDPickingType != null;
        }
    }

    public virtual ReferenceEntry MDPickingTypeReference 
    {
        get { return Context.Entry(this).Reference("MDPickingType"); }
    }
    
    private ICollection<MaintOrder> _MaintOrder_Picking;
    public virtual ICollection<MaintOrder> MaintOrder_Picking
    {
        get { return LazyLoader.Load(this, ref _MaintOrder_Picking); }
        set { _MaintOrder_Picking = value; }
    }

    public bool MaintOrder_Picking_IsLoaded
    {
        get
        {
            return MaintOrder_Picking != null;
        }
    }

    public virtual CollectionEntry MaintOrder_PickingReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrder_Picking); }
    }

    private ICollection<PickingConfig> _PickingConfig_Picking;
    public virtual ICollection<PickingConfig> PickingConfig_Picking
    {
        get { return LazyLoader.Load(this, ref _PickingConfig_Picking); }
        set { _PickingConfig_Picking = value; }
    }

    public bool PickingConfig_Picking_IsLoaded
    {
        get
        {
            return PickingConfig_Picking != null;
        }
    }

    public virtual CollectionEntry PickingConfig_PickingReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingConfig_Picking); }
    }

    private ICollection<PickingPos> _PickingPos_Picking;
    public virtual ICollection<PickingPos> PickingPos_Picking
    {
        get { return LazyLoader.Load(this, ref _PickingPos_Picking); }
        set { _PickingPos_Picking = value; }
    }

    public bool PickingPos_Picking_IsLoaded
    {
        get
        {
            return PickingPos_Picking != null;
        }
    }

    public virtual CollectionEntry PickingPos_PickingReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPos_Picking); }
    }

    private Tourplan _Tourplan;
    public virtual Tourplan Tourplan
    { 
        get { return LazyLoader.Load(this, ref _Tourplan); } 
        set { SetProperty<Tourplan>(ref _Tourplan, value); }
    }

    public bool Tourplan_IsLoaded
    {
        get
        {
            return Tourplan != null;
        }
    }

    public virtual ReferenceEntry TourplanReference 
    {
        get { return Context.Entry(this).Reference("Tourplan"); }
    }
    
    private VisitorVoucher _VisitorVoucher;
    public virtual VisitorVoucher VisitorVoucher
    { 
        get { return LazyLoader.Load(this, ref _VisitorVoucher); } 
        set { SetProperty<VisitorVoucher>(ref _VisitorVoucher, value); }
    }

    public bool VisitorVoucher_IsLoaded
    {
        get
        {
            return VisitorVoucher != null;
        }
    }

    public virtual ReferenceEntry VisitorVoucherReference 
    {
        get { return Context.Entry(this).Reference("VisitorVoucher"); }
    }
    }
