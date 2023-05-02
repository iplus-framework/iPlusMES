using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class DbSyncerInfo : VBEntityObject
{

    public DbSyncerInfo()
    {
    }

    private DbSyncerInfo(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    int _DbSyncerInfoID;
    public int DbSyncerInfoID 
    {
        get { return _DbSyncerInfoID; }
        set { SetProperty<int>(ref _DbSyncerInfoID, value); }
    }

    string _DbSyncerInfoContextID;
    public string DbSyncerInfoContextID 
    {
        get { return _DbSyncerInfoContextID; }
        set { SetProperty<string>(ref _DbSyncerInfoContextID, value); }
    }

    DateTime _ScriptDate;
    public DateTime ScriptDate 
    {
        get { return _ScriptDate; }
        set { SetProperty<DateTime>(ref _ScriptDate, value); }
    }

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    string _UpdateAuthor;
    public string UpdateAuthor 
    {
        get { return _UpdateAuthor; }
        set { SetProperty<string>(ref _UpdateAuthor, value); }
    }

    private DbSyncerInfoContext _DbSyncerInfoContext;
    public virtual DbSyncerInfoContext DbSyncerInfoContext
    { 
        get => LazyLoader.Load(this, ref _DbSyncerInfoContext);
        set => _DbSyncerInfoContext = value;
    }

    public bool DbSyncerInfoContext_IsLoaded
    {
        get
        {
            return DbSyncerInfoContext != null;
        }
    }

    public virtual ReferenceEntry DbSyncerInfoContextReference 
    {
        get { return Context.Entry(this).Reference("DbSyncerInfoContext"); }
    }
    }
