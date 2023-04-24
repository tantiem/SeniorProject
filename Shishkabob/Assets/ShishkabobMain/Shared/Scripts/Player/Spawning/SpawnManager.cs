using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    public List<Transform> spawns;

    public List<PlayerController> players;

    public TMPro.TextMeshProUGUI winText;

    public UnityEvent onWinner;

    public void SpawnPlayers()
    {
        int i  = 0;
        foreach(PlayerController player in players)
        {
            player.Spawn(spawns[i].position);
            player.onDeath.AddListener(CheckIfWin);
            i++;
        }
    }

    void CheckIfWin(PlayerController controller)
    {
        Debug.Log("CheckIfWin");
        bool weHaveAWinner = true;
        int winnerIndex = -1;
        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].lives > 0)
            {
                Debug.Log($"Player {i+1} is still alive");
                if(winnerIndex == -1)
                {
                    winnerIndex = i + 1;
                }
                else
                {
                    weHaveAWinner = false;
                }
            }
        }
        if(winnerIndex == -1)
        {
            winText.text = $"EVERY1 DED";
        }
        else if(weHaveAWinner)
        {
            Debug.Log("a Winner is u");
            winText.text = $"P{winnerIndex} WINS!";
            FindObjectOfType<PersistentInteractor>().AddWin(winnerIndex-1);
            StartCoroutine(WinTime(5f));
        }
    }

    IEnumerator WinTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        onWinner?.Invoke();
    }

}
