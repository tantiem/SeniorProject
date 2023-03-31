using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public SmartFitFollowCam cam;
    public PersistentPlayerLoader loader;
    public SpawnManager spawner;
    void Start()
    {
        loader = GameObject.FindGameObjectWithTag("PlayerSpawnInfo").GetComponent<PersistentPlayerLoader>();
        for(int i = 0; i < loader.numPlayers; i++)
        {
            GameObject player = Instantiate(loader.playerPrefab, Vector3.right * i, Quaternion.identity) as GameObject;
            PlayerController controller = player.GetComponentInChildren<PlayerController>();
            spawner.players.Add(controller);
            cam.AddPOI(player.transform);
        }

        SceneManager.MoveGameObjectToScene(loader.gameObject, SceneManager.GetActiveScene());
    }
}
