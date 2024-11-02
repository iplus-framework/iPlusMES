// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDDelivType : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDDelivType()
    {
    }

    private MDDelivType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDDelivTypeID;
    public Guid MDDelivTypeID 
    {
        get { return _MDDelivTypeID; }
        set { SetProperty<Guid>(ref _MDDelivTypeID, value); }
    }

    short _MDDelivTypeIndex;
    public short MDDelivTypeIndex 
    {
        get { return _MDDelivTypeIndex; }
        set { SetProperty<short>(ref _MDDelivTypeIndex, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    private ICollection<CompanyAddress> _CompanyAddress_MDDelivType;
    public virtual ICollection<CompanyAddress> CompanyAddress_MDDelivType
    {
        get { return LazyLoader.Load(this, ref _CompanyAddress_MDDelivType); }
        set { _CompanyAddress_MDDelivType = value; }
    }

    public bool CompanyAddress_MDDelivType_IsLoaded
    {
        get
        {
            return CompanyAddress_MDDelivType != null;
        }
    }

    public virtual CollectionEntry CompanyAddress_MDDelivTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyAddress_MDDelivType); }
    }

    private ICollection<InOrder> _InOrder_MDDelivType;
    public virtual ICollection<InOrder> InOrder_MDDelivType
    {
        get { return LazyLoader.Load(this, ref _InOrder_MDDelivType); }
        set { _InOrder_MDDelivType = value; }
    }

    public bool InOrder_MDDelivType_IsLoaded
    {
        get
        {
            return InOrder_MDDelivType != null;
        }
    }

    public virtual CollectionEntry InOrder_MDDelivTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_MDDelivType); }
    }

    private ICollection<InRequest> _InRequest_MDDelivType;
    public virtual ICollection<InRequest> InRequest_MDDelivType
    {
        get { return LazyLoader.Load(this, ref _InRequest_MDDelivType); }
        set { _InRequest_MDDelivType = value; }
    }

    public bool InRequest_MDDelivType_IsLoaded
    {
        get
        {
            return InRequest_MDDelivType != null;
        }
    }

    public virtual CollectionEntry InRequest_MDDelivTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequest_MDDelivType); }
    }

    private ICollection<OutOffer> _OutOffer_MDDelivType;
    public virtual ICollection<OutOffer> OutOffer_MDDelivType
    {
        get { return LazyLoader.Load(this, ref _OutOffer_MDDelivType); }
        set { _OutOffer_MDDelivType = value; }
    }

    public bool OutOffer_MDDelivType_IsLoaded
    {
        get
        {
            return OutOffer_MDDelivType != null;
        }
    }

    public virtual CollectionEntry OutOffer_MDDelivTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_MDDelivType); }
    }

    private ICollection<OutOrder> _OutOrder_MDDelivType;
    public virtual ICollection<OutOrder> OutOrder_MDDelivType
    {
        get { return LazyLoader.Load(this, ref _OutOrder_MDDelivType); }
        set { _OutOrder_MDDelivType = value; }
    }

    public bool OutOrder_MDDelivType_IsLoaded
    {
        get
        {
            return OutOrder_MDDelivType != null;
        }
    }

    public virtual CollectionEntry OutOrder_MDDelivTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_MDDelivType); }
    }
}
