using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoader : MonoBehaviour
{
    public Transform loader;
    public float rotationSpeed = 200f; // Degrees per second

    void Update()
    {
        loader.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }
}
