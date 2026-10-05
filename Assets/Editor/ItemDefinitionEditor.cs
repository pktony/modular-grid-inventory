using UnityEditor;
using UnityEngine;

namespace InventorySystem.Editor
{
    [CustomEditor(typeof(ItemDefinition))]
    public sealed class ItemDefinitionEditor : UnityEditor.Editor
    {
        private UnityEditor.Editor containerEditor;
        private void OnDisable() { if (containerEditor != null) DestroyImmediate(containerEditor); }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                EditorGUILayout.LabelField("Item definition", EditorStyles.boldLabel);
                foreach (var field in new[] { "identifier", "title", "category", "icon" }) EditorGUILayout.PropertyField(serializedObject.FindProperty(field));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("width"), new GUIContent("External width"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("height"), new GUIContent("External height"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxStack"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("container"));
                serializedObject.ApplyModifiedProperties();
            }
            var container = serializedObject.FindProperty("container").objectReferenceValue;
            if (container == null)
            {
                if (containerEditor != null) { DestroyImmediate(containerEditor); containerEditor = null; }
                return;
            }
            EditorGUILayout.Space(); EditorGUILayout.LabelField("Container pockets", EditorStyles.boldLabel);
            CreateCachedEditor(container, typeof(ContainerDefinitionEditor), ref containerEditor); containerEditor.OnInspectorGUI();
        }
    }
}
