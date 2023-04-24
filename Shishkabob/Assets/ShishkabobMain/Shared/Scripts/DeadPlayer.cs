using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPlayer : MonoBehaviour
{
    public void Init(Vector2 inheritedVel)
    {
        foreach(Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())
        {
            rb.velocity = inheritedVel;
        }
        StartCoroutine(Remove(5f));
    }
    IEnumerator Remove(float s)
    {
        yield return new WaitForSeconds(s);
        Destroy(this.gameObject);
    }
}
