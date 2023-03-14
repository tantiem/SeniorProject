using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public struct HitInfo
{

}
public class HitBox : MonoBehaviour
{
    public delegate void OnChanged(float time);
    public event OnChanged onAttackTimeEnd;
    
    [SerializeField]
    GameObject owner;
    PlayerController ownerPlayerController;
    public ContactFilter2D filter;
    BoxCollider2D col;
    float damage;
    Collider2D[] hits;
    ///This position is relative to the transform
    Vector2 offset;
    Vector2 inheritedVelocity;
    public UnityEvent OnHitConnect;

    private bool _ready;
    public bool ready
    {
        get {return _ready;}
        set 
        {
            bool oldValue = _ready;
            _ready = value;
            if(oldValue != value)
            {
                OnReadyChanged();
            }
        }
    }
    private bool _active;

    Coroutine midAttackCoroutine;

    public bool active
    {
        get {return _active;}
        set {
                bool oldValue = _active;
                _active = value; 
                if(oldValue != value)
                {
                    OnActiveChanged();
                }
            }
    }


    private void Awake() {
        ready = true;
        owner = transform.parent.gameObject;
        col = GetComponent<BoxCollider2D>();
        hits = new Collider2D[4];
        ownerPlayerController = owner.GetComponent<PlayerController>();
        //filter.NoFilter();
    }
    private void FixedUpdate() {
        transform.localPosition = offset;
    }

    void OnActiveChanged()
    {
        if(!active)
        {

        }
    }

    void OnReadyChanged()
    {

    }
    ///<summary>
    ///The public method for hitbox. Generate a hit box with dimensions, that does damage, at offset, for lifetime.
    ///</summary>
    public void Generate(AttackData attackData, Vector2 offset, Vector2 rightAlign,Vector2 speed)
    {
        inheritedVelocity = speed;
        SetDimensions(attackData.attackSize);
        SetDamage(attackData.attackDamage);
        SetDirection(rightAlign);
        Spawn(attackData.preTime,attackData.attackTime,attackData.postTime,offset);
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
        Physics2D.SyncTransforms();
        transform.right = right;
    }

    ///<summary>
    ///Set offset of hitbox and spawn it for lifetime seconds
    ///</summary>
    void Spawn(float preTime, float lifetime, float postTime, Vector2 offset)
    {
        if(!active && ready)
        {
            this.offset = offset;
            StartCoroutine(AttackSequence(preTime,lifetime,postTime));
        }
        else if(active)
        {
            Debug.LogWarning("Trying to spawn an already active hitbox.",this);
        }
        else
        {
            Debug.LogWarning("Trying to attack when not ready",this);
        }
    }


    //Attack timing logic: Start the attack after preTime seconds,
    //leave hitbox active for lifetime seconds (or until interrupted)
    //after active is false, call PostAttack
  

    IEnumerator AttackSequence(float preTime, float lifetime, float postTime)
    {
        ready = false;
        yield return new WaitForSeconds(preTime); //preTime charge up
        active = true;

        float curTime = 0f;
        while(curTime < lifetime)
        {
            if(CheckHit())
            {
                active = false;
                break;
            }
            yield return new WaitForFixedUpdate();
            curTime+=Time.fixedDeltaTime;
        }
        
        StartCoroutine(CancelAttack(postTime));

    }

    IEnumerator CancelAttack(float postTime)
    {
        active = false;
        yield return new WaitForSeconds(postTime);
        ready = true;
    }

    bool CheckHit()
    {
        bool hitHappened = false;
        hits = new Collider2D[4];//clear before checking
        Physics2D.SyncTransforms();
        if(col.OverlapCollider(filter,hits) > 0)
        {
            //active = false;
            foreach(Collider2D recipient in hits)
            {
                
                //if the collider array isn't full there will be a null
                //also, if the recipient is yourself, dont do anything
                if(recipient != null && recipient.gameObject != owner )
                {
                    OnHitConnect?.Invoke();
                    hitHappened = true;
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
                            ResolveHitSword(recipient.gameObject,recipient);
                        }
                    }
                }
                
            }
        }
        return hitHappened;

    }

    void ResolveHitEnvironment()
    {
        //backlog for now
    }
    void ResolveHitPlayer(GameObject otherPlayer)
    {
        
        PlayerController otherController;
        bool valid = otherPlayer.TryGetComponent<PlayerController>(out otherController);

        if(valid)
        {
            if(otherController.IsBlocking())
            {
                ownerPlayerController.Stun(2f);
                otherController.BlockSuccess();
            }
            else
            {
                otherController.Damage(damage);
                Debug.Log($"Inherited velocity: {inheritedVelocity}");
                otherController.SetKnockback(inheritedVelocity);
            }
            
        }
        else
        {
            Debug.LogError($"{this.gameObject} encountered an error trying to get PlayerController from {otherPlayer}",this);
        }
    }
    void ResolveHitSword(GameObject otherSword, Collider2D otherCollider)
    {
        if(otherCollider.bounds.size.y > col.bounds.size.y)
        {
            //this means your stab got slashed, so
            //Disarm();
        }
        else if(otherCollider.bounds.size == col.bounds.size)
        {
            //this means both are equal
            //Parry();
        }
    }

    

}
