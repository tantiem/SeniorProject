using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    Rigidbody2D rb;

    public void Inititalize(GameObject player)
    {
        rb = player.GetComponent<Rigidbody2D>();
    }
    private void Update() {
        rb.velocity = Vector2.up;
    }
}
