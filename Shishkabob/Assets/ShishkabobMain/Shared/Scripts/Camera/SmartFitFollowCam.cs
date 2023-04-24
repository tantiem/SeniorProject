using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartFitFollowCam : MonoBehaviour
{
    public List<Transform> POIs;
    Camera cam;
    Bounds camBounds;
    Vector2 centroid;
    public float margin = 2f;
    public float acceleration;
    

    //camera orthographic size is the height

    private void Awake() 
    {
        cam = GetComponent<Camera>();
        RefreshCamBounds();
    }
    private void FixedUpdate() 
    {
        if(POIs.Count > 0)
        {
        centroid = GetCentroid(POIs);
        MoveToCentroid(acceleration,centroid);   
        }
    }
    Vector3 Vector2Pos(Vector3 v3pos, Vector3 target)
    {
        return new Vector3(target.x , target.y,v3pos.z);
    }
    void RefreshCamBounds()
    {
        camBounds = cam.GetBounds();
    }
    Vector2 GetCentroid(List<Transform> points)
    {
        float yxRatio = Screen.height/Screen.width;
        //amount of x pixels per y
        float _blx = GetMinX(points);
        float _bly = GetMinY(points);
        float _trx = GetMaxX(points);
        float _try = GetMaxY(points);
        //make bounding box
        float centerX = _blx + (_trx - _blx) / 2;
        float centerY = _bly + (_try - _bly) / 2;
        //find center
        Vector2 centroid = new Vector2(centerX,centerY);

        //The ortho size needs to be half the distance from bl to tr plus margin
        float diagonalDistance = Mathf.Sqrt((_trx-_blx)*(_trx-_blx) + (_try-_bly)*(_try-_bly));
        cam.orthographicSize = (diagonalDistance / 2f) + margin;
        

        return centroid;
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

    float GetMinX(List<Transform> verts)
    {
        return Min(GetXArrayFromTransformList(verts));
    }
    float GetMinY(List<Transform> verts)
    {
        return Min(GetYArrayFromTransformList(verts));
    }
    float GetMaxX(List<Transform> verts)
    {
        return Max(GetXArrayFromTransformList(verts));
    }
    float GetMaxY(List<Transform> verts)
    {
        return Max(GetYArrayFromTransformList(verts));
    }
    float Min(float[] a)
    {
        float min = a[0];
        for(int i = 1; i < a.Length; i++)
        {
            if(a[i] < min)
            {
                min = a[i];
            }
        }
        return min;
    }
    float Max(float[] a)
    {
        float max = a[0];
        for(int i = 1; i < a.Length; i++)
        {
            if(a[i]>max)
            {
                max = a[i];
            }
        }
        return max;
    }
    float[] GetXArrayFromTransformList(List<Transform> verts)
    {
        float[] a = new float[verts.Count];
        for(int i = 0; i < verts.Count; i++)
        {
            a[i] = verts[i].position.x;
        }
        return a;
    }
    float[] GetYArrayFromTransformList(List<Transform> verts)
    {
        float[] a = new float[verts.Count];
        for(int i = 0; i < verts.Count; i++)
        {
            a[i] = verts[i].position.y;
        }
        return a;
    }
}
