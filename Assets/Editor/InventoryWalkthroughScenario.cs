using InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InventorySystem.Editor
{
    public sealed class InventoryWalkthroughScenario : System.IDisposable
    {
        private readonly Inventory inventory;
        private readonly InventoryGridGeometry geometry;
        private ItemData item;
        private readonly WalkthroughPointerView cursor;
        private Vector2 cursorPosition, stageStart, pressPosition;
        private int currentStage=-1;
        private bool acted, dragging;
        private GameObject draggedObject;
        public InventoryWalkthroughScenario(Inventory inventory)
        {
            this.inventory = inventory;
            geometry = new InventoryGridGeometry(GameObject.Find("Grid").GetComponent<RectTransform>(), 80);
            cursor=new WalkthroughPointerView(GameObject.Find("DragOverlay").GetComponent<RectTransform>());
            cursorPosition=Pointer(0,0).position;
        }
        private PointerEventData Pointer(int x, int y)
        {
            var p = RectTransformUtility.WorldToScreenPoint(null, geometry.WorldPosition(x, y) + new Vector3(8, -8));
            return new PointerEventData(EventSystem.current) { position = p, pressPosition = p };
        }
        private void Begin(int x, int y)
        {
            item = inventory.Model.GetItemAt(x, y);
            cursorPosition=Pointer(x,y).position; pressPosition=cursorPosition;
            draggedObject=GameObject.Find(item.itemId); dragging=true;
            ExecuteEvents.Execute(draggedObject,Pointer(x,y),ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(draggedObject,Pointer(x,y),ExecuteEvents.beginDragHandler);
        }
        private void End(int x, int y)
        { ExecuteEvents.Execute(draggedObject,Pointer(x,y),ExecuteEvents.endDragHandler); dragging=false; }
        private void Reset() => GameObject.Find("Reset").GetComponent<Button>().onClick.Invoke();
        public void Apply(int stage)
        {
            switch (stage)
            {
                case 1: item = inventory.Model.GetItemAt(0, 0); inventory.View.PointerEvents.Select(item, Pointer(0, 0)); break;
                case 2: Begin(0, 0); break;
                case 3: End(0, 3); break;
                case 4: Reset(); break;
                case 5: Begin(8, 1); break;
                case 6: inventory.Interaction.Rotate(); break;
                case 7: End(2, 3); break;
                case 8: Begin(2, 3); break;
                case 9: End(5, 2); break;
                case 10: Begin(2, 3); break;
                case 11: inventory.Interaction.Cancel(); dragging=false; break;
                case 12: inventory.View.PointerEvents.Select(inventory.Model.GetItemAt(7, 2), Pointer(7, 2)); inventory.Interaction.RemoveSelected(); break;
                case 13: GameObject.Find("AddItem").GetComponent<Button>().onClick.Invoke(); break;
                case 14: Reset(); break;
            }
            string[] captions = {"01 / Different sizes. One grid. 7 items, 180 cells.", "02 / Select an item to inspect its size and orientation.",
                "03 / Drag to a valid area. Green cells preview the placement.", "04 / Release to commit. The original cells are now free.",
                "05 / Reset restores the starting layout.", "06 / Pick up a vertical pistol.", "07 / R rotates the preview while keeping the grabbed cell.",
                "08 / Release to commit the rotated placement.", "09 / Overlap is rejected. Red cells show the blocked area.",
                "10 / Rejected drop preserves the original placement.", "11 / Move again. The model remains unchanged during preview.",
                "12 / Esc cancels the preview and restores the original item.", "13 / Delete removes the selected item and releases its cells.",
                "14 / Add creates an independent instance in the next available space.", "15 / Reset. Ready for the next round."};
            inventory.View.SetStatus(captions[stage]);
        }
        private Vector2 ButtonPoint(string name)
        {
            var rect=GameObject.Find(name).GetComponent<RectTransform>();
            return RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(rect.rect.center));
        }
        private Vector2 Target(int stage) => stage switch {
            1 or 2=>Pointer(0,0).position,3=>Pointer(0,3).position,4 or 14=>ButtonPoint("Reset"),
            5=>Pointer(8,1).position,7=>Pointer(2,3).position,9=>Pointer(5,2).position,
            8 or 10=>Pointer(2,3).position,12=>Pointer(7,2).position,13=>ButtonPoint("AddItem"),_=>cursorPosition
        };
        public void Tick(float elapsed)
        {
            int stage=Mathf.Min(14,(int)(elapsed/4)); float phase=elapsed-stage*4;
            if(stage!=currentStage) {currentStage=stage;acted=false;stageStart=cursorPosition;}
            if(!acted) cursorPosition=Vector2.Lerp(stageStart,Target(stage),Mathf.SmoothStep(0,1,Mathf.Clamp01(phase)));
            if(!acted && phase>=1) {acted=true;Apply(stage);stageStart=cursorPosition;}
            if(acted && (stage==2 || stage==6 || stage==8 || stage==10))
            {
                var target=stage switch {2=>Pointer(0,3).position,6=>Pointer(2,3).position,8=>Pointer(5,2).position,_=>Pointer(6,3).position};
                cursorPosition=Vector2.Lerp(stageStart,target,Mathf.SmoothStep(0,1,Mathf.Clamp01((phase-1)/1.6f)));
            }
            if(dragging)
                ExecuteEvents.Execute(draggedObject,new PointerEventData(EventSystem.current) {
                    position=cursorPosition,pressPosition=pressPosition,button=PointerEventData.InputButton.Left
                },ExecuteEvents.dragHandler);
            string action=dragging ? "HOLD" : "";
            if(acted && phase<2 && (stage==1 || stage==4 || stage==13 || stage==14)) action="CLICK";
            if(acted && stage==6) action="R / HOLD";
            if(acted && stage==11) action="ESC";
            if(acted && stage==12) action="DELETE";
            cursor.Show(cursorPosition,action,dragging || action=="CLICK",elapsed);
        }
        public void Dispose() => cursor.Dispose();
    }
}
