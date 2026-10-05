using System;
using System.Linq;
using UnityEngine;
namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Container Layout")]
    public sealed class ContainerLayoutDefinition : ScriptableObject
    {
        [Serializable]
        private sealed class SectionPosition
        {
            public string sectionId;
            public Vector2 position;
        }
        [SerializeField] private SectionPosition[] sections = Array.Empty<SectionPosition>();
        internal ContainerLayoutView Freeze() => new(sections.Select(s => new SectionLayoutView(s.sectionId, s.position.x, s.position.y)));
    }
}
