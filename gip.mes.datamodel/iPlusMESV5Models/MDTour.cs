using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDTour : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDTour()
    {
    }

    private MDTour(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDTourID;
    public Guid MDTourID 
    {
        get { return _MDTourID; }
        set { SetProperty<Guid>(ref _MDTourID, value); }
    }

    string _MDTourNo;
    public string MDTourNo 
    {
        get { return _MDTourNo; }
        set { SetProperty<string>(ref _MDTourNo, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    bool _Monday;
    public bool Monday 
    {
        get { return _Monday; }
        set { SetProperty<bool>(ref _Monday, value); }
    }

    bool _Tuesday;
    public bool Tuesday 
    {
        get { return _Tuesday; }
        set { SetProperty<bool>(ref _Tuesday, value); }
    }

    bool _Wednesday;
    public bool Wednesday 
    {
        get { return _Wednesday; }
        set { SetProperty<bool>(ref _Wednesday, value); }
    }

    bool _Thursday;
    public bool Thursday 
    {
        get { return _Thursday; }
        set { SetProperty<bool>(ref _Thursday, value); }
    }

    bool _Friday;
    public bool Friday 
    {
        get { return _Friday; }
        set { SetProperty<bool>(ref _Friday, value); }
    }

    bool _Saturday;
    public bool Saturday 
    {
        get { return _Saturday; }
        set { SetProperty<bool>(ref _Saturday, value); }
    }

    bool _Sunday;
    public bool Sunday 
    {
        get { return _Sunday; }
        set { SetProperty<bool>(ref _Sunday, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    private ICollection<Tourplan> _Tourplan_MDTour;
    public virtual ICollection<Tourplan> Tourplan_MDTour
    {
        get => LazyLoader.Load(this, ref _Tourplan_MDTour);
        set => _Tourplan_MDTour = value;
    }

    public bool Tourplan_MDTour_IsLoaded
    {
        get
        {
            return Tourplan_MDTour != null;
        }
    }

    public virtual CollectionEntry Tourplan_MDTourReference
    {
        get { return Context.Entry(this).Collection(c => c.Tourplan_MDTour); }
    }
}
