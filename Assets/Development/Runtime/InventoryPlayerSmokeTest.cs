using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
namespace Pktony.GridInventory
{
    public sealed class InventoryPlayerSmokeTest : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (Application.isEditor || !Environment.GetCommandLineArgs().Contains("-inventory-smoke-test")) return;
            new GameObject("InventorySmokeTest").AddComponent<InventoryPlayerSmokeTest>();
        }
        private IEnumerator Start()
        {
            yield return null; yield return null;
            var inventory = FindAnyObjectByType<InventoryBootstrapper>(); inventory.enabled = false;
            var scenario = new InventoryWalkthroughScenario(inventory); var verifier = new InventoryScenarioVerifier();
            string directory = Path.GetFullPath(Path.Combine(Application.dataPath, "../../TestResults")); Directory.CreateDirectory(directory);
            int checkedStages = 0; string failure = null;
            for (int stage = 0; stage < InventoryWalkthroughScenario.StageCount; stage++)
            {
                try { Canvas.ForceUpdateCanvases(); scenario.Tick(stage * InventoryWalkthroughScenario.StageDuration); verifier.Verify(inventory, stage); checkedStages++; }
                catch (Exception error) { failure = error.ToString(); break; }
                yield return new WaitForSecondsRealtime(0.15f);
                try { scenario.Tick(stage * InventoryWalkthroughScenario.StageDuration + 0.9f); }
                catch (Exception error) { failure = error.ToString(); break; }
                yield return new WaitForSecondsRealtime(0.15f);
            }
            InventorySmokeReportWriter.Write(directory, checkedStages, failure);
            scenario.Dispose(); Application.Quit(failure == null ? 0 : 1);
        }
    }
}
