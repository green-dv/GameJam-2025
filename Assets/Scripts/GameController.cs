using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public PlayerHealth playerhealth;

    private bool isGameOver = false;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerhealth == null || isGameOver)
        {
            return;
        }
        if (playerhealth.health <= 0)
        {
            GameOver();
        }
    }
    public void GameOver()
    {
        if(isGameOver)
        {
           return ;
        }

        isGameOver = true;
        SceneManager.LoadScene("GameOver");
    }
}
