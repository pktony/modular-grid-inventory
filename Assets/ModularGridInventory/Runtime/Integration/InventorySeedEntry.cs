using System;
using UnityEngine;
namespace Pktony.GridInventory
{
    [Serializable]
    public sealed class InventorySeedEntry
    {
        [SerializeField] private string key;
        [SerializeField] private ItemDefinition item;
        [SerializeField, Min(1)] private int quantity = 1;
        [SerializeField] private string parentKey;
        [SerializeField] private string section = "main";
        [SerializeField, Min(0)] private int x;
        [SerializeField, Min(0)] private int y;
        [SerializeField] private bool rotated;
        public string Key => key;
        public ItemDefinition Item => item;
        public int Quantity => quantity;
        public string ParentKey => parentKey;
        public string Section => section;
        public int X => x;
        public int Y => y;
        public bool Rotated => rotated;
    }
}
