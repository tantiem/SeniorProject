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

    public Vector3 GetNewSpawnPoint()
    {
        int randomIndex = Random.Range(0,spawns.Count);
        return spawns[randomIndex].position;
    }

    public void RespawnPlayer(PlayerController pc)
    {
        if(pc.lives > 0)
        {
            StartCoroutine(RespawnTime(3f,pc));
        }
        CheckIfWin();
    }

    IEnumerator RespawnTime(float seconds,PlayerController pc)
    {
        yield return new WaitForSeconds(seconds);
        pc.Revive(GetNewSpawnPoint());
    }

    void CheckIfWin()
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
        if(weHaveAWinner)
        {
            Debug.Log("a Winner is u");
            winText.text = $"P{winnerIndex} WINS!\n y'all rest be lackin";
            StartCoroutine(WinTime(5f));
        }
    }

    IEnumerator WinTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        onWinner?.Invoke();
    }

}
