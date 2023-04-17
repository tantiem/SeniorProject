using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class JoinScreenManager : MonoBehaviour
{
    public List<PlayerData> unReadyPlayerBanners;
    public List<PlayerData> readyPlayerBanners;

    public UnityEvent<Color> playerAdded;

    public void OnPlayerJoin(PlayerInput pi)
    {
        PlayerData pd = unReadyPlayerBanners[0];
        unReadyPlayerBanners.RemoveAt(0);

        pd.Activate();
        pd.SetPlayerInput(pi);
        playerAdded?.Invoke(pd.playerColor);
    }
    public void OnPlayerExit(PlayerInput pi)
    {
        
    }
}
