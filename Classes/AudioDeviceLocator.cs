using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AudioLevels.Interfaces;
using Microsoft.Win32;
using NAudio.CoreAudioApi;

namespace AudioLevels.Classes
{
    public class AudioDeviceLocator : IHeadMountedAudioDeviceLocator
    {
        /// <summary>
        ///     Locate all connected audio devices based on the given state.
        /// </summary>
        /// <param name="deviceState"></param>
        /// <returns></returns>
        public IReadOnlyCollection<AudioDevice> LocateConnectedAudioDevices(DeviceState deviceState = DeviceState.Active)
        {
            var enumerator = new MMDeviceEnumerator();
            var relatedAudioDevices = new ConcurrentDictionary<string, List<MMDevice>>();
            var audioDevices = new List<AudioDevice>();

            // Locate all connected audio devices.
            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.All, deviceState))
            {
                // Gets the DataFlow and DeviceID from the connected audio device.
                var index = device.ID.LastIndexOf('.');
                var dataFlow = device.ID.Substring(0, index).Contains("0.0.0") ? DataFlow.Render : DataFlow.Capture;
                var deviceId = device.ID.Substring(index + 1);

                // Gets the unique hardware token.
                var hardwareToken = GetHardwareToken(dataFlow, deviceId);

                var audioDevice = relatedAudioDevices.GetOrAdd(hardwareToken, o => new List<MMDevice>());
                audioDevice.Add(device);
            }

            // Combines the related devices into a headset object.
            foreach (var devicePair in relatedAudioDevices)
            {
                var capture = devicePair.Value.FirstOrDefault(o => o.ID.Contains("0.0.1"));
                var render = devicePair.Value.FirstOrDefault(o => o.ID.Contains("0.0.0"));

                if (capture != null && render != null)
                {
                    audioDevices.Add(new Headset("Headset", render.DeviceFriendlyName, capture, render));
                }
                else if (render != null)
                {
                    audioDevices.Add(new AudioDevice("Headset", render.DeviceFriendlyName, render));
                }
            }

            return new ReadOnlyCollection<AudioDevice>(audioDevices);
        }

        /// <summary>
        ///     Gets the token of the USB device.
        /// </summary>
        /// <param name="dataFlow"></param>
        /// <param name="audioDeviceId"></param>
        /// <returns></returns>
        public string GetHardwareToken(DataFlow dataFlow, string audioDeviceId)
        {
            using (var registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                var captureKey =
                    registryKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\" + dataFlow +
                                           @"\" + audioDeviceId + @"\Properties");
                if (captureKey != null)
                {
                    return captureKey.GetValue("{b3f8fa53-0004-438e-9003-51a46e139bfc},2") as string;
                }
            }

            return null;
        }
    }
}