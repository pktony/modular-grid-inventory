using System;
using System.Linq;
using InventorySystem.Domain;
using InventorySystem.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InventorySystem
{
    public sealed class InventoryWalkthroughScenario : IDisposable
    {
        public const int StageCount = 27;
        public const float StageDuration = 1.5f;
        public const int FramesPerStage = 45;
        public const int FrameCount = StageCount * FramesPerStage;
        public const float Duration = FrameCount / 30f;
        private readonly ExpandedInventory inventory;
        private int stage = -1;
        private GameObject dragged;
        private PointerEventData data;
        private Vector2 start, destination, pointer;
        private bool holding, cancelPreviewStarted;
        private float flashUntil, dragStarted;
        private ItemInstanceId firstBag, secondBag, nested;
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
            var panel = container == Root ? inventory.Screen.Stash : inventory.Windows.Find(container).Panel;
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
            ExecuteEvents.Execute(View(id), e, ExecuteEvents.pointerClickHandler); flashUntil = time + 0.2f;
        }
        private void Click(Button button, float time)
        {
            pointer = RectTransformUtility.WorldToScreenPoint(null, button.transform.TransformPoint(new Vector3(30, -16)));
            var e = Event(pointer); ExecuteEvents.Execute(button.gameObject, e, ExecuteEvents.pointerClickHandler); flashUntil = time + 0.2f;
        }
        private void Begin(ItemInstanceId id, Vector2 target, float time)
        {
            pointer = start = ItemPoint(id); destination = target; dragged = View(id); data = Event(pointer); holding = true; dragStarted = time;
            ExecuteEvents.Execute(dragged, data, ExecuteEvents.pointerDownHandler); ExecuteEvents.Execute(dragged, data, ExecuteEvents.beginDragHandler);
        }
        private ContainerWindowPresenter Window(ItemInstanceId id) => inventory.Windows.Find(State.Items[id].ChildContainerId);
        private void MoveWindow(ItemInstanceId id, Vector2 position, float time)
        {
            var window = Window(id); dragged = window.Header.gameObject;
            pointer = start = RectTransformUtility.WorldToScreenPoint(null, window.Header.TransformPoint(new Vector3(35, -12)));
            var desktop = window.Panel.Root.parent;
            var delta = position - window.Panel.Root.anchoredPosition;
            destination = start + RectTransformUtility.WorldToScreenPoint(null, desktop.TransformPoint(delta)) - RectTransformUtility.WorldToScreenPoint(null, desktop.position);
            data = Event(pointer); holding = true; dragStarted = time;
            ExecuteEvents.Execute(dragged, data, ExecuteEvents.pointerDownHandler); ExecuteEvents.Execute(dragged, data, ExecuteEvents.beginDragHandler);
        }
        private void End()
        { data.position = destination; ExecuteEvents.Execute(dragged, data, ExecuteEvents.dragHandler); ExecuteEvents.Execute(dragged, data, ExecuteEvents.endDragHandler); pointer = destination; holding = false; }
        private void Scroll(float value = 0.65f) { inventory.Screen.Stash.Scroll.verticalNormalizedPosition = value; Canvas.ForceUpdateCanvases(); }
        public void Tick(float time)
        {
            int next = Mathf.Min(StageCount - 1, Mathf.FloorToInt(time / StageDuration));
            if (next != stage) { stage = next; Apply(stage, time); }
            if (stage == 23 && time - stage * StageDuration > 0.8f && !cancelPreviewStarted)
            {
                cancelPreviewStarted = true; Begin(rifle, Point(Root, main, 4, 14), time);
            }
            if (holding)
            {
                float t = Mathf.Clamp01((time - dragStarted - 0.06f) / 0.48f);
                float progress = 1 - Mathf.Pow(1 - t, 3);
                pointer = Vector2.Lerp(start, destination, progress); data.position = pointer;
                if (inventory.Interaction.Drag?.SplitQuantity > 0) inventory.Interaction.UpdatePointer(pointer);
                else ExecuteEvents.Execute(dragged, data, ExecuteEvents.dragHandler);
            }
            if (stage == 26 && holding && time - dragStarted > 0.65f) End();
            inventory.Indicator.Show(pointer, holding || time < flashUntil);
            inventory.Screen.Status.text = Captions[stage];
        }
        private void Apply(int value, float time)
        {
            switch (value)
            {
                case 0:
                    inventory.ResetDemo();
                    var bags = State.Items.Values.Where(i => i.Definition.Identifier == "berkut").ToArray();
                    firstBag = bags[0].Id; secondBag = bags[1].Id; break;
                case 1: ClickItem(firstBag, time, 2); Canvas.ForceUpdateCanvases(); break;
                case 2: ClickItem(secondBag, time, 2); Canvas.ForceUpdateCanvases(); break;
                case 3: MoveWindow(secondBag, new Vector2(274, -110), time); break;
                case 4: End(); nested = Find("mbss"); ClickItem(nested, time, 2); Canvas.ForceUpdateCanvases(); break;
                case 5: MoveWindow(nested, new Vector2(520, -110), time); break;
                case 6: End(); ClickItem(firstBag, time, 2); break;
                case 7: MoveWindow(firstBag, new Vector2(48, -230), time); break;
                case 8: End(); Begin(nested, Point(State.Items[secondBag].ChildContainerId, main, 0, 0), time); break;
                case 9: End(); break;
                case 10: Begin(Find("ai2", State.Items[nested].ChildContainerId), ItemPoint(firstBag), time); break;
                case 11: End(); break;
                case 12:
                    foreach (var window in inventory.Windows.Windows.Values.ToArray()) Click(window.Close, time);
                    Scroll(); ClickItem(Find("ammo-case", Root), time, 2); Canvas.ForceUpdateCanvases(); break;
                case 13: Begin(Find("pst", Root, 20), Point(Window(Find("ammo-case", Root)).Id, main, 0, 0), time); break;
                case 14: End(); rifle = Find("aks74u", Root); Begin(rifle, ItemPoint(Find("ammo-case", Root)), time); break;
                case 15: End(); ClickItem(Find("rig", Root), time, 2); Canvas.ForceUpdateCanvases(); break;
                case 16: MoveWindow(Find("rig", Root), new Vector2(398, -110), time); break;
                case 17: End(); Begin(Find("pst", Root, 40), Point(Window(Find("rig", Root)).Id, new GridSectionId("small-a"), 0, 0), time); break;
                case 18: End(); break;
                case 19:
                    Click(inventory.Screen.Reset, time); Scroll(); mergeTarget = Find("pst", Root, 40);
                    Begin(Find("pst", Root, 20), ItemPoint(mergeTarget), time); break;
                case 20:
                    End(); ClickItem(mergeTarget, time, 1, true); Click(GameObject.Find("SplitStack").GetComponent<Button>(), time);
                    GameObject.Find("Input").GetComponent<TMPro.TMP_InputField>().text = "5"; break;
                case 21:
                    Click(GameObject.Find("Confirm").GetComponent<Button>(), time);
                    holding = true; start = pointer; destination = Point(Root, main, 8, 11); data = Event(pointer); dragStarted = time; break;
                case 22:
                    pointer = destination; holding = false; ExecuteEvents.Execute(inventory.Screen.Stash.Viewport.gameObject, Event(pointer), ExecuteEvents.pointerClickHandler);
                    Scroll(0.3f); rifle = Find("aks74u", Root); Begin(rifle, Point(Root, main, 1, 14), time); inventory.Interaction.Rotate(); break;
                case 23: End(); Scroll(0.1f); break;
                case 24:
                    inventory.Interaction.Cancel(); holding = false;
                    ClickItem(Find("rk0", Root), time); Click(inventory.Screen.Delete, time); break;
                case 25:
                    Click(inventory.Screen.Reset, time);
                    bags = State.Items.Values.Where(i => i.Definition.Identifier == "berkut").ToArray(); firstBag = bags[0].Id; secondBag = bags[1].Id;
                    ClickItem(firstBag, time, 2); ClickItem(secondBag, time, 2); Canvas.ForceUpdateCanvases(); break;
                case 26: MoveWindow(secondBag, new Vector2(274, -110), time); break;
            }
        }
        private static readonly string[] Captions = {
            "01 / Ten definitions. Independent item instances.",
            "02 / Double-click a backpack to open its inventory.",
            "03 / Several containers stay open together.",
            "04 / Drag a title bar. The active window comes forward.",
            "05 / A backpack inside a backpack has its own window.",
            "06 / Arrange windows freely for quick transfers.",
            "07 / Reopening a bag focuses its existing window.",
            "08 / Moving a window preserves its item placements.",
            "09 / Drag a filled backpack into another window.",
            "10 / Contents and the open window keep the same identity.",
            "11 / Drop onto a bag icon to store an item automatically.",
            "12 / The first permitted free space receives the item.",
            "13 / Close individual windows. Open an ammunition case.",
            "14 / Drag ammunition directly into the case window.",
            "15 / A weapon over the case icon shows a rejected preview.",
            "16 / Release is rejected. Open a rig beside the case.",
            "17 / Bring the rig forward and move its window.",
            "18 / Ten separate compartments. Gaps cannot receive items.",
            "19 / Ammunition occupies one small pocket.",
            "20 / Merge twenty into forty, with a limit of fifty.",
            "21 / Fifty plus ten remain. Split five in the quantity dialog.",
            "22 / A split preview creates no item until release.",
            "23 / Commit five rounds. Press R to rotate the rifle preview.",
            "24 / The rotated footprint is committed on release.",
            "25 / Esc cancels a preview. Delete removes the selected part.",
            "26 / Reset rebuilds the demo and closes its old windows.",
            "27 / Ready: draggable containers and both transfer paths." };
        public void Dispose() { inventory.Interaction.Cancel(); inventory.Indicator.Show(Vector2.zero, false); }
    }
}
