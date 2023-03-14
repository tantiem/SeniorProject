using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabThrownSword : MonoBehaviour
{
    Rigidbody2D rb;
    bool isCollide;
    Vector2 lastPos;
    public float fallSpeed;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(20,20);
    }

    private void FixedUpdate() {
        rb.velocity -= Vector2.up * fallSpeed * Time.fixedDeltaTime;
        if(!isCollide)
        {
            transform.right = rb.velocity;
        }
    }

}
