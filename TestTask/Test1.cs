using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestTask
{
    class Test1
    {
        private string htmlSource; 

        public Test1(string address)
        {  
            if (IsAddress(address))
            {
               GetHtmlString(address);
               CreateHtmlFile(htmlSource);
               _ = CreateDirImage(GetUrlImages(htmlSource));
            }
        }

        /// <summary>
        /// Проверяет на корректность URL.
        /// </summary>
        private bool IsAddress(string address)
        {
            if (Uri.IsWellFormedUriString(address, UriKind.Absolute))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Получить HTML в виде строки.
        /// </summary>
        private string GetHtmlString(string address)
        {
            try
            {
                HttpClient client = new HttpClient();

                using (HttpResponseMessage response = client.GetAsync(address).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        htmlSource = content.ReadAsStringAsync().Result;
                    }
                }

                return htmlSource;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return "";
            }
        }

        /// <summary>
        /// Создать файл HTML.
        /// </summary>
        private void CreateHtmlFile(string htmlSource, string path = "html.txt")
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.Default))
                {
                    writer.WriteLine(htmlSource);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        /// <summary>
        /// Получить ссылки на изображение.
        /// </summary>
        private List<Uri> GetUrlImages(string htmlSource)
        {
            List<Uri> addressImages = new List<Uri>(5);
            const int maxCountAddress = 5;
            int countAddress = 0;

            foreach( Match match in Regex.Matches(htmlSource, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase) )
            {
                if (IsAddress(match.Groups[1].Value) && maxCountAddress > countAddress)
                {
                    addressImages.Add(new Uri(match.Groups[1].Value));
                    Console.WriteLine($"{countAddress + 1}) {addressImages[countAddress]}");                
                    ++countAddress;                
                }
            }

            return addressImages;
        }

        /// <summary>
        /// Получить изображение.
        /// </summary>
        private async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri)
        {
            var httpClient = new HttpClient();

            var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
            var fileExtension = Path.GetExtension(uriWithoutQuery);

            var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
            Directory.CreateDirectory(directoryPath);

            var imageBytes = await httpClient.GetByteArrayAsync(uri);
            File.WriteAllBytes(path, imageBytes);
        }

        /// <summary>
        /// Создать директорию и файлы с изображениями.
        /// </summary>
        private async Task CreateDirImage(List<Uri> uriImages)
        {
            string path = @"C:\Images";
            string fileName = "image";
            int countImage = 0;

            DirectoryInfo dirInfo = new DirectoryInfo(path);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            foreach (var uriImage in uriImages)
            {
                ++countImage;
               await DownloadImageAsync(path, fileName + countImage.ToString(), uriImage);
            }

            CreateFileSaveImages(uriImages, path+@"\saveImage.txt");
        }

        /// <summary>
        /// Создать фалйл с адресами изображений.
        /// </summary>
        private void CreateFileSaveImages(List<Uri> uriImages, string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.Default))
                {
                    int countImages = 0;

                    foreach (var urlImage in uriImages)
                    {
                        writer.WriteLine($"{++countImages} {urlImage}");
                    }
                }
            }
            catch(Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
