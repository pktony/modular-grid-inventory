# 아이템 아이콘 생성 기록

방식: 내장 `image_gen`, 새 이미지 생성 10회 및 배경 제거 편집 2회. CLI/API 키는 사용하지 않았다. 최종 PNG는 `Assets/Resources/ExpansionIcons/`에 저장하며 원본 알파를 유지했다.

공통 프롬프트 사양: `product-mockup`, Unity tactical inventory icon, single whole centered object, realistic game-item render, muted materials, soft studio lighting, small padding, genuine transparent background. No letters, logos, watermark, person, UI, ground or shadow outside the object. 아래 대상 설명을 각 생성 요청에 결합했다.

| 최종 파일 | 대상 프롬프트 |
|---|---|
| `mbss.png` | Olive drab small tactical nylon MBSS-inspired backpack, rear view with slight depth. |
| `berkut.png` | Olive green medium Berkut-style tactical backpack, whole object. |
| `ammo-case.png` | Closed rugged olive ammunition case, metal latches. |
| `medicine-case.png` | Closed light gray medical hard case, subtle red cross, no lettering. |
| `rig.png` | Olive BlackRock-style tactical chest rig, many pouches, front view, no human. |
| `pst.png` | Brass 9x19 cartridges with copper round-nose bullets. |
| `ps.png` | Brass 5.45x39 cartridges with pointed copper bullets. |
| `ai2.png` | Small orange rectangular medical kit box, hinge and latch. |
| `aks74u.png` | Compact short-barrel rifle, wooden handguard, dark curved magazine, horizontal side view, muzzle right. |
| `rk0.png` | Black short angled vertical foregrip with rail clamp, three-quarter view. |

`ammo-case.png`와 `ps.png`에는 다음 배경 제거 편집 프롬프트를 적용했다.

> Remove ONLY the soft halo, vignette and colored background around these objects. Preserve every object, shape, material, lighting, composition and detail. Make the complete area outside the hard object silhouettes fully transparent with no fog, glow, shadow, ground or residual backdrop. True alpha cutout for a small game inventory icon.

10개 이미지를 직접 확인하고 PNG의 RGBA 알파와 바깥 모서리의 투명도를 확인했다. 이 렌더는 포트폴리오용 참고 이미지이며 실물이나 게임 원본의 정확한 복제는 아니다. 기존 저장소 `Sprites` 이미지는 현재 데모에서 사용하지 않는다.

리그 10종 추가 이후에는 위 생성 아이콘 중 `rig.png` 대신 `Assets/Resources/RigIcons/`의 실재 리그 아이콘 10개를 사용한다. 나머지 생성 아이콘 9개는 유지한다. 원본 이미지 URL·포켓 배치 및 출처는 [리그 프리셋](rig-presets.md)에 기록했다.
