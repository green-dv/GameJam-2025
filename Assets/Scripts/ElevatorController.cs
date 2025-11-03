using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorController : MonoBehaviour
{
    [Header("Movimiento del ascensor")]
    [SerializeField] Transform target;
    [SerializeField] ActionableButton doorsControler;
    [SerializeField] float moveSpeed = 2f;

    [Header("Fade de pantalla")]
    [SerializeField] SpriteRenderer fadeImage;
    [SerializeField] float fadeDuration = 1f;

    public List<BoxCollider2D> colliders = new List<BoxCollider2D>();
    

    private Rigidbody2D rb;
    private bool isMoving = false;
    private Transform playerParentOriginal;

    public bool Temporizador = false;

    [Header("Player")]
    [SerializeField] SpriteRenderer playerSR;

    private List<SpriteRenderer> elevatorSprites = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, int> originalOrders = new Dictionary<SpriteRenderer, int>();

    private void Start()
    {
        GetComponentsInChildren(elevatorSprites);
        foreach (var sr in elevatorSprites)
        {
            originalOrders[sr] = sr.sortingOrder;
        }

        rb = GetComponent<Rigidbody2D>();

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);

        Temporizador = doorsControler.tempAct;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isMoving) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(ElevatorSequence(other.transform));
        }
    }

    IEnumerator ElevatorSequence(Transform player)
    {
        playerSR.sortingOrder += 20;
        foreach (var sr in elevatorSprites)
            sr.sortingOrder += 20;
        
        yield return StartCoroutine(FadeScreen(1f));
        foreach (BoxCollider2D col in colliders)
        {
            col.enabled = false;
        }
        yield return new WaitForSeconds(1f);
        isMoving = true;
        if(!Temporizador) doorsControler.EnableElevatorDoors();

        // Guarda el padre original del jugador
        playerParentOriginal = player.parent;

        player.SetParent(transform);

        yield return StartCoroutine(MoveElevator(target.position));

        // 🔹 Fade Out
        yield return StartCoroutine(FadeScreen(0f));

        yield return new WaitForSeconds(0.3f);
        // 🔹 Restaurar jerarquía original del jugador
        player.SetParent(playerParentOriginal);
        foreach (var sr in elevatorSprites)
            sr.sortingOrder = originalOrders[sr];
        playerSR.sortingOrder -= 20;
        doorsControler.ActivateDoors();

    }

    IEnumerator MoveElevator(Vector3 targetPos)
    {
        Vector2 startPos = rb.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            Vector2 newPos = Vector2.Lerp(startPos, targetPos, t);
            rb.MovePosition(newPos);
            yield return null;
        }
    }

    IEnumerator FadeScreen(float targetAlpha)
    {
        if (fadeImage == null) yield break;

        float startAlpha = fadeImage.color.a;
        float t = 0f;
        float duration = 0.3f; // 50 milisegundos

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }
    }
}
