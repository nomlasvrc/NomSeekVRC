using UnityEditor;
using UnityEngine;

namespace Nomlas.NomSeekVRC.Editor
{
    // Implement in each connector's Editor assembly. The core discovers implementations automatically.
    public abstract class NomSeekConnectorSetup
    {
        public abstract string DisplayName { get; }
        public abstract void DrawSetupGUI(SerializedProperty connector, Transform parent);

        protected void DrawPrefabButton<T>(SerializedProperty connector, Transform parent, string prefabPath) where T : NomSeekConnector
        {
            if (!GUILayout.Button(DisplayName)) return;

            // Only reuse this connector type, including inactive children.
            var component = parent.GetComponentInChildren<T>(true);
            GameObject prefab = null;
            if (component == null)
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null || prefab.GetComponent<T>() == null)
                {
                    EditorUtility.DisplayDialog("Error", $"{DisplayName} のPrefabが見つからないか、必要なコンポーネントがありません。", "OK");
                    return;
                }
            }

            // Commit pending Inspector edits before grouping creation and assignment for Undo.
            connector.serializedObject.ApplyModifiedProperties();
            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName($"Place {DisplayName}");
            if (component == null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                Undo.RegisterCreatedObjectUndo(instance, $"Place {DisplayName}");
                component = instance.GetComponent<T>();
            }
            connector.objectReferenceValue = component;
            connector.serializedObject.ApplyModifiedProperties();
            Undo.CollapseUndoOperations(undoGroup);
        }
    }
}
