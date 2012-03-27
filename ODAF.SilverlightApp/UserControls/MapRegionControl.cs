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
using Microsoft.Maps.MapControl;


namespace ODAF.SilverlightApp
{
    public class MapRegionControl : MapPolygon
    {
        private static Color[] _Colors = {Colors.Purple,Colors.Red,Colors.Brown,Colors.Cyan,Colors.Green,Colors.Magenta,Colors.Orange,Colors.Yellow,Colors.Blue};

        private static int colorIndex = 0;

        private Color RegionColor { get; set; }

        public MapRegionControl()
        {
           RegionColor = _Colors[colorIndex++ % _Colors.Length];

           this.MouseEnter += new MouseEventHandler(MapRegionControl_MouseEnter);
           this.MouseLeave += new MouseEventHandler(MapRegionControl_MouseLeave);

           this.Fill = new System.Windows.Media.SolidColorBrush(RegionColor);
           this.Stroke = new System.Windows.Media.SolidColorBrush(RegionColor);
           this.StrokeThickness = 1;
           this.Opacity = 0.25;
           
        }

        void MapRegionControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Fill = new System.Windows.Media.SolidColorBrush(RegionColor);
            this.StrokeThickness = 1;
            this.Opacity = 0.25;
           // ToolTipService.SetToolTip(this,"");

        }

        void MapRegionControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            this.StrokeThickness = 5;
            this.Opacity = 0.75;

            ToolTipService.SetToolTip(this, this.Tag);
            
        }
    }
}