using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OrderLogPosMachine : VBEntityObject 
{

    public OrderLogPosMachine()
    {
    }

    private OrderLogPosMachine(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid? _ChildACProgramLogID;
    public Guid? ChildACProgramLogID 
    {
        get { return _ChildACProgramLogID; }
        set { SetProperty<Guid?>(ref _ChildACProgramLogID, value); }
    }

    string _ACUrl;
    public string ACUrl 
    {
        get { return _ACUrl; }
        set { SetProperty<string>(ref _ACUrl, value); }
    }

    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
    }

    Guid _ACClassID;
    public Guid ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid>(ref _ACClassID, value); }
    }

    string _MachineName;
    public string MachineName 
    {
        get { return _MachineName; }
        set { SetProperty<string>(ref _MachineName, value); }
    }

    Guid? _BasedOnACClassID;
    public Guid? BasedOnACClassID 
    {
        get { return _BasedOnACClassID; }
        set { SetProperty<Guid?>(ref _BasedOnACClassID, value); }
    }

    string _BasedOnMachine;
    public string BasedOnMachine 
    {
        get { return _BasedOnMachine; }
        set { SetProperty<string>(ref _BasedOnMachine, value); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    int _PosSequence;
    public int PosSequence 
    {
        get { return _PosSequence; }
        set { SetProperty<int>(ref _PosSequence, value); }
    }

    string _PosBatchNo;
    public string PosBatchNo 
    {
        get { return _PosBatchNo; }
        set { SetProperty<string>(ref _PosBatchNo, value); }
    }

    double _ActualQuantityUOM;
    public double ActualQuantityUOM 
    {
        get { return _ActualQuantityUOM; }
        set { SetProperty<double>(ref _ActualQuantityUOM, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    int _PartslistSequence;
    public int PartslistSequence 
    {
        get { return _PartslistSequence; }
        set { SetProperty<int>(ref _PartslistSequence, value); }
    }

    string _ProdOrderProgramNo;
    public string ProdOrderProgramNo 
    {
        get { return _ProdOrderProgramNo; }
        set { SetProperty<string>(ref _ProdOrderProgramNo, value); }
    }
}
