using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDDelivNoteState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDDelivNoteState()
    {
    }

    private MDDelivNoteState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDDelivNoteStateID;
    public Guid MDDelivNoteStateID 
    {
        get { return _MDDelivNoteStateID; }
        set { SetProperty<Guid>(ref _MDDelivNoteStateID, value); }
    }

    short _MDDelivNoteStateIndex;
    public short MDDelivNoteStateIndex 
    {
        get { return _MDDelivNoteStateIndex; }
        set { SetProperty<short>(ref _MDDelivNoteStateIndex, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    private ICollection<DeliveryNote> _DeliveryNote_MDDelivNoteState;
    public virtual ICollection<DeliveryNote> DeliveryNote_MDDelivNoteState
    {
        get { return LazyLoader.Load(this, ref _DeliveryNote_MDDelivNoteState); }
        set { _DeliveryNote_MDDelivNoteState = value; }
    }

    public bool DeliveryNote_MDDelivNoteState_IsLoaded
    {
        get
        {
            return DeliveryNote_MDDelivNoteState != null;
        }
    }

    public virtual CollectionEntry DeliveryNote_MDDelivNoteStateReference
    {
        get { return Context.Entry(this).Collection(c => c.DeliveryNote_MDDelivNoteState); }
    }
}
