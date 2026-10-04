using TMPro;
using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class InventoryFontBuilder
    {
        public static void Build()
        {
            const string path = "Assets/Resources/InventoryFont.asset";
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path) != null) return;
            var source = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/LiberationSans.ttf");
            var font = TMP_FontAsset.CreateFontAsset(source);
            if (font == null) throw new System.InvalidOperationException("Font asset creation failed.");
            font.name = "InventoryFont";
            font.TryAddCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 /.-,:+()[]");
            AssetDatabase.CreateAsset(font, path);
            AssetDatabase.AddObjectToAsset(font.material, font);
            foreach (var texture in font.atlasTextures) AssetDatabase.AddObjectToAsset(texture, font);
            EditorUtility.SetDirty(font); AssetDatabase.SaveAssets();
        }
    }
}
