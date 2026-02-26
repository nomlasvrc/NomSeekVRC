
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

using Yamadev.YamaStream;

namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class YamaPlayerConnector : NomSeekConnector
    {
        [SerializeField] private Controller yamaController;
        [SerializeField] private VideoPlayerType videoPlayerType = VideoPlayerType.AVProVideoPlayer;
        public override void ChangeURL(VRCUrl vrcUrl, string title)
        {
            var track = Track.New(videoPlayerType, title, vrcUrl);
            if (yamaController.Stopped && !yamaController.IsLoading)
            {
                yamaController.TakeOwnership();
                yamaController.PlayTrack(track);
            }
            else
            {
                yamaController.Queue.TakeOwnership();
                yamaController.Queue.AddTrack(track);
            }
        }

        public override bool IsValid => yamaController != null;
    }
}
