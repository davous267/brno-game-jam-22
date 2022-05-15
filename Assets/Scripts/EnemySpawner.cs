using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private Transform spawnPointsParent;

    private List<Enemy> spawnedEnemies = new List<Enemy>();

    private IEnumerator Start()
    {
        foreach(Transform child in spawnPointsParent)
        {
            var go = Instantiate(enemyPrefab, child.position, Quaternion.identity);
            var enemy = go.GetComponent<Enemy>();

            if(enemy != null)
            {
                RecordNewEnemy(enemy);
            }

            yield return null;
        }
    }

    public void RecordNewEnemy(Enemy enemy)
    {
        spawnedEnemies.Add(enemy);
    }

    public void DeleteEnemyRecord(Enemy enemy)
    {
        spawnedEnemies.Remove(enemy);

        if(spawnedEnemies.Count == 0)
        {
            GameManager.Instance?.GameWon();
        }
    }
}
