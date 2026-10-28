#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Pktony.GridInventory.Development
{
    public sealed class InventoryWebReady : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void InventoryReady();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            new GameObject("InventoryWebReady").AddComponent<InventoryWebReady>();
        }

        private IEnumerator Start()
        {
            var inventory = FindFirstObjectByType<InventoryBootstrapper>();
            while (inventory != null && inventory.Screen == null) yield return null;
            if (inventory != null && inventory.ReadModel != null) InventoryReady();
        }
    }
}
#endif
