using System;
using TMPro;
using UnityEngine;
namespace InventorySystem.Editor
{
    public sealed class WalkthroughPointerView : IDisposable
    {
        private readonly RectTransform overlay, root, ring;
        private readonly TextMeshProUGUI label;
        private readonly Texture2D arrowTexture, ringTexture;
        private readonly Sprite arrowSprite, ringSprite;
        public WalkthroughPointerView(RectTransform overlay)
        {
            this.overlay = overlay;
            root = UIElementFactory.Rect("WalkthroughPointer",overlay,Vector2.zero,new Vector2(32,40));
            var pulse = UIElementFactory.Panel("ClickRing",root,new Vector2(-28,28),new Vector2(56,56),Color.white);
            ring = pulse.rectTransform;
            ringTexture = new Texture2D(64,64,TextureFormat.RGBA32,false);
            for (int y=0;y<64;y++) for (int x=0;x<64;x++)
            {
                float radius = Vector2.Distance(new Vector2(x,y),new Vector2(31.5f,31.5f));
                ringTexture.SetPixel(x,y,radius>22 && radius<28 ? new Color(1,0.75f,0.2f,0.9f) : Color.clear);
            }
            ringTexture.Apply();
            ringSprite = Sprite.Create(ringTexture,new Rect(0,0,64,64),new Vector2(0.5f,0.5f)); pulse.sprite=ringSprite;
            arrowTexture = new Texture2D(24,32,TextureFormat.RGBA32,false);
            for (int y=0;y<32;y++) for (int x=0;x<24;x++)
            {
                bool triangle=y<25 && x<=y*0.65f;
                bool stem=y>=18 && y<31 && x>=7 && x<=11;
                arrowTexture.SetPixel(x,31-y,triangle || stem ? Color.white : Color.clear);
            }
            arrowTexture.Apply();
            arrowSprite=Sprite.Create(arrowTexture,new Rect(0,0,24,32),new Vector2(0,1));
            UIElementFactory.Panel("Arrow",root,Vector2.zero,new Vector2(24,32),Color.white).sprite=arrowSprite;
            label=UIElementFactory.Label("Action",root,new Vector2(30,-8),new Vector2(135,28),"",18,new Color(1,0.8f,0.3f));
        }
        public void Show(Vector2 point,string action,bool pressed,float time)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(overlay,point,null,out var local);
            root.anchoredPosition=local; root.SetAsLastSibling();
            ring.gameObject.SetActive(pressed);
            ring.localScale=Vector3.one*(1+0.12f*Mathf.Sin(time*12)); label.text=action;
            label.rectTransform.anchoredPosition=new Vector2(local.x>overlay.rect.width-170 ? -140 : 30,-8);
        }
        public void Dispose()
        {
            root.gameObject.SetActive(false); UnityEngine.Object.Destroy(root.gameObject);
            UnityEngine.Object.Destroy(arrowSprite); UnityEngine.Object.Destroy(ringSprite);
            UnityEngine.Object.Destroy(arrowTexture); UnityEngine.Object.Destroy(ringTexture);
        }
    }
}
