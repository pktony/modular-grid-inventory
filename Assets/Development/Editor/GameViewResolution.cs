using System;
using System.Reflection;
using UnityEditor;
namespace Pktony.GridInventory.Editor
{
    public static class GameViewResolution
    {
        [MenuItem("Inventory/Set Capture Resolution 1280x720")]
        public static void SetCaptureResolution() => Set(1280, 720);
        [MenuItem("Inventory/Set Review Resolution 1920x1080")]
        public static void SetReviewResolution() => Set(1920, 1080);
        [MenuItem("Inventory/Restore Standard Play Mode")]
        public static void RestorePlayMode()
        { EditorSettings.enterPlayModeOptionsEnabled = false; EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.None; }
        public static void Set(int width, int height)
        {
            var assembly = typeof(UnityEditor.Editor).Assembly;
            var sizesType = assembly.GetType("UnityEditor.GameViewSizes");
            var singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            var groupType = assembly.GetType("UnityEditor.GameViewSizeGroupType");
            var group = sizesType.GetMethod("GetGroup").Invoke(sizes, new[] {Enum.Parse(groupType, "Standalone")});
            var sizeType = assembly.GetType("UnityEditor.GameViewSize");
            var modeType = assembly.GetType("UnityEditor.GameViewSizeType");
            var size = Activator.CreateInstance(sizeType, Enum.Parse(modeType,"FixedResolution"), width, height, $"Inventory {width}x{height}");
            group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] {size});
            int count = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
            var gameViewType = assembly.GetType("UnityEditor.GameView");
            var window = EditorWindow.GetWindow(gameViewType);
            gameViewType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(window, count - 1);
            window.Repaint();
        }
    }
}
