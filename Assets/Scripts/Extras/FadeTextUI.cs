using System.Collections;
using UnityEngine;

public class FadeTextUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public SpriteRenderer spriteToFade;
    public float fadeSpeed = 2f;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        if (spriteToFade != null)
        {
            Color c = spriteToFade.color;
            c.a = 0f;
            spriteToFade.color = c;
        }
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(1f));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeTo(0f));
    }

    // 👇 HAZ ESTE CAMBIO
    public IEnumerator FadeTo(float target)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, target))
        {
            float newAlpha = Mathf.MoveTowards(canvasGroup.alpha, target, fadeSpeed * Time.deltaTime);
            canvasGroup.alpha = newAlpha;

            if (spriteToFade != null)
            {
                Color c = spriteToFade.color;
                c.a = newAlpha;
                spriteToFade.color = c;
            }

            yield return null;
        }
    }
}
