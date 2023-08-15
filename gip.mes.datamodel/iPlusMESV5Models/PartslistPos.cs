using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PartslistPos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence, ITargetQuantity
{

    public PartslistPos()
    {
    }

    private PartslistPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PartslistPosID;
    public Guid PartslistPosID 
    {
        get { return _PartslistPosID; }
        set { SetProperty<Guid>(ref _PartslistPosID, value); }
    }

    Guid _PartslistID;
    public Guid PartslistID 
    {
        get { return _PartslistID; }
        set { SetProperty<Guid>(ref _PartslistID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    int _SequenceProduction;
    public int SequenceProduction 
    {
        get { return _SequenceProduction; }
        set { SetProperty<int>(ref _SequenceProduction, value); }
    }

    short _MaterialPosTypeIndex;
    public short MaterialPosTypeIndex 
    {
        get { return _MaterialPosTypeIndex; }
        set { SetProperty<short>(ref _MaterialPosTypeIndex, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    double _TargetQuantityUOM;
    public double TargetQuantityUOM 
    {
        get { return _TargetQuantityUOM; }
        set { SetProperty<double>(ref _TargetQuantityUOM, value); }
    }

    Guid? _MDUnitID;
    public Guid? MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid?>(ref _MDUnitID, value); }
    }

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
    }

    bool _IsBaseQuantityExcluded;
    public bool IsBaseQuantityExcluded 
    {
        get { return _IsBaseQuantityExcluded; }
        set { SetProperty<bool>(ref _IsBaseQuantityExcluded, value); }
    }

    Guid? _ParentPartslistPosID;
    public Guid? ParentPartslistPosID 
    {
        get { return _ParentPartslistPosID; }
        set { SetProperty<Guid?>(ref _ParentPartslistPosID, value); }
    }

    Guid? _AlternativePartslistPosID;
    public Guid? AlternativePartslistPosID 
    {
        get { return _AlternativePartslistPosID; }
        set { SetProperty<Guid?>(ref _AlternativePartslistPosID, value); }
    }

    /// <summary>
    /// Selected partslist for production this position from partslist with same output material.
    /// </summary>
    Guid? _ParentPartslistID;
    public Guid? ParentPartslistID 
    {
        get { return _ParentPartslistID; }
        set { SetProperty<Guid?>(ref _ParentPartslistID, value); }
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

    string _LineNumber;
    public string LineNumber 
    {
        get { return _LineNumber; }
        set { SetProperty<string>(ref _LineNumber, value); }
    }

    bool? _RetrogradeFIFO;
    public bool? RetrogradeFIFO 
    {
        get { return _RetrogradeFIFO; }
        set { SetProperty<bool?>(ref _RetrogradeFIFO, value); }
    }

    bool? _ExplosionOff;
    public bool? ExplosionOff 
    {
        get { return _ExplosionOff; }
        set { SetProperty<bool?>(ref _ExplosionOff, value); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    bool? _Anterograde;
    public bool? Anterograde 
    {
        get { return _Anterograde; }
        set { SetProperty<bool?>(ref _Anterograde, value); }
    }

    short? _PostingQuantitySuggestion;
    public short? PostingQuantitySuggestion 
    {
        get { return _PostingQuantitySuggestion; }
        set { SetProperty<short?>(ref _PostingQuantitySuggestion, value); }
    }

    private PartslistPos _PartslistPos1_AlternativePartslistPos;
    public virtual PartslistPos PartslistPos1_AlternativePartslistPos
    { 
        get => LazyLoader.Load(this, ref _PartslistPos1_AlternativePartslistPos);
        set => _PartslistPos1_AlternativePartslistPos = value;
    }

    public bool PartslistPos1_AlternativePartslistPos_IsLoaded
    {
        get
        {
            return PartslistPos1_AlternativePartslistPos != null;
        }
    }

    public virtual ReferenceEntry PartslistPos1_AlternativePartslistPosReference 
    {
        get { return Context.Entry(this).Reference("PartslistPos1_AlternativePartslistPos"); }
    }
    
    private ObservableHashSet<PartslistPos> _PartslistPos_AlternativePartslistPos;
    public virtual ObservableHashSet<PartslistPos> PartslistPos_AlternativePartslistPos
    {
        get => LazyLoader.Load(this, ref _PartslistPos_AlternativePartslistPos);
        set => _PartslistPos_AlternativePartslistPos = value;
    }

    public bool PartslistPos_AlternativePartslistPos_IsLoaded
    {
        get
        {
            return PartslistPos_AlternativePartslistPos != null;
        }
    }

    public virtual CollectionEntry PartslistPos_AlternativePartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPos_AlternativePartslistPos); }
    }

    private ObservableHashSet<PartslistPos> _PartslistPos_ParentPartslistPos;
    public virtual ObservableHashSet<PartslistPos> PartslistPos_ParentPartslistPos
    {
        get => LazyLoader.Load(this, ref _PartslistPos_ParentPartslistPos);
        set => _PartslistPos_ParentPartslistPos = value;
    }

    public bool PartslistPos_ParentPartslistPos_IsLoaded
    {
        get
        {
            return PartslistPos_ParentPartslistPos != null;
        }
    }

    public virtual CollectionEntry PartslistPos_ParentPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPos_ParentPartslistPos); }
    }

    private MDUnit _MDUnit;
    public virtual MDUnit MDUnit
    { 
        get => LazyLoader.Load(this, ref _MDUnit);
        set => _MDUnit = value;
    }

    public bool MDUnit_IsLoaded
    {
        get
        {
            return MDUnit != null;
        }
    }

    public virtual ReferenceEntry MDUnitReference 
    {
        get { return Context.Entry(this).Reference("MDUnit"); }
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
    
    private Partslist _ParentPartslist;
    public virtual Partslist ParentPartslist
    { 
        get => LazyLoader.Load(this, ref _ParentPartslist);
        set => _ParentPartslist = value;
    }

    public bool ParentPartslist_IsLoaded
    {
        get
        {
            return ParentPartslist != null;
        }
    }

    public virtual ReferenceEntry ParentPartslistReference 
    {
        get { return Context.Entry(this).Reference("ParentPartslist"); }
    }
    
    private PartslistPos _PartslistPos1_ParentPartslistPos;
    public virtual PartslistPos PartslistPos1_ParentPartslistPos
    { 
        get => LazyLoader.Load(this, ref _PartslistPos1_ParentPartslistPos);
        set => _PartslistPos1_ParentPartslistPos = value;
    }

    public bool PartslistPos1_ParentPartslistPos_IsLoaded
    {
        get
        {
            return PartslistPos1_ParentPartslistPos != null;
        }
    }

    public virtual ReferenceEntry PartslistPos1_ParentPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("PartslistPos1_ParentPartslistPos"); }
    }
    
    private Partslist _Partslist;
    public virtual Partslist Partslist
    { 
        get => LazyLoader.Load(this, ref _Partslist);
        set => _Partslist = value;
    }

    public bool Partslist_IsLoaded
    {
        get
        {
            return Partslist != null;
        }
    }

    public virtual ReferenceEntry PartslistReference 
    {
        get { return Context.Entry(this).Reference("Partslist"); }
    }
    
    private ObservableHashSet<PartslistPosRelation> _PartslistPosRelation_SourcePartslistPos;
    public virtual ObservableHashSet<PartslistPosRelation> PartslistPosRelation_SourcePartslistPos
    {
        get => LazyLoader.Load(this, ref _PartslistPosRelation_SourcePartslistPos);
        set => _PartslistPosRelation_SourcePartslistPos = value;
    }

    public bool PartslistPosRelation_SourcePartslistPos_IsLoaded
    {
        get
        {
            return PartslistPosRelation_SourcePartslistPos != null;
        }
    }

    public virtual CollectionEntry PartslistPosRelation_SourcePartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPosRelation_SourcePartslistPos); }
    }

    private ObservableHashSet<PartslistPosRelation> _PartslistPosRelation_TargetPartslistPos;
    public virtual ObservableHashSet<PartslistPosRelation> PartslistPosRelation_TargetPartslistPos
    {
        get => LazyLoader.Load(this, ref _PartslistPosRelation_TargetPartslistPos);
        set => _PartslistPosRelation_TargetPartslistPos = value;
    }

    public bool PartslistPosRelation_TargetPartslistPos_IsLoaded
    {
        get
        {
            return PartslistPosRelation_TargetPartslistPos != null;
        }
    }

    public virtual CollectionEntry PartslistPosRelation_TargetPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPosRelation_TargetPartslistPos); }
    }

    private ObservableHashSet<PartslistPosSplit> _PartslistPosSplit_PartslistPos;
    public virtual ObservableHashSet<PartslistPosSplit> PartslistPosSplit_PartslistPos
    {
        get => LazyLoader.Load(this, ref _PartslistPosSplit_PartslistPos);
        set => _PartslistPosSplit_PartslistPos = value;
    }

    public bool PartslistPosSplit_PartslistPos_IsLoaded
    {
        get
        {
            return PartslistPosSplit_PartslistPos != null;
        }
    }

    public virtual CollectionEntry PartslistPosSplit_PartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPosSplit_PartslistPos); }
    }

    private ObservableHashSet<ProdOrderPartslistPos> _ProdOrderPartslistPos_BasedOnPartslistPos;
    public virtual ObservableHashSet<ProdOrderPartslistPos> ProdOrderPartslistPos_BasedOnPartslistPos
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos_BasedOnPartslistPos);
        set => _ProdOrderPartslistPos_BasedOnPartslistPos = value;
    }

    public bool ProdOrderPartslistPos_BasedOnPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_BasedOnPartslistPos != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_BasedOnPartslistPosReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_BasedOnPartslistPos); }
    }
}
