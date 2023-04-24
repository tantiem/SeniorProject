using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 axis;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axis * speed * Time.deltaTime);
    }
}