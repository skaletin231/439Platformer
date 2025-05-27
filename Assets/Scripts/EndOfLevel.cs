using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TimerHolder))]
public class EndOfLevel : MonoBehaviour
{
    [SerializeField] string thisLevel;
    [SerializeField] string nextLevel;

    [SerializeField] TimerScriptable[] timers;
    [SerializeField] SpawnPointTracker[] spawnPointTrackers;

    public bool isMultiPlayer = false;

    public bool isEnded = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnded)
        {
            return;
        }

        isEnded = true;
        if (collision.gameObject.CompareTag("Player"))
        {
            float finishTime = GetComponent<TimerHolder>().GetTime();
            print($"finishTime: {finishTime}");

            if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<NewHeroController>() != null)
            {
                NewHeroController heroController = collision.gameObject.GetComponent<NewHeroController>();
                bool isMultiPlayer = (heroController.view != null);
                if (!isMultiPlayer)
                {
                    //single player
                    //FirebaseDatabaseController.instance.writeToDB(finishTime, thisLevel, false);
                }
                else if (heroController.view.IsMine)
                {
                    //multi player
                    //FirebaseDatabaseController.instance.writeToDB(finishTime, thisLevel, true);
                }
                EndLevel(finishTime, isMultiPlayer);
            }
        }
    }

    public void EndLevel(float finishTime, bool isMultiPlayer)
    {
        /*if (!isMultiPlayer)
        {
            LeaderBoard.instance.showBoard(isMultiPlayer, thisLevel);
            LeaderBoard.instance.showRecord(finishTime, FirebaseDatabaseController.instance.localScore);
        }*/
        
        
        foreach (TimerScriptable timer in timers)
        {
            timer.Reset();
        }
        foreach (SpawnPointTracker tracker in spawnPointTrackers)
        {
            tracker.Reset();
        }

        if (isMultiPlayer)
        {
            SceneManager.LoadScene("MainMenu");
        }
        

         if (nextLevel != null)
             SceneManager.LoadScene(nextLevel);
         else
             SceneManager.LoadScene("MainMenu");
    }

}
