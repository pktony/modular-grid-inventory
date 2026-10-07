using System.IO;
using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public static class ContainerShowcaseRecorder
    {
        [MenuItem("Inventory/Record Container Showcase (Play Mode)")]
        public static void Begin()
        {
            if (ContainerShowcaseScenario.Captions.Length != ContainerShowcaseScenario.StageCount)
                throw new System.InvalidOperationException("Showcase captions must match its stages.");
            var inventory = Object.FindAnyObjectByType<InventoryBootstrapper>();
            var scenario = new ContainerShowcaseScenario(inventory);
            var verifier = new ContainerShowcaseVerifier();
            InventoryFrameRecorder.Begin(inventory, scenario.Tick, stage => verifier.Verify(inventory, stage), scenario,
                ContainerShowcaseScenario.StageCount, ContainerShowcaseScenario.FramesPerStage, "container-showcase", "container-showcase.txt");
            File.WriteAllLines(Path.GetFullPath(Path.Combine(Application.dataPath, "../Recordings/container-showcase/captions.txt")), ContainerShowcaseScenario.Captions);
        }
    }
}
