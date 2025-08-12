using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassPropertyRelation : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACClassPropertyRelation()
    {
    }

    private ACClassPropertyRelation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACClassPropertyRelationID;
    public Guid ACClassPropertyRelationID 
    {
        get { return _ACClassPropertyRelationID; }
        set { SetProperty<Guid>(ref _ACClassPropertyRelationID, value); }
    }

    Guid _SourceACClassID;
    public Guid SourceACClassID 
    {
        get { return _SourceACClassID; }
        set { SetForeignKeyProperty<Guid>(ref _SourceACClassID, value, "SourceACClass", _SourceACClass, SourceACClass != null ? SourceACClass.ACClassID : default(Guid)); }
    }

    Guid? _SourceACClassPropertyID;
    public Guid? SourceACClassPropertyID 
    {
        get { return _SourceACClassPropertyID; }
        set { SetForeignKeyProperty<Guid?>(ref _SourceACClassPropertyID, value, "SourceACClassProperty", _SourceACClassProperty, SourceACClassProperty != null ? SourceACClassProperty.ACClassPropertyID : default(Guid?)); }
    }

    Guid _TargetACClassID;
    public Guid TargetACClassID 
    {
        get { return _TargetACClassID; }
        set { SetForeignKeyProperty<Guid>(ref _TargetACClassID, value, "TargetACClass", _TargetACClass, TargetACClass != null ? TargetACClass.ACClassID : default(Guid)); }
    }

    Guid? _TargetACClassPropertyID;
    public Guid? TargetACClassPropertyID 
    {
        get { return _TargetACClassPropertyID; }
        set { SetForeignKeyProperty<Guid?>(ref _TargetACClassPropertyID, value, "TargetACClassProperty", _TargetACClassProperty, TargetACClassProperty != null ? TargetACClassProperty.ACClassPropertyID : default(Guid?)); }
    }

    short _ConnectionTypeIndex;
    public short ConnectionTypeIndex 
    {
        get { return _ConnectionTypeIndex; }
        set { SetProperty<short>(ref _ConnectionTypeIndex, value); }
    }

    short _DirectionIndex;
    public short DirectionIndex 
    {
        get { return _DirectionIndex; }
        set { SetProperty<short>(ref _DirectionIndex, value); }
    }

    string _XMLValue;
    public string XMLValue 
    {
        get { return _XMLValue; }
        set { SetProperty<string>(ref _XMLValue, value); }
    }

    short _LogicalOperationIndex;
    public short LogicalOperationIndex 
    {
        get { return _LogicalOperationIndex; }
        set { SetProperty<short>(ref _LogicalOperationIndex, value); }
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

    double? _Multiplier;
    public double? Multiplier 
    {
        get { return _Multiplier; }
        set { SetProperty<double?>(ref _Multiplier, value); }
    }

    double? _Divisor;
    public double? Divisor 
    {
        get { return _Divisor; }
        set { SetProperty<double?>(ref _Divisor, value); }
    }

    string _ConvExpressionT;
    public string ConvExpressionT 
    {
        get { return _ConvExpressionT; }
        set { SetProperty<string>(ref _ConvExpressionT, value); }
    }

    string _ConvExpressionS;
    public string ConvExpressionS 
    {
        get { return _ConvExpressionS; }
        set { SetProperty<string>(ref _ConvExpressionS, value); }
    }

    short _RelationWeight;
    public short RelationWeight 
    {
        get { return _RelationWeight; }
        set { SetProperty<short>(ref _RelationWeight, value); }
    }

    short _UseFactor;
    public short UseFactor 
    {
        get { return _UseFactor; }
        set { SetProperty<short>(ref _UseFactor, value); }
    }

    DateTime _LastManipulationDT;
    public DateTime LastManipulationDT 
    {
        get { return _LastManipulationDT; }
        set { SetProperty<DateTime>(ref _LastManipulationDT, value); }
    }

    bool _IsDeactivated;
    public bool IsDeactivated 
    {
        get { return _IsDeactivated; }
        set { SetProperty<bool>(ref _IsDeactivated, value); }
    }

    short? _DisplayGroup;
    public short? DisplayGroup 
    {
        get { return _DisplayGroup; }
        set { SetProperty<short?>(ref _DisplayGroup, value); }
    }

    string _GroupName;
    public string GroupName 
    {
        get { return _GroupName; }
        set { SetProperty<string>(ref _GroupName, value); }
    }

    string _StateName;
    public string StateName 
    {
        get { return _StateName; }
        set { SetProperty<string>(ref _StateName, value); }
    }

    private ICollection<ACClassConfig> _ACClassConfig_ACClassPropertyRelation;
    public virtual ICollection<ACClassConfig> ACClassConfig_ACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _ACClassConfig_ACClassPropertyRelation); }
        set { SetProperty<ICollection<ACClassConfig>>(ref _ACClassConfig_ACClassPropertyRelation, value); }
    }

    public bool ACClassConfig_ACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _ACClassConfig_ACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry ACClassConfig_ACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassConfig_ACClassPropertyRelation); }
    }

    private ICollection<ACClassMethodConfig> _ACClassMethodConfig_VBiACClassPropertyRelation;
    public virtual ICollection<ACClassMethodConfig> ACClassMethodConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _ACClassMethodConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<ACClassMethodConfig>>(ref _ACClassMethodConfig_VBiACClassPropertyRelation, value); }
    }

    public bool ACClassMethodConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _ACClassMethodConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry ACClassMethodConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassMethodConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<ACProgramConfig> _ACProgramConfig_ACClassPropertyRelation;
    public virtual ICollection<ACProgramConfig> ACProgramConfig_ACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _ACProgramConfig_ACClassPropertyRelation); }
        set { SetProperty<ICollection<ACProgramConfig>>(ref _ACProgramConfig_ACClassPropertyRelation, value); }
    }

    public bool ACProgramConfig_ACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _ACProgramConfig_ACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry ACProgramConfig_ACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgramConfig_ACClassPropertyRelation); }
    }

    private ICollection<HistoryConfig> _HistoryConfig_VBiACClassPropertyRelation;
    public virtual ICollection<HistoryConfig> HistoryConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _HistoryConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<HistoryConfig>>(ref _HistoryConfig_VBiACClassPropertyRelation, value); }
    }

    public bool HistoryConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _HistoryConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry HistoryConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.HistoryConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<InOrderConfig> _InOrderConfig_VBiACClassPropertyRelation;
    public virtual ICollection<InOrderConfig> InOrderConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _InOrderConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<InOrderConfig>>(ref _InOrderConfig_VBiACClassPropertyRelation, value); }
    }

    public bool InOrderConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _InOrderConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry InOrderConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<InRequestConfig> _InRequestConfig_VBiACClassPropertyRelation;
    public virtual ICollection<InRequestConfig> InRequestConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _InRequestConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<InRequestConfig>>(ref _InRequestConfig_VBiACClassPropertyRelation, value); }
    }

    public bool InRequestConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _InRequestConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry InRequestConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<MaterialConfig> _MaterialConfig_VBiACClassPropertyRelation;
    public virtual ICollection<MaterialConfig> MaterialConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _MaterialConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<MaterialConfig>>(ref _MaterialConfig_VBiACClassPropertyRelation, value); }
    }

    public bool MaterialConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _MaterialConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry MaterialConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<MaterialWFACClassMethodConfig> _MaterialWFACClassMethodConfig_VBiACClassPropertyRelation;
    public virtual ICollection<MaterialWFACClassMethodConfig> MaterialWFACClassMethodConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _MaterialWFACClassMethodConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<MaterialWFACClassMethodConfig>>(ref _MaterialWFACClassMethodConfig_VBiACClassPropertyRelation, value); }
    }

    public bool MaterialWFACClassMethodConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _MaterialWFACClassMethodConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry MaterialWFACClassMethodConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFACClassMethodConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<OutOfferConfig> _OutOfferConfig_VBiACClassPropertyRelation;
    public virtual ICollection<OutOfferConfig> OutOfferConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _OutOfferConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<OutOfferConfig>>(ref _OutOfferConfig_VBiACClassPropertyRelation, value); }
    }

    public bool OutOfferConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _OutOfferConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry OutOfferConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<OutOrderConfig> _OutOrderConfig_VBiACClassPropertyRelation;
    public virtual ICollection<OutOrderConfig> OutOrderConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _OutOrderConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<OutOrderConfig>>(ref _OutOrderConfig_VBiACClassPropertyRelation, value); }
    }

    public bool OutOrderConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _OutOrderConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry OutOrderConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<PartslistConfig> _PartslistConfig_VBiACClassPropertyRelation;
    public virtual ICollection<PartslistConfig> PartslistConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _PartslistConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<PartslistConfig>>(ref _PartslistConfig_VBiACClassPropertyRelation, value); }
    }

    public bool PartslistConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _PartslistConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry PartslistConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<PickingConfig> _PickingConfig_VBiACClassPropertyRelation;
    public virtual ICollection<PickingConfig> PickingConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _PickingConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<PickingConfig>>(ref _PickingConfig_VBiACClassPropertyRelation, value); }
    }

    public bool PickingConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _PickingConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry PickingConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<ProdOrderPartslistConfig> _ProdOrderPartslistConfig_VBiACClassPropertyRelation;
    public virtual ICollection<ProdOrderPartslistConfig> ProdOrderPartslistConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<ProdOrderPartslistConfig>>(ref _ProdOrderPartslistConfig_VBiACClassPropertyRelation, value); }
    }

    public bool ProdOrderPartslistConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _ProdOrderPartslistConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistConfig_VBiACClassPropertyRelation); }
    }

    private ACClass _SourceACClass;
    public virtual ACClass SourceACClass
    { 
        get { return LazyLoader.Load(this, ref _SourceACClass); } 
        set { SetProperty<ACClass>(ref _SourceACClass, value); }
    }

    public bool SourceACClass_IsLoaded
    {
        get
        {
            return _SourceACClass != null;
        }
    }

    public virtual ReferenceEntry SourceACClassReference 
    {
        get { return Context.Entry(this).Reference("SourceACClass"); }
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
    
    private ACClass _TargetACClass;
    public virtual ACClass TargetACClass
    { 
        get { return LazyLoader.Load(this, ref _TargetACClass); } 
        set { SetProperty<ACClass>(ref _TargetACClass, value); }
    }

    public bool TargetACClass_IsLoaded
    {
        get
        {
            return _TargetACClass != null;
        }
    }

    public virtual ReferenceEntry TargetACClassReference 
    {
        get { return Context.Entry(this).Reference("TargetACClass"); }
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
    
    private ICollection<TourplanConfig> _TourplanConfig_VBiACClassPropertyRelation;
    public virtual ICollection<TourplanConfig> TourplanConfig_VBiACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _TourplanConfig_VBiACClassPropertyRelation); }
        set { SetProperty<ICollection<TourplanConfig>>(ref _TourplanConfig_VBiACClassPropertyRelation, value); }
    }

    public bool TourplanConfig_VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _TourplanConfig_VBiACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry TourplanConfig_VBiACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanConfig_VBiACClassPropertyRelation); }
    }

    private ICollection<VBConfig> _VBConfig_ACClassPropertyRelation;
    public virtual ICollection<VBConfig> VBConfig_ACClassPropertyRelation
    {
        get { return LazyLoader.Load(this, ref _VBConfig_ACClassPropertyRelation); }
        set { SetProperty<ICollection<VBConfig>>(ref _VBConfig_ACClassPropertyRelation, value); }
    }

    public bool VBConfig_ACClassPropertyRelation_IsLoaded
    {
        get
        {
            return _VBConfig_ACClassPropertyRelation != null;
        }
    }

    public virtual CollectionEntry VBConfig_ACClassPropertyRelationReference
    {
        get { return Context.Entry(this).Collection(c => c.VBConfig_ACClassPropertyRelation); }
    }
}
