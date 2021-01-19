// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-15-2012
// ***********************************************************************
// <copyright file="BSOBingMaps.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Maps.MapControl.WPF;
using System.Text.RegularExpressions;
using System.Windows;
using gip.mes.autocomponent;

namespace gip.bso.logistics
{
    /// <summary>
    /// BSOBingMaps
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Bing Maps'}de{'Bing Maps'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, false)]
    public class BSOBingMaps : ACBSOvb
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOBingMaps"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOBingMaps(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        /// <summary>
        /// ACs the post init.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        /// <summary>
        /// ACs the de init.
        /// </summary>
        /// <param name="deleteACClassTask">if set to <c>true</c> [delete AC class task].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._ContentPopupPosition = null;
            this._ContentPopupText = null;
            this._directions = null;
            this._from = null;
            this._routeResult = null;
            this._to = null;
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Implementation
        /// <summary>
        /// The _from
        /// </summary>
        private string _from = "Meridian, ID";
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        [ACPropertyInfo(501, "", "en{'From'}de{'Start-Adresse:'}")]
        public string From
        {
            get { return _from; }
            set
            {
                _from = value;
                OnPropertyChanged("From");
            }
        }

        /// <summary>
        /// The _to
        /// </summary>
        private string _to = "Boise, ID";
        /// <summary>
        /// Gets or sets to.
        /// </summary>
        /// <value>To.</value>
        [ACPropertyInfo(502, "", "en{'To'}de{'End-Adresse:'}")]
        public string To
        {
            get { return _to; }
            set
            {
                _to = value;
                OnPropertyChanged("To");
            }
        }

        /// <summary>
        /// The _route result
        /// </summary>
        private BingServices.RouteResult _routeResult;
        /// <summary>
        /// Gets or sets the route result.
        /// </summary>
        /// <value>The route result.</value>
        [ACPropertyInfo(503)]
        public BingServices.RouteResult RouteResult
        {
            get { return _routeResult; }
            set
            {
                _routeResult = value;
                OnPropertyChanged("RouteResult");
            }
        }


        /// <summary>
        /// The _directions
        /// </summary>
        private ObservableCollection<Direction> _directions;
        /// <summary>
        /// Gets or sets the directions.
        /// </summary>
        /// <value>The directions.</value>
        [ACPropertyInfo(504)]
        public ObservableCollection<Direction> Directions
        {
            get { return _directions; }
            set
            {
                _directions = value;
                OnPropertyChanged("Directions");
            }
        }


        /// <summary>
        /// Calculates the route.
        /// </summary>
        [ACMethodInfo("", "en{'Calculate route'}de{'Route berechnen'}", 501, false)]
        public void CalculateRoute()
        {
            var from = GeocodeAddress(From);
            var to = GeocodeAddress(To);

            CalculateRoute(from, to);
        }

        /// <summary>
        /// Geocodes the address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns>BingServices.GeocodeResult.</returns>
        public BingServices.GeocodeResult GeocodeAddress(string address)
        {
            BingServices.GeocodeResult result = null;

            using (BingServices.GeocodeServiceClient client = new BingServices.GeocodeServiceClient("CustomBinding_IGeocodeService"))
            {
                BingServices.GeocodeRequest request = new BingServices.GeocodeRequest();
                request.Credentials = new Credentials() { ApplicationId = "AlmItRpXQsVwvgEwCpoGdYGR6wQ-_76rj0XDlTKKptNoNmvQhlwTKkz1FxN_-Tnv" };
                request.Query = address;
                gip.bso.logistics.BingServices.GeocodeResponse geoCode = client.Geocode(request);
                var result2 = client.Geocode(request).Results;
                if (geoCode != null && result2.Any())
                    result = result2[0];
            }

            return result;
        }

        /// <summary>
        /// Calculates the route.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        private void CalculateRoute(BingServices.GeocodeResult from, BingServices.GeocodeResult to)
        {
            List<BingServices.GeocodeResult> waypoints = new List<BingServices.GeocodeResult>();
            waypoints.Add(from);
            waypoints.Add(to);
            CalculateRoute(waypoints);
        }

        /// <summary>
        /// Calculates the route.
        /// </summary>
        /// <param name="waypoints">The waypoints.</param>
        protected void CalculateRoute(IEnumerable<BingServices.GeocodeResult> waypoints)
        {
            if (waypoints.Count() <= 1)
                return;
            using (BingServices.RouteServiceClient client = new BingServices.RouteServiceClient("CustomBinding_IRouteService"))
            {
                BingServices.RouteRequest request = new BingServices.RouteRequest();
                request.Credentials = new Credentials() { ApplicationId = "AlmItRpXQsVwvgEwCpoGdYGR6wQ-_76rj0XDlTKKptNoNmvQhlwTKkz1FxN_-Tnv" };
                request.Waypoints = new ObservableCollection<BingServices.Waypoint>();
                foreach (var waypoint in waypoints)
                {
                    request.Waypoints.Add(ConvertResultToWayPoint(waypoint));
                }

                request.Options = new BingServices.RouteOptions();
                request.Options.RoutePathType = BingServices.RoutePathType.Points;
                request.Options.TrafficUsage = BingServices.TrafficUsage.TrafficBasedRouteAndTime;

                RouteResult = client.CalculateRoute(request).Result;
            }

            GetDirections();
        }

        /// <summary>
        /// Gets the directions.
        /// </summary>
        private void GetDirections()
        {
            Directions = new ObservableCollection<Direction>();

            foreach (BingServices.ItineraryItem item in RouteResult.Legs[0].Itinerary)
            {
                var direction = new Direction();
                direction.Description = GetDirectionText(item);
                direction.Location = new Location(item.Location.Latitude, item.Location.Longitude);
                Directions.Add(direction);
            }
        }

        /// <summary>
        /// Gets the direction text.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.String.</returns>
        private static string GetDirectionText(BingServices.ItineraryItem item)
        {
            string contentString = item.Text;
            //Remove tags from the string
            Regex regex = new Regex("<(.|\n)*?>");
            contentString = regex.Replace(contentString, string.Empty);
            //if (item.Warnings.Count > 0)
            //{
            //    foreach (BingServices.ItineraryItemWarning warning in item.Warnings)
            //    {
            //        if (warning.WarningType == BingServices.ItineraryWarningType.TrafficFlow)
            //        {
            //            contentString += "Stau: ";
            //        }
            //        contentString += warning.Text;
            //    }
            //}
            return contentString;
        }

        /// <summary>
        /// Converts the result to way point.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>BingServices.Waypoint.</returns>
        private BingServices.Waypoint ConvertResultToWayPoint(BingServices.GeocodeResult result)
        {
            BingServices.Waypoint waypoint = new BingServices.Waypoint();
            waypoint.Description = result.DisplayName;
            waypoint.Location = result.Locations[0];
            return waypoint;
        }


        /// <summary>
        /// The _ content popup text
        /// </summary>
        private string _ContentPopupText = "";
        /// <summary>
        /// Gets or sets the content popup text.
        /// </summary>
        /// <value>The content popup text.</value>
        [ACPropertyInfo(505)]
        public string ContentPopupText
        {
            get
            {
                return _ContentPopupText;
            }
            set
            {
                _ContentPopupText = value;
                OnPropertyChanged("ContentPopupText");
            }
        }

        /// <summary>
        /// The _ content popup visibility
        /// </summary>
        private Visibility _ContentPopupVisibility = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets the content popup visibility.
        /// </summary>
        /// <value>The content popup visibility.</value>
        [ACPropertyInfo(506)]
        public Visibility ContentPopupVisibility
        {
            get
            {
                return _ContentPopupVisibility;
            }
            set
            {
                _ContentPopupVisibility = value;
                OnPropertyChanged("ContentPopupVisibility");
            }
        }

        /// <summary>
        /// The _ content popup position
        /// </summary>
        private Location _ContentPopupPosition;
        /// <summary>
        /// Gets or sets the content popup position.
        /// </summary>
        /// <value>The content popup position.</value>
        [ACPropertyInfo(507)]
        public Location ContentPopupPosition
        {
            get
            {
                return _ContentPopupPosition;
            }
            set
            {
                _ContentPopupPosition = value;
                OnPropertyChanged("ContentPopupPosition");
            }
        }

        /// <summary>
        /// Route_s the mouse enter.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        [ACMethodInfo("", "en{'Route_MouseEnter'}de{'Route_MouseEnter'}", 502, false)]
        public void Route_MouseEnter(MouseEventArgs e)
        {
            FrameworkElement pin = e.Source as FrameworkElement;
            var location = (Direction)pin.Tag;
            ContentPopupText = location.Description;
            ContentPopupPosition = MapLayer.GetPosition(pin);
            ContentPopupVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Route_s the mouse leave.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        [ACMethodInfo("", "en{'Route_MouseLeave'}de{'Route_MouseLeave'}", 503, false)]
        public void Route_MouseLeave(MouseEventArgs e)
        {
            ContentPopupVisibility = Visibility.Collapsed;

            // TODO: Component doesn't know x:Names in XAML: Do with Binding or ACURLCommand
            //IACObject objectView = FindGui("VBDockingContainerTabbedDoc", "", "");
            //if (objectView != null)
            //{
            //    System.Windows.FrameworkElement ContentPopup = gip.core.layoutengine.Helperclasses.VBVisualTreeHelper.FindChildObjectInVisualTree((objectView as gip.core.layoutengine.VBDockingContainerTabbedDoc).VBDesignContent as System.Windows.DependencyObject, "ContentPopup") as System.Windows.FrameworkElement;
            //    if (ContentPopup != null)
            //    {
            //        ContentPopup.Visibility = System.Windows.Visibility.Collapsed;
            //    }
            //}
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "CalculateRoute":
                    CalculateRoute();
                    return true;
                case "Route_MouseEnter":
                    Route_MouseEnter((MouseEventArgs)acParameter[0]);
                    return true;
                case "Route_MouseLeave":
                    Route_MouseLeave((MouseEventArgs)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
