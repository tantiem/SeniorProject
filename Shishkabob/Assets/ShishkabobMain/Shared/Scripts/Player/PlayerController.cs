using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerState))]
[RequireComponent(typeof(PlayerMover))]
///<summary>
///Handle functions control all input for actions that have multiple possible states. 
///Each slash or stab based attack is built to have a fakeout, being that
///holding the action is not enough to perform it, but letting go.
///When standing, holding for long enough initiates the throw
///when ducking, holding the sword back is a fake out, and letting go performs the action
///</summary>
public class PlayerController : MonoBehaviour
{
    public GameObject player;
    PlayerState state;
    PlayerMover mover;
    PlayerFightingInterface fighter;
    PlayerInputInterface inputInterface;

    public PlayerControllerDataSO data;

    public Vector2 aim;

    ///action variables
    bool justJumped;


    private void Awake() 
    {
        state = GetComponent<PlayerState>();
        mover = GetComponent<PlayerMover>();
        fighter = GetComponent<PlayerFightingInterface>();
        mover.Inititalize(player);
        
        inputInterface = player.GetComponent<PlayerInputInterface>();

        SetUpCallbacks();
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
            else
            {
                Walk(stick.x);
            }
        }
        //if player is in a ducking state, check if we should rise them up from their ducking state
        if(state.GetState() == PlayerState.State.Ducking)
        {
            //realistically, should already be grounded, but we are going to double check.
            if(state.IsGrounded())
            {
                if(stick.y > data.crouchThreshold)
                {
                    InitiateGetUp();
                }
            }
        }
    }

    void Walk(float inputX)
    {
        //state should have already been checked.
        mover.SetAddititveHorizontalAirWalkVelocity(inputX * data.airWalkSpeed * data.airWalkSpeedMult, data.airWalkSpeed);
        mover.SetHorizontalWalkVelocity(inputX * data.walkSpeed * data.walkAccelMult, Mathf.Abs(inputX)*data.walkSpeed);
    }

    void SetAim(Vector2 stick)
    {
        //no state checks required, the aim should be an always tracked thing.
        aim = stick;
    }

    void HandleSlashActions(InputAction.CallbackContext context)
    {
        if(state.GetState() == PlayerState.State.Blocking || state.GetState() == PlayerState.State.Stunned)
        {
            //early return, this should never be allowed on attacks. Or really anything.
            return;
        }
        else if(state.GetState() == PlayerState.State.Active)
        {
            //slash actions here
            //does not matter if we are grounded or not.
            if(context.started)
            {
                InitiateSlashAction();
            }
            else if(context.performed)
            {
                //this should be the throw action performed
                SlashThrow();
            }
            else if(context.canceled)
            {
                Slash();
            }
        }
        else if(state.GetState() == PlayerState.State.Ducking)
        {
            //low slash action here
            //if we are ducking , we should be grounded. But just in case:
            if(state.IsGrounded())
            {
                if(context.started)
                {
                    InitiateLowSlash();
                }
                else if(context.canceled || context.performed)
                {
                    LowSlash();
                }
            }
        }
    }

    void HandleStabActions(InputAction.CallbackContext context)
    {
        if(state.GetState() == PlayerState.State.Blocking || state.GetState() == PlayerState.State.Stunned)
        {
            //early return, this should never be allowed on attacks. Or really anything.
            return;
        }
        else if(state.GetState() == PlayerState.State.Active)
        {
            //stab actions here
            //does not matter if we are grounded or not.
            if(context.started)
            {
                InitiateStabAction();
            }
            else if(context.performed)
            {
                StabThrow();
            }
            else if(context.canceled)
            {
                Stab();
            }
        }
        else if(state.GetState() == PlayerState.State.Ducking)
        {
            //low slash action here
            //if we are ducking , we should be grounded. But just in case:
            if(state.IsGrounded())
            {
                if(context.started)
                {
                    InitiateLowStab();
                }
                else if(context.canceled || context.performed)
                {
                    LowStab();
                }
            }
        }
    }

    void HandleJump(InputAction.CallbackContext context)
    {
        //Valid states to jump from:
        //Active,Ducking- if grounded

        //if action is performed / started, start jumping if valid
        //if action is canceled, and the time since the last jump start is less than the time it takes
        //to complete a full jump, do an early stop on the jump height.
        if(state.IsGrounded())
        {
            //if we are grounded, now to check if we are active or ducking. Both are valid.
            if(state.GetState() == PlayerState.State.Active || state.GetState() == PlayerState.State.Ducking)
            {
                if(context.performed)
                {
                    JumpStart(data.jumpSpeed);
                    //set just jumped true so the player can early cancel their jump
                    justJumped = true;
                }
            }
        }
        else
        {
            if(state.GetState() == PlayerState.State.Active)
            {
                if(context.canceled)
                {
                    //Just jumped is a variable specifically for the early jump cancel
                    if(justJumped)
                    {
                        justJumped = false;
                        JumpStop(data.jumpEarlyStopSlowdownMult);
                    }
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////
    ///All second hand action responses//////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////
    void InitiateDuck()
    {
        //Set state to ducking
        if(state.IsGrounded())
        {
            mover.SetHorizontalInputVelocity(0);
            state.SetState(PlayerState.State.Ducking);
        }
        
    }

    void InitiateSlashAction()
    {
        //the cock back of a slash, either leading to a slash throw or a slash.
        //valid to be done from Active- grounded or not
        Debug.Log("InitiateSlashAction");
    }
    void InitiateStabAction()
    {
        //the cock back of a slash, either leading to a stab throw or a stab.
        //valid to be done from Active- grounded or not
        Debug.Log("InitiateStabAction");
    }
    void InitiateGetUp()
    {
        //Found out this is necessary lol, basically change state to active from some other state.
        if(state.GetState() == PlayerState.State.Ducking)
        {
            //trigger the correct transition animation maybe?
            state.SetState(PlayerState.State.Active);
        }
    }
    void SlashThrow()
    {
        //should already have validated state.
        Debug.Log("SlashThrow");
    }
    void Slash()
    {
        //should already have validated state.
        Debug.Log("Slash");
        fighter.GenerateSlash(aim,0f);
    }
    void Stab()
    {
        Debug.Log("Stab");
    }
    void StabThrow()
    {
        //should already have validated state.
        Debug.Log("StabThrow");
    }

    void InitiateLowStab()
    {
        Debug.Log("InitiateLowStab");
        //Valid states to low stab from:
        //Ducking - grounded (ducking should only be available when grounded)
    }
    void InitiateLowSlash()
    {
        Debug.Log("InitiateLowSlash");
        //Valid states to low slash from:
        //Ducking - grounded (ducking should only be available when grounded)
    }
    void LowStab()
    {
        Debug.Log("LowStab");
    }
    void LowSlash()
    {
        Debug.Log("LowSlash");
    }
    /////////////////////////////////////////////////////////////////////////
    /////////////Simple one off actions, no intermediary handler/////////////
    /////////////////////////////////////////////////////////////////////////
    void BlockStart()
    {
        Debug.Log("BlockStart");
        //Valid states to block from:
        //Active- grounded or not
    }

    void Taunt()
    {
        Debug.Log("Taunt");
        //Valid states to taunt from:
        //Not blocking or stunned, grounded or not.
    }
    
    void JumpStart(float jumpSpeed)
    {
        mover.SetInstantVelocityY(jumpSpeed);
    }
    void JumpStop(float jumpCancelMultiplier)
    {
        mover.EarlyJumpCancelSlowDown(jumpCancelMultiplier);
    }
    void Kick()
    {
        Debug.Log("Kick");
        //Valid states to kick from:
        //Active -grounded or not
    }
    void Accelerate()
    {
        //WIP, but probably from any state that isnt blocking or stunned.
        if(state.GetState() != PlayerState.State.Blocking || state.GetState() != PlayerState.State.Stunned)
        {
            mover.AddVelocity(aim * data.accelerateActionMultiplier);
        }
    }


}
