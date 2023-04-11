using gip.core.datamodel;
using System.Collections.Generic;
using gip.mes.facility;
using System.ComponentModel;
using System;

namespace gip.bso.masterdata
{

    [ACClassInfo(Const.PackName_VarioMaterial, "en{'RuleSelection'}de{'RuleSelection'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleSelection : INotifyPropertyChanged
    {

        #region DI

        public RuleGroup RuleGroup { get; private set; }

        public bool PropagateRuleSelection { get; set; }

        #endregion

        #region ctor's
        public RuleSelection(RuleGroup ruleGroup)
        {
            RuleGroup = ruleGroup;
            RuleSelectionID = Guid.NewGuid();
        }
        #endregion

        #region Properies

        public Guid RuleSelectionID;

        public string PreConfigACUrl { get; set; }

        public string ConfigACUrl_Excluded_process_modules
        {
            get
            {
                return WF?.PWNode?.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Excluded_process_modules.ToString();
            }
        }

        [ACPropertyInfo(100, "", Const.Workflow)]
        public MapPosToWFConn WF { get; set; }

        private List<RuleItem> _Items;
        [ACPropertyInfo(101, "", "en{'Items'}de{'Items'}")]
        public List<RuleItem> Items
        {
            get
            {
                return _Items;
            }

            set
            {
                _Items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (WF == null)
                return base.ToString();
            return $"{WF.PWNode?.ACCaption} - [{WF.MatWFConn?.Material?.MaterialNo}]";
        }

        #endregion

    }
}
