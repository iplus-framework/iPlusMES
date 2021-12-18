using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.ComponentModel;
using System.Linq;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Storage Location'}de{'Lagerplatz'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName)]
    public class BSOFacility : ACBSOvb
    {

        #region consts
        public const string ClassName = "BSOFacility";
        public const string ParameterName_ParentFacilityID = "ParentFacilityID";
        #endregion

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityLocation"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacility(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            ACFSItem rootItem = FacilityTree.LoadFacilityTree(DatabaseApp);
            rootItem.ShowFirst();
            CurrentFacilityRoot = rootItem;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        public override object Clone()
        {
            object clonedObject = base.Clone();
            BSOFacility clonedBSOFacility = clonedObject as BSOFacility;
            if (CurrentFacility != null && CurrentFacility.ACObject != null)
            {
                Facility facility = CurrentFacility.ACObject as Facility;
                clonedBSOFacility.CurrentFacility = new ACFSItem(null, CurrentFacility.Container, facility, FacilityTree.FacilityACCaption(facility), ResourceTypeEnum.IACObject);
            }
            return clonedBSOFacility;
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
                if (_CurrentFacility != value)
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
                            if(aCFSItem.ACObject == null)
                                aCFSItem.IsVisible = true;
                            else
                            {
                                aCFSItem.IsVisible = false;
                                Facility facility = aCFSItem.ACObject as Facility;
                                if(facility.FacilityNo.ToLower().Contains(value.ToLower()) || facility.FacilityName.ToLower().Contains(value.ToLower()))
                                {
                                    aCFSItem.IsVisible = true;
                                    FacilityTree.SetupCurrentVisible(aCFSItem);
                                    if(preselectedItem == null)
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
        #region Mehtods ->  BSO -> ACMethod

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("StorageLocation", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Material.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
            DatabaseApp.OnPropertyChanged(Facility.ClassName);
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }


        #region Mehtods ->  BSO -> ACMethod -> Tree operation

        [ACMethodInteraction("AddFacility", "en{'New'}de{'Neu'}", (short)MISort.New, true, "CurrentFacility", Global.ACKinds.MSMethodPrePost)]
        public virtual void AddFacility()
        {
            if (!IsEnabledAddFacility()) return;
            ACFSItem currentFacilityACFSItem = CurrentFacility;

            Facility parentFacility = null;
            if (CurrentFacility.ACObject != null)
                parentFacility = CurrentFacility.ACObject as Facility;
            Facility facility = Facility.NewACObject(DatabaseApp, parentFacility);

            MDFacilityType mDFacilityType = DatabaseApp.MDFacilityType.FirstOrDefault(C => C.IsDefault);
            if (mDFacilityType == null)
                mDFacilityType = DatabaseApp.MDFacilityType.OrderBy(c => c.SortIndex).FirstOrDefault();
            int nr = 0;
            Action<ACFSItem> countNewItems = null;
            countNewItems = (ACFSItem item) =>
            {
                if (item.ACObject != null && (item.ACObject as Facility).FacilityNo.StartsWith("NewFacility"))
                    nr++;
                foreach (var childItem in item.Items)
                    countNewItems(childItem as ACFSItem);
            };
            countNewItems(CurrentFacilityRoot);

            facility.FacilityNo = string.Format(@"NewFacility{0}", nr + 1);
            facility.FacilityName = "-";
            facility.MDFacilityType = mDFacilityType;
            facility.MDFacilityTypeID = mDFacilityType.MDFacilityTypeID;
            DatabaseApp.Facility.AddObject(facility);
            if (parentFacility != null)
                parentFacility.Facility_ParentFacility.Add(facility);

            ACFSItem newFacilityACFSItem = new ACFSItem(null, CurrentFacility.Container, facility, FacilityTree.FacilityACCaption(facility), ResourceTypeEnum.IACObject);
            newFacilityACFSItem.OnACFSItemChange += FacilityTree.CFSItem_OnACFSItemChange;
            currentFacilityACFSItem.Add(newFacilityACFSItem);

            CurrentFacilityRootChangeInfo = new ChangeInfo(currentFacilityACFSItem, newFacilityACFSItem, Const.CmdAddChildData);

            OnPropertyChanged("CurrentFacility");
            OnPropertyChanged("SelectedFacility");
        }

        public virtual bool IsEnabledAddFacility()
        {
            return CurrentFacility != null;
        }

        [ACMethodInteraction("DeleteFacility", "en{'Delete'}de{'Loschen'}", (short)MISort.Delete, true, "CurrentFacility", Global.ACKinds.MSMethodPrePost)]
        public virtual void DeleteFacility()
        {
            if (!IsEnabledDeleteFacility()) return;
            ACFSItem parent = CurrentFacility.ParentACObject as ACFSItem;
            ACFSItem current = CurrentFacility;

            parent.Remove(current);

            Facility dbFacility = current.ACObject as Facility;
            MsgWithDetails msg = dbFacility.DeleteACObject(DatabaseApp, false);

            CurrentFacility = parent;

            CurrentFacilityRootChangeInfo = new ChangeInfo(null, CurrentFacility, Const.CmdDeleteData);


            OnPropertyChanged("CurrentFacility");
            OnPropertyChanged("SelectedFacility");
        }
        public virtual bool IsEnabledDeleteFacility()
        {
            return CurrentFacility != null && CurrentFacility.ACObject != null && !CurrentFacility.Items.Any();
        }


        #endregion

        #endregion

        #endregion

    }
}
