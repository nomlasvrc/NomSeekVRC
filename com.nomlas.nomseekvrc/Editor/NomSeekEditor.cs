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
        }
    }
}