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

namespace ODAF.SilverlightApp.UserControls
{
    public partial class TweetEnterWin : ChildWindow
    {
        public TweetEnterWin()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void twitterText_TextChanged(object sender, TextChangedEventArgs e)
        {
            charCount.Text = (140 - twitterText.Text.Length).ToString();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PointDataSummary pds = (PointDataSummary)this.DataContext;
            if (pds != null)
            {
                this.twitterText.Text = "";
                twitterText_TextChanged(null, null);
            }
        }

    }
}

