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

    bool isStab;

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
    public bool Generate(AttackData attackData, Vector2 offset, Vector2 rightAlign,Vector2 speed,bool isStab)
    {
        this.isStab = isStab;
        inheritedVelocity = speed;
        SetDimensions(attackData.attackSize);
        SetDamage(attackData.attackDamage);
        SetDirection(rightAlign);
        return Spawn(attackData.preTime,attackData.attackTime,attackData.postTime,offset);
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
    bool Spawn(float preTime, float lifetime, float postTime, Vector2 offset)
    {
        if(!active && ready)
        {
            this.offset = offset;
            StartCoroutine(AttackSequence(preTime,lifetime,postTime));
            return true;
        }
        else if(active)
        {
            Debug.LogWarning("Trying to spawn an already active hitbox.",this);
            return false;
        }
        else
        {
            Debug.LogWarning("Trying to attack when not ready",this);
            return false;
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
                    
                    if(recipient.tag == "Environment")
                    {
                        OnHitConnect?.Invoke();
                        hitHappened = true;
                        ResolveHitEnvironment();
                    }
                    else if(recipient.tag == "Player")
                    {
                        OnHitConnect?.Invoke();
                        hitHappened = true;
                        ResolveHitPlayer(recipient.gameObject);
                    }
                    else if(recipient.tag == "SwordHitBox")
                    {
                        HitBox otherHitBox = recipient.GetComponent<HitBox>();
                        if(otherHitBox.active)
                        {
                            OnHitConnect?.Invoke();
                            hitHappened = true;
                            ResolveHitSword(recipient.gameObject,recipient,otherHitBox);
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
                if(isStab)
                {
                    //do half damage to the other person if you stab them even though they blocking
                    otherController.Damage(damage/2);
                    otherController.SetKnockback(inheritedVelocity/2);

                }
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
    void ResolveHitSword(GameObject otherSword, Collider2D otherCollider,HitBox otherHitBox)
    {
        if(this.isStab && !otherHitBox.isStab)
        {
            //this means your stab got slashed, so
            //Disarm();
            active = false;
            ownerPlayerController.Disarm();
            

        }
        else if(this.isStab && otherHitBox.isStab || !this.isStab && !otherHitBox.isStab)
        {
            //this means both are equal
            Parry();
            otherHitBox.Parry();
        }
    }

    void Parry()
    {
        //add  audio cue I guess?
        StartCoroutine(CancelAttack(0.75f));
        Debug.Log("Parried!",this);
    }

    

}
