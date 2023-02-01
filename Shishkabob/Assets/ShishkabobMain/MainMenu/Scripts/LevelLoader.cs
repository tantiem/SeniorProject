using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public int selectedLevelIndex;

    public void GoToSelected()
    {
        SceneManager.LoadScene(selectedLevelIndex,LoadSceneMode.Single);
    }
}
