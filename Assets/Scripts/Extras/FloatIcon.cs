using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatIcon : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start() => startPos = transform.localPosition;

    void Update()
    {
        transform.localPosition = startPos + Vector3.up * Mathf.Sin(Time.time * speed) * amplitude;
    }
}
