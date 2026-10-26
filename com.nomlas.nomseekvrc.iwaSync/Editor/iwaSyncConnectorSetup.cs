using UnityEditor;
using UnityEngine;

namespace Nomlas.NomSeekVRC.Editor
{
    public sealed class iwaSyncConnectorSetup : NomSeekConnectorSetup
    {
        public override string DisplayName => "iwaSync Connector";

        public override void DrawSetupGUI(SerializedProperty connector, Transform parent)
        {
            DrawPrefabButton<iwaSyncConnector>(connector, parent, "Packages/com.nomlas.nomseekvrc.iwasync/Runtime/iwaSyncConnector.prefab");
        }
    }
}
