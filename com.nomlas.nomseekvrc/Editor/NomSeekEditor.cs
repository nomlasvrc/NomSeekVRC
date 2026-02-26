using System.IO;
using UnityEditor;
using UnityEngine;

namespace Nomlas.NomSeekVRC.Editor
{
    [CustomEditor(typeof(NomSeek))]
    public class NomSeekEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var nomSeek = target as NomSeek;
            DrawDefaultInspector();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("// 設定アシスタント", EditorStyles.boldLabel);

            if (nomSeek == null) return;

            if (nomSeek.vrcurlSetter == null)
            {
                string prefabPath = $"{NomSeekPrefabCreatorWindow.OutputDirectory}/{NomSeekPrefabCreatorWindow.PrefabName}.prefab";
                if (File.Exists(prefabPath))
                {
                    EditorGUILayout.HelpBox("作成したVRCURLSetter.prefabをシーン上に配置し、指定してください。", MessageType.Info);
                    if (GUILayout.Button("Prefabを自動設定"))
                    {
                        Object prefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
                        if (prefab != null)
                        {
                            Undo.RecordObject(nomSeek, "Assign VRCURLSetter Prefab");
                            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, nomSeek.gameObject.transform);
                            nomSeek.vrcurlSetter = instance.GetComponent<VRCURLSetter>();
                            EditorUtility.SetDirty(nomSeek);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("VRCURLSetter.prefabが見つかりません。Prefab Creatorで作成してください。", MessageType.Info);

                    if (GUILayout.Button("Prefab Creatorを開く"))
                    {
                        NomSeekPrefabCreatorWindow.OpenWindow();
                    }
                }
            }

            DrawConnectorInspector(nomSeek);
        }

        private void DrawConnectorInspector(NomSeek nomSeek)
        {
            bool iwaSync = 
            #if IWASYNC_CONNECTOR
            true
            #else
            false
            #endif
            ;

            bool vizVid =
            #if VIZVID_CONNECTOR
            true
            #else
            false
            #endif
            ;

            bool yamaPlayer =
            #if YAMA_CONNECTOR
            true
            #else
            false
            #endif
            ;

            string iwaSyncPath = "Packages/com.nomlas.nomseekvrc.iwasync/Runtime/iwaSyncConnector.prefab";
            string vizVidPath = "Packages/com.nomlas.nomseekvrc.vizvid/Runtime/VizVidConnector.prefab";
            string yamaPlayerPath = "Packages/com.nomlas.nomseekvrc.yamaplayer/Runtime/YamaPlayerConnector.prefab";

            EditorGUILayout.LabelField("Connectors", EditorStyles.boldLabel);
            if (iwaSync && GUILayout.Button("iwaSync Connector")) EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(iwaSyncPath));
            if (vizVid && GUILayout.Button("VizVid Connector")) EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(vizVidPath));
            if (yamaPlayer && GUILayout.Button("YamaPlayer Connector")) EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(yamaPlayerPath));
            EditorGUILayout.HelpBox("コネクターをシーン上に配置し、適切な連携設定を行った後、「Connector」欄に指定してください。", MessageType.Info);
        }
    }
}