using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AudioLevels.Classes;

namespace AudioLevels
{
    /// <summary>
    ///     Interaction logic for LevelAdjustmentCluster.xaml
    /// </summary>
    public partial class LevelAdjustmentCluster
    {
        public LevelAdjustmentCluster(AudioDevice device)
        {
            InitializeComponent();
            Device = device;
            LblHeader.Content = device.DeviceFriendlyName;
        }

        private AudioDevice Device { get; }

        private void lblHeader_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindowCol = Color.FromArgb((byte) (0.85*255), 31, 31, 31);
            //   var lighter = Color.FromArgb((byte)(mainWindowCol.A * 1.15), mainWindowCol.R, mainWindowCol.G, mainWindowCol.B);
            var darker = Color.FromArgb((byte) (mainWindowCol.A*0.85), mainWindowCol.R, mainWindowCol.G, mainWindowCol.B);


            // Add bottom border
            var rect = new Rectangle
            {
                Fill = new SolidColorBrush(darker),
                Height = 1
            };
            DockPanel.SetDock(rect, Dock.Top);
            Levels.Children.Insert(0, rect);

            if (Device.Render != null)
            {
                Levels.Children.Add(new LevelAdjustment(Device.Render, AdjustmentsType.Playback));
            }
            var device = Device as Headset;
            if (device != null)
                Levels.Children.Add(new LevelAdjustment(device.Capture, AdjustmentsType.Recording));
        }
    }
}