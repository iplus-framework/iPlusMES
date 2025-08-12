using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassWFEdge : VBEntityObject
{

    public ACClassWFEdge()
    {
    }

    private ACClassWFEdge(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACClassWFEdgeID;
    public Guid ACClassWFEdgeID 
    {
        get { return _ACClassWFEdgeID; }
        set { SetProperty<Guid>(ref _ACClassWFEdgeID, value); }
    }

    Guid _ACClassMethodID;
    public Guid ACClassMethodID 
    {
        get { return _ACClassMethodID; }
        set { SetForeignKeyProperty<Guid>(ref _ACClassMethodID, value, "ACClassMethod", _ACClassMethod, ACClassMethod != null ? ACClassMethod.ACClassMethodID : default(Guid)); }
    }

    string _XName;
    public string XName 
    {
        get { return _XName; }
        set { SetProperty<string>(ref _XName, value); }
    }

    string _ACIdentifier;
    public override string ACIdentifier 
    {
        get { return _ACIdentifier; }
        set { SetProperty<string>(ref _ACIdentifier, value); }
    }

    Guid _SourceACClassWFID;
    public Guid SourceACClassWFID 
    {
        get { return _SourceACClassWFID; }
        set { SetForeignKeyProperty<Guid>(ref _SourceACClassWFID, value, "SourceACClassWF", _SourceACClassWF, SourceACClassWF != null ? SourceACClassWF.ACClassWFID : default(Guid)); }
    }

    Guid _SourceACClassPropertyID;
    public Guid SourceACClassPropertyID 
    {
        get { return _SourceACClassPropertyID; }
        set { SetForeignKeyProperty<Guid>(ref _SourceACClassPropertyID, value, "SourceACClassProperty", _SourceACClassProperty, SourceACClassProperty != null ? SourceACClassProperty.ACClassPropertyID : default(Guid)); }
    }

    Guid? _SourceACClassMethodID;
    public Guid? SourceACClassMethodID 
    {
        get { return _SourceACClassMethodID; }
        set { SetForeignKeyProperty<Guid?>(ref _SourceACClassMethodID, value, "SourceACClassMethod", _SourceACClassMethod, SourceACClassMethod != null ? SourceACClassMethod.ACClassMethodID : default(Guid?)); }
    }

    Guid _TargetACClassWFID;
    public Guid TargetACClassWFID 
    {
        get { return _TargetACClassWFID; }
        set { SetForeignKeyProperty<Guid>(ref _TargetACClassWFID, value, "TargetACClassWF", _TargetACClassWF, TargetACClassWF != null ? TargetACClassWF.ACClassWFID : default(Guid)); }
    }

    Guid _TargetACClassPropertyID;
    public Guid TargetACClassPropertyID 
    {
        get { return _TargetACClassPropertyID; }
        set { SetForeignKeyProperty<Guid>(ref _TargetACClassPropertyID, value, "TargetACClassProperty", _TargetACClassProperty, TargetACClassProperty != null ? TargetACClassProperty.ACClassPropertyID : default(Guid)); }
    }

    Guid? _TargetACClassMethodID;
    public Guid? TargetACClassMethodID 
    {
        get { return _TargetACClassMethodID; }
        set { SetForeignKeyProperty<Guid?>(ref _TargetACClassMethodID, value, "TargetACClassMethod", _TargetACClassMethod, TargetACClassMethod != null ? TargetACClassMethod.ACClassMethodID : default(Guid?)); }
    }

    short _ConnectionTypeIndex;
    public short ConnectionTypeIndex 
    {
        get { return _ConnectionTypeIndex; }
        set { SetProperty<short>(ref _ConnectionTypeIndex, value); }
    }

    int _BranchNo;
    public int BranchNo 
    {
        get { return _BranchNo; }
        set { SetProperty<int>(ref _BranchNo, value); }
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
            return _ACClassMethod != null;
        }
    }

    public virtual ReferenceEntry ACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("ACClassMethod"); }
    }
    
    private ACClassMethod _SourceACClassMethod;
    public virtual ACClassMethod SourceACClassMethod
    { 
        get { return LazyLoader.Load(this, ref _SourceACClassMethod); } 
        set { SetProperty<ACClassMethod>(ref _SourceACClassMethod, value); }
    }

    public bool SourceACClassMethod_IsLoaded
    {
        get
        {
            return _SourceACClassMethod != null;
        }
    }

    public virtual ReferenceEntry SourceACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("SourceACClassMethod"); }
    }
    
    private ACClassProperty _SourceACClassProperty;
    public virtual ACClassProperty SourceACClassProperty
    { 
        get { return LazyLoader.Load(this, ref _SourceACClassProperty); } 
        set { SetProperty<ACClassProperty>(ref _SourceACClassProperty, value); }
    }

    public bool SourceACClassProperty_IsLoaded
    {
        get
        {
            return _SourceACClassProperty != null;
        }
    }

    public virtual ReferenceEntry SourceACClassPropertyReference 
    {
        get { return Context.Entry(this).Reference("SourceACClassProperty"); }
    }
    
    private ACClassWF _SourceACClassWF;
    public virtual ACClassWF SourceACClassWF
    { 
        get { return LazyLoader.Load(this, ref _SourceACClassWF); } 
        set { SetProperty<ACClassWF>(ref _SourceACClassWF, value); }
    }

    public bool SourceACClassWF_IsLoaded
    {
        get
        {
            return _SourceACClassWF != null;
        }
    }

    public virtual ReferenceEntry SourceACClassWFReference 
    {
        get { return Context.Entry(this).Reference("SourceACClassWF"); }
    }
    
    private ACClassMethod _TargetACClassMethod;
    public virtual ACClassMethod TargetACClassMethod
    { 
        get { return LazyLoader.Load(this, ref _TargetACClassMethod); } 
        set { SetProperty<ACClassMethod>(ref _TargetACClassMethod, value); }
    }

    public bool TargetACClassMethod_IsLoaded
    {
        get
        {
            return _TargetACClassMethod != null;
        }
    }

    public virtual ReferenceEntry TargetACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("TargetACClassMethod"); }
    }
    
    private ACClassProperty _TargetACClassProperty;
    public virtual ACClassProperty TargetACClassProperty
    { 
        get { return LazyLoader.Load(this, ref _TargetACClassProperty); } 
        set { SetProperty<ACClassProperty>(ref _TargetACClassProperty, value); }
    }

    public bool TargetACClassProperty_IsLoaded
    {
        get
        {
            return _TargetACClassProperty != null;
        }
    }

    public virtual ReferenceEntry TargetACClassPropertyReference 
    {
        get { return Context.Entry(this).Reference("TargetACClassProperty"); }
    }
    
    private ACClassWF _TargetACClassWF;
    public virtual ACClassWF TargetACClassWF
    { 
        get { return LazyLoader.Load(this, ref _TargetACClassWF); } 
        set { SetProperty<ACClassWF>(ref _TargetACClassWF, value); }
    }

    public bool TargetACClassWF_IsLoaded
    {
        get
        {
            return _TargetACClassWF != null;
        }
    }

    public virtual ReferenceEntry TargetACClassWFReference 
    {
        get { return Context.Entry(this).Reference("TargetACClassWF"); }
    }
    }
