using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class LabelTranslation : VBEntityObject
{

    public LabelTranslation()
    {
    }

    private LabelTranslation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _LabelTranslationID;
    public Guid LabelTranslationID 
    {
        get { return _LabelTranslationID; }
        set { SetProperty<Guid>(ref _LabelTranslationID, value); }
    }

    Guid _VBLanguageID;
    public Guid VBLanguageID 
    {
        get { return _VBLanguageID; }
        set { SetProperty<Guid>(ref _VBLanguageID, value); }
    }

    Guid _LabelID;
    public Guid LabelID 
    {
        get { return _LabelID; }
        set { SetProperty<Guid>(ref _LabelID, value); }
    }

    string _Name;
    public string Name 
    {
        get { return _Name; }
        set { SetProperty<string>(ref _Name, value); }
    }

    string _Desc;
    public string Desc 
    {
        get { return _Desc; }
        set { SetProperty<string>(ref _Desc, value); }
    }

    private Label _Label;
    public virtual Label Label
    { 
        get { return LazyLoader.Load(this, ref _Label); } 
        set { SetProperty<Label>(ref _Label, value); }
    }

    public bool Label_IsLoaded
    {
        get
        {
            return _Label != null;
        }
    }

    public virtual ReferenceEntry LabelReference 
    {
        get { return Context.Entry(this).Reference("Label"); }
    }
    
    private VBLanguage _VBLanguage;
    public virtual VBLanguage VBLanguage
    { 
        get { return LazyLoader.Load(this, ref _VBLanguage); } 
        set { SetProperty<VBLanguage>(ref _VBLanguage, value); }
    }

    public bool VBLanguage_IsLoaded
    {
        get
        {
            return _VBLanguage != null;
        }
    }

    public virtual ReferenceEntry VBLanguageReference 
    {
        get { return Context.Entry(this).Reference("VBLanguage"); }
    }
    }
