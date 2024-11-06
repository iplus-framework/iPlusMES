using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class History : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public History()
    {
    }

    private History(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _HistoryID;
    public Guid HistoryID 
    {
        get { return _HistoryID; }
        set { SetProperty<Guid>(ref _HistoryID, value); }
    }

    short _TimePeriodIndex;
    public short TimePeriodIndex 
    {
        get { return _TimePeriodIndex; }
        set { SetProperty<short>(ref _TimePeriodIndex, value); }
    }

    int _PeriodNo;
    public int PeriodNo 
    {
        get { return _PeriodNo; }
        set { SetProperty<int>(ref _PeriodNo, value); }
    }

    DateTime _BalanceDate;
    public DateTime BalanceDate 
    {
        get { return _BalanceDate; }
        set { SetProperty<DateTime>(ref _BalanceDate, value); }
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

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    private ICollection<CompanyMaterialHistory> _CompanyMaterialHistory_History;
    public virtual ICollection<CompanyMaterialHistory> CompanyMaterialHistory_History
    {
        get { return LazyLoader.Load(this, ref _CompanyMaterialHistory_History); }
        set { _CompanyMaterialHistory_History = value; }
    }

    public bool CompanyMaterialHistory_History_IsLoaded
    {
        get
        {
            return CompanyMaterialHistory_History != null;
        }
    }

    public virtual CollectionEntry CompanyMaterialHistory_HistoryReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterialHistory_History); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_History;
    public virtual ICollection<FacilityBooking> FacilityBooking_History
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_History); }
        set { _FacilityBooking_History = value; }
    }

    public bool FacilityBooking_History_IsLoaded
    {
        get
        {
            return FacilityBooking_History != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_HistoryReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_History); }
    }

    private ICollection<FacilityHistory> _FacilityHistory_History;
    public virtual ICollection<FacilityHistory> FacilityHistory_History
    {
        get { return LazyLoader.Load(this, ref _FacilityHistory_History); }
        set { _FacilityHistory_History = value; }
    }

    public bool FacilityHistory_History_IsLoaded
    {
        get
        {
            return FacilityHistory_History != null;
        }
    }

    public virtual CollectionEntry FacilityHistory_HistoryReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityHistory_History); }
    }

    private ICollection<HistoryConfig> _HistoryConfig_History;
    public virtual ICollection<HistoryConfig> HistoryConfig_History
    {
        get { return LazyLoader.Load(this, ref _HistoryConfig_History); }
        set { _HistoryConfig_History = value; }
    }

    public bool HistoryConfig_History_IsLoaded
    {
        get
        {
            return HistoryConfig_History != null;
        }
    }

    public virtual CollectionEntry HistoryConfig_HistoryReference
    {
        get { return Context.Entry(this).Collection(c => c.HistoryConfig_History); }
    }

    private ICollection<MaterialHistory> _MaterialHistory_History;
    public virtual ICollection<MaterialHistory> MaterialHistory_History
    {
        get { return LazyLoader.Load(this, ref _MaterialHistory_History); }
        set { _MaterialHistory_History = value; }
    }

    public bool MaterialHistory_History_IsLoaded
    {
        get
        {
            return MaterialHistory_History != null;
        }
    }

    public virtual CollectionEntry MaterialHistory_HistoryReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialHistory_History); }
    }
}
