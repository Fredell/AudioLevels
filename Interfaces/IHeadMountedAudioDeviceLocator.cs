using System.Collections.Generic;
using AudioLevels.Classes;
using NAudio.CoreAudioApi;

namespace AudioLevels.Interfaces
{
    public interface IHeadMountedAudioDeviceLocator
    {
        /// <summary>
        ///     Locate all connected audio devices based on the given state.
        /// </summary>
        /// <param name="deviceState"></param>
        /// <returns></returns>
        IReadOnlyCollection<AudioDevice> LocateConnectedAudioDevices(DeviceState deviceState = DeviceState.Active);
    }
}