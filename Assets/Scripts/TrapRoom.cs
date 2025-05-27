using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapRoom : MonoBehaviour
{
    [SerializeField] GameObject[] theGatesToOpen;
    [SerializeField] int numberOfEnemiesMax;
    int numberOfEnemiesSpawned;
    int numberOfEnemiesKilled = 0;
    [SerializeField] slimeController slimePrefab;
    [SerializeField] SlimeSpawner slimeSpawnerPrefab;
    [SerializeField] GameObject leftPointer;
    [SerializeField] GameObject rightPointer;
    [SerializeField] float timeBetweenSpawns = 2f;

    bool fightHasStarted = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!fightHasStarted)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                print("Fight Start!");
                StartSpawning();
            }
        }
    }

    private void StartSpawning()
    {
        foreach (GameObject gate in theGatesToOpen)
        {
            gate.SetActive(true);
        }
        fightHasStarted = true;
        StartCoroutine(SpawnTimer());
    }

    IEnumerator SpawnTimer()
    {
        numberOfEnemiesSpawned += 1;
        SpawnSlime();
        yield return new WaitForSeconds(timeBetweenSpawns);
        if (numberOfEnemiesSpawned < numberOfEnemiesMax)
        {
            StartCoroutine(SpawnTimer());
        }
    }

    private void SpawnSlime()
    {
        float x = Random.Range(leftPointer.transform.position.x, rightPointer.transform.position.x);
        SlimeSpawner slimeSpawner = Instantiate(slimeSpawnerPrefab, new Vector2(x, leftPointer.transform.position.y), Quaternion.identity,this.transform);
        /*slimeController slime = Instantiate(slimePrefab, new Vector2(x,leftPointer.transform.position.y), Quaternion.identity);
        slime.onDeath += () =>
        {
            numberOfEnemiesKilled += 1;
            if (numberOfEnemiesKilled >= numberOfEnemiesMax)
            {
                foreach (GameObject gate in theGatesToOpen)
                {
                    gate.SetActive(false);
                }
            }
        };*/
    }

    public void SetOnDeath(slimeController slime)
    {
        slime.onDeath += () =>
        {
            numberOfEnemiesKilled += 1;
            if (numberOfEnemiesKilled >= numberOfEnemiesMax)
            {
                foreach (GameObject gate in theGatesToOpen)
                {
                    gate.SetActive(false);
                }
            }
        };
    }

}
