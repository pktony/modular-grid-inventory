using Pktony.GridInventory.InputAdapters;
using System;
using System.Collections.Generic;
using Pktony.GridInventory.Presentation;
using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public sealed class InventoryInstallationValidator
    {
        public IReadOnlyList<string> Validate(SerializedObject data)
        {
            var errors = new List<string>();
            var catalog = data.FindProperty("catalog").objectReferenceValue as InventoryCatalogAsset;
            var root = data.FindProperty("rootContainer").objectReferenceValue as ContainerDefinition;
            var canvas = data.FindProperty("canvas").objectReferenceValue as Canvas;
            var theme = data.FindProperty("theme").objectReferenceValue as InventoryUiTheme;
            if (canvas == null || canvas.renderMode != RenderMode.ScreenSpaceOverlay) errors.Add("Assign a Screen Space Overlay Canvas.");
            if (theme == null || theme.Font == null) errors.Add("Assign a UI theme with a TMP font.");
            if (theme != null && (theme.CellPitch < 32 || theme.CellPitch > 64 || theme.CellGap < 1 || theme.CellGap > 6))
                errors.Add("Cell pitch must be 32–64 and cell gap must be 1–6.");
            var source = data.FindProperty("inputSource").objectReferenceValue as InventoryInputSourceBehaviour;
            if (source == null && !new LegacyInventoryInputSource().IsAvailable)
                errors.Add("Assign the Input System adapter and use an InputSystemUIInputModule on your EventSystem.");
            try
            {
                var definitions = new InventoryCatalogSnapshotFactory().Create(catalog);
                var container = new ContainerDefinitionSnapshotFactory().Create(root);
                new InventorySeedBuilder().Create(definitions, container,
                    data.FindProperty("initialState").objectReferenceValue as InventoryInitialState);
            }
            catch (Exception error) { errors.Add(error.Message); }
            return errors;
        }
    }
}
