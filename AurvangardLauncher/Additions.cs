using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AurvangardLauncher.Model;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
//using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Devices.PointOfService;
using Windows.Media.Devices;

namespace AurvangardLauncher
{
    public static class Additions
    {
        public static int Percent = 0;
        public static string PathToGame;
        public static string BuildUrl = "http://45.130.214.139:8080/api/public/dl/QRjvAuxT/BepInEx.zip";
        private static string NewsUrl = "http://45.130.214.139:8080/api/public/dl/UyAQHt5v/News.json";
        private static string[] FileNames = { "winhttp.dll", "start_server_bepinex.sh", "start_game_bepinex.sh", "doorstop_config.ini", "changelog.txt" };
        private static string[] DirectoryNames = { "doorstop_libs", "BepInEx" };

        public static async void DownloadAndInstallAsync(string path)
        {

            string zipPath = Path.Combine(path, "build.zip");
            using (var client = new WebClient())
            {
                //client.DownloadProgressChanged += 

                client.DownloadFileAsync(new System.Uri(Additions.BuildUrl), zipPath);

                while (client.IsBusy)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            ZipFile.ExtractToDirectory(zipPath, path);
            Additions.CreateAndWriteTempFile(path);
            /*
            new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText("Aurvangard установлен")
            .Show();
            */
            Percent = 110;
        }
        public static List<News> GetNews()
        {
            using (HttpClient client = new HttpClient())
            {
                return client.GetFromJsonAsync<List<News>>(NewsUrl).Result;
            }
        }
        public static async Task<Bitmap> ImageDownload(string url)
        {
            if (url.Contains(".svg"))
                return null;
            using (HttpClient httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var byteArray = await response.Content.ReadAsByteArrayAsync();
                    using (var stream = new MemoryStream(byteArray))
                    {
                        return new Bitmap(stream);
                    }
                }
            }
            return null;
        }
        public static string TempFileRead()
        {
            string tempFilePath = Path.GetTempPath() + "Aurvangard.txt";
            return File.ReadAllText(tempFilePath);
        }

        public static bool TempFileExist()
        {
            string tempFilePath = Path.GetTempPath() + "Aurvangard.txt";
            return File.Exists(tempFilePath);
        }
        public static void TempFileDelete()
        {
            string tempFilePath = Path.GetTempPath() + "Aurvangard.txt";
            if (TempFileExist())
                File.Delete(tempFilePath);
        }

        public static void CreateAndWriteTempFile(string text)
        {
            string tempFilePath = Path.GetTempPath() + "Aurvangard.txt";
            //if (!File.Exists(tempFilePath))
            //    File.Create(tempFilePath);
            File.WriteAllText(tempFilePath, text);
            Additions.PathToGame = text;
        }

        public static void WindowCenter(Window someWindow)
        {
            var screen = someWindow.Screens.Primary!.WorkingArea;
            var windowWidth = someWindow.Width;
            var windowHeight = someWindow.Height;
            if (windowWidth <= 0 || windowHeight <= 0)
            {
                windowWidth = 300;
                windowHeight = 200;
            }
            double x = (screen.Width - windowWidth) / 2;
            double y = (screen.Height - windowHeight) / 2;
            someWindow.Position = new PixelPoint((int)x, (int)y);
        }
        public static void CheckAndDeleteFiles(string gamePath)
        {

            if (TempFileExist())
            {
                TempFileDelete();
            }
            foreach (var file in FileNames)
            {
                if (File.Exists(gamePath + "\\" + file))
                    File.Delete(gamePath + "\\" + file);
            }
            foreach (var directory in DirectoryNames)
            {
                if (Directory.Exists(gamePath + "\\" + directory))
                    Directory.Delete(gamePath + "\\" + directory, true);
            }
        }


    }
}
