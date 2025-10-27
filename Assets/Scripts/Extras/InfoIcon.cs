using System.Collections;
using UnityEngine;

public class InfoIcon : MonoBehaviour
{
    public FadeTextUI fadeUI;
    public GameObject infoContainer; // Asigna el mismo objeto que tiene el FadeTextUI

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Activa el contenedor antes de iniciar el fade
            if (!infoContainer.activeSelf)
                infoContainer.SetActive(true);

            fadeUI.FadeIn();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Ejecuta fade y luego desactiva el objeto desde aquí
            StartCoroutine(FadeAndDeactivate());
        }
    }

    private IEnumerator FadeAndDeactivate()
    {
        // Espera a que termine el fade out antes de desactivar
        yield return fadeUI.StartCoroutine(fadeUI.FadeTo(0f));
        infoContainer.SetActive(false);
    }
}
