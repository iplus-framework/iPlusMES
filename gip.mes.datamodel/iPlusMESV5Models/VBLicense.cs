using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBLicense : VBEntityObject 
{

    public VBLicense()
    {
    }

    private VBLicense(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _VBLicenseID;
    public Guid VBLicenseID 
    {
        get { return _VBLicenseID; }
        set { SetProperty<Guid>(ref _VBLicenseID, value); }
    }

    string _LicenseNo;
    public string LicenseNo 
    {
        get { return _LicenseNo; }
        set { SetProperty<string>(ref _LicenseNo, value); }
    }

    string _ProjectNo;
    public string ProjectNo 
    {
        get { return _ProjectNo; }
        set { SetProperty<string>(ref _ProjectNo, value); }
    }

    string _CustomerName;
    public string CustomerName 
    {
        get { return _CustomerName; }
        set { SetProperty<string>(ref _CustomerName, value); }
    }

    byte[] _PackageSystem;
    public byte[] PackageSystem 
    {
        get { return _PackageSystem; }
        set { SetProperty<byte[]>(ref _PackageSystem, value); }
    }

    string _PackageSystem1;
    public string PackageSystem1 
    {
        get { return _PackageSystem1; }
        set { SetProperty<string>(ref _PackageSystem1, value); }
    }

    string _SystemDB;
    public string SystemDB 
    {
        get { return _SystemDB; }
        set { SetProperty<string>(ref _SystemDB, value); }
    }

    string _SystemDS;
    public string SystemDS 
    {
        get { return _SystemDS; }
        set { SetProperty<string>(ref _SystemDS, value); }
    }

    string _SystemRemote;
    public string SystemRemote 
    {
        get { return _SystemRemote; }
        set { SetProperty<string>(ref _SystemRemote, value); }
    }

    byte[] _SystemCommon;
    public byte[] SystemCommon 
    {
        get { return _SystemCommon; }
        set { SetProperty<byte[]>(ref _SystemCommon, value); }
    }

    byte[] _SystemCommon1;
    public byte[] SystemCommon1 
    {
        get { return _SystemCommon1; }
        set { SetProperty<byte[]>(ref _SystemCommon1, value); }
    }

    string _SystemKey;
    public string SystemKey 
    {
        get { return _SystemKey; }
        set { SetProperty<string>(ref _SystemKey, value); }
    }
}
