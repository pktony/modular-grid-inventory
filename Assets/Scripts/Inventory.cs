using System;
using System.Threading.Tasks;
using UnityEngine;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        private InventoryUI inventoryUI;

        private Cell[] cells;

        private int width = 9;
        private int height = 20;

        private void Awake()
        {
            InitializeInventoryData();
        }

        // ######################## 인벤토리 초기화 ##############################
        public void InitializeInventoryData()
        {
            InventoryCellData inventoryData = new(width, height);
            cells = new Cell[width * height];

            var testItem = new ItemData("test_01", 3, 2);
            var emptyIndex = inventoryData.GetNextEmptyIndex(testItem);
            inventoryData.AddItem(emptyIndex, testItem);

            var testItem2 = new ItemData("test_02", 3, 2);
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem2), testItem2);

            var testItem3 = new ItemData("test_03", 5, 1);
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem3), testItem3);

            var testItem4 = new ItemData("test_03", 4, 2);
            testItem4.itemDirection = ItemDirection.Vertical;
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem4), testItem4);

            var testItem5 = new ItemData("test_05", 1, 1);
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem5), testItem5);

            var testItem6 = new ItemData("test_06", 2, 1);
            testItem6.itemDirection = ItemDirection.Vertical;
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem6), testItem6);

            var testItem7 = new ItemData("test_07", 2, 1);
            testItem7.itemDirection = ItemDirection.Vertical;
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem7), testItem7);

            inventoryUI = FindObjectOfType<InventoryUI>();
            inventoryUI.Initialize(inventoryData);
            inventoryUI.SetListeners(OnClickedCell);
        }

        private void OnClickedCell(ItemData data)
        {
            Debug.Log($"Inventory: OnClickedCell - {data.itemId}");
        }

        private void InitializeCellData(int i, int j)
        {
            
        }

        // ######################## 인벤토리 데이터 초기화 ##############################
        private async void LoadUserInventoryData()
        {
            var inventoryAsset = Resources.LoadAsync<TextAsset>("InventoryData");
            while (!inventoryAsset.isDone) await Task.Yield();

            var data = inventoryAsset.asset as TextAsset;
            Debug.Log($"Inventory: data - {data.text}");
            // var inventoryData = JsonUtility.FromJson<InventoryData>(data.text);
            

            // foreach (var cell in inventoryData.cells)
            // {
            //     var cellIndex = cell.cellIndex;
            //     cellUIs[cellIndex].AssignItem(cell.itemId);
            // }

            // for (int i = 0, cellCount = inventoryData.cells.Count; i < cellCount; i++)
            // {
            //     var cellData = inventoryData.cells[i];
            //     Debug.Log($"Inventory: cellData - {i} -- {cellData.itemId}");
            //     cellUIs[i].AssignItem(cellData.itemId, LoadImage);
            // }
        }

        private async void LoadImage(CellUI cellUI, string itemId)
        {
            var spriteLoadHandler = Resources.LoadAsync<Sprite>($"Sprites/{itemId}");
            while (!spriteLoadHandler.isDone) await Task.Yield();

            if (spriteLoadHandler.asset == null) return;
            if (spriteLoadHandler.asset is not Sprite) return;

            // var image = spriteLoadHandler.asset as Sprite;
            // cellUI.SetItemImage(image);
            cellUI.Test(itemId);
        }
    }
}
