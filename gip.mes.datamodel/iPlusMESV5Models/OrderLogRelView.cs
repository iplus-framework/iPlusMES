using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OrderLogRelView : VBEntityObject 
{

    public OrderLogRelView()
    {
    }

    private OrderLogRelView(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _ACProgramLogID;
    public Guid ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid>(ref _ACProgramLogID, value); }
    }

    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
    }

    Guid? _ProdOrderPartslistPosRelationID;
    public Guid? ProdOrderPartslistPosRelationID 
    {
        get { return _ProdOrderPartslistPosRelationID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosRelationID, value); }
    }

    string _ACUrl;
    public string ACUrl 
    {
        get { return _ACUrl; }
        set { SetProperty<string>(ref _ACUrl, value); }
    }

    Guid? _RefACClassID;
    public Guid? RefACClassID 
    {
        get { return _RefACClassID; }
        set { SetProperty<Guid?>(ref _RefACClassID, value); }
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

    int _RelSequence;
    public int RelSequence 
    {
        get { return _RelSequence; }
        set { SetProperty<int>(ref _RelSequence, value); }
    }

    string _MaterialNo;
    public string MaterialNo 
    {
        get { return _MaterialNo; }
        set { SetProperty<string>(ref _MaterialNo, value); }
    }

    string _MaterialName1;
    public string MaterialName1 
    {
        get { return _MaterialName1; }
        set { SetProperty<string>(ref _MaterialName1, value); }
    }

    double _ActualQuantityUOM;
    public double ActualQuantityUOM 
    {
        get { return _ActualQuantityUOM; }
        set { SetProperty<double>(ref _ActualQuantityUOM, value); }
    }

    double _TargetQuantityUOM;
    public double TargetQuantityUOM 
    {
        get { return _TargetQuantityUOM; }
        set { SetProperty<double>(ref _TargetQuantityUOM, value); }
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

    short _MaterialPosTypeIndex;
    public short MaterialPosTypeIndex 
    {
        get { return _MaterialPosTypeIndex; }
        set { SetProperty<short>(ref _MaterialPosTypeIndex, value); }
    }

    string _PosMaterialNo;
    public string PosMaterialNo 
    {
        get { return _PosMaterialNo; }
        set { SetProperty<string>(ref _PosMaterialNo, value); }
    }

    string _PosMaterialName;
    public string PosMaterialName 
    {
        get { return _PosMaterialName; }
        set { SetProperty<string>(ref _PosMaterialName, value); }
    }

    double _PosTargetQuantityUOM;
    public double PosTargetQuantityUOM 
    {
        get { return _PosTargetQuantityUOM; }
        set { SetProperty<double>(ref _PosTargetQuantityUOM, value); }
    }

    double _PosActualQuantityUOM;
    public double PosActualQuantityUOM 
    {
        get { return _PosActualQuantityUOM; }
        set { SetProperty<double>(ref _PosActualQuantityUOM, value); }
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
