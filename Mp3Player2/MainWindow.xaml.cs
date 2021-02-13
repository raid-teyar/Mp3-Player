using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace Mp3Player
{

    public partial class MainWindow : Window
    {



        static public double volume = 0.5;



        static MediaPlayer mediaPlayer = new MediaPlayer();



       


        public MainWindow()
        {


            InitializeComponent();

            musicPosition.ApplyTemplate();

            Thumb thumb = (musicPosition.Template.FindName("PART_Track", musicPosition) as Track).Thumb;

            thumb.MouseEnter += new MouseEventHandler(thumb_MouseEnter);

            mediaPlayer.Volume = volume;

            


          
        }


        private void thumb_MouseEnter(object sender,MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.MouseDevice.Captured == null)

            {     
                MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left);

                args.RoutedEvent = MouseLeftButtonDownEvent;

                (sender as Thumb).RaiseEvent(args);

            }

        }

        public int MusicPosition
        {
            get { return (int)GetValue(MusicPositionProperty); }
            set { SetValue(MusicPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MusicPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MusicPositionProperty =
            DependencyProperty.Register("MusicPosition", typeof(int), typeof(MainWindow), new PropertyMetadata((int)mediaPlayer.Position.TotalSeconds));



        public string MusicName
        {
            get { return (string)GetValue(MusicNameProperty); }
            set { SetValue(MusicNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MusicName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MusicNameProperty =
            DependencyProperty.Register("MusicName", typeof(string), typeof(MainWindow), new PropertyMetadata("Choose a song to start."));




        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volume.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(MainWindow), new PropertyMetadata(volume));

        private TimeSpan TotalTime;
        private void OpenFolder(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Audio files|*.avi;*.mp3;*.wav";

            if (openFile.ShowDialog() == true)
            {
                TagLib.File f = TagLib.File.Create(openFile.FileName);
                TotalTime = f.Properties.Duration;
                DispatcherTimer timerVideoTime = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1000)
                };
                timerVideoTime.Tick += new EventHandler(timer_Tick);
                timerVideoTime.Start();
                void timer_Tick(object senderr, EventArgs eventArgs)
                {


                    textposition.Text = mediaPlayer.Position.ToString(@"hh\:mm\:ss");


                    musicPosition.Value = mediaPlayer.Position.TotalSeconds;


                }





                musicPosition.Maximum = (int)f.Properties.Duration.TotalSeconds;
                musicLenght.Text = f.Properties.Duration.ToString(@"hh\:mm\:ss");
                pauseBtn.IsEnabled = true;
                string filename = System.IO.Path.GetFileNameWithoutExtension(openFile.FileName);
                musicName.Text = filename;
                mediaPlayer.Open(new Uri(openFile.FileName));
                mediaPlayer.Play();

            }
        }

        private void Pause(object sender, RoutedEventArgs e)
        {

            mediaPlayer.Pause();
            pauseBtn.Visibility = Visibility.Hidden;
            pauseBtn.IsEnabled = false;
            playBtn.Visibility = Visibility.Visible;
            playBtn.IsEnabled = true;
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            pauseBtn.Visibility = Visibility.Visible;
            pauseBtn.IsEnabled = true;
            playBtn.Visibility = Visibility.Hidden;
            playBtn.IsEnabled = false;
        }





        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = volumeSlider.Value;

        }



        private void musicPosition_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(musicPosition.Value);
        }
    }
}
