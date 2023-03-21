using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<Transform> spawns;

    public Vector3 GetNewSpawnPoint()
    {
        int randomIndex = Random.Range(0,spawns.Count);
        return spawns[randomIndex].position;
    }
}
