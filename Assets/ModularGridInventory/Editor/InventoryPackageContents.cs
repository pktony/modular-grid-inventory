using System;
using System.Linq;
using UnityEditor;
namespace Pktony.GridInventory.Editor
{
    public sealed class InventoryPackageContents
    {
        public string[] Collect(string root, bool inputSystemOnly)
        {
            var integration = root + "/Integrations";
            var paths = AssetDatabase.FindAssets("", new[] { root }).Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => inputSystemOnly ? path.StartsWith(integration + "/", StringComparison.Ordinal)
                    || path == integration : path != integration && !path.StartsWith(integration + "/", StringComparison.Ordinal));
            return paths.Append(root).Distinct().OrderBy(path => path, StringComparer.Ordinal).ToArray();
        }
    }
}
