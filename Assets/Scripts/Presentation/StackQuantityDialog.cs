using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem.Presentation
{
    public sealed class StackQuantityDialog
    {
        private readonly RectTransform root;
        private readonly TMP_InputField input;
        private readonly TextMeshProUGUI prompt;
        private Action<int> confirm;
        private int maximum;
        public bool IsOpen => root.gameObject.activeSelf;
        public event Action Rejected, Cancelled;
        public StackQuantityDialog(RectTransform root, TMP_InputField input, TextMeshProUGUI prompt, Button accept, Button cancel)
        {
            this.root = root; this.input = input; this.prompt = prompt;
            accept.onClick.AddListener(Confirm); cancel.onClick.AddListener(Cancel); Hide();
        }
        public void Show(int quantity, Action<int> confirm)
        {
            maximum = quantity - 1; this.confirm = confirm; prompt.text = $"Split quantity (1 - {maximum})";
            input.text = Mathf.Max(1, quantity / 2).ToString(); root.gameObject.SetActive(true); root.SetAsLastSibling(); input.ActivateInputField();
        }
        public void Confirm()
        {
            if (!int.TryParse(input.text, out int quantity) || quantity < 1 || quantity > maximum)
            { prompt.text = $"Enter a whole number from 1 to {maximum}."; input.ActivateInputField(); Rejected?.Invoke(); return; }
            var callback = confirm; Hide(); callback?.Invoke(quantity);
        }
        public void Hide() { root.gameObject.SetActive(false); confirm = null; }
        private void Cancel() { if (!IsOpen) return; Hide(); Cancelled?.Invoke(); }
    }
}
