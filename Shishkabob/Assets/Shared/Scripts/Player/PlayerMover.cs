using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    Rigidbody2D rb;
    float horizontalInputVelocity;
    float verticalInputVelocity;

    private void Update() 
    {
        rb.velocity = new Vector2(horizontalInputVelocity,rb.velocity.y);
    }

    public void Inititalize(GameObject player)
    {
        rb = player.GetComponent<Rigidbody2D>();
    }
    
    public void SetHorizontalVelocity(float v)
    {
        horizontalInputVelocity = v;
    }
}
