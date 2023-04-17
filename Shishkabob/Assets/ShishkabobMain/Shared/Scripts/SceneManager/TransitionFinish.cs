using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransitionFinish : MonoBehaviour
{
    public UnityEvent onTransitionFinish;

    public void OnFinish()
    {
        onTransitionFinish?.Invoke();
    }
}
