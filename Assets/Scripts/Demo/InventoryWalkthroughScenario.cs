using System;
using System.Linq;
using InventorySystem.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InventorySystem
{
    public sealed class InventoryWalkthroughScenario : IDisposable
    {
        public const int StageCount = 18;
        public const int Duration = StageCount * 4;
        private readonly ExpandedInventory inventory;
        private int stage = -1;
        private GameObject dragged;
        private PointerEventData data;
        private Vector2 start, destination, pointer;
        private bool holding, cancelPreviewStarted;
        private float flashUntil;
        private ItemInstanceId ammo, rifle, mergeSource, mergeTarget;
        private InventorySnapshot State => inventory.ReadModel.Snapshot;
        private ContainerId Root => State.RootContainerId;
        private readonly GridSectionId main = new("main");
        public InventoryWalkthroughScenario(ExpandedInventory inventory) { this.inventory = inventory; }
        private ItemInstanceId Find(string id, ContainerId owner = default, int quantity = 0) => State.Items.Values.First(i =>
            i.Definition.Identifier == id && (owner.IsEmpty || State.Containers[owner].Entries.ContainsKey(i.Id)) && (quantity == 0 || i.Quantity == quantity)).Id;
        private GameObject View(ItemInstanceId id) => GameObject.Find("Item-" + id);
        private Vector2 Point(ContainerId container, GridSectionId section, int x, int y)
        {
            var panel = container == Root ? inventory.Screen.Stash : inventory.Screen.Bag;
            return panel.Sections[section].Geometry.ScreenPoint(x, y, new Vector2(25, 25));
        }
        private Vector2 ItemPoint(ItemInstanceId id)
        {
            State.Registry.TryGetOwner(id, out var owner); var entry = State.Containers[owner].Entries[id];
            return Point(owner, entry.SectionId, entry.X, entry.Y);
        }
        private PointerEventData Event(Vector2 point, int clicks = 1, bool right = false) => new(EventSystem.current)
            { position = point, pressPosition = point, clickCount = clicks, button = right ? PointerEventData.InputButton.Right : PointerEventData.InputButton.Left };
        private void ClickItem(ItemInstanceId id, float time, int clicks = 1, bool right = false)
        {
            pointer = ItemPoint(id); var e = Event(pointer, clicks, right);
            ExecuteEvents.Execute(View(id), e, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(View(id), e, ExecuteEvents.pointerClickHandler); flashUntil = time + 0.5f;
        }
        private void Click(Button button, float time)
        {
            pointer = RectTransformUtility.WorldToScreenPoint(null, button.transform.TransformPoint(new Vector3(30, -16)));
            var e = Event(pointer); ExecuteEvents.Execute(button.gameObject, e, ExecuteEvents.pointerClickHandler); flashUntil = time + 0.5f;
        }
        private void Begin(ItemInstanceId id, Vector2 target)
        {
            pointer = start = ItemPoint(id); destination = target; dragged = View(id); data = Event(pointer); holding = true;
            ExecuteEvents.Execute(dragged, data, ExecuteEvents.pointerDownHandler); ExecuteEvents.Execute(dragged, data, ExecuteEvents.beginDragHandler);
        }
        private void End()
        { data.position = destination; ExecuteEvents.Execute(dragged, data, ExecuteEvents.endDragHandler); pointer = destination; holding = false; }
        private void Scroll() { inventory.Screen.Stash.Scroll.verticalNormalizedPosition = 0.65f; Canvas.ForceUpdateCanvases(); }
        public void Tick(float time)
        {
            int next = Mathf.Min(StageCount - 1, Mathf.FloorToInt(time / 4));
            if (next != stage) { stage = next; Apply(stage, time); }
            if (stage == 15 && time % 4 > 1.6f && !cancelPreviewStarted)
            {
                cancelPreviewStarted = true; Begin(rifle, Point(Root, main, 4, 14));
            }
            if (holding)
            {
                float progress = Mathf.SmoothStep(0, 1, Mathf.Clamp01((time % 4 - 0.4f) / 2.8f));
                pointer = Vector2.Lerp(start, destination, progress); data.position = pointer;
                if (inventory.Interaction.Drag?.SplitQuantity > 0) inventory.Interaction.UpdatePointer(pointer);
                else ExecuteEvents.Execute(dragged, data, ExecuteEvents.dragHandler);
            }
            inventory.Indicator.Show(pointer, holding || time < flashUntil);
            inventory.Screen.Status.text = Captions[stage];
        }
        private void Apply(int value, float time)
        {
            switch (value)
            {
                case 0: inventory.ResetDemo(); break;
                case 1: ClickItem(Find("berkut", Root), time, 2); break;
                case 2: ClickItem(Find("mbss"), time, 2); break;
                case 3:
                    Click(inventory.Screen.Back, time); Click(inventory.Screen.Close, time);
                    var filledBag = Find("berkut", Root); Begin(filledBag, ItemPoint(filledBag));
                    inventory.Screen.Stash.Scroll.verticalNormalizedPosition = 0.3f; Canvas.ForceUpdateCanvases();
                    destination = Point(Root, main, 0, 13); break;
                case 4: End(); Scroll(); ClickItem(Find("ammo-case", Root), time, 2); break;
                case 5:
                    ammo = Find("pst", Root, 20); Begin(ammo, Point(inventory.Screen.Bag.Container, main, 0, 0)); break;
                case 6: End(); flashUntil = time + 0.5f; break;
                case 7: rifle = Find("aks74u", Root); Begin(rifle, Point(inventory.Screen.Bag.Container, main, 2, 1)); break;
                case 8: End(); ClickItem(Find("rig", Root), time, 2); break;
                case 9:
                    ammo = Find("pst", Root, 40); Begin(ammo, Point(inventory.Screen.Bag.Container, new GridSectionId("small-a"), 0, 0)); break;
                case 10: End(); flashUntil = time + 0.5f; break;
                case 11:
                    inventory.ResetDemo(); Scroll(); mergeSource = Find("pst", Root, 20); mergeTarget = Find("pst", Root, 40);
                    Begin(mergeSource, ItemPoint(mergeTarget)); break;
                case 12:
                    End(); ClickItem(mergeTarget, time, 1, true); Click(GameObject.Find("SplitStack").GetComponent<Button>(), time);
                    GameObject.Find("Input").GetComponent<TMPro.TMP_InputField>().text = "5"; break;
                case 13:
                    Click(GameObject.Find("Confirm").GetComponent<Button>(), time);
                    holding = true; start = pointer; destination = Point(Root, main, 8, 11); data = Event(pointer); break;
                case 14:
                    pointer = destination; holding = false; ExecuteEvents.Execute(inventory.Screen.Stash.Viewport.gameObject, Event(pointer), ExecuteEvents.pointerClickHandler);
                    inventory.Screen.Stash.Scroll.verticalNormalizedPosition = 0.3f; Canvas.ForceUpdateCanvases();
                    rifle = Find("aks74u", Root); Begin(rifle, Point(Root, main, 1, 14)); inventory.Interaction.Rotate(); break;
                case 15:
                    End(); inventory.Screen.Stash.Scroll.verticalNormalizedPosition = 0.15f; Canvas.ForceUpdateCanvases();
                    pointer = ItemPoint(rifle); flashUntil = time + 0.5f; break;
                case 16:
                    inventory.Interaction.Cancel(); holding = false;
                    ClickItem(Find("rk0", Root), time); Click(inventory.Screen.Delete, time); break;
                case 17: Click(inventory.Screen.Reset, time); break;
            }
        }
        private static readonly string[] Captions = {
            "01 / 10 item definitions. Independent instances. Data-defined containers.",
            "02 / Open Berkut. Its own inventory contains another backpack.",
            "03 / Open MBSS. Three levels: stash > Berkut > MBSS > medkit.",
            "04 / Move a filled bag. Its contents remain attached to the same ID.",
            "05 / Ammunition case accepts the Ammo category and its descendants.",
            "06 / Drag ammunition between panels. Preview changes no state.",
            "07 / Release to commit. The stack now belongs to the case.",
            "08 / Weapon inside an ammunition case: rejected by the data policy.",
            "09 / Split rig: ten independent compartments, twenty usable cells.",
            "10 / Each item must fit inside one compartment. Gaps are not targets.",
            "11 / Release inside a small pocket. One container, several sections.",
            "12 / Merge 20 into 40, with a maximum stack of 50.",
            "13 / Partial merge: 50 + 10. Right-click and choose a split quantity.",
            "14 / Pick up five. Nothing is created until an empty-cell drop.",
            "15 / Split committed. Rotate a rifle preview with R.",
            "16 / Rotated placement committed; occupied cells match the footprint.",
            "17 / Esc cancels a new preview. Delete removes the selected part.",
            "18 / Reset atomically rebuilds independent demo instances. Ready." };
        public void Dispose() { inventory.Interaction.Cancel(); inventory.Indicator.Show(Vector2.zero, false); }
    }
}
