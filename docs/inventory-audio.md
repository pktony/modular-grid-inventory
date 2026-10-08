# 인벤토리 사운드

Project에서 `Assets/Resources/InventoryAudioSettings.asset`을 선택한다. `Master Volume`은 전체 음량, `Muted`는 전체 음소거다. `Defaults`는 동작별 클립과 음량, `Item Profiles`는 분류별 클립과 음량이다. 클립 배열에 여러 음원을 넣으면 순서대로 교대한다. Play 중 음량·음소거·클립 변경도 반영된다.

`InventoryDemo`의 `Audio Settings`에 다른 설정 SO를 연결할 수 있다. 새 설정은 `Create > Inventory > Audio Settings`로 만든다. 참조가 없으면 Resources의 기본 설정을 사용하며, 설정이나 클립이 없으면 해당 효과음만 생략한다.

| 동작 | 재생 시점 | 기본 음원 |
|---|---|---|
| Select / Context | 아이템 좌클릭 / 우클릭 메뉴 / 분할 창 열기 | `click_003` |
| Pickup / Place | 드래그 시작 / 수납 성공 | 유형별 Impact 음원 |
| Merge / Split | 스택 합치기 성공 / 분할 아이템 배치 성공 | `confirmation_001` / `pluck_001` |
| Rotate / Cancel | 드래그 중 회전 / 명시적 취소 | `switch_002` / `back_002` |
| Reject | 드롭·삭제 거절 / 분할 수량 오류 | `error_002` |
| Delete | 삭제 성공 | `drop_001` |
| Open / Close | 새 가방 창 생성 / X 또는 Esc로 닫기 | `open_002` / `close_002` |
| Reset | RESET 버튼 | `click_003` |

| Item Profile 분류 ID | Pickup / Place 음원 |
|---|---|
| `Container/Backpack`, `Container/Rig` | `impactSoft_medium_000` / `_001` |
| `Container/Case` | `impactTin_medium_000` / `_001` |
| `Weapon` | `impactMetal_medium_000` / `_001` |
| `WeaponPart` | `impactMetal_light_000` / `_001` |
| `Ammo` | `impactPlate_light_000` / `_001` |
| `Consumable/Medical` | `impactGeneric_light_000` / `_001` |

분류 선택은 카탈로그의 실제 부모 관계를 따른다. 가장 가까운 분류부터 해당 동작의 유효한 클립을 찾고, 없으면 부모와 Defaults 순서로 찾는다. 예를 들어 `Ammo/9x19`는 `Ammo`의 음원을 사용한다. 분류 ID의 문자열 접두사를 사용하지 않는다.

미리보기와 마우스 이동은 무음이다. 거절음은 실제 확정 시도에 한 번만 발생하며 내부 드래그 정리에 취소음을 섞지 않는다. 같은 가방의 기존 창을 앞으로 가져오는 동작은 열기음을 다시 재생하지 않는다. 초기화로 여러 창이 사라질 때는 RESET 효과음만 한 번 재생한다.

입력 컨트롤러와 창 관리자는 동작 이벤트만 발행한다. `InventorySoundPresenter`는 이벤트 구독과 연결, `InventorySoundResolver`는 분류·클립 선택, `InventoryAudioOutput`은 2D 재생과 음량을 담당한다. 출력은 6개 AudioSource를 재사용하며 장면 종료 시 구독과 출력을 해제한다. Domain과 아이템 정의에는 오디오 의존성을 추가하지 않는다.

음원은 Kenney의 [Impact Sounds](https://kenney.nl/assets/impact-sounds)와 [Interface Sounds](https://kenney.nl/assets/interface-sounds), CC0다. 원본 OGG 21개, 총 162,259 bytes를 `Assets/Audio/Inventory/`에 포함했다. 각 폴더에 원본 `License.txt`를 보관한다. 타르코프 원본 음원은 사용하지 않는다. Unity에서는 짧은 UI 효과음을 Mono·PCM·Decompress On Load·Preload로 가져온다.

녹화 메뉴는 Unity `AudioRenderer`의 메인 믹스를 화면과 함께 캡처한다. Game View를 선택하고 Play에서 수납 시연을 녹화하면 `Recordings/container-showcase/audio.wav`와 `audio.json`을 생성한다. 출력 장치가 없어도 캡처 가능하며 전체 음소거 또는 무음 결과는 완료 검증에서 실패한다. `scripts/edit_container_showcase.py`는 영상과 같은 구간으로 WAV를 잘라 AAC 트랙으로 합친다. 실제 재생된 선택·잡기·수납·거절·창 열기 소리를 사용하고 별도 더빙은 하지 않는다. [재생성 절차](../README.md#검증과-시연-재생성).
