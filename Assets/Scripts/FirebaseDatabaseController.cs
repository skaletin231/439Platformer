/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
public class FirebaseDatabaseController : MonoBehaviour
{
    public static FirebaseDatabaseController instance;
    private DatabaseReference reference;

    private string _deviceID = "";
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
        _deviceID = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Device id: " + _deviceID);
    }

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void addScore(int score)
    {
        this.localScore += score;
    }

    public void writeToDB(float finishTime, string level, bool isMultiPlayer) {
        Debug.Log("write to DB");
        UserRecord data = new UserRecord(this.localScore, finishTime, level);
        string json = JsonUtility.ToJson(data);

        if (isMultiPlayer)
        {
            reference.Child("Leaderboard").Child("MultiPlayer").Child(level).Child(_deviceID).SetRawJsonValueAsync(json);
        }
        else
        {
            reference.Child("Leaderboard").Child("SinglePlayer").Child(level).Child(_deviceID).SetRawJsonValueAsync(json);
        }
    }

    public int localScore = 0;
    
    public void uploadScore(int score)
    {
        localScore = score;
    }

    public void refreshScore()
    {
        
    }
}

public class UserRecord
{
    public int score;
    public float finishTime;
    public UserRecord(int score, float finishTime, string level)
    {
        this.finishTime = finishTime;
        this.score = score;
    }
}
*/