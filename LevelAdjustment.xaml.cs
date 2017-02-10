using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AudioLevels.Classes;
using AudioLevels.Images;
using AudioLevels.Interfaces;
using NAudio.CoreAudioApi;

namespace AudioLevels
{
    /// <summary>
    ///     Interaction logic for LevelAdjustment.xaml
    /// </summary>
    public partial class LevelAdjustment
    {
        public LevelAdjustment(MMDevice renderer, AdjustmentsType type)
        {
            Renderer = renderer;
            Type = type;
            InitializeComponent();
        }

        private MMDevice Renderer { get; }
        private AdjustmentsType Type { get; }
        private double CurrentSliderPercentage { get; set; }
        private UserControl CurrentImage { get; set; }

        private void lblPercentage_Loaded(object sender, RoutedEventArgs e)
        {
            SetVolumeText();
        }

        private void SetVolumeText()
        {
            var max = Renderer.AudioEndpointVolume.VolumeRange.MaxDecibels -
                      Renderer.AudioEndpointVolume.VolumeRange.MinDecibels;
            var range = max;
            var currentValue = Slider.Value - Renderer.AudioEndpointVolume.VolumeRange.MinDecibels;
            CurrentSliderPercentage = Math.Round(currentValue/range*100);
            LblPercentage.Content = CurrentSliderPercentage;

            // Update image UI if available
            (CurrentImage as IVolumeAdjustment)?.UpdateUi(CurrentSliderPercentage);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolumeText();
            Renderer.AudioEndpointVolume.MasterVolumeLevel = (float) Slider.Value;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Type == AdjustmentsType.Playback)
                CurrentImage = new Speaker();
            else if (Type == AdjustmentsType.Recording)
                CurrentImage = new Microphone();

            var result = CurrentImage;
            var group = new TransformGroup();
            group.Children.Add(new ScaleTransform(1, 1));
            result.LayoutTransform = group;
            result.HorizontalAlignment = HorizontalAlignment.Left;
            LevelAdjustments.Children.Add(result);

            Slider.Minimum = Renderer.AudioEndpointVolume.VolumeRange.MinDecibels;
            Slider.Maximum = Renderer.AudioEndpointVolume.VolumeRange.MaxDecibels;
            Slider.Value = Renderer.AudioEndpointVolume.MasterVolumeLevel;
            Slider.ValueChanged += slider_ValueChanged;
        }
    }
}