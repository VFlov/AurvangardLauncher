using Avalonia;
using Avalonia.Controls;
using System.Collections.Generic;
using AurvangardLauncher.Model;
using Avalonia.Media;
using System.Diagnostics;

namespace AurvangardLauncher
{
    public partial class MainWindow : Window
    {
        private List<News> NewsList;
        private WrapPanel _CardWrapPanel;
        private bool showCards = false;
        public MainWindow()
        {
            InitializeComponent();
            
            Additions.WindowCenter(this);
            _CardWrapPanel = this.FindControl<WrapPanel>("CardWrapPanel");
            _CardWrapPanel.IsVisible = false;
            if (Additions.TempFileExist())
            {
                Additions.PathToGame = Additions.TempFileRead();
                PlayText.Text = "Играть";
            }

            InitializeCardsAsync();
            AddSoftware();
        }

        private void InitializeCardsAsync()
        {
            NewsList = Additions.GetNews();
        }

        private void AddSoftware()
        {
            CardWrapPanel.Children.Clear();

            foreach (var news in NewsList)
            {
                var border = new Border
                {
                    Padding = new Thickness(0),
                    BorderThickness = new Thickness(1),
                    Width = 145,
                    Height = 50,
                    Margin = new Thickness(0, 10, 0, 0),
                    Background = Brushes.Black,
                    BorderBrush = Brushes.WhiteSmoke
                };

                var stackPanel = new StackPanel();
                border.Child = stackPanel;

                var textBlock = new TextBlock
                {
                    Text = news.Name,
                    FontSize = 12,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                    Margin = new Thickness(10, 0, 0, 0),
                    Foreground = Brushes.WhiteSmoke
                };

                stackPanel.Children.Add(textBlock);

                border.Tapped += (sender, e) =>
                {
                    var detailWindow = new NewsDetails(news);
                    detailWindow.Show();
                };

                CardWrapPanel.Children.Add(border);
            }
        }

        private void Play_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Additions.TempFileExist())
            {
                Process.Start(Additions.PathToGame + "\\" + "Valheim.exe");
                Close();
                return;
            }

            var detailWindow = new GamePath();
            detailWindow.Show();
        }

        private void Options_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var detailWindow = new GamePath();
            detailWindow.Show();
        }
        private void News_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            showCards = !showCards; // Переключение видимости
            _CardWrapPanel.IsVisible = showCards;
        }
    }
}