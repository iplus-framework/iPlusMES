using System;
using System.Collections.Generic;
using gip.core.datamodel;
using gip.mes.datamodel;


namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'ACClassTaskWrapperVB'}de{'ACClassTaskWrapperVB'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable)]
    public class ACClassTaskWrapperVB
    {
        public ACClassTaskWrapperVB()
        {
        }

        public ACClassTaskWrapperVB(gip.mes.datamodel.ACClassTask acClassTask)
        {
            ACClassTask = acClassTask;
        }

        private gip.mes.datamodel.ACClassTask _ACClassTask;
        [ACPropertyInfo(1, "ACClassTask", "en{'ACClassTask'}de{'ACClassTask'}")]
        public gip.mes.datamodel.ACClassTask ACClassTask 
        {
            get
            {
                return _ACClassTask;
            }
            set
            {
                _ACClassTask = value;
                //if (_ACClassTask.ACProgram != null)
                //    _ProdOrderPartslist = _ACClassTask.ACProgram.ACProgramLog_ACProgram
                //                                    .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPosID.HasValue && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos != null)
                //                                    .Select(f => f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist)
                //                                    .FirstOrDefault();
                //_ProdOrderPartslistPos = _ACClassTask.ProdOrderPartslistPos_ACClassTask.FirstOrDefault();
                //if (_ProdOrderPartslistPos != null)
                //    _ProdOrderBatch = ProdOrderPartslistPos.ProdOrderBatch;
                //else
                //{
                //    if (_ACClassTask.ACProgram != null)
                //    {
                //        _PickingPos = _ACClassTask.ACProgram.ACProgramLog_ACProgram
                //                                        .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.PickingPosID != null)
                //                                        .Select(f => f.OrderLog_VBiACProgramLog.PickingPos)
                //                                        .FirstOrDefault();
                //    }
                //}
            }
        }

        private ProdOrderPartslist _ProdOrderPartslist;
        [ACPropertyInfo(2, "Entity", "en{'ProdOrderPartslist'}de{'ProdOrderPartslist'}")]
        public ProdOrderPartslist ProdOrderPartslist
        {
            get
            {
                //if (_ProdOrderPartslist == null && _ACClassTask.ACProgram != null)
                //    _ProdOrderPartslist = _ACClassTask.ACProgram.ACProgramLog_ACProgram
                //                                    .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.ProdOrderPartslistPosID.HasValue)
                //                                    .Select(f => f.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist)
                //                                    .FirstOrDefault();
                return _ProdOrderPartslist;
            }
            set
            {
                _ProdOrderPartslist = value;
            }
        }

        private ProdOrderBatch _ProdOrderBatch;
        [ACPropertyInfo(3, "Entity", "en{'ProdOrderBatch'}de{'ProdOrderBatch'}")]
        public ProdOrderBatch ProdOrderBatch
        {
            get
            {
                if (_ProdOrderBatch == null && ProdOrderPartslistPos != null)
                    _ProdOrderBatch = ProdOrderPartslistPos.ProdOrderBatch;
                return _ProdOrderBatch;
            }
            set
            {
                _ProdOrderBatch = value;
            }
        }

        private ProdOrderPartslistPos _ProdOrderPartslistPos;
        [ACPropertyInfo(4, "Entity", "en{'ProdOrderPartslistPos'}de{'ProdOrderPartslistPos'}")]
        public ProdOrderPartslistPos ProdOrderPartslistPos
        {
            get
            {
                //if (_ProdOrderPartslistPos == null)
                //    _ProdOrderPartslistPos = _ACClassTask.ProdOrderPartslistPos_ACClassTask.FirstOrDefault();
                return _ProdOrderPartslistPos;
            }
            set
            {
                _ProdOrderPartslistPos = value;
            }
        }

        private PickingPos _PickingPos;
        [ACPropertyInfo(5, "Entity", "en{'PickingPos'}de{'PickingPos'}")]
        public PickingPos PickingPos
        {
            get
            {
                //if (_PickingPos == null && _ACClassTask.ACProgram != null)
                //    _PickingPos = _ACClassTask.ACProgram.ACProgramLog_ACProgram
                //                                    .Where(f => f.OrderLog_VBiACProgramLog != null && f.OrderLog_VBiACProgramLog.PickingPosID != null)
                //                                    .Select(f => f.OrderLog_VBiACProgramLog.PickingPos)
                //                                    .FirstOrDefault();
                return _PickingPos;
            }
            set
            {
                _PickingPos = value;
            }
        }

        [ACPropertyInfo(6, "Entity", "en{'Picking'}de{'Picking'}")]
        public Picking Picking
        {
            get
            {
                if (PickingPos != null)
                    return PickingPos.Picking;
                return null;
            }
        }

        [ACPropertyInfo(7, "ShowCol", "en{'Order-No.'}de{'Auftrag-Nr.'}")]
        public string OrderInfo1
        {
            get
            {
                if (ProdOrderPartslist != null && ProdOrderPartslist.ProdOrder != null)
                    return ProdOrderPartslist.ProdOrder.ProgramNo;
                if (Picking != null)
                    return Picking.PickingNo;
                return "";
            }
        }

        [ACPropertyInfo(8, "ShowCol", "en{'Batch-No.'}de{'Batch-Nr.'}")]
        public string OrderInfo2
        {
            get
            {
                if (ProdOrderBatch != null)
                    return ProdOrderBatch.BatchSeqNo.ToString();
                if (PickingPos != null && PickingPos.FromFacility != null)
                    return PickingPos.FromFacility.FacilityName;
                return "";
            }
        }

        [ACPropertyInfo(9, "ShowCol", "en{'Material'}de{'Material'}")]
        public string OrderInfo3
        {
            get
            {
                if (ProdOrderPartslist != null && ProdOrderPartslist.Partslist != null && ProdOrderPartslist.Partslist.Material != null)
                    return ProdOrderPartslist.Partslist.Material.MaterialName1;
                if (PickingPos != null && PickingPos.Material != null)
                    return PickingPos.Material.MaterialName1;
                return "";
            }
        }

        [ACPropertyInfo(10, "ShowCol", "en{'Startet'}de{'Gestartet'}")]
        public DateTime InsertDate
        {
            get
            {
                return ACClassTask != null ? ACClassTask.InsertDate : DateTime.MinValue;
            }
        }


        [ACPropertyInfo(11, "ShowCol", "en{'Id'}de{'Id'}")]
        public string ACIdentifier
        {
            get
            {
                return ACClassTask != null ? ACClassTask.ACIdentifier : "";
            }
        }

        [ACPropertyInfo(12, "ShowCol", "en{'Program-No'}de{'Programm-Nr.'}")]
        public string ProgramNo
        {
            get
            {
                return ACClassTask != null && ACClassTask.ACProgram != null ? ACClassTask.ACProgram.ProgramNo : "";
            }
        }


        [ACPropertyInfo(13, "ShowCol", "en{'Programname'}de{'Programmname'}")]
        public string ProgramName
        {
            get
            {
                return ACClassTask != null && ACClassTask.ACProgram != null ? ACClassTask.ACProgram.ProgramName : "";
            }
        }


        [ACPropertyInfo(14, "ShowCol", "en{'Planned Starttime'}de{'Gepl.Startzeit'}")]
        public DateTime PlannedStartDate
        {
            get
            {
                return ACClassTask != null && ACClassTask.ACProgram != null ? ACClassTask.ACProgram.PlannedStartDate : DateTime.MinValue;
            }
        }



    }


    public class ACClassTaskWrapperVBComparer : IEqualityComparer<ACClassTaskWrapperVB>
    {
        public bool Equals(ACClassTaskWrapperVB val1, ACClassTaskWrapperVB val2)
        {
            if (val1.ACClassTask.ACClassTaskID == val2.ACClassTask.ACClassTaskID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(ACClassTaskWrapperVB val)
        {
            return val.ACClassTask.ACClassTaskID.GetHashCode();
        }
    }

}
