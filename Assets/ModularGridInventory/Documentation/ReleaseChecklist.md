# 0.1.0 Candidate Validation

The source package is under development. Compilation is not equivalent to a successful Unity import, Play Mode test or Asset Store validator pass.

| Check | Current evidence |
|---|---|
| Package/development separation | Explicit asset root; capture/build tools outside it |
| C# compilation | Offline compiler against installed Unity 6000.6.4f1 and cached uGUI assemblies |
| Existing tests | Source compilation checked; post-migration execution pending |
| New integration tests | Seed ordering, missing parents, cycles, noncontainer parents, atomic failure, independent IDs and frozen themes; execution pending |
| Icons | 19 generated RGBA PNGs, preserved asset GUIDs, recorded hashes |
| Unity asset import / shader render | Pending |
| New consumer project / settings preservation | Pending |
| Optional Input System assembly with package installed | Pending |
| Unity 6000.0, URP/HDRP, Domain Reload off | Pending |
| Asset Store Publishing Tools validator | Pending |

## Required execution before submission

1. Run Edit and Play Mode suites after import. Check the Console for package errors/warnings and the supplied font shader for rendering/clipping errors.
2. Import the core package into a new Unity 6000.6.4f1 URP project with uGUI, then repeat in HDRP. Confirm icons, fonts, audio, container windows and rejection reasons.
3. Test Legacy, Both and Input System only with the separate adapter. Confirm the core compiles without the optional package.
4. Repeat Play ten times with Domain Reload enabled and disabled; inspect subscriptions, windows and AudioSources for accumulation.
5. Test two independent inventories, scene changes and component destruction. Compare host ProjectSettings, build settings, scenes and package manifest before/after import.
6. Move the package folder, reimport, then test an upgrade without replacing user-created data.
7. Run Unity Asset Store Publishing Tools validation. Resolve findings, re-export through the package export window and repeat the same clean-consumer checks.

Use **Tools → Modular Grid Inventory → Export Package** for explicit Unity export. Core and Input System add-on are separate packages. Repository-side candidate archives are preliminary and must be import-tested before release.

Submission requirements: https://assetstore.unity.com/publishing/submission-guidelines

Publisher upload workflow: https://docs.unity.com/en-us/asset-store/publishing/asset-packages/upload
