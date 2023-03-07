using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public SmartFitFollowCam cam;
    public PersistentPlayerLoader loader;
    void Start()
    {
        loader = GameObject.FindGameObjectWithTag("PlayerSpawnInfo").GetComponent<PersistentPlayerLoader>();
        for(int i = 0; i < loader.numPlayers; i++)
        {
            GameObject player = Instantiate(loader.playerPrefab, Vector3.right * i, Quaternion.identity) as GameObject;
            cam.AddPOI(player.transform);
        }
    }
}
