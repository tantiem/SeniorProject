using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapLoader : MonoBehaviour
{
    //list of maps
    //randomize the order on 'init'
    //every time load next map is called, get next scene
    //every map should be self sufficient, so that all this has to do is load the next map

    public List<int> indexes;
    LevelLoader loader;

[SerializeField]
    int[] randomizedOrder;
    int curMapIndex;

    private void Start() 
    {
        loader = GetComponent<LevelLoader>();
        randomizedOrder = new int[indexes.Count]; 
        randomizedOrder = indexes.ToArray();  
        curMapIndex = 0;
        RandomizeOrder(); 

    }

    public void RandomizeOrder()
    {
        for(int i = 0; i < 20; i++)
        {
            int randIndexA = Random.Range(0,indexes.Count);
            int randIndexB = Random.Range(0,indexes.Count);
            SwapIndexes(randIndexA,randIndexB);
        }
    }
    void SwapIndexes(int a, int b)
    {
        int temp = randomizedOrder[a];
        randomizedOrder[a] = randomizedOrder[b];
        randomizedOrder[b] = temp;
    }
    public void TransitionToNextMap()
    {
        loader.SetIndex(randomizedOrder[curMapIndex]);
        curMapIndex++;
        if(curMapIndex > randomizedOrder.Length-1)
        {
            curMapIndex = 0;
        }
        FinishTransition();
    }
    void FinishTransition()
    {
        loader.GoToSelected();
    }
}
