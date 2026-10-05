using System.Linq;
using InventorySystem.Domain;
using NUnit.Framework;
using UnityEditor;
namespace InventorySystem.Tests
{
    public sealed class RigCatalogTests
    {
        private InventoryCatalog Catalog() => new InventoryCatalogSnapshotFactory().Create(
            AssetDatabase.LoadAssetAtPath<InventoryCatalogAsset>("Assets/Items/Expansion/Catalog.asset"));

        [TestCase("rig-scav", 2, 3, 6, "0,0:1x1;1,0:1x2;2,0:1x2;3,0:1x1")]
        [TestCase("rig-micro", 2, 3, 8, "0,0:2x2;2,0:1x2;3,0:1x2")]
        [TestCase("rig-wartech", 3, 3, 10, "0,0:2x2;2,0:1x2;3,0:1x2;4,0:1x1;4,1:1x1")]
        [TestCase("rig-scout", 3, 4, 12, "0,0:1x2;1,0:1x2;2,0:1x2;3,0:1x2;0,2:1x1;1,2:1x1;2,2:1x1;3,2:1x1")]
        [TestCase("rig-d3crx", 3, 3, 16, "0,0:1x2;1,0:1x2;2,0:1x2;3,0:1x2;0,2:1x2;1,2:1x1;1,3:1x1;2,2:1x1;2,3:1x1;3,2:1x2")]
        [TestCase("rig", 3, 4, 20, "0,0:1x2;1,0:1x2;2,0:1x2;3,0:1x2;0,2:1x1;1,2:1x1;2,2:1x1;3,2:1x1;0,3:2x2;2,3:1x2;3,3:1x2")]
        [TestCase("rig-mk3", 3, 4, 20, "0,0:1x2;1,0:1x2;2,0:1x2;3,0:1x2;0,2:1x2;1,2:1x2;2,2:1x2;3,2:1x2;0,4:1x1;1,4:1x1;2,4:2x1")]
        [TestCase("rig-alpha", 4, 4, 20, "0,0:1x3;1,0:1x3;2,0:1x3;3,0:1x3;0,3:1x2;1,3:2x2;3,3:1x2")]
        [TestCase("rig-mppv", 4, 3, 24, "0,0:1x2;1,0:1x2;2,0:1x2;3,0:1x2;0,2:1x2;1,2:1x2;2,2:1x2;3,2:1x2;0,4:1x2;1,4:1x1;2,4:1x1;1,5:1x1;2,5:1x1;3,4:1x2")]
        [TestCase("rig-belt", 4, 4, 25, "0,0:1x1;1,0:1x2;2,0:1x2;3,0:1x2;4,0:1x1;0,1:1x1;4,1:1x1;0,2:1x1;1,2:1x2;2,2:1x2;3,2:1x2;4,2:1x1;0,3:1x1;4,3:1x1;0,4:1x1;1,4:1x1;2,4:1x1;3,4:1x1;4,4:1x1")]
        public void PresetMatchesReferencePockets(string id, int width, int height, int capacity, string expected)
        {
            Assert.That(Catalog().TryGet(new DefinitionId(id), out var rig), Is.True);
            Assert.That((rig.Width, rig.Height, rig.MaxStack), Is.EqualTo((width, height, 1)));
            Assert.That(rig.Container.Sections.Sum(s => s.Width * s.Height), Is.EqualTo(capacity));
            var pockets = expected.Split(';');
            Assert.That(rig.Container.Sections.Count, Is.EqualTo(pockets.Length));
            for (int i = 0; i < pockets.Length; i++)
            {
                var values = pockets[i].Split(':'); var position = values[0].Split(','); var size = values[1].Split('x');
                var section = rig.Container.Sections[i]; var layout = rig.Container.Layout.Sections.Single(l => l.SectionId == section.Id);
                Assert.That((section.Width, section.Height), Is.EqualTo((int.Parse(size[0]), int.Parse(size[1]))));
                Assert.That(layout.X, Is.EqualTo(int.Parse(position[0]) * 1.06f).Within(0.0001f));
                Assert.That(layout.Y, Is.EqualTo(int.Parse(position[1]) * 1.06f).Within(0.0001f));
            }
        }
        [Test] public void TenRigsHaveDistinctIconsAndIndependentDemoInstances()
        {
            var catalog = Catalog(); var rigs = catalog.Definitions.Where(d => d.CategoryId == "Container/Rig").ToArray();
            Assert.That(rigs.Length, Is.EqualTo(10)); Assert.That(rigs.Select(r => r.Icon).Distinct().Count(), Is.EqualTo(10));
            var state = new ExpandedDemoSeed().Create(catalog);
            var instances = state.Items.Values.Where(i => i.Definition.CategoryId == "Container/Rig").ToArray();
            Assert.That(instances.Length, Is.EqualTo(10)); Assert.That(instances.Select(i => i.ChildContainerId).Distinct().Count(), Is.EqualTo(10));
            Assert.That(state.Items.Count, Is.EqualTo(22));
        }
        [Test] public void BlackRockCannotTreatTwoNeighborPocketsAsOneLargePocket()
        {
            using var runtime = new InventoryRuntime(Catalog(), ExpandedDemoSeed.StashDefinition());
            var root = runtime.ReadModel.Snapshot.RootContainerId;
            var rig = runtime.Edit.Add(new AddRequest(new DefinitionId("rig"), 1, new PlacementTarget(root, new GridSectionId("main"), 0, 0))).CreatedItemId;
            var child = runtime.ReadModel.Snapshot.Items[rig].ChildContainerId;
            var before = runtime.ReadModel.Snapshot;
            var result = runtime.Edit.Add(new AddRequest(new DefinitionId("ammo-case"), 1, new PlacementTarget(child, new GridSectionId("tall-e"), 0, 0)));
            Assert.That(result.Success, Is.False); Assert.That(runtime.ReadModel.Snapshot, Is.SameAs(before));
            Assert.That(runtime.Edit.Add(new AddRequest(new DefinitionId("ammo-case"), 1, new PlacementTarget(child, new GridSectionId("large-a"), 0, 0))).Success, Is.True);
        }
    }
}
