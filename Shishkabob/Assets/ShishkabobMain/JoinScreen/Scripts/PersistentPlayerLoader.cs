using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


public class PersistentPlayerLoader : MonoBehaviour
{
    public class PersistentPlayerData
    {
        public int winCount;
        public Color color;
        public PlayerInput pi;
        public InputDevice device;


        public PersistentPlayerData(int wins, Color color, PlayerInput pi,InputDevice device)
        {
            this.winCount = wins;
            this.color = color;
            this.pi = pi;
            this.device = device;
        }
    }
    public UnityEvent onDoneLoading;

    public List<PersistentPlayerData> playerData;


    public int numPlayers = 0;
    public GameObject playerPrefab;

    private void Awake() {
        DontDestroyOnLoad(this);
        playerData = new List<PersistentPlayerData>();
        onDoneLoading?.Invoke();
    }

    public void IncreasePlayerCount(Color color,PlayerInput pi)
    {
        Debug.Log("called",this);
        numPlayers++;
        
        Gamepad gp = pi.GetDevice<Gamepad>();
        Keyboard kb = pi.GetDevice<Keyboard>();

        if(gp != null)
        {
            playerData.Add(new PersistentPlayerData(0,color,pi,gp.device));
            Debug.Log(playerData[numPlayers-1].device);
        }
        else
        {
            playerData.Add(new PersistentPlayerData(0,color,pi,kb.device));
            Debug.Log(playerData[numPlayers-1].device);
        }
        
    }


}
