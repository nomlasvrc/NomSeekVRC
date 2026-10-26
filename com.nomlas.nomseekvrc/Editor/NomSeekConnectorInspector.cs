using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nomlas.NomSeekVRC.Editor
{
    internal static class NomSeekConnectorInspector
    {
        private static readonly NomSeekConnectorSetup[] Setups = TypeCache.GetTypesDerivedFrom<NomSeekConnectorSetup>()
            .Where(type => !type.IsAbstract && !type.ContainsGenericParameters)
            .Select(type => (NomSeekConnectorSetup)Activator.CreateInstance(type))
            .OrderBy(setup => setup.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        internal static void Draw(SerializedProperty connector, Transform parent)
        {
            EditorGUILayout.LabelField("Connectors", EditorStyles.boldLabel);
            if (connector.objectReferenceValue == null)
            {
                if (Setups.Length == 0)
                {
                    EditorGUILayout.HelpBox("利用可能なコネクターがありません。VCCまたはALCOMでインポートしてください。", MessageType.Warning);
                    return;
                }
                foreach (var setup in Setups)
                {
                    setup.DrawSetupGUI(connector, parent);
                    if (connector.objectReferenceValue != null) break;
                }
                if (connector.objectReferenceValue == null)
                    EditorGUILayout.HelpBox("コネクターをシーン上に配置し、適切な連携設定を行った後、「Connector」欄に指定してください。", MessageType.Info);
            }
            var assignedConnector = connector.objectReferenceValue as NomSeekConnector;
            if (assignedConnector != null && !assignedConnector.IsValid)
                EditorGUILayout.HelpBox("指定されたコネクターが正しく設定されていません。", MessageType.Warning);
        }
    }
}
