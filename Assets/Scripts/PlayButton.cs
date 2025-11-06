using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public bool muerto = false;
    public void StartFirstScene()
    {
        SceneManager.LoadScene("DemoScene");
    }
    

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CargarNivel()
    {
        muerto = true;
        SceneManager.LoadScene("Kael-Nivel3");
    }
    public void CargarMenu()
    {
        muerto = false;
        SceneManager.LoadScene("Menu");
    }
}
