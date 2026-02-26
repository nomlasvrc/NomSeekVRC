using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Nomlas.NomSeekVRC.Editor
{
    [CustomEditor(typeof(ColorChanger))]
    public class ColorChangerEditor : UnityEditor.Editor
    {
        private SerializedProperty presetProp;
        private SerializedProperty imagesProp;
        private SerializedProperty textsProp;
        private SerializedProperty iconsProp;
        private SerializedProperty imagesColorProp;
        private SerializedProperty textColorProp;

        private int selectedPresetIndex = -1;
        private string[] presetNames;

        private void OnEnable()
        {
            presetProp = serializedObject.FindProperty("preset");
            imagesProp = serializedObject.FindProperty("images");
            textsProp = serializedObject.FindProperty("texts");
            iconsProp = serializedObject.FindProperty("icons");
            imagesColorProp = serializedObject.FindProperty("imagesColor");
            textColorProp = serializedObject.FindProperty("textColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Color Preset", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(presetProp);

            var presetObj = presetProp.objectReferenceValue as ColorPresetList;
            if (presetObj != null && presetObj.presets != null && presetObj.presets.Count > 0)
            {
                if (presetNames == null || presetNames.Length != presetObj.presets.Count)
                {
                    presetNames = new string[presetObj.presets.Count];
                    for (int i = 0; i < presetObj.presets.Count; i++)
                    {
                        presetNames[i] = presetObj.presets[i].presetName;
                    }
                }

                int newIndex = EditorGUILayout.Popup("Preset", selectedPresetIndex, presetNames);
                if (newIndex != selectedPresetIndex)
                {
                    selectedPresetIndex = newIndex;
                    if (selectedPresetIndex >= 0 && selectedPresetIndex < presetObj.presets.Count)
                    {
                        Undo.RecordObject(target, "Apply Color Preset");
                        var preset = presetObj.presets[selectedPresetIndex];
                        imagesColorProp.colorValue = preset.imageColor;
                        textColorProp.colorValue = preset.textColor;
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("ColorPresetListにプリセットがありません。", MessageType.Info);
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Target Objects", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(imagesProp, true);
            EditorGUILayout.PropertyField(textsProp, true);
            EditorGUILayout.PropertyField(iconsProp, true);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(imagesColorProp);
            EditorGUILayout.PropertyField(textColorProp);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Apply Color To Objects"))
            {
                Undo.RecordObject(target, "Apply Color To Objects");

                serializedObject.ApplyModifiedProperties();

                var changer = (ColorChanger)target;
                changer.ApplyColor();

                EditorUtility.SetDirty(changer);
                PrefabUtility.RecordPrefabInstancePropertyModifications(changer);

                if (!Application.isPlaying)
                {
                    EditorSceneManager.MarkSceneDirty(changer.gameObject.scene);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}