using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDebug : MonoBehaviour
{
    public TMPro.TextMeshProUGUI debugText;
    PlayerState.State curState;
        bool curGrounded;
    public void SetCurrentState(PlayerState.State state)
    {
        curState = state;
        SetDebugText();
    }
    public void SetCurrentGrounded(bool b)
    {
        curGrounded = b;
        SetDebugText();
    }

    void SetDebugText()
    {
        
        string stateLine = $"State: {curState}";
        string healthLine = $"Health: ";
        string speedLine = $"Speed: ";
        string groundedLine = $"Grounded: {curGrounded}";

        debugText.text = $"{stateLine}\n{groundedLine}\n{speedLine}\n{healthLine}";
    }
}
