# 패키지 후보 검증 기록

2026-10-07 · `0.1.0-candidate` · Unity 실행 검증 대기.

## 확인한 결과

| 검사 | 결과 | 한계 |
|---|---|---|
| C# 오프라인 컴파일 | 어셈블리 11개 성공, 배포 어셈블리 7개 경고 0 | 설치된 Unity 6000.6.4f1 DLL과 캐시 uGUI 사용; Unity 임포트·실행 결과가 아님 |
| 개발 도구·테스트 경고 | 기존 미사용 필드 2개, 기존 API 사용 중단 경고 2개 | 고객 패키지에 포함되지 않음 |
| 정적 에셋 검사 | 파일 267개, 아이콘 19개, 정의 19개, 초기 인스턴스 22개; 오류 0 | 참조·GUID·배치·수량·순환·이미지 해시·알파·배포 경계 검사 |
| 호스트 GUID 충돌 | 자체 패키지와 저장소의 다른 Assets 간 충돌 0 | 다른 고객 프로젝트의 임포트 검사는 별도 필요 |
| 이미지 갤러리 | 1280/390 폭에서 19개 로딩, 가로 넘침 없음 | 게임 화면의 실제 표시 검사가 아님 |
| 후보 아카이브 | Core와 Input System 어댑터를 별도로 생성, 포함 경로·개수·해시 기록 | Unity 자체 내보내기 및 새 프로젝트 임포트 미검증 |

아이템·컨테이너·씬의 자체 GUID는 유지했다. 공통 폰트 복사본은 새 GUID를 사용해 호스트의 TMP 기본 폰트를 덮어쓰지 않도록 했다. 리그 JSON 생성 원본은 `Assets/Development/Data`로 분리하고 배포물의 SO를 편집 원본으로 둔다.

추가한 통합 테스트 10개는 초기 상태의 순서·잘못된 부모·순환·중복 키·실패 불변·독립 인스턴스·불변 테마를 다룬다. 기존 테스트와 함께 소스 컴파일만 확인했으며 실행 통과 수를 새로 주장하지 않는다. 선택 Input System 어셈블리는 해당 패키지를 설치한 환경에서 별도로 컴파일·실행해야 한다.

## 재현

```powershell
uv run --with pyyaml --with pillow python tools/validate_package.py
uv run --with pyyaml --with pillow python tools/build_candidate.py
```

- 정적 결과: `Builds/Package/validation.json`.
- 후보 및 SHA-256: `Builds/Package/candidate.json`.
- Core: `Builds/Package/ModularGridInventory-0.1.0-candidate.unitypackage`.
- 선택 입력 어댑터: `Builds/Package/ModularGridInventory-InputSystem-0.1.0-candidate.unitypackage`.
- Unity 공식 API 내보내기: **Tools → Modular Grid Inventory → Export Package**.

빌드 결과와 로컬 컴파일 도구는 Git 및 고객 패키지에 포함하지 않는다. 배포에 포함되는 검증 도구는 Editor 메뉴의 명시적 검사·내보내기 기능이다.

## 실행 대기

Unity와 MCP 서버의 백그라운드 시작 명령을 자동 실행 검토가 거절했다. 응답에는 구체적인 사유가 없었으며, 다른 경로로 실행을 우회하지 않았다. 현재 Unity 임포트·셰이더 렌더링·Edit/Play 테스트·새 리소스 영상·깨끗한 소비자 프로젝트·Publisher Validator 결과는 없다.

실행 가능한 환경에서 [ReleaseChecklist](../Assets/ModularGridInventory/Documentation/ReleaseChecklist.md)의 검증 행렬을 완료해야 한다. 이전 패키지 이동 전 테스트·시연 기록은 회귀 기준으로만 사용한다. 이 후보를 Asset Store 제출 완료 또는 호환성 인증 결과로 취급하지 않는다.

## 순차 검토 단위

계획 → 패키지 본체·통합 경계 → 중립 샘플 → 편집기 설치 검사 → 모델 예제·통합 테스트 → 검증·내보내기 → 문서·제출 초안 순서로 PR을 연결한다. 각 PR의 base는 바로 앞 PR의 브랜치이며, 앞 PR부터 merge commit 방식으로 합친다.
