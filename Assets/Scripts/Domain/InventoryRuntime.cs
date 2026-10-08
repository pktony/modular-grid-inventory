using System;
using System.Collections.Generic;
using System.Linq;
namespace InventorySystem.Domain
{
    public sealed class InventoryRuntime : IDisposable
    {
        public IInventoryReadModel ReadModel { get; }
        public IInventoryTransferService Transfer { get; }
        public IInventoryStackService Stack { get; }
        public IInventoryEditService Edit { get; }
        public InventoryResetService Reset { get; }
        private readonly InventorySession session;
        public InventoryRuntime(InventoryCatalog catalog, ContainerDefinitionView rootDefinition, Action<Exception> report = null)
        {
            var errors = new List<string>();
            new ContainerDefinitionValidator().Validate(rootDefinition, new HashSet<string>(catalog.Categories.Select(c => c.Id)),
                new HashSet<DefinitionId>(catalog.Definitions.Select(i => i.Id)), errors);
            if (errors.Count > 0) throw new CatalogValidationException(errors);
            var placement = new InventoryPlacementRules(new AcceptanceRules(catalog), new ContainerHierarchyRules());
            var publisher = new InventoryChangePublisher(report);
            var draft = new InventoryDraft(new ContainerId(Guid.NewGuid().ToString("N")), rootDefinition);
            session = new InventorySession(new InventorySnapshot(0, draft), publisher);
            var pipeline = new InventoryMutationPipeline(session, new InventoryInvariantValidator(placement, catalog),
                new InventoryMutationCommitter(session, publisher));
            var factory = new ItemInstanceFactory();
            ReadModel = session;
            Transfer = new InventoryTransferService(session, pipeline, placement);
            Stack = new InventoryStackService(session, pipeline, new StackRules(), placement, factory);
            Edit = new InventoryEditService(catalog, pipeline, placement, factory);
            Reset = new InventoryResetService(pipeline);
        }
        public void Dispose() => session.Dispose();
    }
}
