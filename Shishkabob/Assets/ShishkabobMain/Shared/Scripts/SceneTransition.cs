using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator tranisitionAnimator;
    
    public void TransitionOut()
    {
        tranisitionAnimator.Play("TransitionIn",0);
    }

    public void OnFinish()
    {
        
    }
}
