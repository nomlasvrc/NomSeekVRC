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

            NomSeekConnectorInspector.Draw(serializedObject.FindProperty("connector"), nomSeek.transform);

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
                Undo.RegisterCreatedObjectUndo(instance, $"Place {typeof(T).Name} Prefab");
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