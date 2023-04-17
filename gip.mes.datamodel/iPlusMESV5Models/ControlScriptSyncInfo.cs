using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ControlScriptSyncInfo : VBEntityObject 
{

    public ControlScriptSyncInfo()
    {
    }

    private ControlScriptSyncInfo(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    int _ControlScriptSyncInfoID;
    public int ControlScriptSyncInfoID 
    {
        get { return _ControlScriptSyncInfoID; }
        set { SetProperty<int>(ref _ControlScriptSyncInfoID, value); }
    }

    DateTime _VersionTime;
    public DateTime VersionTime 
    {
        get { return _VersionTime; }
        set { SetProperty<DateTime>(ref _VersionTime, value); }
    }

    DateTime _UpdateTime;
    public DateTime UpdateTime 
    {
        get { return _UpdateTime; }
        set { SetProperty<DateTime>(ref _UpdateTime, value); }
    }

    string _UpdateAuthor;
    public string UpdateAuthor 
    {
        get { return _UpdateAuthor; }
        set { SetProperty<string>(ref _UpdateAuthor, value); }
    }
}
