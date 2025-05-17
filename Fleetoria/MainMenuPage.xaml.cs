using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fleetoria
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : PageWithScaling
    {
        public MainMenuPage()
        {
            InitializeComponent();

            WhiteOverlay.Visibility = Visibility.Visible;
            Logo.Visibility = Visibility.Collapsed;
            PlayButton.Visibility = Visibility.Collapsed;
            SettingsButton.Visibility = Visibility.Collapsed;
            ExitButton.Visibility = Visibility.Collapsed;

            FadeOutOverlay();

            Loaded += AnimateAllButtons;
        }

        private void AnimateAllButtons(object sender, RoutedEventArgs e)
        {
            Loaded -= AnimateAllButtons;
            AnimateElement(Logo, delaySeconds: 0.5);
            AnimateElement(PlayButton, delaySeconds: 1);
            AnimateElement(SettingsButton, delaySeconds: 1.5);
            AnimateElement(ExitButton, delaySeconds: 2);
        }

        private void AnimateElement(FrameworkElement element, double delaySeconds)
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            if (element.RenderTransform == null || !(element.RenderTransform is TranslateTransform))
                element.RenderTransform = new TranslateTransform();

            var animation = new DoubleAnimation
            {
                From = screenHeight,
                To = 0,
                Duration = TimeSpan.FromSeconds(1.2),
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseOut }
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));

            var delayTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(delaySeconds)
            };
            delayTimer.Tick += (s, args) =>
            {
                delayTimer.Stop();
                element.Visibility = Visibility.Visible;
                storyboard.Begin();
            };
            delayTimer.Start();
        }
        private void FadeOutOverlay()
        {
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(2),
                BeginTime = TimeSpan.FromSeconds(0.6),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            fadeOut.Completed += (s, e) =>
            {
                WhiteOverlay.Visibility = Visibility.Collapsed;
            };

            WhiteOverlay.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new SelectGameModePage());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new SettingsPage());

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
