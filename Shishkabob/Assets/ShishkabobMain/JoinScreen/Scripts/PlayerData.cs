using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour
{
    public bool leader;
    public TextMeshProUGUI playerNumText,joinText;

    public Color playerNumActiveColor,lightMeshColor,lightBulbColor,lightColor,chairColor;
    public GameObject inactivePlayer,activePlayer,canLight,lightBulb,lightLight,chair;
    SpriteRenderer canLightRend,lightBulbRend,lightLightRend,chairRend;

    public Color playerColor;
    public PlayerInput pi;

    private void Awake() 
    {
        canLightRend = canLight.GetComponent<SpriteRenderer>();
        lightBulbRend = lightBulb.GetComponent<SpriteRenderer>();
        lightLightRend = lightLight.GetComponent<SpriteRenderer>();
        chairRend = chair.GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        playerNumText.color = playerNumActiveColor;
        joinText.text = "READY!";
        inactivePlayer.SetActive(false);
        activePlayer.SetActive(true);
        canLightRend.color = lightMeshColor;
        lightBulbRend.color = lightBulbColor;
        lightLightRend.color = lightColor;
        chairRend.color = chairColor;
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
