using gip.core.datamodel;
using VD = gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.processapplication
{
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Schedule for WF-Batch-Manager'}de{'Zeitplan für WF-Batch-Manager'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class PAScheduleForPWNodeList : SafeList<PAScheduleForPWNode>, ICloneable
    {
        #region Constructors
        public PAScheduleForPWNodeList()
            : base()
        {
        }

        public PAScheduleForPWNodeList(IEnumerable<PAScheduleForPWNode> Items)
            : base(Items)
        {
        }

        public PAScheduleForPWNodeList(int Capacity) : base(Capacity)
        {
        }
        #endregion

        [Flags]
        public enum DiffResult
        {
            Equal = 0x0000,

            /// <summary>
            /// One of the Items in the List have different Values
            /// </summary>
            ValueChangesInList = 0x0001,

            /// <summary>
            /// The StartMode-Property is different to the passed "selectedNode"
            /// => StartMode-Combobox must be refreshed
            /// </summary>
            StartModeChanged = 0x0002,

            /// <summary>
            /// The RefreshCounter-Property is different to the passed "selectedNode"
            /// => BatchPlans must be relaoded in view
            /// </summary>
            RefreshCounterChanged = 0x0004,

            /// <summary>
            /// New Entries in list because new Planning-Workflows were added
            /// => Reload ACClassWF from Database
            /// </summary>
            NewPWNodesDetected = 0x0008,

            /// <summary>
            /// Some elements were removed in new list. This should not happen because a workflow must be deleted in master data.
            /// </summary>
            PWNodesRemoved = 0x0010,
        }

        public DiffResult CompareAndUpdateFrom(PAScheduleForPWNodeList changedScheduleNodeList, PAScheduleForPWNode selectedNode)
        {
            if (changedScheduleNodeList == null)
                throw new ArgumentException("changedScheduleNode is null");

            DiffResult diffResult = DiffResult.Equal;
            foreach (PAScheduleForPWNode changedNode in changedScheduleNodeList)
            {
                PAScheduleForPWNode nodeInThisList = this.Where(c => c.MDSchedulingGroupID == changedNode.MDSchedulingGroupID).FirstOrDefault();
                if (nodeInThisList == null)
                {
                    diffResult |= DiffResult.NewPWNodesDetected;
                    this.Add(changedNode);
                }
                else
                {
                    if (nodeInThisList.StartMode != changedNode.StartMode)
                    {
                        if (nodeInThisList == selectedNode)
                            diffResult |= DiffResult.StartModeChanged;
                    }
                    if (nodeInThisList.RefreshCounter != changedNode.RefreshCounter)
                    {
                        diffResult |= DiffResult.ValueChangesInList;
                        if (nodeInThisList == selectedNode)
                            diffResult |= DiffResult.RefreshCounterChanged;
                    }
                    nodeInThisList.CopyFrom(changedNode, true);
                }
            }
            foreach (PAScheduleForPWNode nodeInThisList in this.ToArray())
            {
                if (!changedScheduleNodeList.Where(c => c.MDSchedulingGroupID == nodeInThisList.MDSchedulingGroupID).Any())
                {
                    diffResult |= DiffResult.PWNodesRemoved;
                    this.Remove(nodeInThisList);
                }
            }

            return diffResult;
        }

        public object Clone()
        {
            PAScheduleForPWNodeList clone = new PAScheduleForPWNodeList();
            foreach (PAScheduleForPWNode node in this)
            {
                clone.Add(node.Clone() as PAScheduleForPWNode);
            }
            return clone;
        }
    }
}
