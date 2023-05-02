using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class DbSyncerInfoContext : VBEntityObject
{

    public DbSyncerInfoContext()
    {
    }

    private DbSyncerInfoContext(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    string _DbSyncerInfoContextID;
    public string DbSyncerInfoContextID 
    {
        get { return _DbSyncerInfoContextID; }
        set { SetProperty<string>(ref _DbSyncerInfoContextID, value); }
    }

    string _Name;
    public string Name 
    {
        get { return _Name; }
        set { SetProperty<string>(ref _Name, value); }
    }

    string _ConnectionName;
    public string ConnectionName 
    {
        get { return _ConnectionName; }
        set { SetProperty<string>(ref _ConnectionName, value); }
    }

    int _Order;
    public int Order 
    {
        get { return _Order; }
        set { SetProperty<int>(ref _Order, value); }
    }

    private ICollection<DbSyncerInfo> _DbSyncerInfo_DbSyncerInfoContext;
    public virtual ICollection<DbSyncerInfo> DbSyncerInfo_DbSyncerInfoContext
    {
        get => LazyLoader.Load(this, ref _DbSyncerInfo_DbSyncerInfoContext);
        set => _DbSyncerInfo_DbSyncerInfoContext = value;
    }

    public bool DbSyncerInfo_DbSyncerInfoContext_IsLoaded
    {
        get
        {
            return DbSyncerInfo_DbSyncerInfoContext != null;
        }
    }

    public virtual CollectionEntry DbSyncerInfo_DbSyncerInfoContextReference
    {
        get { return Context.Entry(this).Collection(c => c.DbSyncerInfo_DbSyncerInfoContext); }
    }
}
