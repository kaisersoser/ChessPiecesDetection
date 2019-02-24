using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ChessPiecesDetection
{
    public sealed partial class ImageLoaded : Page
    {
        private PersistentObjects _localPersistentObject;                

        public ImageLoaded()
        {
            this.InitializeComponent();
            _localPersistentObject = new PersistentObjects();
        }

        /// <summary>
        /// Sets the currently loaded image in the ImageView control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _localPersistentObject = e.Parameter as PersistentObjects;
            ImageView.Source = _localPersistentObject.bitmapProcessingImage;
        }

    }
}
