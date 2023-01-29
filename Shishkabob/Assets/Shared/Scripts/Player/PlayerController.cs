using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerState))]
[RequireComponent(typeof(PlayerMover))]
public class PlayerController : MonoBehaviour
{
    public GameObject player;
    PlayerState state;
    PlayerMover mover;
    PlayerInputInterface inputInterface;

    public PlayerControllerDataSO data;

    private void Awake() 
    {
        state = GetComponent<PlayerState>();
        mover = GetComponent<PlayerMover>();
        mover.Inititalize(player);
        
        inputInterface = player.GetComponent<PlayerInputInterface>();
    }

    void SetUpCallbacks()
    {
        //Move / Aim
        inputInterface.onMoveAimAxisUpdated += HandleMoveAimVector;

        //Slash stuff
        inputInterface.onSlashActionUpdated += HandleSlashActions;

        //Stab stuff
        inputInterface.onStabActionUpdated += HandleStabActions;

        //Jump stuff
        inputInterface.onJumpActionUpdated += HandleJump;

        //Other one off actions
        inputInterface.onBlock += BlockStart;
        inputInterface.onKick += Kick;
        inputInterface.onTaunt += Taunt;
        inputInterface.onAccelerate += Accelerate;
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
        //state should have already been checked.
    }

    void AirWalk(float inputX)
    {
        //state should already be checked.
    }

    void SetAim(Vector2 stick)
    {
        //no state checks required, the aim should be an always tracked thing.
    }

    void HandleSlashActions(InputAction.CallbackContext context)
    {

    }

    void HandleStabActions(InputAction.CallbackContext context)
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

    void InitiateSlashAction()
    {
        //the cock back of a slash, either leading to a slash throw or a slash.
        //valid to be done from Active- grounded or not
    }
    void InitiateStabAction()
    {
        //the cock back of a slash, either leading to a stab throw or a stab.
        //valid to be done from Active- grounded or not
    }
    void SlashThrow()
    {
        //should already have validated state.
    }
    void Slash()
    {
        //should already have validated state.
    }
    void StabThrow()
    {
        //should already have validated state.
    }

    void LowStab()
    {
        //Valid states to low stab from:
        //Ducking - grounded (ducking should only be available when grounded)
    }
    void LowSlash()
    {
        //Valid states to low slash from:
        //Ducking - grounded (ducking should only be available when grounded)
    }

    /////////////Simple one off actions, no intermediary handler/////////////
    void BlockStart()
    {
        //Valid states to block from:
        //Active- grounded or not
    }

    void Taunt()
    {
        //Valid states to taunt from:
        //Not blocking or stunned, grounded or not.
    }
    void HandleJump(InputAction.CallbackContext context)
    {
        //Valid states to jump from:
        //Active,Ducking- if grounded

        //if action is performed / started, start jumping if valid
        //if action is canceled, and the time since the last jump start is less than the time it takes
        //to complete a full jump, do an early stop on the jump height.
    }
    void Kick()
    {
        //Valid states to kick from:
        //Active -grounded or not
    }
    void Accelerate()
    {
        //WIP, but probably from any state that isnt blocking or stunned.
    }


}
