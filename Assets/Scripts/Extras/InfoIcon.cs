using System.Collections;
using UnityEngine;

public class InfoIcon : MonoBehaviour
{
    public FadeTextUI fadeUI;
    public GameObject infoContainer; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!infoContainer.activeSelf)
                infoContainer.SetActive(true);

            fadeUI.FadeIn();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeAndDeactivate());
        }
    }

    private IEnumerator FadeAndDeactivate()
    {
        yield return fadeUI.StartCoroutine(fadeUI.FadeTo(0f));
        infoContainer.SetActive(false);
    }
}
