using System.Collections.Generic;
namespace Pktony.GridInventory.Presentation
{
    public interface IInventoryPanelSource
    {
        IEnumerable<InventoryPanelBindings> FrontToBack { get; }
    }
}
