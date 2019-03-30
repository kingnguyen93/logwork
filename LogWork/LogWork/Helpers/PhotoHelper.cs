using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;

namespace LogWork.Helpers
{
    public static class PhotoHelper
    {
        public static async Task<MediaFile> TakePhotoAsync(string imageName = null)
        {
            try
            {
                MediaFile result = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var name = imageName ?? ("IMG_" + DateTime.Now.ToString("yyyyMMddhhmmss"));

                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        DefaultCamera = CameraDevice.Rear,
                        SaveToAlbum = false,
                        CompressionQuality = 85,
                        Directory = "LogWork",
                        Name = name
                    });

                    if (file == null)
                        return null;

                    file.Dispose();
                }

                return result;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), e.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }

        public static async Task<Stream> TakePhotoStreamAsync(string imageName = null)
        {
            try
            {
                Stream result = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var name = imageName ?? ("IMG_" + DateTime.Now.ToString("yyyyMMddhhmmss"));

                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        DefaultCamera = CameraDevice.Rear,
                        SaveToAlbum = false,
                        CompressionQuality = 85,
                        Directory = "LogWork",
                        Name = name
                    });

                    if (file == null)
                        return null;

                    result = file.GetStream();

                    file.Dispose();
                }

                return result;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), e.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }

        public static async Task<string> TakePhotoPathAsync(string imageName = null)
        {
            try
            {
                var path = string.Empty;

                if (await CameraHelper.CanTakePhoto())
                {
                    var name = imageName ?? ("IMG_" + DateTime.Now.ToString("yyyyMMddhhmmss"));
                    MediaFile file = null;

                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        DefaultCamera = CameraDevice.Rear,
                        SaveToAlbum = false,
                        CompressionQuality = 85,
                        Directory = "LogWork",
                        Name = name
                    });

                    if (file == null)
                        return null;

                    path = file.Path;

                    file.Dispose();
                }

                return path;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), e.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }

        public static async Task<MediaFile> PickPhotoAsync()
        {
            try
            {
                MediaFile result = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        CompressionQuality = 85
                    });

                    if (file == null)
                        return null;

                    file.Dispose();
                }

                return result;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), e.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }

        public static async Task<Stream> PickPhotoStreamAsync()
        {
            try
            {
                Stream result = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        CompressionQuality = 85
                    });

                    if (file == null)
                        return null;

                    result = file.GetStream();

                    file.Dispose();
                }

                return result;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), e.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }

        public static async Task<string> PickPhotoPathAsync()
        {
            try
            {
                var path = string.Empty;

                if (await CameraHelper.CanTakePhoto())
                {
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1600,
                        CompressionQuality = 85
                    });

                    if (file == null)
                        return null;

                    path = file.Path;

                    file.Dispose();
                }

                return path;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), e.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }

        public static async Task<Stream> TakeVideoStreamAsync(string videoName = null)
        {
            try
            {
                Stream stream = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var name = videoName ?? ("VID_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".mp4");

                    var file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
                    {
                        Directory = "Videos",
                        Name = name
                    });

                    if (file == null)
                        return null;

                    stream = file.GetStream();

                    file.Dispose();
                }

                return stream;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), e.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }

        public static async Task<Stream> PickVideoStreamAsync()
        {
            try
            {
                Stream stream = null;

                if (await CameraHelper.CanTakePhoto())
                {
                    var file = await CrossMedia.Current.PickVideoAsync();

                    if (file == null)
                        return null;

                    stream = file.GetStream();

                    file.Dispose();
                }

                return stream;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), e.GetBaseException().Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }
    }
}