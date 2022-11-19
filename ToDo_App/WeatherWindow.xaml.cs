using System;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ToDo_App.Helpers;
using ToDo_App.Models;

namespace ToDo_App
{
    /// <summary>
    /// Interaction logic for WeatherWindow.xaml
    /// </summary>
    public partial class WeatherWindow : Window
    {
        private readonly AppSecrets _appSecrets;

        public WeatherWindow()
        {
            InitializeComponent();
            _appSecrets = AppSecretsHelpers.AppSecrets();
        }

        private static Uri? ResolveWeatherImageUri(string weatherIcon)
        {
            Uri? uri = null;

            switch (weatherIcon)
            {
                case "01d":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\01d.png");
                    break;

                case "01n":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\01n.png");
                    break;

                case "02d":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\02d.png");
                    break;

                case "02n":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\02n.png");
                    break;

                case "03d":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\03d.png");
                    break;

                case "03n":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\03n.png");
                    break;

                case "04d":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\04d.png");
                    break;

                case "04n":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\04n.png");
                    break;

                case "09d":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\09d.png");
                    break;

                case "09n":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\09n.png");
                    break;

                case "10d":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\10d.png");
                    break;

                case "10n":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\10n.png");
                    break;

                case "11d":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\11d.png");
                    break;

                case "11n":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\11n.png");
                    break;

                case "13d":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\13d.png");
                    break;

                case "13n":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\13n.png");
                    break;

                case "50d":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\50d.png");
                    break;

                case "50n":
                    uri = new Uri("C:\\Users\\chris\\Documents\\Programming\\C#\\2022\\09152022 ToDo App\\ToDo_App\\ToDo_App\\Images\\50n.png");
                    break;

                default:
                    break;
            }

            return uri;
        }

        // Could not avoid "async void" method type here.
        private async void btnWeather_ClickAsync(object sender, RoutedEventArgs e)
        {
            tboxCity.Text = tboxCity.Text.Trim();
            tboxCountry.Text = tboxCountry.Text.Trim();

            if (tboxCity.Text == null || tboxCity.Text == "" || tboxCountry.Text == null || tboxCountry.Text == "") return;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var cityTextHtmlEncoded = HttpUtility.UrlEncode(tboxCity.Text);
                    var countryTextHtmlEncoded = HttpUtility.UrlEncode(tboxCountry.Text);

                    using (var response = await httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={cityTextHtmlEncoded},{countryTextHtmlEncoded}&APPID={_appSecrets.WeatherApiKey}&units=metric"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(apiResponse)!;

                        if (weatherApiResponse != null)
                        {
                            PopulateWeatherFields(weatherApiResponse);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error obtaining weather data");
            }
        }

        private void PopulateWeatherFields(WeatherApiResponse weatherApiResponse)
        {
            lblCurrentTemp.Content = weatherApiResponse.main.temp;
            lblFeelsLikeTemp.Content = weatherApiResponse.main.feels_like;
            lblMinTemp.Content = weatherApiResponse.main.temp_min;
            lblMaxTemp.Content = weatherApiResponse.main.temp_max;
            lblWindSpeed.Content = weatherApiResponse.wind.speed;
            lblSunrise.Content = DateTimeHelpers.ConvertUtcTo24HrTime(weatherApiResponse.sys.sunrise);
            lblSunset.Content = DateTimeHelpers.ConvertUtcTo24HrTime(weatherApiResponse.sys.sunset);
            lblCurrentWeather.Content = weatherApiResponse.weather[0].main;
            lblConditions.Content = weatherApiResponse.weather[0].description;

            if (weatherApiResponse.weather[0].icon != null && weatherApiResponse.weather[0].icon != "")
            {
                imgWeatherIcon.Source = new BitmapImage(ResolveWeatherImageUri(weatherApiResponse.weather[0].icon));
            }
        }

        private void tbox_GotFocus_SetCursorToEnd(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;

            if (textBox.Text.Length > 0)
            {
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }
}