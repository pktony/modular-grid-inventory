#if UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;

namespace Pktony.GridInventory.Development
{
    internal static class InventoryWebRuntimeSettings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Configure()
        {
            Application.runInBackground = true;
            WebGLInput.captureAllKeyboardInput = false;
        }
    }
}
#endif
