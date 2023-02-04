using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    BoxCollider2D col;
    float damage;
    public bool active = false;
    ///This position is relative to the transform
    Vector2 offset;

    private void Awake() {
        col = GetComponent<BoxCollider2D>();
    }
    private void FixedUpdate() {
        transform.localPosition = offset;
    }
    ///<summary>
    ///The public method for hitbox. Generate a hit box with dimensions, that does damage, at offset, for lifetime.
    ///</summary>
    public void Generate(Vector2 dimensions, float damage, Vector2 offset, float lifetime, Vector2 rightAlign)
    {
        SetDimensions(dimensions);
        SetDamage(damage);
        SetDirection(rightAlign);
        Spawn(lifetime,offset);
    }
    
    void SetDimensions(Vector2 dimensions)
    {
        col.size = dimensions;
    }
    void SetDamage(float damage)
    {
        this.damage = damage;
    }

    void SetDirection(Vector2 right)
    {
        transform.right = right;
    }

    ///<summary>
    ///Set offset of hitbox and spawn it for lifetime seconds
    ///</summary>
    void Spawn(float lifetime, Vector2 offset)
    {
        if(!active)
        {
            this.offset = offset;
            active = true;
            StartCoroutine(Evaporate(lifetime));
        }
        else
        {
            Debug.LogWarning("Trying to spawn an already active hitbox.",this);
        }
    }


    IEnumerator Evaporate(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        active = false;
    }

    void OnDrawGizmos()
    {
        if(active)
        {
            // Draw a semitransparent red cube at the transforms position
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawCube(transform.position, col.size);
        }
    }
}
