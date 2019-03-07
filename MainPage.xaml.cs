using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChessPiecesDetection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PersistentObjects _PersistentObjects;
        public MainPage()
        {
            this.InitializeComponent();
            _PersistentObjects = new PersistentObjects();
            _PersistentObjects.isCroppingImage = false;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        /// <summary>
        /// Switches between each Menu option by loading the relevant XAML Frame in the MainFrame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoadFile.IsSelected)
            {
                VisibleFrame.Navigate(typeof(PrepareImagePage), _PersistentObjects);
            }
            else if (ProcessItem.IsSelected)
            {
                VisibleFrame.Navigate(typeof(ProcessFilePage), _PersistentObjects);
            }
            else if (ViewDatabase.IsSelected)
            {
                VisibleFrame.Navigate(typeof(PiecesTableDBPage), _PersistentObjects);
            }
            else if (ConfigureView.IsSelected)
            {
                VisibleFrame.Navigate(typeof(ConfigurationsPage), _PersistentObjects);
            }
        }
    }
}
