using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField]
    GameObject owner;
    public ContactFilter2D filter;
    BoxCollider2D col;
    float damage;
    public bool active = false;
    Collider2D[] hits;
    ///This position is relative to the transform
    Vector2 offset;
    Vector2 inheritedVelocity;
    

    private void Awake() {
        owner = transform.parent.gameObject;
        col = GetComponent<BoxCollider2D>();
        hits = new Collider2D[4];
        filter.NoFilter();
    }
    private void FixedUpdate() {
        transform.localPosition = offset;
        CheckHit();
    }
    ///<summary>
    ///The public method for hitbox. Generate a hit box with dimensions, that does damage, at offset, for lifetime.
    ///</summary>
    public void Generate(Vector2 dimensions, float damage, Vector2 offset, float lifetime, Vector2 rightAlign,Vector2 speed)
    {
        inheritedVelocity = speed;
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

    

    void CheckHit()
    {
        if(active)
        {
            
            if(col.OverlapCollider(filter,hits) > 0)
            {
                
                foreach(Collider2D recipient in hits)
                {
                    
                    //if the collider array isn't full there will be a null
                    //also, if the recipient is yourself, dont do anything
                    if(recipient != null && recipient.gameObject != owner )
                    {
                        
                        if(recipient.tag == "Environment")
                        {
                            ResolveHitEnvironment();
                        }
                        else if(recipient.tag == "Player")
                        {
                            ResolveHitPlayer(recipient.gameObject);
                        }
                        else if(recipient.tag == "SwordHitBox")
                        {
                            if(recipient.GetComponent<HitBox>().active)
                            {
                                ResolveHitSword(recipient.gameObject);
                            }
                        }
                    }
                    
                }
            }
        }
    }

    void ResolveHitEnvironment()
    {

    }
    void ResolveHitPlayer(GameObject otherPlayer)
    {
        active = false;
        PlayerController otherController;
        bool valid = otherPlayer.TryGetComponent<PlayerController>(out otherController);

        if(valid)
        {
            otherController.Damage(damage);
            Debug.Log($"Inherited velocity: {inheritedVelocity}");
            otherController.SetKnockback(inheritedVelocity);
        }
        else
        {
            Debug.LogError($"{this.gameObject} encountered an error trying to get PlayerController from {otherPlayer}",this);
        }
    }
    void ResolveHitSword(GameObject otherSword)
    {

    }

}
