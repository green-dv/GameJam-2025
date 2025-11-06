using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathController : MonoBehaviour
{
    void start()
    {
        GameObject carrier = GameObject.Find("PlayButton");
        GameObject player = GameObject.Find("Player");
        if (carrier != null)
        {
            player.transform.position = transform.position;
        }
    }
}
