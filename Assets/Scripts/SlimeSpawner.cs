using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    [SerializeField] float spawnDelay = .5f;
    [SerializeField] slimeController slimePrefab;
    float timePassedSoFar = 0;

    private void Start()
    {
        print("Spawner Spawned");
    }

    private void Update()
    {
        timePassedSoFar += Time.deltaTime;
        if (timePassedSoFar > spawnDelay)
            SpawnSlime();
    }

    private void SpawnSlime()
    {
        slimeController slime = Instantiate(slimePrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
        GetComponentInParent<TrapRoom>().SetOnDeath(slime);
        /*slime.onDeath += () =>
        {
            //TODO: Find a way to make this tick the slime kill count

            *//*numberOfEnemiesKilled += 1;
            if (numberOfEnemiesKilled >= numberOfEnemiesMax)
            {
                foreach (GameObject gate in theGatesToOpen)
                {
                    gate.SetActive(false);
                }
            }*//*
        };*/
        Destroy(gameObject);
    }
}
