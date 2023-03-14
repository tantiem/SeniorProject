using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashThrowSwordLifetime : MonoBehaviour
{
    Rigidbody2D rb;
    float damage;

    public void SetParameters(Vector2 velocity,float damage)
    {
        rb.velocity = velocity;
        this.damage = damage;
    }
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        SetParameters(new Vector2(-50,25),50);
    }
}
