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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для VideoView.xaml
    /// </summary>
    public partial class VideoView : UserControl
    {
        public VideoView()
        {
            InitializeComponent();
        }

        private DispatcherTimer timerVideoTime;

        public bool IsPlaying = false;
        public bool SliderIsDragging = false;

        // Create the timer and otherwise get ready.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(0.1);
            timerVideoTime.Tick += new EventHandler(timer_Tick);
            Player.Stop();
            IsPlaying = false;
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            sbarPosition.Minimum = 0;
            sbarPosition.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
           // sbarPosition.TickFrequency = Player.NaturalDuration.TimeSpan.TotalSeconds / 10;
            //sbarPosition.Visibility = Visibility.Visible;
        }

        // Show the play position in the ScrollBar and TextBox.
        private void ShowPosition()
        {
            
            sbarPosition.Value = Player.Position.TotalSeconds;
            txtPosition.Text = TimeSpan.FromSeconds(sbarPosition.Value).ToString(@"hh\:mm\:ss");//Player.Position.TotalSeconds.ToString("hh:mm:ss");
        }

        // Enable and disable appropriate buttons.
        private void EnableButtons(bool is_playing)
        {
            //if (is_playing)
            //{
            //    btnPlay.IsEnabled = false;
            //    btnPause.IsEnabled = true;
            //    btnPlay.Opacity = 0.5;
            //    btnPause.Opacity = 1.0;
            //}
            //else
            //{
            //    btnPlay.IsEnabled = true;
            //    btnPause.IsEnabled = false;
            //    btnPlay.Opacity = 1.0;
            //    btnPause.Opacity = 0.5;
            //}
            IsPlaying = is_playing;
            timerVideoTime.IsEnabled = is_playing;
            btnPlay.IsChecked = is_playing;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (IsPlaying == true) { 
                Player.Pause();
                IsPlaying = false;
                
            }
            else { 
                Player.Play();
                IsPlaying = true;
            }
            timerVideoTime.IsEnabled = IsPlaying;
            //EnableButtons();
        }
        //private void btnPause_Click(object sender, RoutedEventArgs e)
        //{
        //    Player.Pause();
        //    EnableButtons(false);
        //}
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            EnableButtons(false);
            ShowPosition();
        }
        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            Player.Play();
            EnableButtons(true);
        }
        private void btnFaster_Click(object sender, RoutedEventArgs e)
        {
            Player.SpeedRatio *= 1.5;
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Player.Position += TimeSpan.FromSeconds(10);
            ShowPosition();
        }
        private void btnSlower_Click(object sender, RoutedEventArgs e)
        {
            Player.SpeedRatio /= 1.5;
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            Player.Position -= TimeSpan.FromSeconds(10);
            ShowPosition();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!SliderIsDragging) { 
            ShowPosition();}
        }

        //private void btnSetPosition_Click(object sender, RoutedEventArgs e)
        //{
        //    TimeSpan timespan = TimeSpan.FromSeconds(double.Parse(txtPosition.Text));
        //    Player.Position = timespan;
        //    ShowPosition();
        //}

        private void sbarPosition_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            SliderIsDragging = false;            
            Player.Position = TimeSpan.FromSeconds(sbarPosition.Value);
            
        }

        private void sbarPosition_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            SliderIsDragging = true;
        }
    }
}
