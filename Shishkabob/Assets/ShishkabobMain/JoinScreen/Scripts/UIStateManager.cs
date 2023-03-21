using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIStateManager : MonoBehaviour
{
    public RectTransform joinSection,mapSection;
    public EventSystem eventSystem;
    public GameObject goBackButton,readyButton;

    public void SwitchPositions()
    {
        Vector2 wantedJoinPos = mapSection.transform.localPosition;
        Vector2 wantedMapPos = joinSection.transform.localPosition;

        StartCoroutine(LerpSwap(2f,new Vector2(5000,0),new Vector2(0,0))); //busted right now, but it works
        eventSystem.SetSelectedGameObject(goBackButton);
    }

    public void SwitchPositionsBack()
    {
        Vector2 wantedJoinPos = mapSection.transform.localPosition;
        Vector2 wantedMapPos = joinSection.transform.localPosition;

        StartCoroutine(LerpSwap(2f,new Vector2(0,0),new Vector2(5000,0))); //busted right now, but it works
        eventSystem.SetSelectedGameObject(readyButton);
    }

    IEnumerator LerpSwap(float t,Vector2 wantedJoinPos, Vector2 wantedMapPos)
    {
        float time = 0f;
        Vector2 curJoinPos = joinSection.transform.localPosition;
        Vector2 curMapPos = mapSection.transform.localPosition;
        while(time < t)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;

            joinSection.transform.localPosition = Vector2.Lerp(curJoinPos,wantedJoinPos,time);
            mapSection.transform.localPosition = Vector2.Lerp(curMapPos,wantedMapPos,time);
        }
        joinSection.transform.localPosition =  wantedJoinPos;
        mapSection.transform.localPosition = wantedMapPos;

    }
}
