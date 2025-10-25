using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public void Destroy()
    {
        StartCoroutine(DestroyObject());
    }
    IEnumerator DestroyObject()
    {   
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
}
