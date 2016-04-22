using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPTabunClient
{
    class CacheManager
    {
        public static async Task<bool> createImageFile(string path, string name, SoftwareBitmap image)
        {
            StorageFolder storageFolder = 
                await ApplicationData.Current.LocalFolder.CreateFolderAsync(path, CreationCollisionOption.OpenIfExists);

            StorageFile storageFile = await storageFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);

            using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetSoftwareBitmap(image);

                await encoder.FlushAsync();
            }

            return true;
        }

        public static async Task<SoftwareBitmap> readImageFile(string path, string name)
        {
            StorageFolder storageFolder =
                await ApplicationData.Current.LocalFolder.CreateFolderAsync(path, CreationCollisionOption.OpenIfExists);

            StorageFile storageFile = await storageFolder.GetFileAsync(name);

            var stream = await storageFile.OpenAsync(FileAccessMode.Read);

            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            SoftwareBitmap resultBitmap = await decoder.GetSoftwareBitmapAsync();

            resultBitmap = SoftwareBitmap.Convert(resultBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore);
            

            return resultBitmap;
        }

        public static async Task<bool> isFileActual(string path, string name)
        {
            try {
                StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(path);
                StorageFile storageFile = await storageFolder.GetFileAsync(name);
            } catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
