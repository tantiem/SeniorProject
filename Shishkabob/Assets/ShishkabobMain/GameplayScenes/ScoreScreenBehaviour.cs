using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreScreenBehaviour : MonoBehaviour
{
    public TMPro.TextMeshProUGUI p1Score,p2Score,p3Score,p4Score;
    public TMPro.TextMeshProUGUI header;
    PersistentInteractor interactor;
    private void Awake() 
    {
        interactor= GetComponent<PersistentInteractor>();
    }

    private void Start() {
        StartCoroutine(ComeAndGo(5f));
    }

    IEnumerator ComeAndGo(float f)
    {
        int p1,p2,p3,p4;
        interactor.GetScores(out p1, out p2, out p3, out p4);
        SetScores(p1,p2,p3,p4);
        bool victor = CheckForVictor(p1,p2,p3,p4,3);
        yield return new WaitForSeconds(f);
        if(victor) {
            interactor.GoHome();
            }
        else {interactor.GoToNextMap();}
    }

    void SetScores(int p1,int p2, int p3, int p4)
    {
        p1Score.text = $"{p1}";
        p2Score.text = $"{p2}";
        p3Score.text = $"{p3}";
        p4Score.text = $"{p4}";
        
    }

    bool CheckForVictor(int p1,int p2, int p3, int p4,int winCount)
    {
        if(p1 == winCount)
        {
            header.text = "PLAYER 1 WINS!";
            return true;
        }
        else if(p2 == winCount)
        {
            header.text = "PLAYER 2 WINS!";
            return true;
        }
        else if(p3 == winCount)
        {
            header.text = "PLAYER 3 WINS!";
            return true;
        }
        else if(p4 == winCount)
        {
            header.text = "PLAYER 4 WINS!";
            return true;
        }
        return false;
    }
}
