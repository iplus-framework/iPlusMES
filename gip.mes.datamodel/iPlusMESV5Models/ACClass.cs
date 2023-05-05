using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClass : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACClass()
    {
    }

    private ACClass(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACClassID;
    public Guid ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid>(ref _ACClassID, value); }
    }

    Guid _ACProjectID;
    public Guid ACProjectID 
    {
        get { return _ACProjectID; }
        set { SetProperty<Guid>(ref _ACProjectID, value); }
    }

    Guid? _BasedOnACClassID;
    public Guid? BasedOnACClassID 
    {
        get { return _BasedOnACClassID; }
        set { SetProperty<Guid?>(ref _BasedOnACClassID, value); }
    }

    Guid? _ParentACClassID;
    public Guid? ParentACClassID 
    {
        get { return _ParentACClassID; }
        set { SetProperty<Guid?>(ref _ParentACClassID, value); }
    }

    string _ACIdentifier;
    public override string ACIdentifier 
    {
        get { return _ACIdentifier; }
        set { SetProperty<string>(ref _ACIdentifier, value); }
    }

    int? _ACIdentifierKey;
    public int? ACIdentifierKey 
    {
        get { return _ACIdentifierKey; }
        set { SetProperty<int?>(ref _ACIdentifierKey, value); }
    }

    string _ACCaptionTranslation;
    public string ACCaptionTranslation 
    {
        get { return _ACCaptionTranslation; }
        set { SetProperty<string>(ref _ACCaptionTranslation, value); }
    }

    short _ACKindIndex;
    public short ACKindIndex 
    {
        get { return _ACKindIndex; }
        set { SetProperty<short>(ref _ACKindIndex, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
    }

    Guid _ACPackageID;
    public Guid ACPackageID 
    {
        get { return _ACPackageID; }
        set { SetProperty<Guid>(ref _ACPackageID, value); }
    }

    string _AssemblyQualifiedName;
    public string AssemblyQualifiedName 
    {
        get { return _AssemblyQualifiedName; }
        set { SetProperty<string>(ref _AssemblyQualifiedName, value); }
    }

    Guid? _PWACClassID;
    public Guid? PWACClassID 
    {
        get { return _PWACClassID; }
        set { SetProperty<Guid?>(ref _PWACClassID, value); }
    }

    Guid? _PWMethodACClassID;
    public Guid? PWMethodACClassID 
    {
        get { return _PWMethodACClassID; }
        set { SetProperty<Guid?>(ref _PWMethodACClassID, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    bool _IsAutostart;
    public bool IsAutostart 
    {
        get { return _IsAutostart; }
        set { SetProperty<bool>(ref _IsAutostart, value); }
    }

    bool _IsAbstract;
    public bool IsAbstract 
    {
        get { return _IsAbstract; }
        set { SetProperty<bool>(ref _IsAbstract, value); }
    }

    short _ACStartTypeIndex;
    public short ACStartTypeIndex 
    {
        get { return _ACStartTypeIndex; }
        set { SetProperty<short>(ref _ACStartTypeIndex, value); }
    }

    short _ACStorableTypeIndex;
    public short ACStorableTypeIndex 
    {
        get { return _ACStorableTypeIndex; }
        set { SetProperty<short>(ref _ACStorableTypeIndex, value); }
    }

    bool _IsAssembly;
    public bool IsAssembly 
    {
        get { return _IsAssembly; }
        set { SetProperty<bool>(ref _IsAssembly, value); }
    }

    bool _IsMultiInstance;
    public bool IsMultiInstance 
    {
        get { return _IsMultiInstance; }
        set { SetProperty<bool>(ref _IsMultiInstance, value); }
    }

    bool _IsRightmanagement;
    public bool IsRightmanagement 
    {
        get { return _IsRightmanagement; }
        set { SetProperty<bool>(ref _IsRightmanagement, value); }
    }

    string _ACSortColumns;
    public string ACSortColumns 
    {
        get { return _ACSortColumns; }
        set { SetProperty<string>(ref _ACSortColumns, value); }
    }

    string _ACFilterColumns;
    public string ACFilterColumns 
    {
        get { return _ACFilterColumns; }
        set { SetProperty<string>(ref _ACFilterColumns, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    string _XMLACClass;
    public string XMLACClass 
    {
        get { return _XMLACClass; }
        set { SetProperty<string>(ref _XMLACClass, value); }
    }

    int _BranchNo;
    public int BranchNo 
    {
        get { return _BranchNo; }
        set { SetProperty<int>(ref _BranchNo, value); }
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

    int? _ChangeLogMax;
    public int? ChangeLogMax 
    {
        get { return _ChangeLogMax; }
        set { SetProperty<int?>(ref _ChangeLogMax, value); }
    }

    string _ACURLCached;
    public string ACURLCached 
    {
        get { return _ACURLCached; }
        set { SetProperty<string>(ref _ACURLCached, value); }
    }

    string _ACURLComponentCached;
    public string ACURLComponentCached 
    {
        get { return _ACURLComponentCached; }
        set { SetProperty<string>(ref _ACURLComponentCached, value); }
    }

    bool _IsStatic;
    public bool IsStatic 
    {
        get { return _IsStatic; }
        set { SetProperty<bool>(ref _IsStatic, value); }
    }

    private ICollection<ACChangeLog> _ACChangeLog_ACClass;
    public virtual ICollection<ACChangeLog> ACChangeLog_ACClass
    {
        get => LazyLoader.Load(this, ref _ACChangeLog_ACClass);
        set => _ACChangeLog_ACClass = value;
    }

    public bool ACChangeLog_ACClass_IsLoaded
    {
        get
        {
            return ACChangeLog_ACClass != null;
        }
    }

    public virtual CollectionEntry ACChangeLog_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACChangeLog_ACClass); }
    }

    private ICollection<ACClassConfig> _ACClassConfig_ACClass;
    public virtual ICollection<ACClassConfig> ACClassConfig_ACClass
    {
        get => LazyLoader.Load(this, ref _ACClassConfig_ACClass);
        set => _ACClassConfig_ACClass = value;
    }

    public bool ACClassConfig_ACClass_IsLoaded
    {
        get
        {
            return ACClassConfig_ACClass != null;
        }
    }

    public virtual CollectionEntry ACClassConfig_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassConfig_ACClass); }
    }

    private ICollection<ACClassConfig> _ACClassConfig_ValueTypeACClass;
    public virtual ICollection<ACClassConfig> ACClassConfig_ValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _ACClassConfig_ValueTypeACClass);
        set => _ACClassConfig_ValueTypeACClass = value;
    }

    public bool ACClassConfig_ValueTypeACClass_IsLoaded
    {
        get
        {
            return ACClassConfig_ValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry ACClassConfig_ValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassConfig_ValueTypeACClass); }
    }

    private ICollection<ACClassDesign> _ACClassDesign_ACClass;
    public virtual ICollection<ACClassDesign> ACClassDesign_ACClass
    {
        get => LazyLoader.Load(this, ref _ACClassDesign_ACClass);
        set => _ACClassDesign_ACClass = value;
    }

    public bool ACClassDesign_ACClass_IsLoaded
    {
        get
        {
            return ACClassDesign_ACClass != null;
        }
    }

    public virtual CollectionEntry ACClassDesign_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassDesign_ACClass); }
    }

    private ICollection<ACClassDesign> _ACClassDesign_ValueTypeACClass;
    public virtual ICollection<ACClassDesign> ACClassDesign_ValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _ACClassDesign_ValueTypeACClass);
        set => _ACClassDesign_ValueTypeACClass = value;
    }

    public bool ACClassDesign_ValueTypeACClass_IsLoaded
    {
        get
        {
            return ACClassDesign_ValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry ACClassDesign_ValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassDesign_ValueTypeACClass); }
    }

    private ICollection<ACClassMessage> _ACClassMessage_ACClass;
    public virtual ICollection<ACClassMessage> ACClassMessage_ACClass
    {
        get => LazyLoader.Load(this, ref _ACClassMessage_ACClass);
        set => _ACClassMessage_ACClass = value;
    }

    public bool ACClassMessage_ACClass_IsLoaded
    {
        get
        {
            return ACClassMessage_ACClass != null;
        }
    }

    public virtual CollectionEntry ACClassMessage_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassMessage_ACClass); }
    }

    private ICollection<ACClassMethod> _ACClassMethod_ACClass;
    public virtual ICollection<ACClassMethod> ACClassMethod_ACClass
    {
        get => LazyLoader.Load(this, ref _ACClassMethod_ACClass);
        set => _ACClassMethod_ACClass = value;
    }

    public bool ACClassMethod_ACClass_IsLoaded
    {
        get
        {
            return ACClassMethod_ACClass != null;
        }
    }

    public virtual CollectionEntry ACClassMethod_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassMethod_ACClass); }
    }

    private ICollection<ACClassMethod> _ACClassMethod_AttachedFromACClass;
    public virtual ICollection<ACClassMethod> ACClassMethod_AttachedFromACClass
    {
        get => LazyLoader.Load(this, ref _ACClassMethod_AttachedFromACClass);
        set => _ACClassMethod_AttachedFromACClass = value;
    }

    public bool ACClassMethod_AttachedFromACClass_IsLoaded
    {
        get
        {
            return ACClassMethod_AttachedFromACClass != null;
        }
    }

    public virtual CollectionEntry ACClassMethod_AttachedFromACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassMethod_AttachedFromACClass); }
    }

    private ICollection<ACClassMethodConfig> _ACClassMethodConfig_VBiACClass;
    public virtual ICollection<ACClassMethodConfig> ACClassMethodConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _ACClassMethodConfig_VBiACClass);
        set => _ACClassMethodConfig_VBiACClass = value;
    }

    public bool ACClassMethodConfig_VBiACClass_IsLoaded
    {
        get
        {
            return ACClassMethodConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry ACClassMethodConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassMethodConfig_VBiACClass); }
    }

    private ICollection<ACClassMethodConfig> _ACClassMethodConfig_ValueTypeACClass;
    public virtual ICollection<ACClassMethodConfig> ACClassMethodConfig_ValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _ACClassMethodConfig_ValueTypeACClass);
        set => _ACClassMethodConfig_ValueTypeACClass = value;
    }

    public bool ACClassMethodConfig_ValueTypeACClass_IsLoaded
    {
        get
        {
            return ACClassMethodConfig_ValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry ACClassMethodConfig_ValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassMethodConfig_ValueTypeACClass); }
    }

    private ICollection<ACClassMethod> _ACClassMethod_PWACClass;
    public virtual ICollection<ACClassMethod> ACClassMethod_PWACClass
    {
        get => LazyLoader.Load(this, ref _ACClassMethod_PWACClass);
        set => _ACClassMethod_PWACClass = value;
    }

    public bool ACClassMethod_PWACClass_IsLoaded
    {
        get
        {
            return ACClassMethod_PWACClass != null;
        }
    }

    public virtual CollectionEntry ACClassMethod_PWACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassMethod_PWACClass); }
    }

    private ICollection<ACClassMethod> _ACClassMethod_ValueTypeACClass;
    public virtual ICollection<ACClassMethod> ACClassMethod_ValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _ACClassMethod_ValueTypeACClass);
        set => _ACClassMethod_ValueTypeACClass = value;
    }

    public bool ACClassMethod_ValueTypeACClass_IsLoaded
    {
        get
        {
            return ACClassMethod_ValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry ACClassMethod_ValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassMethod_ValueTypeACClass); }
    }

    private ICollection<ACClassProperty> _ACClassProperty_ACClass;
    public virtual ICollection<ACClassProperty> ACClassProperty_ACClass
    {
        get => LazyLoader.Load(this, ref _ACClassProperty_ACClass);
        set => _ACClassProperty_ACClass = value;
    }

    public bool ACClassProperty_ACClass_IsLoaded
    {
        get
        {
            return ACClassProperty_ACClass != null;
        }
    }

    public virtual CollectionEntry ACClassProperty_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassProperty_ACClass); }
    }

    private ICollection<ACClassProperty> _ACClassProperty_ConfigACClass;
    public virtual ICollection<ACClassProperty> ACClassProperty_ConfigACClass
    {
        get => LazyLoader.Load(this, ref _ACClassProperty_ConfigACClass);
        set => _ACClassProperty_ConfigACClass = value;
    }

    public bool ACClassProperty_ConfigACClass_IsLoaded
    {
        get
        {
            return ACClassProperty_ConfigACClass != null;
        }
    }

    public virtual CollectionEntry ACClassProperty_ConfigACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassProperty_ConfigACClass); }
    }

    private ICollection<ACClassPropertyRelation> _ACClassPropertyRelation_SourceACClass;
    public virtual ICollection<ACClassPropertyRelation> ACClassPropertyRelation_SourceACClass
    {
        get => LazyLoader.Load(this, ref _ACClassPropertyRelation_SourceACClass);
        set => _ACClassPropertyRelation_SourceACClass = value;
    }

    public bool ACClassPropertyRelation_SourceACClass_IsLoaded
    {
        get
        {
            return ACClassPropertyRelation_SourceACClass != null;
        }
    }

    public virtual CollectionEntry ACClassPropertyRelation_SourceACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassPropertyRelation_SourceACClass); }
    }

    private ICollection<ACClassPropertyRelation> _ACClassPropertyRelation_TargetACClass;
    public virtual ICollection<ACClassPropertyRelation> ACClassPropertyRelation_TargetACClass
    {
        get => LazyLoader.Load(this, ref _ACClassPropertyRelation_TargetACClass);
        set => _ACClassPropertyRelation_TargetACClass = value;
    }

    public bool ACClassPropertyRelation_TargetACClass_IsLoaded
    {
        get
        {
            return ACClassPropertyRelation_TargetACClass != null;
        }
    }

    public virtual CollectionEntry ACClassPropertyRelation_TargetACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassPropertyRelation_TargetACClass); }
    }

    private ICollection<ACClassProperty> _ACClassProperty_ValueTypeACClass;
    public virtual ICollection<ACClassProperty> ACClassProperty_ValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _ACClassProperty_ValueTypeACClass);
        set => _ACClassProperty_ValueTypeACClass = value;
    }

    public bool ACClassProperty_ValueTypeACClass_IsLoaded
    {
        get
        {
            return ACClassProperty_ValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry ACClassProperty_ValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassProperty_ValueTypeACClass); }
    }

    private ICollection<ACClassTask> _ACClassTask_TaskTypeACClass;
    public virtual ICollection<ACClassTask> ACClassTask_TaskTypeACClass
    {
        get => LazyLoader.Load(this, ref _ACClassTask_TaskTypeACClass);
        set => _ACClassTask_TaskTypeACClass = value;
    }

    public bool ACClassTask_TaskTypeACClass_IsLoaded
    {
        get
        {
            return ACClassTask_TaskTypeACClass != null;
        }
    }

    public virtual CollectionEntry ACClassTask_TaskTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassTask_TaskTypeACClass); }
    }

    private ICollection<ACClassText> _ACClassText_ACClass;
    public virtual ICollection<ACClassText> ACClassText_ACClass
    {
        get => LazyLoader.Load(this, ref _ACClassText_ACClass);
        set => _ACClassText_ACClass = value;
    }

    public bool ACClassText_ACClass_IsLoaded
    {
        get
        {
            return ACClassText_ACClass != null;
        }
    }

    public virtual CollectionEntry ACClassText_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassText_ACClass); }
    }

    private ICollection<ACClassWF> _ACClassWF_PWACClass;
    public virtual ICollection<ACClassWF> ACClassWF_PWACClass
    {
        get => LazyLoader.Load(this, ref _ACClassWF_PWACClass);
        set => _ACClassWF_PWACClass = value;
    }

    public bool ACClassWF_PWACClass_IsLoaded
    {
        get
        {
            return ACClassWF_PWACClass != null;
        }
    }

    public virtual CollectionEntry ACClassWF_PWACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassWF_PWACClass); }
    }

    private ICollection<ACClassWF> _ACClassWF_RefPAACClass;
    public virtual ICollection<ACClassWF> ACClassWF_RefPAACClass
    {
        get => LazyLoader.Load(this, ref _ACClassWF_RefPAACClass);
        set => _ACClassWF_RefPAACClass = value;
    }

    public bool ACClassWF_RefPAACClass_IsLoaded
    {
        get
        {
            return ACClassWF_RefPAACClass != null;
        }
    }

    public virtual CollectionEntry ACClassWF_RefPAACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassWF_RefPAACClass); }
    }

    private ACPackage _ACPackage;
    public virtual ACPackage ACPackage
    { 
        get => LazyLoader.Load(this, ref _ACPackage);
        set => _ACPackage = value;
    }

    public bool ACPackage_IsLoaded
    {
        get
        {
            return ACPackage != null;
        }
    }

    public virtual ReferenceEntry ACPackageReference 
    {
        get { return Context.Entry(this).Reference("ACPackage"); }
    }
    
    private ICollection<ACProgramConfig> _ACProgramConfig_ACClass;
    public virtual ICollection<ACProgramConfig> ACProgramConfig_ACClass
    {
        get => LazyLoader.Load(this, ref _ACProgramConfig_ACClass);
        set => _ACProgramConfig_ACClass = value;
    }

    public bool ACProgramConfig_ACClass_IsLoaded
    {
        get
        {
            return ACProgramConfig_ACClass != null;
        }
    }

    public virtual CollectionEntry ACProgramConfig_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgramConfig_ACClass); }
    }

    private ICollection<ACProgramConfig> _ACProgramConfig_ValueTypeACClass;
    public virtual ICollection<ACProgramConfig> ACProgramConfig_ValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _ACProgramConfig_ValueTypeACClass);
        set => _ACProgramConfig_ValueTypeACClass = value;
    }

    public bool ACProgramConfig_ValueTypeACClass_IsLoaded
    {
        get
        {
            return ACProgramConfig_ValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry ACProgramConfig_ValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgramConfig_ValueTypeACClass); }
    }

    private ICollection<ACProgram> _ACProgram_WorkflowTypeACClass;
    public virtual ICollection<ACProgram> ACProgram_WorkflowTypeACClass
    {
        get => LazyLoader.Load(this, ref _ACProgram_WorkflowTypeACClass);
        set => _ACProgram_WorkflowTypeACClass = value;
    }

    public bool ACProgram_WorkflowTypeACClass_IsLoaded
    {
        get
        {
            return ACProgram_WorkflowTypeACClass != null;
        }
    }

    public virtual CollectionEntry ACProgram_WorkflowTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProgram_WorkflowTypeACClass); }
    }

    private ACProject _ACProject;
    public virtual ACProject ACProject
    { 
        get => LazyLoader.Load(this, ref _ACProject);
        set => _ACProject = value;
    }

    public bool ACProject_IsLoaded
    {
        get
        {
            return ACProject != null;
        }
    }

    public virtual ReferenceEntry ACProjectReference 
    {
        get { return Context.Entry(this).Reference("ACProject"); }
    }
    
    private ICollection<ACProject> _ACProject_PAAppClassAssignmentACClass;
    public virtual ICollection<ACProject> ACProject_PAAppClassAssignmentACClass
    {
        get => LazyLoader.Load(this, ref _ACProject_PAAppClassAssignmentACClass);
        set => _ACProject_PAAppClassAssignmentACClass = value;
    }

    public bool ACProject_PAAppClassAssignmentACClass_IsLoaded
    {
        get
        {
            return ACProject_PAAppClassAssignmentACClass != null;
        }
    }

    public virtual CollectionEntry ACProject_PAAppClassAssignmentACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACProject_PAAppClassAssignmentACClass); }
    }

    private ICollection<ACPropertyLogRule> _ACPropertyLogRule_ACClass;
    public virtual ICollection<ACPropertyLogRule> ACPropertyLogRule_ACClass
    {
        get => LazyLoader.Load(this, ref _ACPropertyLogRule_ACClass);
        set => _ACPropertyLogRule_ACClass = value;
    }

    public bool ACPropertyLogRule_ACClass_IsLoaded
    {
        get
        {
            return ACPropertyLogRule_ACClass != null;
        }
    }

    public virtual CollectionEntry ACPropertyLogRule_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACPropertyLogRule_ACClass); }
    }

    private ICollection<ACPropertyLog> _ACPropertyLog_ACClass;
    public virtual ICollection<ACPropertyLog> ACPropertyLog_ACClass
    {
        get => LazyLoader.Load(this, ref _ACPropertyLog_ACClass);
        set => _ACPropertyLog_ACClass = value;
    }

    public bool ACPropertyLog_ACClass_IsLoaded
    {
        get
        {
            return ACPropertyLog_ACClass != null;
        }
    }

    public virtual CollectionEntry ACPropertyLog_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACPropertyLog_ACClass); }
    }

    private ACClass _ACClass1_BasedOnACClass;
    public virtual ACClass ACClass1_BasedOnACClass
    { 
        get => LazyLoader.Load(this, ref _ACClass1_BasedOnACClass);
        set => _ACClass1_BasedOnACClass = value;
    }

    public bool ACClass1_BasedOnACClass_IsLoaded
    {
        get
        {
            return ACClass1_BasedOnACClass != null;
        }
    }

    public virtual ReferenceEntry ACClass1_BasedOnACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass1_BasedOnACClass"); }
    }
    
    private ICollection<CompanyPersonRole> _CompanyPersonRole_VBiRoleACClass;
    public virtual ICollection<CompanyPersonRole> CompanyPersonRole_VBiRoleACClass
    {
        get => LazyLoader.Load(this, ref _CompanyPersonRole_VBiRoleACClass);
        set => _CompanyPersonRole_VBiRoleACClass = value;
    }

    public bool CompanyPersonRole_VBiRoleACClass_IsLoaded
    {
        get
        {
            return CompanyPersonRole_VBiRoleACClass != null;
        }
    }

    public virtual CollectionEntry CompanyPersonRole_VBiRoleACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyPersonRole_VBiRoleACClass); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_VBiStackCalculatorACClass;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_VBiStackCalculatorACClass
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_VBiStackCalculatorACClass);
        set => _FacilityBookingCharge_VBiStackCalculatorACClass = value;
    }

    public bool FacilityBookingCharge_VBiStackCalculatorACClass_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_VBiStackCalculatorACClass != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_VBiStackCalculatorACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_VBiStackCalculatorACClass); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_VBiStackCalculatorACClass;
    public virtual ICollection<FacilityBooking> FacilityBooking_VBiStackCalculatorACClass
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_VBiStackCalculatorACClass);
        set => _FacilityBooking_VBiStackCalculatorACClass = value;
    }

    public bool FacilityBooking_VBiStackCalculatorACClass_IsLoaded
    {
        get
        {
            return FacilityBooking_VBiStackCalculatorACClass != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_VBiStackCalculatorACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_VBiStackCalculatorACClass); }
    }

    private ICollection<FacilityReservation> _FacilityReservation_VBiACClass;
    public virtual ICollection<FacilityReservation> FacilityReservation_VBiACClass
    {
        get => LazyLoader.Load(this, ref _FacilityReservation_VBiACClass);
        set => _FacilityReservation_VBiACClass = value;
    }

    public bool FacilityReservation_VBiACClass_IsLoaded
    {
        get
        {
            return FacilityReservation_VBiACClass != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_VBiACClass); }
    }

    private ICollection<Facility> _Facility_VBiFacilityACClass;
    public virtual ICollection<Facility> Facility_VBiFacilityACClass
    {
        get => LazyLoader.Load(this, ref _Facility_VBiFacilityACClass);
        set => _Facility_VBiFacilityACClass = value;
    }

    public bool Facility_VBiFacilityACClass_IsLoaded
    {
        get
        {
            return Facility_VBiFacilityACClass != null;
        }
    }

    public virtual CollectionEntry Facility_VBiFacilityACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_VBiFacilityACClass); }
    }

    private ICollection<Facility> _Facility_VBiStackCalculatorACClass;
    public virtual ICollection<Facility> Facility_VBiStackCalculatorACClass
    {
        get => LazyLoader.Load(this, ref _Facility_VBiStackCalculatorACClass);
        set => _Facility_VBiStackCalculatorACClass = value;
    }

    public bool Facility_VBiStackCalculatorACClass_IsLoaded
    {
        get
        {
            return Facility_VBiStackCalculatorACClass != null;
        }
    }

    public virtual CollectionEntry Facility_VBiStackCalculatorACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_VBiStackCalculatorACClass); }
    }

    private ICollection<HistoryConfig> _HistoryConfig_VBiACClass;
    public virtual ICollection<HistoryConfig> HistoryConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _HistoryConfig_VBiACClass);
        set => _HistoryConfig_VBiACClass = value;
    }

    public bool HistoryConfig_VBiACClass_IsLoaded
    {
        get
        {
            return HistoryConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry HistoryConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.HistoryConfig_VBiACClass); }
    }

    private ICollection<HistoryConfig> _HistoryConfig_VBiValueTypeACClass;
    public virtual ICollection<HistoryConfig> HistoryConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _HistoryConfig_VBiValueTypeACClass);
        set => _HistoryConfig_VBiValueTypeACClass = value;
    }

    public bool HistoryConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return HistoryConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry HistoryConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.HistoryConfig_VBiValueTypeACClass); }
    }

    private ICollection<InOrderConfig> _InOrderConfig_VBiACClass;
    public virtual ICollection<InOrderConfig> InOrderConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _InOrderConfig_VBiACClass);
        set => _InOrderConfig_VBiACClass = value;
    }

    public bool InOrderConfig_VBiACClass_IsLoaded
    {
        get
        {
            return InOrderConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry InOrderConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderConfig_VBiACClass); }
    }

    private ICollection<InOrderConfig> _InOrderConfig_VBiValueTypeACClass;
    public virtual ICollection<InOrderConfig> InOrderConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _InOrderConfig_VBiValueTypeACClass);
        set => _InOrderConfig_VBiValueTypeACClass = value;
    }

    public bool InOrderConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return InOrderConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry InOrderConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderConfig_VBiValueTypeACClass); }
    }

    private ICollection<InRequestConfig> _InRequestConfig_VBiACClass;
    public virtual ICollection<InRequestConfig> InRequestConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _InRequestConfig_VBiACClass);
        set => _InRequestConfig_VBiACClass = value;
    }

    public bool InRequestConfig_VBiACClass_IsLoaded
    {
        get
        {
            return InRequestConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry InRequestConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestConfig_VBiACClass); }
    }

    private ICollection<InRequestConfig> _InRequestConfig_VBiValueTypeACClass;
    public virtual ICollection<InRequestConfig> InRequestConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _InRequestConfig_VBiValueTypeACClass);
        set => _InRequestConfig_VBiValueTypeACClass = value;
    }

    public bool InRequestConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return InRequestConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry InRequestConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestConfig_VBiValueTypeACClass); }
    }

    private ICollection<ACClass> _ACClass_BasedOnACClass;
    public virtual ICollection<ACClass> ACClass_BasedOnACClass
    {
        get => LazyLoader.Load(this, ref _ACClass_BasedOnACClass);
        set => _ACClass_BasedOnACClass = value;
    }

    public bool ACClass_BasedOnACClass_IsLoaded
    {
        get
        {
            return ACClass_BasedOnACClass != null;
        }
    }

    public virtual CollectionEntry ACClass_BasedOnACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClass_BasedOnACClass); }
    }

    private ICollection<ACClass> _ACClass_PWACClass;
    public virtual ICollection<ACClass> ACClass_PWACClass
    {
        get => LazyLoader.Load(this, ref _ACClass_PWACClass);
        set => _ACClass_PWACClass = value;
    }

    public bool ACClass_PWACClass_IsLoaded
    {
        get
        {
            return ACClass_PWACClass != null;
        }
    }

    public virtual CollectionEntry ACClass_PWACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClass_PWACClass); }
    }

    private ICollection<ACClass> _ACClass_PWMethodACClass;
    public virtual ICollection<ACClass> ACClass_PWMethodACClass
    {
        get => LazyLoader.Load(this, ref _ACClass_PWMethodACClass);
        set => _ACClass_PWMethodACClass = value;
    }

    public bool ACClass_PWMethodACClass_IsLoaded
    {
        get
        {
            return ACClass_PWMethodACClass != null;
        }
    }

    public virtual CollectionEntry ACClass_PWMethodACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClass_PWMethodACClass); }
    }

    private ICollection<ACClass> _ACClass_ParentACClass;
    public virtual ICollection<ACClass> ACClass_ParentACClass
    {
        get => LazyLoader.Load(this, ref _ACClass_ParentACClass);
        set => _ACClass_ParentACClass = value;
    }

    public bool ACClass_ParentACClass_IsLoaded
    {
        get
        {
            return ACClass_ParentACClass != null;
        }
    }

    public virtual CollectionEntry ACClass_ParentACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClass_ParentACClass); }
    }

    private ICollection<MaintACClass> _MaintACClass_VBiACClass;
    public virtual ICollection<MaintACClass> MaintACClass_VBiACClass
    {
        get => LazyLoader.Load(this, ref _MaintACClass_VBiACClass);
        set => _MaintACClass_VBiACClass = value;
    }

    public bool MaintACClass_VBiACClass_IsLoaded
    {
        get
        {
            return MaintACClass_VBiACClass != null;
        }
    }

    public virtual CollectionEntry MaintACClass_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintACClass_VBiACClass); }
    }

    private ICollection<MaintOrder> _MaintOrder_VBiPAACClass;
    public virtual ICollection<MaintOrder> MaintOrder_VBiPAACClass
    {
        get => LazyLoader.Load(this, ref _MaintOrder_VBiPAACClass);
        set => _MaintOrder_VBiPAACClass = value;
    }

    public bool MaintOrder_VBiPAACClass_IsLoaded
    {
        get
        {
            return MaintOrder_VBiPAACClass != null;
        }
    }

    public virtual CollectionEntry MaintOrder_VBiPAACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrder_VBiPAACClass); }
    }

    private ICollection<MaterialConfig> _MaterialConfig_VBiACClass;
    public virtual ICollection<MaterialConfig> MaterialConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _MaterialConfig_VBiACClass);
        set => _MaterialConfig_VBiACClass = value;
    }

    public bool MaterialConfig_VBiACClass_IsLoaded
    {
        get
        {
            return MaterialConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry MaterialConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialConfig_VBiACClass); }
    }

    private ICollection<MaterialConfig> _MaterialConfig_VBiValueTypeACClass;
    public virtual ICollection<MaterialConfig> MaterialConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _MaterialConfig_VBiValueTypeACClass);
        set => _MaterialConfig_VBiValueTypeACClass = value;
    }

    public bool MaterialConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return MaterialConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry MaterialConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialConfig_VBiValueTypeACClass); }
    }

    private ICollection<MaterialWFACClassMethodConfig> _MaterialWFACClassMethodConfig_VBiACClass;
    public virtual ICollection<MaterialWFACClassMethodConfig> MaterialWFACClassMethodConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _MaterialWFACClassMethodConfig_VBiACClass);
        set => _MaterialWFACClassMethodConfig_VBiACClass = value;
    }

    public bool MaterialWFACClassMethodConfig_VBiACClass_IsLoaded
    {
        get
        {
            return MaterialWFACClassMethodConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry MaterialWFACClassMethodConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFACClassMethodConfig_VBiACClass); }
    }

    private ICollection<MaterialWFACClassMethodConfig> _MaterialWFACClassMethodConfig_VBiValueTypeACClass;
    public virtual ICollection<MaterialWFACClassMethodConfig> MaterialWFACClassMethodConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _MaterialWFACClassMethodConfig_VBiValueTypeACClass);
        set => _MaterialWFACClassMethodConfig_VBiValueTypeACClass = value;
    }

    public bool MaterialWFACClassMethodConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return MaterialWFACClassMethodConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry MaterialWFACClassMethodConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFACClassMethodConfig_VBiValueTypeACClass); }
    }

    private ICollection<Material> _Material_VBiStackCalculatorACClass;
    public virtual ICollection<Material> Material_VBiStackCalculatorACClass
    {
        get => LazyLoader.Load(this, ref _Material_VBiStackCalculatorACClass);
        set => _Material_VBiStackCalculatorACClass = value;
    }

    public bool Material_VBiStackCalculatorACClass_IsLoaded
    {
        get
        {
            return Material_VBiStackCalculatorACClass != null;
        }
    }

    public virtual CollectionEntry Material_VBiStackCalculatorACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_VBiStackCalculatorACClass); }
    }

    private ICollection<MsgAlarmLog> _MsgAlarmLog_ACClass;
    public virtual ICollection<MsgAlarmLog> MsgAlarmLog_ACClass
    {
        get => LazyLoader.Load(this, ref _MsgAlarmLog_ACClass);
        set => _MsgAlarmLog_ACClass = value;
    }

    public bool MsgAlarmLog_ACClass_IsLoaded
    {
        get
        {
            return MsgAlarmLog_ACClass != null;
        }
    }

    public virtual CollectionEntry MsgAlarmLog_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.MsgAlarmLog_ACClass); }
    }

    private ICollection<OperationLog> _OperationLog_RefACClass;
    public virtual ICollection<OperationLog> OperationLog_RefACClass
    {
        get => LazyLoader.Load(this, ref _OperationLog_RefACClass);
        set => _OperationLog_RefACClass = value;
    }

    public bool OperationLog_RefACClass_IsLoaded
    {
        get
        {
            return OperationLog_RefACClass != null;
        }
    }

    public virtual CollectionEntry OperationLog_RefACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.OperationLog_RefACClass); }
    }

    private ICollection<OutOfferConfig> _OutOfferConfig_VBiACClass;
    public virtual ICollection<OutOfferConfig> OutOfferConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _OutOfferConfig_VBiACClass);
        set => _OutOfferConfig_VBiACClass = value;
    }

    public bool OutOfferConfig_VBiACClass_IsLoaded
    {
        get
        {
            return OutOfferConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry OutOfferConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferConfig_VBiACClass); }
    }

    private ICollection<OutOfferConfig> _OutOfferConfig_VBiValueTypeACClass;
    public virtual ICollection<OutOfferConfig> OutOfferConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _OutOfferConfig_VBiValueTypeACClass);
        set => _OutOfferConfig_VBiValueTypeACClass = value;
    }

    public bool OutOfferConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return OutOfferConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry OutOfferConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferConfig_VBiValueTypeACClass); }
    }

    private ICollection<OutOrderConfig> _OutOrderConfig_VBiACClass;
    public virtual ICollection<OutOrderConfig> OutOrderConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _OutOrderConfig_VBiACClass);
        set => _OutOrderConfig_VBiACClass = value;
    }

    public bool OutOrderConfig_VBiACClass_IsLoaded
    {
        get
        {
            return OutOrderConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry OutOrderConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderConfig_VBiACClass); }
    }

    private ICollection<OutOrderConfig> _OutOrderConfig_VBiValueTypeACClass;
    public virtual ICollection<OutOrderConfig> OutOrderConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _OutOrderConfig_VBiValueTypeACClass);
        set => _OutOrderConfig_VBiValueTypeACClass = value;
    }

    public bool OutOrderConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return OutOrderConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry OutOrderConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderConfig_VBiValueTypeACClass); }
    }

    private ACClass _ACClass1_PWACClass;
    public virtual ACClass ACClass1_PWACClass
    { 
        get => LazyLoader.Load(this, ref _ACClass1_PWACClass);
        set => _ACClass1_PWACClass = value;
    }

    public bool ACClass1_PWACClass_IsLoaded
    {
        get
        {
            return ACClass1_PWACClass != null;
        }
    }

    public virtual ReferenceEntry ACClass1_PWACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass1_PWACClass"); }
    }
    
    private ACClass _ACClass1_PWMethodACClass;
    public virtual ACClass ACClass1_PWMethodACClass
    { 
        get => LazyLoader.Load(this, ref _ACClass1_PWMethodACClass);
        set => _ACClass1_PWMethodACClass = value;
    }

    public bool ACClass1_PWMethodACClass_IsLoaded
    {
        get
        {
            return ACClass1_PWMethodACClass != null;
        }
    }

    public virtual ReferenceEntry ACClass1_PWMethodACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass1_PWMethodACClass"); }
    }
    
    private ACClass _ACClass1_ParentACClass;
    public virtual ACClass ACClass1_ParentACClass
    { 
        get => LazyLoader.Load(this, ref _ACClass1_ParentACClass);
        set => _ACClass1_ParentACClass = value;
    }

    public bool ACClass1_ParentACClass_IsLoaded
    {
        get
        {
            return ACClass1_ParentACClass != null;
        }
    }

    public virtual ReferenceEntry ACClass1_ParentACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass1_ParentACClass"); }
    }
    
    private ICollection<PartslistConfig> _PartslistConfig_VBiACClass;
    public virtual ICollection<PartslistConfig> PartslistConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _PartslistConfig_VBiACClass);
        set => _PartslistConfig_VBiACClass = value;
    }

    public bool PartslistConfig_VBiACClass_IsLoaded
    {
        get
        {
            return PartslistConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry PartslistConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistConfig_VBiACClass); }
    }

    private ICollection<PartslistConfig> _PartslistConfig_VBiValueTypeACClass;
    public virtual ICollection<PartslistConfig> PartslistConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _PartslistConfig_VBiValueTypeACClass);
        set => _PartslistConfig_VBiValueTypeACClass = value;
    }

    public bool PartslistConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return PartslistConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry PartslistConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistConfig_VBiValueTypeACClass); }
    }

    private ICollection<PickingConfig> _PickingConfig_VBiACClass;
    public virtual ICollection<PickingConfig> PickingConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _PickingConfig_VBiACClass);
        set => _PickingConfig_VBiACClass = value;
    }

    public bool PickingConfig_VBiACClass_IsLoaded
    {
        get
        {
            return PickingConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry PickingConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingConfig_VBiACClass); }
    }

    private ICollection<PickingConfig> _PickingConfig_VBiValueTypeACClass;
    public virtual ICollection<PickingConfig> PickingConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _PickingConfig_VBiValueTypeACClass);
        set => _PickingConfig_VBiValueTypeACClass = value;
    }

    public bool PickingConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return PickingConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry PickingConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingConfig_VBiValueTypeACClass); }
    }

    private ICollection<ProdOrderPartslistConfig> _ProdOrderPartslistConfig_VBiACClass;
    public virtual ICollection<ProdOrderPartslistConfig> ProdOrderPartslistConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistConfig_VBiACClass);
        set => _ProdOrderPartslistConfig_VBiACClass = value;
    }

    public bool ProdOrderPartslistConfig_VBiACClass_IsLoaded
    {
        get
        {
            return ProdOrderPartslistConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistConfig_VBiACClass); }
    }

    private ICollection<ProdOrderPartslistConfig> _ProdOrderPartslistConfig_VBiValueTypeACClass;
    public virtual ICollection<ProdOrderPartslistConfig> ProdOrderPartslistConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistConfig_VBiValueTypeACClass);
        set => _ProdOrderPartslistConfig_VBiValueTypeACClass = value;
    }

    public bool ProdOrderPartslistConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return ProdOrderPartslistConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistConfig_VBiValueTypeACClass); }
    }

    private ICollection<TourplanConfig> _TourplanConfig_VBiACClass;
    public virtual ICollection<TourplanConfig> TourplanConfig_VBiACClass
    {
        get => LazyLoader.Load(this, ref _TourplanConfig_VBiACClass);
        set => _TourplanConfig_VBiACClass = value;
    }

    public bool TourplanConfig_VBiACClass_IsLoaded
    {
        get
        {
            return TourplanConfig_VBiACClass != null;
        }
    }

    public virtual CollectionEntry TourplanConfig_VBiACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanConfig_VBiACClass); }
    }

    private ICollection<TourplanConfig> _TourplanConfig_VBiValueTypeACClass;
    public virtual ICollection<TourplanConfig> TourplanConfig_VBiValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _TourplanConfig_VBiValueTypeACClass);
        set => _TourplanConfig_VBiValueTypeACClass = value;
    }

    public bool TourplanConfig_VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return TourplanConfig_VBiValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry TourplanConfig_VBiValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanConfig_VBiValueTypeACClass); }
    }

    private ICollection<VBConfig> _VBConfig_ACClass;
    public virtual ICollection<VBConfig> VBConfig_ACClass
    {
        get => LazyLoader.Load(this, ref _VBConfig_ACClass);
        set => _VBConfig_ACClass = value;
    }

    public bool VBConfig_ACClass_IsLoaded
    {
        get
        {
            return VBConfig_ACClass != null;
        }
    }

    public virtual CollectionEntry VBConfig_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.VBConfig_ACClass); }
    }

    private ICollection<VBConfig> _VBConfig_ValueTypeACClass;
    public virtual ICollection<VBConfig> VBConfig_ValueTypeACClass
    {
        get => LazyLoader.Load(this, ref _VBConfig_ValueTypeACClass);
        set => _VBConfig_ValueTypeACClass = value;
    }

    public bool VBConfig_ValueTypeACClass_IsLoaded
    {
        get
        {
            return VBConfig_ValueTypeACClass != null;
        }
    }

    public virtual CollectionEntry VBConfig_ValueTypeACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.VBConfig_ValueTypeACClass); }
    }

    private ICollection<VBGroupRight> _VBGroupRight_ACClass;
    public virtual ICollection<VBGroupRight> VBGroupRight_ACClass
    {
        get => LazyLoader.Load(this, ref _VBGroupRight_ACClass);
        set => _VBGroupRight_ACClass = value;
    }

    public bool VBGroupRight_ACClass_IsLoaded
    {
        get
        {
            return VBGroupRight_ACClass != null;
        }
    }

    public virtual CollectionEntry VBGroupRight_ACClassReference
    {
        get { return Context.Entry(this).Collection(c => c.VBGroupRight_ACClass); }
    }
}
