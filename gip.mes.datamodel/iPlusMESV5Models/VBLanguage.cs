using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBLanguage : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public VBLanguage()
    {
    }

    private VBLanguage(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _VBLanguageID;
    public Guid VBLanguageID 
    {
        get { return _VBLanguageID; }
        set { SetProperty<Guid>(ref _VBLanguageID, value); }
    }

    string _VBLanguageCode;
    public string VBLanguageCode 
    {
        get { return _VBLanguageCode; }
        set { SetProperty<string>(ref _VBLanguageCode, value); }
    }

    string _VBNameTrans;
    public string VBNameTrans 
    {
        get { return _VBNameTrans; }
        set { SetProperty<string>(ref _VBNameTrans, value); }
    }

    string _VBKey;
    public string VBKey 
    {
        get { return _VBKey; }
        set { SetProperty<string>(ref _VBKey, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    bool _IsTranslation;
    public bool IsTranslation 
    {
        get { return _IsTranslation; }
        set { SetProperty<bool>(ref _IsTranslation, value); }
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

    private ICollection<LabelTranslation> _LabelTranslation_VBLanguage;
    public virtual ICollection<LabelTranslation> LabelTranslation_VBLanguage
    {
        get => LazyLoader.Load(this, ref _LabelTranslation_VBLanguage);
        set => _LabelTranslation_VBLanguage = value;
    }

    public bool LabelTranslation_VBLanguage_IsLoaded
    {
        get
        {
            return LabelTranslation_VBLanguage != null;
        }
    }

    public virtual CollectionEntry LabelTranslation_VBLanguageReference
    {
        get { return Context.Entry(this).Collection(c => c.LabelTranslation_VBLanguage); }
    }

    private ICollection<VBUser> _VBUser_VBLanguage;
    public virtual ICollection<VBUser> VBUser_VBLanguage
    {
        get => LazyLoader.Load(this, ref _VBUser_VBLanguage);
        set => _VBUser_VBLanguage = value;
    }

    public bool VBUser_VBLanguage_IsLoaded
    {
        get
        {
            return VBUser_VBLanguage != null;
        }
    }

    public virtual CollectionEntry VBUser_VBLanguageReference
    {
        get { return Context.Entry(this).Collection(c => c.VBUser_VBLanguage); }
    }
}
