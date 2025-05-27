using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spawn Point Tracker")]
public class SpawnPointTracker : ScriptableObject
{
    Vector3 lastSpawnPoint;
    SpawnPoint lastSpawnPointObject;
    bool useSpawnPoint = false;

    private void OnDisable()
    {
        useSpawnPoint = false;
        lastSpawnPointObject = null;
    }

    public void SetSpawnPoint(SpawnPoint spawnPoint)
    {
        if (lastSpawnPointObject == spawnPoint)
        {
            return;
        }

        if (lastSpawnPointObject != spawnPoint && lastSpawnPointObject != null)
        {
            lastSpawnPointObject.StopAnimation();
        }

        lastSpawnPointObject = spawnPoint;
        lastSpawnPoint = spawnPoint.transform.position + new Vector3(0,.5f,0);
        useSpawnPoint = true;
    }

    public void Reset()
    {
        useSpawnPoint = false;
    }

    public Vector3 LastSpawnPoint => lastSpawnPoint;

    public bool UseSpawnPoint => useSpawnPoint;
}
