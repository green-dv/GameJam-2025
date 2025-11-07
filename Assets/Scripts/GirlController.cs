using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GirlController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float speed = 6.0f;
    [SerializeField] float jumpForce = 6.0f;

    [Header("Detección de suelo")]
    [SerializeField] float groundDistance = 0.15f;
    [SerializeField] float closeToGround = 0.5f;
    [SerializeField] LayerMask groundMask;

    [Header("Canalización del tiempo (habilidad especial)")]
    [SerializeField] float channelTime = 1.5f; // duración del canal
    [SerializeField] float cooldown = 4f;      // enfriamiento
    [SerializeField] Image abilityImage;
    [SerializeField] Image timePanel;
    [SerializeField] float transitionSpeed = 2f;

    [Header("Colores por estado temporal")]
    [SerializeField] Color futureColor = new Color(70f / 255f, 50f / 255f, 90f / 255f, 110f / 255f);
    [SerializeField] Color pastColor = new Color(100f / 255f, 100f / 255f, 70f / 255f, 90f / 255f);
    [SerializeField] Color normalColor = new Color(20f / 255f, 35f / 255f, 60f / 255f, 170f / 255f);

    [Header("Referencias externas")]
    [SerializeField] Animator animations;
    public TimeObjectController timeObjects;

    Rigidbody2D rb;
    PlayerInput actionMaps;
    Collider2D col;

    bool isGrounded = false;
    bool isWalking = false;
    bool isChanneling = false;
    bool canUseAbility = true;

    enum TimeState { Present, Future, Past }
    TimeState currentState = TimeState.Present;

    float lastMoveDir = 1f;
    float stopDelay = 0.05f;
    float lastMoveTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        actionMaps = GetComponent<PlayerInput>();

        if (abilityImage != null) abilityImage.fillAmount = 1f;
        if (timePanel != null) timePanel.color = normalColor;

        // Asignar acciones
        actionMaps.actions["Jump"].performed += Jump;
        actionMaps.actions["HabilidadEspecial"].performed += HabilidadEspecial;
    }

    void Update()
    {
        if (!isChanneling)
        {
            CheckGround();
            MovePlayerInput();
        }
    }

    // ---------------- MOVIMIENTO ----------------
    void MovePlayerInput()
    {
        Vector2 input = actionMaps.actions["Move"].ReadValue<Vector2>();
        float moveX = input.x;

        if (moveX != 0)
        {
            lastMoveTime = Time.time;
            lastMoveDir = Mathf.Sign(moveX);
            GetComponent<SpriteRenderer>().flipX = moveX < 0;

            if (!isWalking)
            {
                isWalking = true;
                animations.SetBool("Walking", true);
            }
        }
        else if (Time.time - lastMoveTime > stopDelay && isWalking)
        {
            isWalking = false;
            animations.SetBool("Walking", false);
        }

        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
    }

    // ---------------- SALTO ----------------
    void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isChanneling)
        {
            animations.SetBool("CloseToGround", false);
            animations.SetTrigger("Jumping");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // ---------------- DETECCIÓN DE SUELO ----------------
    void CheckGround()
    {
        Vector2 originRight = new Vector2(col.bounds.max.x, col.bounds.min.y);
        Vector2 originLeft = new Vector2(col.bounds.min.x, col.bounds.min.y);

        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.down, groundDistance, groundMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.down, groundDistance, groundMask);

        RaycastHit2D hitRightClose = Physics2D.Raycast(originRight, Vector2.down, closeToGround, groundMask);
        RaycastHit2D hitLeftClose = Physics2D.Raycast(originLeft, Vector2.down, closeToGround, groundMask);

        bool closeToGroundDetected = hitRightClose.collider != null || hitLeftClose.collider != null;
        animations.SetBool("CloseToGround", closeToGroundDetected);

        isGrounded = hitRight.collider != null || hitLeft.collider != null;
        animations.SetBool("Grounded", isGrounded);
    }

    // ---------------- HABILIDAD ESPECIAL ----------------
    void HabilidadEspecial(InputAction.CallbackContext context)
    {
        if (!context.performed || !canUseAbility || isChanneling) return;
        StartCoroutine(ChannelTimeAbility());
    }

    IEnumerator ChannelTimeAbility()
    {
        isChanneling = true;
        canUseAbility = false;
        float tempSpeed = speed;
        speed = 0f;

        // animación de canalización
        animations.SetTrigger("Special");

        float elapsed = 0f;
        while (elapsed <= channelTime)
        {
            elapsed += Time.deltaTime;
            if (abilityImage != null)
                abilityImage.fillAmount = 1 - (elapsed / channelTime);
            yield return null;
        }

        // Cambio de tiempo
        ChangeTimeState();

        // Reiniciar
        speed = tempSpeed;
        isChanneling = false;

        // Cooldown
        elapsed = 0f;
        while (elapsed <= cooldown)
        {
            elapsed += Time.deltaTime;
            if (abilityImage != null)
                abilityImage.fillAmount = (elapsed / cooldown);
            yield return null;
        }

        canUseAbility = true;
        if (abilityImage != null) abilityImage.fillAmount = 1f;
    }

    void ChangeTimeState()
    {
        switch (currentState)
        {
            case TimeState.Present:
                StartCoroutine(ChangePanelColor(futureColor));
                currentState = TimeState.Future;
                timeObjects?.SetTimeState(true, false);
                break;
            case TimeState.Future:
                StartCoroutine(ChangePanelColor(pastColor));
                currentState = TimeState.Past;
                timeObjects?.SetTimeState(false, true);
                break;
            case TimeState.Past:
                StartCoroutine(ChangePanelColor(normalColor));
                currentState = TimeState.Present;
                timeObjects?.SetTimeState(false, false);
                break;
        }
    }

    IEnumerator ChangePanelColor(Color targetColor)
    {
        if (timePanel == null) yield break;

        Color start = timePanel.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            timePanel.color = Color.Lerp(start, targetColor, t);
            yield return null;
        }
    }
}
