using System.Linq;
namespace InventorySystem.Editor
{
    internal static class RigCatalogBuilder
    {
        public static ItemDefinition[] Build(ItemCategoryDefinition category)
        {
            var table = RigPresetLoader.Load();
            return table.rigs.Select(rig => Build(rig, table.displayStride, category)).ToArray();
        }
        private static ItemDefinition Build(RigPreset rig, float stride, ItemCategoryDefinition category)
        {
            var sections = rig.pockets.Select(p => (p.id, p.width, p.height, p.column * stride, p.row * stride)).ToArray();
            var layout = ContainerLayoutAssetWriter.Write(rig.id, sections);
            var container = ContainerDefinitionAssetWriter.Write(rig.id, sections, layout, null);
            var icon = SpriteImportUtility.Load($"Assets/Resources/RigIcons/{rig.id}.png");
            return ItemDefinitionAssetWriter.Write(rig.id, rig.title, category, rig.width, rig.height, 1, icon, container);
        }
    }
}
