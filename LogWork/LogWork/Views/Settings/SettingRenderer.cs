using LogWork.Effects;
using LogWork.Models;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Controls;
using Xamarin.Forms.Converters;
using Xamarin.Forms.Extensions;

namespace LogWork.Views.SystemSettings
{
    public class SettingRenderer : ContentView
    {
        public static readonly BindableProperty SettingProperty = BindableProperty.Create(nameof(Setting), typeof(Setting), typeof(SettingRenderer), null, BindingMode.TwoWay, propertyChanged: (b, o, n) => ((SettingRenderer)b).OnSettingChanged(b, o, n));

        public Setting Setting
        {
            get { return (Setting)GetValue(SettingProperty); }
            set { SetValue(SettingProperty, value); }
        }

        public SettingRenderer()
        {
        }

        private void OnSettingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (Setting != null)
            {
                RenderView();
            }
        }

        private void RenderView()
        {
            try
            {
                switch (Setting.Type)
                {
                    case "Text":
                        RenderText();
                        break;

                    case "Alert":
                        RenderAlert();
                        break;

                    case "List":
                        RenderList();
                        break;

                    case "Checkbox":
                        RenderCheckbox();
                        break;

                    case "Time":
                        RenderTime();
                        break;

                    case "":
                        RenderLabel();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void RenderText()
        {
            var stk = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 10,
                Spacing = 2
            };

            var name = new Label()
            {
                FontSize = 14,
                Text = TranslateExtension.GetValue(Setting.Name),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.Center
            };

            var value = new Label()
            {
                FontSize = 13,
                TextColor = Color.Gray,
                VerticalOptions = LayoutOptions.Center
            };
            if (Setting.Name.Equals("APP_LAST_SYNCHRO") || Setting.Name.Equals("APP_LAST_SYNCHRO_PRODUCT") || Setting.Name.Equals("APP_LAST_SYNCHRO_INVOICE"))
            {
                value.SetBinding(Label.TextProperty, new Binding("Value", BindingMode.Default, converter: new UnixToDateTimeConverter(), stringFormat: TranslateExtension.GetValue("label_text_current_value_2") + " {0:dd/MM/yyyy HH:mm:ss}", source: Setting));
            }
            else
            {
                value.SetBinding(Label.TextProperty, new Binding("Value", BindingMode.Default, stringFormat: TranslateExtension.GetValue("label_text_current_value_2") + " {0}", source: Setting));
            }

            stk.Children.Add(name);
            stk.Children.Add(value);

            stk.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async (sender) =>
                {
                    if (await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue(Setting.Name), TranslateExtension.GetValue(Setting.Message), TranslateExtension.GetValue("yes"), TranslateExtension.GetValue("no")))
                    {
                        if (Setting.Name.Equals("APP_LAST_SYNCHRO"))
                        {
                            Settings.LastSync = "0";
                            if (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO")) is Setting lastSync)
                            {
                                lastSync.Value = "0";
                                App.LocalDb.Update(lastSync);
                            }
                            Setting.Value = "0";
                        }
                        else if (Setting.Name.Equals("APP_LAST_SYNCHRO_PRODUCT"))
                        {
                            Settings.LastSyncProduct = 0;
                            if (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO_PRODUCT")) is Setting lastSyncProduct)
                            {
                                lastSyncProduct.Value = "0";
                                App.LocalDb.Update(lastSyncProduct);
                            }
                            Setting.Value = "0";
                        }
                        else if (Setting.Name.Equals("APP_LAST_SYNCHRO_INVOICE"))
                        {
                            Settings.LastSyncInvoice = 0;
                            if (App.LocalDb.Table<Setting>().ToList().Find(se => se.Name.Equals("APP_LAST_SYNCHRO_INVOICE")) is Setting lastSyncInvoice)
                            {
                                lastSyncInvoice.Value = "0";
                                App.LocalDb.Update(lastSyncInvoice);
                            }
                            Setting.Value = "0";
                        }
                    }
                })
            });

            Content = stk;
        }

        private void RenderAlert()
        {
            var stk = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 10,
                Spacing = 2
            };

            var name = new Label()
            {
                FontSize = 14,
                Text = TranslateExtension.GetValue(Setting.Name),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.Center
            };

            var description = new Label()
            {
                FontSize = 13,
                Text = TranslateExtension.GetValue(Setting.Description),
                TextColor = Color.Gray,
                VerticalOptions = LayoutOptions.Center
            };

            stk.Children.Add(name);
            stk.Children.Add(description);

            stk.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async (sender) =>
                {
                    if (await Application.Current.MainPage.DisplayAlert(TranslateExtension.GetValue(Setting.Name), TranslateExtension.GetValue(Setting.Message), TranslateExtension.GetValue("yes"), TranslateExtension.GetValue("no")))
                    {
                        if (Setting.Name.Equals("APP_RESET_DATA"))
                        {
                            Setting.OnPropertyChanged();
                        }
                    }
                })
            });

            Content = stk;
        }

        private void RenderList()
        {
            var stk = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 5,
                Spacing = 0
            };

            var title = new Label()
            {
                FontSize = 14,
                Text = TranslateExtension.GetValue(Setting.Name),
                TextColor = Color.Black
            };
            stk.Children.Add(title);

            var input = new Picker()
            {
                Title = TranslateExtension.GetValue(Setting.Name),
                FontSize = 14,
                TextColor = Color.Black,
                ItemDisplayBinding = new Binding(".", converter: new TranslateConverter()),
            };
            input.Effects.Add(new BorderlessEffect());
            input.SetBinding(Picker.ItemsSourceProperty, new Binding("Arrange", BindingMode.TwoWay, source: Setting));
            input.SelectedIndexChanged += Input_SelectedIndexChanged;
            if (!string.IsNullOrWhiteSpace(Setting.Value))
            {
                input.SelectedItem = Setting.Value;
            }
            stk.Children.Add(input);

            Content = stk;
        }

        private void Input_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && Setting.Arrange.ElementAt(picker.SelectedIndex) is string item)
            {
                Setting.Value = item;
            }
        }

        private void RenderCheckbox()
        {
            var grid = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 10,
                RowSpacing = 2,
                ColumnDefinitions = { new ColumnDefinition() { Width = GridLength.Star }, new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Absolute) } },
                RowDefinitions = { new RowDefinition() { Height = GridLength.Auto }, new RowDefinition() { Height = GridLength.Auto } }
            };

            var name = new Label()
            {
                FontSize = 14,
                Text = TranslateExtension.GetValue(Setting.Name),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.Center
            };

            var description = new Label()
            {
                FontSize = 13,
                Text = TranslateExtension.GetValue(Setting.Description),
                TextColor = Color.Gray,
                VerticalOptions = LayoutOptions.Center
            };

            var input = new ToggleBox()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 30,
                WidthRequest = 30,
                CheckedImage = "ic_checked_box_black",
                UnCheckedImage = "ic_unchecked_box_black"
            };
            input.SetBinding(ToggleBox.IsToggledProperty, new Binding("Value", BindingMode.TwoWay, converter: new IntToBoolConverter(), source: Setting));
            if (!string.IsNullOrWhiteSpace(Setting.Value) && ConvertToBool(Setting.Value, out bool value))
            {
                input.IsToggled = value;
            }

            grid.Children.Add(name, 0, 0);
            grid.Children.Add(description, 0, 1);
            grid.Children.Add(input, 1, 2, 0, 2);

            Content = grid;
        }

        public bool ConvertToBool(object value, out bool result)
        {
            try
            {
                result = Convert.ToInt32(value) != 0;
                return true;
            }
            catch
            {
                result = false;
                return false;
            }
        }

        private void RenderTime()
        {
            var stk = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 5,
                Spacing = 0
            };

            var title = new Label()
            {
                FontSize = 14,
                Text = TranslateExtension.GetValue(Setting.Name),
                TextColor = Color.Black
            };

            var content = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            var value = new Label()
            {
                FontSize = 13,
                Text = TranslateExtension.GetValue("label_text_current_value_2"),
                TextColor = Color.Gray,
                VerticalOptions = LayoutOptions.Center
            };

            var input = new TimePicker()
            {
                FontSize = 14,
                TextColor = Color.Black,
                Format = "HH:mm"
            };
            input.Effects.Add(new BorderlessEffect());
            input.SetBinding(TimePicker.TimeProperty, new Binding("Value", BindingMode.TwoWay, converter: new StringToTimeSpanConverter(), source: Setting));
            if (!string.IsNullOrWhiteSpace(Setting.Value))
            {
                input.Time = ConvertStringToTimeSpan(Setting.Value);
            }

            content.Children.Add(value);
            content.Children.Add(input);

            stk.Children.Add(title);
            stk.Children.Add(content);

            stk.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command((sender) =>
                {
                    input.Focus();
                })
            });

            Content = stk;
        }

        private TimeSpan ConvertStringToTimeSpan(string value)
        {
            if (TimeSpan.TryParse(value, out TimeSpan ts))
                return ts;

            return TimeSpan.Zero;
        }

        private void RenderLabel()
        {
            var stk = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 10,
                Spacing = 2
            };

            var name = new Label()
            {
                FontSize = 14,
                Text = TranslateExtension.GetValue(Setting.Name),
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.Center
            };

            var detail = new Label()
            {
                FontSize = 13,
                Text = Setting.Value,
                TextColor = Color.Gray,
                VerticalOptions = LayoutOptions.Center
            };

            stk.Children.Add(name);
            stk.Children.Add(detail);

            Content = stk;
        }
    }
}