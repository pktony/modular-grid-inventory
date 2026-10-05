using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace InventorySystem.Editor
{
    internal static class ContainerAuthoringValidation
    {
        public static string[] Errors(ContainerDefinition container, IReadOnlyList<PocketAuthoringData> pockets)
        {
            var errors = new List<string>(); var sections = new Dictionary<string, GridSectionDefinitionView>();
            if (pockets.Count == 0) errors.Add("Add at least one pocket.");
            foreach (var pocket in pockets)
            {
                if (string.IsNullOrWhiteSpace(pocket.Id) || !sections.TryAdd(pocket.Id,
                    new GridSectionDefinitionView(pocket.Id, pocket.Width, pocket.Height, null)))
                    errors.Add("Pocket IDs must be non-empty and unique.");
                if (pocket.Width < 1 || pocket.Height < 1) errors.Add("Pocket size must be positive.");
            }
            using var data = new SerializedObject(container); var source = data.FindProperty("layout").objectReferenceValue;
            ContainerLayoutView layout = null;
            if (source != null)
            {
                using var positions = new SerializedObject(source); var entries = positions.FindProperty("sections"); var views = new List<SectionLayoutView>();
                for (int i = 0; i < entries.arraySize; i++)
                {
                    var entry = entries.GetArrayElementAtIndex(i); var pos = entry.FindPropertyRelative("position").vector2Value;
                    views.Add(new SectionLayoutView(entry.FindPropertyRelative("sectionId").stringValue, pos.x, pos.y));
                }
                layout = new ContainerLayoutView(views);
            }
            var view = new ContainerDefinitionView(container.name, sections.Values, null, layout);
            new ContainerLayoutValidator().Validate(view, sections, errors);
            return errors.Distinct().ToArray();
        }
    }
}
