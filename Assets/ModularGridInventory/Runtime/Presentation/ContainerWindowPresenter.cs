using Pktony.GridInventory.Domain;
using UnityEngine;
using UnityEngine.UI;
namespace Pktony.GridInventory.Presentation
{
    public sealed class ContainerWindowPresenter
    {
        public ItemInstanceId Owner { get; }
        public ContainerId Id { get; }
        public InventoryPanelBindings Panel { get; }
        public RectTransform Header { get; }
        public Button Close { get; }
        private readonly InventoryPanelPresenter contents;
        private readonly Outline frame;
        private readonly Image titleBar;
        private readonly InventoryPaletteView palette;
        public ContainerWindowPresenter(ItemInstance owner, InventoryPanelBindings panel, RectTransform header, Button close,
            InventoryPanelPresenter contents, Outline frame, Image titleBar, InventoryPaletteView palette)
        { this.palette = palette; Owner = owner.Id; Id = owner.ChildContainerId; Panel = panel; Header = header; Close = close; this.contents = contents; this.frame = frame; this.titleBar = titleBar; }
        public void Render(InventorySnapshot snapshot, ItemInstanceId selected, ItemInstanceId dragging)
        {
            contents.Render(snapshot, Id, selected, dragging);
            Panel.Title.text = snapshot.Items[Owner].Definition.DisplayName;
            Panel.Policy.text = InventoryInspectorPresenter.Policy(snapshot.Containers[Id].Definition);
        }
        public void Select(ItemInstanceId previous, ItemInstanceId current) => contents.Select(previous, current);
        public void Drag(ItemInstanceId id, bool active) => contents.Drag(id, active);
        public void SetFocused(bool focused)
        { frame.effectColor = focused ? palette.Accent : palette.Border; titleBar.color = focused ? palette.ActiveTitle : palette.Title; }
    }
}
