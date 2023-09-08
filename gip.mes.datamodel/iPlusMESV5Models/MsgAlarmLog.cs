using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MsgAlarmLog : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MsgAlarmLog()
    {
    }

    private MsgAlarmLog(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MsgAlarmLogID;
    public Guid MsgAlarmLogID 
    {
        get { return _MsgAlarmLogID; }
        set { SetProperty<Guid>(ref _MsgAlarmLogID, value); }
    }

    short _MessageLevelIndex;
    public short MessageLevelIndex 
    {
        get { return _MessageLevelIndex; }
        set { SetProperty<short>(ref _MessageLevelIndex, value); }
    }

    string _ACIdentifier;
    public override string ACIdentifier 
    {
        get { return _ACIdentifier; }
        set { SetProperty<string>(ref _ACIdentifier, value); }
    }

    int _Column;
    public int Column 
    {
        get { return _Column; }
        set { SetProperty<int>(ref _Column, value); }
    }

    int _Row;
    public int Row 
    {
        get { return _Row; }
        set { SetProperty<int>(ref _Row, value); }
    }

    string _Message;
    public string Message 
    {
        get { return _Message; }
        set { SetProperty<string>(ref _Message, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    DateTime _TimeStampOccurred;
    public DateTime TimeStampOccurred 
    {
        get { return _TimeStampOccurred; }
        set { SetProperty<DateTime>(ref _TimeStampOccurred, value); }
    }

    DateTime _TimeStampAcknowledged;
    public DateTime TimeStampAcknowledged 
    {
        get { return _TimeStampAcknowledged; }
        set { SetProperty<DateTime>(ref _TimeStampAcknowledged, value); }
    }

    string _AcknowledgedBy;
    public string AcknowledgedBy 
    {
        get { return _AcknowledgedBy; }
        set { SetProperty<string>(ref _AcknowledgedBy, value); }
    }

    Guid? _ACProgramLogID;
    public Guid? ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid?>(ref _ACProgramLogID, value); }
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

    string _TranslID;
    public string TranslID 
    {
        get { return _TranslID; }
        set { SetProperty<string>(ref _TranslID, value); }
    }

    Guid? _ACClassID;
    public Guid? ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid?>(ref _ACClassID, value); }
    }

    private ACClass _ACClass;
    public virtual ACClass ACClass
    { 
        get { return LazyLoader.Load(this, ref _ACClass); } 
        set { SetProperty<ACClass>(ref _ACClass, value); }
    }

    public bool ACClass_IsLoaded
    {
        get
        {
            return ACClass != null;
        }
    }

    public virtual ReferenceEntry ACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass"); }
    }
    
    private ACProgramLog _ACProgramLog;
    public virtual ACProgramLog ACProgramLog
    { 
        get { return LazyLoader.Load(this, ref _ACProgramLog); } 
        set { SetProperty<ACProgramLog>(ref _ACProgramLog, value); }
    }

    public bool ACProgramLog_IsLoaded
    {
        get
        {
            return ACProgramLog != null;
        }
    }

    public virtual ReferenceEntry ACProgramLogReference 
    {
        get { return Context.Entry(this).Reference("ACProgramLog"); }
    }
    }
