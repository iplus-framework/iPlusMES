using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBTranslationView : VBEntityObject 
{

    public VBTranslationView()
    {
    }

    private VBTranslationView(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    string _ACProjectName;
    public string ACProjectName 
    {
        get { return _ACProjectName; }
        set { SetProperty<string>(ref _ACProjectName, value); }
    }

    string _TableName;
    public string TableName 
    {
        get { return _TableName; }
        set { SetProperty<string>(ref _TableName, value); }
    }

    Guid _MandatoryID;
    public Guid MandatoryID 
    {
        get { return _MandatoryID; }
        set { SetProperty<Guid>(ref _MandatoryID, value); }
    }

    string _MandatoryACIdentifier;
    public string MandatoryACIdentifier 
    {
        get { return _MandatoryACIdentifier; }
        set { SetProperty<string>(ref _MandatoryACIdentifier, value); }
    }

    string _MandatoryACURLCached;
    public string MandatoryACURLCached 
    {
        get { return _MandatoryACURLCached; }
        set { SetProperty<string>(ref _MandatoryACURLCached, value); }
    }

    Guid _ID;
    public Guid ID 
    {
        get { return _ID; }
        set { SetProperty<Guid>(ref _ID, value); }
    }

    string _ACIdentifier;
    public override string ACIdentifier 
    {
        get { return _ACIdentifier; }
        set { SetProperty<string>(ref _ACIdentifier, value); }
    }

    string _TranslationValue;
    public string TranslationValue 
    {
        get { return _TranslationValue; }
        set { SetProperty<string>(ref _TranslationValue, value); }
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
}
