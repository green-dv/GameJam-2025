using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FemaleForestMovement : MonoBehaviour
{
    [SerializeField] GameObject female;
    [SerializeField] float moveDistance = 3f; // cuántos metros avanzará
    [SerializeField] float moveSpeed = 2f;    // velocidad de movimiento

    private Animator animator;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private Vector2 targetPosition;

    private void Start()
    {
        animator = female.GetComponent<Animator>();
        rb = female.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isMoving)
        {
            // activar animación
            animator.SetBool("Walking", true);

            // definir destino
            targetPosition = new Vector2(female.transform.position.x + moveDistance, female.transform.position.y);
            // iniciar movimiento
            StartCoroutine(MoveToTarget());

        }
    }

    private IEnumerator MoveToTarget()
    {
        isMoving = true;

        while (Vector2.Distance(female.transform.position, targetPosition) > 0.05f)
        {
            Vector2 newPos = Vector2.MoveTowards(female.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
            yield return null;
        }

        // detener animación
        animator.SetBool("Walking", false);
    }
}
