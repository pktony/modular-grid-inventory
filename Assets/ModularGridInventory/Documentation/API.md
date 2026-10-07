# Public API and Ownership

Namespaces and assemblies use `Pktony.GridInventory`.

| Layer | Responsibility |
|---|---|
| Catalog | ScriptableObject authoring, validation and immutable definition snapshots |
| Domain | Session instances, atomic mutations, placement, stacks and container ancestry |
| Presentation | Views, interaction, windows, visual feedback and audio |
| Integration | Bootstrap composition, initial-state construction and UI lifetime |
| Editor | Inspector authoring, catalog selection, installation checks and explicit export |
| Samples | Examples using the public API; no dependency from Runtime to Samples |

## Model without a view

```csharp
var definitions = new InventoryCatalogSnapshotFactory().Create(catalogAsset);
var root = new ContainerDefinitionSnapshotFactory().Create(rootAsset);
using var inventory = new InventoryRuntime(definitions, root);

var target = new PlacementTarget(inventory.ReadModel.Snapshot.RootContainerId,
    new GridSectionId("main"), 0, 0);
var result = inventory.Edit.Add(new AddRequest(new DefinitionId("ammo-light"), 20, target));
if (!result.Success) Debug.Log(result.Reason);
```

Choose an ID present in your own catalog. See `Samples/ModelOnly/InventoryModelExample.cs` for a scene component with no Canvas, EventSystem or audio objects.

## Services

| Contract | Operation |
|---|---|
| `IInventoryReadModel` | Immutable `Snapshot` and committed `Changed` batches |
| `IInventoryEditService` | Add definitions as instances; delete instances |
| `IInventoryTransferService` | Preview and transfer a whole instance |
| `IInventoryStackService` | Preview/commit merge and split |
| `IInventoryStorageService` | Preview/commit automatic placement in a container |
| `InventoryResetService` | Replace the complete inventory state atomically |

Mutations return `MutationResult`: Success, Reason, MovedQuantity and CreatedItemId. A rejected operation keeps the previous snapshot. React to committed changes through `ReadModel.Changed`; unsubscribe when the subscriber is disposed.

Definition IDs identify shared immutable item configuration. Instance IDs identify individual items. Container IDs identify independent inventories. Opening a second instance of the same bag definition opens a different window; reopening the same instance reuses its window. Moving a bag preserves its contents and child container ID. Reset creates new instance and container IDs, so cached runtime handles must be refreshed.

Do not write model internals, move visuals to commit state, or store mutable session state in ScriptableObjects. The mutation pipeline owns validation and commit ordering. The hierarchy rules reject storing a container inside itself or any descendant. Pockets remain separate logical grids even when their visuals touch.

## Bootstrap and UI

`InventoryBootstrapper` owns one runtime and its composed UI/audio objects. `InventorySeedBuilder` constructs a validated snapshot from explicit data. `InventoryUiInstaller` composes and disposes the default presentation. `InventoryUiSettings` captures font, palette and grid values per installation; editing a shared source theme does not change existing session geometry.

The default view supports Screen Space Overlay and uses the sample CanvasScaler reference layout. Camera/world-space canvases, touch, gamepad navigation, localization and UI Toolkit are outside this release candidate's tested scope. The bundled SDF shader supplies basic face rendering and UI clipping; use your own compatible TMP material for advanced outline/underlay styling.

Input is supplied through `IInventoryInputSource.Read()` returning `InventoryInputFrame`. Custom MonoBehaviour adapters derive from `InventoryInputSourceBehaviour`. The Legacy adapter is guarded by `ENABLE_LEGACY_INPUT_MANAGER`; the optional Input System adapter compiles in its own assembly. UI pointer events still come through the host EventSystem.

Patterns are Composition Root, Factory, MVP, Observer and Adapter. There is no global inventory registry or service locator. The repository's `Assets/Development` tools own capture scenarios, build settings and demonstrations; they are excluded from the product package.
