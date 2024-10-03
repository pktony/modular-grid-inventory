using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventorySystem.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private RectTransform inventoryAttachPoint;

        private CellUI[] cellUIs;

        private int completeCount;
        private int cellCount;

        private int width;
        private int height;

        private GridLayoutGroup layoutGroup;

        private InventoryCellData inventoryData;

        public Action<ItemData> OnClickedCell;

        public void Initialize(InventoryCellData inventoryData)
        {
            this.inventoryData = inventoryData;

            layoutGroup = inventoryAttachPoint.GetComponent<GridLayoutGroup>();
            layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layoutGroup.constraintCount = inventoryData.capacityWidth;
            layoutGroup.cellSize = new Vector2(66, 66);

            height = inventoryData.capacityHeight;
            width = inventoryData.capacityWidth;
            cellCount = height * width;
            CreateCellUIs();
        }

        public void SetListeners(Action<ItemData> onClickedCell)
        {
            this.OnClickedCell = onClickedCell;
        }

        private void CreateCellUIs()
        {
            cellUIs = new CellUI[width * height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    InstantiateCellUI(i, j);
                }
            }
        }

        private async void InstantiateCellUI(int xCoordinate, int yCoordinate)
        {
            var cell = Resources.LoadAsync<CellUI>("Cell");
            while (!cell.isDone) await Task.Yield();
            
            var cellAsset = cell.asset as CellUI;
            var cellUI = Instantiate(cellAsset);
            cellUI.SetListeners(OnClickedCell);
            cellUI.InitializeCells(xCoordinate, yCoordinate);

            cellUIs[xCoordinate + yCoordinate * width] = cellUI;

            completeCount++;

            TryPositionCellUIs();
        }

        private void TryPositionCellUIs()
        {
            if (completeCount < cellCount) return;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    SetParent(cellUIs[i + j * width].transform);
                }
            }

            Debug.Log($"Cell UI Created Succesfully - cellCount : {cellCount}/{cellUIs.Length}, completeCount : {completeCount}");
            AssignItemData();
        }

        private void AssignItemData()
        {   
            for (int x = 0; x < inventoryData.itemData.Length; x++)
            {
                var itemData = inventoryData.itemData[x];
                if (itemData == null)
                    continue;
                
                cellUIs[x].AssignItem(itemData, OnAssignedItem);
            }
        }

        private void OnAssignedItem(CellUI cellUI, string itemId)
        {
            Debug.Log($"OnAssignedItem: {itemId}");

            cellUI.SetItemImage(ResourceUtility.LoadSprite($"Weapons/{itemId}"));
        }

        public void SetParent(Transform cell)
        {
            cell.SetParent(inventoryAttachPoint);
        }
    }
}