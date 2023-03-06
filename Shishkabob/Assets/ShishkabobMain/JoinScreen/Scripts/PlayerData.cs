using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour
{
    public bool leader;
    public Image backPanel,frontPanel;
    public TextMeshProUGUI playerNumText,joinText;

    Color backPanelBaseColor,frontPanelBaseColor,playerNumBaseColor;
    Color backPanelActiveColor,frontPanelActiveColor,playerNumActiveColor;
    public PlayerInput pi;

    private void Awake() 
    {
        backPanelBaseColor = backPanel.color;
        frontPanelBaseColor = frontPanel.color;
        playerNumBaseColor = playerNumText.color;

        backPanelActiveColor = new Color(backPanelBaseColor.r,backPanelBaseColor.g,backPanelBaseColor.b,1);
        frontPanelActiveColor = new Color(frontPanelBaseColor.r,frontPanelBaseColor.g,frontPanelBaseColor.b,1);
        playerNumActiveColor = new Color(playerNumBaseColor.r/4,playerNumBaseColor.g*2,playerNumBaseColor.b,1);
    }

    public void Activate()
    {
        backPanel.color = backPanelActiveColor;
        frontPanel.color = frontPanelActiveColor;
        playerNumText.color = playerNumActiveColor;
        joinText.text = "READY!";
    }

    public void SetPlayerInput(PlayerInput pi)
    {
        this.pi = pi;
        if(leader)
        {
            pi.gameObject.GetComponent<UIPlayerController>().leader = true;
        }
    }

}
