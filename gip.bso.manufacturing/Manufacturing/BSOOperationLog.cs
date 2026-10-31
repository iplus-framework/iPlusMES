// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Business object or App for viewing and closing open operation logs (scan bookings) of process functions.
    /// This is a viewer app without a Current entity: select a process function in the SelectedProcessFunction
    /// property (ProcessFunctionsList contains all process functions derived from PAFWorkInOutOperation); the open
    /// operation logs (OperationState = Open) of this function are loaded into the OperationLogList property and the
    /// selected log is in the SelectedOperationLog property.
    /// The SearchText property filters the OperationLogList by material number, material name, program number and
    /// lot number (reloading occurs when the text is cleared).
    /// The OpLogGroupItemList property contains the logs grouped by material (GroupByMaterial) or by lot (GroupByLOT);
    /// the active grouping is set by activating the corresponding tab (All / Material / LOT) and the grouped result is
    /// selected in the SelectedOpLogGroupItem property. Each group item sums the stock quantities of its charges and
    /// converts them into the unit variants A/B/C of the material.
    /// CloseSelectedOperationLog() closes the selected operation log (OperationLog.CloseOperationLog) and reloads the
    /// list; only enabled for open "RegisterEntityOnScan" logs.
    /// NavigateToQuantManagement() opens the quant management dialog for the facility charge of the selected log.
    /// This BSO is read-only regarding the database except for closing operation logs; there are no New/Save/Delete
    /// entity workflows.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOperationLog&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Operation logs'}de{'Betriebsprotokolle'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"Business object or App for viewing and closing open operation logs (scan bookings) of process functions.
        This is a viewer app without a Current entity: select a process function in the SelectedProcessFunction property (ProcessFunctionsList contains all process functions derived from PAFWorkInOutOperation); the open operation logs (OperationState = Open) of this function are loaded into the OperationLogList property and the selected log is in the SelectedOperationLog property.
        The SearchText property filters the OperationLogList by material number, material name, program number and lot number (reloading occurs when the text is cleared).
        The OpLogGroupItemList property contains the logs grouped by material (GroupByMaterial) or by lot (GroupByLOT); the active grouping is set by activating the corresponding tab (All / Material / LOT) and the grouped result is selected in the SelectedOpLogGroupItem property. Each group item sums the stock quantities of its charges and converts them into the unit variants A/B/C of the material.
        CloseSelectedOperationLog() closes the selected operation log (OperationLog.CloseOperationLog) and reloads the list; only enabled for open ""RegisterEntityOnScan"" logs.
        NavigateToQuantManagement() opens the quant management dialog for the facility charge of the selected log.
        This BSO is read-only regarding the database except for closing operation logs; there are no New/Save/Delete entity workflows.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOOperationLog&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOOperationLog : ACBSOvb
    {
        #region c'tors

        public BSOOperationLog(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();

            ProcessFunctionsList = s_cQry_GetRelevantPAProcessFunctions(DatabaseApp.ContextIPlus, nameof(gip.mes.processapplication.PAFWorkInOutOperation))
                                                        .ToArray().OrderBy(c => c.ACCaption).ToList();

            if (ProcessFunctionsList != null && ProcessFunctionsList.Any())
            {
                SelectedProcessFunction = ProcessFunctionsList.FirstOrDefault();
            }

            return result;
        }

        #endregion

        #region Properties

        private const string LOTGroupVBContent = "LOT";
        private const string MaterialGroupVBContent = "Material";

        private core.datamodel.ACClass _SelectedProcessFunction;
        [ACPropertySelected(9999, "ProcessFunction", "en{'Selected process function'}de{'Ausgewählte Prozessfunktion'}", Description = @"Selected process function whose open operation logs are displayed. Setting it reloads the OperationLogList.")]
        public core.datamodel.ACClass SelectedProcessFunction
        {
            get => _SelectedProcessFunction;
            set
            {
                _SelectedProcessFunction = value;
                LoadOperationLogs();
                OnPropertyChanged();
            }
        }

        private List<core.datamodel.ACClass> _ProcessFunctionsList;
        [ACPropertyList(9999, "ProcessFunction", Description = @"All process functions derived from PAFWorkInOutOperation (up to 5 inheritance levels) available for selection.")]
        public List<core.datamodel.ACClass> ProcessFunctionsList
        {
            get => _ProcessFunctionsList;
            set
            {
                _ProcessFunctionsList = value;
                OnPropertyChanged();
            }
        }

        private OperationLog _SelectedOperationLog;
        [ACPropertySelected(9999, "OperationLog", "en{'Operation log'}de{'Betriebsprotokoll'}", Description = @"Currently selected operation log in the list; target of CloseSelectedOperationLog() and NavigateToQuantManagement().")]
        public OperationLog SelectedOperationLog
        {
            get => _SelectedOperationLog;
            set
            {
                _SelectedOperationLog = value;
                OnPropertyChanged();
            }
        }

        private List<OperationLog> _OperationLogList;
        [ACPropertyList(9999, "OperationLog", Description = @"Open operation logs (OperationState = Open) of the selected process function, ordered by operation time; filtered by SearchText.")]
        public List<OperationLog> OperationLogList
        {
            get => _OperationLogList;
            set
            {
                _OperationLogList = value;
                OnPropertyChanged();
                OnPropertyChanged();
            }
        }

        private OperationLogGroupItem _SelectedOpLogGroupItem;
        [ACPropertySelected(9999, "OperationLogGroup", "en{'Operation log group'}de{'Betriebsprotokoll Gruppe'}", Description = @"Currently selected group item (material or lot) in the grouped view.")]
        public OperationLogGroupItem SelectedOpLogGroupItem
        {
            get => _SelectedOpLogGroupItem;
            set
            {
                _SelectedOpLogGroupItem = value;
                OnPropertyChanged();
            }
        }

        private List<OperationLogGroupItem> _OpLogGroupItemList;
        [ACPropertyList(9999, "OperationLogGroup", Description = @"Operation logs grouped by material or by lot (depending on the active tab); each item sums the stock quantities and converts them into unit variants A/B/C.")]
        public List<OperationLogGroupItem> OpLogGroupItemList
        {
            get => _OpLogGroupItemList;
            set
            {
                _OpLogGroupItemList = value;
                OnPropertyChanged();
            }
        }

        private string _SearchText;
        [ACPropertyInfo(9999, "", "en{'Search text'}de{'Suchtext'}", Description = @"Filters the OperationLogList by material number, material name, program number and lot number. Clearing the text reloads the full list.")]
        public string SearchText
        {
            get => _SearchText;
            set
            {
                if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(_SearchText))
                {
                    LoadOperationLogs();
                }
                else
                {
                    string searchText = value.ToLower();

                    OperationLogList = OperationLogList.Where(c => c.FacilityCharge.Material.MaterialNo.ToLower().Contains(searchText) 
                                                                || c.FacilityCharge.Material.MaterialName1.ToLower().Contains(searchText)
                                                                || c.FacilityCharge.ProdOrderProgramNo.ToLower().Contains(searchText)
                                                                || (c.FacilityCharge.FacilityLot == null || c.FacilityCharge.FacilityLot.LotNo.ToLower().Contains(searchText))).ToList();

                    if (_CurrentGroupTabVBContent == MaterialGroupVBContent)
                    {
                        GroupByMaterial();
                    }
                    else if (_CurrentGroupTabVBContent == LOTGroupVBContent)
                    {
                        GroupByLOT();
                    }
                }
                _SearchText = value;
                OnPropertyChanged();
            }
        }

        private string _CurrentGroupTabVBContent;

        #endregion

        #region Methods

        private void LoadOperationLogs()
        {
            if (_SelectedProcessFunction != null)
            {
                OperationLogList = DatabaseApp.OperationLog.Include(c => c.FacilityCharge)
                                                           .Include(c => c.FacilityCharge.Material)
                                                           .Include(c => c.FacilityCharge.Material.BaseMDUnit)
                                                           .Include("FacilityCharge.Material.MaterialUnit_Material.ToMDUnit")
                                                           .Include(c => c.FacilityCharge.FacilityLot)
                                                           .Where(c => c.RefACClassID == _SelectedProcessFunction.ACClassID
                                                                    && c.OperationState == (short)OperationLogStateEnum.Open)
                                                           .OrderBy(c => c.OperationTime)
                                                           .ToList();

                if (_CurrentGroupTabVBContent == MaterialGroupVBContent)
                {
                    GroupByMaterial();
                }
                else if (_CurrentGroupTabVBContent == LOTGroupVBContent)
                {
                    GroupByLOT();
                }
            }
            else
            {
                OperationLogList = null;
            }
        }

        private void GroupByMaterial()
        {
            List<OperationLogGroupItem> result = new List<OperationLogGroupItem>();

            var groupedByMaterial = OperationLogList.GroupBy(c => c.FacilityCharge.Material);

            foreach (var key in groupedByMaterial)
            {
                OperationLogGroupItem item = new OperationLogGroupItem()
                {
                    Material = key.Key,
                    Unit = key.Where(c => c.FacilityCharge != null).FirstOrDefault()?.FacilityCharge?.MDUnit,
                    StockQuantity = key.Where(c => c.FacilityCharge != null).Sum(c => c.FacilityCharge.StockQuantity)
                };
                result.Add(item);

                Tuple<MDUnit, double> conv = item.Material.ConvertBaseQuantity(item.StockQuantity, 0);
                item.StockUnitA = conv != null ? conv.Item2 : 0;
                item.UnitA = conv != null ? conv.Item1.MDUnitName : null;
                conv = item.Material.ConvertBaseQuantity(item.StockQuantity, 1);
                item.StockUnitB = conv != null ? conv.Item2 : 0;
                item.UnitB = conv != null ? conv.Item1.MDUnitName : null;
                conv = item.Material.ConvertBaseQuantity(item.StockQuantity, 2);
                item.StockUnitC = conv != null ? conv.Item2 : 0;
                item.UnitC = conv != null ? conv.Item1.MDUnitName : null;
            }

            OpLogGroupItemList = result;
        }

        private void GroupByLOT()
        {
            List<OperationLogGroupItem> result = new List<OperationLogGroupItem>();

            var groupedByLot = OperationLogList.GroupBy(c => c.FacilityCharge.FacilityLot);

            foreach (var key in groupedByLot)
            {
                OperationLogGroupItem item = new OperationLogGroupItem()
                {
                    Lot = key.Key,
                    Material = key.Where(c => c.FacilityCharge != null).FirstOrDefault()?.FacilityCharge.Material,
                    Unit = key.Where(c => c.FacilityCharge != null).FirstOrDefault()?.FacilityCharge?.MDUnit,
                    StockQuantity = key.Where(c => c.FacilityCharge != null).Sum(c => c.FacilityCharge.StockQuantity)
                };
                result.Add(item);

                Tuple<MDUnit, double> conv = item.Material.ConvertBaseQuantity(item.StockQuantity, 0);
                item.StockUnitA = conv != null ? conv.Item2 : 0;
                item.UnitA = conv != null ? conv.Item1.MDUnitName : null;
                conv = item.Material.ConvertBaseQuantity(item.StockQuantity, 1);
                item.StockUnitB = conv != null ? conv.Item2 : 0;
                item.UnitB = conv != null ? conv.Item1.MDUnitName : null;
                conv = item.Material.ConvertBaseQuantity(item.StockQuantity, 2);
                item.StockUnitC = conv != null ? conv.Item2 : 0;
                item.UnitC = conv != null ? conv.Item1.MDUnitName : null;
            }

            OpLogGroupItemList = result;
        }

        [ACMethodInfo("", "en{'Close operation'}de{'Betrieb schließen'}", 9999, true, Description = @"Closes the selected operation log (only open RegisterEntityOnScan logs) and reloads the list of open logs.")]
        public void CloseSelectedOperationLog()
        {
            MsgWithDetails msg = OperationLog.CloseOperationLog(DatabaseApp, SelectedOperationLog, null);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            OperationLogList = DatabaseApp.OperationLog.Where(c => c.RefACClassID == _SelectedProcessFunction.ACClassID
                                                                        && c.OperationState == (short)OperationLogStateEnum.Open)
                                                               .OrderBy(c => c.OperationTime)
                                                               .ToList();
        }

        public bool IsEnabledCloseSelectedOperationLog()
        {
            return SelectedOperationLog != null && SelectedOperationLog.OperationState == (short)OperationLogStateEnum.Open
                                                && SelectedOperationLog.Operation == (short)OperationLogEnum.RegisterEntityOnScan;
        }

        [ACMethodInteraction("", "en{'Navigate to Quantmanagement'}de{'Navigieren Sie zu Quantverwaltung'}", 9999, true, nameof(SelectedOperationLog), Description = @"Opens the quant management dialog for the facility charge of the selected operation log.")]
        public void NavigateToQuantManagement()
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 0 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityCharge), SelectedOperationLog.FacilityCharge.FacilityChargeID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToQuantManagement()
        {
            return SelectedOperationLog != null && SelectedOperationLog.FacilityCharge != null;
        }


        public override void ACAction(ACActionArgs actionArgs)
        {
            base.ACAction(actionArgs);

            if (actionArgs.ElementAction == Global.ElementActionType.TabItemActivated)
            {
                if (actionArgs.DropObject.VBContent == MaterialGroupVBContent)
                {
                    GroupByMaterial();
                }
                else if (actionArgs.DropObject.VBContent == LOTGroupVBContent)
                {
                    GroupByLOT();
                }
                _CurrentGroupTabVBContent = actionArgs.DropObject.VBContent;
            }

        }

        #endregion

#region PrecompiledQuery
        public static readonly Func<Database, string, IEnumerable<gip.core.datamodel.ACClass>> s_cQry_GetRelevantPAProcessFunctions =
            EF.CompileQuery<Database, string, IEnumerable<gip.core.datamodel.ACClass>>(
                                                (ctx, pafACIdentifier) => ctx.ACClass.Where(c => (c.BasedOnACClassID.HasValue
                                                    && (c.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 1. Ableitungsstufe
                                                        || (c.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 2. Ableitungsstufe
                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 3. Ableitungsstufe
                                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                        && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier) // 4. Ableitungsstufe
                                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier) // 5. Ableitungsstufe
                                                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                    && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier)
                                                                                    )
                                                                        )
                                                                    )
                                                            )
                                                        )



    ) && c.ACProject != null && c.ACProject.ACProjectTypeIndex == (short)Global.ACProjectTypes.Application && c.ACStartTypeIndex == (short)Global.ACStartTypes.Automatic));

        #endregion

        #region HandleExecuteACMethod

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch (acMethodName)
            {
                case nameof(CloseSelectedOperationLog):
                    CloseSelectedOperationLog();
                    return true;
                case nameof(IsEnabledCloseSelectedOperationLog):
                    result = IsEnabledCloseSelectedOperationLog();
                    return true;
                case nameof(NavigateToQuantManagement):
                    NavigateToQuantManagement();
                    return true;
                case nameof(IsEnabledNavigateToQuantManagement):
                    result = IsEnabledNavigateToQuantManagement();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(CloseSelectedOperationLog):
                case nameof(IsEnabledCloseSelectedOperationLog):
                    return new string[] { nameof(SelectedOperationLog) };
                case nameof(NavigateToQuantManagement):
                case nameof(IsEnabledNavigateToQuantManagement):
                    return new string[] { nameof(SelectedOperationLog) };
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion
    }
}
