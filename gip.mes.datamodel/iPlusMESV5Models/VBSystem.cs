using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBSystem : VBEntityObject 
{

    public VBSystem()
    {
    }

    private VBSystem(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _VBSystemID;
    public Guid VBSystemID 
    {
        get { return _VBSystemID; }
        set { SetProperty<Guid>(ref _VBSystemID, value); }
    }

    string _SystemName;
    public string SystemName 
    {
        get { return _SystemName; }
        set { SetProperty<string>(ref _SystemName, value); }
    }

    string _CustomerName;
    public string CustomerName 
    {
        get { return _CustomerName; }
        set { SetProperty<string>(ref _CustomerName, value); }
    }

    string _Company;
    public string Company 
    {
        get { return _Company; }
        set { SetProperty<string>(ref _Company, value); }
    }

    string _ProjectNo;
    public string ProjectNo 
    {
        get { return _ProjectNo; }
        set { SetProperty<string>(ref _ProjectNo, value); }
    }

    byte[] _SystemInternal;
    public byte[] SystemInternal 
    {
        get { return _SystemInternal; }
        set { SetProperty<byte[]>(ref _SystemInternal, value); }
    }

    byte[] _SystemInternal1;
    public byte[] SystemInternal1 
    {
        get { return _SystemInternal1; }
        set { SetProperty<byte[]>(ref _SystemInternal1, value); }
    }

    string _SystemInternal2;
    public string SystemInternal2 
    {
        get { return _SystemInternal2; }
        set { SetProperty<string>(ref _SystemInternal2, value); }
    }

    byte[] _SystemInternal3;
    public byte[] SystemInternal3 
    {
        get { return _SystemInternal3; }
        set { SetProperty<byte[]>(ref _SystemInternal3, value); }
    }

    string _SystemRemote;
    public string SystemRemote 
    {
        get { return _SystemRemote; }
        set { SetProperty<string>(ref _SystemRemote, value); }
    }

    string _SystemPrivate;
    public string SystemPrivate 
    {
        get { return _SystemPrivate; }
        set { SetProperty<string>(ref _SystemPrivate, value); }
    }

    byte[] _SystemCommon;
    public byte[] SystemCommon 
    {
        get { return _SystemCommon; }
        set { SetProperty<byte[]>(ref _SystemCommon, value); }
    }

    string _SystemCommonPublic;
    public string SystemCommonPublic 
    {
        get { return _SystemCommonPublic; }
        set { SetProperty<string>(ref _SystemCommonPublic, value); }
    }

    byte[] _SystemCommon1;
    public byte[] SystemCommon1 
    {
        get { return _SystemCommon1; }
        set { SetProperty<byte[]>(ref _SystemCommon1, value); }
    }
}
