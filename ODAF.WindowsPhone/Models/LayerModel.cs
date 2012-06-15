using System;
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
using System.ComponentModel;
using ODAF.WindowsPhone.Commands;
using ODAF.WindowsPhone.Services;

namespace ODAF.WindowsPhone.Models
{
    /// <summary>
    /// Represents an ODAF layer. Layers are defined in PointSources.xml file at the root of ODAF website.
    /// Each layer consumes a KML stream exposed by an OGDI instance.
    /// </summary>
    public class LayerModel : INotifyPropertyChanged
    {
        #region Properties

        private string _Id;
        /// <summary>
        /// A Guid which uniquely identify the layer.
        /// </summary>
        public string Id
        {
            get { return _Id; }
            set
            {
                if (value != _Id)
                {
                    _Id = value;
                    NotifyPropertyChanged("BingMapsId");
                }
            }
        }

        private string _Link;
        /// <summary>
        /// The link to the KML stream exposed by OGDI.
        /// </summary>
        public string Link
        {
            get { return _Link; }
            set
            {
                if (value != _Link)
                {
                    _Link = value;
                    NotifyPropertyChanged("Link");
                }
            }
        }

        private string _Title;
        /// <summary>
        /// The friendly title of the layer.
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set
            {
                if (value != _Title)
                {
                    _Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _ImageLink;
        /// <summary>
        /// The link to the image which will permit to identify each pushpin on the map associated with this layer.
        /// </summary>
        public string ImageLink
        {
            get { return _ImageLink; }
            set
            {
                if (value != _ImageLink)
                {
                    _ImageLink = value;
                    NotifyPropertyChanged("ImageLink");
                }
            }
        }

        private string _Summary;
        /// <summary>
        /// A short summary to describe the layer.
        /// </summary>
        public string Summary
        {
            get { return _Summary; }
            set
            {
                if (value != _Summary)
                {
                    _Summary = value;
                    NotifyPropertyChanged("Summary");
                }
            }
        }

        private bool _Visible;
        /// <summary>
        /// A boolean to indicate if the pushpins associated with this layer should be displayer on the map.
        /// </summary>
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (value != _Visible)
                {
                    _Visible = value;
                    NotifyPropertyChanged("Visible");
                }
            }
        }

        #endregion

        public LayerModel()
        {
            _LayerModelCheckedCommand = new DelegateCommand(LayerModelChecked, obj => true);
        }

        #region Commands

        private ICommand _LayerModelCheckedCommand;
        public ICommand LayerModelCheckedCommand
        {
            get { return _LayerModelCheckedCommand; }
        }

        private void LayerModelChecked(object value)
        {
            if (this.Visible == true)
            {
                PushpinService.LoadPushpinsAsync(this, ODAF.WindowsPhone.ViewModels.MainViewModel.Pushpins);
            }
            if (this.Visible == false)
            {
                ODAF.WindowsPhone.ViewModels.MainViewModel.Pushpins.ToList().ForEach(p => { if (p.Layer == this) ODAF.WindowsPhone.ViewModels.MainViewModel.Pushpins.Remove(p); });
            }
        }

        #endregion
                
 
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
