
// Developer: Cory Fowler
// Company: RedBit Software
// Description: 

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search.Abstractions
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    public abstract class SearchViewBase : ViewModelBase
    {
        #region Properties
        public string TextBoxWaterMarkText { get; set; }
        public string TextBoxValue { get; set; }
        public bool IsSearchComponentBoxOpen { get; set; }

        public Brush Icon { get; set; }

        [ImportMany(typeof(ISearchComponent))]
        public IEnumerable<ISearchComponent> SearchComponents { get; set; }
        public ISearchComponent CurrentlySelectedComponent { get; set; }

        public RelayCommand<RoutedEventArgs> PerformAction { get; set; }
        public RelayCommand<RoutedEventArgs> OpenComponentSelector { get; set; }
        public RelayCommand<SelectionChangedEventArgs> SelectComponent { get; set; }
        #endregion

        public SearchViewBase()
        {
            if (!IsInDesignMode)
            {
                CompositionInitializer.SatisfyImports(this);
                IsSearchComponentBoxOpen = false;
                CurrentlySelectedComponent = SearchComponents.First();
            }
        }
    }
}