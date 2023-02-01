using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerState state;
    float horizontalInputVelocity;
    float horizontalInputLimit;
    float additiveHorizontalAirWalkVelocity;
    float additiveHorizontalLimit;


    private void Awake() {
        state = GetComponent<PlayerState>();
    }

    private void Update() 
    {
        //different updates required for different states?
        if(state.IsGrounded())
        {
            AddToCurrentVelocityXClamped(horizontalInputVelocity,horizontalInputLimit);
        }
        else
        {
            //if we are in the air, we can add our velocity from air walking in a sensible way
            AddToCurrentVelocityXClamped(additiveHorizontalAirWalkVelocity,additiveHorizontalLimit);
        }
        
    }

    public void Inititalize(GameObject player)
    {
        rb = player.GetComponent<Rigidbody2D>();
    }
    
    public void SetHorizontalInputVelocity(float v)
    {
        horizontalInputVelocity = v;
    }
    public void SetAddititveHorizontalAirWalkVelocity(float v, float limit)
    {
        additiveHorizontalAirWalkVelocity = v;
        additiveHorizontalLimit = limit;
    }
    public void SetHorizontalWalkVelocity(float v, float limit)
    {
        horizontalInputVelocity = v;
        horizontalInputLimit = limit;
    }
    public void SetInstantVelocityY(float v)
    {
        SetVelocityY(v);
    }
    public void SetInstantVelocityX(float v)
    {
        SetVelocityX(v);
    }
    public void SetInstantVelocity(Vector2 dir)
    {
        SetVelocity(dir);
    }
    public void AddVelocity(Vector2 dir)
    {
        SetVelocity(rb.velocity + dir);
    }
    public void EarlyJumpCancelSlowDown(float slowDownMult)
    {
        if(rb.velocity.y > 0)
        {
            SlowDownVelocityY(slowDownMult);
        }
    }
    ////private
    void SetVelocityX(float v)
    {
        rb.velocity = new Vector2(v, rb.velocity.y);
    }
    void SetVelocityY(float v)
    {
        rb.velocity = new Vector2(rb.velocity.x, v);
    }
    void SetVelocity(Vector2 dir)
    {
        rb.velocity = dir;
    }
    void SlowDownVelocityY(float slowDownMult)
    {
        rb.velocity = new Vector2(rb.velocity.x,rb.velocity.y * slowDownMult);
    }
    void SlowDownVelocityX(float slowDownMult)
    {
        rb.velocity = new Vector2(rb.velocity.x * slowDownMult, rb.velocity.y);
    }
    
    void AddToCurrentVelocityX(float amt)
    {
        SetVelocityX(rb.velocity.x + amt);
    }
    void AddToCurrentVelocityXClamped(float amt, float limit)
    {
        //anything that will add in update needs to be multiplied by deltatime.
        amt *= Time.deltaTime;

        //if we have not reached the max velocity this action allows, and it is in the positive direction, go for it.
        if(rb.velocity.x < limit && amt > 0)
        {
            AddToCurrentVelocityX(amt);
            //if we overstep, set it to our max.
            if(rb.velocity.x > limit)
            {
                SetVelocityX(limit);
            }
        }
        //same thing but left
        if(rb.velocity.x > -limit && amt < 0)
        {
            AddToCurrentVelocityX(amt);
            //if we overstep, set it to our max.
            if(rb.velocity.x < -limit)
            {
                SetVelocityX(-limit);
            }
        }
    }

    
}
