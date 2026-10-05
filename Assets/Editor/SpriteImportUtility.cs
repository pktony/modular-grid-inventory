using UnityEditor;
using UnityEngine;
namespace InventorySystem.Editor
{
    public static class SpriteImportUtility
    {
        public static Sprite Load(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) throw new System.InvalidOperationException($"Missing image: {path}");
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true; importer.maxTextureSize = 512; importer.mipmapEnabled = false;
            importer.spritePixelsPerUnit = 100; importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
