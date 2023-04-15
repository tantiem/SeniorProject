using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneTransition : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator tranisitionAnimator;
    public UnityEvent onFinish;
    
    public void TransitionOut()
    {
        tranisitionAnimator.Play("TransitionOut",0);
    }

    public void OnFinish()
    {
        onFinish?.Invoke();
    }
}
