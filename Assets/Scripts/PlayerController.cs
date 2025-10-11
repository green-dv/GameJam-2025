using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Variables de movimiento")]
    [SerializeField] float speed = 6.0f;
    [SerializeField] float jumpForce = 6.0f;

    [Header("Deteccion de suelo")]
    [SerializeField] float groundDistance = 0.1f;
    [SerializeField] LayerMask groundMask;

    Rigidbody2D rb;
    PlayerInput actionMaps;
    Collider2D col;
    bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        actionMaps = GetComponent<PlayerInput>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        CheckGround();
        MovePlayerInput();
    }
    public void CheckGround()
    {
        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundDistance, groundMask);
        Debug.DrawRay(origin, Vector2.down * groundDistance, hit ? Color.green : Color.red);
        isGrounded = hit.collider != null;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void MovePlayerInput()
    {
        Vector2 inputs = actionMaps.actions["Move"].ReadValue<Vector2>();
        
        Vector2 move = new Vector2(inputs.x * speed, rb.velocity.y);
        rb.velocity = move;
    }
}
