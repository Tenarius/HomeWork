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

        private bool IsAddress(string address)
        {
            if (Uri.IsWellFormedUriString(address, UriKind.Absolute))
            {
                return true;
            }

            Console.WriteLine("Invalid address.");
            return false;
        }

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

        private async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri)
        {
            var httpClient = new HttpClient();

            // Get the file extension
            var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
            var fileExtension = Path.GetExtension(uriWithoutQuery);

            // Create file path and ensure directory exists
            var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
            Directory.CreateDirectory(directoryPath);

            // Download the image and write to the file
            var imageBytes = await httpClient.GetByteArrayAsync(uri);
            File.WriteAllBytes(path, imageBytes);
        }

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
