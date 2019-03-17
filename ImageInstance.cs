using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ChessPiecesDetection
{
    public class ImageInstance : INotifyPropertyChanged
    {
        public ImageInstance(WriteableBitmap imageBitmap)
        {            
            ImageBitmap = imageBitmap;
        }

        public ImageProperties ImageInfoProperties { get; }
        public WriteableBitmap ImageBitmap { get; }

        public async Task<WriteableBitmap> GetImageSourceAsync()
        {
            WriteableBitmap bmp = new WriteableBitmap(ImageBitmap.PixelWidth, ImageBitmap.PixelHeight);
            using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
            {
                await ImageBitmap.ToStream(stream, BitmapEncoder.BmpEncoderId);
                stream.Seek(0);
                await bmp.SetSourceAsync(stream);
            }

            return bmp;
        }

        private float _exposure = 0;
        public float Exposure
        {
            get => _exposure;
            set => SetEditingProperty(ref _exposure, value);
        }

        private float _temperature = 0;
        public float Temperature
        {
            get => _temperature;
            set => SetEditingProperty(ref _temperature, value);
        }

        private float _tint = 0;
        public float Tint
        {
            get => _tint;
            set => SetEditingProperty(ref _tint, value);
        }

        private float _contrast = 0;
        public float Contrast
        {
            get => _contrast;
            set => SetEditingProperty(ref _contrast, value);
        }

        private float _saturation = 1;
        public float Saturation
        {
            get => _saturation;
            set => SetEditingProperty(ref _saturation, value);
        }

        private float _blur = 0;
        public float Blur
        {
            get => _blur;
            set => SetEditingProperty(ref _blur, value);
        }

        private bool _needsSaved = false;
        public bool NeedsSaved
        {
            get => _needsSaved;
            set => SetProperty(ref _needsSaved, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetEditingProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                if (Exposure != 0 || Temperature != 0 || Tint != 0 || Contrast != 0 || Saturation != 1 || Blur != 0)
                {
                    NeedsSaved = true;
                }
                else
                {
                    NeedsSaved = false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }
    }
}
