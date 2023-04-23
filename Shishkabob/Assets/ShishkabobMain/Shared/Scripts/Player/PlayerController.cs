using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

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
    public delegate void DirectionChange(AimDirection dir);
    public event DirectionChange onFacedDirChange;

    public delegate void AttackEvent();
    public event AttackEvent onWindUp,onThrow,onStab,onSlash,onUpStab,onDownStab,onUpSlash,onDownSlash,onLowSlash,onLowStab;

    public enum AimDirection {North, South, East, West, None};
    public GameObject player;
    public GameObject deadPlayer;
    public GameObject visuals;
    public GameObject stabThrow,slashThrow;
    PlayerState state;
    PlayerMover mover;
    PlayerFightingInterface fighter;
    PlayerInputInterface inputInterface;

    public PlayerControllerDataSO data;

    public Vector2 aim;
    AimDirection curCardinalAim;
    AimDirection facedDirection;

    PlayerInput pi;

    ///action variables
    /// 
    /// 
    bool justJumped;
    bool wantToDuck;
    IEnumerator blockStop;
    ///stats
    /// 
    /// 
    [SerializeField]
    float health;
    [SerializeField]
    int swordCount;
    bool hasSword;
    float stamina;
    public int lives;

    /// event broadcast
    /// 
    /// 

    public UnityEvent<float> onBroadcastInputX;
    public UnityEvent<PlayerController> onDeath;

//init shiz
    public void SetColor(Color color)
    {
        //set color visual
        visuals.GetComponentInChildren<SpriteRenderer>().color = color;
    }
    public void SetInputDevice(InputDevice device)
    {
        pi.SwitchCurrentControlScheme(device);
    }
    public void Spawn(Vector3 pos)
    {
        player.transform.position = pos;
    }
    //

    private void Awake() 
    {
        pi = GetComponentInParent<PlayerInput>();

        deadPlayer.SetActive(false);
        health = 100;
        swordCount = 3;
        lives = 1;
        curCardinalAim = AimDirection.East;
        //faced direction can never be anything but east or west.
        facedDirection = AimDirection.East;
        state = GetComponent<PlayerState>();
        mover = GetComponent<PlayerMover>();
        fighter = GetComponent<PlayerFightingInterface>();
        mover.Inititalize(player);
        
        inputInterface = player.GetComponent<PlayerInputInterface>();

        SetUpCallbacks();
    }

    private void Update() 
    {
        if(stamina < 1)
        {
            stamina += data.staminaRechargeRate * Time.deltaTime;  
        }  
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

    public void StateChangeModifications(PlayerState.State newState)
    {
        if(newState == PlayerState.State.Ducking)
        {
            mover.SetFrictionScaleToPercentDefault(data.duckingFrictionPercent);
        }
        if(newState != PlayerState.State.Ducking)
        {
            mover.ResetFrictionScale();
        }
    }
    public void GroundedChangeModifications(bool grounded)
    {
        if(grounded)
        {
            if(wantToDuck)
            {
                if(state.GetState() == PlayerState.State.Active)
                {
                    InitiateDuck();
                    wantToDuck = false;
                }
            }
        }
        //if not grounded while ducking, unduck.
        else if(!grounded)
        {
            if(state.GetState() == PlayerState.State.Ducking)
            {
                state.SetState(PlayerState.State.Active);
                wantToDuck = true;
            }
        }
    }

    AimDirection GetCardinalAimDirection(Vector2 aim)
    {
        if(aim.x >= .5f) {return AimDirection.East;}
        else if(aim.x <= -.5f) {return AimDirection.West;}
        else if(aim.y >= .5f) {return AimDirection.North;}
        else if(aim.y <= -.5f) {return AimDirection.South;}
        else
        {
            return AimDirection.None;
        }
    }

    Vector2 AimDirectionToOffset(AimDirection aimDir)
    {
        switch(aimDir)
        {
            case AimDirection.North:
            {
                return Vector2.up;
            }
            case AimDirection.South:
            {
                return Vector2.down;
            }
            case AimDirection.East:
            {
                return Vector2.right;
            }
            case AimDirection.West:
            {
                return Vector2.left;
            }
            default:
            {
                return AimDirectionToOffset(facedDirection);
            }
        }
    }

    void HandleMoveAimVector(Vector2 stick)
    {
        onBroadcastInputX?.Invoke(stick.x);//for animators
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
                if(stick.y < data.crouchThreshold)
                {
                    wantToDuck = true;
                }
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
            else
            {
                if(stick.y > data.crouchThreshold)
                {
                    wantToDuck = false;
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
        AimDirection newAim = GetCardinalAimDirection(aim);
        //cardinal aim for simplified attacks
        if(newAim != AimDirection.None)
        {
            //if actively aiming, set aim
            curCardinalAim = newAim;
        }
        if(newAim == AimDirection.East || newAim == AimDirection.West)
        {
            //if we ever aim left or right, set our player faced direciton
            facedDirection = newAim;
            onFacedDirChange?.Invoke(facedDirection);
        }
        if(newAim == AimDirection.None)
        {
            //if no aiming is going on, then our current aim for attacks will be wherever we are facing
            curCardinalAim = facedDirection;
        }
        
    }

    void HandleSlashActions(InputAction.CallbackContext context)
    {
        if(swordCount > 0)
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
    }

    void HandleStabActions(InputAction.CallbackContext context)
    {
        if(swordCount > 0)
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
        onWindUp?.Invoke();
        //the cock back of a slash, either leading to a slash throw or a slash.
        //valid to be done from Active- grounded or not
        Debug.Log("InitiateSlashAction");
    }
    void InitiateStabAction()
    {
        onWindUp?.Invoke();
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
        onThrow?.Invoke();

        //should already have validated state.
        Vector3 spawnPosition = transform.position + (Vector3)GetCurrentAim(aim) * data.throwOffsetMult;
        GameObject thrown = Instantiate(slashThrow,spawnPosition,Quaternion.identity) as GameObject;
        Vector2 speed = GetThrowVelocity(mover.GetVelocity(),data.baseThrowSpeed,aim);

        thrown.GetComponent<SlashThrowSwordLifetime>().SetParameters(speed,50,this);
        ReplaceSword();
        Debug.Log("SlashThrow");
    }
    void Slash()
    {
        

        //should already have validated state.
        Debug.Log("Slash");
        Vector2 offset = AimDirectionToOffset(curCardinalAim);
        if(fighter.GenerateSlash(offset,offset,offset + mover.GetVelocity()))
        {
            if(offset.y > 0)
            {
                onUpSlash?.Invoke();
            }
            else if(offset.y < 0)
            {
                onDownSlash?.Invoke();
            }
            else
            {
                onSlash?.Invoke();
            }
        }
    }
    void Stab()
    {
        

        Debug.Log("Stab");
        Vector2 offset = AimDirectionToOffset(curCardinalAim);
        if(fighter.GenerateStab(offset,offset,offset + mover.GetVelocity()))
        {
            if(offset.y > 0)
            {
                onUpStab?.Invoke();
            }
            else if(offset.y < 0)
            {
                onDownStab?.Invoke();
            }
            else
            {
                onStab?.Invoke();
            }
        }
    }
    void StabThrow()
    {
        onThrow?.Invoke();

        //should already have validated state.
        Vector3 spawnPosition = transform.position + (Vector3)GetCurrentAim(aim) * data.throwOffsetMult;
        GameObject thrown = Instantiate(stabThrow,spawnPosition,Quaternion.identity) as GameObject;
        Vector2 speed = GetThrowVelocity(mover.GetVelocity(),data.baseThrowSpeed,aim);

        thrown.GetComponent<StabThrownSword>().SetParameters(speed,100);
        ReplaceSword();
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
        Vector2 offset = AimDirectionToOffset(facedDirection);
        if(fighter.GenerateLowStab(offset,offset,offset + mover.GetVelocity()))
        {
            onLowStab?.Invoke();
        }
    }
    void LowSlash()
    {
        Debug.Log("LowSlash");
        Vector2 offset = AimDirectionToOffset(facedDirection);
        if(fighter.GenerateLowSlash(offset,offset,offset + mover.GetVelocity()))
        {
            onLowSlash?.Invoke();
        }

    }
    /////////////////////////////////////////////////////////////////////////
    /////////////Simple one off actions, no intermediary handler/////////////
    /////////////////////////////////////////////////////////////////////////
    void BlockStart()
    {
        if(!IsBlocking())
        {
            Walk(0f); //stop walking if you are
            Debug.Log("BlockStart");
            state.SetState(PlayerState.State.Blocking);
            blockStop = BlockStop();
            StartCoroutine(blockStop);
            //Valid states to block from:
            //Active- grounded or not
        }
    }
    public void BlockSuccess()
    {
        //stop the blockstop routine that would case the block fail to occur.
        StopCoroutine(blockStop);
        state.SetState(PlayerState.State.Active);
    }
    IEnumerator BlockStop()
    {        
        //wait a second to see if block properly, then stun for a second, then return.
        yield return new WaitForSeconds(1.3f);
        state.SetState(PlayerState.State.Stunned);
        yield return new WaitForSeconds(1f);
        state.SetState(PlayerState.State.Active);
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
            if(stamina >= data.accelerateStaminaUse)
            {
                mover.AddVelocity(aim * data.accelerateActionMultiplier);
                stamina -= data.accelerateStaminaUse;
            }
        }
    }
    
    //Damaging and attacks and stuff

    public void SetKnockback(Vector2 knockback)
    {
        mover.SetInstantVelocity(knockback);
    }
    public void Damage(float amt)
    {
        //potentially add an 'attacker' parameter to track who kills you
        health -= amt;
        if(health <= 0)
        {
            Kill();
        }
    }
    public Vector2 GetVelocity()
    {
        return mover.GetVelocity();
    }
    public void OnHitConnect()
    {
        mover.SetInstantVelocity(mover.GetVelocity()/4);
    }
    public bool IsBlocking()
    {
        return state.GetState() == PlayerState.State.Blocking;
    }
    public void Stun(float seconds)
    {
        Walk(0f);
        state.Stun(2f);
    }
    public void Disarm()
    {
        Debug.Log("Disarmed!",this);
        ReplaceSword();
    }
    void Kill()
    {
        GameObject dead = Instantiate(deadPlayer,transform.position,Quaternion.identity) as GameObject;
        dead.SetActive(true);
        Rigidbody2D deadRb = dead.GetComponent<Rigidbody2D>();
        deadRb.velocity = this.GetVelocity();
        onDeath?.Invoke(this);
        DisableSelf();
    }

    public void Revive(Vector2 pos)
    {
        player.transform.position = pos;
        health = 100;
        swordCount = 3;
        lives--;
        EnableSelf();
    }
    void ReplaceSword()
    {
        swordCount--;
        hasSword = false;
        StartCoroutine(GetNewSword(1f));
    }

    IEnumerator GetNewSword(float seconds)
    {
        if(swordCount>0)
        {
            yield return new WaitForSeconds(seconds);
            hasSword = true;
        }
    }

    Vector2 GetThrowVelocity(Vector2 baseVelocity, float speedMult, Vector2 aimDirection)
    {
        aimDirection = GetCurrentAim(aimDirection);
        return baseVelocity + aimDirection * speedMult;
    }
    Vector2 GetCurrentAim(Vector2 aimDirection)
    {
        if(aimDirection.sqrMagnitude < .9)
        {
            aimDirection = AimDirectionToOffset(curCardinalAim);
        }
        return aimDirection;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.transform.CompareTag("ThrownSword"))
        {
            StabThrownSword pickup = other.transform.GetComponent<StabThrownSword>();
            if(pickup.isCollide)
            {
                AddSword(pickup);
            }
        }
    }

    public void AddSword(StabThrownSword swordToPickup)
    {
        swordCount++;
        swordToPickup.RemoveFromPlay();
    }

    void DisableSelf()
    {
        GetComponentInParent<PlayerInput>().DeactivateInput();
        visuals.SetActive(false);
    }

    void EnableSelf()
    {
        GetComponentInParent<PlayerInput>().ActivateInput();
        visuals.SetActive(true);
    }

    

}
