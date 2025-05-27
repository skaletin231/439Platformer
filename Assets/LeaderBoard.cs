using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using Firebase;
using Firebase.Database;
using Firebase.Extensions;*/
using UnityEngine.SceneManagement;

public class LeaderBoard : MonoBehaviour
{
    public static LeaderBoard instance;
    
    public GameObject panel;

    public GameObject recordItem;

    public GameObject currentRecord;

    public GameObject content;

    public string nextLevel = "Level2";
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
        //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showBoard(bool isMultiPlayer, string level)
    {
        string path = "";
        if (isMultiPlayer)
        {
            path = "Leaderboard/MultiPlayer/" + level;
        }
        else
        {
            path = "Leaderboard/SinglePlayer/" + level;
        }
        panel.SetActive(true);
        
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        
        /*FirebaseDatabase.DefaultInstance
            .GetReference(path)
            .OrderByChild("finishTime")
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted) {
                    Debug.Log("showBoard Error");
                }
                else if (task.IsCompleted) {
                    DataSnapshot snapshot = task.Result;
                    foreach (var record in snapshot.Children)
                    {
                        addItem(record.Key,  Convert.ToSingle(record.Child("finishTime").Value), Convert.ToSingle(record.Child("score").Value));
                        Debug.Log(record.Key + record.Child("finishTime").Value.ToString() + record.Child("score").Value.ToString());
                    }
                }
            });*/
    }

    public void showRecord(float time, float score)
    {
        string idfv = SystemInfo.deviceUniqueIdentifier;
        currentRecord.GetComponent<RecordItem>().init(idfv, time, score);
    }

    public void addItem(string name, float time, float score)
    {
        GameObject newRecord = Instantiate(recordItem) ;
        newRecord.transform.SetParent(content.transform,false);
        newRecord.GetComponent<RecordItem>().init(name, time, score);
    }

    public void NextLevel()
    {
        if (nextLevel.Equals("Level2"))
        {
            SceneManager.LoadScene("Level2");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}