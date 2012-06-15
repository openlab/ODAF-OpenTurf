using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ODAF.WindowsPhone.Models;
using System.Device.Location;
using System.Globalization;
using System.Windows.Threading;
using System.Collections.Generic;
using System.ComponentModel;

namespace ODAF.WindowsPhone.Services
{
    /// <summary>
    /// This helper class proposes a method to download the pushpins associated with a layer.
    /// </summary>
    public static class PushpinService
    {
        /// <summary>
        /// Helper method to asynchronously load pushpins associated with the supplied layer and populate the pushpins collection supplied as a parameter.
        /// </summary>
        /// <param name="layer">The layer to which the downloaded pushpins will be associated.</param>
        /// <param name="pushpins">The PushpinModel collection to be populated.</param>
        public static void LoadPushpinsAsync(LayerModel layer, Collection<PushpinModel> pushpins)
        {
            LayerAndPushpin layerAndPushpin = new LayerAndPushpin
            {
                LayerModel = layer,
                PushpinModels = pushpins
            };

            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(LoadPushpinsAsync_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(layer.Link), layerAndPushpin);
        }

        private static void LoadPushpinsAsync_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            LayerAndPushpin layerAndPushpin = e.UserState as LayerAndPushpin;
            if (layerAndPushpin.LayerModel == null)
            {
                throw new ArgumentNullException("layer", "The layer parameter provided to the LoadPushpinsAsync method is null.");
            }
            if (layerAndPushpin.PushpinModels == null)
            {
                throw new ArgumentNullException("pushpins", "The pushpins parameter provided to the LoadPushpinsAsync method is null.");
            }
            if (e.Error != null)
            {
                throw e.Error;
            }

            XElement root = XElement.Parse(e.Result);
            XNamespace kml = "http://www.opengis.net/kml/2.2";

            var placemarks = from placemark in root.Descendants(kml + "Placemark")
                             select new
                             {
                                 Guid = placemark.Element(kml + "name").Value,
                                 Description = placemark.Element(kml + "description").Value,
                                 Latitude = double.Parse(placemark.Element(kml + "Point").Element(kml + "coordinates").Value.Split(',')[1], CultureInfo.InvariantCulture),
                                 Longitude = double.Parse(placemark.Element(kml + "Point").Element(kml + "coordinates").Value.Split(',')[0], CultureInfo.InvariantCulture)
                             };


            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (send, evt) => 
            {
                foreach (var placemark in placemarks)
                {
                    PushpinModel pushpin = new PushpinModel
                    {
                        Guid = placemark.Guid,
                        Description = placemark.Description,
                        Location = new GeoCoordinate(placemark.Latitude, placemark.Longitude),
                        Layer = layerAndPushpin.LayerModel
                    };

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        layerAndPushpin.PushpinModels.Add(pushpin);
                    });
                }
            };
            worker.RunWorkerAsync();           
        }

        /// <summary>
        /// An internal class that only serves to be passed as the user state in the DownloadStringAsync method.
        /// </summary>
        internal class LayerAndPushpin
        {
            public LayerModel LayerModel { get; set; }
            public Collection<PushpinModel> PushpinModels { get; set; }
        }
    }
}
