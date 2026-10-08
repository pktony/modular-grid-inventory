# Modular Grid Inventory

중첩 가방, 독립 포켓, 유형별 수납 정책과 스택을 지원하는 Unity 인벤토리 소스 패키지입니다. 포트폴리오 데모를 다른 프로젝트에 설치할 수 있는 형태로 구성했습니다.

저장소: [pktony/modular-grid-inventory](https://github.com/pktony/modular-grid-inventory)

```bash
git clone https://github.com/pktony/modular-grid-inventory.git
```

현재는 **0.1.0 후보**입니다. Unity 6000.6.4f1에서 샘플 실행과 Edit 101/101·Play 24/24를 확인했습니다. 깨끗한 소비자 프로젝트 임포트·호환성 행렬·Asset Store Validator 검증이 남아 있습니다.

## 실행과 설정

- 개발 버전: Unity `6000.6.4f1`, uGUI `2.6.0` 및 TMP Essential Resources.
- 샘플 씬: `Assets/ModularGridInventory/Samples/Scenes/Inventory.unity`.
- 기본 프리팹: `Assets/ModularGridInventory/Samples/Prefabs/ModularInventory.prefab`.
- 아이템 크기·스택·유형: `Samples/Catalog/item-*.asset`.
- 가방·리그 포켓과 수납 정책: `Samples/Catalog/container-*.asset`; Inspector에서 편집·미리보기·Undo.
- 초기 배치·중첩 부모: `Samples/Settings/InitialState.asset`.
- 폰트·색·칸 크기·클릭 표시: `Samples/Settings/InventoryTheme.asset`.
- 예제는 무음이며 효과음 파일을 포함하지 않습니다. 소리를 추가하려면 Audio Settings를 생성해 자신의 클립을 연결합니다.
- 메뉴: `Tools → Modular Grid Inventory → Catalog Table / Export Package`.

아이템 정의 19종, 초기 인스턴스 22개, 리그 포켓 10종을 제공합니다. 아이콘 19종은 새 중립 디자인으로 생성했습니다.

## 안내

- [영문 설치 안내](Assets/ModularGridInventory/Documentation/QuickStart.md)
- [공개 API·소유권·확장](Assets/ModularGridInventory/Documentation/API.md)
- [새 아이콘 미리보기](Assets/ModularGridInventory/Documentation/Icons.html)
- [리소스 출처와 라이선스](Assets/ModularGridInventory/Documentation/Third-Party_Notices.txt)
- [현재 검증 결과](docs/package-validation.md)
- [계획](PLAN.md) · [전체 흐름](docs/asset-store-flow.html)

## 조작

| 입력 | 동작 |
|---|---|
| 좌클릭 / 드래그 | 선택 / 이동 |
| R / Esc | 회전 / 취소·앞 창 닫기 |
| 더블클릭 / OPEN | 컨테이너 창 열기; 같은 인스턴스는 기존 창 재사용 |
| 제목 드래그 / X | 창 이동·맨 앞으로 표시 / 닫기 |
| 격자 / 가방 아이콘에 드롭 | 지정 칸 배치 / 내부 빈 칸 자동 수납 |
| 우클릭 / SPLIT | 메뉴 / 수량 분할 |
| 같은 탄약에 드롭 | 최대 스택까지 합치기 |
| Delete / RESET | 삭제 / 초기 상태 재생성 |

회전·수납 거절 사유를 표시하고, 실패 시 상태와 수량을 유지합니다. 자기 자신이나 자손 가방에 넣는 순환 중첩은 거절됩니다. UI 창 상태는 모델의 아이템 배치와 분리됩니다.

## 배포와 개발 도구

배포 루트는 `Assets/ModularGridInventory`입니다. Core와 선택 Input System 어댑터를 별도 `.unitypackage`로 내보냅니다. 호스트의 EventSystem·AudioListener를 사용하며 프로젝트 설정·패키지를 자동 변경하지 않습니다.

`Assets/Development`, Unity MCP, 녹화·빌드·FFmpeg 도구, 기존 게임 참고 이미지·영상은 배포에서 제외됩니다. [영어 무음 데모 영상](docs/modular-grid-inventory-demo-en.mp4)은 현재의 새 리소스로 촬영한 36.5초 시연입니다. [영상 구성과 재촬영 방법](docs/silent-demo.md)을 참고하세요. 기존 시연 영상은 아이콘 교체 전 버전의 참고 자료입니다.

개발용 정적 검사와 후보 아카이브 생성:

```powershell
uv run --with pyyaml --with pillow python tools/validate_package.py
uv run --with pyyaml --with pillow python tools/build_candidate.py
```

생성 위치는 Git에서 제외된 `Builds/Package`입니다. 후보 아카이브는 실제 Unity 임포트를 검증한 릴리스가 아닙니다. 지원 버전·렌더링 환경과 게시 조건은 검사 결과를 확보한 후 확정합니다.
