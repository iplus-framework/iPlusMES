// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACAssembly : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACAssembly()
    {
    }

    private ACAssembly(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACAssemblyID;
    public Guid ACAssemblyID 
    {
        get { return _ACAssemblyID; }
        set { SetProperty<Guid>(ref _ACAssemblyID, value); }
    }

    string _AssemblyName;
    public string AssemblyName 
    {
        get { return _AssemblyName; }
        set { SetProperty<string>(ref _AssemblyName, value); }
    }

    DateTime _LastReflectionDate;
    public DateTime LastReflectionDate 
    {
        get { return _LastReflectionDate; }
        set { SetProperty<DateTime>(ref _LastReflectionDate, value); }
    }

    DateTime _AssemblyDate;
    public DateTime AssemblyDate 
    {
        get { return _AssemblyDate; }
        set { SetProperty<DateTime>(ref _AssemblyDate, value); }
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

    string _SHA1;
    public string SHA1 
    {
        get { return _SHA1; }
        set { SetProperty<string>(ref _SHA1, value); }
    }
}
