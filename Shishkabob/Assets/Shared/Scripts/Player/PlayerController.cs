using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerState))]
public class PlayerController : MonoBehaviour
{
    public GameObject player;
    PlayerState state;
    PlayerInputInterface inputInterface;

    public PlayerControllerDataSO data;

    private void Awake() 
    {
        state = GetComponent<PlayerState>();
        inputInterface = player.GetComponent<PlayerInputInterface>();
    }

    void SetUpCallbacks()
    {
        //Move / Aim
        inputInterface.onMoveAimAxisUpdated += HandleMoveAimVector;
    }

    void HandleMoveAimVector(Vector2 stick)
    {
        //Regardless of state, call SetAim
        SetAim(stick);
        //If you are in the Active state, check if you should duck or walk.
        if(state.GetState() == PlayerState.State.Active)
        {
            //if player is grounded, check if they should walk or crouch
            if(state.IsGrounded())
            {
                //check if the stick is held down enough to crouch
                if(stick.y < data.crouchThreshold)
                {
                    InitiateDuck();
                }
                //Otherwise, you should probably be walking
                else
                {
                   Walk(stick.x);
                }
            }
        }
    }

    void Walk(float inputX)
    {

    }

    void AirWalk(float inputX)
    {
        
    }

    void SetAim(Vector2 stick)
    {

    }

    void InitiateDuck()
    {
        //Set state to ducking
        if(state.IsGrounded())
        {
            state.SetState(PlayerState.State.Ducking);
        }
        
    }

    void InitiateSlash()
    {

    }
    void InitiateStab()
    {

    }
    void InitiateSlashThrow()
    {

    }
    void InitiateStabThrow()
    {

    }
    void InitiateLowStab()
    {

    }
    void InitiateLowSlash()
    {

    }
    void Block()
    {

    }
    void Taunt()
    {

    }
    void Jump()
    {
        //Valid states to jump from:
        //Active,Ducking if grounded
    }
    void Kick()
    {

    }
    void Accelerate()
    {
        
    }


}
