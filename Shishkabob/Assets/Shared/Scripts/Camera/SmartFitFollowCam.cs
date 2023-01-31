using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartFitFollowCam : MonoBehaviour
{
    public List<Transform> POIs;
    Camera cam;

    private void Awake() {
        cam = GetComponent<Camera>();
    }
    private void FixedUpdate() 
    {
        cam.transform.position = Vector2Pos(cam.transform.position,POIs[0].position);
    }
    Vector3 Vector2Pos(Vector3 v3pos, Vector3 target)
    {
        return new Vector3(target.x , target.y,v3pos.z);
    }
    
}
