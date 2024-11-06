using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBNoConfiguration : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public VBNoConfiguration()
    {
    }

    private VBNoConfiguration(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _VBNoConfigurationID;
    public Guid VBNoConfigurationID 
    {
        get { return _VBNoConfigurationID; }
        set { SetProperty<Guid>(ref _VBNoConfigurationID, value); }
    }

    string _VBNoConfigurationName;
    public string VBNoConfigurationName 
    {
        get { return _VBNoConfigurationName; }
        set { SetProperty<string>(ref _VBNoConfigurationName, value); }
    }

    string _UsedPrefix;
    public string UsedPrefix 
    {
        get { return _UsedPrefix; }
        set { SetProperty<string>(ref _UsedPrefix, value); }
    }

    string _UsedDelimiter;
    public string UsedDelimiter 
    {
        get { return _UsedDelimiter; }
        set { SetProperty<string>(ref _UsedDelimiter, value); }
    }

    bool _UseDate;
    public bool UseDate 
    {
        get { return _UseDate; }
        set { SetProperty<bool>(ref _UseDate, value); }
    }

    int _MinCounter;
    public int MinCounter 
    {
        get { return _MinCounter; }
        set { SetProperty<int>(ref _MinCounter, value); }
    }

    int _MaxCounter;
    public int MaxCounter 
    {
        get { return _MaxCounter; }
        set { SetProperty<int>(ref _MaxCounter, value); }
    }

    DateTime _CurrentDate;
    public DateTime CurrentDate 
    {
        get { return _CurrentDate; }
        set { SetProperty<DateTime>(ref _CurrentDate, value); }
    }

    int _CurrentCounter;
    public int CurrentCounter 
    {
        get { return _CurrentCounter; }
        set { SetProperty<int>(ref _CurrentCounter, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
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

    byte[] _RowVersion;
    public byte[] RowVersion 
    {
        get { return _RowVersion; }
        set { SetProperty<byte[]>(ref _RowVersion, value); }
    }
}
