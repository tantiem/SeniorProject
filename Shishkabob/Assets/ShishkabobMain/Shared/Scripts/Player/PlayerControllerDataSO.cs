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
    public float walkAccelMult = 0.5f;
    public float airWalkSpeed = 0.1f;
    public float airWalkSpeedMult = 0.5f;
    public float jumpSpeed = 5f;
    public float jumpEarlyStopSlowdownMult = 0.4f;
    public float blockLength = 1f;
    public float accelerateActionMultiplier = 1f;
    public float baseThrowSpeed = 35f;
    public float throwOffsetMult = 2f;
    public float staminaRechargeRate = 0.05f;
    public float accelerateStaminaUse = 0.33f;
    public float duckingFrictionPercent = 0.1f;
    
}
