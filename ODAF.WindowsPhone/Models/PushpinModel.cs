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
using System.ComponentModel;
using System.Device.Location;
using System.Collections.ObjectModel;

namespace ODAF.WindowsPhone.Models
{
    /// <summary>
    /// Represents a pushpin on a map with associated data.
    /// </summary>
    public class PushpinModel : INotifyPropertyChanged
    {
        private string _Guid;
        /// <summary>
        /// Guid read from KML streams exposed by OGDI. Corresponds to the name element of each placemark in the XML tree.
        /// </summary>
        public string Guid
        {
            get { return _Guid; }
            set
            {
                if (value != _Guid)
                {
                    _Guid = value;
                    NotifyPropertyChanged("Guid");
                }
            }
        }

        private string _Description;
        /// <summary>
        /// Description read from KML streams exposed by OGDI. Corresponds to the description element of each placemark in the XML tree.
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set
            {
                if (value != _Description)
                {
                    _Description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        private GeoCoordinate _Location;
        /// <summary>
        /// Location (latitude and longitude) of the pushpin.
        /// </summary>
        public GeoCoordinate Location
        {
            get { return _Location; }
            set
            {
                if (value != _Location)
                {
                    _Location = value;
                    NotifyPropertyChanged("Location");
                }
            }
        }

        private LayerModel _Layer;
        /// <summary>
        /// The layer to which the pushpin belongs. Layers are exposed in the PointSources.xml file at the root of ODAF website.
        /// </summary>
        public LayerModel Layer
        {
            get { return _Layer; }
            set
            {
                if (value != _Layer)
                {
                    _Layer = value;
                    NotifyPropertyChanged("Layer");
                }
            }
        }

        private CivicAddress _CivicAddress;
        /// <summary>
        /// The civic adress of the pushpin if it can be resolved.
        /// </summary>
        public CivicAddress CivicAddress
        {
            get { return _CivicAddress; }
            set
            {
                if (value != _CivicAddress)
                {
                    _CivicAddress = value;
                    NotifyPropertyChanged("CivicAddress");
                }
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
