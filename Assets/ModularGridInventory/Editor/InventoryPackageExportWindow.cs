using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public sealed class InventoryPackageExportWindow : EditorWindow
    {
        private bool inputSystemOnly;
        private IReadOnlyList<string> errors;
        private int count;
        [MenuItem("Tools/Modular Grid Inventory/Export Package")]
        public static void Open() => GetWindow<InventoryPackageExportWindow>("Inventory Export");
        private string Root => Path.GetDirectoryName(Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)))).Replace('\\', '/');
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Package root", Root);
            inputSystemOnly = EditorGUILayout.Toggle("Input System add-on only", inputSystemOnly);
            EditorGUILayout.HelpBox("Core and optional Input System adapter are exported separately. Host settings, packages and scenes are never modified.", MessageType.Info);
            if (GUILayout.Button("Validate contents")) Validate();
            foreach (var error in errors ?? System.Array.Empty<string>()) EditorGUILayout.HelpBox(error, MessageType.Error);
            if (errors != null && errors.Count == 0) EditorGUILayout.LabelField($"{count} assets; no external Assets dependencies.");
            if (!GUILayout.Button("Validate and export...")) return;
            var paths = Validate(); if (errors.Count > 0) return;
            var name = inputSystemOnly ? "ModularGridInventory-InputSystem" : "ModularGridInventory";
            var target = EditorUtility.SaveFilePanel("Export inventory package", "", name + "-0.1.0", "unitypackage");
            if (string.IsNullOrEmpty(target)) return;
#if UNITY_6000_6_OR_NEWER
            UnityEditor.AssetPackage.Package.Export(new UnityEditor.AssetPackage.ExportPackageParameters(paths, target, string.Empty, ExportPackageOptions.Default));
#else
            AssetDatabase.ExportPackage(paths, target, ExportPackageOptions.Default);
#endif
            ShowNotification(new GUIContent("Package exported"));
        }
        private string[] Validate()
        {
            var paths = new InventoryPackageContents().Collect(Root, inputSystemOnly);
            errors = new InventoryPackageValidator().Validate(Root, paths); count = paths.Length; return paths;
        }
    }
}
