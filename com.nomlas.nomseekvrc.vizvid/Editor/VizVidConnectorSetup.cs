using UnityEditor;
using UnityEngine;

namespace Nomlas.NomSeekVRC.Editor
{
    public sealed class VizVidConnectorSetup : NomSeekConnectorSetup
    {
        public override string DisplayName => "VizVid Connector";

        public override void DrawSetupGUI(SerializedProperty connector, Transform parent)
        {
            DrawPrefabButton<VizVidConnector>(connector, parent, "Packages/com.nomlas.nomseekvrc.vizvid/Runtime/VizVidConnector.prefab");
        }
    }
}
