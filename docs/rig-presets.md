# 실재 리그 10종

포켓 가로·세로·개수·상대 행/열은 원본과 일치한다. 셀은 기존 UI의 50px 피치를 사용하며 포켓 표시 좌표는 `column × 1.06`, `row × 1.06`이다. 원본 게임의 화면 해상도별 픽셀 간격이나 텍스처까지 복제한 것은 아니다.

| 리그 | 외부 크기 | 내부 칸 | 독립 포켓 | 에셋 ID |
|---|---|---|---|---|
| Scav Vest | 2×3 | 6 | 4 | `rig-scav` |
| SOE Micro Rig | 2×3 | 8 | 3 | `rig-micro` |
| WARTECH TV-109 + TV-106 | 3×3 | 10 | 5 | `rig-wartech` |
| UMTBS 6Sh112 Scout-Sniper | 3×4 | 12 | 8 | `rig-scout` |
| Haley Strategic D3CRX | 3×3 | 16 | 10 | `rig-d3crx` |
| BlackRock | 3×4 | 20 | 11 | `rig` |
| WARTECH MK3 TV-104 | 3×4 | 20 | 11 | `rig-mk3` |
| ANA Tactical Alpha | 4×4 | 20 | 7 | `rig-alpha` |
| Velocity Systems MPPV | 4×3 | 24 | 14 | `rig-mppv` |
| Poyas-A + Poyas-B | 4×4 | 25 | 19 | `rig-belt` |

## 편집과 재생성

- Play 데이터는 `Assets/Items/Expansion/item-*.asset`, `container-*.asset`, `layout-*.asset`의 ScriptableObject다. 기존 Catalog Table / Inspector에서 수정하고 다음 Play에 반영한다.
- 초기 프리셋은 [RigPresets.json](../Assets/Items/Expansion/RigPresets.json). `width/height`는 외부 점유, `pockets[].width/height`는 내부 구획, `column/row`는 화면상 위치다. 구획마다 실제 점유 격자가 독립적이다.
- JSON 수정 후 `Inventory > Build Expanded Catalog`를 실행한다. 이 메뉴는 프리셋 크기·이름·아이콘·표시 좌표를 SO에 다시 쓰므로 해당 SO의 직접 편집값을 덮어쓴다. JSON은 실행 중에 읽지 않는다.
- 기존 BlackRock의 `rig`, `tall-a`, `small-a` ID와 에셋 GUID는 유지했다. 2×2는 하나이며 아래 오른쪽은 서로 독립적인 1×2 두 포켓이다.
- 데모 시작 보관함에 BlackRock이 있고 아래쪽 20행부터 나머지 9종이 있다. 더블클릭하면 실제 드래그 가능한 컨테이너 창을 연다. 각 인스턴스는 독립 ContainerId를 갖는다.
- 테이블 파싱, 프리셋 에셋 생성, 시연 인스턴스 추가는 각각 `RigPresetLoader`, `RigCatalogBuilder`, `RigDemoSeed`가 담당한다.

## 조사 및 이미지 출처

2026-10-05 확인. 원본 포켓 배치는 아래 두 자료를 대조했다. 외부 점유는 위키 크기 및 base-image의 63px 칸 배수(127×190, 190×253 등)와 대조했다.

- [사용자가 지정한 Chest rigs Wiki](https://escapefromtarkov.fandom.com/wiki/Chest_rigs).
- [tarkov.dev 포켓 데이터, 고정 커밋 ef62766](https://github.com/the-hideout/tarkov-dev/blob/ef62766bbd7ffb294c7184f9b8bfe8ed0f18320e/src/data/item-grids.json)의 게임 Item ID별 `row/col/width/height`.
- [EfT Unofficial Handbook](https://www.mogelpower.de/manuals/Escape_from_Tarkov_EfT_Unofficial_Handbook.pdf), 59–60쪽의 Inside View: 10종의 포켓 배치와 비교.
- [Tarkov 일본 위키 리그 표](https://wikiwiki.jp/eft/タクティカルリグ): 외부 점유, 내부 칸 수, 구조 이미지 교차 확인.
- 게임 이미지 저작권: Battlestate Games. 아이콘은 tarkov.dev의 공개 base-image를 내려받아 PNG로 포맷 변환했다. 게임 원본 아이콘이며 AI 생성 이미지가 아니다. 원본 URL 및 위키 Inside View URL은 프리셋별 JSON에 기록했다.

아이콘 파일은 `Assets/Resources/RigIcons/<에셋 ID>.png`에 있고 모두 RGBA 투명도를 유지한다. 이 리그들에는 공통/포켓 정책의 기본값 AllowAll을 사용하며, 장착·전투·퀵슬롯 제한은 이 인벤토리 데모의 범위 밖이다.
