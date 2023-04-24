using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoVisual : MonoBehaviour
{
    public RectTransform healthBar,staminaBar;
    public Image playerIcon;
    public TMPro.TextMeshProUGUI swordText;

    public void OnHealthChanged(float newHealth)
    {
        healthBar.anchoredPosition = new Vector2((newHealth - 100) * 1.5f,0);
        if(newHealth <= 0)
        {
            SetColor(new Color(.7f,.7f,.7f,.7f));
        }
    }
    public void OnStaminaChanged(float newStamina)
    {
        //right offset is 50
        staminaBar.anchoredPosition = new Vector2((newStamina - 1) * 110,0);
        //staminaBar.sizeDelta = new Vector2(newStamina * 100f, staminaBar.rect.height);
    }
    public void OnSwordChanged(int newSword)
    {
        swordText.text = $"x{newSword}";
    }
    public void SetColor(Color color)
    {
        playerIcon.color = color;
    }
}
