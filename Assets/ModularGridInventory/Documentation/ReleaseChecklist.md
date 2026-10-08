# 0.1.0 Candidate Validation

Validated on Windows with Unity 6000.6.4f1. Local testing and Asset Store review are separate processes.

| Check | Current evidence |
|---|---|
| Package/development separation | Explicit asset root; capture/build tools outside it |
| C# compilation | Offline compiler against installed Unity 6000.6.4f1 and cached uGUI assemblies |
| Existing tests | Unity 6000.6.4f1: Edit Mode 101/101, Play Mode 24/24 passed |
| New integration tests | All 10 passed within the Edit Mode suite; seed ordering, missing parents, cycles, noncontainer parents, atomic failure, independent IDs and frozen themes |
| Icons | 19 generated RGBA PNGs, preserved asset GUIDs, recorded hashes |
| Unity asset import / shader render | Native export of 293 assets imported into a fresh URP consumer; Built-in development sample rendered with icons, text and windows |
| New consumer project / settings preservation | Fresh URP Play Mode 24/24 passed; initial consumer import preserved host settings, manifest and render-pipeline hashes |
| Optional Input System assembly with package installed | Input System 1.19.0 only, adapter and UI module configured; Play Mode 24/24 passed |
| URP and Domain Reload off | URP 17.6.0 passed; ten enter/exit cycles retained one bootstrap, 22 instances and six audio voices without accumulation |
| Asset Store Publishing Tools validator | 34/34 rules passed with zero validator warnings/failures; Tools 12.0.0 with public-API compatibility changes for Unity 6.6 |
| Unity 6000.0, HDRP, relocation and upgrades | Unverified; no compatibility claim |

## Validation scope and limitations

Asset Store Tools 12.0.0 required three public-API compatibility changes: GUID generation, asset lookup and preview-loading identifiers. Validation rules and thresholds were unchanged. This is not certification by Unity.

Batch runs also emitted Unity Search startup and native JobTempAlloc shutdown diagnostics. Assertions passed; the native diagnostic has not been traced to a package component. Other Unity versions, HDRP, custom pipelines, mobile and gamepad remain unverified. Folder relocation and upgrades preserving custom data are also unverified; keep user assets outside the supplied samples.

The sample scene GUID was replaced after detecting a collision with the official URP template. As of 2026-10-08, sample scene and prefab audio settings are unassigned; sound clips and audio configuration are outside the package in test fixtures. The optional audio API remains available for host-supplied clips. Edit Mode 101/101 and Play Mode 24/24 passed after this change, including silent sample assertions and explicitly configured audio fixture tests.

The Asset Store bundle includes the optional Input System adapter. Repository exports offer separate core and add-on packages through **Tools → Modular Grid Inventory → Export Package**. TMP Essential Resources, EventSystem and its input module belong to the host project.

Submission requirements: https://assetstore.unity.com/publishing/submission-guidelines

Publisher upload workflow: https://docs.unity.com/en-us/asset-store/publishing/asset-packages/upload
