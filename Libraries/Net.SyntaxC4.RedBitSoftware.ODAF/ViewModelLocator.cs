// Developer: Cory Fowler
// Company: RedBit Software
// Description: 

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search
{
    using GalaSoft.MvvmLight;
    using Microsoft.Maps.MapControl;
    using Net.SyntaxC4.RedBitSoftware.ODAF.Search;

    public class ViewModelLocator
    {
        #region Properties

        public ViewModelBase Search
        {
            get { return new SearchViewModel(); }
        }
        #endregion

        /// <summary>
        /// Wire Up ViewModels for Resolution
        /// </summary>
        static ViewModelLocator()
        {
        }

        /// <summary>
        /// Clean up 
        /// </summary>
        public static void Cleanup()
        {

        }


    }
}