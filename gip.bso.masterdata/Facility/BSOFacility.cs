using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using Microsoft.WindowsAPICodePack.ShellExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Spatial;
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
            ACFSItem rootItem = GetRootFacilityACFSItem();
            FactoryFacilityACFSItem(DatabaseApp, rootItem);
            CurrentFacilityRoot = rootItem;
            CurrentFacility = CurrentFacilityRoot;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        public override object Clone()
        {
            object clonedObject =  base.Clone();
            BSOFacility clonedBSOFacility = clonedObject as BSOFacility;
            if (CurrentFacility != null && CurrentFacility.ACObject != null)
            {
                Facility facility = CurrentFacility.ACObject as Facility;
                clonedBSOFacility.CurrentFacility = new ACFSItem(null, CurrentFacility.Container, facility, FacilityACCaption(facility), ResourceTypeEnum.IACObject);
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
                    if (_CurrentFacility != null && _CurrentFacility.ACObject != null)
                        (_CurrentFacility.ACObject as INotifyPropertyChanged).PropertyChanged -= _CurrentFacility_PropertyChanged;
                    _CurrentFacility = value;
                    if (_CurrentFacility != null && _CurrentFacility.ACObject != null)
                        (_CurrentFacility.ACObject as INotifyPropertyChanged).PropertyChanged += _CurrentFacility_PropertyChanged;
                    OnPropertyChanged("CurrentFacility");
                    OnPropertyChanged("SelectedFacility");
                }
            }
        }

        private void _CurrentFacility_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FacilityNo" || e.PropertyName == "FacilityName")
            {
                ACFSItem current = CurrentFacility;
                current.ACCaption = FacilityACCaption(CurrentFacility.ACObject as Facility);
                CurrentFacilityRoot = GetNewRootFacilityACFSItem();
                CurrentFacility = current;
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

            ACFSItem newFacilityACFSItem = new ACFSItem(null, CurrentFacility.Container, facility, FacilityACCaption(facility), ResourceTypeEnum.IACObject);
            currentFacilityACFSItem.Add(newFacilityACFSItem);
            ACFSItem tmpItem = newFacilityACFSItem;
            while (tmpItem != null)
            {
                tmpItem.IsVisible = true;
                if (tmpItem.ParentACObject != null)
                    tmpItem = tmpItem.ParentACObject as ACFSItem;
                else
                    break;
            }


            CurrentFacilityRoot = GetNewRootFacilityACFSItem();
            CurrentFacility = newFacilityACFSItem;
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

            CurrentFacilityRoot = GetNewRootFacilityACFSItem();
            CurrentFacility = parent;
        }

        public virtual bool IsEnabledDeleteFacility()
        {
            return CurrentFacility != null && CurrentFacility.ACObject != null && !CurrentFacility.Items.Any();
        }


        #endregion

        #endregion


        #region private methods


        private ACFSItem GetRootFacilityACFSItem()
        {
            string rootACCaption = Translator.GetTranslation(ConstApp.Facility);
            ACFSItem rootItem = new ACFSItem(null, new ACFSItemContainer(Database), null, rootACCaption, ResourceTypeEnum.IACObject);
            rootItem.IsVisible = true;
            return rootItem;
        }

        public ACFSItem GetNewRootFacilityACFSItem()
        {
            ACFSItem rootFacilityACFSItem = GetRootFacilityACFSItem();
            foreach (var item in CurrentFacilityRoot.Items)
                rootFacilityACFSItem.Add(item);
            return rootFacilityACFSItem;
        }

        private void FactoryFacilityACFSItem(DatabaseApp databaseApp, ACFSItem parentACFSItem)
        {
            Guid? parentFaciltiyID = null;
            if (parentACFSItem.ACObject != null)
            {
                Facility parentFacility = parentACFSItem.ACObject as Facility;
                parentFaciltiyID = parentFacility.FacilityID;
            }

            List<Facility> levelFacilities = DatabaseApp.Facility.Where(c => (c.ParentFacilityID ?? Guid.Empty) == (parentFaciltiyID ?? Guid.Empty)).OrderBy(c => c.FacilityNo).ToList();
            foreach (Facility facility in levelFacilities)
            {
                ACFSItem cFSItem = new ACFSItem(null, parentACFSItem.Container, facility, FacilityACCaption(facility), ResourceTypeEnum.IACObject);
                FactoryFacilityACFSItem(databaseApp, cFSItem);
                parentACFSItem.Add(cFSItem);
            }
        }

        private string FacilityACCaption(Facility facility)
        {
            return string.Format(@"[{0}] {1}", facility.FacilityNo, facility.FacilityName);
        }
        #endregion

        #endregion

    }
}
