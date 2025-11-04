using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorObstaculos : MonoBehaviour
{
    [SerializeField] Transform objeto;
    [SerializeField] bool moveX = false;
    [SerializeField] float move = 1f; 
    [SerializeField] float speed = 1f;     
    [SerializeField] int tiempo = 5;     
    [SerializeField] AudioSource ticTackSF;
    [SerializeField] AudioSource doorStatus;

    private Vector3 initialPos;
    public bool ejecutando = false;

    private void Start()
    {
        if (objeto != null)
            initialPos = objeto.position;
    }

    public void ActivateAction()
    {
        if(!ejecutando)
            StartCoroutine(DoorSequence());
    }

    private IEnumerator DoorSequence()
    {
        ejecutando = true;
        if (ticTackSF != null) ticTackSF.Play();

        yield return StartCoroutine(MoveObject(initialPos + GetMoveVector(move)));
        yield return new WaitForSeconds(tiempo);
        yield return StartCoroutine(MoveObject(initialPos));

        if (ticTackSF != null) ticTackSF.Stop();
        ejecutando = false;
    }

    private Vector3 GetMoveVector(float distance)
    {
        return moveX ? new Vector3(distance, 0f, 0f) : new Vector3(0f, distance, 0f);
    }

    private IEnumerator MoveObject(Vector3 targetPos)
    {
        Vector3 startPos = objeto.position;

        if (doorStatus != null) doorStatus.Play();

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            objeto.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        objeto.position = targetPos;
    }
}
