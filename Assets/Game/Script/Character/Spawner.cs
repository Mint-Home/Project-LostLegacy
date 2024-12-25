using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    List<SpawnPoint> spawnPointsList;

    List<Character> spawnedEnemies;

    bool hasSpawn = false;
    public Collider spawnCollider;

    public UnityEvent OnAllEnemiesDie;

    private void Awake()
    {
        var spawnPointArray = transform.parent.GetComponentsInChildren<SpawnPoint>();
        spawnPointsList = new List<SpawnPoint>(spawnPointArray);
        spawnedEnemies = new List<Character>();
    }

    private void Update()
    {
        if (!hasSpawn || spawnedEnemies.Count == 0)
        {
            return;
        }

        bool allEnemyDie = true;

        foreach(Character enemy in spawnedEnemies)
        {
            if (enemy.currentState != Character.CharacterState.Dead)
            {
                allEnemyDie = false;
                break;
            }
        }
        if (allEnemyDie)
        {
            if(OnAllEnemiesDie != null)
            {
                OnAllEnemiesDie.Invoke();
            }
            spawnedEnemies.Clear();
        }
    }

    public void SpawnCharacter()
    {
        if (hasSpawn)
        {
            return;
        }

        hasSpawn = true;

        foreach (var spawnPoint in spawnPointsList)
        {
            if(spawnPoint.enemyToSpawn != null)
            {
                GameObject enemy = Instantiate(spawnPoint.enemyToSpawn, spawnPoint.transform.position, Quaternion.identity);
                spawnedEnemies.Add(enemy.GetComponent<Character>());
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SpawnCharacter();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnCollider.bounds.size);
    }
}
