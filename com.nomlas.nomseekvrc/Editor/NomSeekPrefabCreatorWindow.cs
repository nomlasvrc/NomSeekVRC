
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace Nomlas.NomSeekVRC.Editor
{
    public class NomSeekPrefabCreatorWindow : EditorWindow
    {
        internal const string ConsentKey = "Nomlas.NomSeekVRC.AgreeApiUsage";
        internal const string OutputDirectory = "Assets/Nomlas/NomSeek";
        internal const string PrefabName = "VRCURLSetter";

        private string worldId = string.Empty;
        private int poolSize = 10000;

        [MenuItem("Tools/NomSeekVRC/Prefab Creator")]
        internal static void OpenWindow()
        {
            GetWindow<NomSeekPrefabCreatorWindow>("NomSeek Prefab Creator");
        }

/*
        [MenuItem("Tools/NomSeekVRC/同意状態のリセット")]
        private static void ResetApiConsent() => EditorPrefs.SetBool(ConsentKey, false);
*/

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Prefab Creator", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            bool agreed = EditorPrefs.GetBool(ConsentKey, false);
            if (!agreed)
            {
                DrawConsentSection();
                return;
            }

            worldId = EditorGUILayout.TextField("World ID", worldId);
            EditorGUILayout.HelpBox("APIが識別に使うためのワールド固有のIDを設定してください。\n英子文字・数字・アンダースコア・ハイフンのみ使用可能です", MessageType.None);
            EditorGUILayout.Space();
            poolSize = EditorGUILayout.IntField("Pool Size", poolSize);
            EditorGUILayout.HelpBox("生成するURLの数です。", MessageType.None);

            if (poolSize < 1) poolSize = 1;

            EditorGUILayout.Space();

            bool canCreate = agreed && IsValid(worldId);

            EditorGUI.BeginDisabledGroup(!canCreate);
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

            Selection.activeObject = savedPrefab;
            EditorGUIUtility.PingObject(savedPrefab);
            EditorUtility.DisplayDialog("Success", $"Prefabを保存しました:\n{prefabPath}", "OK");
            DestroyImmediate(go);
        }

        private static void DrawConsentSection()
        {
            EditorGUILayout.LabelField("API 利用同意", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(consentText + "\n以上に同意しますか？", MessageType.Warning);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("同意しない"))
            {
                EditorPrefs.SetBool(ConsentKey, false);
            }
            if (GUILayout.Button("同意する"))
            {
                EditorPrefs.SetBool(ConsentKey, true);
            }
            EditorGUILayout.EndHorizontal();
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

        // 以下の文章を変更してはなりません。
        private const string consentText = "本アセットは、Lamp氏が提供する「VRChat YouTube Search API」を使用しています。\n本アセットおよび当該APIの利用または利用不能に起因して利用者に発生した損害（直接的・間接的・特別・偶発的・結果的損害を含むがこれらに限りません）について、製作者である「のむらす」は一切の責任を負わないものとします。";
    }
}
