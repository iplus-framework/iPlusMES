using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaterialWFACClassMethod : VBEntityObject
{

    public MaterialWFACClassMethod()
    {
    }

    private MaterialWFACClassMethod(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaterialWFACClassMethodID;
    public Guid MaterialWFACClassMethodID 
    {
        get { return _MaterialWFACClassMethodID; }
        set { SetProperty<Guid>(ref _MaterialWFACClassMethodID, value); }
    }

    Guid _MaterialWFID;
    public Guid MaterialWFID 
    {
        get { return _MaterialWFID; }
        set { SetProperty<Guid>(ref _MaterialWFID, value); }
    }

    Guid _ACClassMethodID;
    public Guid ACClassMethodID 
    {
        get { return _ACClassMethodID; }
        set { SetProperty<Guid>(ref _ACClassMethodID, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    private ACClassMethod _ACClassMethod;
    public virtual ACClassMethod ACClassMethod
    { 
        get => LazyLoader.Load(this, ref _ACClassMethod);
        set => _ACClassMethod = value;
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
    
    private MaterialWF _MaterialWF;
    public virtual MaterialWF MaterialWF
    { 
        get => LazyLoader.Load(this, ref _MaterialWF);
        set => _MaterialWF = value;
    }

    public bool MaterialWF_IsLoaded
    {
        get
        {
            return MaterialWF != null;
        }
    }

    public virtual ReferenceEntry MaterialWFReference 
    {
        get { return Context.Entry(this).Reference("MaterialWF"); }
    }
    
    private ObservableHashSet<MaterialWFACClassMethodConfig> _MaterialWFACClassMethodConfig_MaterialWFACClassMethod;
    public virtual ObservableHashSet<MaterialWFACClassMethodConfig> MaterialWFACClassMethodConfig_MaterialWFACClassMethod
    {
        get => LazyLoader.Load(this, ref _MaterialWFACClassMethodConfig_MaterialWFACClassMethod);
        set => _MaterialWFACClassMethodConfig_MaterialWFACClassMethod = value;
    }

    public bool MaterialWFACClassMethodConfig_MaterialWFACClassMethod_IsLoaded
    {
        get
        {
            return MaterialWFACClassMethodConfig_MaterialWFACClassMethod != null;
        }
    }

    public virtual CollectionEntry MaterialWFACClassMethodConfig_MaterialWFACClassMethodReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFACClassMethodConfig_MaterialWFACClassMethod); }
    }

    private ObservableHashSet<MaterialWFConnection> _MaterialWFConnection_MaterialWFACClassMethod;
    public virtual ObservableHashSet<MaterialWFConnection> MaterialWFConnection_MaterialWFACClassMethod
    {
        get => LazyLoader.Load(this, ref _MaterialWFConnection_MaterialWFACClassMethod);
        set => _MaterialWFConnection_MaterialWFACClassMethod = value;
    }

    public bool MaterialWFConnection_MaterialWFACClassMethod_IsLoaded
    {
        get
        {
            return MaterialWFConnection_MaterialWFACClassMethod != null;
        }
    }

    public virtual CollectionEntry MaterialWFConnection_MaterialWFACClassMethodReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFConnection_MaterialWFACClassMethod); }
    }

    private ObservableHashSet<PartslistACClassMethod> _PartslistACClassMethod_MaterialWFACClassMethod;
    public virtual ObservableHashSet<PartslistACClassMethod> PartslistACClassMethod_MaterialWFACClassMethod
    {
        get => LazyLoader.Load(this, ref _PartslistACClassMethod_MaterialWFACClassMethod);
        set => _PartslistACClassMethod_MaterialWFACClassMethod = value;
    }

    public bool PartslistACClassMethod_MaterialWFACClassMethod_IsLoaded
    {
        get
        {
            return PartslistACClassMethod_MaterialWFACClassMethod != null;
        }
    }

    public virtual CollectionEntry PartslistACClassMethod_MaterialWFACClassMethodReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistACClassMethod_MaterialWFACClassMethod); }
    }

    private ObservableHashSet<ProdOrderBatchPlan> _ProdOrderBatchPlan_MaterialWFACClassMethod;
    public virtual ObservableHashSet<ProdOrderBatchPlan> ProdOrderBatchPlan_MaterialWFACClassMethod
    {
        get => LazyLoader.Load(this, ref _ProdOrderBatchPlan_MaterialWFACClassMethod);
        set => _ProdOrderBatchPlan_MaterialWFACClassMethod = value;
    }

    public bool ProdOrderBatchPlan_MaterialWFACClassMethod_IsLoaded
    {
        get
        {
            return ProdOrderBatchPlan_MaterialWFACClassMethod != null;
        }
    }

    public virtual CollectionEntry ProdOrderBatchPlan_MaterialWFACClassMethodReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderBatchPlan_MaterialWFACClassMethod); }
    }
}
