using System.Windows.Controls;

namespace Net.SyntaxC4.RedBitSoftware.ODAF.SearchComponent
{
	public partial class SearchView : UserControl
	{
		public SearchView()
		{
			// Required to initialize variables
			InitializeComponent();
		}


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchTypePopup.IsOpen = false;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            searchTypePopup.IsOpen = true;
        }

        private void WatermarkedTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                // TODO: Cory, invoke your stuff
               // .Invoke();
               // btnSearch.Triggers.
            }
        }


	}
}