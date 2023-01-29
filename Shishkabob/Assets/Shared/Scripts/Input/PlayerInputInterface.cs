using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputInterface : MonoBehaviour
{
    public delegate void ActionHandler();
    public delegate void ActionAxisHandler(Vector2 axis);
    public event ActionAxisHandler onMoveAimAxisUpdated;

    public event ActionHandler onSlashCocked;
    public event ActionHandler onSlashThrow;
    public event ActionHandler onSlash;

    public event ActionHandler onStabCocked;
    public event ActionHandler onStabThrow;
    public event ActionHandler onStab;

    public event ActionHandler onKick;
    public event ActionHandler onTaunt;
    public event ActionHandler onJumpStart;
    public event ActionHandler onJumpStop;

    public event ActionHandler onStartBlock;
    public event ActionHandler onStopBlock;

    public event ActionHandler onAccelerate;
    
    ///<summary>
    ///Directly calls the onmoveAimAxisUpdated event with the value of the input joystick.
    ///</summary>
    public void MoveAndAim(InputAction.CallbackContext context)
    {
        onMoveAimAxisUpdated?.Invoke(context.ReadValue<Vector2>());
    }

    public void Slash(InputAction.CallbackContext context)
    {
        //might be useless
    }
    public void SlashThrow(InputAction.CallbackContext context)
    {
        //On started: cock back
        //On performed: slash throw
        //On canceled: regular slash
        switch(context.phase)
        {
            case InputActionPhase.Started:
            {
                onSlashCocked?.Invoke();
                break;
            }
            case InputActionPhase.Performed:
            {
                onSlashThrow?.Invoke();
                break;
            }
            case InputActionPhase.Canceled:
            {
                onSlash?.Invoke();
                break;
            }
            default:
            {
                Debug.LogWarning("Unkown action status on Slash/Throw", this);
                break;
            }
        }

    }
    public void Stab(InputAction.CallbackContext context)
    {
        //might be useless
    }
    public void StabThrow(InputAction.CallbackContext context)
    {
        switch(context.phase)
        {
            case InputActionPhase.Started:
            {
                onSlashCocked?.Invoke();
                break;
            }
            case InputActionPhase.Performed:
            {
                onStabThrow?.Invoke();
                break;
            }
            case InputActionPhase.Canceled:
            {
                onStab?.Invoke();
                break;
            }
            default:
            {
                Debug.LogWarning("Unkown action status on Stab/Throw", this);
                break;
            }
        }
    }
    public void Kick(InputAction.CallbackContext context)
    {
        
        if(context.performed)
        {
            onKick?.Invoke();
        }
    }
    public void Block(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            onStopBlock?.Invoke();
        }
        else if(context.performed)
        {
            onStartBlock?.Invoke();
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if(context.canceled)
        {
            onJumpStop?.Invoke();
        }
        else if(context.performed)
        {
            onJumpStart?.Invoke();
        }

    }
    public void Taunt(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            onTaunt?.Invoke();
        }
    }

    public void Accelerate(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            onAccelerate?.Invoke();
        }
    }
}
