using Avalonia;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using System.Linq;
using AurvangardLauncher.Model;
using Tmds.DBus.Protocol;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace AurvangardLauncher
{
    public partial class NewsDetails : Window
    {
        private News News { get; }

        public NewsDetails(News news)
        {
            this.News = news;
            InitializeComponent();
            Additions.WindowCenter(this);
            Name.Text = news.Name;
            Description.Text = news.Description;

            this.KeyDown += SoftwareDetailWindow_KeyDown;
        }

        private void SoftwareDetailWindow_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Escape)
            {
                this.Close();
            }
        }

        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            
        }

    }
}