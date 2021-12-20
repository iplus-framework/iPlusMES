using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;

namespace gip.bso.masterdata
{
    public delegate void RemindSelectedFacility(Facility facility);

    [ACClassInfo(Const.PackName_VarioFacility, "en{'Storage Location'}de{'Lagerplatz'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName)]
    public class BSOFacilityExplorer : ACBSOvb
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityLocation"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityExplorer(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }


        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool baseDeInit = base.ACDeInit(deleteACClassTask);
            _CurrentFacility = null;
            _CurrentFacilityRoot = null;
            return baseDeInit;
        }

        #endregion

        #region Properties 

        #region FacilityTree

        ACFSItem _CurrentFacilityRoot;
        ACFSItem _CurrentFacility;


        /// <summary>
        /// Gets or sets the current import project item root.
        /// </summary>
        /// <value>The current import project item root.</value>
        [ACPropertyCurrent(9999, "FacilityRoot")]
        public ACFSItem CurrentFacilityRoot
        {
            get
            {
                return _CurrentFacilityRoot;
            }
            set
            {
                _CurrentFacilityRoot = value;
                OnPropertyChanged("CurrentFacilityRoot");
            }

        }

        /// <summary>
        /// Gets or sets the current import project item.
        /// </summary>
        /// <value>The current import project item.</value>
        [ACPropertyCurrent(9999, "Facility")]
        public ACFSItem CurrentFacility
        {
            get
            {
                return _CurrentFacility;
            }
            set
            {
                if (
                        (_CurrentFacility == null && value != null)
                        || (_CurrentFacility != null && value == null)
                        || (_CurrentFacility.ACObject == null && value.ACObject != null)
                        || (_CurrentFacility.ACObject != null && value.ACObject == null)
                        || (
                                _CurrentFacility != null
                                && _CurrentFacility.ACObject != null
                                && value != null
                                && value.ACObject != null
                                && ((_CurrentFacility.ACObject as Facility).FacilityID != (value.ACObject as Facility).FacilityID)
                           )
                    )
                {
                    _CurrentFacility = value;
                    OnPropertyChanged("CurrentFacility");
                    OnPropertyChanged("SelectedFacility");
                }
            }
        }

        ChangeInfo _CurrentFacilityRootChangeInfo = null;
        /// <summary>
        /// Gets or sets the current project item root change info.
        /// </summary>
        /// <value>The current project item root change info.</value>
        [ACPropertyChangeInfo(404, "Facility")]
        public ChangeInfo CurrentFacilityRootChangeInfo
        {
            get
            {
                return _CurrentFacilityRootChangeInfo;
            }
            set
            {
                _CurrentFacilityRootChangeInfo = value;
                OnPropertyChanged("CurrentFacilityRootChangeInfo");
            }
        }

        [ACPropertyInfo(9999, "SelectedFacility")]
        public Facility SelectedFacility
        {
            get
            {
                if (CurrentFacility != null && CurrentFacility.ACObject != null)
                    return CurrentFacility.ACObject as Facility;
                return null;
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterFacility;
        [ACPropertySelected(999, "FilterFacility", "en{'Search'}de{'Suchen'}")]
        public string FilterFacility
        {
            get
            {
                return _FilterFacility;
            }
            set
            {
                if (_FilterFacility != value)
                {
                    _FilterFacility = value;
                    OnPropertyChanged("FilterFacility");
                    ACFSItem preselectedItem = null;
                    ACFSItem treeRoot = FacilityTree.LoadFacilityTree(DatabaseApp);
                    if (!string.IsNullOrEmpty(value))
                    {
                        Action<ACFSItem, object[]> filterAction = delegate (ACFSItem aCFSItem, object[] args)
                        {
                            if (aCFSItem.ACObject == null)
                                aCFSItem.IsVisible = true;
                            else
                            {
                                aCFSItem.IsVisible = false;
                                Facility facility = aCFSItem.ACObject as Facility;
                                if (facility.FacilityNo.ToLower().Contains(value.ToLower()) || facility.FacilityName.ToLower().Contains(value.ToLower()))
                                {
                                    aCFSItem.IsVisible = true;
                                    FacilityTree.SetupCurrentVisible(aCFSItem);
                                    if (preselectedItem == null)
                                    {
                                        preselectedItem = aCFSItem;
                                    }
                                }
                            }
                        };
                        treeRoot.CallAction(filterAction);
                    }
                    CurrentFacilityRoot = treeRoot;
                    CurrentFacility = preselectedItem;
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            ACFSItem rootItem = FacilityTree.LoadFacilityTree(DatabaseApp);
            rootItem.ShowFirst();
            CurrentFacilityRoot = rootItem;
        }

        public VBDialogResult DialogResult { get; set; }

        /// <summary>
        /// Source Property: ShowDialog
        /// </summary>
        [ACMethodInfo("ShowDialog", "en{'Select facility}de{'Lager auswählen'}", 999)]
        public VBDialogResult ShowDialog(Facility facility = null)
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            if (!IsEnabledShowDialog())
                return DialogResult;
            if (facility != null)
            {
                ACFSItem selectedItem = null;
                Action<ACFSItem, object[]> filterAction = delegate (ACFSItem aCFSItem, object[] args)
                {
                    if (aCFSItem.ACObject != null)
                    {
                        Facility tmpFacility = aCFSItem.ACObject as Facility;
                        if (tmpFacility.FacilityID == facility.FacilityID)
                            selectedItem = aCFSItem;
                    }
                };
                CurrentFacilityRoot.CallAction(filterAction);
                CurrentFacility = selectedItem;
            }
            ShowDialog(this, "Explorer");
            return DialogResult;
        }

        public bool IsEnabledShowDialog()
        {
            return true;
        }

        [ACMethodCommand("Dialog", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogOK()
        {
            if (DialogResult != null)
            {
                DialogResult.SelectedCommand = eMsgButton.OK;
                DialogResult.ReturnValue = SelectedFacility;
            }
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void DialogCancel()
        {
            CloseTopDialog();
        }

        #endregion
    }
}
