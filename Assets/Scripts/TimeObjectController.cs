using System.Collections.Generic;
using UnityEngine;

public class TimeObjectController : MonoBehaviour
{
    // Listas internas para controlar los objetos
    private List<GameObject> futureObjects = new List<GameObject>();
    private List<GameObject> pastObjects = new List<GameObject>();

    void Awake()
    {
        // Buscar todos los objetos con tag FutureOnly y PastOnly en la escena
        GameObject[] futures = GameObject.FindGameObjectsWithTag("FutureOnly");
        futureObjects.AddRange(futures);

        GameObject[] pasts = GameObject.FindGameObjectsWithTag("PastOnly");
        pastObjects.AddRange(pasts);

        // Asegurarse de que al inicio solo esté presente el estado actual (presente)
        SetTimeState(false, false);
    }

    // Activar/desactivar objetos según el tiempo
    // futureActive: si estamos en futuro
    // pastActive: si estamos en pasado
    public void SetTimeState(bool futureActive, bool pastActive)
    {
        foreach (GameObject obj in futureObjects)
        {
            if (obj != null) obj.SetActive(futureActive);
        }

        foreach (GameObject obj in pastObjects)
        {
            if (obj != null) obj.SetActive(pastActive);
        }
    }
}
