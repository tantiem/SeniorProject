using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackVisualizer : MonoBehaviour
{
    HitBox hb;
    public GameObject rend;
    Collider2D col;

    private void Awake() {
        hb = GetComponent<HitBox>();
        col = GetComponent<Collider2D>();
    }

    private void Update() {
        if(hb.active)
        {
            rend.SetActive(true);
            rend.transform.localScale = col.bounds.size;
            rend.transform.position = transform.position;
        }
        else
        {
            rend.SetActive(false);
        }
    }
}
