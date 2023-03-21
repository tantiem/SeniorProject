using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashThrowSwordLifetime : MonoBehaviour
{
    [SerializeField]
    PlayerController owner;
    Rigidbody2D rb;
    float damage;

    public GameObject thrownSwordPrefab;

    public int bounceCount  = 0;

    public void SetParameters(Vector2 velocity,float damage,PlayerController owner)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = velocity;
        this.damage = damage;
        this.owner = owner;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        bounceCount++;
        if(other.transform.CompareTag("Player"))
        {
            PlayerController hitPlayer = other.transform.GetComponentInChildren<PlayerController>();
            bool hitOwner = hitPlayer.gameObject == owner.gameObject;
            if(!hitOwner || bounceCount > 1)
            {
                Debug.Log($"this craz happened, bounce number {bounceCount}");
                hitPlayer.Damage(damage);
            }
        }

        if(bounceCount >= 2)
        {
            Vector3 spawnPosition = transform.position + (Vector3)rb.velocity.normalized;
            GameObject stabThrownSword = Instantiate(thrownSwordPrefab,spawnPosition,Quaternion.identity) as GameObject;
            StabThrownSword fallingSword = stabThrownSword.GetComponent<StabThrownSword>();
            fallingSword.SetParameters(rb.velocity,100);
            Destroy(this.gameObject);
        }
    }
    
}
