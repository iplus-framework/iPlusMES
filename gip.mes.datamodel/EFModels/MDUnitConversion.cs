using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDUnitConversion : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MDUnitConversion()
    {
    }

    private MDUnitConversion(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDUnitConversionID;
    public Guid MDUnitConversionID 
    {
        get { return _MDUnitConversionID; }
        set { SetProperty<Guid>(ref _MDUnitConversionID, value); }
    }

    Guid _MDUnitID;
    public Guid MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetForeignKeyProperty<Guid>(ref _MDUnitID, value, "MDUnit", _MDUnit, MDUnit != null ? MDUnit.MDUnitID : default(Guid)); }
    }

    Guid _ToMDUnitID;
    public Guid ToMDUnitID 
    {
        get { return _ToMDUnitID; }
        set { SetForeignKeyProperty<Guid>(ref _ToMDUnitID, value, "ToMDUnit", _ToMDUnit, ToMDUnit != null ? ToMDUnit.MDUnitID : default(Guid)); }
    }

    double _Multiplier;
    public double Multiplier 
    {
        get { return _Multiplier; }
        set { SetProperty<double>(ref _Multiplier, value); }
    }

    double _Divisor;
    public double Divisor 
    {
        get { return _Divisor; }
        set { SetProperty<double>(ref _Divisor, value); }
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

    private MDUnit _MDUnit;
    public virtual MDUnit MDUnit
    { 
        get { return LazyLoader.Load(this, ref _MDUnit); } 
        set { SetProperty<MDUnit>(ref _MDUnit, value); }
    }

    public bool MDUnit_IsLoaded
    {
        get
        {
            return _MDUnit != null;
        }
    }

    public virtual ReferenceEntry MDUnitReference 
    {
        get { return Context.Entry(this).Reference("MDUnit"); }
    }
    
    private MDUnit _ToMDUnit;
    public virtual MDUnit ToMDUnit
    { 
        get { return LazyLoader.Load(this, ref _ToMDUnit); } 
        set { SetProperty<MDUnit>(ref _ToMDUnit, value); }
    }

    public bool ToMDUnit_IsLoaded
    {
        get
        {
            return _ToMDUnit != null;
        }
    }

    public virtual ReferenceEntry ToMDUnitReference 
    {
        get { return Context.Entry(this).Reference("ToMDUnit"); }
    }
    }
