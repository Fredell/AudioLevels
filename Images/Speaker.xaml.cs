using System.Windows;
using AudioLevels.Interfaces;

namespace AudioLevels.Images
{
    /// <summary>
    ///     Interaction logic for Speaker.xaml
    /// </summary>
    public partial class Speaker : IVolumeAdjustment
    {
        public Speaker()
        {
            InitializeComponent();
        }

        public void UpdateUi(double volumePercentage)
        {
            Volume1.Visibility = volumePercentage > 0f ? Visibility.Visible : Visibility.Hidden;
            Volume2.Visibility = volumePercentage >= 33f ? Visibility.Visible : Visibility.Hidden;
            Volume3.Visibility = volumePercentage >= 66f ? Visibility.Visible : Visibility.Hidden;
        }
    }
}