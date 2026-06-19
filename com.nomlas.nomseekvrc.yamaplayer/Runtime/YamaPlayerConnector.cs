
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(NomSeekYamaModule))]
    public class YamaPlayerConnector : NomSeekConnector
    {
        private NomSeekYamaModule module;

        private void Start()
        {
            module = GetComponent<NomSeekYamaModule>();
        }
        public override void ChangeURL(VRCUrl vrcUrl, string title) => module.PlayOrAddQueue(vrcUrl, title);
        public override bool IsValid => true;
    }
}
