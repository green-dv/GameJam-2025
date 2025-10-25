using System.Collections;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;

    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        
        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines(); 
            StartCoroutine(FadeSprites(0.2f, true));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            StopAllCoroutines(); 
            StartCoroutine(FadeSprites(0.2f, false));
        }
    }

    IEnumerator FadeSprites(float duration, bool isFadingOut)
    {
        float time = 0f;


        while (time < duration)
        {
            time += Time.deltaTime;
            float lerpFactor = time / duration;
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color startColor = isFadingOut ? originalColors[i] : new Color(0f, 0f, 0f, 0f);
                Color targetColor = isFadingOut ? new Color(0f, 0f, 0f, 0f) : originalColors[i];
                spriteRenderers[i].color = Color.Lerp(startColor, targetColor, lerpFactor);
            }
            
            yield return null;
        }


        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = isFadingOut ? new Color(0f, 0f, 0f, 0f) : originalColors[i];
        }
    }
}