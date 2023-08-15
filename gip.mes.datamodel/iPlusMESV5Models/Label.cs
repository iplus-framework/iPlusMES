using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Label : VBEntityObject
{

    public Label()
    {
    }

    private Label(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
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

    private ObservableHashSet<LabelTranslation> _LabelTranslation_Label;
    public virtual ObservableHashSet<LabelTranslation> LabelTranslation_Label
    {
        get => LazyLoader.Load(this, ref _LabelTranslation_Label);
        set => _LabelTranslation_Label = value;
    }

    public bool LabelTranslation_Label_IsLoaded
    {
        get
        {
            return LabelTranslation_Label != null;
        }
    }

    public virtual CollectionEntry LabelTranslation_LabelReference
    {
        get { return Context.Entry(this).Collection(c => c.LabelTranslation_Label); }
    }

    private ObservableHashSet<Material> _Material_Label;
    public virtual ObservableHashSet<Material> Material_Label
    {
        get => LazyLoader.Load(this, ref _Material_Label);
        set => _Material_Label = value;
    }

    public bool Material_Label_IsLoaded
    {
        get
        {
            return Material_Label != null;
        }
    }

    public virtual CollectionEntry Material_LabelReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_Label); }
    }
}
