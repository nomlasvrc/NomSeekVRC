using System.IO;
using UnityEditor;
using UnityEngine;

namespace Nomlas.NomSeekVRC.Editor
{
    [CustomEditor(typeof(NomSeek))]
    public class NomSeekEditor : UnityEditor.Editor
    {
        private static bool showOtherProperties = false;

        public override void OnInspectorGUI()
        {
            var nomSeek = target as NomSeek;

            serializedObject.Update();

            // vrcurlSetterとconnectorは常に表示
            EditorGUILayout.PropertyField(serializedObject.FindProperty("vrcurlSetter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("connector"));

            EditorGUILayout.Space();

            if (nomSeek == null) return;

            if (nomSeek.vrcurlSetter == null)
            {
                string prefabPath = $"{NomSeekPrefabCreatorWindow.OutputDirectory}/{NomSeekPrefabCreatorWindow.PrefabName}.prefab";
                if (File.Exists(prefabPath))
                {
                    if (GUILayout.Button("VRCURLSetter.prefabを配置"))
                    {
                        nomSeek.vrcurlSetter = LoadAndPlacePrefab<VRCURLSetter>(nomSeek, prefabPath);
                        EditorUtility.SetDirty(nomSeek);
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

            EditorGUILayout.Space();

            // その他のプロパティを折り畳み
            showOtherProperties = EditorGUILayout.BeginFoldoutHeaderGroup(showOtherProperties, "Other Properties");
            if (showOtherProperties)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("content"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("template"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inputField"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("videoHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("material"));
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
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
            if (!iwaSync && !vizVid && !yamaPlayer)
            {
                EditorGUILayout.HelpBox("利用可能なコネクターがありません。VCCまたはALCOMでインポートしてください。", MessageType.Warning);
                return;
            }
            if (nomSeek.connector == null)
            {
                if (iwaSync && GUILayout.Button("iwaSync Connector"))
                {
                    nomSeek.connector = LoadAndPlacePrefab<NomSeekConnector>(nomSeek, iwaSyncPath);
                    EditorUtility.SetDirty(nomSeek);
                }
                if (vizVid && GUILayout.Button("VizVid Connector"))
                {
                    nomSeek.connector = LoadAndPlacePrefab<NomSeekConnector>(nomSeek, vizVidPath);
                    EditorUtility.SetDirty(nomSeek);
                }
                if (yamaPlayer && GUILayout.Button("YamaPlayer Connector"))
                {
                    nomSeek.connector = LoadAndPlacePrefab<NomSeekConnector>(nomSeek, yamaPlayerPath);
                    EditorUtility.SetDirty(nomSeek);
                }
                EditorGUILayout.HelpBox("コネクターをシーン上に配置し、適切な連携設定を行った後、「Connector」欄に指定してください。", MessageType.Info);
            }
            if (nomSeek.connector != null && !nomSeek.connector.IsValid)
            {
                EditorGUILayout.HelpBox("指定されたコネクターが正しく設定されていません。", MessageType.Warning);
            }
        }

        private T LoadAndPlacePrefab<T>(NomSeek nomSeek, string prefabPath) where T : Component
        {
            var comp = nomSeek.GetComponentInChildren<T>();
            if (comp != null)
            {
                EditorUtility.DisplayDialog("Info", $"{typeof(T).Name}はすでに配置されているため、配置済みのものを使用します。", "OK");
                return comp;
            }
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, nomSeek.transform);
                Undo.RegisterCreatedObjectUndo(instance, "Place Connector Prefab");
                return instance.GetComponent<T>();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Prefabが見つかりませんでした。", "OK");
                return null;
            }
        }
    }
}