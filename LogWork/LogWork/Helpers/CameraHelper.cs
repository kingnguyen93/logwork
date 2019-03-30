using Plugin.Media;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LogWork.Helpers
{
    public static class CameraHelper
    {
        public static async Task<bool> CheckCameraPermission()
        {
            return await PermissionsHelper.CheckPermissions(Permission.Camera);
        }

        public static async Task<bool> CanTakePhoto()
        {
            var cameraStatus = await PermissionsHelper.CheckPermissions(Permission.Camera);

            if (!cameraStatus)
            {
                await Application.Current.MainPage.DisplayAlert(Permission.Camera.ToString() + " Denied", "Unable to take photos.", "OK");
                return false;
            }

            var storageStatus = await PermissionsHelper.CheckPermissions(Permission.Storage);

            if (!storageStatus)
            {
                await Application.Current.MainPage.DisplayAlert(Permission.Storage.ToString() + " Denied", "Unable to take photos.", "OK");
                return false;
            }

            if (Device.RuntimePlatform == Device.Android)
                await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", "No camera available.", "OK");
                return false;
            }

            return true;
        }
    }
}