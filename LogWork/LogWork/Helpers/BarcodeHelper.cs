using LogWork.Constants;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using ZXing;
using ZXing.Mobile;

namespace LogWork.Helpers
{
    public class BarcodeHelper
    {
        private static readonly Lazy<BarcodeHelper> lazy = new Lazy<BarcodeHelper>(() => new BarcodeHelper());
        public static BarcodeHelper Instance => lazy.Value;

        private BarcodeHelper()
        {
        }

        public event EventHandler<string> OnScanned;

        public async Task StartScan(MobileBarcodeScanningOptions options = null)
        {
            try
            {
                var scanner = new MobileBarcodeScanner()
                {
                    TopText = "Scan Code"
                };
                scanner.ScanContinuously(options ?? new MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.EAN_8,
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.CODE_39,
                        BarcodeFormat.CODE_128,
                        BarcodeFormat.QR_CODE
                    },
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = false,
                    TryHarder = true
                }, (result) =>
                {
                    CrossSimpleAudioPlayer.Current.Load("beep.wav");
                    CrossSimpleAudioPlayer.Current.Play();

                    OnScanned?.Invoke(this, result.Text);
                    MessagingCenter.Send(this, MessageKey.BARCODE_SCANNED, result.Text);
                });

                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    scanner.AutoFocus();
                    return true;
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), ex.Message, TranslateExtension.GetValue("alert_message_ok"));
            }
        }

        public async Task StartScan(Action<string> OnScanned, MobileBarcodeScanningOptions options = null)
        {
            try
            {
                var scanner = new MobileBarcodeScanner()
                {
                    TopText = "Scan Code"
                };
                scanner.ScanContinuously(options ?? new MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.EAN_8,
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.CODE_39,
                        BarcodeFormat.CODE_128,
                        BarcodeFormat.QR_CODE
                    },
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = false,
                    TryHarder = true
                }, (result) =>
                {
                    CrossSimpleAudioPlayer.Current.Load("beep.wav");
                    CrossSimpleAudioPlayer.Current.Play();

                    OnScanned.Invoke(result.Text);
                });

                Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                {
                    scanner.AutoFocus();
                    return true;
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), ex.Message, TranslateExtension.GetValue("alert_message_ok"));
            }
        }

        public async Task<string> ScanGeneralBarcode()
        {
            try
            {
                var result = await new MobileBarcodeScanner()
                {
                    TopText = "Scan Code"
                }.Scan(new MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.EAN_8,
                        BarcodeFormat.EAN_13,
                        BarcodeFormat.CODE_39,
                        BarcodeFormat.CODE_128,
                        BarcodeFormat.QR_CODE
                    },
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = false,
                    TryHarder = true
                });

                if (result != null)
                {
                    CrossSimpleAudioPlayer.Current.Load("beep.wav");
                    CrossSimpleAudioPlayer.Current.Play();

                    return result.Text;
                }

                return null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), ex.Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }

        public async Task<string> ScanQrCode()
        {
            try
            {
                var result = await new MobileBarcodeScanner()
                {
                    TopText = "Scan Code"
                }.Scan(new MobileBarcodeScanningOptions
                {
                    PossibleFormats = new List<BarcodeFormat>
                    {
                        BarcodeFormat.QR_CODE
                    },
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = false,
                    TryHarder = true
                });

                if (result != null)
                {
                    CrossSimpleAudioPlayer.Current.Load("beep.wav");
                    CrossSimpleAudioPlayer.Current.Play();

                    return result.Text;
                }

                return null;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue("alert_title_error"), ex.Message, TranslateExtension.GetValue("alert_message_ok"));
                return null;
            }
        }
    }
}