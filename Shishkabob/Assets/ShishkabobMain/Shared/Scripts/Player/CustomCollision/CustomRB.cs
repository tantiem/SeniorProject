using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))] //only required for cast method to work, should be static
public class CustomRB : MonoBehaviour
{
    public float gravity;
    public float drag;
    public float mass = 1f;
    public float frictionScale;
    public Vector2 velocity;

    protected BoxCollider2D col;
    protected RaycastHit2D[] futureColResults;

    protected void Initialize()
    {
        col = GetComponentInChildren<BoxCollider2D>();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        futureColResults = new RaycastHit2D[4];
    }
}
