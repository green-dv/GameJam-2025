using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathZoneController : MonoBehaviour
{
    [SerializeField] Transform respawn;
    [SerializeField] Transform player;
    [SerializeField] SpriteRenderer fadePanel;
    [SerializeField] AudioSource deathSound;
    [SerializeField] float fadeDuration = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeTeleport());
        }
    }

    IEnumerator FadeTeleport()
    {
        // Fade a negro
        yield return StartCoroutine(Fade(1f));

        deathSound.Play();
        yield return new WaitForSeconds(0.1f);
        // Teletransportar al jugador
        player.position = respawn.position;
        
        
        // Esperar un momento (opcional)
        
        
        // Volver a transparente
        yield return StartCoroutine(Fade(0f));
    }

    IEnumerator Fade(float targetAlpha)
    {
        Color color = fadePanel.color;
        float startAlpha = color.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadePanel.color = color;
            yield return null;
        }
    }
}
