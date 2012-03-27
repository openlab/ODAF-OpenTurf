using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ODAF.SilverlightApp.VO;

namespace ODAF.SilverlightApp
{
    public partial class TagView : UserControl
    {
        public PointDataView ParentView { get; set; }

        private PointDataSummary _data;
        public PointDataSummary Data
        {
            get { return _data; }


            set
            {
                _data = value;
                UpdateView();
            }
        }

        public TagView()
        {
            InitializeComponent();
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void UpdateView()
        {
            tbTitle.Text = Data.Name;
            if (Data.Tag != null && Data.Tag.Length > 0)
            {
                tbDescription.Text = Data.Tag;
            }
            else
            {
                tbDescription.Text = "This landmark has not been tagged.";
            }

            TagEntry.Text = "";
            if (ParentView.User.IsAuthenticated)
            {
                tagEntryForm.Visibility = Visibility.Visible;
                tagEntryHint.Visibility = Visibility.Collapsed;
            }
            else
            {
                tagEntryForm.Visibility = Visibility.Collapsed;
                tagEntryHint.Visibility = Visibility.Visible;
            }
        }


        private void TagSubmit_Click(object sender, RoutedEventArgs e)
        {
            ParentView.AddItemTag(this.Data.Guid, TagEntry.Text);
        }
	}
   
}
