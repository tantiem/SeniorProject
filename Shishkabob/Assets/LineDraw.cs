using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDraw : MonoBehaviour
{
    public LineRenderer line;
    public Transform origin,destination;

    private void Awake() {
        line.positionCount = 2;
    }

    private void FixedUpdate() {
        line.SetPosition(0,origin.position);
        line.SetPosition(1,destination.position);
    }
}
