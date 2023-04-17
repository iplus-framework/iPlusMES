using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBUser : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public VBUser()
    {
    }

    private VBUser(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _VBUserID;
    public Guid VBUserID 
    {
        get { return _VBUserID; }
        set { SetProperty<Guid>(ref _VBUserID, value); }
    }

    string _VBUserNo;
    public string VBUserNo 
    {
        get { return _VBUserNo; }
        set { SetProperty<string>(ref _VBUserNo, value); }
    }

    string _VBUserName;
    public string VBUserName 
    {
        get { return _VBUserName; }
        set { SetProperty<string>(ref _VBUserName, value); }
    }

    string _Initials;
    public string Initials 
    {
        get { return _Initials; }
        set { SetProperty<string>(ref _Initials, value); }
    }

    string _Password;
    public string Password 
    {
        get { return _Password; }
        set { SetProperty<string>(ref _Password, value); }
    }

    bool _AllowChangePW;
    public bool AllowChangePW 
    {
        get { return _AllowChangePW; }
        set { SetProperty<bool>(ref _AllowChangePW, value); }
    }

    Guid? _MenuACClassDesignID;
    public Guid? MenuACClassDesignID 
    {
        get { return _MenuACClassDesignID; }
        set { SetProperty<Guid?>(ref _MenuACClassDesignID, value); }
    }

    Guid _VBLanguageID;
    public Guid VBLanguageID 
    {
        get { return _VBLanguageID; }
        set { SetProperty<Guid>(ref _VBLanguageID, value); }
    }

    bool _IsSuperuser;
    public bool IsSuperuser 
    {
        get { return _IsSuperuser; }
        set { SetProperty<bool>(ref _IsSuperuser, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
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

    private ICollection<ACChangeLog> _ACChangeLog_VBUser;
    public virtual ICollection<ACChangeLog> ACChangeLog_VBUser
    {
        get => LazyLoader.Load(this, ref _ACChangeLog_VBUser);
        set => _ACChangeLog_VBUser = value;
    }

    public bool ACChangeLog_VBUser_IsLoaded
    {
        get
        {
            return ACChangeLog_VBUser != null;
        }
    }

    public virtual CollectionEntry ACChangeLog_VBUserReference
    {
        get { return Context.Entry(this).Collection(c => c.ACChangeLog_VBUser); }
    }

    private ICollection<ACClassTaskValue> _ACClassTaskValue_VBUser;
    public virtual ICollection<ACClassTaskValue> ACClassTaskValue_VBUser
    {
        get => LazyLoader.Load(this, ref _ACClassTaskValue_VBUser);
        set => _ACClassTaskValue_VBUser = value;
    }

    public bool ACClassTaskValue_VBUser_IsLoaded
    {
        get
        {
            return ACClassTaskValue_VBUser != null;
        }
    }

    public virtual CollectionEntry ACClassTaskValue_VBUserReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassTaskValue_VBUser); }
    }

    private ICollection<Company> _Company_VBUser;
    public virtual ICollection<Company> Company_VBUser
    {
        get => LazyLoader.Load(this, ref _Company_VBUser);
        set => _Company_VBUser = value;
    }

    public bool Company_VBUser_IsLoaded
    {
        get
        {
            return Company_VBUser != null;
        }
    }

    public virtual CollectionEntry Company_VBUserReference
    {
        get { return Context.Entry(this).Collection(c => c.Company_VBUser); }
    }

    private ACClassDesign _MenuACClassDesign;
    public virtual ACClassDesign MenuACClassDesign
    { 
        get => LazyLoader.Load(this, ref _MenuACClassDesign);
        set => _MenuACClassDesign = value;
    }

    public bool MenuACClassDesign_IsLoaded
    {
        get
        {
            return MenuACClassDesign != null;
        }
    }

    public virtual ReferenceEntry MenuACClassDesignReference 
    {
        get { return Context.Entry(this).Reference("MenuACClassDesign"); }
    }
    
    private ICollection<UserSettings> _UserSetting_VBUser;
    public virtual ICollection<UserSettings> UserSetting_VBUser
    {
        get => LazyLoader.Load(this, ref _UserSetting_VBUser);
        set => _UserSetting_VBUser = value;
    }

    public bool UserSetting_VBUser_IsLoaded
    {
        get
        {
            return UserSetting_VBUser != null;
        }
    }

    public virtual CollectionEntry UserSetting_VBUserReference
    {
        get { return Context.Entry(this).Collection(c => c.UserSetting_VBUser); }
    }

    private VBLanguage _VBLanguage;
    public virtual VBLanguage VBLanguage
    { 
        get => LazyLoader.Load(this, ref _VBLanguage);
        set => _VBLanguage = value;
    }

    public bool VBLanguage_IsLoaded
    {
        get
        {
            return VBLanguage != null;
        }
    }

    public virtual ReferenceEntry VBLanguageReference 
    {
        get { return Context.Entry(this).Reference("VBLanguage"); }
    }
    
    private ICollection<VBUserACClassDesign> _VBUserACClassDesign_VBUser;
    public virtual ICollection<VBUserACClassDesign> VBUserACClassDesign_VBUser
    {
        get => LazyLoader.Load(this, ref _VBUserACClassDesign_VBUser);
        set => _VBUserACClassDesign_VBUser = value;
    }

    public bool VBUserACClassDesign_VBUser_IsLoaded
    {
        get
        {
            return VBUserACClassDesign_VBUser != null;
        }
    }

    public virtual CollectionEntry VBUserACClassDesign_VBUserReference
    {
        get { return Context.Entry(this).Collection(c => c.VBUserACClassDesign_VBUser); }
    }

    private ICollection<VBUserACProject> _VBUserACProject_VBUser;
    public virtual ICollection<VBUserACProject> VBUserACProject_VBUser
    {
        get => LazyLoader.Load(this, ref _VBUserACProject_VBUser);
        set => _VBUserACProject_VBUser = value;
    }

    public bool VBUserACProject_VBUser_IsLoaded
    {
        get
        {
            return VBUserACProject_VBUser != null;
        }
    }

    public virtual CollectionEntry VBUserACProject_VBUserReference
    {
        get { return Context.Entry(this).Collection(c => c.VBUserACProject_VBUser); }
    }

    private ICollection<VBUserGroup> _VBUserGroup_VBUser;
    public virtual ICollection<VBUserGroup> VBUserGroup_VBUser
    {
        get => LazyLoader.Load(this, ref _VBUserGroup_VBUser);
        set => _VBUserGroup_VBUser = value;
    }

    public bool VBUserGroup_VBUser_IsLoaded
    {
        get
        {
            return VBUserGroup_VBUser != null;
        }
    }

    public virtual CollectionEntry VBUserGroup_VBUserReference
    {
        get { return Context.Entry(this).Collection(c => c.VBUserGroup_VBUser); }
    }

    private ICollection<VBUserInstance> _VBUserInstance_VBUser;
    public virtual ICollection<VBUserInstance> VBUserInstance_VBUser
    {
        get => LazyLoader.Load(this, ref _VBUserInstance_VBUser);
        set => _VBUserInstance_VBUser = value;
    }

    public bool VBUserInstance_VBUser_IsLoaded
    {
        get
        {
            return VBUserInstance_VBUser != null;
        }
    }

    public virtual CollectionEntry VBUserInstance_VBUserReference
    {
        get { return Context.Entry(this).Collection(c => c.VBUserInstance_VBUser); }
    }
}
