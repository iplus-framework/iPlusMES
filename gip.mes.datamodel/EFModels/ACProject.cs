using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACProject : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACProject()
    {
    }

    private ACProject(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACProjectID;
    public Guid ACProjectID 
    {
        get { return _ACProjectID; }
        set { SetProperty<Guid>(ref _ACProjectID, value); }
    }

    string _ACProjectNo;
    public string ACProjectNo 
    {
        get { return _ACProjectNo; }
        set { SetProperty<string>(ref _ACProjectNo, value); }
    }

    string _ACProjectName;
    public string ACProjectName 
    {
        get { return _ACProjectName; }
        set { SetProperty<string>(ref _ACProjectName, value); }
    }

    short _ACProjectTypeIndex;
    public short ACProjectTypeIndex 
    {
        get { return _ACProjectTypeIndex; }
        set { SetProperty<short>(ref _ACProjectTypeIndex, value); }
    }

    Guid? _BasedOnACProjectID;
    public Guid? BasedOnACProjectID 
    {
        get { return _BasedOnACProjectID; }
        set { SetForeignKeyProperty<Guid?>(ref _BasedOnACProjectID, value, "ACProject1_BasedOnACProject", _ACProject1_BasedOnACProject, ACProject1_BasedOnACProject != null ? ACProject1_BasedOnACProject.ACProjectID : default(Guid?)); }
    }

    Guid? _PAAppClassAssignmentACClassID;
    public Guid? PAAppClassAssignmentACClassID 
    {
        get { return _PAAppClassAssignmentACClassID; }
        set { SetForeignKeyProperty<Guid?>(ref _PAAppClassAssignmentACClassID, value, "PAAppClassAssignmentACClass", _PAAppClassAssignmentACClass, PAAppClassAssignmentACClass != null ? PAAppClassAssignmentACClass.ACClassID : default(Guid?)); }
    }

    bool _IsEnabled;
    public bool IsEnabled 
    {
        get { return _IsEnabled; }
        set { SetProperty<bool>(ref _IsEnabled, value); }
    }

    bool _IsGlobal;
    public bool IsGlobal 
    {
        get { return _IsGlobal; }
        set { SetProperty<bool>(ref _IsGlobal, value); }
    }

    bool _IsWorkflowEnabled;
    public bool IsWorkflowEnabled 
    {
        get { return _IsWorkflowEnabled; }
        set { SetProperty<bool>(ref _IsWorkflowEnabled, value); }
    }

    bool _IsControlCenterEnabled;
    public bool IsControlCenterEnabled 
    {
        get { return _IsControlCenterEnabled; }
        set { SetProperty<bool>(ref _IsControlCenterEnabled, value); }
    }

    bool _IsVisualisationEnabled;
    public bool IsVisualisationEnabled 
    {
        get { return _IsVisualisationEnabled; }
        set { SetProperty<bool>(ref _IsVisualisationEnabled, value); }
    }

    bool _IsProduction;
    public bool IsProduction 
    {
        get { return _IsProduction; }
        set { SetProperty<bool>(ref _IsProduction, value); }
    }

    bool _IsDataAccess;
    public bool IsDataAccess 
    {
        get { return _IsDataAccess; }
        set { SetProperty<bool>(ref _IsDataAccess, value); }
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

    private ICollection<ACClass> _ACClass_ACProject;
    public virtual ICollection<ACClass> ACClass_ACProject
    {
        get { return LazyLoader.Load(this, ref _ACClass_ACProject); }
        set { SetProperty<ICollection<ACClass>>(ref _ACClass_ACProject, value); }
    }

    public bool ACClass_ACProject_IsLoaded
    {
        get
        {
            return _ACClass_ACProject != null;
        }
    }

    public virtual CollectionEntry ACClass_ACProjectReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClass_ACProject); }
    }

    private ACProject _ACProject1_BasedOnACProject;
    public virtual ACProject ACProject1_BasedOnACProject
    { 
        get { return LazyLoader.Load(this, ref _ACProject1_BasedOnACProject); } 
        set { SetProperty<ACProject>(ref _ACProject1_BasedOnACProject, value); }
    }

    public bool ACProject1_BasedOnACProject_IsLoaded
    {
        get
        {
            return _ACProject1_BasedOnACProject != null;
        }
    }

    public virtual ReferenceEntry ACProject1_BasedOnACProjectReference 
    {
        get { return Context.Entry(this).Reference("ACProject1_BasedOnACProject"); }
    }
    
    private ICollection<CalendarShift> _CalendarShift_VBiACProject;
    public virtual ICollection<CalendarShift> CalendarShift_VBiACProject
    {
        get { return LazyLoader.Load(this, ref _CalendarShift_VBiACProject); }
        set { SetProperty<ICollection<CalendarShift>>(ref _CalendarShift_VBiACProject, value); }
    }

    public bool CalendarShift_VBiACProject_IsLoaded
    {
        get
        {
            return _CalendarShift_VBiACProject != null;
        }
    }

    public virtual CollectionEntry CalendarShift_VBiACProjectReference
    {
        get { return Context.Entry(this).Collection(c => c.CalendarShift_VBiACProject); }
    }

    private ICollection<ACProject> _ACProject_BasedOnACProject;
    public virtual ICollection<ACProject> ACProject_BasedOnACProject
    {
        get { return LazyLoader.Load(this, ref _ACProject_BasedOnACProject); }
        set { SetProperty<ICollection<ACProject>>(ref _ACProject_BasedOnACProject, value); }
    }

    public bool ACProject_BasedOnACProject_IsLoaded
    {
        get
        {
            return _ACProject_BasedOnACProject != null;
        }
    }

    public virtual CollectionEntry ACProject_BasedOnACProjectReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProject_BasedOnACProject); }
    }

    private ACClass _PAAppClassAssignmentACClass;
    public virtual ACClass PAAppClassAssignmentACClass
    { 
        get { return LazyLoader.Load(this, ref _PAAppClassAssignmentACClass); } 
        set { SetProperty<ACClass>(ref _PAAppClassAssignmentACClass, value); }
    }

    public bool PAAppClassAssignmentACClass_IsLoaded
    {
        get
        {
            return _PAAppClassAssignmentACClass != null;
        }
    }

    public virtual ReferenceEntry PAAppClassAssignmentACClassReference 
    {
        get { return Context.Entry(this).Reference("PAAppClassAssignmentACClass"); }
    }
    
    private ICollection<VBUserACProject> _VBUserACProject_ACProject;
    public virtual ICollection<VBUserACProject> VBUserACProject_ACProject
    {
        get { return LazyLoader.Load(this, ref _VBUserACProject_ACProject); }
        set { SetProperty<ICollection<VBUserACProject>>(ref _VBUserACProject_ACProject, value); }
    }

    public bool VBUserACProject_ACProject_IsLoaded
    {
        get
        {
            return _VBUserACProject_ACProject != null;
        }
    }

    public virtual CollectionEntry VBUserACProject_ACProjectReference
    {
        get { return Context.Entry(this).Collection(c => c.VBUserACProject_ACProject); }
    }
}
