using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDGMPAdditive : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDGMPAdditive()
    {
    }

    private MDGMPAdditive(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDGMPAdditiveID;
    public Guid MDGMPAdditiveID 
    {
        get { return _MDGMPAdditiveID; }
        set { SetProperty<Guid>(ref _MDGMPAdditiveID, value); }
    }

    string _MDGMPAdditiveNo;
    public string MDGMPAdditiveNo 
    {
        get { return _MDGMPAdditiveNo; }
        set { SetProperty<string>(ref _MDGMPAdditiveNo, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    float _DeleteLevel;
    public float DeleteLevel 
    {
        get { return _DeleteLevel; }
        set { SetProperty<float>(ref _DeleteLevel, value); }
    }

    Guid? _MDQuantityUnitID;
    public Guid? MDQuantityUnitID 
    {
        get { return _MDQuantityUnitID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDQuantityUnitID, value, "MDQuantityUnit", _MDQuantityUnit, MDQuantityUnit != null ? MDQuantityUnit.MDUnitID : default(Guid?)); }
    }

    float _SafetyFactor;
    public float SafetyFactor 
    {
        get { return _SafetyFactor; }
        set { SetProperty<float>(ref _SafetyFactor, value); }
    }

    Guid? _MDProcessErrorActionID;
    public Guid? MDProcessErrorActionID 
    {
        get { return _MDProcessErrorActionID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDProcessErrorActionID, value, "MDProcessErrorAction", _MDProcessErrorAction, MDProcessErrorAction != null ? MDProcessErrorAction.MDProcessErrorActionID : default(Guid?)); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    private ICollection<MDGMPMaterialGroupPos> _MDGMPMaterialGroupPos_MDGMPAdditive;
    public virtual ICollection<MDGMPMaterialGroupPos> MDGMPMaterialGroupPos_MDGMPAdditive
    {
        get { return LazyLoader.Load(this, ref _MDGMPMaterialGroupPos_MDGMPAdditive); }
        set { SetProperty<ICollection<MDGMPMaterialGroupPos>>(ref _MDGMPMaterialGroupPos_MDGMPAdditive, value); }
    }

    public bool MDGMPMaterialGroupPos_MDGMPAdditive_IsLoaded
    {
        get
        {
            return _MDGMPMaterialGroupPos_MDGMPAdditive != null;
        }
    }

    public virtual CollectionEntry MDGMPMaterialGroupPos_MDGMPAdditiveReference
    {
        get { return Context.Entry(this).Collection(c => c.MDGMPMaterialGroupPos_MDGMPAdditive); }
    }

    private MDProcessErrorAction _MDProcessErrorAction;
    public virtual MDProcessErrorAction MDProcessErrorAction
    { 
        get { return LazyLoader.Load(this, ref _MDProcessErrorAction); } 
        set { SetProperty<MDProcessErrorAction>(ref _MDProcessErrorAction, value); }
    }

    public bool MDProcessErrorAction_IsLoaded
    {
        get
        {
            return _MDProcessErrorAction != null;
        }
    }

    public virtual ReferenceEntry MDProcessErrorActionReference 
    {
        get { return Context.Entry(this).Reference("MDProcessErrorAction"); }
    }
    
    private MDUnit _MDQuantityUnit;
    public virtual MDUnit MDQuantityUnit
    { 
        get { return LazyLoader.Load(this, ref _MDQuantityUnit); } 
        set { SetProperty<MDUnit>(ref _MDQuantityUnit, value); }
    }

    public bool MDQuantityUnit_IsLoaded
    {
        get
        {
            return _MDQuantityUnit != null;
        }
    }

    public virtual ReferenceEntry MDQuantityUnitReference 
    {
        get { return Context.Entry(this).Reference("MDQuantityUnit"); }
    }
    
    private ICollection<MaterialGMPAdditive> _MaterialGMPAdditive_MDGMPAdditive;
    public virtual ICollection<MaterialGMPAdditive> MaterialGMPAdditive_MDGMPAdditive
    {
        get { return LazyLoader.Load(this, ref _MaterialGMPAdditive_MDGMPAdditive); }
        set { SetProperty<ICollection<MaterialGMPAdditive>>(ref _MaterialGMPAdditive_MDGMPAdditive, value); }
    }

    public bool MaterialGMPAdditive_MDGMPAdditive_IsLoaded
    {
        get
        {
            return _MaterialGMPAdditive_MDGMPAdditive != null;
        }
    }

    public virtual CollectionEntry MaterialGMPAdditive_MDGMPAdditiveReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialGMPAdditive_MDGMPAdditive); }
    }
}
