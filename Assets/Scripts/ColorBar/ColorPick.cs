
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorPick : MonoBehaviour
{

    public Image Saturation;
    public Image Hue;
    public Image Paint;

    public RectTransform Point_Stauration;
    public RectTransform Point_Hue;

    private Sprite Saturation_Sprite;
    private Sprite Hue_Sprite;

    private Color32 currentHue = Color.red;


    private void Awake()
    {
        UpdateStauration();
        UpdateHue();
    }

    private void Start()
    {
        
    }

    float sWidth = 200, sHeight = 200;
    //更新饱和度
    private void UpdateStauration()
    {

        Saturation_Sprite = Sprite.Create(new Texture2D((int)sWidth, (int)sHeight), new Rect(0, 0, sWidth, sHeight), new Vector2(0, 0));

      
        for (int y = 0; y <= sHeight; y++)
        {
            for (int x = 0; x < sWidth; x++)
            {
                var pixColor = GetSaturation(currentHue, x / sWidth, y / sHeight);
                Saturation_Sprite.texture.SetPixel(x, ((int)sHeight - y), pixColor);
            }
        }
        Saturation_Sprite.texture.Apply();

        Saturation.sprite = Saturation_Sprite;
    }

    //更新色泽度 
    private void UpdateHue()
    {

        float w = 50, h = 50;

        Hue_Sprite = Sprite.Create(new Texture2D((int)w, (int)h), new Rect(0, 0, w, h), new Vector2(0, 0));

        for (int y = 0; y <= h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var pixColor = GetHue(y / h);
                Hue_Sprite.texture.SetPixel(x, ((int)h - y), pixColor);
            }
        }
        Hue_Sprite.texture.Apply();

        Hue.sprite = Hue_Sprite;
    }

    private Vector2 clickPoint = Vector2.zero;
    public void OnStaurationClick(ColorPickClick sender)
    {
        var size2 = Saturation.rectTransform.sizeDelta / 2;
        var pos = Vector2.zero;
        pos.x = Mathf.Clamp(sender.ClickPoint.x, -size2.x, size2.x);
        pos.y = Mathf.Clamp(sender.ClickPoint.y, -size2.y, size2.y);
        Point_Stauration.anchoredPosition = clickPoint = pos;

        UpdateColor();
    }

    public void UpdateColor()
    {
        var size2 = Saturation.rectTransform.sizeDelta / 2;
        var pos = clickPoint;
        pos += size2;

        var color = GetSaturation(currentHue, pos.x / Saturation.rectTransform.sizeDelta.x, 1 - pos.y / Saturation.rectTransform.sizeDelta.y);
        Paint.color = color;
    }

    public void OnHueClick(ColorPickClick sender)
    {
        var h = Hue.rectTransform.sizeDelta.y / 2.0f;
        var y = Mathf.Clamp(sender.ClickPoint.y, -h, h);
        Point_Hue.anchoredPosition = new Vector2(0, y);

        y += h;
        currentHue = GetHue(1 - y / Hue.rectTransform.sizeDelta.y);
        UpdateStauration();
        UpdateColor();
    }


    private static Color GetSaturation(Color color, float x, float y)
    {
        Color newColor = Color.white;
        for (int i = 0; i < 3; i++)
        {
            if (color[i] != 1)
            {
                newColor[i] = (1 - color[i]) * (1 - x) + color[i];
            }
        }

        newColor *= (1 - y);
        newColor.a = 1;
        return newColor;
    }


    //B,r,G,b,R,g //大写是升，小写是降
    private readonly static int[] hues = new int[] { 2, 0, 1, 2, 0, 1 };

    private readonly static Color[] colors = new Color[] { Color.red, Color.blue, Color.blue, Color.green, Color.green, Color.red };

    private readonly static float c = 1.0f / hues.Length;

    private static Color GetHue(float y)
    {
        y = Mathf.Clamp01(y);

        var index = (int)(y / c);

        var h = hues[index];

        var newColor = colors[index];

        float less = (y - index * c) / c;

        newColor[h] = index % 2 == 0 ? less : 1 - less;

        return newColor;
    }

}
