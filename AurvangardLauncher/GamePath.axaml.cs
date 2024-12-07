using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
//using Microsoft.Toolkit.Uwp.Notifications;

namespace AurvangardLauncher
{
    public partial class GamePath : Window
    {
        private CancellationTokenSource _cancellationTokenSource;

        public GamePath()
        {
            InitializeComponent();
            Additions.WindowCenter(this);
            if (Additions.TempFileExist())
                PathTextBox.Text = Additions.TempFileRead();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Выберите путь установки"
            };

            if (await dialog.ShowAsync(this) is string folderPath)
            {
                PathTextBox.Text = folderPath;
                CheckInstallPath(folderPath);
            }
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            var path = PathTextBox.Text;
            if (!string.IsNullOrEmpty(path))
            {
                InstallAurvangard(path);
            }
        }

        private async void Reinstall_Click(object sender, RoutedEventArgs e)
        {
            var path = PathTextBox.Text;
            if (!string.IsNullOrEmpty(path))
            {
                ReinstallAurvangard(path);
                Close();
            }
        }

        private void CheckInstallPath(string path)
        {
            if (File.Exists(path + "\\" + "valheim.exe"))
            {
                Description.Text = "Valheim.exe найден";
                Description.Foreground = Avalonia.Media.Brushes.Green;
            }
            else
            {
                Description.Text = "Valheim.exe не найден. Пожалуйста укажите папку с установленным Valheim";
                Description.Foreground = Avalonia.Media.Brushes.Red;
            }
        }

        private async void InstallAurvangard(string path)
        {
            try
            {
                await DownloadAndInstallAsync(path, _cancellationTokenSource.Token);
                ShowNotification("Aurvangard установлен");
                Close();
            }
            catch (OperationCanceledException)
            {
                ShowNotification("Установка отменена");
            }
            catch (Exception ex)
            {
                ShowNotification($"Ошибка установки: {ex.Message}");
            }
        }

        private async Task DownloadAndInstallAsync(string path, CancellationToken cancellationToken)
        {
            string zipPath = Path.Combine(path, "build.zip");

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(Additions.BuildUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();
                    long totalBytes = response.Content.Headers.ContentLength ?? 0;

                    using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write))
                    {
                        using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken))
                        {
                            long bytesRead = 0;
                            byte[] buffer = new byte[8192];
                            int readBytes;
                            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                            while ((readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, readBytes, cancellationToken);
                                bytesRead += readBytes;

                                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                                double speed = elapsedSeconds > 0 ? bytesRead / elapsedSeconds : 0;

                                UpdateDownloadSpeed(speed);
                                UpdateProgressBar(bytesRead, totalBytes);
                            }
                        }
                    }
                }

                ZipFile.ExtractToDirectory(zipPath, path, true);
                Additions.CreateAndWriteTempFile(path);
                File.Delete(zipPath);
            }
        }

        private void ReinstallAurvangard(string path)
        {
            Additions.CheckAndDeleteFiles(path);
            ShowNotification("Установка Aurvangard очищена");
        }

        private void UpdateDownloadSpeed(double speed)
        {
            DownloadSpeed.Text = $"Скорость: {speed / 1024 / 1024:F2} МБ/с";
        }

        private void UpdateProgressBar(long bytesRead, long totalBytes)
        {
            double progress = totalBytes > 0 ? (double)bytesRead / totalBytes * 100 : 0;
            DownloadProgressBar.Value = progress;
        }

        private void ShowNotification(string message)
        {
            /*
            new ToastContentBuilder()
                .AddText(message)
                .Show();
            */
        }
    }
}