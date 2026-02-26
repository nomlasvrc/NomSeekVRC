
using HoshinoLabs.IwaSync3.Udon;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
#endif

namespace Nomlas.NomSeekVRC
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iwaSyncConnector : NomSeekConnector
    {
        [SerializeField] VideoCore core;

        public override void ChangeURL(VRCUrl vrcUrl, string title)
        {
            Debug.Log($"[<color=#47F1FF>NomSeek For VRC iwaSync3 connector</color>] The url has changed to `{vrcUrl}`.");
            uint _mode =
#if COMPILER_UDONSHARP
            core.MODE_VIDEO
#else
            VideoCore.MODE_VIDEO
#endif
;

            core.TakeOwnership();
            core.PlayURL(_mode, vrcUrl);
            core.RequestSerialization();
        }

        public override bool IsValid => core != null;
    }

#if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(iwaSyncConnector))]
    public class iwaSyncConnectorEditor : Editor
    {
        private const string queueListDocURL = "https://docs.google.com/document/d/1AOMawwq9suEgfa0iLCUX4MRhOiSLBNCLvPCnqW9yQ3g/edit?tab=t.0#heading=h.3mxq2mm4df49";
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("キューリストと連携して再生したい場合は、iwaSync側でQueuelistを登録してください。\n詳しくはドキュメントを参照してください。", MessageType.Info);
            if (GUILayout.Button("ドキュメントを開く"))
            {
                Application.OpenURL(queueListDocURL);
            }

        }
    }
#endif
}