using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerHolder : MonoBehaviour
{
    [SerializeField] TimerScriptable timer;
    [SerializeField] Text timerText;

    // Update is called once per frame
    void Update()
    {
        timer.IncreaseTime(Time.deltaTime);
        int theTime = (int)timer.TimePassedSoFar;
        int seconds = theTime % 60;
        if (seconds >= 10)
            timerText.text = $"Time: {theTime/60}:{seconds}";
        else
            timerText.text = $"Time: {theTime / 60}:0{seconds}";
    }

    public float GetTime() => timer.TimePassedSoFar;
}
