using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACPropertyLogRule : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACPropertyLogRule()
    {
    }

    private ACPropertyLogRule(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACPropertyLogRuleID;
    public Guid ACPropertyLogRuleID 
    {
        get { return _ACPropertyLogRuleID; }
        set { SetProperty<Guid>(ref _ACPropertyLogRuleID, value); }
    }

    Guid _ACClassID;
    public Guid ACClassID 
    {
        get { return _ACClassID; }
        set { SetForeignKeyProperty<Guid>(ref _ACClassID, value, "ACClass", _ACClass, ACClass != null ? ACClass.ACClassID : default(Guid)); }
    }

    short _RuleType;
    public short RuleType 
    {
        get { return _RuleType; }
        set { SetProperty<short>(ref _RuleType, value); }
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

    private ACClass _ACClass;
    public virtual ACClass ACClass
    { 
        get { return LazyLoader.Load(this, ref _ACClass); } 
        set { SetProperty<ACClass>(ref _ACClass, value); }
    }

    public bool ACClass_IsLoaded
    {
        get
        {
            return _ACClass != null;
        }
    }

    public virtual ReferenceEntry ACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass"); }
    }
    }
