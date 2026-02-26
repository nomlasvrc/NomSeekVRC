
using UdonSharp;

namespace Nomlas.NomSeekVRC
{
    public abstract class NomSeekConnector : UdonSharpBehaviour
    {
        public abstract bool IsValid { get; }
        public abstract void ChangeURL(VRC.SDKBase.VRCUrl vrcUrl, string title);
    }
}