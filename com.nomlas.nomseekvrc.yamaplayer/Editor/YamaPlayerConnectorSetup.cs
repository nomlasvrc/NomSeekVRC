using UnityEditor;
using UnityEngine;

namespace Nomlas.NomSeekVRC.Editor
{
    public sealed class YamaPlayerConnectorSetup : NomSeekConnectorSetup
    {
        public override string DisplayName => "YamaPlayer Connector";

        public override void DrawSetupGUI(SerializedProperty connector, Transform parent)
        {
            EditorGUILayout.HelpBox("YamaPlayerからモジュールとして追加して設定してください。", MessageType.Info);
            using (new EditorGUI.DisabledScope(true)) GUILayout.Button(DisplayName);//TODO: YamaPlayer本体と連携
        }
    }
}
