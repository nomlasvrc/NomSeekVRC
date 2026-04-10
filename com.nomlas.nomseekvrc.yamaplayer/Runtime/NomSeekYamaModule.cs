
using UdonSharp;
using UnityEngine;
using Yamadev.YamaStream;

namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class NomSeekYamaModule : YamaPlayerModule
    {
        [SerializeField] private VideoPlayerType videoPlayerType = VideoPlayerType.AVProVideoPlayer;

        
        public VideoPlayerType VideoPlayerType => videoPlayerType;
        public QueueList QueueList => _controller.Queue;
    }
}