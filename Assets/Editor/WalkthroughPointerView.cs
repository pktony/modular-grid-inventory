using System;
using UnityEngine;
namespace InventorySystem.Editor
{
    public sealed class WalkthroughPointerView : IDisposable
    {
        private readonly RectTransform overlay, root;
        private readonly Texture2D ringTexture;
        private readonly Sprite ringSprite;
        public WalkthroughPointerView(RectTransform overlay)
        {
            this.overlay = overlay;
            root = UIElementFactory.Rect("WalkthroughPointer",overlay,Vector2.zero,Vector2.zero);
            var ring = UIElementFactory.Panel("ClickRing",root,new Vector2(-16,16),new Vector2(32,32),Color.white);
            ringTexture = new Texture2D(64,64,TextureFormat.RGBA32,false);
            for (int y=0;y<64;y++) for (int x=0;x<64;x++)
            {
                float radius = Vector2.Distance(new Vector2(x,y),new Vector2(31.5f,31.5f));
                float alpha = Mathf.Clamp01(2.5f-Mathf.Abs(radius-26));
                ringTexture.SetPixel(x,y,new Color(0.95f,0.97f,0.97f,alpha*0.9f));
            }
            ringTexture.Apply();
            ringSprite = Sprite.Create(ringTexture,new Rect(0,0,64,64),new Vector2(0.5f,0.5f)); ring.sprite=ringSprite;
            root.gameObject.SetActive(false);
        }
        public void Show(Vector2 point,bool pressed)
        {
            root.gameObject.SetActive(pressed);
            if (!pressed) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(overlay,point,null,out var local);
            root.anchoredPosition=local; root.SetAsLastSibling();
        }
        public void Dispose()
        {
            root.gameObject.SetActive(false); UnityEngine.Object.Destroy(root.gameObject);
            UnityEngine.Object.Destroy(ringSprite);
            UnityEngine.Object.Destroy(ringTexture);
        }
    }
}
