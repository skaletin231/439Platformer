using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string firstLevelName;
    [SerializeField] TimerScriptable timer;
    [SerializeField] SpawnPointTracker spawnPointTracker;

    public void StartFirstLevel()
    {
        timer.Reset();
        spawnPointTracker.Reset();
        SceneManager.LoadScene(firstLevelName);
    }

    public void StartMultiPlayer()
    {
        SceneManager.LoadScene("MultiplayerLoading");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
