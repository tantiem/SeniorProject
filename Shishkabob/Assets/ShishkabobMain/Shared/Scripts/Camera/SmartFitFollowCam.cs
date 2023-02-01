using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartFitFollowCam : MonoBehaviour
{
    public List<Transform> POIs;
    Camera cam;
    Bounds camBounds;
    Vector2 centroid;
    public float baseOrthoSize = 2f;
    public float acceleration;
    

    //camera orthographic size is the height

    private void Awake() 
    {
        cam = GetComponent<Camera>();
        RefreshCamBounds();
    }
    private void FixedUpdate() 
    {
        float averageDistance = 0f;
        centroid = GetCentroid(POIs, out averageDistance);
        MoveToCentroid(acceleration,centroid);
        cam.orthographicSize = baseOrthoSize + averageDistance;
    }
    Vector3 Vector2Pos(Vector3 v3pos, Vector3 target)
    {
        return new Vector3(target.x , target.y,v3pos.z);
    }
    void RefreshCamBounds()
    {
        camBounds = cam.GetBounds();
    }
    Vector2 GetCentroid(List<Transform> points, out float avgDistance)
    {
        
        int size = points.Count;
        if(size == 1)
        {
            avgDistance = 0;
            return points[0].position;
        }
        else
        {
            //get the average position of all points.
            float sumX = 0;
            float sumY = 0;

            for(int i = 0; i < size; i++)
            {
                sumX += points[i].position.x;
                sumY += points[i].position.y;
            }

            Vector2 centroid = new Vector2(sumX / size, sumY / size);
            avgDistance = ((Vector2)points[1].position - centroid).magnitude;

            return centroid;
        }
    }
    public void AddPOI(Transform poi)
    {
        POIs.Add(poi);
    }
    public void RemovePOI(Transform poi)
    {
        POIs.Remove(poi);
    }
    void MoveToCentroid(float accel,Vector2 pf)
    {
        Vector2 pi = cam.transform.position;
        Vector2 dir = pf - pi;
        
        Vector3 distThisFrame = dir * dir.magnitude * accel * new Vector3(1,1,0);
        if(dir.sqrMagnitude < distThisFrame.sqrMagnitude)
        {
            cam.transform.position = new Vector3(pf.x,pf.y,cam.transform.position.z);
        }
        else
        {
            cam.transform.position = cam.transform.position + distThisFrame;
        }
    }
}
