using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using gip.core.manager;
using gip.mes.datamodel; using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.sales
{
    /// <summary>
    /// Verladung von Loserware und verpackter Ware
    /// 
    /// Die eigentliche Kommissionierung der vorbereiteten Lieferscheine findet hier statt
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Shipment'}de{'Verladung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + DeliveryNote.ClassName)]
    public class BSOOutLoading : ACBSOvbNav 
    {
        #region c´tors

        public BSOOutLoading(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }
        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        ACAccessNav<DeliveryNote> _AccessPrimary;
        [ACPropertyAccessPrimary(690, "OutDeliveryNote")]
        public ACAccessNav<DeliveryNote> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<DeliveryNote>("OutDeliveryNote", this);
                }
                return _AccessPrimary;
            }
        }

        //[ACPropertyCurrent(9999, "OutDeliveryNote")]
        //public OutDeliveryNote CurrentOutDeliveryNote
        //{
        //    get
        //    {
        //        if (AccessPrimary == null) return null; return AccessPrimary.CurrentNavObject as OutDeliveryNote;
        //    }
        //    set
        //    {
        //        AccessPrimary.CurrentNavObject = value;
        //        OnPropertyChanged("CurrentOutDeliveryNote");
        //        OnPropertyChanged("OutDeliveryNotePosList");
        //        OnPropertyChanged("OutDeliveryNotePosLoadlistList");
        //    }
        //}

        //[ACPropertyList(9999, "OutDeliveryNote")]
        //public IEnumerable<OutDeliveryNote> OutDeliveryNoteList
        //{
        //    get
        //    {
        //        return AccessPrimary.NavObjectList as IEnumerable<OutDeliveryNote>;
        //    }
        //}

        //[ACPropertySelected(9999, "OutDeliveryNote")]
        //public OutDeliveryNote SelectedOutDeliveryNote
        //{
        //    get
        //    {
        //        if (AccessPrimary == null) return null; return AccessPrimary.SelectedNavObject as OutDeliveryNote;
        //    }
        //    set
        //    {
        //        AccessPrimary.SelectedNavObject = value;
        //        OnPropertyChanged("SelectedOutDeliveryNote");
        //    }
        //}

        //OutDeliveryNotePos _CurrentOutDeliveryNotePos;
        //[ACPropertyCurrent(9999, "OutDeliveryNotePos")]
        //public OutDeliveryNotePos CurrentOutDeliveryNotePos
        //{
        //    get
        //    {
        //        return _CurrentOutDeliveryNotePos;
        //    }
        //    set
        //    {
        //        _CurrentOutDeliveryNotePos = value;
        //        OnPropertyChanged("CurrentOutDeliveryNotePos");
        //    }
        //}

        //[ACPropertyList(9999, "OutDeliveryNotePos")]
        //public IEnumerable<OutDeliveryNotePos> OutDeliveryNotePosList
        //{
        //    get
        //    {
        //        return from c in CurrentOutDeliveryNote.OutDeliveryNotePos_OutDeliveryNote select c;

        //    }
        //}

        //OutDeliveryNotePos _SelectedOutDeliveryNotePos;
        //[ACPropertySelected(9999, "OutDeliveryNotePos")]
        //public OutDeliveryNotePos SelectedOutDeliveryNotePos
        //{
        //    get
        //    {
        //        return _SelectedOutDeliveryNotePos;
        //    }
        //    set
        //    {
        //        _SelectedOutDeliveryNotePos = value;
        //        OnPropertyChanged("SelectedOutDeliveryNotePos");
        //    }
        //}

        //OutDeliveryNotePosLoadlist _CurrentOutDeliveryNotePosLoadlist;
        //[ACPropertyCurrent(9999, "OutDeliveryNotePosLoadlist")]
        //public OutDeliveryNotePosLoadlist CurrentOutDeliveryNotePosLoadlist
        //{
        //    get
        //    {
        //        return _CurrentOutDeliveryNotePosLoadlist;
        //    }
        //    set
        //    {
        //        _CurrentOutDeliveryNotePosLoadlist = value;
        //        OnPropertyChanged("CurrentOutDeliveryNotePosLoadlist");
        //    }
        //}

        //[ACPropertyList(9999, "OutDeliveryNotePosLoadlist")]
        //public IEnumerable<OutDeliveryNotePosLoadlist> OutDeliveryNotePosLoadlistList
        //{
        //    get
        //    {
        //        return from c in CurrentOutDeliveryNotePos.OutDeliveryNotePosLoadlist_OutDeliveryNotePos
        //               select c;

        //    }
        //}

        //OutDeliveryNotePosLoadlist _SelectedOutDeliveryNotePosLoadlist;
        //[ACPropertySelected(9999, "OutDeliveryNotePosLoadlist")]
        //public OutDeliveryNotePosLoadlist SelectedOutDeliveryNotePosLoadlist
        //{
        //    get
        //    {
        //        return _SelectedOutDeliveryNotePosLoadlist;
        //    }
        //    set
        //    {
        //        _SelectedOutDeliveryNotePosLoadlist = value;
        //        OnPropertyChanged("SelectedOutDeliveryNotePosLoadlist");
        //    }
        //}

        /// <summary>
        #endregion

        #region BSO->ACMethod
        [ACMethodCommand("OutDeliveryNote", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand("OutDeliveryNote", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction("OutDeliveryNote", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutDeliveryNote", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            //if (SelectedOutDeliveryNote != null && ACSaveOrUndoChanges())
            //{
            //    if (!PreExecute("Load")) return;
            //    CurrentOutDeliveryNote = (from c in Database.OutDeliveryNote
            //        .Include("OutDeliveryNotePos_OutDeliveryNote")
            //        .Include(OutOrder.ClassName)
            //        .Include("OutOrder.DeliveryCompanyAddress")
            //                              where c.OutDeliveryNoteID == SelectedOutDeliveryNote.OutDeliveryNoteID
            //                              select c).First();
            //    ACState = Const.SMEdit;
            //    PostExecute("Load");
               
            //}
        }

        public bool IsEnabledLoad()
        {
            return false; // SelectedOutDeliveryNote != null;
        }

        [ACMethodInteraction("OutDeliveryNote", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedOutDeliveryNote", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            //CurrentOutDeliveryNote = OutDeliveryNote.NewACObject(Database, null);
            //Database.OutDeliveryNote.AddObject(CurrentOutDeliveryNote);

            //CurrentOutDeliveryNotePos = OutDeliveryNotePos.NewACObject(Database, CurrentOutDeliveryNote);
            //CurrentOutDeliveryNotePos.OutDeliveryNote = CurrentOutDeliveryNote;
            //CurrentOutDeliveryNote.OutDeliveryNotePos_OutDeliveryNote.Add(CurrentOutDeliveryNotePos);
            ACState = Const.SMNew;
            PostExecute("New");
           
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction("OutDeliveryNote", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentOutDeliveryNote", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            //Msg msg = CurrentOutDeliveryNote.DeleteACObject(Database, true);
            //if (msg != null)
            //{
            //    Messages.Msg(msg);
            //    return;
            //}

            PostExecute("Delete");
            New();
        }

        public bool IsEnabledDelete()
        {
            return false; // CurrentOutDeliveryNote != null;
        }

        [ACMethodCommand("OutDeliveryNote", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutDeliveryNoteList");
        }

        [ACMethodInteraction("OutDeliveryNotePos", "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, "SelectedOutDeliveryNotePos", Global.ACKinds.MSMethodPrePost)]
        public void LoadOutDeliveryNotePos()
        {
            if (!IsEnabledLoadOutDeliveryNotePos())
                return;
            if (!PreExecute("LoadOutDeliveryNotePos")) return;
            // Laden des aktuell selektierten OutDeliveryNotePos 
            //CurrentOutDeliveryNotePos = (from c in CurrentOutDeliveryNote.OutDeliveryNotePos_OutDeliveryNote
            //                             where c.OutDeliveryNotePosID == SelectedOutDeliveryNotePos.OutDeliveryNotePosID
            //                             select c).First();
            PostExecute("LoadOutDeliveryNotePos");
        }

        public bool IsEnabledLoadOutDeliveryNotePos()
        {
            return false; // SelectedOutDeliveryNotePos != null && CurrentOutDeliveryNote != null;
        }

        [ACMethodInteraction("OutDeliveryNotePos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedOutDeliveryNotePos", Global.ACKinds.MSMethodPrePost)]
        public void NewOutDeliveryNotePos()
        {
            if (!PreExecute("NewOutDeliveryNotePos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            //CurrentOutDeliveryNotePos = OutDeliveryNotePos.NewACObject(Database, CurrentOutDeliveryNote);
            //CurrentOutDeliveryNotePos.OutDeliveryNote = CurrentOutDeliveryNote;
            //CurrentOutDeliveryNote.OutDeliveryNotePos_OutDeliveryNote.Add(CurrentOutDeliveryNotePos);
            PostExecute("NewOutDeliveryNotePos");
        }

        public bool IsEnabledNewOutDeliveryNotePos()
        {
            return false; // CurrentOutDeliveryNote != null;
        }

        [ACMethodInteraction("OutDeliveryNotePos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentOutDeliveryNotePos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteOutDeliveryNotePos()
        {
            if (!PreExecute("DeleteOutDeliveryNotePos")) return;
            //Msg msg = CurrentOutDeliveryNotePos.DeleteACObject(Database, true);
            //if (msg != null)
            //{
            //    Messages.Msg(msg);
            //    return;
            //}

            PostExecute("DeleteOutDeliveryNotePos");
        }

        public bool IsEnabledDeleteOutDeliveryNotePos()
        {
            return false; // CurrentOutDeliveryNote != null && CurrentOutDeliveryNotePos != null;
        }

        [ACMethodInteraction("OutDeliveryNotePosLoadlist", "en{'Load Declaration'}de{'Deklaration laden'}", (short)MISort.Load, false, "SelectedOutDeliveryNotePosLoadlist", Global.ACKinds.MSMethodPrePost)]
        public void LoadOutDeliveryNotePosLoadlist()
        {
            if (!IsEnabledLoadOutDeliveryNotePosLoadlist())
                return;
            if (!PreExecute("LoadOutDeliveryNotePosLoadlist")) return;
            //CurrentOutDeliveryNotePosLoadlist = (from c in CurrentOutDeliveryNotePos.OutDeliveryNotePosLoadlist_OutDeliveryNotePos
            //                                     where c.OutDeliveryNotePosLoadlistID == SelectedOutDeliveryNotePosLoadlist.OutDeliveryNotePosLoadlistID
            //                                     select c).First();
            PostExecute("LoadOutDeliveryNotePosLoadlist");
        }

        public bool IsEnabledLoadOutDeliveryNotePosLoadlist()
        {
            return false; // SelectedOutDeliveryNotePosLoadlist != null && CurrentOutDeliveryNotePos != null;
        }

        [ACMethodInteraction("OutDeliveryNotePosLoadlist", "en{'New Declaration'}de{'Neue Deklaration'}", (short)MISort.New, true, "SelectedOutDeliveryNotePosLoadlist", Global.ACKinds.MSMethodPrePost)]
        public void NewOutDeliveryNotePosLoadlist()
        {
            if (!PreExecute("NewOutDeliveryNotePosLoadlist")) return;
            //CurrentOutDeliveryNotePosLoadlist = OutDeliveryNotePosLoadlist.NewACObject(Database, CurrentOutDeliveryNotePos);
            //CurrentOutDeliveryNotePosLoadlist.OutDeliveryNotePos = CurrentOutDeliveryNotePos;
            //CurrentOutDeliveryNotePos.OutDeliveryNotePosLoadlist_OutDeliveryNotePos.Add(CurrentOutDeliveryNotePosLoadlist);
            PostExecute("NewOutDeliveryNotePosLoadlist");
        }

        public bool IsEnabledNewOutDeliveryNotePosLoadlist()
        {
            return true;
        }

        [ACMethodInteraction("OutDeliveryNotePosLoadlist", "en{'Delete Declaration'}de{'Deklaration löschen'}", (short)MISort.Delete, true, "CurrentOutDeliveryNotePosLoadlist", Global.ACKinds.MSMethodPrePost)]
        public void DeleteOutDeliveryNotePosLoadlist()
        {
            if (!PreExecute("DeleteOutDeliveryNotePosLoadlist")) return;
            //Msg msg = CurrentOutDeliveryNotePosLoadlist.DeleteACObject(Database, true);
            //if (msg != null)
            //{
            //    Messages.Msg(msg);
            //    return;
            //}

            PostExecute("DeleteOutDeliveryNotePosLoadlist");
        }

        public bool IsEnabledDeleteOutDeliveryNotePosLoadlist()
        {
            return false; // CurrentOutDeliveryNote != null && CurrentOutDeliveryNotePosLoadlist != null;
        }
        #endregion


        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"Save":
                    Save();
                    return true;
                case"IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case"UndoSave":
                    UndoSave();
                    return true;
                case"IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case"Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case"IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case"New":
                    New();
                    return true;
                case"IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case"Delete":
                    Delete();
                    return true;
                case"IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case"Search":
                    Search();
                    return true;
                case"LoadOutDeliveryNotePos":
                    LoadOutDeliveryNotePos();
                    return true;
                case"IsEnabledLoadOutDeliveryNotePos":
                    result = IsEnabledLoadOutDeliveryNotePos();
                    return true;
                case"NewOutDeliveryNotePos":
                    NewOutDeliveryNotePos();
                    return true;
                case"IsEnabledNewOutDeliveryNotePos":
                    result = IsEnabledNewOutDeliveryNotePos();
                    return true;
                case"DeleteOutDeliveryNotePos":
                    DeleteOutDeliveryNotePos();
                    return true;
                case"IsEnabledDeleteOutDeliveryNotePos":
                    result = IsEnabledDeleteOutDeliveryNotePos();
                    return true;
                case"LoadOutDeliveryNotePosLoadlist":
                    LoadOutDeliveryNotePosLoadlist();
                    return true;
                case"IsEnabledLoadOutDeliveryNotePosLoadlist":
                    result = IsEnabledLoadOutDeliveryNotePosLoadlist();
                    return true;
                case"NewOutDeliveryNotePosLoadlist":
                    NewOutDeliveryNotePosLoadlist();
                    return true;
                case"IsEnabledNewOutDeliveryNotePosLoadlist":
                    result = IsEnabledNewOutDeliveryNotePosLoadlist();
                    return true;
                case"DeleteOutDeliveryNotePosLoadlist":
                    DeleteOutDeliveryNotePosLoadlist();
                    return true;
                case"IsEnabledDeleteOutDeliveryNotePosLoadlist":
                    result = IsEnabledDeleteOutDeliveryNotePosLoadlist();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
