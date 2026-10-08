using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
namespace Pktony.GridInventory.Editor
{
    public sealed class InventoryPackageValidator
    {
        public IReadOnlyList<string> Validate(string root, string[] paths)
        {
            var errors = new List<string>(); var guids = new HashSet<string>();
            foreach (var path in paths)
            {
                if (path != root && !path.StartsWith(root + "/", StringComparison.Ordinal)) errors.Add("Asset outside package: " + path);
                if (path.Length >= 150) errors.Add("Asset path is too long: " + path);
                var extension = Path.GetExtension(path).ToLowerInvariant();
                if (extension == ".exe" || extension == ".mp4" || extension == ".dll" || extension == ".zip") errors.Add("Development artifact: " + path);
                var guid = AssetDatabase.AssetPathToGUID(path);
                if (string.IsNullOrEmpty(guid) || !guids.Add(guid)) errors.Add("Missing or duplicate GUID: " + path);
            }
            foreach (var dependency in AssetDatabase.GetDependencies(paths, true))
                if (dependency.StartsWith("Assets/", StringComparison.Ordinal) && dependency != root
                    && !dependency.StartsWith(root + "/", StringComparison.Ordinal))
                    errors.Add("External Assets dependency: " + dependency);
            return errors;
        }
    }
}
