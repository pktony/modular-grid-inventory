# Modular Grid Inventory — Quick Start

This is a source package for desktop grid inventories. The 0.1.0 candidate includes nested containers, separated pockets, category policies, rotation, stacks, movable container windows, optional sounds and Inspector authoring.

## Requirements

- Candidate editor: Unity 6000.6.4f1. Other versions remain unverified.
- Unity uGUI (`com.unity.ugui`, 2.6.0 in the development project), including TextMeshPro.
- TMP Essential Resources installed by the host. If absent, use **Window → TextMeshPro → Import TMP Essential Resources** before running a UI scene.
- Screen Space Overlay Canvas, one host EventSystem and a compatible UI input module.
- Mouse and keyboard. The provided view uses a 1280 × 720 reference layout with CanvasScaler.

Unity MCP, rounded-corner plugins and recording tools are not dependencies. Importing this package does not install packages or change project settings.

## Run the sample

1. Import the core `.unitypackage` into a project with uGUI installed.
2. Open `Samples/Scenes/Inventory.unity` and enter Play Mode.
3. Double-click a pack, case or carrier to open its window. Drag its title bar to move it.
4. Drag an item onto a pocket or onto a container icon. Press R to rotate and Esc to cancel.
5. Drop matching ammunition together to merge. Use Split to choose a partial quantity.

The sample has 19 definitions, 22 instances and 10 distinct carrier layouts. Packs accept nested containers; cases demonstrate category restrictions. Reset replaces all sample instances and closes their windows.

For Input System only projects, import the separate Input System add-on after installing your project's compatible `com.unity.inputsystem` version. Add `InputSystemInventoryInputSource` to the bootstrap object and assign it to Input Source. Use `InputSystemUIInputModule` instead of `StandaloneInputModule` on the EventSystem. The package never changes Active Input Handling automatically.

## Integrate into your scene

Drag `Samples/Prefabs/ModularInventory.prefab` into your scene. It contains an overlay Canvas, CanvasScaler, GraphicRaycaster and configured bootstrapper. It does not create an EventSystem or AudioListener; use your host scene's existing ones. Select the bootstrapper and click **Validate installation**.

Create your own Category, Item, Container, Layout, Catalog, Initial State and UI Theme assets using **Assets → Create → Modular Grid Inventory**. Assign them to the bootstrapper. Leave Initial State empty for an empty inventory, and Audio Settings empty for silent operation.

| Asset | What it configures |
|---|---|
| Item | Stable definition ID, display name, category, width, height, max stack, icon, internal container |
| Container | Independent pocket dimensions, acceptance policy and layout |
| Layout | Each pocket's position in units of grid cells; gaps are allowed |
| Initial State | Unique key, item, quantity, parent key, pocket ID, x/y and rotation |
| UI Theme | TMP font, palette, cell pitch/gap, heading, storage title, optional click indicator |
| Audio Settings | Master volume, mute, action clips and category sound profiles |

In Initial State, an empty parent key means root storage. A nonempty parent key names another seed entry whose item must be a container. Entry order is arbitrary; missing parents, cycles, invalid quantities and invalid placement are rejected.

Definitions, initial state and theme values are captured for the session. Restart Play Mode after editing configuration. Container Inspector changes have an immediate layout preview and support Undo/Redo. The catalog table accepts catalogs at arbitrary asset paths.

Keep user-created data in your own folder so updating the supplied samples cannot replace your edits. To customize the entire view, use the public domain services and your own presentation; see `API.md` and the model-only sample.

## Troubleshooting

- Input does nothing: check the EventSystem module and input adapter against Active Input Handling.
- Text is missing: assign the Theme font and verify uGUI/TMP installation. The sample font uses its bundled UI SDF shader.
- No sound: assign Audio Settings, check mute/volume and use one enabled AudioListener.
- Drop rejected: read the status message; pocket bounds, occupancy, stack limits, category rules and container ancestry are enforced.
- Asset Store readiness: the development sample and regression suites passed in Unity 6000.6.4f1; clean consumer import, compatibility matrix and Publisher validator checks remain. See `ReleaseChecklist.md`.
