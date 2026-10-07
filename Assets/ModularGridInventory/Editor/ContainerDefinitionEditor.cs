using System;
using UnityEditor;
using UnityEngine;

namespace Pktony.GridInventory.Editor
{
    [CustomEditor(typeof(ContainerDefinition))]
    public sealed class ContainerDefinitionEditor : UnityEditor.Editor
    {
        private ContainerAuthoringSession session;
        private int selected;
        private string editError;
        private void OnEnable() { session = new ContainerAuthoringSession((ContainerDefinition)target); Undo.undoRedoPerformed += Repaint; }
        private void OnDisable() => Undo.undoRedoPerformed -= Repaint;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.HelpBox("Edit the asset directly. Preview updates immediately; the next Play uses these values. No catalog generation required.", MessageType.Info);
            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("identifier"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("policy"), new GUIContent("Container rules"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("layout"), new GUIContent("Layout asset"));
                serializedObject.ApplyModifiedProperties();
                var pockets = session.Read(); selected = Mathf.Clamp(selected, 0, Math.Max(0, pockets.Length - 1));
                selected = ContainerPocketPreview.Draw(pockets, selected);
                var errors = ContainerAuthoringValidation.Errors((ContainerDefinition)target, pockets);
                foreach (var error in errors) EditorGUILayout.HelpBox(error, MessageType.Error);
                if (editError != null) EditorGUILayout.HelpBox(editError, MessageType.Error);
                if (pockets.Length > 0) DrawPocket(pockets);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add pocket")) Run(() => selected = session.Add());
                using (new EditorGUI.DisabledScope(pockets.Length <= 1))
                    if (GUILayout.Button("Remove selected")) Run(() => session.Remove(selected));
                EditorGUILayout.EndHorizontal();
            }
            if (EditorApplication.isPlayingOrWillChangePlaymode) EditorGUILayout.HelpBox("Exit Play mode to edit the preset.", MessageType.Info);
        }

        private void DrawPocket(PocketAuthoringData[] pockets)
        {
            var names = new string[pockets.Length]; for (int i = 0; i < names.Length; i++) names[i] = $"{i + 1}. {pockets[i].Id}";
            selected = EditorGUILayout.Popup("Selected pocket", selected, names); var p = pockets[selected];
            EditorGUI.BeginChangeCheck();
            string id = EditorGUILayout.DelayedTextField("Pocket ID", p.Id);
            int width = EditorGUILayout.DelayedIntField("Width (cells)", p.Width), height = EditorGUILayout.DelayedIntField("Height (cells)", p.Height);
            float x = EditorGUILayout.DelayedFloatField("X (display cells)", p.Position.x), y = EditorGUILayout.DelayedFloatField("Y (display cells)", p.Position.y);
            if (EditorGUI.EndChangeCheck()) Run(() => session.Update(selected, id, width, height, new Vector2(x, y)));
            serializedObject.Update(); var section = serializedObject.FindProperty("sections").GetArrayElementAtIndex(selected);
            EditorGUILayout.PropertyField(section.FindPropertyRelative("policy"), new GUIContent("Pocket rules"), true);
            serializedObject.ApplyModifiedProperties();
        }

        private void Run(Action edit)
        {
            try { edit(); editError = null; GUI.FocusControl(null); }
            catch (ArgumentException error) { editError = error.Message; }
            catch (InvalidOperationException error) { editError = error.Message; }
            Repaint();
        }
    }
}
