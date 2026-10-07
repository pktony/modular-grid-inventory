using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public static class RigWindowShowcase
    {
        [MenuItem("Inventory/Show All Rig Windows (Play Mode)")]
        public static void Show()
        {
            var inventory = UnityEngine.Object.FindAnyObjectByType<InventoryBootstrapper>();
            if (!Application.isPlaying || inventory?.ReadModel == null)
                throw new InvalidOperationException("Enter the inventory scene in Play mode first.");
            inventory.Interaction.Cancel(); inventory.Windows.CloseAll();
            var state = inventory.ReadModel.Snapshot;
            var rigs = state.Items.Values.Where(i => i.Definition.CategoryId == "Container/Rig")
                .OrderBy(i => i.Definition.Container.Sections.Sum(s => s.Width * s.Height)).ToArray();
            float x = 24, y = 80, rowHeight = 0;
            for (int i = 0; i < rigs.Length; i++)
            {
                if (i > 0 && i % 5 == 0) { x = 24; y += rowHeight + 10; rowHeight = 0; }
                inventory.Windows.Open(state, rigs[i].Id, default, default);
                var root = inventory.Windows.Find(rigs[i].ChildContainerId).Panel.Root;
                root.anchoredPosition = new Vector2(x, -y);
                x += root.sizeDelta.x + 8; rowHeight = Mathf.Max(rowHeight, root.sizeDelta.y);
            }
            Canvas.ForceUpdateCanvases();
        }
    }
}
