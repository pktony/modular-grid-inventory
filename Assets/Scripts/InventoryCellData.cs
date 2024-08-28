using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InventorySystem
{
    public class InventoryCellData
    {
        public int capacityWidth;
        public int capacityHeight;

        public ItemData[] itemData;

        public bool[] isAssigned;

        public InventoryCellData(int capacityWidth, int capacityHeight)
        {
            this.capacityWidth = capacityWidth;
            this.capacityHeight = capacityHeight;

            // this.itemData = itemData;
            itemData = new ItemData[capacityWidth * capacityHeight];
            RecalculateAssignment();
        }

        public void AddItem(int coordinate, ItemData itemData)
        {
            this.itemData[coordinate] = itemData;

            RecalculateAssignment();
        }

        public int GetNextEmptyIndex(ItemData insertingItemData)
        {
            for (int i = 0; i < isAssigned.Length; i++)
            {
                if (CheckIsAssigned(i, insertingItemData))
                    continue;

                return i;
            }

            return -1;
        }

        private bool CheckIsAssigned(int index, ItemData itemData)
        {
            var (absoluteWidth, absoluteHeight) = itemData.GetAbsoluteSize();

            //check width overflow
            if ((index % capacityWidth) + (absoluteWidth - 1) >= capacityWidth) // position cursor + item width 
                return true;

            //check height overflow
            if (index + ((absoluteHeight - 1) * capacityWidth) >= isAssigned.Length)
                return true;

            // iterate items
            for (int x = 0; x < absoluteWidth; x++)
            {
                for (int y = 0; y < absoluteHeight; y++)
                {
                    if (isAssigned[index + x + (y * capacityWidth)])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void RecalculateAssignment()
        {
            isAssigned = new bool[capacityWidth * capacityHeight];

            for (var i = 0; i < itemData.Length; i++)
            {
                if (itemData[i] == null || isAssigned[i])
                    continue;

                var width = itemData[i].itemDirection == ItemDirection.Horizontal ?
                    itemData[i].width : itemData[i].height;
                var height = itemData[i].itemDirection == ItemDirection.Horizontal ?
                    itemData[i].height : itemData[i].width;

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var checkIndex = i + x + (y * capacityWidth);
                        if (checkIndex >= isAssigned.Length)
                            continue;

                        isAssigned[checkIndex] = true;
                    }
                }
            }
        }
    }
}