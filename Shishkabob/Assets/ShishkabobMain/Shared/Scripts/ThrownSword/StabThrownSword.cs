using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabThrownSword : MonoBehaviour
{
    Rigidbody2D rb;
    bool isCollide;
    Vector2 lastPos;
    public float fallSpeed;
    float damage;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        rb.velocity -= Vector2.up * fallSpeed * Time.fixedDeltaTime;
        if(!isCollide)
        {
            transform.right = rb.velocity;
        }
    }

    public void SetParameters(Vector2 velocity, float damage)
    {
        rb = GetComponent<Rigidbody2D>();
        this.damage = damage;
        rb.velocity = velocity;
    }

}
