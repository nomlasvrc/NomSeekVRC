using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using JLChnToZ.VRC.Foundation;
using JLChnToZ.VRC.Foundation.I18N;
using JLChnToZ.VRC.VVMW;

namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VizVidConnector : NomSeekConnector
    {
        [SerializeField, Locatable, LocalizedLabel(Key = "VVMW.Handler")] FrontendHandler handler;

        public override void ChangeURL(VRCUrl vrcUrl, string title)
        {
            byte index = 1;
            if (Utilities.IsValid(handler)) handler.PlayUrl(vrcUrl, null, title, index);
        }

        public override bool IsValid => Utilities.IsValid(handler);
    }
}