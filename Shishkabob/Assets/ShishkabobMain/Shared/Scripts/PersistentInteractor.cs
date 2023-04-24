using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PersistentInteractor : MonoBehaviour
{
    public PersistentPlayerLoader persistent;
    MapLoader loader;
    GameObject pGo;
    private void Awake() {
        pGo = GameObject.FindGameObjectWithTag("PlayerSpawnInfo");
        persistent = pGo.GetComponent<PersistentPlayerLoader>();
        loader = pGo.GetComponent<MapLoader>();

    }

    public void GoToNextMap()
    {
        loader.TransitionToNextMap();
    }

    public void OnPlayerAdded(Color color, PlayerInput pi)
    {
        persistent.IncreasePlayerCount(color,pi);
    }

    public void AddWin(int i)
    {
        persistent.playerData[i].winCount++;
    }

    public void GetScores(out int p1, out int p2, out int p3, out int p4)
    {
        p1 = 0;
        p2 = 0;
        p3 = 0;
        p4 = 0;
        if(persistent.playerData.Count > 0)
        {p1 = persistent.playerData[0].winCount;}
        if(persistent.playerData.Count > 1)
        {p2 = persistent.playerData[1].winCount;}
        if(persistent.playerData.Count > 2)
        {p3 = persistent.playerData[2].winCount;}
        if(persistent.playerData.Count > 3)
        {p4 = persistent.playerData[3].winCount;}
    }
    public void ResetScores()
    {
        persistent.ResetScores();
    }

    public void GoHome()
    {
        loader.GoHome();
    }
    public void GoToScoreScreen()
    {
        loader.GoToScoreScreen();
    }
}
