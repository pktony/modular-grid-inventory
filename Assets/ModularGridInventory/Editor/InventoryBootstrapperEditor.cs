using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    [CustomEditor(typeof(InventoryBootstrapper))]
    public sealed class InventoryBootstrapperEditor : UnityEditor.Editor
    {
        private IReadOnlyList<string> errors;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.HelpBox("Use one host EventSystem and a compatible UI input module. Audio is optional. Settings are never changed automatically.", MessageType.Info);
            if (GUILayout.Button("Validate installation")) errors = new InventoryInstallationValidator().Validate(serializedObject);
            if (errors == null) return;
            foreach (var error in errors) EditorGUILayout.HelpBox(error, MessageType.Error);
            if (errors.Count == 0) EditorGUILayout.HelpBox("Configuration is valid.", MessageType.Info);
        }
    }
}
