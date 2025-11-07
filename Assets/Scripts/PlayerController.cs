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
    [SerializeField] float closeToGround = 0.5f;
    [SerializeField] LayerMask groundMask;

    [Header("Dash (habilidad especial)")]
    [SerializeField] float dashForce = 2f;
    [SerializeField] float dashDuration = 0.15f;
    [SerializeField] float dashCooldown = 2f;
    [SerializeField] bool disableGravityDuringDash = true;
    [SerializeField] Image abilityImage;

    [SerializeField] Animator animations;
    Rigidbody2D rb;
    PlayerInput actionMaps;
    Collider2D col;
    bool isGrounded = false;
    bool isWalking = false;
    //* * * TEMPORAL BOOL (CHANGE FOR TAGS LATER)
    bool isPlayerOne = true;
    //* * * DASHING OPTIONS * * *
    bool isDashing = false;
    float lastDashTime = -999f;
    float lastMoveDir = 1f;
    Destroyable destroyable = null;
    ActionableButton actionableButton = null;
    ActivadorObstaculos activadorObstaculos = null;
    CintaController cintaController = null;
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
                destroyable.Destroy();
            }
            if(actionableButton != null)
            {
                actionableButton.ActivateAction();
            }
            if(activadorObstaculos != null)
            {
                activadorObstaculos.ActivateAction();
            }
            if(cintaController != null)
            {
                Debug.Log("2");
                cintaController.ChangeStatus();
            }
        }
    }

    public void CheckGround()
    {
        if (!isGrounded)
        {
            Vector2 originRightClose = new Vector2(col.bounds.max.x, col.bounds.min.y);
            RaycastHit2D hitRightClose = Physics2D.Raycast(originRightClose, Vector2.down, closeToGround, groundMask);

            Vector2 originLeftClose = new Vector2(col.bounds.min.x, col.bounds.min.y);
            RaycastHit2D hitLeftClose = Physics2D.Raycast(originLeftClose, Vector2.down, closeToGround, groundMask);
            animations.SetBool("CloseToGround", hitRightClose.collider != null || hitLeftClose.collider != null);

        }
        // * * * GROUNDED * * *
        Vector2 originRight = new Vector2(col.bounds.max.x, col.bounds.min.y);
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.down, groundDistance, groundMask);

        Vector2 originLeft = new Vector2(col.bounds.min.x, col.bounds.min.y);
        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.down, groundDistance, groundMask);

        isGrounded = hitRight.collider != null || hitLeft.collider != null;
        animations.SetBool("Grounded", isGrounded);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isDashing)
        {
            animations.SetBool("CloseToGround", false);
            animations.SetTrigger("Jumping");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    public void IsWalking(InputAction.CallbackContext context)
    {

    }
    [SerializeField] float stopDelay = 0.05f;
    float lastMoveTime = 0f;

    void MovePlayerInput()
    {
        Vector2 inputs = actionMaps.actions["Move"].ReadValue<Vector2>();
        float moveX = inputs.x;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        // Si hay movimiento (izq o der)
        if (moveX != 0)
        {
            lastMoveTime = Time.time;
            lastMoveDir = Mathf.Sign(moveX);

            if (sprite != null)
                sprite.flipX = moveX < 0;

            if (!isWalking)
            {
                isWalking = true;
                animations.SetBool("Walking", true);
            }
        }
        else
        {
            // Solo se detiene si ya pas� el delay desde el �ltimo movimiento
            if (Time.time - lastMoveTime > stopDelay && isWalking)
            {
                isWalking = false;
                animations.SetBool("Walking", false);
            }
        }

        // Movimiento del personaje
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
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
        animations.SetTrigger("Dashing");
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
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyableObject"))
        {
            destroyable = null;
        }
        if (collision.gameObject.CompareTag("ActionableButton"))
        {
            actionableButton = null;
        }
        if (collision.gameObject.CompareTag("activadorObstaculos"))
        {
            activadorObstaculos = null;
        }
        if (collision.gameObject.CompareTag("cintaController"))
        {
            cintaController = null;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyableObject"))
        {
            destroyable = collision.gameObject.GetComponent<Destroyable>();
        }
        if (collision.gameObject.CompareTag("ActionableButton"))
        {
            actionableButton = collision.gameObject.GetComponent<ActionableButton>();
        }
        if (collision.gameObject.CompareTag("activadorObstaculos"))
        {
            activadorObstaculos = collision.gameObject.GetComponent<ActivadorObstaculos>();
        }
        if (collision.gameObject.CompareTag("cintaController"))
        {
            Debug.Log("1");
            cintaController = collision.gameObject.GetComponent<CintaController>();
        }
    }
}
