using System;
namespace Pktony.GridInventory.Domain
{
    internal sealed class ItemInstanceFactory
    {
        internal ItemInstance Prepare(ItemDefinitionView definition, int quantity) => new(
            new ItemInstanceId(Guid.NewGuid().ToString("N")), definition, quantity,
            definition.Container == null ? default : new ContainerId(Guid.NewGuid().ToString("N")));
    }
}
