using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public SmartFitFollowCam cam;
    public PersistentInteractor persistentInteractor;
    public SpawnManager spawner;
    void Start()
    {
        PersistentPlayerLoader loader = persistentInteractor.persistent;
        for(int i = 0; i < loader.numPlayers; i++)
        {
            GameObject player = Instantiate(loader.playerPrefab, Vector3.right * i, Quaternion.identity) as GameObject;
            PlayerController controller = player.GetComponentInChildren<PlayerController>();
            spawner.players.Add(controller);
            cam.AddPOI(player.transform);

            controller.SetColor(loader.playerData[i].color);
            controller.SetInputDevice(loader.playerData[i].device);

            Debug.Log(loader.playerData[i].device);

        }

        spawner.SpawnPlayers();
    }


}
