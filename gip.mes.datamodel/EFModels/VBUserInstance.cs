using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBUserInstance : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public VBUserInstance()
    {
    }

    private VBUserInstance(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _VBUserInstanceID;
    public Guid VBUserInstanceID 
    {
        get { return _VBUserInstanceID; }
        set { SetProperty<Guid>(ref _VBUserInstanceID, value); }
    }

    Guid _VBUserID;
    public Guid VBUserID 
    {
        get { return _VBUserID; }
        set { SetProperty<Guid>(ref _VBUserID, value); }
    }

    bool _IsUserDefined;
    public bool IsUserDefined 
    {
        get { return _IsUserDefined; }
        set { SetProperty<bool>(ref _IsUserDefined, value); }
    }

    string _ServerIPV4;
    public string ServerIPV4 
    {
        get { return _ServerIPV4; }
        set { SetProperty<string>(ref _ServerIPV4, value); }
    }

    string _ServerIPV6;
    public string ServerIPV6 
    {
        get { return _ServerIPV6; }
        set { SetProperty<string>(ref _ServerIPV6, value); }
    }

    int _ServicePortHTTP;
    public int ServicePortHTTP 
    {
        get { return _ServicePortHTTP; }
        set { SetProperty<int>(ref _ServicePortHTTP, value); }
    }

    int _ServicePortTCP;
    public int ServicePortTCP 
    {
        get { return _ServicePortTCP; }
        set { SetProperty<int>(ref _ServicePortTCP, value); }
    }

    int _ServicePortObserverHTTP;
    public int ServicePortObserverHTTP 
    {
        get { return _ServicePortObserverHTTP; }
        set { SetProperty<int>(ref _ServicePortObserverHTTP, value); }
    }

    bool _ServiceAppEnbledHTTP;
    public bool ServiceAppEnbledHTTP 
    {
        get { return _ServiceAppEnbledHTTP; }
        set { SetProperty<bool>(ref _ServiceAppEnbledHTTP, value); }
    }

    bool _ServiceAppEnabledTCP;
    public bool ServiceAppEnabledTCP 
    {
        get { return _ServiceAppEnabledTCP; }
        set { SetProperty<bool>(ref _ServiceAppEnabledTCP, value); }
    }

    bool _ServiceWorkflowEnabledHTTP;
    public bool ServiceWorkflowEnabledHTTP 
    {
        get { return _ServiceWorkflowEnabledHTTP; }
        set { SetProperty<bool>(ref _ServiceWorkflowEnabledHTTP, value); }
    }

    bool _ServiceWorkflowEnabledTCP;
    public bool ServiceWorkflowEnabledTCP 
    {
        get { return _ServiceWorkflowEnabledTCP; }
        set { SetProperty<bool>(ref _ServiceWorkflowEnabledTCP, value); }
    }

    bool _ServiceObserverEnabledTCP;
    public bool ServiceObserverEnabledTCP 
    {
        get { return _ServiceObserverEnabledTCP; }
        set { SetProperty<bool>(ref _ServiceObserverEnabledTCP, value); }
    }

    string _Hostname;
    public string Hostname 
    {
        get { return _Hostname; }
        set { SetProperty<string>(ref _Hostname, value); }
    }

    bool _NameResolutionOn;
    public bool NameResolutionOn 
    {
        get { return _NameResolutionOn; }
        set { SetProperty<bool>(ref _NameResolutionOn, value); }
    }

    bool _UseIPV6;
    public bool UseIPV6 
    {
        get { return _UseIPV6; }
        set { SetProperty<bool>(ref _UseIPV6, value); }
    }

    bool _UseTextEncoding;
    public bool UseTextEncoding 
    {
        get { return _UseTextEncoding; }
        set { SetProperty<bool>(ref _UseTextEncoding, value); }
    }

    DateTime? _LoginDate;
    public DateTime? LoginDate 
    {
        get { return _LoginDate; }
        set { SetProperty<DateTime?>(ref _LoginDate, value); }
    }

    DateTime? _LogoutDate;
    public DateTime? LogoutDate 
    {
        get { return _LogoutDate; }
        set { SetProperty<DateTime?>(ref _LogoutDate, value); }
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

    string _SessionInfo;
    public string SessionInfo 
    {
        get { return _SessionInfo; }
        set { SetProperty<string>(ref _SessionInfo, value); }
    }

    int _SessionCount;
    public int SessionCount 
    {
        get { return _SessionCount; }
        set { SetProperty<int>(ref _SessionCount, value); }
    }

    private VBUser _VBUser;
    public virtual VBUser VBUser
    { 
        get { return LazyLoader.Load(this, ref _VBUser); } 
        set { SetProperty<VBUser>(ref _VBUser, value); }
    }

    public bool VBUser_IsLoaded
    {
        get
        {
            return _VBUser != null;
        }
    }

    public virtual ReferenceEntry VBUserReference 
    {
        get { return Context.Entry(this).Reference("VBUser"); }
    }
    }
