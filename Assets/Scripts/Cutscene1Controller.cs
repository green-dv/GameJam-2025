using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene1Controller : MonoBehaviour
{
    public GameObject cutsceneCanvas;
    public Image[] panels;
    public float fadeDuration = 1f;
    public float displayDuration = 2f;

    public AudioSource audio1; // Sonido para panel 3
    public AudioSource audio2; // Sonido para panel 6

    private bool isRunning = false;

    void Start()
    {
        if (cutsceneCanvas != null)
            cutsceneCanvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador detectado, iniciando cutscene...");
            StartCutscene();
        }
    }

    public void StartCutscene()
    {
        if (!isRunning)
            StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        isRunning = true;
        cutsceneCanvas.gameObject.SetActive(true);

        foreach (var panel in panels)
            SetAlpha(panel, 0);

        for (int i = 0; i < panels.Length; i++)
        {
            var panel = panels[i];

            // Fade in
            yield return StartCoroutine(Fade(panel, 0, 1, fadeDuration));

            // 🔊 Reproducir sonidos en paneles específicos
            if (i == 2 && audio1 != null) audio1.Play(); // Panel #3
            if (i == 5 && audio2 != null) audio2.Play(); // Panel #6

            // Mantener visible
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(Fade(panel, 1, 0, fadeDuration));
        }

        cutsceneCanvas.gameObject.SetActive(false);
        isRunning = false;
    }

    private IEnumerator Fade(Image image, float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;
        Color color = image.color;

        while (time < duration)
        {
            float t = time / duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            image.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        image.color = color;
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
