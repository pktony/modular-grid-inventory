using System.IO;
namespace InventorySystem
{
    public static class InventorySmokeReportWriter
    {
        public static void Write(string directory, int checkedStages, string failure)
        {
            File.WriteAllText(Path.Combine(directory, "player-smoke.txt"), failure == null
                ? $"Passed / {checkedStages} scenario stages / actual Windows player / Unity pointer events"
                : $"Failed after {checkedStages} stages: {failure}");
        }
    }
}
