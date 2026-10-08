using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InventorySystem.Editor
{
    internal static class ContainerPocketPreview
    {
        public static int Draw(IReadOnlyList<PocketAuthoringData> pockets, int selected)
        {
            EditorGUILayout.LabelField("Layout preview", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"{pockets.Count} pockets  |  {pockets.Sum(p => (long)p.Width * p.Height)} cells");
            var canvas = GUILayoutUtility.GetRect(40, 240, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(canvas, new Color(0.06f, 0.08f, 0.08f));
            float width = 1, height = 1;
            foreach (var pocket in pockets)
            {
                if (!float.IsFinite(pocket.Position.x) || !float.IsFinite(pocket.Position.y)) continue;
                width = Mathf.Max(width, pocket.Position.x + pocket.Width); height = Mathf.Max(height, pocket.Position.y + pocket.Height);
            }
            float scale = Mathf.Max(0.01f, Mathf.Min((canvas.width - 24) / width, (canvas.height - 24) / height, 42));
            var origin = new Vector2(canvas.center.x - width * scale / 2, canvas.center.y - height * scale / 2);
            for (int i = 0; i < pockets.Count; i++)
            {
                var p = pockets[i]; if (!float.IsFinite(p.Position.x) || !float.IsFinite(p.Position.y) || p.Width < 1 || p.Height < 1) continue;
                var rect = new Rect(origin.x + p.Position.x * scale, origin.y + p.Position.y * scale, p.Width * scale, p.Height * scale);
                EditorGUI.DrawRect(rect, i == selected ? new Color(0.32f, 0.34f, 0.24f) : new Color(0.16f, 0.18f, 0.18f));
                for (int x = 1; x < p.Width && x < 256; x++) EditorGUI.DrawRect(new Rect(rect.x + x * scale, rect.y, 1, rect.height), Color.black);
                for (int y = 1; y < p.Height && y < 256; y++) EditorGUI.DrawRect(new Rect(rect.x, rect.y + y * scale, rect.width, 1), Color.black);
                var edge = i == selected ? new Color(0.87f, 0.86f, 0.64f) : new Color(0.49f, 0.53f, 0.52f);
                EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), edge); EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1, rect.width, 1), edge);
                EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), edge); EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.y, 1, rect.height), edge);
                if (scale >= 14) GUI.Label(new Rect(rect.x + 3, rect.y + 2, rect.width - 3, 20), (i + 1).ToString(), EditorStyles.whiteMiniLabel);
                if (GUI.Button(rect, new GUIContent("", $"{p.Id}: {p.Width} x {p.Height}"), GUIStyle.none)) selected = i;
            }
            return selected;
        }
    }
}
