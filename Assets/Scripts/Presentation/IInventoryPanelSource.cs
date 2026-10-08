using System.Collections.Generic;
namespace InventorySystem.Presentation
{
    public interface IInventoryPanelSource
    {
        IEnumerable<InventoryPanelBindings> FrontToBack { get; }
    }
}
