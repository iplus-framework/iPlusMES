// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.test
{
    [ACClassInfo(Const.PackName_VarioTest, "en{'Methodtest'}de{'Methodentest'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, false, true)]
    public class VXBSOMethodTest : ACBSOvb
    {
        #region c´tors

        public VXBSOMethodTest(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }
        #endregion

        public override void TaskCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            base.TaskCallback(sender, e, wrapObject);
        }

        #region BSO->ACProperty
        [ACPropertyInfo(9999, "", "en{'ACUrlOfProject'}de{'ACUrlOfProject'}")]
        public string ACUrlOfProject
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", "en{'Methodname'}de{'Methodennamme'}")]
        public string MethodName
        {
            get;set;
        }

        Nullable<Boolean> _CurrentCheck = null;
        [ACPropertyCurrent(999,"", "en{'Check'}de{'Check'}")]
        public Nullable<Boolean> CurrentCheck
        {
            get
            {
                return _CurrentCheck;
            }
            set
            {
                _CurrentCheck = value;
                OnPropertyChanged("CurrentCheck");
            }
        }
        #endregion

        #region BSO->ACMethod

        [ACMethodInfo("", "en{'Assembly Parameter'}de{'Assembly Parameter'}", 9999, false, false, true)]
        public void CallAssemblyParameter()
        {
            var result = ACUrlCommand("!Subtrahiere", 20, 14);
        }

        [ACMethodInfo("", "en{'Assembly ACMethod'}de{'Assembly ACMethod'}", 9999, false, false, true)]
        public void CallAssemblyACMethod()
        {
            ACClass acClass = this.ACType as ACClass;
            ACClassMethod acClassMethod = acClass.Methods.Where(c => c.ACIdentifier == "Subtrahiere").First();
            ACMethod acMethod = acClassMethod.TypeACSignature();
            acMethod.ParameterValueList["valueA"] = 20;
            acMethod.ParameterValueList["valueB"] = 14;

            var result = ACUrlCommand("!Subtrahiere", acMethod);
        }

        [ACMethodInfo("", "en{'Assembly ACMethod(SP)'}de{'Assembly ACMethod(SP)'}", 9999, false, false, true)]
        public void CallAssemblyACMethodSP()
        {
        }

        [ACMethodInfo("", "en{'Script Parameter'}de{'Script Parameter'}", 9999, false, false, true)]
        public void CallScriptParameter()
        {
            var result = ACUrlCommand("!Addiere", 20, 14);
        }

        [ACMethodInfo("", "en{'Script ACMethod'}de{'Script ACMethod'}", 9999, false, false, true)]
        public void CallScriptACMethod()
        {
            ACClass acClass = this.ACType as ACClass;
            ACClassMethod acClassMethod = acClass.Methods.Where(c => c.ACIdentifier == "Addiere").First();
            ACMethod acMethod = acClassMethod.TypeACSignature();
            acMethod.ParameterValueList["valueA"] = 20;
            acMethod.ParameterValueList["valueB"] = 14;

            var result = ACUrlCommand("!Addiere", acMethod);
        }

        [ACMethodInfo("", "en{'Script ACMethod(SP)'}de{'Script ACMethod(SP)'}", 9999, false, false, true)]
        public void CallScriptACMethodSP()
        {
        }

        [ACMethodInfo("", "en{'Workflow ACMethod'}de{'Workflow ACMethod'}", 9999, false, false, true)]
        public void CallSyncWorkflowACMethod()
        {
            ACClass acClass = this.ACType as ACClass;
            ACClassMethod acClassMethod = acClass.Methods.Where(c => c.ACIdentifier == "Multipliziere").First();
            ACMethod acMethod = acClassMethod.TypeACSignature();
            acMethod.ParameterValueList["valueA"] = 20;
            acMethod.ParameterValueList["valueB"] = 14;

            acMethod.ResultValueList["result"] = 280;
            var result = ACUrlCommand("!Multipliziere", acMethod);
        }

        [ACMethodInfo("", "en{'Workflow ACMethod(SP)'}de{'Workflow ACMethod(SP)'}", 9999, false, false, true)]
        public void CallSyncWorkflowACMethodSP()
        {
        }

        [ACMethodInfo("", "en{'Async Test'}de{'Async Test'}", 9999, false, false, true)]
        public void CallAsyncMethod()
        {
            ACComponent mailComp = this.ACUrlCommand("\\DataAccess\\Mail") as ACComponent;
            if (mailComp == null)
                return;

            //ACMethod acMethod = mailComp.ACUrlACTypeSignature("!SendMailAsync", Database);
            //acMethod.ParameterValueList["From"] = "damir.lisak@automation-gip.de";
            //acMethod.ParameterValueList["Recipients"] = "damir.lisak@kajbum.com";
            //acMethod.ParameterValueList["Subject"] = "Test";
            //acMethod.ParameterValueList["Body"] = "Halo, Halo";

            ACMethod acMethod = mailComp.ACUrlACTypeSignature("!SendMailToMailingListAsync", DatabaseApp.ContextIPlus);
            acMethod.ParameterValueList["Subject"] = "Test";
            acMethod.ParameterValueList["Body"] = "Halo, Halo";

            IACPointAsyncRMI rmiInvocationPoint = mailComp.GetPoint(Const.TaskInvocationPoint) as IACPointAsyncRMI;
            if (rmiInvocationPoint != null)
                rmiInvocationPoint.AddTask(acMethod, this);
        }

        [ACMethodInfo("", "en{'WorkOrder ACMethod'}de{'WorkOrder ACMethod'}", 9999, false, false, true)]
        public void CallAsyncWorkflowProgram()
        {
            if (String.IsNullOrEmpty(ACUrlOfProject) || String.IsNullOrEmpty(MethodName))
                return;
            ACComponent linie = ACUrlCommand("\\Produktionslinie1", null) as ACComponent;
            if (linie == null)
                return;

            //IEnumerable<ACChildInstanceInfo> childs = linie.GetChildInstanceInfo(1,true);
            //if (childs != null)
            //{
            //    var query = childs.Where(c => c.ACType.Obj.IsWorkflowType);
            //    if (query.Any())
            //    {
            //        foreach (ACChildInstanceInfo child in childs)
            //        {
            //            IEnumerable<ACChildInstanceInfo> childsWF = linie.GetChildInstanceInfo(0, true, child.ACIdentifier);
            //            if (childsWF != null && childsWF.Any())
            //            {
            //                ACChildInstanceInfo childWF = childsWF.First();
            //                if (linie.IsProxy)
            //                {
            //                    IACComponent wfInstance = StartACComponentByInstanceInfo(childWF, Global.ACStartTypes.Automatic, true);
            //                    if (wfInstance == null)
            //                        continue;
            //                }
            //            }
            //        }
            //    }
            //}

            linie = ACUrlCommand(ACUrlOfProject, null) as ACComponent;
            if (linie == null)
                return;

            ACMethod acMethod = linie.NewACMethod(MethodName);
            if (acMethod == null)
                return;
            // Norbert: Warum steht im ACNameIdentier "Workflow1116" ???
            if (acMethod.ACIdentifier != MethodName)
            {
                acMethod.ACIdentifier = MethodName;
            }

            IACPointAsyncRMI rmiInvocationPoint = linie.GetPoint(Const.TaskInvocationPoint) as IACPointAsyncRMI;
            if (rmiInvocationPoint != null)
                rmiInvocationPoint.AddTask(acMethod, this);

            //ACClass acClass = this.ACType as ACClass;
            //ACClassMethod acClassMethod = acClass.MyACClassMethodList.Where(c => c.ACIdentifier == "Dividiere").First();
            //ACMethod acMethod = acClassMethod.NewACMethod();

            //acMethod.ParameterValueList[ACProgram.ClassName] = Database.ACProgram.Where(c=>c.ProgramNo == "1000").First();

            //var result = ACUrlCommand("!Dividiere", acMethod);
        }

        [ACMethodInfo("", "en{'WorkOrder ACMethod(SP)'}de{'WorkOrder ACMethod(SP)'}", 9999, false, false, true)]
        public void CallAsyncWorkflowProgramSP()
        {
            ACComponent linie = ACUrlCommand("\\Produktionslinie1", null) as ACComponent;
            if (linie == null)
                return;
            ACMethod acMethod = linie.NewACMethod("Workflow1151");
            if (acMethod == null)
                return;
            // Norbert: Warum steht im ACNameIdentier "Workflow1116" ???
            if (acMethod.ACIdentifier != "Workflow1151")
            {
                acMethod.ACIdentifier = "Workflow1151";
            }
            var result = linie.ACUrlCommand("!Workflow1151", acMethod);
        }

        [ACMethodInfo("", "en{'Subtrahiere'}de{'Subtrahiere'}", 9999, false, false, false)]
        public int Subtrahiere(int valueA, int valueB)
        {
            return valueA - valueB;
        }
        #endregion

        #region Test Variobatch
        [ACMethodInfo("", "en{'TestVariobatch'}de{'TestVariobatch'}", 9999, false, false, true)]
        public void TestVariobatch()
        {
            // VariobatchTest-Instanz erzeugen
            var variobatchTest = ACUrlCommand("\\" + Const.ACRootProjectNameTest) as ACRoot;

            // Der Backslash liefert nun auch die VariobatchTest-Instanz
            var refVariobatchTest = variobatchTest.ACUrlCommand("\\");

            // Nur über den Trick kommt man zurück
            var refVariobatchLive = variobatchTest.ACUrlCommand("\\..");
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"CallAssemblyParameter":
                    CallAssemblyParameter();
                    return true;
                case"CallAssemblyACMethod":
                    CallAssemblyACMethod();
                    return true;
                case"CallAssemblyACMethodSP":
                    CallAssemblyACMethodSP();
                    return true;
                case"CallScriptParameter":
                    CallScriptParameter();
                    return true;
                case"CallScriptACMethod":
                    CallScriptACMethod();
                    return true;
                case"CallScriptACMethodSP":
                    CallScriptACMethodSP();
                    return true;
                case"CallSyncWorkflowACMethod":
                    CallSyncWorkflowACMethod();
                    return true;
                case"CallSyncWorkflowACMethodSP":
                    CallSyncWorkflowACMethodSP();
                    return true;
                case"CallAsyncMethod":
                    CallAsyncMethod();
                    return true;
                case"CallAsyncWorkflowProgram":
                    CallAsyncWorkflowProgram();
                    return true;
                case"CallAsyncWorkflowProgramSP":
                    CallAsyncWorkflowProgramSP();
                    return true;
                case"Subtrahiere":
                    result = Subtrahiere((Int32)acParameter[0], (Int32)acParameter[1]);
                    return true;
                case"TestVariobatch":
                    TestVariobatch();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
