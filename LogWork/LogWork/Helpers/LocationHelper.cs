using Acr.UserDialogs;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LogWork.Helpers
{
    public class LocationHelper
    {
        public static async Task<bool> CheckLocationPermission(bool showAlert)
        {
            if (!await PermissionsHelper.CheckPermissions(Permission.Location))
            {
                if (showAlert)
                    ShowMessage("Location permission is denied. Please go into Settings and turn on Location for the app");
                return false;
            }

            return true;
        }

        public static bool IsGeolocationAvailable(bool showAlert)
        {
            if (!CrossGeolocator.IsSupported)
            {
                if (showAlert)
                    ShowMessage("Location is not supported");
                return false;
            }

            if (!CrossGeolocator.Current.IsGeolocationAvailable)
            {
                if (showAlert)
                    ShowMessage("Location is not available");
                return false;
            }

            return true;
        }

        public static bool IsGeolocationEnabled(bool showAlert)
        {
            if (!CrossGeolocator.Current.IsGeolocationEnabled)
            {
                if (showAlert)
                    ShowMessage("GPS is turn off, please turn GPS on to use Location");
                return false;
            }

            return true;
        }

        private static void ShowMessage(string content)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Application.Current?.MainPage?.DisplayAlert("Location", content, "Ok");
            });
        }

        public static async Task<Position> GetCurrentPosition(double desiredAccuracy = 100, double timeOut = 20, CancellationToken? token = null, bool showOverlay = false)
        {
            Position position = null;

            try
            {
                CrossGeolocator.Current.DesiredAccuracy = desiredAccuracy;
                
                if (showOverlay)
                    UserDialogs.Instance.Loading("Getting location...").Show();

                position = await CrossGeolocator.Current.GetLastKnownLocationAsync();

                if (position != null)
                {
                    //got a cahched position, so let's use it.
                    return position;
                }

                position = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(timeOut), token, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location: " + ex);
            }
            finally
            {
                if (showOverlay)
                    UserDialogs.Instance.Loading().Hide();
            }

            if (position == null)
                return null;

            Debug.WriteLine(string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                    position.Timestamp, position.Latitude, position.Longitude, position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed));

            return position;
        }

        public static async Task<List<Address>> GetAddressesForPosition(Position position)
        {
            List<Address> addresses = null;

            try
            {
                addresses = (await CrossGeolocator.Current.GetAddressesForPositionAsync(position))?.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location: " + ex);
            }

            return addresses ?? new List<Address>();
        }

        public static event EventHandler<PositionEventArgs> PositionChanged;

        public static async Task StartListening(TimeSpan? interval = null, double minimumDistance = 100)
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StartListeningAsync(interval ?? TimeSpan.FromMinutes(1), minimumDistance, true);

            CrossGeolocator.Current.PositionChanged += OnPositionChanged;
            CrossGeolocator.Current.PositionError += OnPositionError;
        }

        public static async Task StopListening()
        {
            if (!CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StopListeningAsync();

            CrossGeolocator.Current.PositionChanged -= OnPositionChanged;
            CrossGeolocator.Current.PositionError -= OnPositionError;
        }

        private static void OnPositionChanged(object sender, PositionEventArgs e)
        {
            //If updating the UI, ensure you invoke on main thread
            var position = e.Position;
            var output = "Full: Lat: " + position.Latitude + " Long: " + position.Longitude;
            output += "\n" + $"Time: {position.Timestamp}";
            output += "\n" + $"Heading: {position.Heading}";
            output += "\n" + $"Speed: {position.Speed}";
            output += "\n" + $"Accuracy: {position.Accuracy}";
            output += "\n" + $"Altitude: {position.Altitude}";
            output += "\n" + $"Altitude Accuracy: {position.AltitudeAccuracy}";
            Debug.WriteLine(output);
            
            App.LocalDb.InsertOrReplace(new Models.Location()
            {
                Id = Guid.NewGuid(),
                UserId = Settings.CurrentUserId,
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Altitude = position.Altitude,
                EditDate = DateTime.Now,
                IsToSync = true
            });

            Debug.WriteLine("Location Added!");

            PositionChanged?.Invoke(sender, e);
            //MessagingCenter.Send(this, MessageKey.POSITION_CHANGED, e.Position);
        }

        private static void OnPositionError(object sender, PositionErrorEventArgs e)
        {
            Debug.WriteLine(e.Error);
            //Handle event here for errors
        }
    }
}