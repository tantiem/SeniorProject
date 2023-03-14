using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerState : MonoBehaviour
{
    public enum State {Active,Stunned,Ducking,Blocking};

    State state;
    bool grounded;
    BoxCollider2D col;
    Vector2 defaultBoundSize;
    Vector2 duckingBoundSize;
    Vector2 defaultOffset;
    Vector2 duckingOffset;

    public UnityEvent<State> onStateChanged;
    public UnityEvent<bool> onGroundedChanged;

    private void Start() {
        col = GetComponent<BoxCollider2D>();
        duckingBoundSize = new Vector2(col.size.y,col.size.x);
        defaultBoundSize = col.size;
        defaultOffset = col.offset;
        duckingOffset = new Vector2(0,-duckingBoundSize.y/2);
        state = State.Active;
        grounded = true;
    }

    public void SetGrounded(bool b)
    {
        if(b != grounded){
            grounded = b;
            onGroundedChanged?.Invoke(grounded);
        }
        grounded = b;
        
    }
    public bool IsGrounded()
    {
        return grounded;
    }

    //abstraction layer for state machine
    public void SetState(State s)
    {
        DuckStateColliderChange(state,s);
        if(s != state){
            state = s;
            onStateChanged?.Invoke(state);
        }
        state = s;
        
    }

    public State GetState()
    {
        return state;
    }

    void DuckStateColliderChange(State prev, State cur)
    {
        if(cur == State.Ducking)
        {
            col.size = duckingBoundSize;
            col.offset = duckingOffset;
        }
        else if(cur != State.Ducking && prev == State.Ducking)
        {
            col.size = defaultBoundSize;
            col.offset = defaultOffset;
        }
    }

    public void Stun(float seconds)
    {
        StartCoroutine(StunForSeconds(seconds));
    }

    IEnumerator StunForSeconds(float s)
    {
        SetState(State.Stunned);
        yield return new WaitForSeconds(s);
        SetState(State.Active);
    }

    //State tests to implement:
    //If not grounded, you cant crouch.
    //

    ///BASIC REGROUNDING
    
}
