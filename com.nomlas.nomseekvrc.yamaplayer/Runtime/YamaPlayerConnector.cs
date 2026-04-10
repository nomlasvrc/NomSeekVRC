
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

using Yamadev.YamaStream;

namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(NomSeekYamaModule))]
    public class YamaPlayerConnector : NomSeekConnector
    {
        private VideoPlayerType videoPlayerType;
        private QueueList queueList;

        private void Start()
        {
            var module = GetComponent<NomSeekYamaModule>();
            videoPlayerType = module.VideoPlayerType;
            queueList = module.QueueList;
        }

        public override void ChangeURL(VRCUrl vrcUrl, string title)
        {
            queueList.AddTrack(TrackUtils.NewTrack(videoPlayerType, title, vrcUrl));
        }

        public override bool IsValid => true;
    }
}
