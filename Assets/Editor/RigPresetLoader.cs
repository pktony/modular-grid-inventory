using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    internal static class RigPresetLoader
    {
        public static RigPresetTable Load()
        {
            const string path = "Assets/Items/Expansion/RigPresets.json";
            var text = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (text == null) throw new InvalidOperationException($"Missing rig table: {path}");
            var table = JsonUtility.FromJson<RigPresetTable>(text.text);
            if (table?.rigs == null || table.rigs.Length == 0 || !float.IsFinite(table.displayStride) || table.displayStride < 1)
                throw new InvalidOperationException("Invalid rig table or display stride.");
            if (table.rigs.Any(r => r == null || string.IsNullOrWhiteSpace(r.id) || string.IsNullOrWhiteSpace(r.title)
                || r.width < 1 || r.height < 1 || r.pockets == null || r.pockets.Length == 0
                || r.pockets.Any(p => p == null || string.IsNullOrWhiteSpace(p.id) || p.width < 1 || p.height < 1 || p.column < 0 || p.row < 0)
                || r.pockets.Select(p => p.id).Distinct().Count() != r.pockets.Length)
                || table.rigs.Select(r => r.id).Distinct().Count() != table.rigs.Length)
                throw new InvalidOperationException("Invalid or duplicate rig preset.");
            return table;
        }
    }
}
