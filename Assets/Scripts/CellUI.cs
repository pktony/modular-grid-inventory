using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class CellUI : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {

        private TextMeshProUGUI test;

        private ItemData itemData;

        [SerializeField] private Image itemImage;
        [SerializeField] private Button cellButton;
        [SerializeField] private RectTransform itemImageRect;

        /// <summary>
        /// 초기에 비어있는 cell을 만드는 함수
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void InitializeCells(int x, int y)
        {
            // itemImage = transform.Find("Image").GetComponent<Image>();
            test = transform.Find("Text").GetComponent<TextMeshProUGUI>();

            // test.text = $"{x}, {y}";
        }

        public void SetListeners(Action<ItemData> onClickedCell)
        {
            cellButton.onClick.AddListener(() => onClickedCell?.Invoke(itemData));
        }

        public void AssignItem(ItemData itemData, Action<CellUI, string> callback)
        {
            this.itemData = itemData;
            Test(itemData.itemId);

            ApplySize();

            callback?.Invoke(this, itemData.itemId);
        }

        public void SetItemImage(Sprite sprite)
        {
            itemImage.sprite = sprite;
        }

        public void Test(string itemId)
        {
            test.text = itemId;
        }

        protected virtual void ApplySize()
        {
            if (itemData == null)
            {
                itemImage.gameObject.SetActive(false);
                return;
            }

            itemImageRect.gameObject.SetActive(true);
            itemImageRect.sizeDelta = GetRotateCellSize();
            // target.sizeDelta = GetCellSize();
            // target.localEulerAngles = Vector3.forward * (CellData?.IsRotate ?? false ? 90 : 0);
        }

        public Vector2 GetRotateCellSize()
        {
            var isRotate = itemData.itemDirection == ItemDirection.Vertical;
            var cellSize = GetCellSize();
            if (isRotate)
            {
                var tmp = cellSize.x;
                cellSize.x = cellSize.y;
                cellSize.y = tmp;
            }

            return cellSize;
        }

        private Vector2 GetCellSize()
        {
            var width = (itemData?.width ?? 1) * 66f;
            var height = (itemData?.height ?? 1) * 66f;
            return new Vector2(width, height);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // 아이템 정보 표기
            // 드래그 중에는 아이템을 놓을 수 있는지 판별
            //  - 놓을 수 없으면 빨간색
            //  - 놓을 수 있으면 초록색

            itemImage.color = Color.white;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 아이템 정보 표기 해제
            // 아이템 놓을 수 있는지 판별한 색 원상복구

            itemImage.color = Color.gray;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // 아이템 놓을 수 있는지 판별
            //  - 놓을 수 있으면 놓기
        }
    }
}