using System;
using Wpf.Ui.Common.Interfaces;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using System.Windows.Controls;


namespace car.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<ViewModels.SettingsViewModel>
    {
        public ViewModels.SettingsViewModel ViewModel { get; }

        private DispatcherTimer timer;
        private bool webViewLoaded = false;

        public SettingsPage(ViewModels.SettingsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            webView.Visibility = Visibility.Collapsed;
            timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            if (!webViewLoaded)
            {
                await InitializeWebView();
            }

            webView.Visibility = Visibility.Visible;
        }

        private async Task InitializeWebView()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.NavigationCompleted += WebView_NavigationCompleted;
            webView.CoreWebView2.Navigate("file:///C:/Users/admin/Desktop/temibeszprojekt-main/car/car/car/Views/Pages/index.html");
            webView.ZoomFactor = 0.8f;

        }

        private void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            webViewLoaded = true;
        }

        private void Ellipse_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SetZoomLevel(float zoomLevel)
        {
            webView.ZoomFactor = zoomLevel;
        }

        private void Ellipse_PreviewMouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            }
        }
    }
}
