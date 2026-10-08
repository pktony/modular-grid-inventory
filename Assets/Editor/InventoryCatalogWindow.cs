using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public sealed class InventoryCatalogWindow : EditorWindow
    {
        private InventoryCatalogAsset catalog;
        private Vector2 scroll;
        private Object detail;
        private UnityEditor.Editor detailEditor;
        [MenuItem("Inventory/Catalog Table")]
        public static void Open() => GetWindow<InventoryCatalogWindow>("Inventory Catalog");
        private void OnEnable() => catalog = AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/Items/Expansion/Catalog.asset");
        private void OnDisable() { if (detailEditor != null) DestroyImmediate(detailEditor); }
        private void OnGUI()
        {
            catalog = (InventoryCatalogAsset)EditorGUILayout.ObjectField("Catalog", catalog, typeof(InventoryCatalogAsset), false);
            EditorGUILayout.HelpBox("Edits apply to the next session. Container policy and section layout are editable below. Values are portfolio presets.", MessageType.Info);
            if (catalog == null) return;
            if (GUILayout.Button("Save and validate catalog"))
            {
                AssetDatabase.SaveAssets();
                try { new InventoryCatalogSnapshotFactory().Create(catalog); ShowNotification(new GUIContent("Catalog valid")); }
                catch (System.Exception error) { Debug.LogError(error.Message); ShowNotification(new GUIContent("Catalog invalid; see Console")); }
            }
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.BeginHorizontal();
            foreach (var title in new[] { "Item / ID", "Name", "Category", "W", "H", "Stack", "Container", "Icon" }) GUILayout.Label(title, GUILayout.Width(title == "W" || title == "H" || title == "Stack" ? 48 : 140));
            EditorGUILayout.EndHorizontal();
            var catalogData = new SerializedObject(catalog); var items = catalogData.FindProperty("items");
            for (int i = 0; i < items.arraySize; i++)
            {
                var item = items.GetArrayElementAtIndex(i).objectReferenceValue;
                if (item == null) { EditorGUILayout.HelpBox("Missing item reference", MessageType.Error); continue; }
                var data = new SerializedObject(item); EditorGUILayout.BeginHorizontal();
                foreach (var property in new[] { "identifier", "title", "category", "width", "height", "maxStack", "container", "icon" })
                {
                    var field = data.FindProperty(property);
                    EditorGUILayout.PropertyField(field, GUIContent.none, GUILayout.Width(property == "width" || property == "height" || property == "maxStack" ? 48 : 140));
                }
                if (GUILayout.Button("Inspect", GUILayout.Width(64))) SetDetail(item);
                if (GUILayout.Button("Rules", GUILayout.Width(64))) SetDetail(data.FindProperty("container").objectReferenceValue);
                EditorGUILayout.EndHorizontal(); data.ApplyModifiedProperties();
            }
            EditorGUILayout.Space(); EditorGUILayout.LabelField("Categories / parent tree", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(catalogData.FindProperty("categories"), true);
            EditorGUILayout.PropertyField(items, true); catalogData.ApplyModifiedProperties();
            detail = EditorGUILayout.ObjectField("Inspect definition / policy / layout", detail, typeof(Object), false);
            if (detail != null)
            {
                if (detailEditor == null || detailEditor.target != detail) SetDetail(detail);
                detailEditor.OnInspectorGUI();
            }
            EditorGUILayout.EndScrollView();
        }
        private void SetDetail(Object value)
        { detail = value; if (detailEditor != null) DestroyImmediate(detailEditor); detailEditor = value == null ? null : UnityEditor.Editor.CreateEditor(value); }
    }
}
