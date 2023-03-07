using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersistentPlayerLoader : MonoBehaviour
{
    public int numPlayers = 0;
    public GameObject playerPrefab;

    private void Awake() {
        DontDestroyOnLoad(this);
    }

    public void IncreasePlayerCount()
    {
        numPlayers++;
    }


}
