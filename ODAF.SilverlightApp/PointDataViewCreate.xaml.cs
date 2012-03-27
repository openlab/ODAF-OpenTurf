using System.Windows.Controls;
using ODAF.SilverlightApp.VO;
using ODAF.SilverlightApp.CloudService;
using Microsoft.Maps.MapControl;

namespace ODAF.SilverlightApp
{
    public partial class PointDataViewCreate : UserControl
    {
        private PlaceMark currPlaceMark;
        public PlaceMark CurrentPlaceMark
        {
            get { return currPlaceMark; }
            set
            {
                this.currPlaceMark = value;
            }
        }

        private PointDataSummary _currentSummary;
        public PointDataSummary CurrentSummary
        {
            get { return _currentSummary; }
            set
            {
                _currentSummary = value;
            }
        }

        private SummariesController _summaryController;
        public SummariesController Summary
        {
            get { return _summaryController; }
            set
            {
                _summaryController = value;
                _summaryController.SummaryUpdate += new SummaryUpdateEventHandler(OnSummaryUpdate);
            }

        }

        private UserController _userController;
        public UserController User
        {
            get { return _userController; }
            set
            {
                _userController = value;
               // _userController.AuthUpdate += new AuthUpdateEventHandler(OnAuthUpdate);
            }
        }

        public MainPage MainPage { get; set; }


        public Pushpin Pin { get; set; }
        

        public PointDataViewCreate()
        {
            InitializeComponent();

            this.createViewBox.ParentView = this;
        }

        protected void UpdateData()
        {

        }

        public void Cancel()
        {
            createViewBox.ClearForm();
            MainPage.PointCreateCancelled();
            CurrentSummary = null;
        }

        // Called by SummariesController when new data is retrieved
        // the data may not belong to our current placemark so we need to check the ids
        void OnSummaryUpdate(object sender, SummaryUpdateArgs e)
        {
            PointDataSummary summary = e.result;
            if (CurrentSummary != null && summary.Guid == CurrentSummary.Guid)
            {
                // NOTE: because we are called by an ASynch thread, 
                // we have to marshall to the UI thread with Dispatcher
                this.Dispatcher.BeginInvoke(delegate()
                {
                    createViewBox.ClearForm();
                    MainPage.PointCreated(summary);
                    CurrentSummary = summary;
                });
            }
        }
    }
}
