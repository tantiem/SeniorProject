using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UIPlayerController : MonoBehaviour
{
    public bool leader;

    public enum CurrentUIState {PlayerJoin,MapChoice}
    public CurrentUIState currentUIState;
    public UIStateManager uiManager;

    private void Awake() {
        currentUIState = CurrentUIState.PlayerJoin;
        uiManager = GameObject.Find("Canvas").GetComponent<UIStateManager>();
    }
    public void Submit(InputAction.CallbackContext context)
    {
        if(leader)
        {
            if(currentUIState == CurrentUIState.PlayerJoin)
            {
                uiManager.SwitchPositions();
            }
        }
    }
}
