# Modular Grid Inventory 패키지화 계획

Unity 격자 인벤토리를 다른 프로젝트에서 재사용할 수 있는 소스 패키지로 만든다. 중첩 가방·독립 포켓·스택·커스텀 Inspector를 제품으로 정리하고 Asset Store 배포로 포트폴리오의 설치·확장·검증 과정을 보여준다.

흐름: [패키지 경계·6단계·배포 검증](./docs/asset-store-flow.html)

- 작업 브랜치: `feature/asset-store-package`; 기준 구현: `4e08a36`.
- 이번 산출물: 계획과 흐름도; 구현·게시·계정 설정은 후속 작업.
- 제품명은 `Modular Grid Inventory`를 임시 사용; 최초 배포 형식은 `.unitypackage`.

## 범위

- 포함: 불변 카탈로그, 안전한 상태 변경, 중첩·독립 구획·수납 정책·회전·스택, 다중 창, 효과음, SO 편집·미리보기.
- 통합: 프로젝트별 카탈로그·초기 상태·폰트·테마·입력 주입, Canvas 프리팹, 공개 API와 이벤트 예제.
- 샘플: 자체 이름·중립 아이콘의 가방·케이스·탄약·의료품·부품·무기, 독립 포켓 프리셋 10종.
- 납품: 영문 오프라인 안내·API 설명·라이선스 목록·깨끗한 프로젝트 검증·Validator 결과·스토어 소개 초안.
- 제외: 전투·장착·소모 효과·경제·네트워크·저장/불러오기, UI Toolkit·모바일·콘솔 지원 보장, UPM 스토어 배포.
- 개발 전용: Unity MCP, Windows 빌드·녹화·FFmpeg 도구, 원본 게임 자료, 테스트 실행 도구; 배포물에서 제외.

## 확인한 현재 구조

| 위치 | 패키지화에 필요한 변경 |
|---|---|
| `Assets/Scripts/ExpandedInventory.cs` | 구성 루트가 `ExpandedDemoSeed`·RESET·UI·오디오 조립을 연결; 초기 상태와 시연을 본체에서 분리 |
| `Assets/Scripts/Presentation/InventoryElementFactory.cs` | 고정 Resources 폰트 로드를 명시적 테마 참조로 교체 |
| `Assets/Scripts/Presentation/InventoryInteractionInput.cs` | Legacy Input 직접 조회를 입력 어댑터로 분리 |
| `Assets/Scripts/Presentation/InventoryScreenFactory.cs` | 1280×720·색상·문구·버튼 배치를 설정과 교체 가능한 뷰 구성으로 분리 |
| `Assets/Editor/InventoryCatalogWindow.cs` 등 | `Assets/Items/Expansion` 고정 경로를 선택된 카탈로그와 사용자 저장 경로로 교체 |
| `Assets/Editor/InventorySceneBuilder.cs` | Build Settings·Player Settings 수정과 현재 씬 덮어쓰기를 배포 도구에서 제거 |
| `Assets/Editor/GameViewResolution.cs` | 내부 Editor API 리플렉션을 사용하는 개발용 도구; 배포 제외 |
| `Assets/Resources/RigIcons/`, `docs/rig-presets.md` | 게임 원본 아이콘 10개를 중립 이미지로 교체; 원본 URL·게임 이름이 있는 샘플 데이터 제외 |
| `Packages/manifest.json`, `Assets/TextMesh Pro/` | 개발 패키지·외부 Git 의존성·Unity 기본 리소스 복제를 배포 의존성과 분리 |

## 배포 구조와 책임

모든 배포 파일은 `Assets/ModularGridInventory/` 아래에 둔다. 이동은 `.meta`의 GUID를 유지하고 씬·SO·프리팹 참조를 검사한다.

| 경로 / 구성 | 단일 책임 |
|---|---|
| `Runtime/Catalog` | SO 정의·불변 카탈로그·검증 |
| `Runtime/Domain` | 조회 계약·소유권·규칙·이동/수납/스택·원자적 확정; UI·Editor·샘플 참조 금지 |
| `Runtime/Presentation` | 입력 명령·MVP(Model–View–Presenter)·화면·창·사운드 연결; 데모 초기화 금지 |
| `Runtime/Integration` | 구성 루트의 의존성 조립·해제, 기본 uGUI 연결 |
| `Runtime/Input` | 입력 원천 계약과 Legacy 어댑터; Input System 어댑터는 선택 어셈블리 |
| `Editor` | 카탈로그 선택·SO 편집·프리팹 설정 검증; Runtime에서 참조 금지 |
| `Samples` | 초기 상태·중립 아이콘·포켓 10종·데모 장면·효과음 |
| `Documentation`, `ThirdParty` | 영문 설치·확장 안내, 변경 기록·출처·라이선스 |
| 저장소의 개발 도구·테스트 | 검증·녹화·배포 파일 생성; 고객 패키지에 자동 포함 금지 |

- 네임스페이스·어셈블리는 `Pktony.GridInventory` 접두사로 통일; 이름 변경 시 직렬화 식별자·테스트 참조를 검증한다.
- `InventoryBootstrapper`는 조립·수명만, 초기 상태 빌더는 배치만, UI 설치기는 화면 생성만 담당한다.
- 아이템·컨테이너 정의는 불변 복사로 공유; 인스턴스·배치·창 상태의 소유권과 종료 책임은 각각 유지한다.
- 조회·이동·스택·수납·편집 계약을 유지하고 샘플은 공개 API만 사용; 내부 모델 setter·전역 저장소를 공개하지 않는다.
- UI/오디오 설정 누락은 설치 검증에서 안내; 고정 Resources 키로 다른 프로젝트의 에셋을 찾지 않는다.
- 기본 UI를 바로 설치할 수 있는 프리팹을 제공하되, 모델만 사용하는 예제로 UI 독립성을 검증한다.

## 단계와 독립 PR

| # | 단계 | PR 분리 단위 | 산출물 | 완료 기준 |
|---|---|---|---|---|
| 1 | 배포 경계 확정 | 폴더·GUID 이동 / 의존성·내보내기 | 단일 루트·명시적 포함 목록·의존성 표 | 참조 누락 0, MCP·레거시·녹화·게임 원본 파일 미포함, 경로 150자 미만 |
| 2 | 본체 통합 계약 | 구성·초기 상태 / 입력 어댑터 | 공개 API·Bootstrapper·입력 연결 | 샘플 없이 시작, 사용자 카탈로그·초기 상태 주입, Legacy/Both/Input System 환경에서 설정 강제 변경 없이 작동 |
| 3 | 교체 가능한 UI·편집기 | 테마·프리팹 / 범용 Inspector | 폰트·색·격자 설정, 카탈로그 선택·생성 UI | 두 인벤토리 동시 독립 실행, 임의 경로 데이터 편집·Undo/Redo, 1280/1920 화면과 스케일 대응 |
| 4 | 배포용 샘플·출처 | 중립 샘플 / 음원·폰트·고지 | 독립 포켓 10종·기본/모델 전용 예제·Third-Party Notices | 게임 이름·원본 이미지 참조 0, 중첩·스택·수납 설정 유지, 포함 파일별 출처와 배포 조건 확인 |
| 5 | 소비자 환경 검증·문서 | 깨끗한 프로젝트 검증 / 영문 문서 | 호환성 표·회귀 결과·Quick Start·API/확장 설명 | 아래 검증 행렬 통과, 필수 설치만으로 샘플 실행, 패키지 기인 오류·경고 0, GUID·호스트 설정 보존 |
| 6 | 제출물 준비 | 릴리스 생성 / 소개·심사 자료 | 버전별 unitypackage·검사 결과·스토어 초안·영상 | Validator 지적 해결·재검사, 소개와 실제 지원 범위 일치, 게시 선택 사항 확정 후 사용자 검토 가능한 제출물 준비 |

각 PR은 한 책임과 완료 증거를 포함하고 바로 앞 PR을 기준으로 만든다. 검증 가능한 단위마다 커밋하며 최종 전체 패키지를 한 PR로 묶지 않는다.

## SOLID와 패턴

- SRP(단일 책임 원칙)는 예외 없이 적용: 생성·검증·상태 확정·입력·표시·오디오·샘플·배포를 분리한다.
- 기존 Factory·MVP·Composition Root·Observer를 유지; 입력 원천 교체에 Adapter를 적용한다.
- 실제 교체 지점에만 인터페이스를 둔다; 유형별 상속·전역 이벤트 버스·Service Locator·거대 Installer·SO/JSON 이중 원본은 도입하지 않는다.
- 사용자 조작은 포인터를 즉시 따른다; 영상용 이징·클릭 강조·자동 시나리오는 개발 도구가 소유한다.
- 임포트 시 씬 전환·설정 덮어쓰기·의존 패키지 자동 설치를 하지 않는다; 데이터 복원은 대상과 덮어쓰기 범위를 확인하는 명시적 명령이다.

## 검증 행렬

- 버전 후보: Unity `6000.0` LTS와 현재 `6000.6.4f1`; 지금 설치된 버전은 후자뿐이며 전자는 설치 후 검증 전까지 지원으로 표기하지 않는다.
- 렌더링: 두 버전의 깨끗한 URP(Universal Render Pipeline) 프로젝트를 필수 검증; Built-in은 별도 통과 후에만 지원 표기.
- 입력: Legacy·Both·Input System only; 선택 어댑터가 없거나 있거나 본체 컴파일이 성공해야 한다.
- 수명: Domain Reload 켬/끔, Play 반복 10회·씬 이동·컴포넌트 제거, 리스너·창·AudioSource 누적 0.
- 설치: 최초 임포트·경로 이동·같은 버전 재임포트·다음 버전 업그레이드, Missing Script/Sprite/Font 0·수정한 사용자 SO 보존.
- 상태: 중첩·순환·독립 구획·수량 보존·실패 불변·재진입 격리의 기존 회귀 테스트와 공개 API 예제 검증.
- 화면: 다중 창·전면 순서·두 수납 경로·Undo/Redo·회전·스택·사운드, 기존 EventSystem/Canvas/AudioListener와 충돌 없음.
- 호스트: ProjectSettings·Build Settings·기존 씬·다른 패키지 Resources가 변경되지 않는지 임포트 전후 대조.
- 기준 증거: 현재 Edit Mode 91/91·Play Mode 24/24·Windows 시연 27/27·녹화 49/49; 패키지 이동 이후 별도로 재검증한다.

## Asset Store 제출 기준 적용

2026-10-07 [제출 가이드](https://assetstore.unity.com/publishing/submission-guidelines), [업로드·검증 절차](https://docs.unity.com/en-us/asset-store/publishing/asset-packages/upload), [배포 형식](https://docs.unity.com/en-us/asset-store/publishing)을 확인했다.

- `.unitypackage` 먼저 제출; Unity Package Manager(UPM) 스토어 배포는 별도 신청 경로이므로 이번 범위에서 제외.
- Unity 6.5 이상 제출 기준의 URP/HDRP 지원과 6.6의 Domain Reload 비활성 대응을 필수 검증에 반영.
- 내부 Editor API 리플렉션·불필요한 파일·실행 파일·MP4는 배포 제외; 영상은 제품 소개의 외부 링크로 제공.
- 메뉴는 `Tools/Modular Grid Inventory`·`Window/Modular Grid Inventory`; 설정·패키지 자동 변경 금지.
- 폰트·Kenney CC0 음원 등은 포함 항목별 라이선스와 Third-Party Notices를 제공; 호환 불명확한 파일은 교체하거나 제외.
- AI 보조 코드·생성 이미지의 도구·적용 범위·수정 내용을 AI description에 사실대로 정리; 제품 설명은 실제 기능과 대조해 검토.
- Asset Store Publishing Tools로 명시적 루트만 검증·내보내기; 오류·지적을 해결하고 동일 릴리스로 깨끗한 프로젝트 재검사.
- 준비 완료는 제출 가능 상태이며 심사 승인·판매·게시 완료와 구분한다.

## 위험과 완화

| 위험 | 심각도 | 완화 |
|---|---|---|
| 게임 원본 이미지·브랜드가 배포물에 남음 | 높음 | 중립 샘플 교체, 파일·참조·소개 문구 포함 목록 검사 |
| 이동·어셈블리 변경으로 GUID/직렬화 참조 손실 | 높음 | GUID 유지, Missing Script/참조와 업그레이드 검증 |
| 기본 폰트·샘플 ID·입력·프로젝트 설정 의존 | 높음 | 명시적 주입, 샘플 없는 실행과 새 호스트 프로젝트 검사 |
| Domain Reload 비활성에서 구독·정적 상태 누적 | 높음 | 인스턴스 소유·Dispose·필요한 초기화, 반복 Play 검증 |
| 최소 버전·렌더링 지원을 검증 없이 광고 | 높음 | 버전별 빌드·동작 결과 확보 후 지원 표기 |
| 기존 코드에 배포·시연·초기화 책임 재집중 | 중간 | 책임별 어셈블리·클래스·PR, 샘플→공개 API 단방향 의존 |

## 결정

- 배포 소스는 단일 Assets 루트 — 일반 Asset Store 업로드와 명시적 내보내기를 우선. 버린 안: UPM/Assets 두 벌 유지(원본·GUID 불일치).
- 타르코프 참고 데모는 저장소 기록으로 보존하고 제품 샘플은 중립화 — 재사용성과 출처 검토를 분리. 버린 안: 게임 원본 아이콘 재배포(배포 권한 미확인).
- 첫 버전은 uGUI와 기존 기능에 집중 — 설치·확장 경로를 먼저 완성. 버린 안: UI Toolkit·저장·전투까지 확장(검증 범위 증가).

## 열린 질문

- [ ] 최종 제품명·Publisher 표시명·지원 연락처.
- [ ] 무료 공개 또는 유료 가격·지원 범위; 결정 전 유료 판매 문구를 작성하지 않는다.
- [ ] Unity `6000.0` 지원 가능 여부와 최종 버전·렌더링 행렬은 실제 검사 결과로 확정.
- [ ] 소스 저장소 공개 정책과 Asset Store 배포 조건의 연결.
- [ ] 최종 제출·공개 시점과 Publisher 계정 설정; 이번 계획 단계에서는 계정·스토어를 변경하지 않는다.
