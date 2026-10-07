using Pktony.GridInventory;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Pktony.GridInventory.Editor
{
    public static class InventorySceneBuilder
    {
        [MenuItem("Inventory/Build Demo Scene")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var catalog = AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/ModularGridInventory/Samples/Catalog/Catalog.asset");
            if (catalog == null) throw new System.InvalidOperationException("Build the item catalog first.");
            var canvas = new GameObject("InventoryCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280,720); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            var inventory = new GameObject("InventoryDemo").AddComponent<InventoryBootstrapper>();
            var so = new SerializedObject(inventory);
            so.FindProperty("catalog").objectReferenceValue = catalog;
            so.FindProperty("rootContainer").objectReferenceValue = AssetDatabase.LoadAssetAtPath<ContainerDefinition>("Assets/ModularGridInventory/Samples/Settings/RootContainer.asset");
            so.FindProperty("initialState").objectReferenceValue = AssetDatabase.LoadAssetAtPath<InventoryInitialState>("Assets/ModularGridInventory/Samples/Settings/InitialState.asset");
            so.FindProperty("theme").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Presentation.InventoryUiTheme>("Assets/ModularGridInventory/Samples/Settings/InventoryTheme.asset");
            so.FindProperty("audioSettings").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Presentation.InventoryAudioSettings>("Assets/ModularGridInventory/Samples/Settings/InventoryAudioSettings.asset");
            so.FindProperty("canvas").objectReferenceValue = canvas.GetComponent<Canvas>(); so.ApplyModifiedPropertiesWithoutUndo();
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)); camera.tag = "MainCamera";
            camera.GetComponent<Camera>().backgroundColor = new Presentation.InventoryPalette().Background;
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            EditorSceneManager.SaveScene(scene,"Assets/ModularGridInventory/Samples/Scenes/Inventory.unity");
            EditorBuildSettings.scenes = new[] {new EditorBuildSettingsScene("Assets/ModularGridInventory/Samples/Scenes/Inventory.unity", true)};
            PlayerSettings.productName = "Tactical Inventory"; PlayerSettings.companyName = "Psangwon";
            PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
