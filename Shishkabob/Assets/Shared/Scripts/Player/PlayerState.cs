using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerState : MonoBehaviour
{
    public enum State {Active,Stunned,Ducking,Blocking};

    State state;
    bool grounded;

    public UnityEvent<State> onStateChanged;
    public UnityEvent<bool> onGroundedChanged;

    private void Start() {
        state = State.Active;
        grounded = true;
    }

    public void SetGrounded(bool b)
    {
        if(b != grounded){
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
        if(s != state){
            onStateChanged?.Invoke(state);
        }
        state = s;
        
    }

    public State GetState()
    {
        return state;
    }

    //State tests to implement:
    //If not grounded, you cant crouch.
    //
}
