using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 3;
    public bool start = true;

    public SpriteRenderer playerSr;
    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    public void TakeDamage(int amount)
    {
        if (start)
        {
            start = false;
            return;
        }
        health -= amount;
        if(health <= 0)
        {
            health = 0;
            PlayerDeath();
        }
    }

    void PlayerDeath()
    {
        playerSr.enabled= false;
        playerController.enabled= false;

        if(GameController.instance != null)
        {
            GameController.instance.GameOver(); 
            SceneManager.LoadScene("GameOver");
        }

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
    }
}
