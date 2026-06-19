
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using Yamadev.YamaStream;

namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class NomSeekYamaModule : YamaPlayerModule
    {
        [SerializeField] private VideoPlayerType videoPlayerType = VideoPlayerType.AVProVideoPlayer;

        internal void PlayOrAddQueue(VRCUrl vrcUrl, string title)
        {
            var track = TrackUtils.NewTrack(videoPlayerType, title, vrcUrl);
            if (_controller.IsPlaying)
            {
                _controller.Queue.TakeOwnership();
                _controller.Queue.AddTrack(track);
            }
            else
            {
                _controller.TakeOwnership();
                _controller.PlayTrack(track);
            }
        }
    }
}