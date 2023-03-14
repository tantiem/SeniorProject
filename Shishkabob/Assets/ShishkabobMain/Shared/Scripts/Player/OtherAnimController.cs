using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OtherAnimController : MonoBehaviour
{
    public PlayerController controller;
    SpriteRenderer sRenderer;
    Animator animator;
    public CustomPlayerRigidbody rb;

    bool blocking,active,ducking,stunned,grounded;

    private void Awake() {
        sRenderer = GetComponent<SpriteRenderer>();
        controller.onFacedDirChange += OnDirChange;
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate() {
        animator.SetFloat("SpeedX",Mathf.Abs(rb.velocity.x));
        animator.SetFloat("SpeedY",rb.velocity.y);
        
    }
    void SetAllStateFalse()
    {
        blocking = false;
        active = false;
        ducking = false;
        stunned = false;
    }
    void SetAllStateToCurrent()
    {
        animator.SetBool("StateActive",active);
        animator.SetBool("StateBlocking",blocking);
        animator.SetBool("StateStunned",stunned);
        animator.SetBool("StateDucking",ducking);
    }

    public void SetInputX(float x)
    {
        animator.SetFloat("InputX", Mathf.Abs(x));
    }

    void OnDirChange(PlayerController.AimDirection dir)
    {
        if(dir == PlayerController.AimDirection.East)
        {
            sRenderer.flipX = false;
        }
        else
        {
            sRenderer.flipX = true;
        }
    }

    public void OnStateChange(PlayerState.State state)
    {
        SetAllStateFalse();
        switch(state)
        {
            case PlayerState.State.Active:
            {
                active = true;
                break;
            }
            case PlayerState.State.Blocking:
            {
                blocking = true;
                break;
            }
            case PlayerState.State.Ducking:
            {
                ducking = true;
                break;
            }
            default:
            {
                stunned = true;
                break;
            }
        }
        SetAllStateToCurrent();
    }
    public void OnGroundedChanged(bool b)
    {
        animator.SetBool("Grounded",b);
    }
}
