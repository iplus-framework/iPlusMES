using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PlanningMR : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public PlanningMR()
    {
    }

    private PlanningMR(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PlanningMRID;
    public Guid PlanningMRID 
    {
        get { return _PlanningMRID; }
        set { SetProperty<Guid>(ref _PlanningMRID, value); }
    }

    string _PlanningMRNo;
    public string PlanningMRNo 
    {
        get { return _PlanningMRNo; }
        set { SetProperty<string>(ref _PlanningMRNo, value); }
    }

    string _PlanningName;
    public string PlanningName 
    {
        get { return _PlanningName; }
        set { SetProperty<string>(ref _PlanningName, value); }
    }

    DateTime? _RangeFrom;
    public DateTime? RangeFrom 
    {
        get { return _RangeFrom; }
        set { SetProperty<DateTime?>(ref _RangeFrom, value); }
    }

    DateTime? _RangeTo;
    public DateTime? RangeTo 
    {
        get { return _RangeTo; }
        set { SetProperty<DateTime?>(ref _RangeTo, value); }
    }

    bool _Template;
    public bool Template 
    {
        get { return _Template; }
        set { SetProperty<bool>(ref _Template, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
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

    private ICollection<PlanningMRProposal> _PlanningMRProposal_PlanningMR;
    public virtual ICollection<PlanningMRProposal> PlanningMRProposal_PlanningMR
    {
        get { return LazyLoader.Load(this, ref _PlanningMRProposal_PlanningMR); }
        set { _PlanningMRProposal_PlanningMR = value; }
    }

    public bool PlanningMRProposal_PlanningMR_IsLoaded
    {
        get
        {
            return PlanningMRProposal_PlanningMR != null;
        }
    }

    public virtual CollectionEntry PlanningMRProposal_PlanningMRReference
    {
        get { return Context.Entry(this).Collection(c => c.PlanningMRProposal_PlanningMR); }
    }
}
