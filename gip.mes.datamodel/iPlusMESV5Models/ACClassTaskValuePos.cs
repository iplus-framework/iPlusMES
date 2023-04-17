using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassTaskValuePos : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public ACClassTaskValuePos()
    {
    }

    private ACClassTaskValuePos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _ACClassTaskValuePosID;
    public Guid ACClassTaskValuePosID 
    {
        get { return _ACClassTaskValuePosID; }
        set { SetProperty<Guid>(ref _ACClassTaskValuePosID, value); }
    }

    Guid _ACClassTaskValueID;
    public Guid ACClassTaskValueID 
    {
        get { return _ACClassTaskValueID; }
        set { SetProperty<Guid>(ref _ACClassTaskValueID, value); }
    }

    string _ACUrl;
    public string ACUrl 
    {
        get { return _ACUrl; }
        set { SetProperty<string>(ref _ACUrl, value); }
    }

    short _StateIndex;
    public short StateIndex 
    {
        get { return _StateIndex; }
        set { SetProperty<short>(ref _StateIndex, value); }
    }

    bool _InProcess;
    public bool InProcess 
    {
        get { return _InProcess; }
        set { SetProperty<bool>(ref _InProcess, value); }
    }

    long _SequenceNo;
    public long SequenceNo 
    {
        get { return _SequenceNo; }
        set { SetProperty<long>(ref _SequenceNo, value); }
    }

    string _ClientPointName;
    public string ClientPointName 
    {
        get { return _ClientPointName; }
        set { SetProperty<string>(ref _ClientPointName, value); }
    }

    string _AsyncCallbackDelegateName;
    public string AsyncCallbackDelegateName 
    {
        get { return _AsyncCallbackDelegateName; }
        set { SetProperty<string>(ref _AsyncCallbackDelegateName, value); }
    }

    Guid _RequestID;
    public Guid RequestID 
    {
        get { return _RequestID; }
        set { SetProperty<Guid>(ref _RequestID, value); }
    }

    string _ACIdentifier;
    public override string ACIdentifier 
    {
        get { return _ACIdentifier; }
        set { SetProperty<string>(ref _ACIdentifier, value); }
    }

    string _XMLACMethod;
    public string XMLACMethod 
    {
        get { return _XMLACMethod; }
        set { SetProperty<string>(ref _XMLACMethod, value); }
    }

    string _ExecutingInstanceURL;
    public string ExecutingInstanceURL 
    {
        get { return _ExecutingInstanceURL; }
        set { SetProperty<string>(ref _ExecutingInstanceURL, value); }
    }

    bool _CallbackIsPending;
    public bool CallbackIsPending 
    {
        get { return _CallbackIsPending; }
        set { SetProperty<bool>(ref _CallbackIsPending, value); }
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

    private ACClassTaskValue _ACClassTaskValue;
    public virtual ACClassTaskValue ACClassTaskValue
    { 
        get => LazyLoader.Load(this, ref _ACClassTaskValue);
        set => _ACClassTaskValue = value;
    }

    public bool ACClassTaskValue_IsLoaded
    {
        get
        {
            return ACClassTaskValue != null;
        }
    }

    public virtual ReferenceEntry ACClassTaskValueReference 
    {
        get { return Context.Entry(this).Reference("ACClassTaskValue"); }
    }
    }
