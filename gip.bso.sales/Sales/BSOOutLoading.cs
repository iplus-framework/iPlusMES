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
        }

        public bool IsEnabledLoad()
        {
            return false; // SelectedOutDeliveryNote != null;
        }

        [ACMethodInteraction("OutDeliveryNote", Const.New, (short)MISort.New, true, "SelectedOutDeliveryNote", Global.ACKinds.MSMethodPrePost)]
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

        [ACMethodInteraction("OutDeliveryNote", Const.Delete, (short)MISort.Delete, true, "CurrentOutDeliveryNote", Global.ACKinds.MSMethodPrePost)]
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
                case nameof(Save):
                    Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(LoadOutDeliveryNotePos):
                    LoadOutDeliveryNotePos();
                    return true;
                case nameof(IsEnabledLoadOutDeliveryNotePos):
                    result = IsEnabledLoadOutDeliveryNotePos();
                    return true;
                case nameof(NewOutDeliveryNotePos):
                    NewOutDeliveryNotePos();
                    return true;
                case nameof(IsEnabledNewOutDeliveryNotePos):
                    result = IsEnabledNewOutDeliveryNotePos();
                    return true;
                case nameof(DeleteOutDeliveryNotePos):
                    DeleteOutDeliveryNotePos();
                    return true;
                case nameof(IsEnabledDeleteOutDeliveryNotePos):
                    result = IsEnabledDeleteOutDeliveryNotePos();
                    return true;
                case nameof(LoadOutDeliveryNotePosLoadlist):
                    LoadOutDeliveryNotePosLoadlist();
                    return true;
                case nameof(IsEnabledLoadOutDeliveryNotePosLoadlist):
                    result = IsEnabledLoadOutDeliveryNotePosLoadlist();
                    return true;
                case nameof(NewOutDeliveryNotePosLoadlist):
                    NewOutDeliveryNotePosLoadlist();
                    return true;
                case nameof(IsEnabledNewOutDeliveryNotePosLoadlist):
                    result = IsEnabledNewOutDeliveryNotePosLoadlist();
                    return true;
                case nameof(DeleteOutDeliveryNotePosLoadlist):
                    DeleteOutDeliveryNotePosLoadlist();
                    return true;
                case nameof(IsEnabledDeleteOutDeliveryNotePosLoadlist):
                    result = IsEnabledDeleteOutDeliveryNotePosLoadlist();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                #region Always Enabled
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                case nameof(IsEnabledNew):
                    return new string[] { nameof(InitState) };
                #endregion

                #region Load
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new string[] { "SelectedOutDeliveryNote" };
                #endregion

                #region Delete
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(ACState) };
                #endregion

                #region OutDeliveryNotePos operations
                case nameof(LoadOutDeliveryNotePos):
                case nameof(IsEnabledLoadOutDeliveryNotePos):
                case nameof(NewOutDeliveryNotePos):
                case nameof(IsEnabledNewOutDeliveryNotePos):
                case nameof(DeleteOutDeliveryNotePos):
                case nameof(IsEnabledDeleteOutDeliveryNotePos):
                    return new string[] { "SelectedOutDeliveryNote", "SelectedOutDeliveryNotePos" };
                #endregion

                #region OutDeliveryNotePosLoadlist operations
                case nameof(LoadOutDeliveryNotePosLoadlist):
                case nameof(IsEnabledLoadOutDeliveryNotePosLoadlist):
                case nameof(NewOutDeliveryNotePosLoadlist):
                case nameof(IsEnabledNewOutDeliveryNotePosLoadlist):
                case nameof(DeleteOutDeliveryNotePosLoadlist):
                case nameof(IsEnabledDeleteOutDeliveryNotePosLoadlist):
                    return new string[] { "SelectedOutDeliveryNote", "SelectedOutDeliveryNotePosLoadlist" };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion


    }
}
