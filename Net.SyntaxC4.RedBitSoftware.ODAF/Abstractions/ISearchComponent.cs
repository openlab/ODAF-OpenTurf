
// Developer: Cory Fowler
// Company: RedBit Software
// Description: Provides Extensibility Point for new Search Features

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search.Abstractions
{
    using System;
    using System.Windows;
    using GalaSoft.MvvmLight.Command;

    public interface ISearchComponent
    {
        #region Properties
        string WaterMarkText { get; set; }
        string DisplayText { get; set; }
        string UserInput { get; set; }
        Uri Icon { get; set; }

        RelayCommand<RoutedEventArgs> PerformAction { get; set; }
        #endregion
    }
}