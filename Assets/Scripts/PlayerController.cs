using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerController : MonoBehaviour
{
    [Header("Variables de movimiento")]
    [SerializeField] float speed = 6.0f;
    [SerializeField] float jumpForce = 6.0f;

    [Header("Deteccion de suelo")]
    [SerializeField] float groundDistance = 0.1f;
    [SerializeField] LayerMask groundMask;

    [Header("Dash (habilidad especial)")]
    [SerializeField] float dashForce = 2f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] float dashCooldown = 2f;
    [SerializeField] bool disableGravityDuringDash = true;
    [SerializeField] Image abilityImage;

    Rigidbody2D rb;
    PlayerInput actionMaps;
    Collider2D col;
    bool isGrounded = false;
    //* * * TEMPORAL BOOL (CHANGE FOR TAGS LATER)
    bool isPlayerOne = true;
    //* * * DASHING OPTIONS * * *
    bool isDashing = false;
    float lastDashTime = -999f;
    float lastMoveDir = 1f;
    Destroyable destroyable = null;
    //* * * TIME MANIPULATION CONTROL * * *
    Coroutine executedCoroutine;
    void Start()
    {
        abilityImage.fillAmount = 1;
        rb = GetComponent<Rigidbody2D>();
        actionMaps = GetComponent<PlayerInput>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!isDashing)
        {
            CheckGround();
            MovePlayerInput();
        }
        if (isDashing)
        {
            if (destroyable != null)
            {
                rb.velocity = Vector2.zero;
                destroyable.Destroy();
            }
        }
    }

    public void CheckGround()
    {
        Vector2 originRight = new Vector2(col.bounds.max.x, col.bounds.min.y);
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.down, groundDistance, groundMask);

        Vector2 originLeft = new Vector2(col.bounds.min.x, col.bounds.min.y);
        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.down, groundDistance, groundMask);
        isGrounded = hitRight.collider != null || hitLeft.collider != null;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isDashing)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void MovePlayerInput()
    {
        Vector2 inputs = actionMaps.actions["Move"].ReadValue<Vector2>();
        //* * * OBTENEMOS LA DIRECCION A LA QUE SE APUNTA * * *
        if (inputs.x != 0)
        {
            lastMoveDir = Mathf.Sign(inputs.x);
        }

        Vector2 move = new Vector2(inputs.x * speed, rb.velocity.y);
        rb.velocity = move;
    }
    #region Time Manipulation
    public void ChangeTimeFuture(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            executedCoroutine = StartCoroutine(PerformDash(lastMoveDir));
        }
    }

    public void ChangeTimePast(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            executedCoroutine = StartCoroutine(PerformDash(lastMoveDir));
        }
    }
    #endregion
    #region DASH CODE
    public void DashAbility(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            executedCoroutine = StartCoroutine(PerformDash(lastMoveDir));
        }
    }
    IEnumerator PerformDash(float dir)
    {
        isDashing = true;
        lastDashTime = Time.time;
        //* * * DESACTIVAMOS LA GRAVEDAD * * *
        float prevGravity = rb.gravityScale;
        if (disableGravityDuringDash)
            rb.gravityScale = 0f;
        //* * * REALIZAMOS EL DASH CON LA POSICION GUARDADA * * *
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(dir * dashForce, 0f), ForceMode2D.Impulse);
        float imageFill = 1f;
        float elapsedTime = 0;
        while(elapsedTime <= dashDuration)
        {
            elapsedTime += Time.deltaTime;
            imageFill = 1 - (elapsedTime / dashDuration);
            abilityImage.fillAmount = imageFill;
            yield return null;
        }
        abilityImage.fillAmount = 0f;
        //* * * REACTIVAMOS LA GRAVEDAD Y QUITAMOS LA VELOCIDAD * * *
        rb.velocity = Vector2.zero;
        rb.gravityScale = prevGravity;
        //* * * COOLDOWN * * *
        isDashing = false;
        elapsedTime = 0;
        while (elapsedTime <= dashCooldown)
        {
            elapsedTime += Time.deltaTime;
            imageFill = (elapsedTime / dashCooldown);
            abilityImage.fillAmount = imageFill;
            yield return null;
        }
        abilityImage.fillAmount = 1f;
    }
    #endregion
    private void OnTriggerExit(Collider other)
    {
        if (isDashing)
        {
            if (other.gameObject.CompareTag("DestroyableObject"))
            {
                destroyable = null;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDashing)
        {
            if (collision.gameObject.CompareTag("DestroyableObject"))
            {
                destroyable = collision.gameObject.GetComponent<Destroyable>();
            }
        }
    }
}
