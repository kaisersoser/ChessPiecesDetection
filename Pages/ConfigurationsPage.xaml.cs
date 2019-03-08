using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ChessPiecesDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigurationsPage : Page
    {
        private PersistentObjects _LocalPersistentObject;
        public ConfigurationsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the currently loaded image in the ImageView control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _LocalPersistentObject = e.Parameter as PersistentObjects;
            PredictionURL.Text = _LocalPersistentObject.predictionURLString;
        }

        /// <summary>
        /// Updates the URL with the value entered in the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PredictionURL_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            _LocalPersistentObject.predictionURLString = sender.Text;
        }
    }
}
