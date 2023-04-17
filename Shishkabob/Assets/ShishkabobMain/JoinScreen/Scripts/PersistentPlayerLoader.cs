using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersistentPlayerLoader : MonoBehaviour
{
    public class PersistentPlayerData
    {
        public int winCount;
        public Color color;

        public PersistentPlayerData(int wins, Color color)
        {
            this.winCount = wins;
            this.color = color;
        }
    }

    public List<PersistentPlayerData> playerData;


    public int numPlayers = 0;
    public GameObject playerPrefab;

    private void Awake() {
        DontDestroyOnLoad(this);
        playerData = new List<PersistentPlayerData>();
    }

    public void IncreasePlayerCount(Color color)
    {
        Debug.Log("called",this);
        numPlayers++;
        playerData.Add(new PersistentPlayerData(0,color));
    }


}
