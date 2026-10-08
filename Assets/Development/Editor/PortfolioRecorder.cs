using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public static class PortfolioRecorder
    {
        [MenuItem("Inventory/Record Walkthrough (Silent, Play Mode)")]
        public static void Begin()
        {
            var inventory = Object.FindAnyObjectByType<InventoryBootstrapper>();
            var scenario = new InventoryWalkthroughScenario(inventory);
            var verifier = new InventoryScenarioVerifier();
            InventoryFrameRecorder.Begin(inventory, scenario.Tick, stage => verifier.Verify(inventory, stage), scenario,
                InventoryWalkthroughScenario.StageCount, InventoryWalkthroughScenario.FramesPerStage, "frames-silent", "recording-silent.txt");
        }
    }
}
