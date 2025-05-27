using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Timer Object")]

public class TimerScriptable: ScriptableObject
{
    float timePassedSoFar = 0;
    private void OnDisable()
    {
        timePassedSoFar = 0;
    }

    public float TimePassedSoFar => timePassedSoFar;

    public void Reset()
    {
        timePassedSoFar = 0;
    }

    public void IncreaseTime(float time) => timePassedSoFar += time;
}
