using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerControllerData", menuName = "Shishkabob/PlayerControllerData", order=1)]
///<summary>
///This is a scriptable object to separate all the constant modifier values that a player controller uses from the
///Script itself to reduce bloat.
///</summary>
public class PlayerControllerDataSO : ScriptableObject
{
    //This is the negative axis input value of the left stick.
    public float crouchThreshold = -0.75f;
    public float walkSpeed = 1f;
    public float jumpSpeed = 5f;
    public float blockLength = 1f;
}
