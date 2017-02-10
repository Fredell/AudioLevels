using NAudio.CoreAudioApi;

namespace AudioLevels.Classes
{
    public class Headset : AudioDevice
    {
        public Headset(string headset, string deviceFriendlyName, MMDevice capture, MMDevice render)
            : base(headset, deviceFriendlyName, render)
        {
            Capture = capture;
        }

        public MMDevice Capture { get; set; }
    }
}