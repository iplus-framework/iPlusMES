using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip2006.variobatch.processapplication
{
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'Tool for CONV'}de{'Tool for CONV'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, false, true)]
    public class BSOGipConv2006VarioTool : ACBSO
    {
        public BSOGipConv2006VarioTool(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        [ACPropertyInfo(9999)]
        public UInt16 DataBlockCMD
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public UInt16 DataBlockState
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public UInt16 OffsetToStartCMD
        {
            get;
            set;
        }

        [ACPropertyInfo(9999)]
        public UInt16 OffsetToStartState
        {
            get;
            set;
        }

        private List<string> _Errors;
        [ACPropertyInfo(9999)]
        public List<string> Errors
        {
            get => _Errors;
            set
            {
                _Errors = value;
                OnPropertyChanged("Errors");
            }
        }

        [ACMethodInfo("","",9999)]
        public void InsertDBAndOffsetAccordingAggrNo()
        {
            List<string> errorACUrl = new List<string>();

            var appManagers = Root.FindChildComponents<ApplicationManager>(c => c is ApplicationManager 
                                                                             && c.ComponentClass.ACProject.ACProjectTypeIndex == (short)Global.ACProjectTypes.Application);

            using (Database db = new gip.core.datamodel.Database())
            {

                foreach (var appManager in appManagers)
                {
                    var converters = Root.FindChildComponents<GIPConv2006Vario>(c => c is GIPConv2006Vario);

                    foreach (var conv in converters)
                    {
                        short aggrNo = conv.AggrNo.ValueT;
                        if (aggrNo <= 0)
                        {
                            errorACUrl.Add(conv.ACUrl + "AggrNo is 0");
                            continue;
                        }

                        UInt16 offsetParam = Convert.ToUInt16(aggrNo * 100 + OffsetToStartCMD);
                        UInt16 offsetResult = Convert.ToUInt16(aggrNo * 100 + OffsetToStartState);

                        ACClass compClass = conv.ComponentClass.FromIPlusContext<ACClass>(db);

                        IACConfig cmdDBNo = compClass.ConfigurationEntries.FirstOrDefault(c => c.LocalConfigACUrl == "CmdDBNo");
                        IACConfig stateDBNo = compClass.ConfigurationEntries.FirstOrDefault(c => c.LocalConfigACUrl == "StateDBNo");
                        IACConfig cmdDBOffset = compClass.ConfigurationEntries.FirstOrDefault(c => c.LocalConfigACUrl == "CmdDBOffset");
                        IACConfig stateDBOffset = compClass.ConfigurationEntries.FirstOrDefault(c => c.LocalConfigACUrl == "StateDBOffset");

                        if (cmdDBNo == null || stateDBNo == null || cmdDBOffset == null || stateDBOffset == null)
                        {
                            errorACUrl.Add(conv.ACUrl + "Missing configuration entries");
                            continue;
                        }

                        cmdDBNo.Value = DataBlockCMD;
                        cmdDBOffset.Value = offsetParam;

                        stateDBNo.Value = DataBlockState;
                        stateDBOffset.Value = offsetResult;
                    }
                }

                Msg msg = db.ACSaveChanges();
                if (msg != null)
                    Messages.Msg(msg);

                Errors = errorACUrl;
            }

        }
    }
}
