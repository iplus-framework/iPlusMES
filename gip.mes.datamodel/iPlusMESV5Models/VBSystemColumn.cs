using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class VBSystemColumn : VBEntityObject 
{

    public VBSystemColumn()
    {
    }

    private VBSystemColumn(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    string _tablename;
    public string tablename 
    {
        get { return _tablename; }
        set { SetProperty<string>(ref _tablename, value); }
    }

    string _columnname;
    public string columnname 
    {
        get { return _columnname; }
        set { SetProperty<string>(ref _columnname, value); }
    }

    string _columntype;
    public string columntype 
    {
        get { return _columntype; }
        set { SetProperty<string>(ref _columntype, value); }
    }

    short? _columnlength;
    public short? columnlength 
    {
        get { return _columnlength; }
        set { SetProperty<short?>(ref _columnlength, value); }
    }

    int? _columnnullable;
    public int? columnnullable 
    {
        get { return _columnnullable; }
        set { SetProperty<int?>(ref _columnnullable, value); }
    }
}
