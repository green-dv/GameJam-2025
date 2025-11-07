using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPObject : MonoBehaviour
{
    [SerializeField] float posX;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.position = new Vector2(posX, transform.position.y);
        }
    }
}
