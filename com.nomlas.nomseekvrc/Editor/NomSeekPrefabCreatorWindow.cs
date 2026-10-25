
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.NomSeekVRC.Editor
{
    public class NomSeekPrefabCreatorWindow : EditorWindow
    {
        internal const string OutputDirectory = "Assets/Nomlas/NomSeek";
        internal const string PrefabName = "VRCURLSetter";

        private string worldId = string.Empty;
        private int poolSize = 10000;

        [MenuItem("Tools/NomSeekVRC/Prefab Creator")]
        internal static void OpenWindow()
        {
            GetWindow<NomSeekPrefabCreatorWindow>("NomSeek Prefab Creator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Prefab Creator", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            worldId = EditorGUILayout.TextField("World ID", worldId);
            EditorGUILayout.HelpBox("APIが識別に使うためのワールド固有のIDを設定してください。\n英子文字・数字・アンダースコア・ハイフンのみ使用可能です", MessageType.None);
            EditorGUILayout.Space();
            poolSize = EditorGUILayout.IntField("Pool Size", poolSize);
            EditorGUILayout.HelpBox("生成するURLの数です。", MessageType.None);

            if (poolSize < 1) poolSize = 1;

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(!IsValid(worldId));
            if (GUILayout.Button("Prefabを作成"))
            {
                CreatePrefab();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void CreatePrefab()
        {
            EnsureFolder(OutputDirectory);

            string prefabPath = $"{OutputDirectory}/{PrefabName}.prefab";

            GameObject go = new GameObject(PrefabName);
            var setter = go.AddComponent<VRCURLSetter>();

            GeneratedUrls(setter, worldId, poolSize);

            bool success;
            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath, out success);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (!success || savedPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Prefab保存に失敗しました。", "OK");
                return;
            }

            Selection.activeObject = savedPrefab; //Udonのセットアップを誘導
            EditorUtility.DisplayDialog("Success", $"Prefabを保存しました:\n{prefabPath}", "OK");
            DestroyImmediate(go);
        }

        private static void GeneratedUrls(VRCURLSetter setter, string worldId, int poolSize)
        {
            string baseUrl = $"{VRCURLSetter.apiEndpoint}/vrcurl/{worldId}{poolSize}/";

            int arraySize = poolSize + 1;
            var urlArray = new VRCUrl[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                urlArray[i] = new VRCUrl($"{baseUrl}{i}");
            }

            setter.urls = urlArray;

            string needUrl = $"{VRCURLSetter.apiEndpoint}/search?pool={worldId}{poolSize}&thumbnails=true&icons=true&mode=latestontop&input=";
            setter.defaultVRCUrl = new VRCUrl(needUrl + "      ここに検索ワードを入力 →                                      ");
            setter.needUrl = needUrl;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string[] split = folderPath.Split('/');
            string current = split[0];
            for (int i = 1; i < split.Length; i++)
            {
                string next = $"{current}/{split[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, split[i]);
                }
                current = next;
            }
        }

        public static bool IsValid(string worldId)
        {
            if (string.IsNullOrWhiteSpace(worldId)) return false;

            const string pattern = @"^[a-z0-9_-]+$";
            return Regex.IsMatch(worldId, pattern);
        }
    }
}
