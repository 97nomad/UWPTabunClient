using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace UWPTabunClient.Managers
{
    class CacheManager
    {
        public CacheManager()
        {
        }

        public async Task<bool> createImageFile(string uri, SoftwareBitmap image)
        {
            KeyValuePair<string, string> PathNamePair = WebManager.convertPathFilenameFromUri(uri);

            StorageFolder storageFolder = 
                await ApplicationData.Current.LocalFolder
                    .CreateFolderAsync(PathNamePair.Key, CreationCollisionOption.OpenIfExists);

            StorageFile storageFile = await storageFolder
                    .CreateFileAsync(PathNamePair.Value, CreationCollisionOption.ReplaceExisting);

            using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetSoftwareBitmap(image);

                await encoder.FlushAsync();
            }

            return true;
        }

        public async Task<SoftwareBitmap> readImageFile(string uri)
        {
            KeyValuePair<string, string> PathNamePair = WebManager.convertPathFilenameFromUri(uri);

            StorageFolder storageFolder =
                await ApplicationData.Current.LocalFolder
                    .CreateFolderAsync(PathNamePair.Key, CreationCollisionOption.OpenIfExists);

            StorageFile storageFile = await storageFolder.GetFileAsync(PathNamePair.Value);

            using (var stream = await storageFile.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                SoftwareBitmap resultBitmap = await decoder.GetSoftwareBitmapAsync();

                resultBitmap = SoftwareBitmap.Convert(resultBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore);

                return resultBitmap;
            }
        }

        public async Task<bool> isFileActual(string uri)
        {
            KeyValuePair<string, string> PathNamePair = WebManager.convertPathFilenameFromUri(uri);

            StorageFolder storageFolder =
                await ApplicationData.Current.LocalFolder
                    .CreateFolderAsync(PathNamePair.Key, CreationCollisionOption.OpenIfExists);

            if (storageFolder.TryGetItemAsync(PathNamePair.Value) == null)
                return false;
            else
                return true;
        }
    }
}
