using InventorySystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InventorySystem.Editor
{
    public static class InventorySceneBuilder
    {
        [MenuItem("Inventory/Build Demo Scene")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var catalog = AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/Items/Expansion/Catalog.asset");
            if (catalog == null) throw new System.InvalidOperationException("Build the item catalog first.");
            var canvas = new GameObject("InventoryCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280,720); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            var inventory = new GameObject("InventoryDemo").AddComponent<ExpandedInventory>();
            var so = new SerializedObject(inventory);
            so.FindProperty("catalog").objectReferenceValue = catalog;
            so.FindProperty("canvas").objectReferenceValue = canvas.GetComponent<Canvas>(); so.ApplyModifiedPropertiesWithoutUndo();
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            var camera = new GameObject("Main Camera", typeof(Camera)); camera.tag = "MainCamera";
            camera.GetComponent<Camera>().backgroundColor = InventoryTheme.Background;
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/Inventory.unity");
            EditorBuildSettings.scenes = new[] {new EditorBuildSettingsScene("Assets/Scenes/Inventory.unity", true)};
            PlayerSettings.productName = "Tactical Inventory"; PlayerSettings.companyName = "Psangwon";
            PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
