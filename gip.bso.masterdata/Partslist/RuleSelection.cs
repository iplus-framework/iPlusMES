using gip.core.datamodel;
using System.Collections.Generic;
using gip.mes.facility;
using System.ComponentModel;
using System;
using dbMes = gip.mes.datamodel;
using System.ComponentModel.Design;

namespace gip.bso.masterdata
{

    [ACClassInfo(Const.PackName_VarioMaterial, "en{'RuleSelection'}de{'RuleSelection'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class RuleSelection : INotifyPropertyChanged
    {

        #region DI

        public RuleGroup RuleGroup { get; set; }

        public bool PropagateRuleSelection { get; set; }

        #endregion

        #region ctor's
        //public RuleSelection(RuleGroup ruleGroup, dbMes.Material material, RouteItem routeItem, string preConfigACUrl)
        //{
        //    RuleSelectionID = Guid.NewGuid();
        //    RuleGroup = ruleGroup;
        //    Material = material;
        //    RouteItem = routeItem;
        //    PreConfigACUrl = preConfigACUrl;
        //}
        #endregion

        #region Properies

        public Guid RuleSelectionID;

        public string PreConfigACUrl { get; set; }

        public string GetConfigACUrl(ACClassWF aCClassWF)
        {
            return aCClassWF?.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Excluded_process_modules.ToString();
        }

        [ACPropertyInfo(100, "", Const.Workflow)]
        public List<ACClassWF> PWNodes { get; private set; } = new List<ACClassWF>();

        [ACPropertyInfo(101, "", dbMes.ConstApp.Material)]
        public dbMes.Material Material { get; set; }

        [ACPropertyInfo(102, "", Const.Workflow)]
        public ACClass Source { get; set; }

        [ACPropertyInfo(103, "", Const.Workflow)]
        public ACClass Target { get; set; }

        private bool _IsSelected;
        [ACPropertyInfo(104, "", gip.mes.datamodel.ConstApp.Select)]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                OnPropertyChanged(nameof(IsSelected));
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
            string material = Material != null ? Material.ToString() : "-";
            return $"[{(IsSelected ? "x" : "")}] {material} | {Source} -> {Target}";
        }

        #endregion

    }
}
