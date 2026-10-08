# Modular Grid Inventory — 제출 문구 초안

상태: 0.1.0 후보. 개발 프로젝트 실행·회귀는 통과했으며 깨끗한 소비자 설치·호환성 행렬·Publisher Validator 검증 전까지 게시하지 않는다.

## Description

Build a configurable desktop grid inventory with nested containers, separated pockets, category-based acceptance policies and stack limits. Author items and layouts in ScriptableObjects, preview pocket arrangements in the Unity Inspector, and use the public mutation services to build your own view.

The default uGUI view provides movable container windows, rotation, drag/drop previews, stack merging/splitting and optional category-based sounds. Samples include 19 fictional item definitions and 10 carrier layouts with newly generated icons.

## Technical details

- Source code; uGUI/TextMeshPro dependency; separate optional Input System adapter.
- Overlay Canvas reference layout: 1280 × 720, CanvasScaler for display scaling.
- Atomic state changes and immutable snapshots; no inventory singleton.
- English offline setup, API and resource notices.
- No save/load, networking, equipment/combat logic, UI Toolkit or mobile/gamepad support claim.
- Unity version and render-pipeline support fields must follow executed compatibility results.

## AI description

OpenAI Codex assisted with C# implementation, package organization, tests and documentation. OpenAI imagegen generated 19 fictional sample equipment icons from original text prompts. Some generated images received transparency cleanup requests. Artwork selection, import settings and SHA-256 provenance are included in the package. Existing game icons are excluded.

## 게시 전 사용자 결정

- 제품명·Publisher 표시명·지원 연락처.
- 무료/유료·가격과 지원 범위.
- 원본 소스의 공개·배포 정책.
- 실제 검증 결과와 동일한 소개 이미지·영상, 최종 제출 시점.
