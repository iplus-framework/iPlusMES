// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="MapInteractivity.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/*
using System;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Media;
using gip.bso.logistics.BingServices;

namespace gip.bso.logistics
{
    /// <summary>
    /// Class MapInteractivity
    /// </summary>
    public class MapInteractivity
    {
        #region RouteResult

        /// <summary>
        /// The route result property
        /// </summary>
        public static readonly DependencyProperty RouteResultProperty = DependencyProperty.RegisterAttached("RouteResult", typeof(RouteResult), typeof(MapInteractivity), new UIPropertyMetadata(null, OnRouteResultChanged));
        /// <summary>
        /// Gets the route result.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>RouteResult.</returns>
        public static RouteResult GetRouteResult(DependencyObject target)
        {
            return (RouteResult)target.GetValue(RouteResultProperty);
        }
        /// <summary>
        /// Sets the route result.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetRouteResult(DependencyObject target, RouteResult value)
        {
            target.SetValue(RouteResultProperty, value);
        }

        /// <summary>
        /// Called when [route result changed].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRouteResultChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnRouteResultChanged((Map)o, (RouteResult)e.OldValue, (RouteResult)e.NewValue);
        }

        /// <summary>
        /// Called when [route result changed].
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void OnRouteResultChanged(Map map, RouteResult oldValue, RouteResult newValue)
        {
            MapPolyline routeLine = new MapPolyline();
            routeLine.Locations = new LocationCollection();
            routeLine.Opacity = 0.65;
            routeLine.Stroke = new SolidColorBrush(Colors.Blue);
            routeLine.StrokeThickness = 5.0;

            if (newValue == null)
                return;

            foreach (BingServices.Location loc in newValue.RoutePath.Points)
            {
                routeLine.Locations.Add(new Microsoft.Maps.MapControl.WPF.Location(loc.Latitude, loc.Longitude));
            }

            var routeLineLayer = GetRouteLineLayer(map);
            if (routeLineLayer == null)
            {
                routeLineLayer = new MapLayer();
                SetRouteLineLayer(map, routeLineLayer);
            }

            routeLineLayer.Children.Clear();
            routeLineLayer.Children.Add(routeLine);

            //Set the map view
            LocationRect rect = new LocationRect(routeLine.Locations[0], routeLine.Locations[routeLine.Locations.Count - 1]);
            if ((map.ActualHeight < 0.1) || (map.ActualWidth < 0.1))
                return;
            map.SetView(rect);
        }

        #endregion //RouteResult

        #region RouteLineLayer

        /// <summary>
        /// The route line layer property
        /// </summary>
        public static readonly DependencyProperty RouteLineLayerProperty = DependencyProperty.RegisterAttached("RouteLineLayer", typeof(MapLayer), typeof(MapInteractivity), new UIPropertyMetadata(null, OnRouteLineLayerChanged));
        /// <summary>
        /// Gets the route line layer.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>MapLayer.</returns>
        public static MapLayer GetRouteLineLayer(DependencyObject target)
        {
            return (MapLayer)target.GetValue(RouteLineLayerProperty);
        }
        /// <summary>
        /// Sets the route line layer.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetRouteLineLayer(DependencyObject target, MapLayer value)
        {
            target.SetValue(RouteLineLayerProperty, value);
        }
        /// <summary>
        /// Called when [route line layer changed].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRouteLineLayerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnRouteLineLayerChanged((Map)o, (MapLayer)e.OldValue, (MapLayer)e.NewValue);
        }
        /// <summary>
        /// Called when [route line layer changed].
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private static void OnRouteLineLayerChanged(Map map, MapLayer oldValue, MapLayer newValue)
        {
            if (newValue == null)
                return;
            if (!map.Children.Contains(newValue))
                map.Children.Add(newValue);
        }

        #endregion //RouteLineLayer
    }
}
*/