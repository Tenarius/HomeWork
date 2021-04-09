using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace TestTask
{
    class Test2
    {
        private string url; // Запрос для получения JSON файла.
        private string weatherData; // Хранить JSON в виде строки.
        private WeatherResponce weatherResponce; // Класс для хранения объекта Weather.
        public Test2(string city)
        {
            if (!TestForNullOrEmpty(city))
            {
                url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid=8ed06c2797d8bd091c9046a5923f439d";
                GetWeatherData(url);
                SetWeatherResponce(weatherData);
                CreateFileDataWeather();
            }
            else
            {
                Console.WriteLine("Invalid city name.");
                return;
            }       
        }

        // Получаем JSON.
        private void GetWeatherData(string url)
        {
            if (IsAddress(url))
            {
                try
                {
                    HttpClient client = new HttpClient();

                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = response.Content)
                        {
                            weatherData = content.ReadAsStringAsync().Result;
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
        }

        // Десериализация.
        private WeatherResponce SetWeatherResponce(string weather)
        {
            if (!TestForNullOrEmpty(weather))
            {
                weatherResponce = JsonConvert.DeserializeObject<WeatherResponce>(weather);
            }

            return weatherResponce;
        }

        // Проверка на корректность URL.
        private bool IsAddress(string address)
        {
            if (Uri.IsWellFormedUriString(address, UriKind.Absolute))
            {
                return true;
            }

            Console.WriteLine("Invalid address.");
            return false;
        }

        // Проверка на пустую строку и NULL.
        private bool TestForNullOrEmpty(string s)
        {
            bool result;
            result = s == null || s == string.Empty;
            return result;
        }

        // Вывод данных в косноль.
        public void GetWeather()
        {
            if (!IsNullWeather(weatherResponce))
            {
                Console.WriteLine($"Температура: {weatherResponce.Main.Temp}°C\nВлажность: {weatherResponce.Main.Humidity}%\n");
                Console.WriteLine($"Восход: {weatherResponce.Sys.Sunrise}\nЗакат: {weatherResponce.Sys.Sunset}");
            }
        }

        /// <summary>
        /// Создает файл с данными о погоде.
        /// </summary>
        private void CreateFileDataWeather()
        {
            string pathDir = @"C:\WeatherData";
           
            DirectoryInfo dirInfo = new DirectoryInfo(pathDir);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            string pathFile = pathDir + @"\" + DateTime.Now.ToString("dd.MM.yyyy") + ".txt";

            if (!IsNullWeather(weatherResponce))
            {
                using (StreamWriter writer = new StreamWriter(pathFile, false, Encoding.Default))
                {
                    writer.WriteLine($"Температура: {weatherResponce.Main.Temp}°C\nВлажность: {weatherResponce.Main.Humidity}%\n");
                    writer.WriteLine($"Восход: {weatherResponce.Sys.Sunrise}\nЗакат: {weatherResponce.Sys.Sunset}");
                }
            }
        }

        /// <summary>
        /// Поверка на NULL.
        /// </summary>
       private bool IsNullWeather(WeatherResponce weatherResponce)
        {
            if (weatherResponce == null)
            {
                return true;
            }

            if(weatherResponce.Main == null && weatherResponce.Sys == null)
            {
                return true;
            }

            return false;
        }
    }
}
