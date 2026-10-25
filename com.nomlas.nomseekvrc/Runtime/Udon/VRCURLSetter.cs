
using System.Runtime.CompilerServices;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[assembly: InternalsVisibleTo("NomSeekVRC.Editor")]
namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VRCURLSetter : UdonSharpBehaviour
    {
        public const string apiEndpoint = "https://y2b.nmls.workers.dev";
        [SerializeField] internal VRCUrl[] urls;
        [SerializeField] internal VRCUrl defaultVRCUrl;
        [SerializeField] internal string needUrl;
        public VRCUrl[] VRCUrls => urls;
        public VRCUrl DefaultVRCUrl => defaultVRCUrl;
        public string NeedUrl => needUrl;
        public bool IsValid()
        {
            return NeedUrl.StartsWith(apiEndpoint) && DefaultVRCUrl.ToString().StartsWith(apiEndpoint) && VRCUrls.Length > 0;
        }
    }
}
