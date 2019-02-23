﻿using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ChessPiecesDetection
{
    public sealed partial class ImageCropping : Page
    {
        public ImageCropping()
        {
            this.InitializeComponent();            
        }

        /// <summary>
        /// Sets the currently loaded image in to the ImageCropper
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PersistentObjects persistenceObjectsInstance = e.Parameter as PersistentObjects;
            

            if (!persistenceObjectsInstance.isCroppingImage)
            {
                base.OnNavigatedTo(e);                
                CroppedImage.Source = persistenceObjectsInstance.bitmapProcessingImage;
                persistenceObjectsInstance.ImageCropperInstance = this;

                persistenceObjectsInstance.isCroppingImage = true;
            }

        }

        /// <summary>
        /// Returns a cropped image
        /// </summary>
        /// <returns></returns>
        public async Task<WriteableBitmap> GetCropppedImageBitmap()
        {
            WriteableBitmap bmp;

            //Saves the cropped image to a stream.
            using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
            {
                await CroppedImage.SaveAsync(stream, BitmapFileFormat.Bmp);
                stream.Seek(0);

                Windows.Graphics.Imaging.BitmapDecoder decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                bmp =  BitmapFactory.New((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                bmp.SetSource(stream);
            }
            return bmp;
        }

    }
}
