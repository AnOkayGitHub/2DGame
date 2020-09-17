using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashSprite : MonoBehaviour
{
    public Color flashColor;
    public float flashTime;
    public SpriteRenderer spriteRenderer;
    public float t = 1;

    void Update() {
        if (t < 1) { 
            t += Time.deltaTime  / flashTime;
            spriteRenderer.color = Color.Lerp(flashColor, Color.white, t);
        }
    }

    void Flash() {
        t = 0;
        spriteRenderer.color = flashColor;
    }
}
