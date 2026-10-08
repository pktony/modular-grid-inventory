using System;
using System.Linq;
using Pktony.GridInventory.Domain;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public sealed class ContainerShowcaseScenario : IDisposable
    {
        public const int FramesPerStage = 36;
        public const int StageCount = 49;
        public const int FrameCount = StageCount * FramesPerStage;
        private readonly InventoryBootstrapper inventory;
        private readonly ShowcasePointerDriver pointer;
        private int stage = -1;
        private ItemInstanceId firstBag, secondBag, nested, rig;
        private InventorySnapshot State => inventory.ReadModel.Snapshot;
        private ContainerId Root => State.RootContainerId;
        public ContainerShowcaseScenario(InventoryBootstrapper inventory)
        { this.inventory = inventory; pointer = new ShowcasePointerDriver(inventory); }
        private ItemInstanceId Find(string definition, ContainerId owner = default, int quantity = 0) => State.Items.Values.First(i =>
            i.Definition.Identifier == definition && (owner.IsEmpty || State.Containers[owner].Entries.ContainsKey(i.Id)) && (quantity == 0 || i.Quantity == quantity)).Id;
        private ContainerId Child(ItemInstanceId id) => State.Items[id].ChildContainerId;
        private void Scroll(float value) { inventory.Screen.Stash.Scroll.verticalNormalizedPosition = value; Canvas.ForceUpdateCanvases(); }
        private void Close(ItemInstanceId id) => inventory.Windows.Find(Child(id)).Close.onClick.Invoke();
        private void CloseAll() { foreach (var window in inventory.Windows.Windows.Values.ToArray()) window.Close.onClick.Invoke(); }
        public void Tick(float time)
        {
            int next = Mathf.Min(StageCount - 1, Mathf.FloorToInt(time * 30 / FramesPerStage + 0.001f));
            if (next != stage) { stage = next; Apply(stage, time); }
            pointer.Tick(time);
        }
        private void Apply(int value, float time)
        {
            switch (value)
            {
                case 0:
                    inventory.ResetDemo(); Scroll(0.86f);
                    var bags = State.Items.Values.Where(i => i.Definition.Identifier == "pack-large").ToArray();
                    firstBag = bags[0].Id; secondBag = bags[1].Id; nested = Find("pack-small"); rig = Find("rig", Root);
                    pointer.Open(rig, time); break;
                case 1: pointer.MoveWindow(rig, new Vector2(32, -100), time); break;
                case 2: pointer.End(); pointer.Begin(Find("ammo-light", Root, 40), pointer.Cell(Child(rig), "small-a"), time); break;
                case 3: pointer.End(); break;
                case 4: pointer.Begin(Find("medical-kit", Root), pointer.Cell(Child(rig), "small-b"), time); break;
                case 5: pointer.End(); break;
                case 6: pointer.Begin(Find("grip", Root), pointer.Cell(Child(rig), "small-c"), time); break;
                case 7: pointer.End(); break;
                case 8: pointer.Begin(Find("ammo-heavy", Root), pointer.Cell(Child(rig), "small-a"), time); break;
                case 9: pointer.End(); break;
                case 10: pointer.Begin(Find("carbine", Root), pointer.Cell(Child(rig), "tall-a"), time); break;
                case 11: pointer.End(); break;
                case 12: pointer.Begin(Find("carbine", Root), pointer.ItemPoint(rig), time); break;
                case 13: pointer.End(); Scroll(1); pointer.Open(secondBag, time); break;
                case 14: pointer.MoveWindow(secondBag, new Vector2(340, -100), time); break;
                case 15: pointer.End(); break;
                case 16: Scroll(0.86f); pointer.Begin(rig, pointer.Cell(Child(secondBag), "main"), time); break;
                case 17: pointer.End(); break;
                case 18: Scroll(1); Close(rig); pointer.Open(firstBag, time); break;
                case 19: pointer.MoveWindow(firstBag, new Vector2(32, -100), time); break;
                case 20: pointer.End(); pointer.Open(nested, time); break;
                case 21: pointer.MoveWindow(nested, new Vector2(580, -240), time); break;
                case 22: pointer.End(); pointer.Begin(nested, pointer.ItemPoint(secondBag), time); break;
                case 23: pointer.End(); Close(nested); break;
                case 24: Scroll(0.7f); pointer.Begin(nested, pointer.Cell(Root, "main", 0, 16), time); break;
                case 25: pointer.End(); break;
                case 26: Scroll(1); pointer.Begin(secondBag, pointer.Cell(Child(firstBag), "main"), time); break;
                case 27: pointer.End(); break;
                case 28: pointer.Open(rig, time); pointer.MoveWindow(rig, new Vector2(580, -120), time); break;
                case 29: pointer.End(); pointer.Open(secondBag, time); break;
                case 30: pointer.Begin(firstBag, pointer.ItemPoint(secondBag), time); break;
                case 31: pointer.End(); break;
                case 32: CloseAll(); inventory.ResetDemo(); Scroll(0.86f); pointer.Open(Find("case-medical", Root), time); break;
                case 33: pointer.MoveWindow(Find("case-medical", Root), new Vector2(32, -100), time); break;
                case 34: pointer.End(); pointer.Begin(Find("medical-kit", Root), pointer.Cell(Child(Find("case-medical", Root)), "main"), time); break;
                case 35: pointer.End(); break;
                case 36: pointer.Begin(Find("ammo-heavy", Root), pointer.ItemPoint(Find("case-medical", Root)), time); break;
                case 37: pointer.End(); break;
                case 38: CloseAll(); pointer.Open(Find("case-ammo", Root), time); break;
                case 39: pointer.MoveWindow(Find("case-ammo", Root), new Vector2(32, -100), time); break;
                case 40: pointer.End(); pointer.Begin(Find("ammo-light", Root, 20), pointer.Cell(Child(Find("case-ammo", Root)), "main"), time); break;
                case 41: pointer.End(); break;
                case 42: pointer.Begin(Find("carbine", Root), pointer.Cell(Child(Find("case-ammo", Root)), "main", 2, 1), time); break;
                case 43: pointer.End(); break;
                case 44: pointer.Open(Find("case-medical", Root), time); pointer.MoveWindow(Find("case-medical", Root), new Vector2(440, -100), time); break;
                case 45: pointer.End(); pointer.Begin(Find("medical-kit", Child(Find("case-medical", Root))), pointer.Cell(Child(Find("case-ammo", Root)), "main", 2, 2), time); break;
                case 46: pointer.End(); break;
                case 47: Close(Find("case-medical", Root)); break;
                case 48: CloseAll(); inventory.ResetDemo(); Scroll(1); break;
            }
        }
        public void Dispose() => pointer.Dispose();
        public static readonly string[] Captions = {
            "모듈형 리그 · 서로 떨어진 11개 포켓", "빠르게 리그 창 배치",
            "탄약 40발 → 리그의 1칸 포켓", "탄약 수납 완료",
            "응급 키트 의료품 → 다른 포켓", "의료품 수납 완료",
            "RK-0 무기 부품 → 다른 포켓", "탄약 · 의료품 · 부품을 함께 수납",
            "이미 다른 탄약이 있는 칸", "겹친 칸에 수납 거절 · 원본 유지",
            "4×2 무기 → 1×2 포켓", "포켓보다 큰 아이템 · 수납 거절",
            "리그 아이콘 위에 무기 드롭", "빈 칸이 많아도 연속 공간이 없으면 거절",
            "가방 창도 자유롭게 이동", "열린 리그와 가방 창",
            "내용물이 든 리그 → 가방 모달", "리그와 내용물 · 열린 창 유지",
            "다른 가방에는 소형 가방가 들어 있음", "가방 창을 나란히 배치",
            "가방 안의 가방 열기", "소형 가방 안에는 응급 키트 의료품",
            "리그가 든 가방 아이콘 → 소형 가방 드롭", "남은 공간 부족 · 가방 수납 거절",
            "소형 가방를 보관함으로 꺼내기", "안의 의료품도 함께 이동",
            "리그가 든 가방 → 빈 가방 모달", "가방 → 가방 → 리그 → 아이템",
            "중첩된 리그를 다시 열기", "같은 가방은 기존 창 재사용",
            "부모 가방을 자손 가방에 넣기", "순환 중첩 거절 · 내용물 유지",
            "의료품 케이스 · 유형별 수납 규칙", "의료품 케이스 창 이동",
            "응급 키트 의료품 → 의료품 케이스", "허용된 의료품 수납 완료",
            "총알 → 의료품 케이스 아이콘", "허용되지 않은 유형 · 수납 거절",
            "탄약 케이스도 별도 인벤토리", "탄약 케이스 창 이동",
            "탄약 20발 → 탄약 케이스 모달", "허용된 탄약 수납 완료",
            "총 → 탄약 케이스의 빈 칸", "공간이 있어도 무기 유형은 거절",
            "의료품 케이스를 다시 열기", "의료품 → 탄약 케이스 모달",
            "의료품도 거절 · 원래 위치 유지", "각 케이스의 내용물은 독립적으로 유지",
            "초기화 · 모든 성공과 거절은 실제 게임 동작"
        };
    }
}
