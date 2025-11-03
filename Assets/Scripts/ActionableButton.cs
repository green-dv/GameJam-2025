using System.Collections;
using UnityEngine;

public class ActionableButton : MonoBehaviour
{
    [SerializeField] int option = 0;

    [Header("Puertas de ascensor")]
    [SerializeField] GameObject puertaIzq;
    [SerializeField] GameObject puertaDer;
    [SerializeField] float desplazamiento = 0.43f;
    [SerializeField] float velocidad = 1.5f;

    [Header("Temporizador (opcional)")]
    [SerializeField] float tiempoTemp = 5f;
    public bool tempAct = false;

    // Puertas Ascensor
    BoxCollider2D puertaIzqBC;
    BoxCollider2D puertaDerBC;


    bool activated = false;

    void Start()
    {
        if (puertaIzq != null && puertaDer != null)
        {
            puertaIzqBC = puertaIzq.GetComponent<BoxCollider2D>();
            puertaDerBC = puertaDer.GetComponent<BoxCollider2D>();
        }
    }

    public void ActivateAction()
    {
        if (tempAct)
        {
            activated = false;
            tempAct = false;
        }
        if (activated)
        {
            Debug.Log("Return");
            return;
        }
        switch (option)
        {
            case 0: // Puerta ascensor
                DisableElevatorDoors();
                StartCoroutine(AbrirPuertasAscensor(-1));
                break;

            case 1:
                
                Debug.Log("1");
                DisableElevatorDoors();
                StartCoroutine(AbrirPuertasAscensor(-1));
                StartCoroutine(TemporizadorPuertas(tiempoTemp));
                break;
        }
    }

    IEnumerator AbrirPuertasAscensor(int asdf)
    {


        Vector3 posInicialIzq = puertaIzq.transform.localPosition;
        Vector3 posInicialDer = puertaDer.transform.localPosition;

        Vector3 posFinalIzq = posInicialIzq + Vector3.left * desplazamiento * asdf;
        Vector3 posFinalDer = posInicialDer + Vector3.right * desplazamiento * asdf;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * velocidad;

            puertaIzq.transform.localPosition = Vector3.Lerp(posInicialIzq, posFinalIzq, t);
            puertaDer.transform.localPosition = Vector3.Lerp(posInicialDer, posFinalDer, t);

            yield return null;
        }

        // Asegurar posiciones finales exactas
        puertaIzq.transform.localPosition = posFinalIzq;
        puertaDer.transform.localPosition = posFinalDer;
        activated = true;
    }

    IEnumerator TemporizadorPuertas(float temp)
    {
        yield return new WaitForSeconds(temp);
        EnableElevatorDoors();
        tempAct = true;
        activated = false;
    }

    public void ActivateDoors()
    {
        DisableElevatorDoors();
        StartCoroutine(AbrirPuertasAscensor(-1));
    }
    public void DisableElevatorDoors()
    {
        puertaIzqBC.isTrigger = true;
        puertaDerBC.isTrigger = true;
    }
    public void EnableElevatorDoors()
    {
        puertaIzqBC.isTrigger = false;
        puertaDerBC.isTrigger = false;
        StartCoroutine(AbrirPuertasAscensor(1));
    }
}
