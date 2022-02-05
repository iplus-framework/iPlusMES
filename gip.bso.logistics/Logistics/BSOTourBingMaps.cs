// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-15-2012
// ***********************************************************************
// <copyright file="BSOTourBingMaps.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl.WPF;
using gip.mes.datamodel;
using gip.core.datamodel;
using System.Text;
using System.Runtime.CompilerServices;

namespace gip.bso.logistics
{
    /// <summary>
    /// Class MapTourPosInfo
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'MapTourPosInfo'}de{'MapTourPosInfo'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + "MapTourPosInfo", "en{'MapTourPosInfo'}de{'MapTourPosInfo'}", typeof(MapTourPosInfo), "MapTourPosInfo", "Description", "Description")]
    public class MapTourPosInfo : IACObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapTourPosInfo"/> class.
        /// </summary>
        /// <param name="inOrderPos">The in order pos.</param>
        public MapTourPosInfo(InOrderPos inOrderPos)
        {
            _InOrderPos = inOrderPos;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTourPosInfo"/> class.
        /// </summary>
        /// <param name="outOrderPos">The out order pos.</param>
        public MapTourPosInfo(OutOrderPos outOrderPos)
        {
            _OutOrderPos = outOrderPos;
        }

        /// <summary>
        /// The _ in order pos
        /// </summary>
        private InOrderPos _InOrderPos = null;
        /// <summary>
        /// Gets the in order pos.
        /// </summary>
        /// <value>The in order pos.</value>
        [ACPropertyInfo(9999)]
        public InOrderPos InOrderPos
        {
            get
            {
                return _InOrderPos;
            }
        }

        /// <summary>
        /// The _ out order pos
        /// </summary>
        private OutOrderPos _OutOrderPos = null;
        /// <summary>
        /// Gets the out order pos.
        /// </summary>
        /// <value>The out order pos.</value>
        [ACPropertyInfo(9999)]
        public OutOrderPos OutOrderPos
        {
            get
            {
                return _OutOrderPos;
            }
        }

        /// <summary>
        /// Gets the material.
        /// </summary>
        /// <value>The material.</value>
        [ACPropertyInfo(1, "", "en{'Material'}de{'Material'}")]
        public Material Material
        {
            get
            {
                if (InOrderPos != null)
                    return InOrderPos.Material;
                if (OutOrderPos != null)
                    return OutOrderPos.Material;
                return null;
            }
        }

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        [ACPropertyInfo(2, "", "en{'Quantity'}de{'Menge'}")]
        public double Quantity
        {
            get
            {
                if (InOrderPos != null)
                    return InOrderPos.TargetQuantity;
                if (OutOrderPos != null)
                    return OutOrderPos.TargetQuantity;
                return 0;
            }
        }

        /// <summary>
        /// Gets the quantity unit.
        /// </summary>
        /// <value>The quantity unit.</value>
        [ACPropertyInfo(3, "", "en{'Quantityunit'}de{'Mengeneinheit'}")]
        public string QuantityUnit
        {
            get
            {
                //if (InOrderPos != null && InOrderPos.MaterialUnit != null)
                //    return InOrderPos.MaterialUnit.ACCaption;
                //if (OutOrderPos != null && OutOrderPos.MaterialUnit != null)
                //    return OutOrderPos.MaterialUnit.ACCaption;
                return "";
            }
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>The address.</value>
        [ACPropertyInfo(5, "", "en{'Address'}de{'Adresse'}")]
        public CompanyAddress Address
        {
            get
            {
                if (InOrderPos != null)
                    return InOrderPos.InOrder.DeliveryCompanyAddress;
                if (OutOrderPos != null)
                    return OutOrderPos.OutOrder.DeliveryCompanyAddress;
                return null;
            }
        }

        /// <summary>
        /// Gets the delivery date.
        /// </summary>
        /// <value>The delivery date.</value>
        [ACPropertyInfo(4, "", "en{'Delivery date'}de{'Lieferdatum'}")]
        public DateTime DeliveryDate
        {
            get
            {
                if (InOrderPos != null)
                    return InOrderPos.TargetDeliveryDate;
                if (OutOrderPos != null)
                    return OutOrderPos.TargetDeliveryDate;
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        [ACPropertyInfo(6)]
        public string Description
        {
            get
            {
                if (Material == null || Address == null)
                    return String.Format("Error: {0} {1}, {2}", Quantity, QuantityUnit, DeliveryDate);
                return String.Format("{0}, {1} {2}, {3}, {4}", Material.ACCaption, Quantity, QuantityUnit, DeliveryDate, Address.Name1);
            }
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public string ID
        {
            get
            {
                if (InOrderPos != null)
                    return InOrderPos.InOrderPosID.ToString();
                if (OutOrderPos != null)
                    return OutOrderPos.OutOrderPosID.ToString();
                return "";
            }
        }

        public override string ToString()
        {
            return Description;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        public string ACCaption
        {
            get;
        }

        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType
        {
            get;
        }

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList
        {
            get { return null; }
        }

        /// <summary>
        /// The ACUrlCommand is a universal method that can be used to query the existence of an instance via a string (ACUrl) to:
        /// 1. get references to components,
        /// 2. query property values,
        /// 3. execute method calls,
        /// 4. start and stop Components,
        /// 5. and send messages to other components.
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>Result if a property was accessed or a method was invoked. Void-Methods returns null.</returns>
        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        public string GetACUrl(IACObject rootACObject = null)
        {
            return ACIdentifier;
        }


        /// <summary>
        /// Method that returns a source and path for WPF-Bindings by passing a ACUrl.
        /// </summary>
        /// <param name="acUrl">ACUrl of the Component, Property or Method</param>
        /// <param name="acTypeInfo">Reference to the iPlus-Type (ACClass)</param>
        /// <param name="source">The Source for WPF-Databinding</param>
        /// <param name="path">Relative path from the returned source for WPF-Databinding</param>
        /// <param name="rightControlMode">Information about access rights for the requested object</param>
        /// <returns><c>true</c> if binding could resolved for the passed ACUrl<c>false</c> otherwise</returns>
        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return false;
        }

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        public string ACIdentifier
        {
            get;
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return false;
        }
    }


    /// <summary>
    /// Class MapTourGeocodeInfo
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'MapTourGeocodeInfo'}de{'MapTourGeocodeInfo'}", Global.ACKinds.TACClass)]
    public class MapTourGeocodeInfo : INotifyPropertyChanged, IACObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapTourGeocodeInfo"/> class.
        /// </summary>
        /// <param name="geocodeResult">The geocode result.</param>
        public MapTourGeocodeInfo(BingServices.GeocodeResult geocodeResult)
        {
            _GeocodeResult = geocodeResult;
        }

        /// <summary>
        /// The _ geocode result
        /// </summary>
        private BingServices.GeocodeResult _GeocodeResult = null;
        /// <summary>
        /// Gets the geocode result.
        /// </summary>
        /// <value>The geocode result.</value>
        public BingServices.GeocodeResult GeocodeResult
        {
            get
            {
                return _GeocodeResult;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (MapTourPosInfo info in InOrderPosInfo)
                {
                    builder.AppendLine(info.Description);
                }
                foreach (MapTourPosInfo info in OutOrderPosInfo)
                {
                    builder.AppendLine(info.Description);
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// The _ location
        /// </summary>
        private Location _Location = null;
        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Location Location
        {
            get
            {
                if (_Location != null)
                    return _Location;
                if (GeocodeResult == null)
                    return null;
                if (GeocodeResult.Locations.Count <= 0)
                    return null;
                _Location = new Location(GeocodeResult.Locations.First().Latitude, GeocodeResult.Locations.First().Longitude);
                return _Location;
            }
        }

        /// <summary>
        /// Gets the count positions.
        /// </summary>
        /// <value>The count positions.</value>
        public int CountPositions
        {
            get
            {
                return InOrderPosInfo.Count + OutOrderPosInfo.Count;
            }
        }

        /// <summary>
        /// Adds the in order pos.
        /// </summary>
        /// <param name="inOrderPos">The in order pos.</param>
        public void AddInOrderPos(InOrderPos inOrderPos)
        {
            InOrderPosInfo.Add(new MapTourPosInfo(inOrderPos));
            OnPropertyChanged("CountPositions");
            OnPropertyChanged("Description");
        }

        /// <summary>
        /// The _ in order pos info
        /// </summary>
        private List<MapTourPosInfo> _InOrderPosInfo = new List<MapTourPosInfo>();
        /// <summary>
        /// Gets the in order pos info.
        /// </summary>
        /// <value>The in order pos info.</value>
        public List<MapTourPosInfo> InOrderPosInfo
        {
            get
            {
                return _InOrderPosInfo;
            }
        }

        /// <summary>
        /// Adds the out order pos.
        /// </summary>
        /// <param name="outOrderPos">The out order pos.</param>
        public void AddOutOrderPos(OutOrderPos outOrderPos)
        {
            OutOrderPosInfo.Add(new MapTourPosInfo(outOrderPos));
            OnPropertyChanged("CountPositions");
            OnPropertyChanged("Description");
        }

        /// <summary>
        /// The _ out order pos info
        /// </summary>
        private List<MapTourPosInfo> _OutOrderPosInfo = new List<MapTourPosInfo>();
        /// <summary>
        /// Gets the out order pos info.
        /// </summary>
        /// <value>The out order pos info.</value>
        public List<MapTourPosInfo> OutOrderPosInfo
        {
            get
            {
                return _OutOrderPosInfo;
            }
        }


        /// <summary>
        /// Tritt ein, wenn sich ein Eigenschaftswert ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        public string ACCaption
        {
            get { return Description; }
        }

        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType
        {
            get { return null; }
        }

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList
        {
            get
            {
                if (InOrderPosInfo.Count > 0)
                    return InOrderPosInfo;
                if (OutOrderPosInfo.Count > 0)
                    return OutOrderPosInfo;
                return null;
            }
        }

        /// <summary>
        /// The ACUrlCommand is a universal method that can be used to query the existence of an instance via a string (ACUrl) to:
        /// 1. get references to components,
        /// 2. query property values,
        /// 3. execute method calls,
        /// 4. start and stop Components,
        /// 5. and send messages to other components.
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>Result if a property was accessed or a method was invoked. Void-Methods returns null.</returns>
        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return null;
        }

        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        public string GetACUrl(IACObject rootACObject = null)
        {
            return ACIdentifier;
        }


        /// <summary>
        /// Method that returns a source and path for WPF-Bindings by passing a ACUrl.
        /// </summary>
        /// <param name="acUrl">ACUrl of the Component, Property or Method</param>
        /// <param name="acTypeInfo">Reference to the iPlus-Type (ACClass)</param>
        /// <param name="source">The Source for WPF-Databinding</param>
        /// <param name="path">Relative path from the returned source for WPF-Databinding</param>
        /// <param name="rightControlMode">Information about access rights for the requested object</param>
        /// <returns><c>true</c> if binding could resolved for the passed ACUrl<c>false</c> otherwise</returns>
        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return false;
        }

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        public string ACIdentifier
        {
            get;
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return false;
        }
    }



    /// <summary>
    /// BSOBingMaps
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Bing Maps Tour'}de{'Bing Maps Tour'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, false, false)]
    public class BSOTourBingMaps : BSOBingMaps
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOBingMaps" /> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOTourBingMaps(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        /// <summary>
        /// ACs the post init.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACPostInit()
        {
            if (ParentACComponent != null && ParentACComponent is BSOTourplan)
            {
                ParentACComponent.PropertyChanged += ParentACComponent_PropertyChanged;
            }
            return base.ACPostInit();
        }

        /// <summary>
        /// ACs the de init.
        /// </summary>
        /// <param name="deleteACClassTask">if set to <c>true</c> [delete AC class task].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (ParentACComponent != null && ParentACComponent is BSOTourplan)
            {
                ParentACComponent.PropertyChanged -= ParentACComponent_PropertyChanged;
            }
            this._CurrentGeocodeInfo = null;
            this._CurrentTourPinPopupPos = null;
            this._SelectedTourPinPopupPos = null;
            this._DictGeoAddress = null;
            this._DirectionsInOrder = null;
            this._DirectionsInOrderTour = null;
            this._DirectionsOutOrder = null;
            this._DirectionsOutOrderTour = null;
            this._OwnCompanyPos = null;
            this._TourPinPopupList = null;
            this._TourPinPopupPosition = null;
            this._TourPinPopupText = null;
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Implementation
        /// <summary>
        /// The _ dict geo address
        /// </summary>
        private Dictionary<CompanyAddress, BingServices.GeocodeResult> _DictGeoAddress = new Dictionary<CompanyAddress, BingServices.GeocodeResult>();
        /// <summary>
        /// Gets the dict geo address.
        /// </summary>
        /// <value>The dict geo address.</value>
        protected Dictionary<CompanyAddress, BingServices.GeocodeResult> DictGeoAddress
        {
            get
            {
                return _DictGeoAddress;
            }
        }

        /// <summary>
        /// The _ directions in order
        /// </summary>
        private ObservableCollection<MapTourGeocodeInfo> _DirectionsInOrder = new ObservableCollection<MapTourGeocodeInfo>();
        /// <summary>
        /// Gets or sets the directions in order.
        /// </summary>
        /// <value>The directions in order.</value>
        [ACPropertyInfo(9999)]
        public ObservableCollection<MapTourGeocodeInfo> DirectionsInOrder
        {
            get { return _DirectionsInOrder; }
            set
            {
                _DirectionsInOrder = value;
                OnPropertyChanged("DirectionsInOrder");
            }
        }

        /// <summary>
        /// The _ directions out order
        /// </summary>
        private ObservableCollection<MapTourGeocodeInfo> _DirectionsOutOrder = new ObservableCollection<MapTourGeocodeInfo>();
        /// <summary>
        /// Gets or sets the directions out order.
        /// </summary>
        /// <value>The directions out order.</value>
        [ACPropertyInfo(9999)]
        public ObservableCollection<MapTourGeocodeInfo> DirectionsOutOrder
        {
            get { return _DirectionsOutOrder; }
            set
            {
                _DirectionsOutOrder = value;
                OnPropertyChanged("DirectionsOutOrder");
            }
        }

        /// <summary>
        /// The _ directions in order tour
        /// </summary>
        private ObservableCollection<MapTourGeocodeInfo> _DirectionsInOrderTour = new ObservableCollection<MapTourGeocodeInfo>();
        /// <summary>
        /// Gets or sets the directions in order tour.
        /// </summary>
        /// <value>The directions in order tour.</value>
        [ACPropertyInfo(9999)]
        public ObservableCollection<MapTourGeocodeInfo> DirectionsInOrderTour
        {
            get { return _DirectionsInOrderTour; }
            set
            {
                _DirectionsInOrderTour = value;
                OnPropertyChanged("DirectionsInOrderTour");
            }
        }

        /// <summary>
        /// The _ directions out order tour
        /// </summary>
        private ObservableCollection<MapTourGeocodeInfo> _DirectionsOutOrderTour = new ObservableCollection<MapTourGeocodeInfo>();
        /// <summary>
        /// Gets or sets the directions out order tour.
        /// </summary>
        /// <value>The directions out order tour.</value>
        [ACPropertyInfo(9999)]
        public ObservableCollection<MapTourGeocodeInfo> DirectionsOutOrderTour
        {
            get { return _DirectionsOutOrderTour; }
            set
            {
                _DirectionsOutOrderTour = value;
                OnPropertyChanged("DirectionsOutOrderTour");
            }
        }

        /// <summary>
        /// The _ own company pos
        /// </summary>
        private BingServices.GeocodeResult _OwnCompanyPos = null;
        /// <summary>
        /// Gets the own company pos.
        /// </summary>
        /// <value>The own company pos.</value>
        public BingServices.GeocodeResult OwnCompanyPos
        {
            get
            {
                if (_OwnCompanyPos == null)
                {
                    String address = "";
                    var query = DatabaseApp.Company.Where(c => c.IsOwnCompany);
                    if (query.Any())
                    {
                        Company company = query.First();
                        if (company.HouseCompanyAddress != null)
                            address = String.Format("{0}, {1} {2}", company.HouseCompanyAddress.Street, company.HouseCompanyAddress.Postcode, company.HouseCompanyAddress.City);
                    }
                    if (address == "")
                        address = "Gewerbepark Hardtwald 9, 68723 Oftersheim";
                    _OwnCompanyPos = GeocodeAddress(address);
                }
                return _OwnCompanyPos;
            }
        }

        /// <summary>
        /// Gets the BSO tourplan.
        /// </summary>
        /// <value>The BSO tourplan.</value>
        public BSOTourplan BSOTourplan
        {
            get
            {
                return ParentACComponent as BSOTourplan;
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the ParentACComponent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        void ParentACComponent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InOrderPosList")
            {
                DirectionsInOrder.Clear();
                if (BSOTourplan.InOrderPosList != null)
                {
                    foreach (InOrderPos inOrderPos in BSOTourplan.InOrderPosList)
                    {
                        if (inOrderPos.InOrder.DeliveryCompanyAddress == null)
                            continue;
                        BingServices.GeocodeResult geocode = null;
                        if (!DictGeoAddress.TryGetValue(inOrderPos.InOrder.DeliveryCompanyAddress, out geocode))
                        {
                            String address = String.Format("{0}, {1} {2}", inOrderPos.InOrder.DeliveryCompanyAddress.Street, inOrderPos.InOrder.DeliveryCompanyAddress.Postcode, inOrderPos.InOrder.DeliveryCompanyAddress.City);
                            geocode = GeocodeAddress(address);
                            if (geocode != null)
                            {
                                DictGeoAddress.Add(inOrderPos.InOrder.DeliveryCompanyAddress, geocode);
                            }
                        }
                        if (geocode != null && geocode.Locations.Count > 0)
                        {
                            MapTourGeocodeInfo direction = null;
                            var query = DirectionsInOrder.Where(c => c.GeocodeResult == geocode);
                            int count = query.Count();
                            if (count <= 0)
                            {
                                direction = new MapTourGeocodeInfo(geocode);
                            }
                            else
                                direction = query.First();
                            direction.AddInOrderPos(inOrderPos);

                            if (count <= 0)
                                DirectionsInOrder.Add(direction);

                            //direction.Description = String.Format("{0}, {1}, {2}, {3}", inOrderPos.InOrder.InOrderNo, inOrderPos.Material.MaterialName1, inOrderPos.TargetQuantity, inOrderPos.InOrder.DeliveryCompanyAddress.Name1);
                        }
                    }
                }
            }
            else if (e.PropertyName == "InOrderTourPosList")
            {
                DirectionsInOrderTour.Clear();
                if (BSOTourplan.InOrderTourPosList != null)
                {
                    foreach (InOrderPos inOrderPos in BSOTourplan.InOrderTourPosList)
                    {
                        if (inOrderPos.InOrder.DeliveryCompanyAddress == null)
                            continue;
                        BingServices.GeocodeResult geocode = null;
                        if (!DictGeoAddress.TryGetValue(inOrderPos.InOrder.DeliveryCompanyAddress, out geocode))
                        {
                            String address = String.Format("{0}, {1} {2}", inOrderPos.InOrder.DeliveryCompanyAddress.Street, inOrderPos.InOrder.DeliveryCompanyAddress.Postcode, inOrderPos.InOrder.DeliveryCompanyAddress.City);
                            geocode = GeocodeAddress(address);
                            if (geocode != null)
                            {
                                DictGeoAddress.Add(inOrderPos.InOrder.DeliveryCompanyAddress, geocode);
                            }
                        }
                        if (geocode != null && geocode.Locations.Count > 0)
                        {
                            MapTourGeocodeInfo direction = null;
                            var query = DirectionsInOrderTour.Where(c => c.GeocodeResult == geocode);
                            int count = query.Count();
                            if (count <= 0)
                            {
                                direction = new MapTourGeocodeInfo(geocode);
                            }
                            else
                                direction = query.First();
                            direction.AddInOrderPos(inOrderPos);

                            if (count <= 0)
                                DirectionsInOrderTour.Add(direction);

                            //direction.Description = String.Format("{0}, {1}, {2}, {3}", inOrderPos.InOrder.InOrderNo, inOrderPos.Material.MaterialName1, inOrderPos.TargetQuantity, inOrderPos.InOrder.DeliveryCompanyAddress.Name1);
                        }
                    }
                }
                if (AutoCalculateRoute)
                    UpdateRoute();
                else
                {
                    if (Directions != null)
                        Directions.Clear();
                }
            }
            else if (e.PropertyName == "OutOrderPosList")
            {
                DirectionsOutOrder.Clear();
                if (BSOTourplan.OutOrderPosList != null)
                {
                    foreach (OutOrderPos outOrderPos in BSOTourplan.OutOrderPosList)
                    {
                        if (outOrderPos.OutOrder.DeliveryCompanyAddress == null)
                            continue;
                        BingServices.GeocodeResult geocode = null;
                        if (!DictGeoAddress.TryGetValue(outOrderPos.OutOrder.DeliveryCompanyAddress, out geocode))
                        {
                            String address = String.Format("{0}, {1} {2}", outOrderPos.OutOrder.DeliveryCompanyAddress.Street, outOrderPos.OutOrder.DeliveryCompanyAddress.Postcode, outOrderPos.OutOrder.DeliveryCompanyAddress.City);
                            geocode = GeocodeAddress(address);
                            if (geocode != null)
                            {
                                DictGeoAddress.Add(outOrderPos.OutOrder.DeliveryCompanyAddress, geocode);
                            }
                        }
                        if (geocode != null && geocode.Locations.Count > 0)
                        {
                            MapTourGeocodeInfo direction = null;
                            var query = DirectionsOutOrder.Where(c => c.GeocodeResult == geocode);
                            int count = query.Count();
                            if (count <= 0)
                            {
                                direction = new MapTourGeocodeInfo(geocode);
                            }
                            else
                                direction = query.First();
                            direction.AddOutOrderPos(outOrderPos);

                            if (count <= 0)
                                DirectionsOutOrder.Add(direction);
                            //direction.Description = String.Format("{0}, {1}, {2}, {3}", outOrderPos.OutOrder.OutOrderNo, outOrderPos.Material.MaterialName1, outOrderPos.TargetQuantity, outOrderPos.OutOrder.DeliveryCompanyAddress.Name1);
                        }
                    }
                }
            }
            else if (e.PropertyName == "OutOrderTourPosList")
            {
                DirectionsOutOrderTour.Clear();
                if (BSOTourplan.OutOrderTourPosList != null)
                {
                    foreach (OutOrderPos outOrderPos in BSOTourplan.OutOrderTourPosList)
                    {
                        if (outOrderPos.OutOrder.DeliveryCompanyAddress == null)
                            continue;
                        BingServices.GeocodeResult geocode = null;
                        if (!DictGeoAddress.TryGetValue(outOrderPos.OutOrder.DeliveryCompanyAddress, out geocode))
                        {
                            String address = String.Format("{0}, {1} {2}", outOrderPos.OutOrder.DeliveryCompanyAddress.Street, outOrderPos.OutOrder.DeliveryCompanyAddress.Postcode, outOrderPos.OutOrder.DeliveryCompanyAddress.City);
                            geocode = GeocodeAddress(address);
                            if (geocode != null)
                            {
                                DictGeoAddress.Add(outOrderPos.OutOrder.DeliveryCompanyAddress, geocode);
                            }
                        }
                        if (geocode != null && geocode.Locations.Count > 0)
                        {
                            MapTourGeocodeInfo direction = null;
                            var query = DirectionsOutOrderTour.Where(c => c.GeocodeResult == geocode);
                            int count = query.Count();
                            if (count <= 0)
                            {
                                direction = new MapTourGeocodeInfo(geocode);
                            }
                            else
                                direction = query.First();
                            direction.AddOutOrderPos(outOrderPos);

                            if (count <= 0)
                                DirectionsOutOrderTour.Add(direction);

                            //direction.Description = String.Format("{0}, {1}, {2}, {3}", outOrderPos.OutOrder.OutOrderNo, outOrderPos.Material.MaterialName1, outOrderPos.TargetQuantity, outOrderPos.OutOrder.DeliveryCompanyAddress.Name1);
                        }
                    }
                    if (AutoCalculateRoute)
                        UpdateRoute();
                    else
                    {
                        if (Directions != null)
                            Directions.Clear();
                    }

                }
            }
        }

        /// <summary>
        /// Gets or sets the auto calculate route.
        /// </summary>
        /// <value>The auto calculate route.</value>
        [ACPropertyInfo(9999, "", "en{'Auto Route Calculation'}de{'Autom. Routenberechnung'}")]
        public Boolean AutoCalculateRoute
        {
            get;
            set;
        }


        /// <summary>
        /// Updates the route.
        /// </summary>
        [ACMethodInfo("", "en{'Calculate Route'}de{'Tour-Route berechnen'}", 9999, false)]
        public void UpdateRoute()
        {
            List<BingServices.GeocodeResult> waypoints = new List<BingServices.GeocodeResult>();
            if (OwnCompanyPos != null)
                waypoints.Add(OwnCompanyPos);
            foreach (MapTourGeocodeInfo info in DirectionsOutOrderTour)
            {
                var query = waypoints.Where(c => c == info.GeocodeResult);
                if (!query.Any())
                    waypoints.Add(info.GeocodeResult);
            }
            foreach (MapTourGeocodeInfo info in DirectionsInOrderTour)
            {
                var query = waypoints.Where(c => c == info.GeocodeResult);
                if (!query.Any())
                    waypoints.Add(info.GeocodeResult);
            }
            if (waypoints.Count <= 1)
            {
                if (Directions != null)
                    Directions.Clear();
            }
            else
            {
                CalculateRoute(waypoints);
            }
        }

        #endregion

        #region MouseEvents
        /// <summary>
        /// The _ tour pin popup text
        /// </summary>
        private string _TourPinPopupText = "";
        /// <summary>
        /// Gets or sets the tour pin popup text.
        /// </summary>
        /// <value>The tour pin popup text.</value>
        [ACPropertyInfo(9999)]
        public string TourPinPopupText
        {
            get
            {
                return _TourPinPopupText;
            }
            set
            {
                _TourPinPopupText = value;
                OnPropertyChanged("TourPinPopupText");
            }
        }


        /// <summary>
        /// The _ tour pin popup list
        /// </summary>
        ObservableCollection<MapTourPosInfo> _TourPinPopupList = new ObservableCollection<MapTourPosInfo>();
        /// <summary>
        /// Gets the tour pin popup list.
        /// </summary>
        /// <value>The tour pin popup list.</value>
        [ACPropertyList(9999, "TourPinPopup")]
        public ObservableCollection<MapTourPosInfo> TourPinPopupList
        {
            get
            {
                return _TourPinPopupList;
            }
        }

        /// <summary>
        /// The _ current tour pin popup pos
        /// </summary>
        MapTourPosInfo _CurrentTourPinPopupPos;
        /// <summary>
        /// Gets or sets the current tour pin popup pos.
        /// </summary>
        /// <value>The current tour pin popup pos.</value>
        [ACPropertyCurrent(9999, "TourPinPopup")]
        public MapTourPosInfo CurrentTourPinPopupPos
        {
            get
            {
                return _CurrentTourPinPopupPos;
            }
            set
            {
                _CurrentTourPinPopupPos = value;
                OnPropertyChanged("CurrentTourPinPopupPos");
            }
        }

        /// <summary>
        /// The _ selected tour pin popup pos
        /// </summary>
        MapTourPosInfo _SelectedTourPinPopupPos;
        /// <summary>
        /// Gets or sets the selected tour pin popup pos.
        /// </summary>
        /// <value>The selected tour pin popup pos.</value>
        [ACPropertySelected(9999, "TourPinPopup")]
        public MapTourPosInfo SelectedTourPinPopupPos
        {
            get
            {
                return _SelectedTourPinPopupPos;
            }
            set
            {
                _SelectedTourPinPopupPos = value;
                OnPropertyChanged("SelectedTourPinPopupPos");
            }
        }


        /// <summary>
        /// The _ tour pin popup visibility
        /// </summary>
        private Visibility _TourPinPopupVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the tour pin popup visibility.
        /// </summary>
        /// <value>The tour pin popup visibility.</value>
        [ACPropertyInfo(9999)]
        public Visibility TourPinPopupVisibility
        {
            get
            {
                return _TourPinPopupVisibility;
            }
            set
            {
                _TourPinPopupVisibility = value;
                OnPropertyChanged("TourPinPopupVisibility");
            }
        }

        /// <summary>
        /// The _ tour pin popup position
        /// </summary>
        private Location _TourPinPopupPosition;
        /// <summary>
        /// Gets or sets the tour pin popup position.
        /// </summary>
        /// <value>The tour pin popup position.</value>
        [ACPropertyInfo(9999)]
        public Location TourPinPopupPosition
        {
            get
            {
                return _TourPinPopupPosition;
            }
            set
            {
                _TourPinPopupPosition = value;
                OnPropertyChanged("TourPinPopupPosition");
            }
        }


        /// <summary>
        /// Tours the pin_ mouse enter.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        [ACMethodInfo("", "en{'TourPin_MouseEnter'}de{'TourPin_MouseEnter'}", 9999, false)]
        public void TourPin_MouseEnter(MouseEventArgs e)
        {
            // TODO: Component doesn't know x:Names in XAML: Do with Binding or ACURLCommand
            FrameworkElement pin = e.Source as FrameworkElement;
            var location = (MapTourGeocodeInfo)pin.Tag;
            TourPinPopupText = location.Description;
            _TourPinPopupList.Clear();
            if (location.InOrderPosInfo != null && location.InOrderPosInfo.Count > 0)
            {
                foreach (MapTourPosInfo pos in location.InOrderPosInfo)
                {
                    TourPinPopupList.Add(pos);
                }
            }
            else if (location.OutOrderPosInfo != null && location.OutOrderPosInfo.Count > 0)
            {
                foreach (MapTourPosInfo pos in location.OutOrderPosInfo)
                {
                    TourPinPopupList.Add(pos);
                }
            }

            TourPinPopupPosition = MapLayer.GetPosition(pin);
            TourPinPopupVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Tours the pin_ mouse leave.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        [ACMethodInfo("", "en{'TourPin_MouseLeave'}de{'TourPin_MouseLeave'}", 9999, false)]
        public void TourPin_MouseLeave(MouseEventArgs e)
        {
            TourPinPopupVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Tours the pin_ mouse right button down.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        [ACMethodInfo("", "en{'TourPin_MouseRightButtonDown'}de{'TourPin_MouseRightButtonDown'}", 9999, false)]
        public void TourPin_MouseRightButtonDown(MouseEventArgs e)
        {
        }


        /// <summary>
        /// The _ current geocode info
        /// </summary>
        protected MapTourGeocodeInfo _CurrentGeocodeInfo = null;

        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if (actionArgs.DropObject != null && actionArgs.ElementAction == Global.ElementActionType.ContextMenu)
            {
                _CurrentGeocodeInfo = null;
                if (actionArgs.DropObject.ACContentList != null)
                {
                    if (actionArgs.DropObject.ACContentList.Any())
                    {
                        _CurrentGeocodeInfo = actionArgs.DropObject.ACContentList.First() as MapTourGeocodeInfo;
                    }
                }
            }
            if (actionArgs.ElementAction == Global.ElementActionType.ACCommand)
            {
                int i = 0;
                i++;
            }
            base.ACAction(actionArgs);
        }

        /// <summary>
        /// Gets the menu.
        /// </summary>
        /// <param name="vbContent">Content of the vb.</param>
        /// <param name="vbControl">The vb control.</param>
        /// <returns>ACMenuItemList.</returns>
        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList acMenuItemList = new ACMenuItemList();
            if (_CurrentGeocodeInfo != null)
            {
                foreach (MapTourPosInfo posInfo in _CurrentGeocodeInfo.InOrderPosInfo)
                {
                    ACValueList acParameterList = new ACValueList();
                    acParameterList.Add(new ACValue("posInfo.ID", posInfo.ID));
                    acMenuItemList.Add(new ACMenuItem(null, posInfo.Description, "!OnPinContextMenuClicked", 0, acParameterList, true, this));
                }
                foreach (MapTourPosInfo posInfo in _CurrentGeocodeInfo.OutOrderPosInfo)
                {
                    ACValueList acParameterList = new ACValueList();
                    acParameterList.Add(new ACValue("posInfo.ID", posInfo.ID));
                    acMenuItemList.Add(new ACMenuItem(null, posInfo.Description, "!OnPinContextMenuClicked", 0, acParameterList, true, this));
                }
            }
            return acMenuItemList;
        }

        /// <summary>
        /// Called when [pin context menu clicked].
        /// </summary>
        /// <param name="id">The id.</param>
        [ACMethodInfo("", "en{'OnPinContextMenuClicked'}de{'OnPinContextMenuClicked'}", 9999, false)]
        public void OnPinContextMenuClicked(string id)
        {
            //string id = parameter["posInfo.ID"] as String;

            if (_CurrentGeocodeInfo != null)
            {
                MapTourPosInfo posInfo = null;
                if (_CurrentGeocodeInfo.InOrderPosInfo.Count > 0)
                {
                    var query = _CurrentGeocodeInfo.InOrderPosInfo.Where(c => c.ID == id);
                    if (query.Any())
                    {
                        posInfo = query.First();
                    }
                }
                if (_CurrentGeocodeInfo.OutOrderPosInfo.Count > 0)
                {
                    var query = _CurrentGeocodeInfo.OutOrderPosInfo.Where(c => c.ID == id);
                    if (query.Any())
                    {
                        posInfo = query.First();
                    }
                }
                if (posInfo != null && BSOTourplan != null)
                {
                    if (posInfo.OutOrderPos != null)
                    {
                        if (posInfo.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null)
                        //if (posInfo.OutOrderPos.MDDelivPosState.MDDelivPosStateIndex != (short)MDDelivPosState.DelivPosStates.NotPlanned)
                        {
                            BSOTourplan.SelectedOutOrderTourPos = posInfo.OutOrderPos;
                            BSOTourplan.UnassignOutOrderPos();
                        }
                        else
                        {
                            BSOTourplan.SelectedOutOrderPos = posInfo.OutOrderPos;
                            BSOTourplan.AssignOutOrderPos();
                        }
                    }
                    else if (posInfo.InOrderPos != null)
                    {
                        if (posInfo.InOrderPos.InOrderPos1_ParentInOrderPos != null)
                        //if (posInfo.InOrderPos.MDDelivPosState.MDDelivPosStateIndex != (short)MDDelivPosState.DelivPosStates.NotPlanned)
                        {
                            BSOTourplan.SelectedInOrderTourPos = posInfo.InOrderPos;
                            BSOTourplan.UnassignInOrderPos();
                        }
                        else
                        {
                            BSOTourplan.SelectedInOrderPos = posInfo.InOrderPos;
                            BSOTourplan.AssignInOrderPos();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
