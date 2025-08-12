using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoader : MonoBehaviour
{
    public float rotationSpeed = 200f; // Degrees per second

    void Update()
    {
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }
}
