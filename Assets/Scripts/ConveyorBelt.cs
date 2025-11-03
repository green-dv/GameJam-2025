using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private bool moveRight = true;
    [SerializeField] private float speed = 3f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null && collision.CompareTag("Player"))
        {
            float direction = moveRight ? 1f : -1f;
            rb.position += new Vector2(direction * speed * Time.deltaTime, 0f);
        }
    }
}
