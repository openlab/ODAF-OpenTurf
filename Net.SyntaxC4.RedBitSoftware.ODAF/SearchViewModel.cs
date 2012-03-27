
// Developer: Cory Fowler
// Company: RedBit Software
// Description:

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Abstractions;
    using GalaSoft.MvvmLight.Command;

    public class SearchViewModel : SearchViewBase
    {
        public SearchViewModel()
            : base()
        {
            Initialize();
            ComponentSelectorVisibility = Visibility.Collapsed;
        }

        private void Initialize()
        {
            if (!IsInDesignMode)
            {
                SelectComponent = new RelayCommand<SelectionChangedEventArgs>(e =>
                {
                    ISearchComponent component = e.AddedItems[0] as ISearchComponent;
                    CurrentlySelectedComponent = component;
                    ToggleComponentSelectorVisibility();
                    UpdateProperties(CurrentlySelectedComponent);
                });
                OpenComponentSelector = new RelayCommand<RoutedEventArgs>(e =>
                {
                    ToggleComponentSelectorVisibility();
                });
                PerformAction = new RelayCommand<RoutedEventArgs>(e =>
                {
                    CurrentlySelectedComponent.UserInput = UserInput;
                    CurrentlySelectedComponent.PerformAction.Execute(e);
                });
            }
        }

        private void ToggleComponentSelectorVisibility()
        {
            ComponentSelectorVisibility = (IsSearchComponentBoxOpen) ? Visibility.Collapsed : Visibility.Visible;
            IsSearchComponentBoxOpen = !IsSearchComponentBoxOpen;
            RaisePropertyChanged("ComponentSelectorVisibility");
        }

        public void UpdateProperties(ISearchComponent component)
        {
            TextBoxWaterMarkText = component.WaterMarkText;
            ImageBrush iconImage = new ImageBrush()
            {
                ImageSource = new BitmapImage(component.Icon)
            };
            Icon = iconImage;

            RaisePropertyChanged("TextBoxWaterMarkText");
            RaisePropertyChanged("Icon");
        }

        public string UserInput { get; set; }
        public Visibility ComponentSelectorVisibility { get; private set; }
    }
}
