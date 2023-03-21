using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabThrownSword : MonoBehaviour
{
    Rigidbody2D rb;
    public bool isCollide;
    Vector2 lastPos;
    public float fallSpeed;
    float damage;
    BoxCollider2D col;
    RaycastHit2D[] results;
    private void Start() {
        results = new RaycastHit2D[4];
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate() {
        rb.velocity -= Vector2.up * fallSpeed * Time.fixedDeltaTime;
        if(!isCollide)
        {
            transform.right = rb.velocity;
        }
        else
        {
            //slow it down
            rb.velocity /= 4;
            if(rb.velocity.sqrMagnitude < 0.25f)
            {
                //if the velocity is low enough, just stop it
                rb.velocity = Vector2.zero;
                if(Vector2.Distance((Vector2)transform.position,results[0].point) > 0.5f)
                {
                    //if by the time it has stopped, it is too far away from where it hit, move it back to where it hit.
                    transform.position = results[0].point;
                }
            }
        }
        if(!isCollide)
        {
            if(col.Cast(Vector2.zero,results,1f) > 0)
            {
                if(results[0].transform.CompareTag("Environment"))
                {
                    isCollide = true;
                }
                else if(results[0].transform.CompareTag("Player"))
                {
                    isCollide = true;
                    PlayerController hitPlayer = results[0].transform.GetComponentInChildren<PlayerController>();
                    hitPlayer.Damage(damage);
                }
            }
        }
    }

    public void SetParameters(Vector2 velocity, float damage)
    {
        rb = GetComponent<Rigidbody2D>();
        this.damage = damage;
        rb.velocity = velocity;
    }

    public void RemoveFromPlay()
    {
        Destroy(this.gameObject);
    }

    

}
