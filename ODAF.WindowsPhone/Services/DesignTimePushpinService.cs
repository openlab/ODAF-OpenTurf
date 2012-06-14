using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ODAF.WindowsPhone.Models;
using System.Collections.ObjectModel;
using System.Device.Location;

namespace ODAF.WindowsPhone.Services
{
    /// <summary>
    /// A dummy design-time pushpin service
    /// </summary>
    public static class DesignTimePushpinService
    {
        /// <summary>
        /// Load dummy pushpins at design-time
        /// </summary>
        /// <param name="layer">The layer to which the downloaded pushpins will be associated.</param>
        /// <param name="pushpins">The PushpinModel collection to be populated.</param>
        public static void LoadPushpinsAsync(LayerModel layer, Collection<PushpinModel> pushpins)
        {
            PushpinModel pushpin1 = new PushpinModel
            {
                Guid = Guid.NewGuid().ToString(),
                Description = "Design-Time Description",
                Layer = layer,
                Location = new GeoCoordinate(48.8925, 2.3449)
            };

            PushpinModel pushpin2 = new PushpinModel
            {
                Guid = Guid.NewGuid().ToString(),
                Description = "Design-Time Description",
                Layer = layer,
                Location = new GeoCoordinate(48.8875, 2.3882)
            };

            PushpinModel pushpin3 = new PushpinModel
            {
                Guid = Guid.NewGuid().ToString(),
                Description = "Design-Time Description",
                Layer = layer,
                Location = new GeoCoordinate(48.8766, 2.3555)
            };

            PushpinModel pushpin4 = new PushpinModel
            {
                Guid = Guid.NewGuid().ToString(),
                Description = "Design-Time Description",
                Layer = layer,
                Location = new GeoCoordinate(48.8641, 2.3977)
            };

            PushpinModel pushpin5 = new PushpinModel
            {
                Guid = Guid.NewGuid().ToString(),
                Description = "Design-Time Description",
                Layer = layer,
                Location = new GeoCoordinate(48.8327, 2.3261)
            };

            PushpinModel pushpin6 = new PushpinModel
            {
                Guid = Guid.NewGuid().ToString(),
                Description = "Design-Time Description",
                Layer = layer,
                Location = new GeoCoordinate(48.8166, 2.3605)
            };

            pushpins.Add(pushpin1);
            pushpins.Add(pushpin2);
            pushpins.Add(pushpin3);
            pushpins.Add(pushpin4);
            pushpins.Add(pushpin5);
            pushpins.Add(pushpin6);
        }
    }
}
