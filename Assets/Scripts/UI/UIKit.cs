using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>Tiny code-driven UI builder (no prefabs, no TextMeshPro import dialogs).</summary>
public static class UIK
{
    static Font font;
    public static Font GetFont()
    {
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 16);
        }
        return font;
    }

    public static Canvas MakeCanvas(string name, int order)
    {
        var go = new GameObject(name);
        var c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay; c.sortingOrder = order;
        var s = go.AddComponent<CanvasScaler>();
        s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        s.referenceResolution = new Vector2(1920, 1080);
        s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        s.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return c;
    }

    public static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            var g = new GameObject("EventSystem");
            g.AddComponent<EventSystem>();
            g.AddComponent<StandaloneInputModule>();
        }
    }

    public static RectTransform NewRT(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        return rt;
    }

    public static RectTransform Rect(Transform parent, string name, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        var rt = NewRT(parent, name);
        rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = pivot;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        return rt;
    }

    public static RectTransform Full(Transform parent, string name)
    {
        var rt = NewRT(parent, name);
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        return rt;
    }

    public static Image Img(Transform parent, string name, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size, Color c)
    {
        var rt = Rect(parent, name, anchor, pivot, pos, size);
        var i = rt.gameObject.AddComponent<Image>();
        i.color = c;
        return i;
    }

    public static Text Txt(Transform parent, string s, int size, TextAnchor al, Color c, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 sz)
    {
        var rt = Rect(parent, "Text", anchor, pivot, pos, sz);
        var t = rt.gameObject.AddComponent<Text>();
        t.font = GetFont(); t.fontSize = size; t.alignment = al; t.color = c; t.text = s;
        t.fontStyle = FontStyle.Bold;
        t.horizontalOverflow = HorizontalWrapMode.Overflow; t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        var sh = rt.gameObject.AddComponent<Shadow>();
        sh.effectDistance = new Vector2(2f, -2f);
        return t;
    }

    public static Button Btn(Transform parent, string label, Vector2 anchor, Vector2 pos, Vector2 size, UnityAction onClick, int fs = 34)
    {
        var img = Img(parent, label + "Button", anchor, new Vector2(0.5f, 0.5f), pos, size, Color.white);
        var b = img.gameObject.AddComponent<Button>();
        b.targetGraphic = img;
        var cb = b.colors;
        cb.normalColor = new Color(0.2f, 0.26f, 0.16f, 0.95f);
        cb.highlightedColor = new Color(0.5f, 0.6f, 0.25f, 1f);
        cb.pressedColor = new Color(0.12f, 0.16f, 0.08f, 1f);
        cb.selectedColor = new Color(0.2f, 0.26f, 0.16f, 0.95f);
        cb.fadeDuration = 0.08f;
        b.colors = cb;
        var ol = img.gameObject.AddComponent<Outline>();
        ol.effectColor = new Color(0.9f, 0.8f, 0.4f, 0.6f); ol.effectDistance = new Vector2(2, -2);
        Txt(img.transform, label, fs, TextAnchor.MiddleCenter, new Color(0.95f, 0.95f, 0.85f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, size);
        b.onClick.AddListener(() => { if (AudioManager.I != null) AudioManager.I.Play(Sfx.Click); });
        if (onClick != null) b.onClick.AddListener(onClick);
        return b;
    }

    public static Slider MakeSlider(Transform parent, Vector2 anchor, Vector2 pos, Vector2 size, float value, UnityAction<float> cb)
    {
        var rt = Rect(parent, "Slider", anchor, new Vector2(0.5f, 0.5f), pos, size);
        var s = rt.gameObject.AddComponent<Slider>();
        var bg = NewRT(rt, "Bg");
        bg.anchorMin = new Vector2(0, 0.35f); bg.anchorMax = new Vector2(1, 0.65f); bg.offsetMin = Vector2.zero; bg.offsetMax = Vector2.zero;
        bg.gameObject.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.95f);
        var fa = NewRT(rt, "FillArea");
        fa.anchorMin = new Vector2(0, 0.35f); fa.anchorMax = new Vector2(1, 0.65f); fa.offsetMin = Vector2.zero; fa.offsetMax = Vector2.zero;
        var fill = NewRT(fa, "Fill");
        fill.anchorMin = Vector2.zero; fill.anchorMax = new Vector2(0, 1); fill.offsetMin = Vector2.zero; fill.offsetMax = Vector2.zero;
        fill.gameObject.AddComponent<Image>().color = new Color(0.55f, 0.7f, 0.25f);
        var ha = NewRT(rt, "HandleArea");
        ha.anchorMin = Vector2.zero; ha.anchorMax = Vector2.one; ha.offsetMin = new Vector2(12, 0); ha.offsetMax = new Vector2(-12, 0);
        var h = NewRT(ha, "Handle");
        h.anchorMin = new Vector2(0, 0); h.anchorMax = new Vector2(0, 1); h.sizeDelta = new Vector2(26, 0);
        var hi = h.gameObject.AddComponent<Image>(); hi.color = new Color(0.9f, 0.85f, 0.6f);
        s.fillRect = fill; s.handleRect = h; s.targetGraphic = hi;
        s.direction = Slider.Direction.LeftToRight; s.minValue = 0f; s.maxValue = 1f; s.value = value;
        s.onValueChanged.AddListener(cb);
        return s;
    }
}
