using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputInterface : MonoBehaviour
{
    public delegate void ActionHandler();
    public delegate void ActionAxisHandler(Vector2 axis);
    public delegate void ActionStateHandler(InputAction.CallbackContext context);
    public event ActionAxisHandler onMoveAimAxisUpdated;

    public event ActionStateHandler onSlashActionUpdated;
    public event ActionStateHandler onStabActionUpdated;
    public event ActionStateHandler onJumpActionUpdated;

    public event ActionHandler onKick;
    public event ActionHandler onTaunt;

    public event ActionHandler onBlock;

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
        onSlashActionUpdated?.Invoke(context);

    }
    public void Stab(InputAction.CallbackContext context)
    {
        //might be useless
    }
    public void StabThrow(InputAction.CallbackContext context)
    {
        onStabActionUpdated?.Invoke(context);
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
        if(context.performed)
        {
            onBlock?.Invoke();
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        onJumpActionUpdated?.Invoke(context);

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
