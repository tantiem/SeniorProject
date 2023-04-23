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
}
