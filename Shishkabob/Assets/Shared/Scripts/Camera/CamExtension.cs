using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CamExtension
{
    public static Bounds GetBounds(this Camera cam)
    {
        return new Bounds(cam.transform.position, new Vector2(cam.GetHalfWidth() * 2, cam.GetHalfHeight() * 2));
    }
    public static float GetHalfWidth(this Camera cam)
    {
        return cam.orthographicSize * Screen.width / Screen.height;
    }
    public static float GetHalfHeight(this Camera cam)
    {
        return cam.orthographicSize;
    }
    
}
