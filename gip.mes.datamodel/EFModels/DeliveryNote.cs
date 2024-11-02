// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class DeliveryNote : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public DeliveryNote()
    {
    }

    private DeliveryNote(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _DeliveryNoteID;
    public Guid DeliveryNoteID 
    {
        get { return _DeliveryNoteID; }
        set { SetProperty<Guid>(ref _DeliveryNoteID, value); }
    }

    string _DeliveryNoteNo;
    public string DeliveryNoteNo 
    {
        get { return _DeliveryNoteNo; }
        set { SetProperty<string>(ref _DeliveryNoteNo, value); }
    }

    short _DeliveryNoteTypeIndex;
    public short DeliveryNoteTypeIndex 
    {
        get { return _DeliveryNoteTypeIndex; }
        set { SetProperty<short>(ref _DeliveryNoteTypeIndex, value); }
    }

    Guid _MDDelivNoteStateID;
    public Guid MDDelivNoteStateID 
    {
        get { return _MDDelivNoteStateID; }
        set { SetProperty<Guid>(ref _MDDelivNoteStateID, value); }
    }

    Guid? _TourplanPosID;
    public Guid? TourplanPosID 
    {
        get { return _TourplanPosID; }
        set { SetProperty<Guid?>(ref _TourplanPosID, value); }
    }

    Guid _DeliveryCompanyAddressID;
    public Guid DeliveryCompanyAddressID 
    {
        get { return _DeliveryCompanyAddressID; }
        set { SetProperty<Guid>(ref _DeliveryCompanyAddressID, value); }
    }

    Guid _ShipperCompanyAddressID;
    public Guid ShipperCompanyAddressID 
    {
        get { return _ShipperCompanyAddressID; }
        set { SetProperty<Guid>(ref _ShipperCompanyAddressID, value); }
    }

    Guid? _VisitorVoucherID;
    public Guid? VisitorVoucherID 
    {
        get { return _VisitorVoucherID; }
        set { SetProperty<Guid?>(ref _VisitorVoucherID, value); }
    }

    DateTime _DeliveryDate;
    public DateTime DeliveryDate 
    {
        get { return _DeliveryDate; }
        set { SetProperty<DateTime>(ref _DeliveryDate, value); }
    }

    string _SupplierDeliveryNo;
    public string SupplierDeliveryNo 
    {
        get { return _SupplierDeliveryNo; }
        set { SetProperty<string>(ref _SupplierDeliveryNo, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    string _LossComment;
    public string LossComment 
    {
        get { return _LossComment; }
        set { SetProperty<string>(ref _LossComment, value); }
    }

    Guid? _WeighingID;
    public Guid? WeighingID 
    {
        get { return _WeighingID; }
        set { SetProperty<Guid?>(ref _WeighingID, value); }
    }

    double _TotalWeight;
    public double TotalWeight 
    {
        get { return _TotalWeight; }
        set { SetProperty<double>(ref _TotalWeight, value); }
    }

    double _EmptyWeight;
    public double EmptyWeight 
    {
        get { return _EmptyWeight; }
        set { SetProperty<double>(ref _EmptyWeight, value); }
    }

    double _LossWeight;
    public double LossWeight 
    {
        get { return _LossWeight; }
        set { SetProperty<double>(ref _LossWeight, value); }
    }

    double _NetWeight;
    public double NetWeight 
    {
        get { return _NetWeight; }
        set { SetProperty<double>(ref _NetWeight, value); }
    }

    double _DeliveryWeightOrderIn;
    public double DeliveryWeightOrderIn 
    {
        get { return _DeliveryWeightOrderIn; }
        set { SetProperty<double>(ref _DeliveryWeightOrderIn, value); }
    }

    double _DeliveryWeightDeliveryIn;
    public double DeliveryWeightDeliveryIn 
    {
        get { return _DeliveryWeightDeliveryIn; }
        set { SetProperty<double>(ref _DeliveryWeightDeliveryIn, value); }
    }

    double _DeliveryWeightStockIn;
    public double DeliveryWeightStockIn 
    {
        get { return _DeliveryWeightStockIn; }
        set { SetProperty<double>(ref _DeliveryWeightStockIn, value); }
    }

    double _DeliveryWeightOrderOut;
    public double DeliveryWeightOrderOut 
    {
        get { return _DeliveryWeightOrderOut; }
        set { SetProperty<double>(ref _DeliveryWeightOrderOut, value); }
    }

    double _DeliveryWeightDeliveryOut;
    public double DeliveryWeightDeliveryOut 
    {
        get { return _DeliveryWeightDeliveryOut; }
        set { SetProperty<double>(ref _DeliveryWeightDeliveryOut, value); }
    }

    double _DeliveryWeightStockOut;
    public double DeliveryWeightStockOut 
    {
        get { return _DeliveryWeightStockOut; }
        set { SetProperty<double>(ref _DeliveryWeightStockOut, value); }
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

    Guid? _Delivery2CompanyAddressID;
    public Guid? Delivery2CompanyAddressID 
    {
        get { return _Delivery2CompanyAddressID; }
        set { SetProperty<Guid?>(ref _Delivery2CompanyAddressID, value); }
    }

    private CompanyAddress _Delivery2CompanyAddress;
    public virtual CompanyAddress Delivery2CompanyAddress
    { 
        get { return LazyLoader.Load(this, ref _Delivery2CompanyAddress); } 
        set { SetProperty<CompanyAddress>(ref _Delivery2CompanyAddress, value); }
    }

    public bool Delivery2CompanyAddress_IsLoaded
    {
        get
        {
            return Delivery2CompanyAddress != null;
        }
    }

    public virtual ReferenceEntry Delivery2CompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("Delivery2CompanyAddress"); }
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
    
    private ICollection<DeliveryNotePos> _DeliveryNotePos_DeliveryNote;
    public virtual ICollection<DeliveryNotePos> DeliveryNotePos_DeliveryNote
    {
        get { return LazyLoader.Load(this, ref _DeliveryNotePos_DeliveryNote); }
        set { _DeliveryNotePos_DeliveryNote = value; }
    }

    public bool DeliveryNotePos_DeliveryNote_IsLoaded
    {
        get
        {
            return DeliveryNotePos_DeliveryNote != null;
        }
    }

    public virtual CollectionEntry DeliveryNotePos_DeliveryNoteReference
    {
        get { return Context.Entry(this).Collection(c => c.DeliveryNotePos_DeliveryNote); }
    }

    private MDDelivNoteState _MDDelivNoteState;
    public virtual MDDelivNoteState MDDelivNoteState
    { 
        get { return LazyLoader.Load(this, ref _MDDelivNoteState); } 
        set { SetProperty<MDDelivNoteState>(ref _MDDelivNoteState, value); }
    }

    public bool MDDelivNoteState_IsLoaded
    {
        get
        {
            return MDDelivNoteState != null;
        }
    }

    public virtual ReferenceEntry MDDelivNoteStateReference 
    {
        get { return Context.Entry(this).Reference("MDDelivNoteState"); }
    }
    
    private ICollection<Rating> _Rating_DeliveryNote;
    public virtual ICollection<Rating> Rating_DeliveryNote
    {
        get { return LazyLoader.Load(this, ref _Rating_DeliveryNote); }
        set { _Rating_DeliveryNote = value; }
    }

    public bool Rating_DeliveryNote_IsLoaded
    {
        get
        {
            return Rating_DeliveryNote != null;
        }
    }

    public virtual CollectionEntry Rating_DeliveryNoteReference
    {
        get { return Context.Entry(this).Collection(c => c.Rating_DeliveryNote); }
    }

    private CompanyAddress _ShipperCompanyAddress;
    public virtual CompanyAddress ShipperCompanyAddress
    { 
        get { return LazyLoader.Load(this, ref _ShipperCompanyAddress); } 
        set { SetProperty<CompanyAddress>(ref _ShipperCompanyAddress, value); }
    }

    public bool ShipperCompanyAddress_IsLoaded
    {
        get
        {
            return ShipperCompanyAddress != null;
        }
    }

    public virtual ReferenceEntry ShipperCompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("ShipperCompanyAddress"); }
    }
    
    private TourplanPos _TourplanPos;
    public virtual TourplanPos TourplanPos
    { 
        get { return LazyLoader.Load(this, ref _TourplanPos); } 
        set { SetProperty<TourplanPos>(ref _TourplanPos, value); }
    }

    public bool TourplanPos_IsLoaded
    {
        get
        {
            return TourplanPos != null;
        }
    }

    public virtual ReferenceEntry TourplanPosReference 
    {
        get { return Context.Entry(this).Reference("TourplanPos"); }
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
