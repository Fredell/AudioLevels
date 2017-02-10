using NAudio.CoreAudioApi;

namespace AudioLevels.Classes
{
    public class AudioDevice
    {
        public AudioDevice(string headset, string deviceFriendlyName, MMDevice render)
        {
            HeadsetId = headset;
            DeviceFriendlyName = deviceFriendlyName;
            Render = render;
        }

        public string HeadsetId { get; set; }
        public string DeviceFriendlyName { get; set; }
        public MMDevice Render { get; set; }
    }
}