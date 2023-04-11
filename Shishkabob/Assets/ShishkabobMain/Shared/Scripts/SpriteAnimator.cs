using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomPlayerAnimStates {BLOCK,BLOCKFAIL,CROUCH,FALL,HURT,IDLE,JUMP,LAND,PREATTACK,RUN,SLIDE,STAB,SLASH,THROWWEP,WALK}
public class SpriteAnimator : MonoBehaviour
{
    public Animator animator;
    SpriteRenderer sRenderer;
    public PlayerController controller;
    public CustomPlayerRigidbody rb;
    PlayerState.State curState;
    
    [System.Serializable]
    public struct AnimStates
    {
        public string block,blockfail,crouch,fall,hurt,idle,jump,land,preattack,run,slide,stab,slash,throwwep,walk;
        public AnimProperties properties;

    }
    public struct AnimProperties
    {
        public float speedX,speedY;
        public bool grounded,attacking;
        public PlayerState.State playerState;
    }

    [SerializeField]
    public AnimStates animData;

    private void Awake() {
        sRenderer = GetComponent<SpriteRenderer>();
        controller.onFacedDirChange += OnDirChange;
        animator = GetComponent<Animator>();
        animData.properties.grounded = false;
        animData.properties.attacking = false;

        controller.onSlash += OnSlash;
        controller.onStab += OnStab;
        controller.onWindUp += OnPreAttack;
        controller.onThrow += OnThrow;
        controller.onUpStab += OnUpStab;
        controller.onUpSlash += OnUpSlash;
        controller.onDownStab += OnDownStab;
        controller.onDownSlash += OnDownSlash;
    }
    private void FixedUpdate() 
    {
        animData.properties.speedX = Mathf.Abs(rb.velocity.x);
        animData.properties.speedY = rb.velocity.y;
        animator.SetFloat("SpeedX",animData.properties.speedX);
        animator.SetFloat("SpeedY",animData.properties.speedY);

        //if not blocking or stunned, play out procedural
        if(animData.properties.playerState != PlayerState.State.Stunned && animData.properties.playerState != PlayerState.State.Blocking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            //if not any of these states, no longer attacking
            
            if(!animData.properties.attacking)
            {
                if(!animData.properties.grounded)
                {
                    CheckVerticalAirState();
                }
                else
                {
                    CheckHorizontalMoveState();
                }
            }
        }

    }

    void OnDirChange(PlayerController.AimDirection dir)
    {
        if(dir == PlayerController.AimDirection.East)
        {
            sRenderer.flipX = true;
        }
        else
        {
            sRenderer.flipX = false;
        }
    }
    public void OnGroundedChanged(bool b)
    {
        bool oldGrounded = animData.properties.grounded;
        bool newGrounded = b;
        animData.properties.grounded = newGrounded;

        if(!animData.properties.attacking)
        {
            if(oldGrounded == false && newGrounded == true)
            {

                //if just landed
                ChangeAnimStateTo(CustomPlayerAnimStates.LAND);
            }
            else
            {
            
                //if no longer grounded
                CheckVerticalAirState();
            }
        }
    }

    void CheckVerticalAirState()
    {
        if(animData.properties.speedY > 0)
            {
                Debug.Log("JUMP");
                ChangeAnimStateTo(CustomPlayerAnimStates.JUMP);
            }
            else
            {
                Debug.Log("FALL");
                ChangeAnimStateTo(CustomPlayerAnimStates.FALL);
            }
    }

    void CheckHorizontalMoveState()
    {
        float speed = animData.properties.speedX;

        if(animData.properties.playerState == PlayerState.State.Active)
        {
            if(speed > 0.2f && speed < 12f)
            {
                ChangeAnimStateTo(CustomPlayerAnimStates.WALK);
            }
            if(speed > 12f)
            {
                ChangeAnimStateTo(CustomPlayerAnimStates.RUN);
            }
        }
        else if(animData.properties.playerState == PlayerState.State.Ducking)
        {
            if(speed > 1f)
            {
                ChangeAnimStateTo(CustomPlayerAnimStates.SLIDE);
            }
            else
            {
                ChangeAnimStateTo(CustomPlayerAnimStates.CROUCH);
            }
        }
    }

    void OnPreAttack()
    {
        if(!animData.properties.attacking)
        {
            animData.properties.attacking = true;
            ChangeAnimStateTo(CustomPlayerAnimStates.PREATTACK);
        }
    }
    void OnSlash()
    {
        ChangeAnimStateTo(CustomPlayerAnimStates.SLASH);
        float lengthOfState = animator.GetCurrentAnimatorStateInfo(0).length + 0.2f;
        StartCoroutine(AttackingDone(lengthOfState));
    }
    void OnThrow()
    {
        ChangeAnimStateTo(CustomPlayerAnimStates.THROWWEP);
        float lengthOfState = animator.GetCurrentAnimatorStateInfo(0).length + 0.2f;
        StartCoroutine(AttackingDone(lengthOfState));
    }
    void OnStab()
    {
        ChangeAnimStateTo(CustomPlayerAnimStates.STAB);
        float lengthOfState = animator.GetCurrentAnimatorStateInfo(0).length + 0.2f;
        StartCoroutine(AttackingDone(lengthOfState));
    }
    void OnUpStab()
    {

    }
    void OnUpSlash()
    {
        
    }
    void OnDownStab()
    {
        
    }
    void OnDownSlash()
    {
        
    }
    void OnLowStab()
    {
        
    }
    void OnLowSlash()
    {
        
    }

    IEnumerator AttackingDone(float f)
    {
        yield return new WaitForSeconds(f);
        animData.properties.attacking = false;
    }


    public void OnStateChange(PlayerState.State state)
    {
        PlayerState.State oldstate = curState;
        curState = state;

        animData.properties.playerState = curState;
        
        if(curState == PlayerState.State.Blocking)
        {
            ChangeAnimStateTo(CustomPlayerAnimStates.BLOCK);
        }
        if(curState == PlayerState.State.Stunned)
        {
            if(oldstate == PlayerState.State.Blocking)
            {
                ChangeAnimStateTo(CustomPlayerAnimStates.BLOCKFAIL);
            }
        }
    }


    public void ChangeAnimStateTo(CustomPlayerAnimStates newAnimState)
    {
        switch(newAnimState)
        {
            case CustomPlayerAnimStates.BLOCK:
            {
                animator.Play(animData.block);
                break;
            }
            case CustomPlayerAnimStates.BLOCKFAIL:
            {
                animator.Play(animData.blockfail);
                break;
            }
            case CustomPlayerAnimStates.CROUCH:
            {
                animator.Play(animData.crouch);
                break;
            }
            case CustomPlayerAnimStates.FALL:
            {
                animator.Play(animData.fall);
                break;
            }
            case CustomPlayerAnimStates.HURT:
            {
                animator.Play(animData.hurt);
                break;
            }
            case CustomPlayerAnimStates.IDLE:
            {
                animator.Play(animData.idle);
                break;
            }
            case CustomPlayerAnimStates.JUMP:
            {
                animator.Play(animData.jump);
                break;
            }
            case CustomPlayerAnimStates.LAND:
            {
                animator.Play(animData.land);
                break;
            }
            case CustomPlayerAnimStates.PREATTACK:
            {
                animator.Play(animData.preattack);
                break;
            }
            case CustomPlayerAnimStates.RUN:
            {
                animator.Play(animData.run);
                break;
            }
            case CustomPlayerAnimStates.SLIDE:
            {
                animator.Play(animData.slide);
                break;
            }
            case CustomPlayerAnimStates.STAB:
            {
                animator.Play(animData.stab);
                break;
            }
            case CustomPlayerAnimStates.SLASH:
            {
                animator.Play(animData.slash);
                break;
            }
            case CustomPlayerAnimStates.THROWWEP:
            {
                animator.Play(animData.throwwep);
                break;
            }
            case CustomPlayerAnimStates.WALK:
            {
                animator.Play(animData.walk);
                break;
            }
        }
    }
}
