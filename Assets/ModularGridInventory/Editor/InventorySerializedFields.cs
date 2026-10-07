using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public static class InventorySerializedFields
    {
        public static void String(SerializedObject data, string property, string value) => data.FindProperty(property).stringValue = value;
        public static void References(SerializedProperty property, Object[] references)
        {
            property.arraySize = references.Length;
            for (int i = 0; i < references.Length; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = references[i];
        }
    }
}
