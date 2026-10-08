using System;
namespace Pktony.GridInventory
{
    public sealed class ContainerDefinitionSnapshotFactory
    {
        public ContainerDefinitionView Create(ContainerDefinition source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return source.Freeze();
        }
    }
}
