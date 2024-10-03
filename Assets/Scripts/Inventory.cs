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

            var testItem = new ItemData("assault rifle 1", 4, 2);
            var emptyIndex = inventoryData.GetNextEmptyIndex(testItem);
            inventoryData.AddItem(emptyIndex, testItem);

            var testItem2 = new ItemData("assault rifle 2", 4, 2);
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem2), testItem2);

            var testItem3 = new ItemData("Grenade launcher 3", 5, 1);
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem3), testItem3);

            var testItem4 = new ItemData("assault rifle 3", 4, 2);
            testItem4.itemDirection = ItemDirection.Vertical;
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem4), testItem4);

            var testItem5 = new ItemData("test_05", 1, 1);
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem5), testItem5);

            var testItem6 = new ItemData("Pistol 1", 2, 1);
            testItem6.itemDirection = ItemDirection.Vertical;
            inventoryData.AddItem(inventoryData.GetNextEmptyIndex(testItem6), testItem6);

            var testItem7 = new ItemData("Pistol 2", 2, 1);
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
    }
}
